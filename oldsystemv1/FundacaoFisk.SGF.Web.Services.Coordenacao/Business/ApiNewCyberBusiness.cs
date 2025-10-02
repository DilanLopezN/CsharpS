using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.GenericModel.Partial;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IDataAccess;
using Newtonsoft.Json;

namespace FundacaoFisk.SGF.Services.Coordenacao.Business
{
    public class ApiNewCyberBusiness : IApiNewCyberBusiness
    {
        public enum TipoComandos
        {
            CADASTRA_ALUNO = 1,
            CADASTRA_PROFESSOR = 2,
            CADASTRA_COLABORADOR = 3,
            CADASTRA_COORDENADOR = 4,
            ATUALIZA_ALUNO = 5,
            ATUALIZA_PROFESSOR = 6,
            ATUALIZA_COLABORADOR = 7,
            ATUALIZA_COORDENADOR = 8,
            ATIVA_ALUNO = 9,
            ATIVA_PROFESSOR = 10,
            ATIVA_COLABORADOR = 11,
            ATIVA_COORDENADOR = 12,
            INATIVA_ALUNO = 13,
            INATIVA_PROFESSOR = 14,
            INATIVA_COLABORADOR = 15,
            INATIVA_COORDENADOR = 16,
            ATIVA_INATIVACAO_ALUNO = 17,
            ATIVA_INATIVACAO_PROFESSOR = 18,
            ATIVA_INATIVACAO_COORDENADOR = 19,
            ATIVA_INATIVACAO_COLABORADOR = 20,
            CADASTRA_GRUPO = 21,
            ATUALIZA_GRUPO = 22,
            ATIVA_GRUPO = 23,
            INATIVA_GRUPO = 24,
            CADASTRA_LIVROALUNO = 25,
            ATUALIZA_LIVROALUNO = 26,
            DELETA_LIVROALUNO = 27,
            CADASTRA_UNIDADE = 28,
            ATUALIZA_UNIDADE = 29,
            ATIVA_UNIDADE = 30,
            INATIVA_UNIDADE = 31
        }

        public ITurmaDataAccess TurmaDataAccess { get; set; }
        private IEmpresaDataAccess EmpresaDataAccess { get; set; }

        public ApiNewCyberBusiness(ITurmaDataAccess turmaDataAccess, IEmpresaDataAccess empresaDataAccess)
        {
            if (turmaDataAccess == null || empresaDataAccess == null)
                throw new ArgumentNullException("ApiNewCyberBusiness");
            TurmaDataAccess = turmaDataAccess;
            EmpresaDataAccess = empresaDataAccess;
        }

        public void configuraUsuario(int codUsuario, int cd_empresa)
        {
            ((SGFWebContext) this.TurmaDataAccess.DB()).IdUsuario = codUsuario;
            ((SGFWebContext) this.TurmaDataAccess.DB()).cd_empresa = cd_empresa;
        }

        public void sincronizarContextos(DbContext dbContext)
        {
            //this.DaoListaEnderecoMala.sincronizaContexto(dbContext);
            //this.DaoListaNaoInscrito.sincronizaContexto(dbContext);
            //BusinessLogGeral.sincronizaContexto(dbContext);
        }

        public string postExecutaCyber(string url, string comando, string chave, string parametros)
        {
            //Se 0 no webConfig não aplica o cyber
            if (!aplicaApiCyber()) return null;

            var parameters = new Dictionary<string, string>();
            string resposta_cyber = "";

            string []parametrosArray  = parametros.Split(',');

            //valida parametros
            foreach (var parametro in parametrosArray)
            {
                string[] para = parametro.Split('=');

                if (!String.IsNullOrEmpty(para[0]) && !String.IsNullOrEmpty(para[1]))
                {
                    if (para[0] == "codigo_professor")
                    {
                        parameters[para[0].ToString()] = para[1].Replace("|", ",");
                        
                    }
                    else
                    {
                        parameters[para[0].ToString()] = para[1].ToString();
                    }
                   
                }
                else
                {
                    throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroParametrosExecutaCyber, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_PARAMETROS_EXECUTA_CYBER, false);
                }

            }

            //valida comando
            var httpClient = new HttpClient();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            if (!String.IsNullOrEmpty(comando))
            {
                parameters["comando"] = comando;
            }
            else
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroComandoExecutaCyber, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_COMANDO_EXECUTA_CYBER, false);
            }

            //valida chave
            if (!String.IsNullOrEmpty(chave))
            {
                parameters["chave"] = chave;
            }
            else
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroChaveExecutaCyber, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_CHAVE_EXECUTA_CYBER, false);
            }
            
            //faz a requisição
            var response =  httpClient.PostAsync(url, new FormUrlEncodedContent(parameters)).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Request Message Information:- \n\n" + response.RequestMessage + "\n");
                Console.WriteLine("Response Message Header \n\n" + response.Content.Headers + "\n");
               
                // Pega o valor da resposta da requisição
                var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult(); ;
                Console.WriteLine("O valor da resposta é: " + responseContent);

                resposta_cyber = responseContent.Split('{')[1].Split('}')[0].Trim();

                //se a resposta_cyber não for nulo e for diferente de (OK)
                if (!String.IsNullOrEmpty(resposta_cyber) && !resposta_cyber.Equals("OK"))
                {
                    if (resposta_cyber.Length > 274)
                    {
                        resposta_cyber = resposta_cyber.Substring(0, 274);
                    }
                    throw new ApiNewCyberException(string.Format("Erro Api Cyber -> " + resposta_cyber + "(statusErro: Erro Api newCyber - Url {0} - Comando: {1} - Parametros - {2})", url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_SEND_EXECUTA_CYBER, false);
                }
                else if (String.IsNullOrEmpty(resposta_cyber))
                {
                    throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroSendExecutaCyber, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_SEND_EXECUTA_CYBER, false);
                }
               
            }
            else
            {
                
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroSendExecutaCyber, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_SEND_EXECUTA_CYBER, false);
            }

           var contents = resposta_cyber;


            return contents;
        }

        public bool verificaRegistro(string url, string comando, string chave, int codigo)
        {
            //Se 0 no webConfig não aplica o cyber
            if (!aplicaApiCyber()) return false;

            var parameters = new Dictionary<string, string>();
            string resposta_cyber = "";

            //valida comando
            var httpClient = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            if (!String.IsNullOrEmpty(comando))
            {
                parameters["comando"] = comando;
            }
            else
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroComandoExecutaCyber, url, comando, codigo), null, ApiNewCyberException.TipoErro.ERRO_COMANDO_EXECUTA_CYBER, false);
            }

            //valida chave
            if (!String.IsNullOrEmpty(chave))
            {
                parameters["chave"] = chave;
            }
            else
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroChaveExecutaCyber, url, comando, codigo), null, ApiNewCyberException.TipoErro.ERRO_CHAVE_EXECUTA_CYBER, false);
            }

            //faz a requisição
            var response = httpClient.GetAsync(url + String.Format("?comando={0}&chave={1}&codigo={2}", comando, chave, codigo)).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Request Message Information:- \n\n" + response.RequestMessage + "\n");
                Console.WriteLine("Response Message Header \n\n" + response.Content.Headers + "\n");

                //Pega a resposta da requisição
                var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult(); ;
                Console.WriteLine("O valor da resposta é: " + responseContent);

                resposta_cyber = responseContent.Split('{')[1].Split('}')[0].Trim();
                string[] stringSeparators = new string[] { "<br>" };
                string[] resposta_cyber_aux = resposta_cyber.Split(stringSeparators, StringSplitOptions.None);

                Console.WriteLine(resposta_cyber_aux[2]);
                //se exite registro
                if (!String.IsNullOrEmpty(resposta_cyber_aux[2]))
                {
                    return true;
                }

                return false;
            }
            else
            {

                return false;
            }
        }

        public bool verificaRegistroGrupos(string url, string comando, string chave, int codigo_grupo)
        {
            //Se 0 no webConfig não aplica o cyber
            if (!aplicaApiCyber()) return false;

            var parameters = new Dictionary<string, string>();
            string resposta_cyber = "";

            //valida comando
            var httpClient = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            if (!String.IsNullOrEmpty(comando))
            {
                parameters["comando"] = comando;
            }
            else
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroComandoExecutaCyber, url, comando, codigo_grupo), null, ApiNewCyberException.TipoErro.ERRO_COMANDO_EXECUTA_CYBER, false);
            }

            //valida chave
            if (!String.IsNullOrEmpty(chave))
            {
                parameters["chave"] = chave;
            }
            else
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroChaveExecutaCyber, url, comando, codigo_grupo), null, ApiNewCyberException.TipoErro.ERRO_CHAVE_EXECUTA_CYBER, false);
            }

            //faz a requisição
            var response = httpClient.GetAsync(url + String.Format("?comando={0}&chave={1}&codigo_grupo={2}", comando, chave, codigo_grupo)).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Request Message Information:- \n\n" + response.RequestMessage + "\n");
                Console.WriteLine("Response Message Header \n\n" + response.Content.Headers + "\n");

                //Pega a resposta da requisição
                var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult(); ;
                Console.WriteLine("O valor da resposta é: " + responseContent);

                resposta_cyber = responseContent.Split('{')[1].Split('}')[0].Trim();
                string[] stringSeparators = new string[] { "<br>" };
                string[] resposta_cyber_aux = resposta_cyber.Split(stringSeparators, StringSplitOptions.None);

                Console.WriteLine(resposta_cyber_aux[2]);
                //se exite registro
                if (!String.IsNullOrEmpty(resposta_cyber_aux[2]))
                {
                    return true;
                }

                return false;
            }
            else
            {

                return false;
            }
        }

        public bool verificaRegistroLivroAlunos(string url, string comando, string chave, int codigo_aluno, int codigo_grupo, int codigo_livro)
        {
            //Se 0 no webConfig não aplica o cyber
            if (!aplicaApiCyber()) return false;

            var parameters = new Dictionary<string, string>();
            string resposta_cyber = "";

            //valida comando
            var httpClient = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            if (!String.IsNullOrEmpty(comando))
            {
                parameters["comando"] = comando;
            }
            else
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroComandoExecutaCyber, url, comando, codigo_aluno), null, ApiNewCyberException.TipoErro.ERRO_COMANDO_EXECUTA_CYBER, false);
            }

            //valida chave
            if (!String.IsNullOrEmpty(chave))
            {
                parameters["chave"] = chave;
            }
            else
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroChaveExecutaCyber, url, comando, codigo_aluno), null, ApiNewCyberException.TipoErro.ERRO_CHAVE_EXECUTA_CYBER, false);
            }

            //faz a requisição
            var response = httpClient.GetAsync(url + String.Format("?comando={0}&codigo_aluno={1}&chave={2}", comando, codigo_aluno, chave)).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Request Message Information:- \n\n" + response.RequestMessage + "\n");
                Console.WriteLine("Response Message Header \n\n" + response.Content.Headers + "\n");

                //Pega a resposta da requisição
                var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult(); ;
                Console.WriteLine("O valor da resposta é: " + responseContent);

                resposta_cyber = responseContent.Split('{')[1].Split('}')[0].Trim();

                string[] stringSeparators = new string[] { "<br>" };
                string[] resposta_cyber_aux = resposta_cyber.Split(stringSeparators, StringSplitOptions.None);

                if (resposta_cyber_aux.Length > 0)
                {
                    for (int i = 1; i < resposta_cyber_aux.Length; i++)
                    {
                        //se a linha não for vazia. ex: 14468 | 54
                        if (!String.IsNullOrEmpty(resposta_cyber_aux[i]))
                        {
                            //tira os espaços e realiza um split pelo delimitador (|)
                            string[] dados_turma_cyber = resposta_cyber_aux[i].Replace(" ", "").Split('|');

                            //Se o codigo do grupo e o codigo do grupo for igual ao retorno do cyber
                            if (!String.IsNullOrEmpty(dados_turma_cyber[0]) && !String.IsNullOrEmpty(dados_turma_cyber[1]) && 
                                dados_turma_cyber[0].Equals(codigo_grupo.ToString()) &&
                                dados_turma_cyber[1].Equals(codigo_livro.ToString()))
                            {
                                return true;
                            }
                        }

                    }

                    return false;

                }

                return false;
            }
            else
            {

                return false;
            }
        }

        public EscolaApiCyberBdUI getEscola(int cd_escola)
        {
            return EmpresaDataAccess.getEscola(cd_escola);
        }

        public PessoaCoordenadorCyberBdUI findPessoaCoordenadorCyberByCdPessoa(int cd_pessoa, int cd_empresa)
        {
            return EmpresaDataAccess.findPessoaCoordenadorCyberByCdPessoa(cd_pessoa, cd_empresa);
        }

        public List<RelacionamentoSGF> findRelacionamentosCoordenadorByEmpresa(int cd_empresa)
        {
            return EmpresaDataAccess.findRelacionamentosCoordenadorByEmpresa(cd_empresa).ToList();
        }

        public bool verificaRegistroFuncionario(string url, string comando, string chave, string codigo)
        {
            //Se 0 no webConfig não aplica o cyber
            if (!aplicaApiCyber()) return false;

            var parameters = new Dictionary<string, string>();
            string resposta_cyber = "";
            bool retorno = false;

            //valida comando
            var httpClient = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            if (!String.IsNullOrEmpty(comando))
            {
                parameters["comando"] = comando;
            }
            else
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroComandoExecutaCyber, url, comando, codigo), null, ApiNewCyberException.TipoErro.ERRO_COMANDO_EXECUTA_CYBER, false);
            }

            //valida chave
            if (!String.IsNullOrEmpty(chave))
            {
                parameters["chave"] = chave;
            }
            else
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroChaveExecutaCyber, url, comando, codigo), null, ApiNewCyberException.TipoErro.ERRO_CHAVE_EXECUTA_CYBER, false);
            }

            //faz a requisição
            var response = httpClient.GetAsync(url + String.Format("?comando={0}&chave={1}&codigo={2}", comando, chave, codigo)).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Request Message Information:- \n\n" + response.RequestMessage + "\n");
                Console.WriteLine("Response Message Header \n\n" + response.Content.Headers + "\n");

                //Pega a resposta da requisição
                var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult(); ;
                Console.WriteLine("O valor da resposta é: " + responseContent);

                resposta_cyber = responseContent.Split('{')[1].Split('}')[0].Trim();

                string[] stringSeparators = new string[] { "<br>" };
                string[] resposta_cyber_aux = resposta_cyber.Split(stringSeparators, StringSplitOptions.None);

                Console.WriteLine(resposta_cyber_aux[2]);
                //se exite registro
                if (!String.IsNullOrEmpty(resposta_cyber_aux[2]))
                {
                    return true;
                }

                retorno = false;
            }
            else
            {

                retorno = false;
            }


            return retorno;
        }

        public bool aplicaApiCyber()
        {
            string aplica = ConfigurationManager.AppSettings["aplicarApiNewCyber"];
            if (string.IsNullOrEmpty(aplica))
            {
                return false;
            }

            return Convert.ToBoolean(Convert.ToInt16(aplica));
        }


    }
}
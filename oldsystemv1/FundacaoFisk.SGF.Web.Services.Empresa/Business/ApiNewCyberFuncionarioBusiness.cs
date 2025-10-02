using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IBusiness;

namespace FundacaoFisk.SGF.Web.Services.Empresa.Business
{
    public class ApiNewCyberFuncionarioBusiness : IApiNewCyberFuncionarioBusiness
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

        public ApiNewCyberFuncionarioBusiness()
        {

        }

        public void configuraUsuario(int codUsuario, int cd_empresa)
        {
            //throw new System.NotImplementedException();
        }

        public string postExecutaCyber(string url, string comando, string chave, string parametros)
        {
            //Se 0 no webConfig não aplica o cyber
            if (!aplicaApiCyber()) return null;

            var parameters = new Dictionary<string, string>();
            string resposta_cyber = "";

            string[] parametrosArray = parametros.Split(',');

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
                    throw new ApiNewCyberFuncionarioException(string.Format(Utils.Messages.Messages.ErroParametrosExecutaCyber, url, comando, parametros), null, ApiNewCyberFuncionarioException.TipoErro.ERRO_PARAMETROS_EXECUTA_CYBER, false);
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
                throw new ApiNewCyberFuncionarioException(string.Format(Utils.Messages.Messages.ErroComandoExecutaCyber, url, comando, parametros), null, ApiNewCyberFuncionarioException.TipoErro.ERRO_COMANDO_EXECUTA_CYBER, false);
            }

            //valida chave
            if (!String.IsNullOrEmpty(chave))
            {
                parameters["chave"] = chave;
            }
            else
            {
                throw new ApiNewCyberFuncionarioException(string.Format(Utils.Messages.Messages.ErroChaveExecutaCyber, url, comando, parametros), null, ApiNewCyberFuncionarioException.TipoErro.ERRO_CHAVE_EXECUTA_CYBER, false);
            }

            //faz a requisição
            var response = httpClient.PostAsync(url, new FormUrlEncodedContent(parameters)).GetAwaiter().GetResult();
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
                    throw new ApiNewCyberFuncionarioException(string.Format("Erro Api Cyber -> " + resposta_cyber + "(statusErro: Erro Api newCyber - Url {0} - Comando: {1} - Parametros - {2})", url, comando, parametros), null, ApiNewCyberFuncionarioException.TipoErro.ERRO_SEND_EXECUTA_CYBER, false);
                }
                else if (String.IsNullOrEmpty(resposta_cyber))
                {
                    throw new ApiNewCyberFuncionarioException(string.Format(Utils.Messages.Messages.ErroSendExecutaCyber, url, comando, parametros), null, ApiNewCyberFuncionarioException.TipoErro.ERRO_SEND_EXECUTA_CYBER, false);
                }

            }
            else
            {

                throw new ApiNewCyberFuncionarioException(string.Format(Utils.Messages.Messages.ErroSendExecutaCyber, url, comando, parametros), null, ApiNewCyberFuncionarioException.TipoErro.ERRO_SEND_EXECUTA_CYBER, false);
            }

            var contents = resposta_cyber;


            return contents;
        }

        public bool verificaRegistro(string url, string comando, string chave, int codigo)
        {var parameters = new Dictionary<string, string>();
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
                throw new ApiNewCyberFuncionarioException(string.Format(Utils.Messages.Messages.ErroComandoExecutaCyber, url, comando, codigo), null, ApiNewCyberFuncionarioException.TipoErro.ERRO_COMANDO_EXECUTA_CYBER, false);
            }

            //valida chave
            if (!String.IsNullOrEmpty(chave))
            {
                parameters["chave"] = chave;
            }
            else
            {
                throw new ApiNewCyberFuncionarioException(string.Format(Utils.Messages.Messages.ErroChaveExecutaCyber, url, comando, codigo), null, ApiNewCyberFuncionarioException.TipoErro.ERRO_CHAVE_EXECUTA_CYBER, false);
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
                throw new ApiNewCyberFuncionarioException(string.Format(Utils.Messages.Messages.ErroComandoExecutaCyber, url, comando, codigo), null, ApiNewCyberFuncionarioException.TipoErro.ERRO_COMANDO_EXECUTA_CYBER, false);
            }

            //valida chave
            if (!String.IsNullOrEmpty(chave))
            {
                parameters["chave"] = chave;
            }
            else
            {
                throw new ApiNewCyberFuncionarioException(string.Format(Utils.Messages.Messages.ErroChaveExecutaCyber, url, comando, codigo), null, ApiNewCyberFuncionarioException.TipoErro.ERRO_CHAVE_EXECUTA_CYBER, false);
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
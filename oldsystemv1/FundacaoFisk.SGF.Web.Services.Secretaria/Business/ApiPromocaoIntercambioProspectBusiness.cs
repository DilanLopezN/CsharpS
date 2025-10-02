using System;
using System.Configuration;
using System.Data.Entity;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.GenericModel.Partial;
using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum;
using Newtonsoft.Json;

namespace FundacaoFisk.SGF.Services.Coordenacao.Business
{
    public class ApiPromocaoIntercambioProspectBusiness: IApiPromocaoIntercambioProspectBussiness
    {


        public ApiPromocaoIntercambioProspectBusiness()
        {
            
        }

        public void configuraUsuario(int codUsuario, int cd_empresa)
        {
        }

        public void sincronizarContextos(DbContext dbContext)
        {
            //this.DaoListaEnderecoMala.sincronizaContexto(dbContext);
            //this.DaoListaNaoInscrito.sincronizaContexto(dbContext);
            //BusinessLogGeral.sincronizaContexto(dbContext);
        }

        public string postExecutaRequestPromocaoIntercambio(PromocaoIntercambioParams parametros)
        {
            //Se 0 no webConfig não aplica a promocao
            if (!aplicaApiPromocao()) return null;



            StringBuilder parametrosMsgError = new StringBuilder();
            parametrosMsgError
                .Append("CPF:").Append(parametros.cpf)
                .Append("|EMAIL:").Append(parametros.email)
                .Append("|TELEFONE:").Append(parametros.telefone)
                .Append("|TIPO:").Append(parametros.tipo)
                .Append("|UNIDADE:").Append(parametros.unidade);

            //valida comando
            var httpClient = new HttpClient();



            string url = ConfigurationManager.AppSettings["urlApiPromocaoIntercambio"];

            if (String.IsNullOrEmpty(url))
            {


                throw new ApiPromocaoIntercambioProspectException(string.Format(Utils.Messages.Messages.msgUrlPromocaoIntercambioNuloVazio, url, parametrosMsgError.ToString()), null, ApiPromocaoIntercambioProspectException.TipoErro.URL_NULO_VAZIO, false);
            }

            string bearerToken = ConfigurationManager.AppSettings["bearerTokenApiPromocaoIntercambio"];

            if (String.IsNullOrEmpty(bearerToken))
            {
                

                throw new ApiPromocaoIntercambioProspectException(string.Format(Utils.Messages.Messages.msgBearerPromocaoIntercambioNuloVazio, url, parametrosMsgError.ToString()), null, ApiPromocaoIntercambioProspectException.TipoErro.BEARER_NULO_VAZIO, false);
            }

            

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", bearerToken);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;



            string resposta_promocao_intercambio = "";

            //faz a requisição
            var jsonContent = JsonConvert.SerializeObject(parametros);
            var contentString = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = httpClient.PostAsync(url, contentString).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
            {
                
                Console.WriteLine("Request Message Information:- \n\n" + response.RequestMessage + "\n");
                Console.WriteLine("Response Message Header \n\n" + response.Content.Headers + "\n");

                // Pega o valor da resposta da requisição
                string responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult(); ;
                Console.WriteLine("O valor da resposta é: " + responseContent);

                PromocaoIntercambioResponse objResponseContent = (PromocaoIntercambioResponse)JsonConvert.DeserializeObject<PromocaoIntercambioResponse>(responseContent);

                if (objResponseContent != null && objResponseContent.status == "error")
                {
                    throw new ApiPromocaoIntercambioProspectException(string.Format(Utils.Messages.Messages.msgErrorRequestApiPromocaoIntercambio, objResponseContent.error, objResponseContent.error_description, url, parametrosMsgError.ToString()), null, ApiPromocaoIntercambioProspectException.TipoErro.ERROR_REQUEST, false);
                }else if ((objResponseContent != null && objResponseContent.status != "error"))
                {
                    if (!string.IsNullOrEmpty(objResponseContent.promotional_code))
                    {
                        resposta_promocao_intercambio = objResponseContent.promotional_code;
                    }
                    else
                    {
                        resposta_promocao_intercambio = null;
                    }
                    
                }else if ((objResponseContent == null))
                {
                    resposta_promocao_intercambio = null;
                }
               

            }
            else
            {

                throw new ApiPromocaoIntercambioProspectException(string.Format(Utils.Messages.Messages.msgErroStatusCodeRequestApiPromocaoIntercambio, "IsSuccessStatusCode -> false", url, parametros), null, ApiPromocaoIntercambioProspectException.TipoErro.ERROR_REQUEST, false);
            }

            var contents = resposta_promocao_intercambio;


            return contents;
        }

        public bool aplicaApiPromocao()
        {
            string aplica = ConfigurationManager.AppSettings["aplicarApiPromocaoIntercambio"];
            if (string.IsNullOrEmpty(aplica))
            {
                return false;
            }

            return Convert.ToBoolean(Convert.ToInt16(aplica));
        }

        public void ValidaParametros(PromocaoIntercambioParams parametros)
        {
            


            if (parametros == null)
            {
                throw new ApiPromocaoIntercambioProspectException(string.Format(Utils.Messages.Messages.msgErroApiPromocaoIntercambioParametros, "Parametros Nulos"), null, ApiPromocaoIntercambioProspectException.TipoErro.ERRO_PARAMETROS, false);
            }

            /*if (String.IsNullOrEmpty(parametros.cpf))
            {
                throw new ApiPromocaoIntercambioProspectException(string.Format(Utils.Messages.Messages.msgErroApiPromocaoIntercambioParametros, "CPF Nulo ou Vazio"), null, ApiPromocaoIntercambioProspectException.TipoErro.ERRO_PARAMETROS, false);
            }*/

           
            if (String.IsNullOrEmpty(parametros.email))
            {
                throw new ApiPromocaoIntercambioProspectException(string.Format(Utils.Messages.Messages.msgErroApiPromocaoIntercambioParametros, "EMAIL Nulo ou Vazio"), null, ApiPromocaoIntercambioProspectException.TipoErro.ERRO_PARAMETROS, false);
            }
            

            //Valida Tipo
            if (String.IsNullOrEmpty(parametros.tipo))
            {
                throw new ApiPromocaoIntercambioProspectException(string.Format(Utils.Messages.Messages.msgErroApiPromocaoIntercambioParametros, "TIPO Nulo ou Vazio"), null, ApiPromocaoIntercambioProspectException.TipoErro.ERRO_PARAMETROS, false);
            }

            //Valida Unidade
            if (String.IsNullOrEmpty(parametros.unidade))
            {
                throw new ApiPromocaoIntercambioProspectException(string.Format(Utils.Messages.Messages.msgErroApiPromocaoIntercambioParametros, "UNIDADE Nulo ou Vazio"), null, ApiPromocaoIntercambioProspectException.TipoErro.ERRO_PARAMETROS, false);
            }
            
        }
    }
}
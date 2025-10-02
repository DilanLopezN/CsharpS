using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Componentes.GenericBusiness;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.IO;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.Business {
    public class PessoaBusinessException : BusinessException{
        public enum TipoErro {
            ERRO_CPFJAEXISTENTE = 2,
            ERRO_CNPJJAEXISTENTE = 3,
            ERRO_EMAILJAEXISTENTE = 4,
            ERRO_ESTADO_NAO_CADASTRADO_CEP = 5,
            ERRO_TELEFONEJAEXISTENTE = 6,
            ERRO_CONVERSAO_DATA = 7,
            ERRO_DATA_FORA_INTERVALO = 8,
            ERRO_AUTO_RELACIONAMENTO = 9,
            ERRO_MINDATE_SMALLDATETIME = 10,
            ERRO_CONVERSAO_ENDERECO = 11,
            ERRO_NAO_EXISTE_IMAGEM = 12,
            ERRO_PAPEL_RELACIONAMENTO = 13
        }
        public TipoErro tipoErro;
        private string mensagem = String.Empty;
            
        public override string Message
        {
            get
            {
                return base.Message + mensagem;
            }
        }
        public PessoaBusinessException(String msg, Exception ex, TipoErro erro, bool mostraStackTrace)
            : base(msg, ex, mostraStackTrace)
        {
            tipoErro = erro;
        }

        [Obsolete]
        public PessoaBusinessException(String msg, Exception ex, TipoErro tipoErro, bool mostraStackTrace, RelacionamentoSGF relacionamento)
            : base(msg, ex, mostraStackTrace)
        {
            this.tipoErro = tipoErro;
            switch ((byte)tipoErro)
            {
                case (byte)TipoErro.ERRO_PAPEL_RELACIONAMENTO:
                        mensagem = JsonConvert.SerializeObjectAsync(relacionamento).Result;
                    break;
            }
        }

        [Obsolete]
        public PessoaBusinessException(String msg, Exception ex, TipoErro tipoErro, bool mostraStackTrace, int cd_logradouro, string nm_cep)
            : base(msg, ex, mostraStackTrace)
        {
            this.tipoErro = tipoErro;
            switch ((byte)tipoErro)
            {
                case (byte)TipoErro.ERRO_CONVERSAO_ENDERECO:
                    mensagem = JsonConvert.SerializeObjectAsync("cd_logradouro:" + cd_logradouro + " , nm_cep:" + nm_cep).Result;
                    break;
            }
        }
    }
}

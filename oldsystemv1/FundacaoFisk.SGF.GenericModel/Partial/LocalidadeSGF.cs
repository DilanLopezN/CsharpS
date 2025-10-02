using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    using Componentes.GenericModel;
    public partial class LocalidadeSGF
    {
        public DadosNF DadosNFCidade { get; set; }

        public int? cd_localidade_bairro { get; set; }
        public int? cd_localidade_cidade { get; set; }
        public int? cd_localidade_estado { get; set; }
        public int? cd_localidade_rua { get; set; }
        public string no_localidade_bairro { get; set; }
        public string no_localidade_cidade { get; set; }
        public string no_localidade_estado { get; set; }
        public string no_tipo_logradouro { get; set; }
        public List<LocalidadeSGF> localidades { get; set; }
        public string nm_cep { get; set; }
        public enum TipoLocalidadeSGF
        {
            CIDADE = 3,
            BAIRRO = 4,
            DISTRITO = 5,
            ESTADO = 2,
            LOGRADOURO = 6,
            PAIS = 1
        }

        public static LocalidadeSGF ChangeValuesLogradouro(LocalidadeSGF locContext, LocalidadeSGF locView)
        {
            locContext.no_localidade = locView.no_localidade;
            locContext.cd_loc_relacionada = locView.cd_loc_relacionada;
            locContext.dc_num_cep = locView.dc_num_cep;
            return locContext;
        }

        public static LocalidadeSGF converterLocalidade(LogradouroCEP logradouro)
        {
            LocalidadeSGF l = new LocalidadeSGF();
            l.no_localidade = logradouro.Logradouro;
            l.no_localidade_bairro = logradouro.Bairro;
            l.no_localidade_cidade = logradouro.Cidade;
            l.no_localidade_estado = logradouro.Uf;
            l.dc_num_cep = logradouro.CEP;
            l.no_tipo_logradouro = logradouro.TipoLog;
            return l;
        }

        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();
                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                // retorno.Add(new DefinicaoRelatorio("cd_localidade", "Código"));
                switch (cd_tipo_localidade)
                {
                    case (byte)TipoLocalidadeSGF.BAIRRO:
                        retorno.Add(new DefinicaoRelatorio("no_localidade", "Bairro", AlinhamentoColuna.Left, "3.8000in"));
                        retorno.Add(new DefinicaoRelatorio("no_localidade_cidade", "Cidade", AlinhamentoColuna.Left, "3.8000in"));
                        break;
                    case (byte)TipoLocalidadeSGF.DISTRITO:
                        retorno.Add(new DefinicaoRelatorio("no_localidade", "Distrito", AlinhamentoColuna.Left, "3.8000in"));
                        retorno.Add(new DefinicaoRelatorio("no_localidade_cidade", "Cidade", AlinhamentoColuna.Left, "3.8000in"));
                        break;
                    case (byte)TipoLocalidadeSGF.LOGRADOURO:
                        retorno.Add(new DefinicaoRelatorio("dc_num_cep", "CEP", AlinhamentoColuna.Center, "1.3000in"));
                        retorno.Add(new DefinicaoRelatorio("no_localidade", "Logradouro", AlinhamentoColuna.Left, "3.0000in"));
                        retorno.Add(new DefinicaoRelatorio("no_localidade_bairro", "Bairro", AlinhamentoColuna.Left, "2.8000in"));
                        retorno.Add(new DefinicaoRelatorio("no_localidade_cidade", "Cidade", AlinhamentoColuna.Left, "2.0000in"));
                        break;
                }

                return retorno;
            }
        }
    }
}

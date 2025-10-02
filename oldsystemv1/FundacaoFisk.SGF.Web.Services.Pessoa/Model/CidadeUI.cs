using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.Model
{
    using Componentes.GenericModel;
    public class CidadeUI : TO
    {
        public int cd_localidade { get; set; }
        public byte cd_tipo_localidade { get; set; }
        public String no_localidade { get; set; }
        public int? cd_loc_relacionada { get; set; }
        public int? nm_municipio { get; set; }
        public String sg_estado { get; set; }

        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();
                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_localidade", "Código"));
                retorno.Add(new DefinicaoRelatorio("no_localidade", "Cidade", AlinhamentoColuna.Left, "3.8000in"));
                retorno.Add(new DefinicaoRelatorio("nm_municipio", "Nr. Município", AlinhamentoColuna.Right, "1.3000in"));
                retorno.Add(new DefinicaoRelatorio("sg_estado", "Estado", AlinhamentoColuna.Center));                
                return retorno;
            }

        }
        public static CidadeUI fromCidade(LocalidadeSGF localidade, String sg_estado)
        {
            CidadeUI cidadeUI = new CidadeUI();
            cidadeUI.cd_localidade = localidade.cd_localidade;
            cidadeUI.cd_loc_relacionada = localidade.cd_loc_relacionada;
            cidadeUI.cd_tipo_localidade = localidade.cd_tipo_localidade;
            cidadeUI.sg_estado = sg_estado;
            cidadeUI.no_localidade = localidade.no_localidade;
            return cidadeUI;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.Model
{
    using Componentes.GenericModel;
    public class EstadoUI : TO
    {
        public int cd_localidade { get; set; }
        public byte cd_tipo_localidade { get; set; }
        public String no_localidade { get; set; }
        public int? cd_loc_relacionada { get; set; }
        public String sg_estado { get; set; }
        public String no_pais { get; set; }
        public int cd_localidade_estado { get; set; }

        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();
                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_localidade", "Código"));
                retorno.Add(new DefinicaoRelatorio("no_localidade", "Estado", AlinhamentoColuna.Left, "3.8000in"));
                retorno.Add(new DefinicaoRelatorio("sg_estado", "Sigla"));
                retorno.Add(new DefinicaoRelatorio("no_pais", "País"));
                return retorno;
            }

        }
        public static EstadoUI fromEstado(LocalidadeSGF localidade, String sg_estado, String no_pais)
        {
            EstadoUI estadoUI = new EstadoUI();
            estadoUI.cd_localidade = localidade.cd_localidade;
            estadoUI.cd_localidade_estado = localidade.cd_localidade;
            estadoUI.cd_tipo_localidade = localidade.cd_tipo_localidade;
            estadoUI.cd_loc_relacionada = localidade.cd_loc_relacionada;
            estadoUI.sg_estado = sg_estado;
            estadoUI.no_localidade = localidade.no_localidade;
            estadoUI.no_pais = no_pais;
            return estadoUI;
        }
    }
}
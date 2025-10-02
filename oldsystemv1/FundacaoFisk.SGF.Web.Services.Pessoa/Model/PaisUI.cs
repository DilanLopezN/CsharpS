using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.Model
{
    using Componentes.GenericModel;
    public class PaisUI : TO
    {
        public int cd_localidade { get; set; }
        public byte cd_tipo_localidade { get; set; }
        public String dc_num_pais { get; set; }
        public String dc_nacionalidade_masc { get; set; }
        public String dc_nacionalidade_fem { get; set; }
        public String sg_pais { get; set; }
        public String dc_pais { get; set; }
        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();
                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_localidade", "Código"));
                retorno.Add(new DefinicaoRelatorio("dc_pais", "País", AlinhamentoColuna.Left, "3.0000in"));
                retorno.Add(new DefinicaoRelatorio("sg_pais", "Sigla"));
                retorno.Add(new DefinicaoRelatorio("dc_num_pais", "Número", AlinhamentoColuna.Left, "1.5000in"));
                retorno.Add(new DefinicaoRelatorio("dc_nacionalidade_masc", "Nacionalidade(Masc.)", AlinhamentoColuna.Left, "2.1000in"));
                retorno.Add(new DefinicaoRelatorio("dc_nacionalidade_fem", "Nacionalidade(Fem.)", AlinhamentoColuna.Left, "2.1000in"));
                return retorno;
            }

        }
        public static PaisUI fromPais(LocalidadeSGF localidade, String dc_pais, String sg_pais, string dc_nacionalidade_fem, string dc_nacionalidade_masc, string dc_num_pais)
        {
            PaisUI paisUI = new PaisUI();
            paisUI.cd_localidade = localidade.cd_localidade;
            //paisUI.cd_localidade_pais = localidade.cd_localidade;
            paisUI.cd_tipo_localidade = localidade.cd_tipo_localidade;
            paisUI.dc_nacionalidade_fem = dc_nacionalidade_fem;
            paisUI.dc_nacionalidade_masc = dc_nacionalidade_masc;
            paisUI.dc_num_pais = dc_num_pais;
            paisUI.dc_pais = dc_pais;
            paisUI.sg_pais = sg_pais;
            return paisUI;
        }
    }
}

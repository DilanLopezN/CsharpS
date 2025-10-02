using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    using Componentes.GenericModel;
    public partial class Atividade
    {
        public string atividade_ativa
        {
            get
            {
                return this.id_atividade_ativa ? "Sim" : "Não";
            }
        }
        public string natureza_atividade
        {
            get
            {
                return this.id_natureza_atividade == 1 ? "Física" : this.id_natureza_atividade == 2 ? "Jurídica" : "Cargo";
            }
        }
        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();
                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_atividade", "Código"));
                retorno.Add(new DefinicaoRelatorio("no_atividade", "Atividade/Profissão", AlinhamentoColuna.Left, "2.2000in"));
                retorno.Add(new DefinicaoRelatorio("natureza_atividade", "Natureza", AlinhamentoColuna.Left));
                retorno.Add(new DefinicaoRelatorio("cd_cnae_atividade", "CNAE", AlinhamentoColuna.Left, "2.2000in"));
                retorno.Add(new DefinicaoRelatorio("atividade_ativa", "Ativa", AlinhamentoColuna.Center));
                return retorno;
            }

        }
    }
}

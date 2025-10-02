using System.Collections.Generic;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class OrgaoFinanceiro
    {
        public string orgao_financeiro_ativo
        {
            get
            {
                return this.id_orgao_ativo == 1 ? "Sim" : this.id_orgao_ativo == 0 ?  "Não" : "";
            }
        }

        /// <summary>
        /// Função que monta as colunos no relatório do TIpo liquidação
        /// </summary>
        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();
                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_tipo_liquidacao", "Código"));
                retorno.Add(new DefinicaoRelatorio("dc_orgao_financeiro", "Orgão Financeiro"));
                retorno.Add(new DefinicaoRelatorio("orgao_financeiro_ativo", "Ativo", AlinhamentoColuna.Center));
                return retorno;
            }
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.Web.Services.Empresa.Model;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Model
{
    public class TipoAvaliacaoTurma
    {
        public enum Hierarquia
        {
            PAI = 1,
            FILHO = 0
        }

        public List<AvaliacaoAlunos> children { get; set; }

        public int cd_tipo_avaliacao { get; set; }
        public string dc_tipo_avaliacao { get; set; }
        public int id { get; set; }
        public int idPai { get; set; }
        public int pai { get; set; }
        public string dc_nome { get; set; }
        public bool ativo { get; set; }
     
        public TipoAvaliacaoTurma()
        {
            children = new List<AvaliacaoAlunos>();
        }
    }

    public class AvaliacaoAuxiliarUI {

        public List<TipoAvaliacaoTurma> tipoAvaliacaoTurma { get; set; }
        public List<HistoricoAluno> historicoAluno { get; set; }
        public List<FuncionarioSearchUI> funcionarioAvaliador { get; set; }
        public ICollection<Conceito> conceitosDisponiveis { get; set; }
    }
}

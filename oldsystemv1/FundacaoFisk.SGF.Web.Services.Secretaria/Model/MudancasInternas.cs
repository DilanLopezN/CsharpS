using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Model {
    public partial class MudancasInternas : TO
    {
        public enum OpcoesMudanca
        {
            MudarTurma = 1,
            RetornarTurmaOri = 2
        }

        public List<AlunoTurma> alunos { get; set; }
        public int cd_escola { get; set; }
        public int cd_usuario { get; set; }
        public int cd_produto { get; set; }
        public int cd_turma_origem { get; set; }
        public int cd_turma_destino { get; set; }
        public int cd_curso { get; set; }
        public int opcao { get; set; }
        public bool id_manter_contrato { get; set; }
        public bool id_renegociacao { get; set; }
        public bool id_ppt { get; set; }
        public DateTime dt_movimentacao { get; set; }
        public DateTime dt_inicio { get; set; }        
        public int cd_tipo_liquidacao { get; set; }
        public int cd_local_movto { get; set; }
        public ICollection<Titulo> titulos { get; set; }
        public ChequeTransacao chequeTransacao { get; set; }
        public bool id_ppt_origem { get; set; }
        public int fusoHorario { get; set; }

    }
}

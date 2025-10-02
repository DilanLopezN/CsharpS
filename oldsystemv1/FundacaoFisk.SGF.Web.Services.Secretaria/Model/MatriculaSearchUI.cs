using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Model {
    public partial class MatriculaSearchUI : TO {
        //public MatriculaSearchUI() { }
        public int cd_contrato { get; set; }
        public int? nm_contrato { get; set; }
        public DateTime? dt_inicial_contrato { get; set; }
        public DateTime? dt_final_contrato { get; set; }
        public DateTime? dt_matricula_contrato { get; set; }
        public int cd_aluno { get; set; }
        public string no_pessoa { get; set; }
        public byte id_tipo_matricula { get; set; }
        public string dc_tipo_matricula { get; set; }
        public string no_aluno { get; set; }
        public int cd_turma { get; set; }
        public string no_turma { get; set; }
        public string dc_situacao_turma { get; set; }
        public int cd_pessoa_escola { get; set; }
        public bool id_renegociacao { get; set; }
        public bool id_transferencia { get; set; }
        public bool id_retorno { get; set; }
        public string situacaoTurma { get; set; }
        public DateTime? dt_matricula { get; set; }
        public DateTime? dt_inicio { get; set; }
        public DateTime? dt_fim { get; set; }
        public Nullable<int> nm_matricula_contrato { get; set; }
        public int tipo { get; set; }
        public string dc_tipo_desconto { get; set; }
    }
}

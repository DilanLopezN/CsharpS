using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Model
{
    public class MatriculaRel : TO
    {
        public string id_tipo_turma { get; set; }
        public string grupoTurma { get; set; }
        public int cd_aluno { get; set; }
        public int cd_turma { get; set; }
        public string no_turma { get; set; }
        public string no_produto { get; set; }
	    public string dc_duracao { get; set; }
        public string telefone { get; set; }
        public string no_curso { get; set; }
        public string no_regime { get; set; }
        public string no_pessoa { get; set; }
        public DateTime? dt_matricula_contrato { get; set; }
        public int? nm_contrato { get; set; }
        public int nm_parcelas_mensalidade { get; set; }
        public byte nm_dia_vcto { get; set; }
        public decimal vl_parcela_contrato { get; set; }
        //Por Motivo Matrícula
        public int cd_motivo_matricula { get; set; }
        public string dc_motivo_matricula { get; set; }
        public int qtd_motivo_por_matricula { get; set; }
        public string motivo_desistencia { get; set; }
        public string no_professor { get; set; }
        public string no_atendente { get; set; }
        public string no_apelido { get; set; }
        public byte nm_parcelas_material { get; set; }
        public decimal vl_material_contrato { get; set; }
        public bool contrato_digitalizado { get; set; }
        public string dc_endereco_completo { get; set; }
        public EnderecoSGF enderecoAluno { get; set; }
        public enum TipoRelatorioMat
        {
            MATRICULA_ANALITICO = 0,
            MATRICULA_MOTIVO = 1,
            MATRICULA_ATENDENTE = 2
        }
        public string dtaMatricula
        {
            get
            {
                if (dt_matricula_contrato != null)
                    return String.Format("{0:dd/MM/yyyy}", dt_matricula_contrato);
                else
                    return String.Empty;
            }
            set { }
        }
        string _dados_turma;
        public string dados_turma
        {
            get
            {
                 _dados_turma = this.no_produto + " - " + this.dc_duracao + " - " + this.no_curso;
                if(this.no_regime != null && this.no_regime != "")
                    _dados_turma = _dados_turma + " - " + this.no_regime;
                return _dados_turma;
            }
            set { }
        }
        
    }
}

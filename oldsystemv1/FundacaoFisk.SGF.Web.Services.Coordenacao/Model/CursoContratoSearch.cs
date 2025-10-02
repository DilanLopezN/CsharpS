using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Services.Coordenacao.Model
{
    public class CursoContratoSearch : TO
    {
        public int cd_curso_contrato { get; set; }
        public int cd_contrato { get; set; }
        public int cd_curso { get; set; }
        public int cd_duracao { get; set; }
        public int cd_tipo_financeiro { get; set; }
        public int cd_pessoa_responsavel { get; set; }
        public byte nm_dia_vcto { get; set; }
        public byte nm_mes_vcto { get; set; }
        public short nm_ano_vcto { get; set; }
        public int nm_parcelas_mensalidade { get; set; }
        public decimal vl_curso_contrato { get; set; }
        public decimal pc_desconto_contrato { get; set; }
        public decimal vl_matricula_curso { get; set; }
        public decimal vl_parcela_contrato { get; set; }
        public decimal vl_desconto_contrato { get; set; }
        public decimal pc_responsavel_contrato { get; set; }
        public decimal vl_parcela_liquida { get; set; }
        public bool id_liberar_certificado { get; set; }
        public decimal vl_curso_liquido { get; set; }
        public string no_pessoa_responsavel { get; set; }
        public string no_curso { get; set; }
        public int? cd_proximo_curso { get; set; }
        public string no_tipo_financeiro { get; set; }
        public Nullable<byte> nm_mes_curso_inicial { get; set; }
        public Nullable<short> nm_ano_curso_inicial { get; set; }
        public Nullable<byte> nm_mes_curso_final { get; set; }
        public Nullable<short> nm_ano_curso_final { get; set; } 
    }
}
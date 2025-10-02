using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class CursoContrato
    {
        public string no_pessoa_responsavel { get; set; }
        public string no_curso { get; set; }
        public int? cd_proximo_curso { get; set; }
        public string no_tipo_financeiro { get; set; }
        public long? cd_curso_ordem { get; set; }


        public static void changeValuesCursoContrato(CursoContrato cursoContratoContext, CursoContrato cursoContratoView)
        {
            cursoContratoContext.cd_curso_contrato = cursoContratoView.cd_curso_contrato;
            cursoContratoContext.cd_contrato = cursoContratoView.cd_contrato;
            cursoContratoContext.cd_curso = cursoContratoView.cd_curso;
            cursoContratoContext.cd_duracao = cursoContratoView.cd_duracao;
            cursoContratoContext.cd_tipo_financeiro = cursoContratoView.cd_tipo_financeiro;
            cursoContratoContext.cd_pessoa_responsavel = cursoContratoView.cd_pessoa_responsavel;
            cursoContratoContext.nm_dia_vcto = cursoContratoView.nm_dia_vcto;
            cursoContratoContext.nm_mes_vcto = cursoContratoView.nm_mes_vcto;
            cursoContratoContext.nm_ano_vcto = cursoContratoView.nm_ano_vcto;
            cursoContratoContext.nm_parcelas_mensalidade = cursoContratoView.nm_parcelas_mensalidade;
            cursoContratoContext.vl_curso_contrato = cursoContratoView.vl_curso_contrato;
            cursoContratoContext.pc_desconto_contrato = cursoContratoView.pc_desconto_contrato;
            cursoContratoContext.vl_matricula_curso = cursoContratoView.vl_matricula_curso;
            cursoContratoContext.vl_parcela_contrato = cursoContratoView.vl_parcela_contrato;
            cursoContratoContext.vl_desconto_contrato = cursoContratoView.vl_desconto_contrato;
            cursoContratoContext.pc_responsavel_contrato = cursoContratoView.pc_responsavel_contrato;
            cursoContratoContext.vl_parcela_liquida = cursoContratoView.vl_parcela_liquida;
            cursoContratoContext.id_liberar_certificado = cursoContratoView.id_liberar_certificado;
            cursoContratoContext.vl_curso_liquido = cursoContratoView.vl_curso_liquido;
            cursoContratoContext.no_pessoa_responsavel = cursoContratoView.no_pessoa_responsavel;
            cursoContratoContext.no_curso = cursoContratoView.no_curso;
            cursoContratoContext.cd_proximo_curso = cursoContratoView.cd_proximo_curso;
            cursoContratoContext.no_tipo_financeiro = cursoContratoView.no_tipo_financeiro;
            cursoContratoContext.nm_mes_curso_inicial = cursoContratoView.nm_mes_curso_inicial;
            cursoContratoContext.nm_ano_curso_inicial = cursoContratoView.nm_ano_curso_inicial;
            cursoContratoContext.nm_mes_curso_final = cursoContratoView.nm_mes_curso_final;
            cursoContratoContext.nm_ano_curso_final = cursoContratoView.nm_ano_curso_final;
            cursoContratoContext.id_incorporar_valor_material = cursoContratoView.id_incorporar_valor_material;
            cursoContratoContext.id_valor_incluso = cursoContratoView.id_valor_incluso;
            cursoContratoContext.nm_parcelas_material = cursoContratoView.nm_parcelas_material;
            cursoContratoContext.vl_material_contrato = cursoContratoView.vl_material_contrato;
            cursoContratoContext.vl_parcela_material = cursoContratoView.vl_parcela_material;
            cursoContratoContext.vl_parcela_liq_material = cursoContratoView.vl_parcela_liq_material;
            cursoContratoContext.pc_bolsa_material = cursoContratoView.pc_bolsa_material;

        }
    }
}
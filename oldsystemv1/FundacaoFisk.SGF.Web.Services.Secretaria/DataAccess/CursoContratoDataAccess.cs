using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess;
using log4net;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess
{
    public class CursoContratoDataAccess : GenericRepository<CursoContrato>, ICursoContratoDataAccess
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(CursoContratoDataAccess));

        //Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public CursoContrato getCursoContratoById(int cd_curso_contrato)
        {
            
                try
                {
                    CursoContrato sql = (from cc in db.CursoContrato
                        where cc.cd_curso_contrato == cd_curso_contrato
                        select cc).FirstOrDefault();

                        return sql;
                    }
                        catch (Exception exe)
                {
                    throw new DataAccessException(exe);
                } 
        }

        public List<CursoContrato> getCursosContratoByCdContrato(int cd_contrato)
        {
            try
            {
                List<CursoContrato> sql = (from cc in db.CursoContrato
                                     join c in db.Curso on cc.cd_curso equals c.cd_curso
                                     join p in db.PessoaSGF on cc.cd_pessoa_responsavel equals p.cd_pessoa
                                     join t in db.TipoFinanceiro on cc.cd_tipo_financeiro equals t.cd_tipo_financeiro
                                     join v in db.vi_curso_ordem on cc.cd_curso equals v.cd_curso
                                           where cc.cd_contrato == cd_contrato
                                     select new
                                     {
                                         cd_curso_contrato = cc.cd_curso_contrato,
                                         cd_contrato = cc.cd_contrato,
                                         cd_curso = cc.cd_curso,
                                         cd_duracao = cc.cd_duracao,
                                         cd_tipo_financeiro = cc.cd_tipo_financeiro,
                                         cd_pessoa_responsavel = cc.cd_pessoa_responsavel,
                                         nm_dia_vcto = cc.nm_dia_vcto,
                                         nm_mes_vcto = cc.nm_mes_vcto,
                                         nm_ano_vcto = cc.nm_ano_vcto,
                                         nm_parcelas_mensalidade = cc.nm_parcelas_mensalidade,
                                         vl_curso_contrato = cc.vl_curso_contrato,
                                         pc_desconto_contrato = cc.pc_desconto_contrato,
                                         vl_matricula_curso = cc.vl_matricula_curso,
                                         vl_parcela_contrato = cc.vl_parcela_contrato,
                                         vl_desconto_contrato = cc.vl_desconto_contrato,
                                         pc_responsavel_contrato = cc.pc_responsavel_contrato,
                                         vl_parcela_liquida = cc.vl_parcela_liquida,
                                         id_liberar_certificado = cc.id_liberar_certificado,
                                         vl_curso_liquido = cc.vl_curso_liquido,
                                         no_pessoa_responsavel = cc.PessoaSGF.no_pessoa,
                                         no_curso = cc.Curso.no_curso,
                                         cd_proximo_curso = cc.Curso.cd_proximo_curso,
                                         no_tipo_financeiro = cc.TipoFinanceiro.dc_tipo_financeiro,
                                         nm_mes_curso_inicial = cc.nm_mes_curso_inicial == null ? cc.nm_mes_vcto : cc.nm_mes_curso_inicial,
                                         nm_ano_curso_inicial = cc.nm_ano_curso_inicial == null ? cc.nm_ano_vcto : cc.nm_ano_curso_inicial,
                                         nm_mes_curso_final = cc.nm_mes_curso_final == null ? cc.nm_mes_vcto : cc.nm_mes_curso_final,
                                         nm_ano_curso_final = cc.nm_ano_curso_final == null ? cc.nm_ano_vcto : cc.nm_ano_curso_final,
                                         id_incorporar_valor_material = cc.id_incorporar_valor_material,
                                         id_valor_incluso = cc.id_valor_incluso,
                                         nm_parcelas_material = cc.nm_parcelas_material,
                                         vl_material_contrato = cc.vl_material_contrato,
                                         vl_parcela_material = cc.vl_parcela_material,
                                         vl_parcela_liq_material = cc.vl_parcela_liq_material,
                                         pc_bolsa_material = cc.pc_bolsa_material,
                                         cd_curso_ordem = v.cd_curso_ordem
                                     }).ToList().Select(x2 => new CursoContrato()
                                     {
                                         cd_curso_contrato = x2.cd_curso_contrato,
                                         cd_contrato = x2.cd_contrato,
                                         cd_curso = x2.cd_curso,
                                         cd_duracao = x2.cd_duracao,
                                         cd_tipo_financeiro = x2.cd_tipo_financeiro,
                                         cd_pessoa_responsavel = x2.cd_pessoa_responsavel,
                                         nm_dia_vcto = x2.nm_dia_vcto,
                                         nm_mes_vcto = x2.nm_mes_vcto,
                                         nm_ano_vcto = x2.nm_ano_vcto,
                                         nm_parcelas_mensalidade = x2.nm_parcelas_mensalidade,
                                         vl_curso_contrato = x2.vl_curso_contrato,
                                         pc_desconto_contrato = x2.pc_desconto_contrato,
                                         vl_matricula_curso = x2.vl_matricula_curso,
                                         vl_parcela_contrato = x2.vl_parcela_contrato,
                                         vl_desconto_contrato = x2.vl_desconto_contrato,
                                         pc_responsavel_contrato = x2.pc_responsavel_contrato,
                                         vl_parcela_liquida = x2.vl_parcela_liquida,
                                         id_liberar_certificado = x2.id_liberar_certificado,
                                         vl_curso_liquido = x2.vl_curso_liquido,
                                         no_pessoa_responsavel = x2.no_pessoa_responsavel,
                                         no_curso = x2.no_curso,
                                         cd_proximo_curso = x2.cd_proximo_curso,
                                         no_tipo_financeiro = x2.no_tipo_financeiro,
                                         nm_mes_curso_inicial = x2.nm_mes_curso_inicial,
                                         nm_ano_curso_inicial = x2.nm_ano_curso_inicial,
                                         nm_mes_curso_final = x2.nm_mes_curso_final,
                                         nm_ano_curso_final = x2.nm_ano_curso_final,
                                         id_incorporar_valor_material = x2.id_incorporar_valor_material,
                                         id_valor_incluso = x2.id_valor_incluso,
                                         nm_parcelas_material = x2.nm_parcelas_material,
                                         vl_material_contrato = x2.vl_material_contrato,
                                         vl_parcela_material = x2.vl_parcela_material,
                                         vl_parcela_liq_material = x2.vl_parcela_liq_material,
                                         pc_bolsa_material = x2.pc_bolsa_material,
                                         cd_curso_ordem = x2.cd_curso_ordem
                                     }).ToList();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            } 
        }
    }
}
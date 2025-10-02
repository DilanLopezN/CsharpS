using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Sockets;
using System.Data;
using System.Threading.Tasks;
using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;
using log4net;
using DALC4NET;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess
{
    public class MatriculaDataAccess : GenericRepository<Contrato>, IMatriculaDataAccess
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(MatriculaDataAccess));

      

        //Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public enum FiltroLayoutContratoEnum
        {
            NENHUM = 1000000
        }

        public IEnumerable<Contrato> getMatriculaSearch(SearchParameters parametros, string descAluno, string descTurma, bool inicio, bool semTurma,
                                                            int situacaoTurma, int nmContrato, int tipo, DateTime? dtaInicio, DateTime? dtaFim,
                                                            bool filtraMat, bool filtraDtaInicio, bool filtraDtaFim, int cdEscola, bool renegocia,
                                                            bool transf, bool retornoEsc, int? cdNomeContrato, IAditamentoDataAccess DataAccessAditamento, int nm_matricula,
                                                            int? cd_ano_escolar, int? cdContratoAnterior, byte tipoC, bool? status, int vinculado)
        {
            try
            {
                IEntitySorter<MatriculaSearchUI> sorter = EntitySorter<MatriculaSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                int id_origem_movimento = Int32.Parse(db.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                IQueryable<vi_contrato> sql;

                sql = from contrato in db.vi_contrato.AsNoTracking()
                      where contrato.cd_pessoa_escola == cdEscola
                      orderby contrato.no_pessoa ascending
                      select contrato;

                if (!String.IsNullOrEmpty(descAluno))
                {
                    if (inicio)
                        sql = from c in sql
                              where c.no_pessoa.StartsWith(descAluno)
                              select c;
                    else
                        sql = from c in sql
                              where c.no_pessoa.Contains(descAluno)
                              select c;

                }

                if (!String.IsNullOrEmpty(descTurma))
                {
                    if (inicio)
                        sql = from c in sql
                              where c.no_turma.StartsWith(descTurma)
                              select c;
                    else
                        sql = from c in sql
                              where c.no_turma.Contains(descTurma)
                              select c;

                }

                if (tipoC != 4)
                {
                    sql = from c in sql
                          where c.id_tipo_contrato == tipoC
                          select c;
                }

                if (cdContratoAnterior == 1)
                {
                    sql = from c in sql
                          where c.cd_contrato_anterior != null
                    select c;
                }
                //else {
                //    sql = from c in sql
                //          select c;
                //}


                if (semTurma)
                    sql = from c in sql
                          where c.nm_turma == 0
                          select c;

                if (situacaoTurma > 0 && !semTurma)
                    sql = from c in sql
                          where c.id_situacao_turma == situacaoTurma
                          select c;

                if (tipo > 0)
                    sql = from c in sql
                          where c.id_tipo_matricula == tipo
                          select c;

                if (nmContrato > 0)
                    sql = from c in sql
                          where c.nm_contrato == nmContrato
                          select c;

                if (nm_matricula > 0)
                    sql = from c in sql
                          where c.nm_matricula_contrato == nm_matricula
                          select c;
                
                if (dtaInicio != null)
                    if (dtaFim != null)
                    {
                        if (filtraMat == true || (filtraMat == false && filtraDtaInicio == false && filtraDtaFim == false))
                            sql = from c in sql
                                  where c.dt_matricula_contrato >= dtaInicio && c.dt_matricula_contrato <= dtaFim
                                  select c;
                        if (filtraDtaInicio == true)
                            sql = from c in sql
                                  where c.dt_inicial_contrato >= dtaInicio && c.dt_inicial_contrato <= dtaFim
                                  select c;
                        if (filtraDtaFim == true)
                            sql = from c in sql
                                  where c.dt_final_contrato >= dtaInicio && c.dt_final_contrato <= dtaFim
                                  select c;
                    }
                    else
                    {
                        if (filtraMat == true || (filtraMat == false && filtraDtaInicio == false && filtraDtaFim == false))
                            sql = from c in sql
                                  where c.dt_matricula_contrato >= dtaInicio
                                  select c;
                        if (filtraDtaInicio == true)
                            sql = from c in sql
                                  where c.dt_inicial_contrato >= dtaInicio
                                  select c;
                        if (filtraDtaFim == true)
                            sql = from c in sql
                                  where c.dt_final_contrato >= dtaInicio
                                  select c;
                    }

                if (renegocia)
                    sql = from c in sql
                          where c.id_renegociacao == true
                          select c;
                if (transf)
                    sql = from c in sql
                          where c.id_transferencia == true
                          select c;
                if (retornoEsc)
                    sql = from c in sql
                          where c.id_retorno == true
                          select c;
                if (cdNomeContrato > 0)
                    if (cdNomeContrato == (int)FiltroLayoutContratoEnum.NENHUM)
                        sql = from c in sql
                              where !db.Aditamento.Where(a => a.cd_contrato == c.cd_contrato).Any() ||
                              db.Aditamento.Where(a => a.cd_contrato == c.cd_contrato && !a.cd_nome_contrato.HasValue).Any()
                              select c;
                    else
                        sql = from c in sql
                              where db.Aditamento.Where(a => a.cd_contrato == c.cd_contrato && a.cd_nome_contrato.Value == cdNomeContrato).Any()
                              select c;
                if (cd_ano_escolar > 0)
                   sql = from c in sql
                         where c.cd_ano_escolar == cd_ano_escolar
                              select c;
                if (status != null)
                    sql = from c in sql
                          where db.Aluno.Any(a => a.cd_aluno == c.cd_aluno && a.id_aluno_ativo == status)
                          select c;

                if (vinculado > 0)
                {
                    if (vinculado == 1)
                    {
                        sql = from c in sql
                              where (from m in db.Movimento
                                     where m.cd_origem_movimento == c.cd_contrato &&
                                           m.id_origem_movimento == id_origem_movimento &&
                                           m.id_material_didatico == true &&
                                           !m.id_venda_futura
                                     select m).Any()
                              select c;
                    }
                    else
                    if (vinculado == 2)
                    {
                        sql = from c in sql
                              where !(from m in db.Movimento
                                     where m.cd_origem_movimento == c.cd_contrato &&
                                           m.id_origem_movimento == id_origem_movimento &&
                                           m.id_material_didatico == true &&
                                           !m.id_venda_futura
                                     select m).Any()
                              select c;
                    }
                    else
                    {
                        sql = from c in sql
                              join ct in db.CursoContrato on c.cd_contrato equals ct.cd_contrato
                              join at in db.AlunoTurma on c.cd_contrato equals at.cd_contrato
                              join co in db.Contrato on c.cd_contrato equals co.cd_contrato
                              where (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                     at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado) &&
                                    !(from m in db.Movimento
                                     where m.cd_origem_movimento == c.cd_contrato &&
                                           m.id_origem_movimento == id_origem_movimento &&
                                           m.id_material_didatico == true &&
                                           !m.id_venda_futura
                                      select m).Any() &&
                                     (from m in db.Movimento
                                      where m.cd_pessoa_empresa == c.cd_pessoa_escola &&
                                            m.id_material_didatico == true &&
                                            !m.id_venda_futura &&
                                            m.id_origem_movimento == null &&
                                            (m.cd_aluno == at.cd_aluno || m.cd_pessoa == co.cd_pessoa_responsavel) &&
                                            m.ItensMovimento.Where(
                                                im => im.Item.cd_tipo_item == (int)TipoItem.TipoItemEnum.MATERIAL_DIDATICO &&
                                                im.Item.id_material_didatico == true &&
                                                im.Item.Cursos.Where(ic => ic.cd_curso == ct.cd_curso).Any()
                                            ).Any()
                                      select m).Any()

                              select c;
                    }
                    
                }

                int limite = sql.ToList().Count();

                var retorno = (from x in sql
                               select new MatriculaSearchUI
                               {
                                   cd_contrato = x.cd_contrato,
                                   nm_contrato = x.nm_contrato,
                                   dt_inicial_contrato = x.dt_inicial_contrato,
                                   dt_final_contrato = x.dt_final_contrato,
                                   dt_matricula_contrato = x.dt_matricula_contrato,
                                   cd_aluno = x.cd_aluno,
                                   no_pessoa = x.no_pessoa,
                                   id_tipo_matricula = x.id_tipo_matricula,
                                   dc_tipo_matricula = x.dc_tipo_matricula,
                                   no_turma = x.no_turma,
                                   //nm_turma = x.nm_turma,
                                   //id_situacao_turma = x.id_situacao_turma,
                                   dc_situacao_turma = x.dc_situacao_turma,
                                   cd_pessoa_escola = x.cd_pessoa_escola,
                                   id_renegociacao = x.id_renegociacao,
                                   id_transferencia = x.id_transferencia,
                                   nm_matricula_contrato = x.nm_matricula_contrato,
                                   id_retorno = x.id_retorno,
                                   dc_tipo_desconto = x.dc_tipo_desconto
                               });

                retorno = sorter.Sort(retorno);

                parametros.ajustaParametrosPesquisa(limite);
                var retorno3 = retorno.Skip(parametros.from).Take(parametros.qtd_limite).ToList();
                parametros.qtd_limite = limite;

                //List<Contrato> convertContrato = new List<Contrato>();
                List<Contrato> convertContrato = (from x in retorno3
                                                  select new Contrato
                                                  {
                                                      cd_contrato = x.cd_contrato,
                                                      nm_contrato = x.nm_contrato,
                                                      dt_inicial_contrato = x.dt_inicial_contrato,
                                                      dt_final_contrato = x.dt_final_contrato,
                                                      dt_matricula_contrato = x.dt_matricula_contrato,
                                                      cd_aluno = x.cd_aluno,
                                                      no_pessoa = x.no_pessoa,
                                                      id_tipo_matricula = x.id_tipo_matricula,
                                                      dc_tipo_matricula = x.dc_tipo_matricula,
                                                      no_turma = x.no_turma,
                                                      //nm_turma = x.nm_turma,
                                                      //id_situacao_turma = x.id_situacao_turma,
                                                      dc_situacao_turma = x.dc_situacao_turma,
                                                      cd_pessoa_escola = x.cd_pessoa_escola,
                                                      id_renegociacao = x.id_renegociacao,
                                                      id_transferencia = x.id_transferencia,
                                                      id_retorno = x.id_retorno,
                                                      nm_matricula_contrato = x.nm_matricula_contrato,
                                                      DescontoContrato = null,
                                                      desc_descontos_contrato = x.dc_tipo_desconto
                                                  }).ToList();

                //foreach (var d in convertContrato)
                //    d.DescontoContrato = DataAccessAditamento.getDescontosAplicadosAditamento(d.cd_contrato, cdEscola).ToList();

                return convertContrato;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<ContratoComboUI> getContratosSemTurmaByAlunoSearch(int cd_aluno,
            bool semTurma, int situacaoTurma, int nmContrato, int tipo, int cdEscola, byte tipoC, bool? status)
        {
            try
            {

                IQueryable<vi_contrato> sql;
                sql = from contrato in db.vi_contrato.AsNoTracking()
                          where contrato.cd_pessoa_escola == cdEscola
                    orderby contrato.no_pessoa ascending
                      select contrato;


                if (cd_aluno > 0)
                {
                    sql = from c in sql
                          where c.cd_aluno == cd_aluno
                          select c;
                }
                

                if (tipoC != 4)
                {
                    sql = from c in sql
                          where c.id_tipo_contrato == tipoC
                          select c;
                }

                


                if (semTurma)
                    sql = from c in sql
                          where c.nm_turma == 0
                          select c;

                if (situacaoTurma > 0 && !semTurma)
                    sql = from c in sql
                          where c.id_situacao_turma == situacaoTurma
                          select c;

                if (tipo > 0)
                    sql = from c in sql
                          where c.id_tipo_matricula == tipo
                          select c;

                if (nmContrato > 0)
                    sql = from c in sql
                          where c.nm_contrato == nmContrato
                          select c;

                
                if (status != null)
                    sql = from c in sql
                          where db.Aluno.Any(a => a.cd_aluno == c.cd_aluno && a.id_aluno_ativo == status)
                          select c;

                int limite = sql.Select(x => x.cd_contrato).ToList().Count();

                var retorno = (from x in sql
                               select new ContratoComboUI
                               {
                                   cd_contrato = x.cd_contrato,
                                   nm_contrato = x.nm_contrato,
                                   nm_matricula_contrato = x.nm_matricula_contrato,
                                   
                               }).ToList();

                

                //foreach (var d in convertContrato)
                //    d.DescontoContrato = DataAccessAditamento.getDescontosAplicadosAditamento(d.cd_contrato, cdEscola).ToList();

                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Contrato getMatriculaById(int id, int cdEscola)
        {
            try
            {
                SGFWebContext dbContext = new SGFWebContext();
                int[] statusCnabTitulo = new int[] { (int)Titulo.StatusCnabTitulo.INICIAL, (int)Titulo.StatusCnabTitulo.CONFIRMADO_PEDIDO_BAIXA };
                int cd_origem = Int32.Parse(dbContext.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                var variaveis = (from c in db.Contrato where c.cd_contrato == id 
                                 select new
                                 {
                                     cd_aluno = c.cd_aluno,
                                     cd_curso = c.cd_curso_atual
                                 }).FirstOrDefault();
                int cd_aluno_destino = variaveis.cd_aluno;
                int[] origens = new int[] { };

                while ((from t in db.TransferenciaAluno where t.cd_aluno_destino == cd_aluno_destino select t.cd_aluno_destino).Any())
                {
                    if (!(from m in db.Movimento where m.cd_aluno == cd_aluno_destino && m.cd_curso == variaveis.cd_curso && !m.id_venda_futura && m.id_material_didatico && m.cd_origem_movimento != null select m).Any())
                    {
                        cd_aluno_destino = (from t in db.TransferenciaAluno where t.cd_aluno_destino == cd_aluno_destino select t.cd_aluno_origem).FirstOrDefault();
                        if (origens.Contains(cd_aluno_destino)) cd_aluno_destino = variaveis.cd_aluno;
                        else
                            origens = origens.Concat(new int[] { cd_aluno_destino }).ToArray();
                    }
                    else
                        break;
                    if (cd_aluno_destino == variaveis.cd_aluno) break;
                }
                Contrato sql = ((from c in db.Contrato
                                 where
                                     c.cd_contrato == id &&
                                     c.cd_pessoa_escola == cdEscola
                                 select new
                                 {
                                     cd_contrato = c.cd_contrato,
                                     cd_aluno = c.cd_aluno,
                                     cd_usuario = c.cd_usuario,
                                     nm_contrato = c.nm_contrato,
                                     dc_serie_contrato = c.dc_serie_contrato,
                                     nm_matricula_contrato = c.nm_matricula_contrato,
                                     dt_inicial_contrato = c.dt_inicial_contrato,
                                     dt_final_contrato = c.dt_final_contrato,
                                     dt_matricula_contrato = c.dt_matricula_contrato,
                                     id_ajuste_manual = c.id_ajuste_manual,
                                     id_contrato_aula = c.id_contrato_aula,
                                     cd_plano_conta = c.cd_plano_conta,
                                     cd_tipo_financeiro = c.cd_tipo_financeiro,
                                     cd_pessoa_responsavel = c.cd_pessoa_responsavel,
                                     pc_responsavel_contrato = c.pc_responsavel_contrato,
                                     cd_produto_atual = c.cd_produto_atual,
                                     cd_curso_atual = c.cd_curso_atual,
                                     cd_regime_atual = c.cd_regime_atual,
                                     cd_duracao_atual = c.cd_duracao_atual,
                                     nm_dia_vcto = c.nm_dia_vcto,
                                     nm_mes_vcto = c.nm_mes_vcto,
                                     nm_ano_vcto = c.nm_ano_vcto,
                                     nm_parcelas_mensalidade = c.nm_parcelas_mensalidade,
                                     vl_curso_contrato = c.vl_curso_contrato,
                                     vl_matricula_contrato = c.vl_matricula_contrato,
                                     vl_parcela_contrato = c.vl_parcela_contrato,
                                     vl_desconto_contrato = c.vl_desconto_contrato,
                                     pc_desconto_contrato = c.pc_desconto_contrato,
                                     vl_divida_contrato = c.vl_divida_contrato,
                                     id_divida_primeira_parcela = c.id_divida_primeira_parcela,
                                     vl_desc_primeira_parcela = c.vl_desc_primeira_parcela,
                                     cd_pessoa_escola = c.cd_pessoa_escola,
                                     id_nf_servico = c.id_nf_servico,
                                     id_tipo_matricula = c.id_tipo_matricula,
                                     id_renegociacao = c.id_renegociacao,
                                     id_retorno = c.id_retorno,
                                     id_transferencia = c.id_transferencia,
                                     id_liberar_certificado = c.id_liberar_certificado,
                                     cd_pessoa_aluno = c.Aluno.cd_pessoa_aluno,
                                     cd_pessoa_cpf = c.Aluno.AlunoPessoaFisica.cd_pessoa_cpf,
                                     pc_aluno_bolsa = c.Aluno.Bolsa.pc_bolsa,
                                     dt_inicio_bolsa = c.Aluno.Bolsa != null ? c.Aluno.Bolsa.dt_inicio_bolsa : new DateTime(),
                                     no_pessoa = c.Aluno.AlunoPessoaFisica.no_pessoa,
                                     cd_pessoa_usuario = c.SysUsuario.cd_pessoa,
                                     no_usuario = c.SysUsuario.PessoaFisica.no_pessoa,
                                     cd_pessoaF_usuario = c.SysUsuario.PessoaFisica.cd_pessoa,
                                     no_responsavel = (from a in db.PessoaSGF where a.cd_pessoa == c.cd_pessoa_responsavel select a.no_pessoa).FirstOrDefault(),
                                         //db.PessoaSGF.Where(p => p.cd_pessoa == c.cd_pessoa_responsavel).FirstOrDefault().no_pessoa,
                                     no_pessoa_cpf = c.Aluno != null && c.Aluno.AlunoPessoaFisica != null ? db.PessoaSGF.Where(p => p.cd_pessoa == c.Aluno.AlunoPessoaFisica.cd_pessoa_cpf).FirstOrDefault().no_pessoa : "",
                                     PlanoConta = c.PlanoConta,
                                     PlanoContaSubgrupo = c.PlanoConta.PlanoContaSubgrupo,
                                     cd_nome_contrato = c.Aditamento.Where(a => a.cd_contrato == c.cd_contrato).OrderByDescending(a => a.dt_aditamento).FirstOrDefault().cd_nome_contrato,
                                     id_tipo_aditamento = c.Aditamento.Where(a => a.cd_contrato == c.cd_contrato).OrderByDescending(a => a.dt_aditamento).FirstOrDefault().id_tipo_aditamento,
                                     perc_descontoParametros_maximo = (from a in db.Parametro where a.cd_pessoa_escola == c.cd_pessoa_escola select a.per_desconto_maximo).FirstOrDefault(),
                                     id_venda_pacote = c.id_venda_pacote,
                                     cd_mala_direta = c.cd_mala_direta,
                                     dc_assunto = c.MalaDireta.dc_assunto,
                                     pc_bolsa = c.pc_desconto_bolsa,
                                     c.vl_pre_matricula,
                                     c.cd_ano_escolar,
                                     id_tipo_contrato = c.id_tipo_contrato,
                                     nm_mes_curso_inicial = c.nm_mes_curso_inicial,
                                     nm_ano_curso_inicial = c.nm_ano_curso_inicial,
                                     nm_mes_curso_final = c.nm_mes_curso_final,
                                     nm_ano_curso_final = c.nm_ano_curso_final,
                                     CursoContrato = c.CursoContrato,
                                     nm_arquivo_digitalizado = c.nm_arquivo_digitalizado,
                                     id_incorporar_valor_material = c.id_incorporar_valor_material,
                                     id_valor_incluso = c.id_valor_incluso,
                                     nm_parcelas_material = c.nm_parcelas_material,
                                     vl_material_contrato = c.vl_material_contrato,
                                     vl_parcela_material = c.vl_parcela_material,
                                     vl_parcela_liq_material = c.vl_parcela_liq_material,
                                     pc_bolsa_material = c.pc_bolsa_material,
                                     notas_material_didatico = (from m in db.Movimento 
                                                                where m.cd_origem_movimento == c.cd_contrato && m.id_material_didatico == true && 
                                                                    m.id_origem_movimento == cd_origem 
                                                                select new NotasVendaMaterialUI {
                                                                    cd_movimento = m.cd_movimento, 
                                                                    cd_curso = m.cd_curso, 
                                                                    nm_movimento = (String.IsNullOrEmpty(m.nm_movimento.ToString()) ? ("Sem N° Saída, id: " + m.cd_movimento) : m.nm_movimento.ToString()) +
                                                                        (m.dc_serie_movimento == null ? "" : "-" + m.dc_serie_movimento), 
                                                                    id_venda_futura = m.id_venda_futura
                                                                }).Union(
                                                                 (from m in db.Movimento
                                                                  where m.cd_aluno == cd_aluno_destino &&
                                                                        m.cd_curso == c.cd_curso_atual &&      //LBM em contratos multiplos vai ter que rever se nota no segundo curso
                                                                        m.id_material_didatico == true &&
                                                                        m.id_origem_movimento != null &&
                                                                        c.cd_aluno != cd_aluno_destino
                                                                  select new NotasVendaMaterialUI
                                                                  {
                                                                      cd_movimento = m.cd_movimento,
                                                                      cd_curso = m.cd_curso,
                                                                      nm_movimento = (String.IsNullOrEmpty(m.nm_movimento.ToString()) ? ("Sem N° Saída, id: " + m.cd_movimento) : m.nm_movimento.ToString()) +
                                                                        (m.dc_serie_movimento == null ? "" : "-" + m.dc_serie_movimento) + " - " + m.Empresa.dc_reduzido_pessoa,
                                                                      id_venda_futura = m.id_venda_futura
                                                                  })).ToList()
                                 }).ToList().Select(x => new Contrato
                                               {
                                                   cd_contrato = x.cd_contrato,
                                                   cd_aluno = x.cd_aluno,
                                                   cd_usuario = x.cd_usuario,
                                                   nm_contrato = x.nm_contrato,
                                                   dc_serie_contrato = x.dc_serie_contrato,
                                                   nm_matricula_contrato = x.nm_matricula_contrato,
                                                   dt_inicial_contrato = x.dt_inicial_contrato,
                                                   dt_final_contrato = x.dt_final_contrato,
                                                   dt_matricula_contrato = x.dt_matricula_contrato,
                                                   id_ajuste_manual = x.id_ajuste_manual,
                                                   id_contrato_aula = x.id_contrato_aula,
                                                   cd_plano_conta = x.cd_plano_conta,
                                                   cd_tipo_financeiro = x.cd_tipo_financeiro,
                                                   cd_pessoa_responsavel = x.cd_pessoa_responsavel,
                                                   pc_responsavel_contrato = x.pc_responsavel_contrato,
                                                   cd_produto_atual = x.cd_produto_atual,
                                                   cd_curso_atual = x.cd_curso_atual,
                                                   cd_regime_atual = x.cd_regime_atual,
                                                   cd_duracao_atual = x.cd_duracao_atual,
                                                   nm_dia_vcto = x.nm_dia_vcto,
                                                   nm_mes_vcto = x.nm_mes_vcto,
                                                   nm_ano_vcto = x.nm_ano_vcto,
                                                   nm_parcelas_mensalidade = x.nm_parcelas_mensalidade,
                                                   vl_curso_contrato = x.vl_curso_contrato,
                                                   vl_matricula_contrato = x.vl_matricula_contrato,
                                                   vl_parcela_contrato = x.vl_parcela_contrato,
                                                   vl_desconto_contrato = x.vl_desconto_contrato,
                                                   pc_desconto_contrato = x.pc_desconto_contrato,
                                                   vl_divida_contrato = x.vl_divida_contrato,
                                                   id_divida_primeira_parcela = x.id_divida_primeira_parcela,
                                                   vl_desc_primeira_parcela = x.vl_desc_primeira_parcela,
                                                   cd_pessoa_escola = x.cd_pessoa_escola,
                                                   id_nf_servico = x.id_nf_servico,
                                                   id_tipo_matricula = x.id_tipo_matricula,
                                                   id_renegociacao = x.id_renegociacao,
                                                   id_retorno = x.id_retorno,
                                                   id_transferencia = x.id_transferencia,
                                                   id_liberar_certificado = x.id_liberar_certificado,
                                                   nm_arquivo_digitalizado = x.nm_arquivo_digitalizado,
                                                 id_incorporar_valor_material = x.id_incorporar_valor_material,
                                                 id_valor_incluso = x.id_valor_incluso,
                                                 nm_parcelas_material = x.nm_parcelas_material,
                                                 vl_material_contrato = x.vl_material_contrato,
                                                 vl_parcela_material = x.vl_parcela_material,
                                                 vl_parcela_liq_material = x.vl_parcela_liq_material,
                                                 pc_bolsa_material = x.pc_bolsa_material,
                                                   Aluno = new Aluno
                                                   {
                                                       cd_pessoa_aluno = x.cd_pessoa_aluno,

                                                       AlunoPessoaFisica = new PessoaFisicaSGF
                                                       {
                                                           cd_pessoa_cpf = x.cd_pessoa_cpf,
                                                           no_pessoa = x.no_pessoa,
                                                       },
                                                       Bolsa = new AlunoBolsa
                                                       {
                                                           pc_bolsa = x.pc_aluno_bolsa,
                                                           dt_inicio_bolsa = x.dt_inicio_bolsa
                                                       }
                                                   },
                                                   SysUsuario = new UsuarioWebSGF
                                                   {
                                                       cd_pessoa = x.cd_pessoa_usuario,
                                                       cd_usuario = x.cd_usuario,
                                                       PessoaFisica = new PessoaFisicaSGF
                                                       {
                                                           cd_pessoa = x.cd_pessoaF_usuario,
                                                           no_pessoa = x.no_usuario
                                                       }
                                                   },
                                                   no_responsavel = x.no_responsavel,
                                                   PlanoConta = x.PlanoConta,
                                                   cd_nome_contrato = x.cd_nome_contrato,
                                                   perc_descontoParametros_maximo = x.perc_descontoParametros_maximo,
                                                   id_tipo_aditamento = x.id_tipo_aditamento,
                                                   id_venda_pacote = x.id_venda_pacote,
                                                   no_pessoa_cpf = x.no_pessoa_cpf,
                                                   MalaDireta = new MalaDireta
                                                   {
                                                       dc_assunto = x.dc_assunto
                                                   },
                                                   pc_desconto_bolsa = x.pc_bolsa,
                                                   vl_pre_matricula = x.vl_pre_matricula,
                                                   cd_ano_escolar = x.cd_ano_escolar,
                                                   id_tipo_contrato = x.id_tipo_contrato,
                                                   nm_mes_curso_inicial = x.nm_mes_curso_inicial,
                                                   nm_ano_curso_inicial = x.nm_ano_curso_inicial,
                                                   nm_mes_curso_final = x.nm_mes_curso_final,
                                                   nm_ano_curso_final = x.nm_ano_curso_final,
                                                   notas_material_didatico = x.notas_material_didatico,
                                                   CursoContrato = (from cc in x.CursoContrato
                                                       join c in db.Curso on cc.cd_curso equals c.cd_curso
                                                       //join p in db.PessoaSGF on cc.cd_pessoa_responsavel equals p.cd_pessoa
                                                       join t in db.TipoFinanceiro on cc.cd_tipo_financeiro equals t.cd_tipo_financeiro 
                                                       join v in db.vi_curso_ordem on cc.cd_curso equals v.cd_curso
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
                                                          no_pessoa_responsavel = (from a in db.PessoaSGF where a.cd_pessoa == cc.cd_pessoa_responsavel select a.no_pessoa).FirstOrDefault(), 
                                                              //cc.PessoaSGF.no_pessoa,
                                                          no_curso = c.no_curso, //cc.Curso.no_curso,
                                                          cd_proximo_curso = c.cd_proximo_curso,
                                                          no_tipo_financeiro = t.dc_tipo_financeiro, //cc.TipoFinanceiro
                                                          nm_mes_curso_inicial = cc.nm_mes_curso_inicial,
                                                          nm_ano_curso_inicial = cc.nm_ano_curso_inicial,
                                                          nm_mes_curso_final = cc.nm_mes_curso_final,
                                                          nm_ano_curso_final = cc.nm_ano_curso_final,
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

                                                     }).ToList()
                                               })).FirstOrDefault();

                //if (sql != null && sql.cd_contrato > 0)
                //    sql.valorSaldoMatricula = (from t in db.Titulo
                //                               where t.cd_origem_titulo == sql.cd_contrato && t.id_origem_titulo == cd_origem && !t.BaixaTitulo.Any()
                //                               select t.vl_titulo).Sum();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Contrato getContratoImpressao(int cd_escola, int cd_contrato)
        {
            Contrato contrato = new Contrato();

            try
            {
                contrato = ((from c in db.Contrato
                             where c.cd_pessoa_escola == cd_escola && c.cd_contrato == cd_contrato
                             select new
                             {
                                 cd_contrato = c.cd_contrato,
                                 vl_curso_contrato = c.vl_curso_contrato,
                                 nm_contrato = c.nm_contrato,
                                 nm_cliente_integracao = c.Escola.nm_cliente_integracao,
                                 cd_pessoa_escola = c.cd_pessoa_escola,
                                 no_pessoa_escola = c.Escola.no_pessoa,
                                 dc_reduzido_pessoa = c.Escola.dc_reduzido_pessoa,
                                 dc_num_cgc = c.Escola.dc_num_cgc,
                                 no_localidade = c.Escola.EnderecoPrincipal.Logradouro.no_localidade,
                                 no_tipo_logradouro_escola = c.Escola.EnderecoPrincipal.TipoLogradouro.sg_tipo_logradouro,
                                 dc_num_endereco = c.Escola.EnderecoPrincipal.dc_num_endereco,
                                 no_localidade_cidade = c.Escola.EnderecoPrincipal.Cidade.no_localidade,
                                 no_localidade_estado = c.Escola.EnderecoPrincipal.Estado.no_localidade,
                                 no_pessoa_responsavel = c.PessoaResponsavel.no_pessoa,
                                 nm_cpf_cgc = c.PessoaResponsavel.nm_natureza_pessoa == 1 ?//Pessoa Fisica
                                     db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().cd_pessoa_cpf != null ?
                                        db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().PessoaSGFQueUsoOCpf.nm_cpf :
                                             db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf :
                                             db.PessoaSGF.OfType<PessoaJuridicaSGF>().Where(pj => pj.cd_pessoa == c.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaJuridicaSGF>().dc_num_cgc,
                                 nm_doc_identidade = c.PessoaResponsavel.nm_natureza_pessoa == 1 ? //Pessoa Fisica
                                     db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_doc_identidade : "",
                                 nm_natureza_pessoa = c.PessoaResponsavel.nm_natureza_pessoa,
                                 dc_fone_mail = c.PessoaResponsavel.Telefone.dc_fone_mail,
                                 dc_email_responsavel = c.PessoaResponsavel.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL).FirstOrDefault().dc_fone_mail,
                                 dc_celular_responsavel = c.PessoaResponsavel.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.CELULAR).FirstOrDefault().dc_fone_mail,
                                 no_tipo_logradouro_responsavel = c.PessoaResponsavel.EnderecoPrincipal.TipoLogradouro.sg_tipo_logradouro,
                                 no_localidade_responsavel = c.PessoaResponsavel.EnderecoPrincipal.Logradouro.no_localidade,
                                 dc_num_endereco_responsavel = c.PessoaResponsavel.EnderecoPrincipal.dc_num_endereco,
                                 dc_compl_endereco_responsavel = c.PessoaResponsavel.EnderecoPrincipal.dc_compl_endereco,
                                 dc_num_cep_responsavel = c.PessoaResponsavel.EnderecoPrincipal.Logradouro.dc_num_cep,
                                 bairro_responsavel = c.PessoaResponsavel.EnderecoPrincipal.Bairro.no_localidade,
                                 no_cidade_responsavel = c.PessoaResponsavel.EnderecoPrincipal.Cidade.no_localidade,
                                 no_estado_responsavel = c.PessoaResponsavel.EnderecoPrincipal.Estado.Estado.sg_estado,
                                 nomeAluno = c.Aluno.AlunoPessoaFisica.no_pessoa,
                                 dc_fone_telefone_aluno = c.Aluno.AlunoPessoaFisica.Telefone.dc_fone_mail,
                                 dc_fone_celular_aluno = c.Aluno.AlunoPessoaFisica.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.CELULAR).FirstOrDefault().dc_fone_mail,
                                 dc_fone_email_aluno = c.Aluno.AlunoPessoaFisica.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL).FirstOrDefault().dc_fone_mail,
                                 nm_doc_identidade_aluno = c.Aluno.AlunoPessoaFisica.nm_doc_identidade,
                                 nm_cpf = c.Aluno.AlunoPessoaFisica.nm_cpf,
                                 nm_natureza_pessoa_aluno = c.Aluno.AlunoPessoaFisica.nm_natureza_pessoa,
                                 dc_estado_civil_fem = c.Aluno.AlunoPessoaFisica.EstadoCivil.dc_estado_civil_fem,
                                 dc_estado_civil_masc = c.Aluno.AlunoPessoaFisica.EstadoCivil.dc_estado_civil_masc,
                                 dt_nascimento = c.Aluno.AlunoPessoaFisica.dt_nascimento,
                                 dt_nasc_reponsavel = c.PessoaResponsavel.nm_natureza_pessoa == (int)PessoaSGF.TipoPessoa.FISICA ? db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().dt_nascimento : null,
                                 no_localidade_aluno = c.Aluno.AlunoPessoaFisica.EnderecoPrincipal.Logradouro.no_localidade,
                                 no_tipo_logradouro_aluno = c.Aluno.AlunoPessoaFisica.EnderecoPrincipal.TipoLogradouro.sg_tipo_logradouro,
                                 dc_num_endereco_aluno = c.Aluno.AlunoPessoaFisica.EnderecoPrincipal.dc_num_endereco,
                                 dc_compl_endereco_aluno = c.Aluno.AlunoPessoaFisica.EnderecoPrincipal.dc_compl_endereco,
                                 dc_num_cep_aluno = c.Aluno.AlunoPessoaFisica.EnderecoPrincipal.Logradouro.dc_num_cep,
                                 bairro_aluno = c.Aluno.AlunoPessoaFisica.EnderecoPrincipal.Bairro.no_localidade,
                                 no_cidade_aluno = c.Aluno.AlunoPessoaFisica.EnderecoPrincipal.Cidade.no_localidade,
                                 no_estado_aluno = c.Aluno.AlunoPessoaFisica.EnderecoPrincipal.Estado.Estado.sg_estado,
                                 //TelefonePessoa = c.Aluno.AlunoPessoaFisica.TelefonePessoa,
                                 no_curso = c.Curso.no_curso,
                                 no_produto = c.Produto.no_produto,
                                 dt_inicial_contrato = c.dt_inicial_contrato,
                                 c.dt_final_contrato,
                                 c.dt_matricula_contrato,
                                 vl_matricula_contrato = c.vl_matricula_contrato,
                                 nm_parcelas_mensalidade = c.nm_parcelas_mensalidade,
                                 nm_dia_vcto = c.nm_dia_vcto,
                                 nm_ano_vcto = c.nm_ano_vcto,
                                 nm_mes_vcto = c.nm_mes_vcto,
                                 dc_tipo_financeiro = c.TipoFinanceiro.dc_tipo_financeiro,
                                 dc_tipo_financeiro_taxa = c.TaxaMatricula.Select(x=> x.TipoFinanceiro.dc_tipo_financeiro).FirstOrDefault(),
                                 Aditamento = c.Aditamento,
                                 //AlunosTurma = c.AlunoTurma.Where(at => at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo),
                                 nm_sexo = c.Aluno.AlunoPessoaFisica.nm_sexo,
                                 pc_desconto_contrato = c.pc_desconto_contrato,
                                 DescontoContrato = c.DescontoContrato.Where(dc => dc.id_desconto_ativo),
                                 cd_aluno = c.cd_aluno,
                                 cd_turma = (int?)c.AlunoTurma.Where(at => at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                                           at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado).FirstOrDefault().cd_turma,
                                 dt_fim_turma = c.AlunoTurma.Where(at => at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                                         at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado).FirstOrDefault().Turma.dt_final_aula,
                                 dc_regime_turma = c.AlunoTurma.Where(at => at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                                         at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado).FirstOrDefault().Turma.Regime.no_regime,
                                 ultimoAditamento = c.Aditamento.OrderByDescending(x => x.dt_aditamento).FirstOrDefault(),
                                 c.id_tipo_matricula,
                                 c.vl_liquido_contrato,
                                 c.vl_parcela_contrato,
                                 c.id_incorporar_valor_material,
                                 c.id_valor_incluso,
                                 c.nm_parcelas_material,
                                 c.vl_material_contrato,
                                 c.vl_parcela_material,
                                 c.vl_parcela_liq_material,
                                 c.pc_bolsa_material
                             }).ToList().Select(x => new Contrato
                             {
                                 Escola = new Escola
                                 {
                                     no_pessoa = x.no_pessoa_escola,
                                     dc_reduzido_pessoa = x.dc_reduzido_pessoa,
                                     dc_num_cgc = x.dc_num_cgc,
                                     nm_cliente_integracao = x.nm_cliente_integracao,
                                     EnderecoPrincipal = new EnderecoSGF
                                     {
                                         Logradouro = new LocalidadeSGF { no_localidade = x.no_localidade },
                                         dc_num_endereco = x.dc_num_endereco,
                                         Cidade = new LocalidadeSGF { no_localidade = x.no_localidade_cidade },
                                         Estado = new LocalidadeSGF { no_localidade = x.no_localidade_estado },
                                         TipoLogradouro = new TipoLogradouroSGF { no_tipo_logradouro = x.no_tipo_logradouro_escola }
                                     }
                                 },
                                 PessoaResponsavel = new PessoaSGF
                                 {
                                     no_pessoa = x.no_pessoa_responsavel,
                                     nm_cpf_cgc = x.nm_cpf_cgc,
                                     nm_doc_identidade = x.nm_doc_identidade,
                                     nm_natureza_pessoa = x.nm_natureza_pessoa,
                                     Telefone = new TelefoneSGF { dc_fone_mail = x.dc_fone_mail },
                                     EnderecoPrincipal = new EnderecoSGF
                                     {
                                         Logradouro = new LocalidadeSGF { no_localidade = x.no_localidade_responsavel },
                                         dc_num_endereco = x.dc_num_endereco_responsavel,
                                         dc_compl_endereco = x.dc_compl_endereco_responsavel,
                                         num_cep = x.dc_num_cep_responsavel,
                                         Bairro = new LocalidadeSGF { no_localidade = x.bairro_responsavel },
                                         TipoLogradouro = new TipoLogradouroSGF { no_tipo_logradouro = x.no_tipo_logradouro_responsavel },
                                         Cidade = new LocalidadeSGF { no_localidade = x.no_cidade_responsavel },
                                         Estado = new LocalidadeSGF { no_localidade = x.no_estado_responsavel }
                                     }
                                 },
                                 Aluno = new Aluno
                                 {
                                     AlunoPessoaFisica = new PessoaFisicaSGF
                                     {
                                         no_pessoa = x.nomeAluno,
                                         Telefone = new TelefoneSGF { dc_fone_mail = x.dc_fone_telefone_aluno },
                                         email = x.dc_fone_email_aluno,
                                         celular = x.dc_fone_celular_aluno,
                                         nm_doc_identidade = x.nm_doc_identidade_aluno,
                                         nm_cpf = x.nm_cpf,
                                         nm_sexo = x.nm_sexo,
                                         nm_natureza_pessoa = x.nm_natureza_pessoa_aluno,
                                         EstadoCivil = new EstadoCivilSGF
                                         {
                                             dc_estado_civil_fem = x.dc_estado_civil_fem,
                                             dc_estado_civil_masc = x.dc_estado_civil_masc
                                         },
                                         dt_nascimento = x.dt_nascimento,
                                         EnderecoPrincipal = new EnderecoSGF
                                         {
                                             Logradouro = new LocalidadeSGF { no_localidade = x.no_localidade_aluno },
                                             dc_num_endereco = x.dc_num_endereco_aluno,
                                             dc_compl_endereco = x.dc_compl_endereco_aluno,
                                             num_cep = x.dc_num_cep_aluno,
                                             Bairro = new LocalidadeSGF { no_localidade = x.bairro_aluno },
                                             TipoLogradouro = new TipoLogradouroSGF { no_tipo_logradouro = x.no_tipo_logradouro_aluno },
                                             Cidade = new LocalidadeSGF { no_localidade = x.no_cidade_aluno },
                                             Estado = new LocalidadeSGF { no_localidade = x.no_estado_aluno }
                                         }//,
                                         //TelefonePessoa = x.TelefonePessoa
                                     }
                                 },
                                 nm_contrato = x.nm_contrato,
                                 vl_curso_contrato = x.vl_curso_contrato,
                                 cd_pessoa_escola = x.cd_pessoa_escola,
                                 cd_contrato = x.cd_contrato,
                                 Curso = new Curso { no_curso = x.no_curso },
                                 Produto = new Produto { no_produto = x.no_produto },
                                 dt_inicial_contrato = x.dt_inicial_contrato,
                                 dt_final_contrato = x.dt_final_contrato,
                                 dt_matricula_contrato = x.dt_matricula_contrato,
                                 vl_matricula_contrato = x.vl_matricula_contrato,
                                 nm_parcelas_mensalidade = x.nm_parcelas_mensalidade,
                                 nm_dia_vcto = x.nm_dia_vcto,
                                 nm_ano_vcto = x.nm_ano_vcto,
                                 nm_mes_vcto = x.nm_mes_vcto,
                                 TipoFinanceiro = new TipoFinanceiro { dc_tipo_financeiro = x.dc_tipo_financeiro },
                                 //Aditamento = x.Aditamento,
                                 //Aluno  = x.AlunosTurma.ToList(),
                                 pc_desconto_contrato = x.pc_desconto_contrato,
                                 DescontoContrato = x.DescontoContrato.ToList(),
                                 e_mail_responsavel = x.dc_email_responsavel,
                                 celular_responsavel = x.dc_celular_responsavel,
                                 cd_aluno = x.cd_aluno,
                                 dt_fim_turma = x.dt_fim_turma,
                                 aditamentoMaxData = x.ultimoAditamento,
                                 cd_turma_atual = x.cd_turma,
                                 dt_nasc_responsavel = x.dt_nasc_reponsavel,
                                 dc_tipo_financeiro_taxa = x.dc_tipo_financeiro_taxa,
                                 dc_regime_turma = x.dc_regime_turma,
                                 dc_tipo_matricula = x.id_tipo_matricula == 1 ? "Matrícula" : "Rematrícula",
                                 vl_liquido_contrato = x.vl_liquido_contrato,
                                 vl_parcela_contrato = x.vl_parcela_contrato,
                                 id_incorporar_valor_material = x.id_incorporar_valor_material,
                                 id_valor_incluso = x.id_valor_incluso,
                                 nm_parcelas_material = x.nm_parcelas_material,
                                 vl_material_contrato = x.vl_material_contrato,
                                 vl_parcela_material = x.vl_parcela_material,
                                 vl_parcela_liq_material = x.vl_parcela_liq_material,
                                 pc_bolsa_material = x.pc_bolsa_material
                             })).FirstOrDefault();
                if (contrato != null)
                {
                    if (contrato.aditamentoMaxData != null)
                    {
                        contrato.Aditamento = new List<Aditamento>();
                        contrato.Aditamento.Add(contrato.aditamentoMaxData);
                    }
                    if (contrato.cd_turma_atual.HasValue)
                    {
                        contrato.AlunoTurma = new List<AlunoTurma>();
                        contrato.AlunoTurma.Add(new AlunoTurma { cd_turma = (int)contrato.cd_turma_atual });
                    }
                }

                return contrato;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Contrato getContratoBaixa(int cd_escola, int cd_contrato)
        {
            Contrato contrato = new Contrato();
            try
            {

                contrato = ((from c in db.Contrato
                             where c.cd_pessoa_escola == cd_escola && c.cd_contrato == cd_contrato
                             select new
                             {
                                 AlunosTurma = c.AlunoTurma.Where(at => (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo) ||
                                                                         at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                                         at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Encerrado),
                                 //DescontoContrato = c.DescontoContrato.Where(dc => dc.id_desconto_ativo),
                                 cd_aluno = c.cd_aluno,
                                 cd_pessoa_escola = c.cd_pessoa_escola,
                                 ultimoAditamento = c.Aditamento.OrderByDescending(x => x.dt_aditamento).FirstOrDefault()
                             }).ToList().Select(x => new Contrato
                             {
                                 AlunoTurma = x.AlunosTurma.ToList(),
                                 //DescontoContrato = x.DescontoContrato.ToList(),
                                 cd_aluno = x.cd_aluno,
                                 cd_pessoa_escola = x.cd_pessoa_escola,
                                 aditamentoMaxData = x.ultimoAditamento
                             })).FirstOrDefault();

                if (contrato != null && contrato.cd_aluno > 0)
                {
                    List<DescontoContrato> descontos = new List<DescontoContrato>();
                    if (db.Aditamento.Any(aditamento => aditamento.cd_contrato == cd_contrato && aditamento.Contrato.cd_pessoa_escola == cd_escola &&
                                          aditamento.id_tipo_aditamento != null && (aditamento.id_tipo_aditamento == (int)Aditamento.TipoAditamento.CONCESSAO_DESCONTO ||
                                          aditamento.id_tipo_aditamento == (int)Aditamento.TipoAditamento.PERDA_DESCONTO)))
                        descontos = (from d in db.DescontoContrato
                                     where d.Aditamento.Contrato.cd_pessoa_escola == cd_escola &&
                                           d.cd_aditamento == db.Aditamento.Where(aditamento => aditamento.cd_contrato == cd_contrato && aditamento.Contrato.cd_pessoa_escola == cd_escola &&
                                                                                  aditamento.id_tipo_aditamento != null && (aditamento.id_tipo_aditamento == (int)Aditamento.TipoAditamento.CONCESSAO_DESCONTO ||
                                                                                                                            aditamento.id_tipo_aditamento == (int)Aditamento.TipoAditamento.PERDA_DESCONTO))
                                                                                  .Max(x => x.cd_aditamento)
                                     select d).ToList();
                    else
                        if (descontos == null || descontos.Count() <= 0)
                            descontos = (from d in db.DescontoContrato
                                         where d.cd_contrato == cd_contrato && d.Contratos.cd_pessoa_escola == cd_escola
                                         select d).ToList();
                    contrato.DescontoContrato = descontos;
                    if (contrato.aditamentoMaxData != null)
                    {
                        contrato.Aditamento = new List<Aditamento>();
                        contrato.Aditamento.Add(contrato.aditamentoMaxData);
                    }
                }
                return contrato;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool deleteAll(List<Contrato> contratos)
        {
            try
            {
                string strContratos = "";
                if (contratos != null && contratos.Count > 0)
                    foreach (Contrato e in contratos)
                        strContratos += e.cd_contrato + ",";

                // Remove o último ponto e virgula:
                if (strContratos.Length > 0)
                    strContratos = strContratos.Substring(0, strContratos.Length - 1);

                int retorno = db.Database.ExecuteSqlCommand("delete from t_contrato where cd_contrato in(" + strContratos + ")");

                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public int getUltimoNroMatricula(int? nm_ultimo_matricula, int cd_escola)
        {
            try
            {
                int ultimo = 0;
                int? ultima_matricula = null;

                if (nm_ultimo_matricula.HasValue)
                {

                    ultima_matricula = (from c in db.Contrato
                                        where c.nm_matricula_contrato == nm_ultimo_matricula.Value
                                        && c.cd_pessoa_escola == cd_escola
                                        select c.nm_matricula_contrato).FirstOrDefault();

                    if (ultima_matricula.HasValue)
                        //Pesquisa a primeira matrícula que possui número de matrícula maior ou igual que a do parâmetro, mas que não tenha a próxima matrícula (nro + 1):
                        ultima_matricula = (from c in db.Contrato
                                            where !(from c2 in db.Contrato
                                                    where c2.nm_matricula_contrato == c.nm_matricula_contrato + 1
                                                    && c2.cd_pessoa_escola == cd_escola
                                                    select c2.nm_matricula_contrato).Any()
                                            where c.nm_matricula_contrato >= nm_ultimo_matricula.Value - 1
                                            && c.cd_pessoa_escola == cd_escola
                                            select c.nm_matricula_contrato).Min();
                    else
                        ultima_matricula = nm_ultimo_matricula - 1;
                }
                else
                    ultima_matricula = (from c in db.Contrato
                                        where c.cd_pessoa_escola == cd_escola
                                        select c.nm_matricula_contrato).Max();
                if (ultima_matricula.HasValue)
                    ultimo = ultima_matricula.Value;
                return ultimo;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool getVerificarNroContrato(int cd_empresa, int nm_contrato)
        {
            SGFWebContext dbComp = new SGFWebContext();
            int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                 
            bool retorno = (from c in db.Contrato
                            where db.Movimento.Where(m => m.id_origem_movimento == cd_origem && m.cd_origem_movimento == c.cd_contrato).Any()
                                && c.nm_contrato == nm_contrato
                                && c.cd_pessoa_escola == cd_empresa
                               select c.nm_contrato).Any();

            return retorno;
        }

        public int getNroUltimoContrato(int? nm_ultimo_contrato, int cd_escola)
        {
            try
            {
                int ultimo = 0;
                int? ultimo_contrato = null;

                if (nm_ultimo_contrato.HasValue)
                {
                    ultimo_contrato = (from c in db.Contrato
                                       where c.nm_contrato == nm_ultimo_contrato.Value
                                        && c.cd_pessoa_escola == cd_escola
                                       select c.nm_contrato).FirstOrDefault();

                    if (ultimo_contrato.HasValue)
                        ultimo_contrato = (from c in db.Contrato
                                           where !(from c2 in db.Contrato
                                                   where c2.nm_contrato == c.nm_contrato + 1
                                                   && c2.cd_pessoa_escola == cd_escola
                                                   select c2.nm_contrato).Any()
                                           where c.nm_contrato >= nm_ultimo_contrato.Value - 1
                                           && c.cd_pessoa_escola == cd_escola
                                           select c.nm_contrato).Min();
                    else
                        ultimo = nm_ultimo_contrato.Value - 1;
                }
                else
                    ultimo_contrato = (from c in db.Contrato
                                       where c.cd_pessoa_escola == cd_escola
                                       select c.nm_contrato).Max();
                if (ultimo_contrato.HasValue)
                    ultimo = ultimo_contrato.Value;

                return ultimo;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Contrato getMatriculaByTurmaAluno(int cdTurma, int cdAluno)
        {
            try
            {

                Contrato sql = (from c in db.Contrato
                                where c.AlunoTurma.Where(at => at.cd_aluno == cdAluno && at.cd_turma == cdTurma).Any()
                                select c).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        // Usada no cadastro de turma ao chamar para nova matricula
        // LBM TODO Verificar t_curso_contrato
        public bool existeMatriculaByProduto(int produto, int cdAluno, DateTime? dt_inicio_aula, int curso, bool id_turma_ppt, int cd_contrato, DateTime? dt_final_aula, int? cd_duracao)
        {
            try
            {
                bool id_permitir_matricula = (from a in db.Curso where a.cd_curso == curso select a.id_permitir_matricula).FirstOrDefault();
                short carga_horaria = (short)(from a in db.Curso where a.cd_curso == curso select a.nm_carga_horaria).FirstOrDefault();
                decimal nmDuracao = (from a in db.Duracao where a.cd_duracao == cd_duracao select a.nm_duracao).FirstOrDefault();
                if (dt_final_aula == null) dt_final_aula = dt_inicio_aula.Value.AddDays(nmDuracao == 0 ? 0 : (int)(carga_horaria / nmDuracao) * 7);

                if (!id_permitir_matricula)
                {
                    //verifica se existe aluno matriculado ou rematriculado com o produto 
                    Contrato sql = (from c in db.Contrato
                                    join cc in db.CursoContrato on c.cd_contrato equals cc.cd_contrato
                                where c.cd_aluno == cdAluno &&
                                      c.cd_contrato != cd_contrato &&
                                       c.cd_produto_atual == produto &&
                                      !cc.Curso.id_permitir_matricula &&
                                      c.AlunoTurma.Where(at => (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                         at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado) &&
                                                         !at.Turma.Curso.id_permitir_matricula &&
                                                            (
                                                                dt_inicio_aula == null || 
                                                                //Regulares
                                                                ((at.Turma.cd_turma_ppt == null) &&
                                                                    (
                                                                        (at.Turma.cd_curso == curso) ||
                                                                         (!at.Turma.dt_final_aula.HasValue && dt_inicio_aula <= DbFunctions.AddDays(at.Turma.dt_inicio_aula, at.Turma.Duracao.nm_duracao == 0 ? 0 : (int)(at.Turma.Curso.nm_carga_horaria / (double)at.Turma.Duracao.nm_duracao * 7.0)) ||
                                                                           at.Turma.dt_final_aula.HasValue && dt_inicio_aula <= at.Turma.dt_final_aula) &&
                                                                         (dt_final_aula > at.Turma.dt_inicio_aula) 
                                                                    )
                                                                ) ||
                                                                //Personalizadas
                                                                ((at.Turma.cd_turma_ppt != null && id_turma_ppt) &&
                                                                    (
                                                                        ((at.Turma.cd_curso != curso) &
                                                                            (!at.Turma.dt_final_aula.HasValue && dt_inicio_aula <= DbFunctions.AddDays(at.Turma.dt_inicio_aula, at.Turma.Duracao.nm_duracao == 0 ? 0 : (int)(at.Turma.Curso.nm_carga_horaria / (double)at.Turma.Duracao.nm_duracao * 7.0)) ||
                                                                            at.Turma.dt_final_aula.HasValue && dt_inicio_aula <= at.Turma.dt_final_aula) &&
                                                                            (dt_final_aula > at.Turma.dt_inicio_aula)) ||
                                                                        ((at.Turma.cd_curso == curso) &
                                                                            (!at.Turma.dt_final_aula.HasValue && dt_inicio_aula > DbFunctions.AddDays(at.Turma.dt_inicio_aula, at.Turma.Duracao.nm_duracao == 0 ? 0 : (int)(at.Turma.Curso.nm_carga_horaria / (double)at.Turma.Duracao.nm_duracao * 7.0)) ||
                                                                            at.Turma.dt_final_aula.HasValue && dt_inicio_aula > at.Turma.dt_final_aula) &&
                                                                            (dt_final_aula <= at.Turma.dt_inicio_aula )) 
                                                                    )
                                                                )
                                                            )
                                                         ).Any()
                                select c).FirstOrDefault();
                    return (sql != null && sql.cd_contrato > 0);
                }
                else
                    return(false);
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Contrato findMatriculaByTurmaAluno(int cdTurma, int cdAluno)
        {
            try
            {
                Contrato sql = (from c in db.Contrato
                                where c.AlunoTurma.Where(at => at.cd_aluno == cdAluno && at.cd_turma == cdTurma).Any()
                                select new
                                {
                                    cd_contrato = c.cd_contrato,
                                    cd_produto = c.cd_produto_atual,
                                    cd_aluno = c.cd_aluno,
                                    cd_situacao_aluno_turma = c.AlunoTurma.Where(at => at.cd_aluno == cdAluno && at.cd_turma == cdTurma).Select(a => a.cd_situacao_aluno_turma).FirstOrDefault()
                                }).ToList().Select(x => new Contrato
                                {
                                    cd_contrato = x.cd_contrato,
                                    cd_produto_atual = x.cd_produto,
                                    cd_aluno = x.cd_aluno,
                                    cd_situacao_aluno_turma = x.cd_situacao_aluno_turma
                                }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public int? getMatriculaAluno(int cd_aluno, int cd_escola)
        {
            try
            {
                var ultima_matricula = (from c in db.Contrato
                                        where c.cd_pessoa_escola == cd_escola
                                        && c.cd_aluno == cd_aluno
                                        orderby c.dt_inicial_contrato descending
                                        select c.nm_matricula_contrato).Max();
                return ultima_matricula;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Contrato getMatriculaByIdVI(int id, int cdEscola)
        {
            try
            {
                vi_contrato sql = (from c in db.vi_contrato
                                   where
                                       c.cd_contrato == id &&
                                       c.cd_pessoa_escola == cdEscola
                                   select c).FirstOrDefault();
                Contrato retorno = new Contrato();
                retorno.copy(sql);
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public DocumentoDigitalizadoEditUI getDocumentoDigitalizadoUpdated(int id, int cdEscola)
        {
            try
            {
                DocumentoDigitalizadoEditUI sql = (from c in db.Contrato
                    where
                        c.cd_contrato == id &&
                        c.cd_pessoa_escola == cdEscola
                    select new
                    {
                        cd_contrato = c.cd_contrato,
                        nm_arquivo_digitalizado = c.nm_arquivo_digitalizado,
                        cd_pessoa_escola = c.cd_pessoa_escola,
                        nm_contrato = c.nm_contrato
                    }).ToList().Select(x => new DocumentoDigitalizadoEditUI
                        {
                            cd_contrato = x.cd_contrato,
                            nm_arquivo_digitalizado = x.nm_arquivo_digitalizado,
                            cd_pessoa_escola = x.cd_pessoa_escola,
                            nm_contrato = x.nm_contrato
                        }).FirstOrDefault();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<Contrato> getMatriculasAluno(int cd_aluno, int cd_escola)
        {
            try
            {

                var contratos = (from c in db.Contrato
                                 where c.cd_pessoa_escola == cd_escola && c.cd_aluno == cd_aluno
                                 select new
                                 {
                                     cd_contrato = c.cd_contrato,
                                     nm_contrato = c.nm_contrato,
                                     no_produto = c.Produto.no_produto,
                                     no_curso = c.Curso.no_curso,
                                     dt_matricula = c.dt_matricula_contrato,
                                     dc_ano_escolar = c.AnoEscolar.dc_ano_escolar,
                                     c.AnoEscolar.Escolaridade.no_escolaridade
                                 }).ToList().Select(x => new Contrato
                                 {
                                     cd_contrato = x.cd_contrato,
                                     nm_contrato = x.nm_contrato,
                                     no_produto = x.no_produto,
                                     no_curso = x.no_curso,
                                     dt_matricula_contrato = x.dt_matricula,
                                     dc_ano_escolar = !string.IsNullOrEmpty(x.no_escolaridade) && !string.IsNullOrEmpty(x.dc_ano_escolar) ? x.no_escolaridade + " - " + x.dc_ano_escolar :
                                                         !string.IsNullOrEmpty(x.dc_ano_escolar) ? x.dc_ano_escolar : ""
                                                       
                                 });


                return contratos.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        // LBM TODO:t_curso_contrato
        // Esta função procura por contratos sem turma é usada no postMatricula. Se for verdadeira gera erro
        // Se encontrar uma com curso simultâneo está liberado
        // Na data de inicio deverá ser passada a data de inicio do contrato para a devida verificação de data
        // A data final será calculada baseada nas cargas horárias do curso e turma
        public bool existeMatriculaByProdutoAluno(int produto, int cdAluno, int cdEscola, DateTime? dt_inicio_aula, int cd_curso, int cd_contrato, DateTime? dt_final_aula, int? cd_duracao)
//        public bool existeMatriculaByProdutoAluno(int produto, int cdAluno, int cdEscola)
        {
            try
            {
                bool id_permitir_matricula = (from a in db.Curso where a.cd_curso == cd_curso select a.id_permitir_matricula).FirstOrDefault();
                short carga_horaria = (short)(from a in db.Curso where a.cd_curso == cd_curso select a.nm_carga_horaria).FirstOrDefault();
                decimal nmDuracao = (from a in db.Duracao where a.cd_duracao == cd_duracao select a.nm_duracao).FirstOrDefault();
                if (dt_final_aula == null) dt_final_aula = dt_inicio_aula.Value.AddDays(nmDuracao==0 ? 0: (int)(carga_horaria/nmDuracao)*7);

                if (!id_permitir_matricula)
                {
                    Contrato sql = (from c in db.Contrato
                                    join cc in db.CursoContrato on c.cd_contrato equals cc.cd_contrato
                                    where c.cd_contrato != cd_contrato && 
                                          c.cd_aluno == cdAluno &&
                                          c.cd_produto_atual == produto &&
                                          c.cd_pessoa_escola == cdEscola &&
                                          !cc.Curso.id_permitir_matricula &&
                                          (dt_inicio_aula == null || cd_curso == cc.cd_curso ||
                                          (cd_curso != cc.cd_curso &&
                                           ((dt_inicio_aula <= DbFunctions.AddDays(c.dt_inicial_contrato, c.Duracao.nm_duracao == 0 ? 0 : (int)(cc.Curso.nm_carga_horaria / (double)c.Duracao.nm_duracao * 7.0))) &&
                                            (dt_final_aula > c.dt_inicial_contrato))
                                           )) &&
                                          !c.HistoricosAluno.Any()
                                    select c).FirstOrDefault();
                    return (sql != null && sql.cd_contrato > 0);
                }
                else
                    return (false);
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Contrato getMatriculaCnabBoleto(int cd_empresa, int cd_contrato)
        {
            try
            {

                var contratos = (from c in db.Contrato
                                 where c.cd_pessoa_escola == cd_empresa && c.cd_contrato == cd_contrato
                                 select new
                                 {
                                     cd_contrato = c.cd_contrato,
                                     nm_contrato = c.nm_contrato,
                                     dt_matricula = c.dt_matricula_contrato
                                 }).ToList().Select(x => new Contrato
                                 {
                                     cd_contrato = x.cd_contrato,
                                     nm_contrato = x.nm_contrato,
                                     dt_matricula_contrato = x.dt_matricula
                                 }).FirstOrDefault();
                return contratos;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public IEnumerable<MatriculaRel> getMatriculaAnalitico(int cd_empresa, int cd_turma, int cd_aluno, List<int> situacaoAlunoTurma, bool semTurma, bool tranferencia, bool retorno, int situacaoContrato, DateTime? dtIni, DateTime? dtFim, int cdAtendente, bool bolsaCem, bool contratoDigitalizado, int? cdProduto, string noProduto, bool exibirEnderecos, int vinculado)
        {
            try
            {
                int id_origem_movimento = Int32.Parse(db.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                var contratos = from c in db.Contrato
                                where c.cd_pessoa_escola == cd_empresa
                                select c;
                if (cd_turma > 0)
                    contratos = from c in contratos
                                where c.AlunoTurma.Where(t => t.cd_turma == cd_turma).Any()
                                select c;

                if (cd_aluno > 0)
                    contratos = from c in contratos
                                where c.cd_aluno == cd_aluno
                                select c;

                if (situacaoAlunoTurma.Count() > 0 && !situacaoAlunoTurma.Contains(100))
                    contratos = from c in contratos
                                where situacaoAlunoTurma.Contains(c.HistoricosAluno.Where(histA => histA.Aluno.cd_pessoa_escola == cd_empresa && histA.cd_aluno == c.cd_aluno &&
                                    histA.cd_contrato == c.cd_contrato && (cd_turma == 0 || histA.cd_turma == cd_turma)).OrderByDescending(n => n.nm_sequencia).FirstOrDefault().id_situacao_historico)
                                select c;

                if (semTurma)
                    contratos = from c in contratos
                                where !c.AlunoTurma.Any()
                                select c;

                if (tranferencia)
                    contratos = from c in contratos
                                where c.id_transferencia == true
                                select c;

                if (bolsaCem == true)
                    contratos = from c in contratos
                                where c.pc_desconto_bolsa < 100
                                select c;

                if (retorno)
                    contratos = from c in contratos
                                where c.id_retorno == true
                                select c;

                if (situacaoContrato > 0)
                    contratos = from c in contratos
                                where c.id_tipo_matricula == situacaoContrato
                                select c;

                if (dtIni.HasValue)
                    contratos = from c in contratos
                                where c.dt_matricula_contrato >= dtIni.Value
                                select c;

                if (dtFim.HasValue)
                    contratos = from c in contratos
                                where c.dt_matricula_contrato <= dtFim.Value
                                select c;

                if (contratoDigitalizado)
                    contratos = from c in contratos
                        where c.nm_arquivo_digitalizado != null && 
                              c.nm_arquivo_digitalizado != ""
                        select c;

                if (cdProduto != null)
                {
                    if (cdProduto > 0)
                    {
                        contratos = from c in contratos
                                    where c.cd_produto_atual == cdProduto
                                    select c;
                    }
                }

                if (exibirEnderecos == true)
                {
                    contratos = from c in contratos
                                where c.Aluno.AlunoPessoaFisica.EnderecoPrincipal != null
                                select c;
                }

                if (vinculado > 0)
                {
                    if (vinculado == 1)
                    {
                        contratos = from c in contratos
                                    where (from m in db.Movimento
                                           where m.cd_origem_movimento == c.cd_contrato &&
                                                 m.id_origem_movimento == id_origem_movimento &&
                                                 m.id_material_didatico == true &&
                                                 !m.id_venda_futura
                                           select m).Any()
                                    select c;
                    }
                    else
                    if (vinculado == 2)
                    {
                        contratos = from c in contratos
                                    where !(from m in db.Movimento
                                            where m.cd_origem_movimento == c.cd_contrato &&
                                                  m.id_origem_movimento == id_origem_movimento &&
                                                  m.id_material_didatico == true &&
                                                  !m.id_venda_futura
                                            select m).Any()
                                    select c;
                    }
                    else
                    {
                        contratos = from c in contratos
                                    join ct in db.CursoContrato on c.cd_contrato equals ct.cd_contrato
                                    join at in db.AlunoTurma on c.cd_contrato equals at.cd_contrato
                                    join co in db.Contrato on c.cd_contrato equals co.cd_contrato
                                    where (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                           at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado) &&
                                          !(from m in db.Movimento
                                            where m.cd_origem_movimento == c.cd_contrato &&
                                                  m.id_origem_movimento == id_origem_movimento &&
                                                  m.id_material_didatico == true &&
                                                  !m.id_venda_futura
                                            select m).Any() &&
                                          (from m in db.Movimento
                                           where m.cd_pessoa_empresa == c.cd_pessoa_escola &&
                                                 m.id_material_didatico == true &&
                                                 !m.id_venda_futura &&
                                                 m.id_origem_movimento == null &&
                                                 (m.cd_aluno == at.cd_aluno || m.cd_pessoa == co.cd_pessoa_responsavel) &&
                                                 m.ItensMovimento.Where(
                                                     im => im.Item.cd_tipo_item == (int)TipoItem.TipoItemEnum.MATERIAL_DIDATICO &&
                                                           im.Item.id_material_didatico == true &&
                                                           im.Item.Cursos.Where(ic => ic.cd_curso == ct.cd_curso).Any()
                                                 ).Any()
                                           select m).Any()

                                    select c;
                    }

                }

                string nm_atendente = "";
                if (cdAtendente > 0)
                {
                    contratos = from c in contratos
                                where c.cd_usuario == cdAtendente
                                select c;

                    var cd_pessoa = db.UsuarioWebSGF.Where(x => x.cd_usuario == cdAtendente).FirstOrDefault().cd_pessoa;
                    if (cd_pessoa != null)
                        nm_atendente = db.PessoaSGF.Where(x => x.cd_pessoa == cd_pessoa).FirstOrDefault().no_pessoa;
                }


                

                IEnumerable<MatriculaRel> sql = (from c in contratos
                                                 select new
                                               {
                                                   no_atendente = nm_atendente,
                                                   cd_produto_atual = c.cd_produto_atual,
                                                   no_produto = c.Produto.no_produto,
                                                   cd_curso_atual = c.cd_curso_atual,
                                                   no_curso = c.Curso.no_curso,
                                                   cd_duracao_atual = c.cd_duracao_atual,
                                                   dc_duracao = c.Duracao.dc_duracao,
                                                   cd_regime_atual = c.cd_regime_atual,
                                                   no_regime = c.Regime != null ? c.Regime.no_regime : null,
                                                   cd_aluno = c.cd_aluno,
                                                   dt_matricula_contrato = situacaoAlunoTurma.Contains((int)AlunoTurma.SituacaoAlunoTurma.Desistente) ? db.Desistencia.Where(d => c.AlunoTurma.Where(at => at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Desistente && at.cd_contrato == c.cd_contrato && (cd_turma == 0 || at.cd_turma == cd_turma)).FirstOrDefault().cd_aluno_turma == d.cd_aluno_turma && d.id_tipo_desistencia == 1).FirstOrDefault().dt_desistencia :
                                                                           (situacaoAlunoTurma.Contains((int)AlunoTurma.SituacaoAlunoTurma.Encerrado) || situacaoAlunoTurma.Contains((int)AlunoTurma.SituacaoAlunoTurma.Movido)) && c.AlunoTurma.Any() ? c.HistoricosAluno.Where(histA => histA.Turma.cd_pessoa_escola == cd_empresa && histA.cd_aluno == c.cd_aluno && (cd_turma == 0 || histA.cd_turma == cd_turma)).OrderByDescending(o => o.nm_sequencia).FirstOrDefault().dt_historico
                                                                           : c.dt_matricula_contrato,
                                                   cd_turma = c.AlunoTurma.Any() ? (c.HistoricosAluno.Where(histA => histA.Aluno.cd_pessoa_escola == cd_empresa && histA.cd_aluno == c.cd_aluno && (cd_turma == 0 || histA.cd_turma == cd_turma)).OrderByDescending(o => o.nm_sequencia).FirstOrDefault() != null) ? c.HistoricosAluno.Where(histA => histA.Aluno.cd_pessoa_escola == cd_empresa && histA.cd_aluno == c.cd_aluno && (cd_turma == 0 || histA.cd_turma == cd_turma)).OrderByDescending(o => o.nm_sequencia).FirstOrDefault().cd_turma : 0 : 0,
                                                   no_turma = c.HistoricosAluno.Where(histA => histA.Aluno.cd_pessoa_escola == cd_empresa && histA.cd_aluno == c.cd_aluno && (cd_turma == 0 || histA.cd_turma == cd_turma)).OrderByDescending(o => o.nm_sequencia).FirstOrDefault().Turma.no_turma,
                                                   no_apelido = c.AlunoTurma.Any() ? c.HistoricosAluno.Where(histA => histA.Aluno.cd_pessoa_escola == cd_empresa && histA.cd_aluno == c.cd_aluno && (cd_turma == 0 || histA.cd_turma == cd_turma)).OrderByDescending(o => o.nm_sequencia).FirstOrDefault().Turma.no_apelido : "",
                                                   telefone = c.Aluno.AlunoPessoaFisica.Telefone.dc_fone_mail,
                                                   no_pessoa = c.Aluno.AlunoPessoaFisica.no_pessoa,
                                                   nm_contrato = c.nm_contrato,
                                                   nm_parcelas_mensalidade = c.nm_parcelas_mensalidade,
                                                   nm_dia_vcto = c.nm_dia_vcto,
                                                   vl_parcela_contrato = c.vl_parcela_contrato,
                                                   motivo_desistencia = situacaoAlunoTurma.Contains((int)AlunoTurma.SituacaoAlunoTurma.Desistente) && c.AlunoTurma.Any() ?
                                                                            db.MotivoDesistencia.Where(m => m.Desistencia.Where(d => c.AlunoTurma.Where(at => at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Desistente && at.cd_contrato == c.cd_contrato && (cd_turma == 0 || at.cd_turma == cd_turma)).FirstOrDefault().cd_aluno_turma == d.cd_aluno_turma && d.id_tipo_desistencia == 1).FirstOrDefault().cd_motivo_desistencia == m.cd_motivo_desistencia).FirstOrDefault().dc_motivo_desistencia : "",
                                                   no_professor = situacaoAlunoTurma.Contains((int)AlunoTurma.SituacaoAlunoTurma.Desistente) ?
                                                                           db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(p => p.PessoaFisicaFuncioanrio.Where(f => c.HistoricosAluno.Where(h => h.Aluno.cd_pessoa_escola == cd_empresa && (cd_turma == 0 || h.cd_turma == cd_turma) && h.cd_contrato == c.cd_contrato &&
                                                                               h.cd_aluno == c.cd_aluno && h.Turma.TurmaProfessorTurma.Where(t => t.id_professor_ativo && f.cd_funcionario == t.cd_professor).Any()).OrderByDescending(o => o.nm_sequencia).Any()).Any()).FirstOrDefault().dc_reduzido_pessoa : "",
                                                   //Aditamento = c.Aditamento.Where(x => x.id_tipo_aditamento == null && x.vl_material_contrato > 0).OrderByDescending(x=>x.dt_aditamento).FirstOrDefault()
                                                   nm_parcelas_material = c.nm_parcelas_material,
                                                   contratoDigitalizado = (c.nm_arquivo_digitalizado != null) ? (c.nm_arquivo_digitalizado != "" ) ? true : false : false,
                                                   vl_material_contrato = c.vl_material_contrato == null ? 0 : c.vl_material_contrato,
                                                   enderecoAluno =
                                                     c.Aluno.AlunoPessoaFisica.EnderecoPrincipal == null ? "" :
                                                     c.Aluno.AlunoPessoaFisica.EnderecoPrincipal.Logradouro.no_localidade == null ? "" :
                                                     c.Aluno.AlunoPessoaFisica.EnderecoPrincipal.TipoLogradouro.no_tipo_logradouro == null ? "" :
                                                     c.Aluno.AlunoPessoaFisica.EnderecoPrincipal.TipoLogradouro.no_tipo_logradouro + " " + c.Aluno.AlunoPessoaFisica.EnderecoPrincipal.Logradouro.no_localidade +
                                                     (c.Aluno.AlunoPessoaFisica.EnderecoPrincipal.dc_num_endereco == null || c.Aluno.AlunoPessoaFisica.EnderecoPrincipal.dc_num_endereco == "" ? "" : " Nº " + c.Aluno.AlunoPessoaFisica.EnderecoPrincipal.dc_num_endereco) +
                                                     (c.Aluno.AlunoPessoaFisica.EnderecoPrincipal.dc_compl_endereco == null || c.Aluno.AlunoPessoaFisica.EnderecoPrincipal.dc_compl_endereco == "" ? "" : " / " + c.Aluno.AlunoPessoaFisica.EnderecoPrincipal.dc_compl_endereco) +
                                                     (c.Aluno.AlunoPessoaFisica.EnderecoPrincipal.Bairro.no_localidade == null ? "" : ", Bairro: " + c.Aluno.AlunoPessoaFisica.EnderecoPrincipal.Bairro.no_localidade) +
                                                     (c.Aluno.AlunoPessoaFisica.EnderecoPrincipal.Logradouro.dc_num_cep == null ? "" : ", CEP: " + c.Aluno.AlunoPessoaFisica.EnderecoPrincipal.Logradouro.dc_num_cep) +
                                                     (c.Aluno.AlunoPessoaFisica.EnderecoPrincipal.Cidade.no_localidade == null ? "" : ", Cidade: " + c.Aluno.AlunoPessoaFisica.EnderecoPrincipal.Cidade.no_localidade) +
                                                     (c.Aluno.AlunoPessoaFisica.EnderecoPrincipal.Estado.no_localidade == null ? "" : " - " + c.Aluno.AlunoPessoaFisica.EnderecoPrincipal.Estado.Estado.sg_estado)

                                               }).ToList().Select(x => new MatriculaRel
                                               {
                                                   no_atendente = x.no_atendente,
                                                   id_tipo_turma = x.cd_produto_atual + "" + x.cd_curso_atual + "" + x.cd_duracao_atual + "" + x.cd_regime_atual,
                                                   cd_aluno = x.cd_aluno,
                                                   cd_turma = x.cd_turma > 0 ? x.cd_turma : 0,
                                                   dt_matricula_contrato = x.dt_matricula_contrato,
                                                   no_turma = String.IsNullOrEmpty(x.no_turma) ? "Sem turma" : x.no_turma,
                                                   no_produto = x.no_produto,
                                                   dc_duracao = x.dc_duracao,
                                                   telefone = x.telefone,
                                                   no_curso = x.no_curso,
                                                   no_regime = x.no_regime,
                                                   no_pessoa = x.no_pessoa,
                                                   nm_contrato = x.nm_contrato,
                                                   nm_parcelas_mensalidade = x.nm_parcelas_mensalidade,
                                                   nm_dia_vcto = x.nm_dia_vcto,
                                                   vl_parcela_contrato = x.vl_parcela_contrato,
                                                   motivo_desistencia = x.motivo_desistencia,
                                                   no_professor = x.no_professor,
                                                   no_apelido = x.no_apelido,
                                                   nm_parcelas_material = (x.nm_parcelas_material != null) ? (byte)x.nm_parcelas_material : (byte)0,
                                                   vl_material_contrato = x.vl_material_contrato != null ?(decimal)x.vl_material_contrato : 0,
                                                   contrato_digitalizado = x.contratoDigitalizado,
                                                   dc_endereco_completo = x.enderecoAluno
                                               }).OrderBy(mtr => mtr.no_pessoa);

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        private string converteEnderecoPrincipal(EnderecoSGF endereco)
        {
            string retorno = "";
            if (endereco == null)
            {
                return "";
            }
            else
            {
                var enderecoBd = (from e in db.EnderecoSGF
                                       .Include(x => x.TipoLogradouro)
                                       .Include(x => x.Logradouro)
                                       .Include(x => x.Bairro)
                                       .Include(x => x.Cidade)
                                       .Include(x => x.Estado)
                                  where e.cd_endereco == endereco.cd_endereco
                                  select e).FirstOrDefault();
                if (enderecoBd != null)
                {
                    if (enderecoBd.Logradouro != null)
                    {
                        if (enderecoBd.TipoLogradouro != null && !String.IsNullOrEmpty(enderecoBd.TipoLogradouro.no_tipo_logradouro))
                            retorno += enderecoBd.TipoLogradouro.no_tipo_logradouro + " ";
                        retorno += enderecoBd.Logradouro.no_localidade;
                        if (!String.IsNullOrEmpty(enderecoBd.dc_num_endereco))
                            retorno += " Nº " + enderecoBd.dc_num_endereco;
                        if (!String.IsNullOrEmpty(enderecoBd.dc_compl_endereco))
                            retorno += " / " + enderecoBd.dc_compl_endereco;
                        if (enderecoBd.Bairro != null)
                            if (!String.IsNullOrEmpty(enderecoBd.Bairro.no_localidade))
                                retorno += ", Bairro: " + enderecoBd.Bairro.no_localidade;
                        if (!String.IsNullOrEmpty(enderecoBd.dc_num_cep))
                            retorno += ", CEP: " + enderecoBd.dc_num_cep;
                        if ((enderecoBd.Cidade != null && !string.IsNullOrEmpty(enderecoBd.Cidade.no_localidade))
                            && (enderecoBd.Estado != null && !string.IsNullOrEmpty(enderecoBd.Estado.no_localidade)))
                            retorno += string.Format(", Cidade: {0} - {1}", enderecoBd.Cidade.no_localidade, enderecoBd.Estado.no_localidade);
                    }
                    else return "";
                }
                else return "";
            }
            return retorno;
        }

        public IEnumerable<MatriculaRel> getMatriculaPorMotivo(int cd_empresa, int cd_turma, int cd_aluno, List<int> situacaoAlunoTurma, bool semTurma, bool tranferencia, bool retorno, int situacaoContrato, DateTime? dtIni, DateTime? dtFim, int vinculado)
        {
            try
            {
                int id_origem_movimento = Int32.Parse(db.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                var motivos = from am in db.MotivoMatricula
                              where db.AlunoMotivoMatricula.Where(amm => amm.cd_motivo_matricula == am.cd_motivo_matricula
                                  && amm.Aluno.cd_pessoa_escola == cd_empresa
                                  && amm.Aluno.Contratos.Where(at => at.cd_pessoa_escola == cd_empresa).Any()
                              ).Any()
                              select am;

                if (cd_turma > 0)
                    motivos = from c in motivos
                              where c.AlunosMotivosMatricula.Where(am => am.Aluno.AlunoTurma.Where(alt => alt.cd_turma == cd_turma).Any()).Any()
                              select c;

                if (cd_aluno > 0)
                    motivos = from c in motivos
                              where c.AlunosMotivosMatricula.Where(al => al.cd_aluno == cd_aluno).Any()
                              select c;

                if (situacaoAlunoTurma.Count() > 0 && !situacaoAlunoTurma.Contains(100))
                    motivos = from c in motivos
                              where c.AlunosMotivosMatricula.Where(am => am.Aluno.AlunoTurma.Where(alt => alt.cd_situacao_aluno_turma.HasValue && situacaoAlunoTurma.Contains(alt.cd_situacao_aluno_turma.Value)).Any()).Any()
                              select c;

                if (semTurma)
                    motivos = from c in motivos
                              where !c.AlunosMotivosMatricula.Where(am => am.Aluno.Contratos.Where(at => at.AlunoTurma.Any() && at.cd_pessoa_escola == cd_empresa).Any()).Any()
                              select c;

                if (tranferencia)
                    motivos = from c in motivos
                              where c.AlunosMotivosMatricula.Where(am => am.Aluno.Contratos.Where(at1 => at1.id_transferencia && at1.cd_pessoa_escola == cd_empresa).Any()).Any()
                              select c;

                if (retorno)
                    motivos = from c in motivos
                              where c.AlunosMotivosMatricula.Where(am => am.Aluno.Contratos.Where(at1 => at1.id_retorno && at1.cd_pessoa_escola == cd_empresa).Any()).Any()
                              select c;

                if (situacaoContrato > 0)
                    motivos = from c in motivos
                              where c.AlunosMotivosMatricula.Where(am => am.Aluno.Contratos.Where(at1 => at1.id_tipo_matricula == situacaoContrato && at1.cd_pessoa_escola == cd_empresa).Any()).Any()
                              select c;

                if (dtIni.HasValue)
                    motivos = from c in motivos
                              where c.AlunosMotivosMatricula.Where(am => am.Aluno.Contratos.Where(at1 => at1.dt_matricula_contrato >= dtIni.Value && at1.cd_pessoa_escola == cd_empresa).Any()).Any()
                              select c;

                if (dtFim.HasValue)
                    motivos = from c in motivos
                              where c.AlunosMotivosMatricula.Where(am => am.Aluno.Contratos.Where(at1 => at1.dt_matricula_contrato <= dtFim.Value && at1.cd_pessoa_escola == cd_empresa).Any()).Any()
                              select c;

                if (vinculado > 0)
                {
                    if (vinculado == 1)
                    {
                        motivos = from c in motivos
                                  where c.AlunosMotivosMatricula.Where(am => am.Aluno.Contratos.Where(at1 =>
                                      (from m in db.Movimento
                                        where m.cd_origem_movimento == at1.cd_contrato &&
                                              m.id_origem_movimento == id_origem_movimento &&
                                              m.id_material_didatico == true &&
                                              !m.id_venda_futura
                                        select m).Any()
                            
                                  ).Any()).Any()
                                  select c;
                        
                    }
                    else
                    if (vinculado == 2)
                    {
                        motivos = from c in motivos
                                  where c.AlunosMotivosMatricula.Where(am => am.Aluno.Contratos.Where(at1 =>
                                      !(from m in db.Movimento
                                        where m.cd_origem_movimento == at1.cd_contrato &&
                                              m.id_origem_movimento == id_origem_movimento &&
                                              m.id_material_didatico == true &&
                                              !m.id_venda_futura
                                        select m).Any()

                                  ).Any()).Any()
                                  select c;
                        
                    }
                    else
                    {

                        motivos = from c in motivos
                                  where c.AlunosMotivosMatricula.Where(am => am.Aluno.Contratos.Where(at1 => 
                                      at1.AlunoTurma.Where(alm => alm.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                                  alm.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado).Any()
                                      && !(from m in db.Movimento
                                           where m.cd_origem_movimento == at1.cd_contrato &&
                                                 m.id_origem_movimento == id_origem_movimento &&
                                                 m.id_material_didatico == true &&
                                                 !m.id_venda_futura
                                           select m).Any() &&
                                      (from m in db.Movimento
                                       where m.cd_pessoa_empresa == at1.cd_pessoa_escola &&
                                             m.id_material_didatico == true &&
                                             !m.id_venda_futura &&
                                             m.id_origem_movimento == null &&
                                             (m.cd_aluno == at1.cd_aluno || m.cd_pessoa == at1.cd_pessoa_responsavel) &&
                                             m.ItensMovimento.Where(
                                                 im => im.Item.cd_tipo_item == (int)TipoItem.TipoItemEnum.MATERIAL_DIDATICO &&
                                                       im.Item.id_material_didatico == true &&
                                                       im.Item.Cursos.Where(ic =>  at1.CursoContrato.Where(cc => cc.cd_curso == ic.cd_curso).Any() ).Any()
                                             ).Any()

                                            select m).Any()
                                           ).Any()).Any()
                                  select c;

                        
                    }

                }

                IEnumerable<MatriculaRel> sql = (from c in motivos
                                                 select new
                                                 {

                                                     cd_motivo_matricula = c.cd_motivo_matricula,
                                                     dc_motivo_matricula = c.dc_motivo_matricula,
                                                     qtd_motivo_por_matricula = c.AlunosMotivosMatricula.Count(am => am.Aluno.Contratos.Where(at =>
                                                         (situacaoContrato <= 0 || at.id_tipo_matricula == situacaoContrato)
                                                         && (!semTurma || !at.AlunoTurma.Any())
                                                         && (!tranferencia || at.id_transferencia)
                                                         && (!retorno || at.id_retorno)
                                                         && (!dtIni.HasValue || at.dt_matricula_contrato >= dtIni.Value)
                                                         && (!dtFim.HasValue || at.dt_matricula_contrato <= dtFim.Value)
                                                         && at.cd_pessoa_escola == cd_empresa
                                                     ).Any())
                                                 }).ToList().Select(x => new MatriculaRel
                                                 {
                                                     cd_motivo_matricula = x.cd_motivo_matricula,
                                                     dc_motivo_matricula = x.dc_motivo_matricula,
                                                     qtd_motivo_por_matricula = x.qtd_motivo_por_matricula
                                                 });


                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public DataTable getMatriculaOutros(int cd_escola, int cd_produto, DateTime? dta_ini, DateTime? dta_fim, byte qtd_max)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringReportProcedure"].ConnectionString;
            string providerName = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringReportProcedure"].ProviderName;


            DataTable dtReportData = new DataTable();

            DBHelper dbSql = new DBHelper(connectionString, providerName);

            DBParameter param1 = new DBParameter("@cd_escola", cd_escola, DbType.Int32);
            DBParameter param2 = new DBParameter("@cd_produto", cd_produto, DbType.Int32);
            DBParameter param3 = new DBParameter("@qtd_max", qtd_max, DbType.Byte);
            DBParameter param4 = new DBParameter("@dta_ini", dta_ini, DbType.DateTime);
            DBParameter param5 = new DBParameter("@dta_fim", dta_fim, DbType.DateTime);

            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(param1);
            paramCollection.Add(param2);
            paramCollection.Add(param3);
            paramCollection.Add(param4);
            paramCollection.Add(param5);

            dtReportData = dbSql.ExecuteDataTable("sp_Rptmatriculas_outros_produtos", paramCollection, CommandType.StoredProcedure);
            return dtReportData;
        }
        public Contrato getMatriculaForMovimento(int id, int cdEscola)
        {
            try
            {
                Contrato sql = ((from c in db.Contrato
                                 where
                                     c.cd_contrato == id &&
                                     c.cd_pessoa_escola == cdEscola
                                 select new
                                 {
                                     cd_contrato = c.cd_contrato,
                                     cd_aluno = c.cd_aluno,
                                     no_aluno = c.Aluno.AlunoPessoaFisica.no_pessoa,
                                     no_pessoa = db.PessoaSGF.Where(p => p.cd_pessoa == c.cd_pessoa_responsavel).FirstOrDefault().no_pessoa,
                                     cd_responsavel = c.cd_pessoa_responsavel,
                                     cd_pessoa_aluno = c.Aluno.AlunoPessoaFisica.cd_pessoa,
                                     cd_curso_atual = c.cd_curso_atual,
                                     cd_uf_aluno = c.Aluno.AlunoPessoaFisica != null & c.Aluno.AlunoPessoaFisica.EnderecoPrincipal != null ? c.Aluno.AlunoPessoaFisica.EnderecoPrincipal.cd_loc_estado : 0,
                                     cd_cidade_aluno = c.Aluno.AlunoPessoaFisica != null & c.Aluno.AlunoPessoaFisica.EnderecoPrincipal != null ? c.Aluno.AlunoPessoaFisica.EnderecoPrincipal.cd_loc_cidade : 0,
                                     CursoContrato = c.CursoContrato,
                                     id_tipo_contrato = c.id_tipo_contrato
                                 }).ToList().Select(x => new Contrato
                                 {
                                     cd_contrato = x.cd_contrato,
                                     cd_aluno = x.cd_aluno,
                                     no_pessoa = x.no_aluno,
                                     no_responsavel = x.no_pessoa,
                                     cd_pessoa_responsavel = x.cd_responsavel,
                                     cd_curso_atual = x.cd_curso_atual,
                                     cd_uf_aluno = x.cd_uf_aluno,
                                     cd_cidade_aluno = x.cd_cidade_aluno,
                                     cd_pessoa_aluno = x.cd_pessoa_aluno,
                                     CursoContrato = x.CursoContrato,
                                     id_tipo_contrato = x.id_tipo_contrato
                                 })).FirstOrDefault();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public byte getTpMatricula(int cdContrato, int cdEscola)
        {
            try
            {

                byte sql = (from c in db.Contrato
                            where c.cd_contrato == cdContrato &&
                            c.cd_pessoa_escola == cdEscola
                            select c.id_tipo_matricula).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public double getBolsaMatricula(int cdContrato, int cdEscola)
        {
            try
            {

                double sql = (from c in db.Contrato
                              where c.cd_contrato == cdContrato &&
                              c.cd_pessoa_escola == cdEscola
                              select c.pc_desconto_bolsa).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Contrato getSaldoMatricula(int cdContrato, int cdEscola, decimal pc_bolsa)
        {
            try
            {
                SGFWebContext dbContext = new SGFWebContext();
                int[] statusCnabTitulo = new int[] { (int)Titulo.StatusCnabTitulo.INICIAL, (int)Titulo.StatusCnabTitulo.CONFIRMADO_PEDIDO_BAIXA };
                int cd_origem = Int32.Parse(dbContext.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                decimal saldo = 0;
                int qtd_parcelas_abertas = 0;
                string[] cdTiposTitulo = new string[3] { "ME", "MA", "MM" };
                var sql = from t in db.Titulo
                                 where t.cd_origem_titulo == cdContrato &&
                                     t.id_origem_titulo == cd_origem &&
                                     statusCnabTitulo.Contains(t.id_status_cnab) &&
                                     (t.dc_tipo_titulo != "TM" && t.dc_tipo_titulo != "TA") &&
                                     t.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO &&
                                     !t.BaixaTitulo.Any(x => x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA &&
                                                                                  x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO)
                                 select t;
                if (sql.Any())
                {
                    saldo = sql.Sum(x => cdTiposTitulo.Contains(x.dc_tipo_titulo) ? (x.vl_saldo_titulo - x.vl_material_titulo) : x.vl_saldo_titulo);
                    qtd_parcelas_abertas = sql.Count();
                }
                var listaTitulosContrato = (from t in db.Titulo
                          where t.cd_origem_titulo == cdContrato &&
                                t.id_origem_titulo == cd_origem &&
                                (t.dc_tipo_titulo != "TM" && t.dc_tipo_titulo != "TA")
                          select new {
                              t.vl_titulo,
                              t.vl_desconto_contrato,
                              t.dc_tipo_titulo,
                              t.id_status_titulo,
                              t.id_status_cnab,
                              t.vl_material_titulo,
                              t.nm_parcela_titulo,
                              possuiBaixa = t.BaixaTitulo.Any(x => x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA &&
                                                              x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA),
                              possuiBaixaBolsa = t.BaixaTitulo.Any(x => x.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA ||
                                                                        x.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO)
                          }).ToList().Select(x=> new Titulo {
                              vl_titulo = x.vl_titulo,
                              vl_desconto_contrato = x.vl_desconto_contrato,
                              dc_tipo_titulo = x.dc_tipo_titulo,
                              id_status_titulo = x.id_status_titulo,
                              id_status_cnab = x.id_status_cnab,
                              vl_material_titulo = x.vl_material_titulo,
                              nm_parcela_titulo = x.nm_parcela_titulo,
                              possuiBaixa = x.possuiBaixa,
                              possuiBaixaBolsa = x.possuiBaixaBolsa
                          }).ToList();

                return new Contrato { valorSaldoMatricula = saldo, qtdTitulosAbertos = qtd_parcelas_abertas, titulos = listaTitulosContrato };
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Contrato getMatriculaByIdGeral(int id, int cdEscola)
        {
            try
            {
                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());

                var notas_material_didatico = (from m in db.Movimento 
                                               where 
                                                m.cd_origem_movimento == id && m.id_material_didatico == true && 
                                                m.id_origem_movimento == cd_origem 
                                               select new NotasVendaMaterialUI { 
                                                   cd_movimento = m.cd_movimento, 
                                                   cd_curso = m.cd_curso, 
                                                   nm_movimento = String.IsNullOrEmpty(m.nm_movimento.ToString()) ? ("Sem N° Saída, id: " + m.cd_movimento) : m.nm_movimento.ToString() +
                                                    m.dc_serie_movimento == null ? "" : "-" + m.dc_serie_movimento,
                                                   id_venda_futura = m.id_venda_futura
                                               }).ToList();

                Contrato sql = (from c in db.Contrato.Include(x=>x.CursoContrato)
                                where
                                    c.cd_contrato == id &&
                                    c.cd_pessoa_escola == cdEscola
                                select c).FirstOrDefault();
                sql.notas_material_didatico = notas_material_didatico;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool getAjusteManualMatricula(int cdContrato, int cdEscola)
        {
            try
            {

                bool sql = (from c in db.Contrato
                            where c.cd_contrato == cdContrato &&
                            c.cd_pessoa_escola == cdEscola
                            select c.id_ajuste_manual).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Contrato getContratoReajusteAnual(int cdContrato, int cdEscola)
        {
            try
            {

                Contrato sql = (from c in db.Contrato
                            where c.cd_contrato == cdContrato &&
                            c.cd_pessoa_escola == cdEscola
                            select new
                            {
                                c.cd_contrato,
                                c.pc_desconto_bolsa,
                                c.cd_tipo_financeiro
                            }).ToList().Select(x => new Contrato { 
                                cd_contrato = x.cd_contrato,
                                pc_desconto_bolsa = x.pc_desconto_bolsa,
                                cd_tipo_financeiro = x.cd_tipo_financeiro
                            }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        // sp_excluir_contrato 
        public string postDeleteMatricula(Nullable<int> cd_contrato, Nullable<int> cd_usuario, Nullable<int> fuso)
        {
            try
            {
                db.Database.Connection.Open();
                var command = db.Database.Connection.CreateCommand();
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = @"sp_excluir_contrato";
                command.CommandTimeout = 180;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>();

                if (cd_contrato != null)
                    sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@cd_contrato", cd_contrato));
                else
                    sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@cd_contrato", DBNull.Value));
                if (cd_usuario != null)
                    sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@cd_usuario", cd_usuario));
                else
                    sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@cd_usuario", DBNull.Value));
                if (fuso != null)
                    sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@fuso", fuso));
                else
                    sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@fuso", DBNull.Value));

                var parameter = new System.Data.SqlClient.SqlParameter("@result", System.Data.SqlDbType.Int);
                parameter.Direction = System.Data.ParameterDirection.ReturnValue;

                sqlParameters.Add(parameter);

                command.Parameters.AddRange(sqlParameters.ToArray());
                command.ExecuteReader();
                
                var retunvalue = Convert.ToString((int)command.Parameters["@result"].Value);
                db.Database.Connection.Close();
                return retunvalue;
            }
            catch (System.Data.SqlClient.SqlException exe)
            {
                db.Database.Connection.Close();
                throw new DataAccessException(exe);
            }
            catch (Exception exe)
            {
                db.Database.Connection.Close();
                throw new DataAccessException(exe);
            }
        }

        public Turma getTurmaById(int cdTurma)
        {
            try
            {

                Turma sql = (from t in db.Turma
                    where t.cd_turma == cdTurma
                    select t).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<AlunoTurma> findAlunosTurma(int cd_turma, int[] cdAlunos)
        {
            try
            {
                var sql = from alunosT in db.AlunoTurma
                          where alunosT.cd_turma == cd_turma && 
                                cdAlunos.Contains(alunosT.cd_aluno)
                          select alunosT;

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public string postGerarVenda(Nullable<int> cd_contrato, Nullable<int> cd_curso, Nullable<int> cd_usuario, Nullable<int> fuso, bool id_futura, int cd_regime)
        {
            try
            {
                db.Database.Connection.Open();
                var command = db.Database.Connection.CreateCommand();
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = @"sp_gerar_venda_material_didatico";
                command.CommandTimeout = 180;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>();

                if (cd_contrato != null)
                    sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@cd_contrato", cd_contrato));
                else
                    sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@cd_contrato", DBNull.Value));

                if (cd_curso != null)
                    sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@cd_curso", cd_curso));
                else
                    sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@cd_curso", DBNull.Value));

                if (cd_usuario != null)
                    sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@cd_usuario", cd_usuario));
                else
                    sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@cd_usuario", DBNull.Value));

                if (fuso != null)
                    sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@fuso", fuso));
                else
                    sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@fuso", DBNull.Value));

                sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@id_futura", id_futura));
                sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@cd_regime", cd_regime));

                var parameterm = new System.Data.SqlClient.SqlParameter("@mensagem", System.Data.SqlDbType.VarChar, 1024);
                parameterm.Direction = System.Data.ParameterDirection.Output;

                sqlParameters.Add(parameterm);

                var parameter = new System.Data.SqlClient.SqlParameter("@result", System.Data.SqlDbType.Int)
                {
                    Direction = System.Data.ParameterDirection.ReturnValue,

                };

                sqlParameters.Add(parameter);

                command.Parameters.AddRange(sqlParameters.ToArray());
                command.ExecuteReader();

                //var retunvalue = Convert.ToString((int)command.Parameters["@result"].Value);
                string retunvalue = null;
                retunvalue = (string)command.Parameters["@mensagem"].Value;

                db.Database.Connection.Close();
                return retunvalue;
            }
            catch (System.Data.SqlClient.SqlException exe)
            {
                db.Database.Connection.Close();
                throw new DataAccessException(exe);
            }
            catch (Exception exe)
            {
                db.Database.Connection.Close();
                throw new DataAccessException(exe);
            }
        }

        public string postVincularVendaMaterial(Nullable<int> cd_movimento, Nullable<int> cd_turma_origem, Nullable<int> cd_contrato)
        {
            try
            {

                db.Database.Connection.Open();
                var command = db.Database.Connection.CreateCommand();
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = @"sp_vincular_venda_contrato";
                command.CommandTimeout = 180;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>();

                if (cd_movimento != null)
                    sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@cd_movimento", cd_movimento));
                else
                    sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@cd_movimento", DBNull.Value));

                if (cd_turma_origem != null)
                    sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@cd_turma_origem", cd_turma_origem));
                else
                    sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@cd_turma_origem", DBNull.Value));

                if (cd_contrato != null)
                    sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@cd_contrato", cd_contrato));
                else
                    sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@cd_contrato", DBNull.Value));


                var parameterm = new System.Data.SqlClient.SqlParameter("@mensagem", System.Data.SqlDbType.VarChar, 1024);
                parameterm.Direction = System.Data.ParameterDirection.Output;

                sqlParameters.Add(parameterm);

                var parameter = new System.Data.SqlClient.SqlParameter("@result", System.Data.SqlDbType.Int);
                parameter.Direction = System.Data.ParameterDirection.ReturnValue;

                sqlParameters.Add(parameter);

                command.Parameters.AddRange(sqlParameters.ToArray());
                command.ExecuteReader();

                //var retunvalue = Convert.ToString((int)command.Parameters["@result"].Value);
                string retunvalue = null;
                retunvalue = (string)command.Parameters["@mensagem"].Value;

                db.Database.Connection.Close();
                return retunvalue;
            }
            catch (System.Data.SqlClient.SqlException exe)
            {
                db.Database.Connection.Close();
                throw new DataAccessException(exe);
            }
            catch (Exception exe)
            {
                db.Database.Connection.Close();
                throw new DataAccessException(exe);
            }
        }

    }
}

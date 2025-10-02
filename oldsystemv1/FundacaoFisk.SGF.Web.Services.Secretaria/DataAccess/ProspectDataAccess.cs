using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;
using System.Data.SqlClient;
using System.Data;
using FundacaoFisk.SGF.GenericModel.Partial;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess
{
    public class ProspectDataAccess : GenericRepository<Prospect>, IProspectDataAccess
    {
        public enum TipoConsultaEnum
        {
            HAS_PROSPECT_ATIVO = 1,
            HAS_PROSPECT_FOLLOWUP = 2
        }
        
       

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<ProspectSearchUI> getProspectFKSearch(SearchParameters parametros, int cdEscola, string nome, bool inicio, string email, string telefone, ProspectDataAccess.TipoConsultaEnum tipo) {
            try {
                IEntitySorter<ProspectSearchUI> sorter = EntitySorter<ProspectSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection) (int) parametros.sortOrder);

                IQueryable<ProspectSearchUI> sql2;
                IQueryable<Prospect> sql = from prospect in db.Prospect.AsNoTracking()
                        join pessoa in db.PessoaSGF.OfType<PessoaFisicaSGF>() on prospect.cd_pessoa_fisica equals pessoa.cd_pessoa
                        where prospect.cd_pessoa_escola == cdEscola
                            && prospect.id_prospect_ativo
                            && !db.Aluno.Where(a => a.cd_pessoa_aluno == prospect.cd_pessoa_fisica).Any()
                        select prospect;
                if (tipo == TipoConsultaEnum.HAS_PROSPECT_FOLLOWUP)
                    sql = from prs in sql
                          where prs.ProspectFollowUp.Any()
                          select prs;

                if(!String.IsNullOrEmpty(nome))
                    if(inicio)
                        sql = from prospect in sql
                              where prospect.PessoaFisica.no_pessoa.StartsWith(nome)
                              select prospect;
                    else
                        sql = from prospect in sql
                              where prospect.PessoaFisica.no_pessoa.Contains(nome)
                              select prospect;

                if(!String.IsNullOrEmpty(email)) {
                    if(inicio)
                        sql = from prospect in sql
                              where prospect.PessoaFisica.TelefonePessoa.Any(t => t.dc_fone_mail.StartsWith(email))
                              select prospect;
                    else
                        sql = from prospect in sql
                              where prospect.PessoaFisica.TelefonePessoa.Any(t => t.dc_fone_mail.Contains(email))
                              select prospect;
                }

                if(!String.IsNullOrEmpty(telefone)) {
                    sql = from prospect in sql
                          where prospect.PessoaFisica.TelefonePessoa.Where(pf => pf.dc_fone_mail.Contains(telefone)).Any()
                          select prospect;
                }

                sql2 = from prospect in sql
                       select new ProspectSearchUI {
                           cd_prospect = prospect.cd_prospect,
                           no_pessoa = prospect.PessoaFisica.no_pessoa,
                           email = db.TelefoneSGF.Where(t => t.cd_tipo_telefone == (int) TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL && t.cd_pessoa == prospect.cd_pessoa_fisica).FirstOrDefault().dc_fone_mail,
                           telefone = prospect.PessoaFisica.Telefone.dc_fone_mail,
                           celular = db.TelefoneSGF.Where(t => t.cd_tipo_telefone == (int) TipoTelefoneSGF.TipoTelefoneSGFEnum.CELULAR && t.cd_pessoa == prospect.cd_pessoa_fisica).FirstOrDefault().dc_fone_mail,
                           dc_reduzido_pessoa_escola = prospect.Escola.dc_reduzido_pessoa
                       };

                sql2 = from c in sql2
                       select c;

                sql2 = sorter.Sort(sql2);

                int limite = sql2.Count();

                parametros.ajustaParametrosPesquisa(limite);
                sql2 = sql2.Skip(parametros.from).Take(parametros.qtd_limite);

                parametros.qtd_limite = limite;
                return sql2;

            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public ProspectIntegracaoRetornoUI postProspectIntegracao(Nullable<int> nm_integracao, Nullable<byte> id_tipo, Nullable<int> id_teste, string no_pessoa, string email, string fone, string cep, string day_week, string periodo, Nullable<System.DateTime> dt_cadastro, string sexo, Nullable<double> hit, string phase, string courseId)
        {
            try
            {

                db.Database.Connection.Open();
                var command = db.Database.Connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"sp_gerar_prospect";

                var sqlParameters = new List<SqlParameter>();

                if (nm_integracao != null)
                    sqlParameters.Add(new SqlParameter("@nm_integracao", nm_integracao));
                else
                    sqlParameters.Add(new SqlParameter("@nm_integracao", DBNull.Value));

                if (id_tipo != null)
                    sqlParameters.Add(new SqlParameter("@id_tipo", id_tipo));
                else
                    sqlParameters.Add(new SqlParameter("@id_tipo", DBNull.Value));

                if (id_teste != null)
                    sqlParameters.Add(new SqlParameter("@id_teste", id_teste));
                else
                    sqlParameters.Add(new SqlParameter("@id_teste", DBNull.Value));

                if (!string.IsNullOrEmpty(no_pessoa))
                    sqlParameters.Add(new SqlParameter("@no_pessoa", no_pessoa));
                else
                    sqlParameters.Add(new SqlParameter("@no_pessoa", DBNull.Value));

                if (!string.IsNullOrEmpty(email))
                    sqlParameters.Add(new SqlParameter("@email", email));
                else
                    sqlParameters.Add(new SqlParameter("@email", DBNull.Value));

                if (!string.IsNullOrEmpty(fone))
                    sqlParameters.Add(new SqlParameter("@fone", fone));
                else
                    sqlParameters.Add(new SqlParameter("@fone", DBNull.Value));

                if (!string.IsNullOrEmpty(cep))
                    sqlParameters.Add(new SqlParameter("@cep", cep));
                else
                    sqlParameters.Add(new SqlParameter("@cep", DBNull.Value));

                if (!string.IsNullOrEmpty(day_week))
                    sqlParameters.Add(new SqlParameter("@day_week", day_week));
                else
                    sqlParameters.Add(new SqlParameter("@day_week", DBNull.Value));

                if (!string.IsNullOrEmpty(periodo))
                    sqlParameters.Add(new SqlParameter("@periodo", periodo));
                else
                    sqlParameters.Add(new SqlParameter("@periodo", DBNull.Value));

                if (dt_cadastro != null)
                    sqlParameters.Add(new SqlParameter("@dt_cadastro", dt_cadastro));
                else
                    sqlParameters.Add(new SqlParameter("@dt_cadastro", DBNull.Value));

                if (!string.IsNullOrEmpty(sexo))
                    sqlParameters.Add(new SqlParameter("@sexo", sexo));
                else
                    sqlParameters.Add(new SqlParameter("@sexo", DBNull.Value));

                if (hit != null)
                    sqlParameters.Add(new SqlParameter("@hit", hit));
                else
                    sqlParameters.Add(new SqlParameter("@hit", DBNull.Value));

                if (!string.IsNullOrEmpty(phase))
                    sqlParameters.Add(new SqlParameter("@phase", phase));
                else
                    sqlParameters.Add(new SqlParameter("@phase", DBNull.Value));

                if (!string.IsNullOrEmpty(courseId))
                    sqlParameters.Add(new SqlParameter("@course_id", courseId));
                else
                    sqlParameters.Add(new SqlParameter("@course_id", DBNull.Value));

                //sqlParameters.Add(new SqlParameter("@retorno", 0));
                var paramRetornoProspect = new System.Data.SqlClient.SqlParameter("@cod_prospect", SqlDbType.Int);
                paramRetornoProspect.Direction = System.Data.ParameterDirection.Output;

                sqlParameters.Add(paramRetornoProspect);


                var parameter = new SqlParameter("@result", SqlDbType.Int);
                parameter.Direction = ParameterDirection.ReturnValue;
                sqlParameters.Add(parameter);

                command.Parameters.AddRange(sqlParameters.ToArray());
                command.ExecuteReader();

                int cod_prospect = 0;
                cod_prospect = (int)command.Parameters["@cod_prospect"].Value;

                //var retunvalue = command.Parameters["@retorno"].Value;
                var retunvalue = Convert.ToBoolean((int)command.Parameters["@result"].Value);

                db.Database.Connection.Close();
                //var sql = db.sp_gerar_prospect(nm_integracao, id_tipo, id_teste, no_pessoa, email, fone, cep, day_week, periodo, dt_cadastro, sexo, hit, phase);

                var retornoProc = retunvalue ? Convert.ToBoolean((int)ProspectIntegracaoUI.StatusProcedure.SUCESSO_EXECUCAO_PROCEDURE) : Convert.ToBoolean((int)ProspectIntegracaoUI.StatusProcedure.ERRO_EXECUCAO_PROCEDURE);

                //retorna o valor da procedure junto com o cd_prospect
                ProspectIntegracaoRetornoUI retorno = new ProspectIntegracaoRetornoUI();
                retorno.cd_prospect = cod_prospect;
                retorno.retunvalue = retornoProc;

                return retorno;
            }
            catch (SqlException exe)
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

        public ProspectGeradoIntegracaoRetornoUI postGetProspectsGeradosSendPromocao()
        {
            try
            {

                db.Database.Connection.Open();
                var command = db.Database.Connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"sp_lista_prospect_gerado";

                var sqlParameters = new List<SqlParameter>();

                

                //sqlParameters.Add(new SqlParameter("@retorno", 0));
                var paramRetornoProspect = new System.Data.SqlClient.SqlParameter("@strcodigos", System.Data.SqlDbType.VarChar, -1);
                paramRetornoProspect.Direction = System.Data.ParameterDirection.Output;

                sqlParameters.Add(paramRetornoProspect);


                var parameter = new SqlParameter("@result", SqlDbType.Int);
                parameter.Direction = ParameterDirection.ReturnValue;
                sqlParameters.Add(parameter);

                command.Parameters.AddRange(sqlParameters.ToArray());
                command.ExecuteReader();

                string cod_prospect = null;
                cod_prospect = (string)command.Parameters["@strcodigos"].Value;

                //var retunvalue = command.Parameters["@retorno"].Value;
                var retunvalue = Convert.ToBoolean((int)command.Parameters["@result"].Value);

                db.Database.Connection.Close();
                //var sql = db.sp_gerar_prospect(nm_integracao, id_tipo, id_teste, no_pessoa, email, fone, cep, day_week, periodo, dt_cadastro, sexo, hit, phase);

                var retornoProc = retunvalue ? Convert.ToBoolean((int)ProspectIntegracaoUI.StatusProcedure.SUCESSO_EXECUCAO_PROCEDURE) : Convert.ToBoolean((int)ProspectIntegracaoUI.StatusProcedure.ERRO_EXECUCAO_PROCEDURE);

                //retorna o valor da procedure junto com o cd_prospect
                ProspectGeradoIntegracaoRetornoUI retorno = new ProspectGeradoIntegracaoRetornoUI();
                retorno.cd_prospect = cod_prospect;
                retorno.retunvalue = retornoProc;

                return retorno;
            }
            catch (SqlException exe)
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


        public bool existeProspectNaoConsultado(int cd_escola)
        {
            try
            {
                var sql = (from p in db.Prospect
                           where p.cd_pessoa_escola == cd_escola
                            && p.id_tipo_online != null
                                && p.id_consultado == false

                           select p).Any();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }


        public IEnumerable<ProspectSearchUI> GetProspectSearch(SearchParameters parametros, string nome, bool inicio, string email, int cdEscola, DateTime? dataIni, DateTime? dataFim, bool? ativo, bool aluno, int testeClassificacaoMatriculaOnline)
        {
            try
            {
                IEntitySorter<ProspectSearchUI> sorter = EntitySorter<ProspectSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);

                IQueryable<Prospect> sql = from p in db.Prospect.AsNoTracking()
                                           where p.cd_pessoa_escola == cdEscola
                                           select p;
                IQueryable<ProspectSearchUI> sql2;

                if (testeClassificacaoMatriculaOnline == 3) 
                {
                    sql = from prospect in sql
                          join midia in db.Midia on prospect.Midia.cd_midia equals midia.cd_midia
                          where prospect.id_tipo_online != null
                          select prospect;
                }
                else if (testeClassificacaoMatriculaOnline == 4)
                {
                    sql = from prospect in sql
                          join midia in db.Midia on prospect.Midia.cd_midia equals midia.cd_midia
                          where prospect.id_tipo_online == null
                          select prospect;
                }
                else if (testeClassificacaoMatriculaOnline == 1)
                {
                    sql = from prospect in sql
                          join midia in db.Midia on prospect.Midia.cd_midia equals midia.cd_midia
                          where (prospect.id_tipo_online == ((int)FundacaoFisk.SGF.GenericModel.Midia.TipoMidiaEnum.TESTECLASSIFICACAO))
                          select prospect;
                }else if (testeClassificacaoMatriculaOnline == 2 )
                {
                    sql = from prospect in sql
                          join midia in db.Midia on prospect.Midia.cd_midia equals midia.cd_midia
                          where (prospect.id_tipo_online == ((int)FundacaoFisk.SGF.GenericModel.Midia.TipoMidiaEnum.MATRICULAONLINE) )
                          select prospect;
                }
                

                
                if (aluno)
                    sql = from prospect in sql
                          where prospect.Aluno.Any()
                          select prospect;
                else
                    sql = from prospect in sql
                          where !prospect.Aluno.Any()
                          select prospect;

                if(dataIni.HasValue)
                    sql = from p in sql
                          where DbFunctions.TruncateTime(p.PessoaFisica.dt_cadastramento) >= DbFunctions.TruncateTime(dataIni)
                          select p;
                if (dataFim.HasValue)
                    sql = from p in sql
                          where DbFunctions.TruncateTime(p.PessoaFisica.dt_cadastramento) <= DbFunctions.TruncateTime(dataFim)
                          select p;

                if (ativo != null)
                    sql = from prospect in sql
                          where (prospect.id_prospect_ativo == ativo)
                          select prospect;

                if (!String.IsNullOrEmpty(nome))
                    if (inicio)
                        sql = from prospect in sql
                              where prospect.PessoaFisica.no_pessoa.StartsWith(nome)
                              select prospect;
                    else
                        sql = from prospect in sql
                              where prospect.PessoaFisica.no_pessoa.Contains(nome)
                              select prospect;

                if (!String.IsNullOrEmpty(email))
                    if (inicio)
                        sql = from prospect in sql
                              where prospect.PessoaFisica.TelefonePessoa.Any(t => t.dc_fone_mail.Contains(email))
                              select prospect;
                    else
                        sql = from prospect in sql
                              where prospect.PessoaFisica.TelefonePessoa.Any(t => t.dc_fone_mail.Contains(email))
                              select prospect;

                sql2 = from prospect in sql
                       select new ProspectSearchUI
                       {
                           cd_prospect = prospect.cd_prospect,
                           cd_pessoa_fisica = prospect.cd_pessoa_fisica,
                           dt_cadastramento = prospect.PessoaFisica.dt_cadastramento,
                           no_pessoa = prospect.PessoaFisica.no_pessoa,
                           email = prospect.PessoaFisica.TelefonePessoa.Where(telefone => telefone.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL).FirstOrDefault().dc_fone_mail,
                           telefone = prospect.PessoaFisica.TelefonePessoa.Where(telefone => telefone.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.TELEFONE).FirstOrDefault().dc_fone_mail,
                           celular = prospect.PessoaFisica.TelefonePessoa.Where(telefone => telefone.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.CELULAR).FirstOrDefault().dc_fone_mail,
                           id_prospect_ativo = prospect.id_prospect_ativo,
                           periodos = prospect.ProspectPeriodo
                       };

                sql2 = sorter.Sort(sql2);

                int limite = sql2.Count();

                parametros.ajustaParametrosPesquisa(limite);
                parametros.qtd_limite = limite;
                sql2 = sql2.Skip(parametros.from).Take(parametros.qtd_limite);

               
                return sql2;
      
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Prospect getProspectForEdit(int cdProspect, int cdEscola, string email)
        {
            try
            {
                var sql = (from p in db.Prospect//.Include(p => p.PessoaFisica).Include(p => p.ProspectFollowUp).Include(p => p.ProspectProduto).Include(p => p.ProspectPeriodo).Include(p => p.SysUsuario).Include(pl => pl.PlanoConta).Include(pl => pl.PlanoConta.PlanoContaSubgrupo)
                        .Include("PessoaFisica.EnderecoPrincipal")
                           where p.cd_pessoa_escola == cdEscola
                                && (String.IsNullOrEmpty(email) || p.PessoaFisica.TelefonePessoa.Where(t => t.dc_fone_mail == email).Any())
                                && (cdProspect == 0 || p.cd_prospect == cdProspect)
                           select new
                                                       {
                                                           cd_prospect = p.cd_prospect,
                                                           cd_pessoa_fisica = p.cd_pessoa_fisica,
                                                           cd_midia = p.cd_midia,
                                                           cd_pessoa_escola = p.cd_pessoa_escola,
                                                           cd_usuario = p.cd_usuario,
                                                           id_periodo = p.id_periodo,
                                                           id_dia_semana= p.id_dia_semana,
                                                           id_prospect_ativo = p.id_prospect_ativo,
                                                           cd_local_movimento = p.cd_local_movimento,
                                                           cd_tipo_liquidacao = p.cd_tipo_liquidacao,
                                                           no_login = p.SysUsuario.no_login,
                                                           //cd_usuario = p.SysUsuario.cd_usuario,
                                                           no_pessoa = p.PessoaFisica.no_pessoa,
                                                           nm_sexo = p.PessoaFisica.nm_sexo,
                                                           dt_cadastramento = p.PessoaFisica.dt_cadastramento,
                                                           vl_matricula_prospect = p.vl_matricula_prospect,
                                                           dt_matricula_prospect = p.dt_matricula_prospect,
                                                           cd_plano_conta = p.cd_plano_conta,
                                                           id_gerar_baixa = p.id_gerar_baixa,
                                                           no_subgrupo_conta = p.PlanoConta.PlanoContaSubgrupo.no_subgrupo_conta,
                                                           no_escola = p.no_escola,
                                                           id_faixa_etaria = p.id_faixa_etaria,
                                                           p.cd_motivo_inativo,
                                                           p.vl_abatimento,                                                        
                                                           id_tipo_online = p.id_tipo_online,
                                                           ProspectProduto = p.ProspectProduto,
                                                           ProspectDia = p.ProspectDia,
                                                           ProspectPeriodo = p.ProspectPeriodo,
                                                           EnderecoPrincipal = p.PessoaFisica.EnderecoPrincipal,
                                                           dt_nascimento_prospect = p.PessoaFisica.dt_nascimento
                                                       }).ToList().Select(x => new Prospect
                                                       {
                                                           cd_prospect = x.cd_prospect,
                                                           cd_pessoa_fisica = x.cd_pessoa_fisica,
                                                           cd_midia = x.cd_midia,
                                                           cd_pessoa_escola = x.cd_pessoa_escola,
                                                           cd_usuario = x.cd_usuario,
                                                           id_periodo = x.id_periodo,
                                                           id_dia_semana = x.id_dia_semana,
                                                           id_prospect_ativo = x.id_prospect_ativo,
                                                           no_escola = x.no_escola,
                                                           id_faixa_etaria = x.id_faixa_etaria,
                                                           cd_motivo_inativo =x.cd_motivo_inativo,
                                                           vl_abatimento = x.vl_abatimento,
                                                           cd_local_movimento = x.cd_local_movimento,
                                                           cd_tipo_liquidacao = x.cd_tipo_liquidacao,                                                       
                                                           id_tipo_online = x.id_tipo_online,
                                                           SysUsuario = new UsuarioWebSGF(){
                                                               cd_usuario = x.cd_usuario,
                                                               no_login = x.no_login
                                                           },
                                                           PessoaFisica = new PessoaFisicaSGF(){
                                                               cd_pessoa = x.cd_pessoa_fisica,
                                                               no_pessoa = x.no_pessoa,
                                                               nm_sexo = x.nm_sexo,
                                                               dt_cadastramento = x.dt_cadastramento,
                                                               EnderecoPrincipal = x.EnderecoPrincipal,
                                                               dt_nascimento = x.dt_nascimento_prospect
                                                           },
                                                           vl_matricula_prospect = x.vl_matricula_prospect,
                                                           dt_matricula_prospect = x.dt_matricula_prospect,
                                                           cd_plano_conta = x.cd_plano_conta,
                                                           id_gerar_baixa = x.id_gerar_baixa,
                                                           PlanoConta = new PlanoConta(){
                                                               PlanoContaSubgrupo = new SubgrupoConta(){
                                                                   no_subgrupo_conta = x.no_subgrupo_conta
                                                               }
                                                           },
                                                           ProspectProduto = x.ProspectProduto.ToList().Select(y => new ProspectProduto()
                                                           {
                                                               cd_produto = y.cd_produto
                                                           }).ToList(),
                                                           ProspectDia = x.ProspectDia.ToList().Select(y => new ProspectDia()
                                                           {
                                                               id_dia_semana = y.id_dia_semana
                                                           }).ToList(),
                                                           ProspectPeriodo = x.ProspectPeriodo.ToList().Select(y => new ProspectPeriodo()
                                                           {
                                                               id_periodo = y.id_periodo
                                                           }).ToList()
                                                       }).FirstOrDefault();

                if (sql != null && sql.PessoaFisica != null) { 
                    sql.PessoaFisica.TelefonePessoa = (from telefone in db.TelefoneSGF
                                                       where telefone.cd_pessoa == sql.PessoaFisica.cd_pessoa && telefone.id_telefone_principal
                                                       select new
                                                       {
                                                           cd_telefone = telefone.cd_telefone,
                                                           cd_operadora = telefone.cd_operadora,
                                                           cd_tipo_telefone = telefone.cd_tipo_telefone,
                                                           dc_fone_mail = telefone.dc_fone_mail,
                                                           id_telefone_principal = telefone.id_telefone_principal
                                                       }).ToList().Select(x => new TelefoneSGF
                                                       {
                                                           cd_telefone = x.cd_telefone,
                                                           cd_operadora = x.cd_operadora,
                                                           cd_tipo_telefone = x.cd_tipo_telefone,
                                                           dc_fone_mail = x.dc_fone_mail,
                                                           id_telefone_principal = x.id_telefone_principal
                                                       }).ToList();
                    sql.PessoaFisica.EnderecoPrincipal = (from e in db.EnderecoSGF
                                                          where e.cd_pessoa == sql.cd_pessoa_fisica
                                                           select new
                                                           {
                                                               cd_endereco = e.cd_endereco,
                                                               cd_loc_estado = e.cd_loc_estado,
                                                               cd_loc_cidade = e.cd_loc_cidade,
                                                               cd_loc_bairro = e.cd_loc_bairro,
                                                               cd_loc_logradouro = e.cd_loc_logradouro,
                                                               cd_tipo_endereco = e.cd_tipo_endereco,
                                                               cd_tipo_logradouro = e.cd_tipo_logradouro,
                                                               noLocCidade = e.Cidade.no_localidade,
                                                               noLocBairro = e.Bairro.no_localidade,
                                                               noLocRua = e.Logradouro.no_localidade,
                                                               dc_num_cep = e.Logradouro.dc_num_cep,
                                                               numero = e.dc_num_endereco,
                                                               dc_complemento = e.dc_compl_endereco,
                                                               cd_loc_pais = e.cd_loc_pais
                                                           }).ToList().Select(x => new EnderecoSGF
                                                           {
                                                               cd_endereco = x.cd_endereco,
                                                               cd_loc_estado = x.cd_loc_estado,
                                                               cd_loc_cidade = x.cd_loc_cidade,
                                                               cd_loc_bairro = x.cd_loc_bairro,
                                                               cd_loc_logradouro = x.cd_loc_logradouro,
                                                               cd_tipo_endereco = x.cd_tipo_endereco,
                                                               cd_tipo_logradouro = x.cd_tipo_logradouro,
                                                               noLocCidade = x.noLocCidade,
                                                               noLocBairro = x.noLocBairro,
                                                               noLocRua = x.noLocRua,
                                                               num_cep = x.dc_num_cep,
                                                               dc_num_endereco = x.numero,
                                                               cd_loc_pais = x.cd_loc_pais,
                                                               dc_compl_endereco = x.dc_complemento,
                                                           }).FirstOrDefault();


                }
                
                if (sql != null && sql.ProspectMotivoNaomatricula != null)
                    sql.ProspectMotivoNaomatricula = (from motivo in db.ProspectMotivoNaoMatricula
                                                               where motivo.cd_prospect == sql.cd_prospect
                                                               select motivo).ToList();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Prospect getProspectPorEmail(int cdEscola, string email)
        {
            try
            {
                var sql = (from prospect in db.Prospect
                           where prospect.cd_pessoa_escola == cdEscola
                                && prospect.PessoaFisica.TelefonePessoa.Where(t => t.dc_fone_mail == email).Any()
                                && !prospect.Aluno.Any()
                           select new
                           {
                               prospect.cd_prospect,
                               prospect.PessoaFisica.no_pessoa,
                               prospect.cd_midia,
                               prospect.PessoaFisica.nm_sexo,
                               prospect.cd_pessoa_fisica,
                               prospect.id_prospect_ativo
                           }).ToList().Select(x => new Prospect
                           {
                               cd_prospect = x.cd_prospect,
                               cd_pessoa_fisica = x.cd_pessoa_fisica,
                               cd_midia = x.cd_midia,
                               id_prospect_ativo = x.id_prospect_ativo,
                               PessoaFisica = new PessoaFisicaSGF
                               {
                                   cd_pessoa = x.cd_pessoa_fisica,
                                   no_pessoa = x.no_pessoa,
                                   nm_sexo = x.nm_sexo
                               }
                           }).FirstOrDefault();


                if (sql != null && sql.PessoaFisica != null)
                {
                    sql.PessoaFisica.TelefonePessoa = (from telefone in db.TelefoneSGF
                                                       where telefone.cd_pessoa == sql.PessoaFisica.cd_pessoa && telefone.id_telefone_principal
                                                       select new
                                                       {
                                                           cd_telefone = telefone.cd_telefone,
                                                           cd_operadora = telefone.cd_operadora,
                                                           cd_tipo_telefone = telefone.cd_tipo_telefone,
                                                           dc_fone_mail = telefone.dc_fone_mail,
                                                           id_telefone_principal = telefone.id_telefone_principal
                                                       }).ToList().Select(x => new TelefoneSGF
                                                       {
                                                           cd_telefone = x.cd_telefone,
                                                           cd_operadora = x.cd_operadora,
                                                           cd_tipo_telefone = x.cd_tipo_telefone,
                                                           dc_fone_mail = x.dc_fone_mail,
                                                           id_telefone_principal = x.id_telefone_principal
                                                       }).ToList();
                    sql.ProspectFollowUp = (from f in db.FollowUp
                                            where f.cd_prospect == sql.cd_prospect && f.FollowUpProspect.cd_pessoa_escola == cdEscola
                                            select new
                                            {
                                                cd_follow_up = f.cd_follow_up,
                                                dt_follow_up = f.dt_follow_up,
                                                no_login = f.FollowUpUsuario.no_login,
                                                dc_assunto = f.dc_assunto
                                            }).ToList().Select(x => new FollowUp
                                            {
                                                dt_follow_up = x.dt_follow_up,
                                                FollowUpUsuario = new UsuarioWebSGF { no_login = x.no_login },
                                                dc_assunto = x.dc_assunto
                                            }).ToList();
                }
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool deleteAll(List<Prospect> prospects)
        {
            try
            {
                string strProspect = "";
                string strPessoaFisica = "";
                if (prospects != null && prospects.Count > 0)
                    foreach (Prospect e in prospects) { 
                        strProspect += e.cd_prospect + ",";
                        strPessoaFisica += e.cd_pessoa_fisica + ",";
                    }
                // Remove o último ponto e virgula:
                if (strProspect.Length > 0)
                    strProspect = strProspect.Substring(0, strProspect.Length - 1);
                if (strPessoaFisica.Length > 0)
                    strPessoaFisica = strPessoaFisica.Substring(0, strPessoaFisica.Length - 1);
                int retorno = db.Database.ExecuteSqlCommand("delete from t_prospect where cd_prospect in(" + strProspect + ")");
                db.Database.ExecuteSqlCommand("delete from t_pessoa_fisica where cd_pessoa_fisica in(" + strPessoaFisica + ")");
                db.Database.ExecuteSqlCommand("delete from t_pessoa where cd_pessoa in(" + strPessoaFisica + ")"); 
                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public ProspectSearchUI verificaExistenciaProspect(string telefone, string celular, int cd_escola, int cd_prospect, string no_pessoa) {
            try {
                var sql = (from prospect in db.Prospect
                           join contato in db.TelefoneSGF on prospect.cd_pessoa_fisica equals contato.cd_pessoa
                           where ((contato.dc_fone_mail == telefone || contato.dc_fone_mail == celular) && prospect.PessoaFisica.no_pessoa == no_pessoa)
                                 && prospect.cd_pessoa_escola == cd_escola
                                 && (contato.cd_tipo_telefone == (int) TipoTelefoneSGF.TipoTelefoneSGFEnum.CELULAR 
                                        || contato.cd_tipo_telefone == (int) TipoTelefoneSGF.TipoTelefoneSGFEnum.TELEFONE)
                                 && prospect.cd_prospect != cd_prospect
                           select new ProspectSearchUI {
                               cd_prospect = prospect.cd_prospect,
                               no_pessoa = prospect.PessoaFisica.no_pessoa
                           }).FirstOrDefault();
                return sql;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public List<ProspectSiteUI> getProspectSite(int cd_prospect, int tipo)
        {
            try
            {
                List<ProspectSiteUI> sql = null;

                //if (tipo == (int)Prospect.TipoOnline.PRE_MATRICULA_ONLINE)
                //{
                    sql = (from prospectSite in db.ProspectSite
                           join produto in db.Produto on prospectSite.cd_produto equals produto.cd_produto
                           where prospectSite.cd_prospect == cd_prospect //&&
                                 //prospectSite.pc_acerto_teste == null &&
                                 //prospectSite.dc_acerto_teste == null
                           select new ProspectSiteUI
                           {
                               cd_prospect_site = prospectSite.cd_prospect_site,
                               cd_prospect = prospectSite.cd_prospect,
                               id_teste_online = prospectSite.id_teste_online,
                               pc_acerto_teste = prospectSite.pc_acerto_teste,
                               dc_acerto_teste = prospectSite.dc_acerto_teste,
                               cd_produto = prospectSite.cd_produto,
                               no_produto = produto.no_produto,
                               dc_produto_online = prospectSite.dc_produto_online
                           }).ToList();
                //}
                //else
                //{
                //    sql = (from prospectSite in db.ProspectSite
                //           join produto in db.Produto on prospectSite.cd_produto equals produto.cd_produto
                //           where prospectSite.cd_prospect == cd_prospect &&
                //                 prospectSite.pc_acerto_teste != null &&
                //                 prospectSite.dc_acerto_teste != null
                //           select new ProspectSiteUI
                //           {
                //               cd_prospect_site = prospectSite.cd_prospect_site,
                //               cd_prospect = prospectSite.cd_prospect,
                //               id_teste_online = prospectSite.id_teste_online,
                //               pc_acerto_teste = prospectSite.pc_acerto_teste,
                //               dc_acerto_teste = prospectSite.dc_acerto_teste,
                //               cd_produto = prospectSite.cd_produto,
                //               no_produto = produto.no_produto,
                //               dc_produto_online = prospectSite.dc_produto_online
                //           }).ToList();
                //}


                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public ProspectSearchUI getExistsProspectEmail(string email,int cdEscola, int cdProspect)
        {
            try
            {
                var sql = (from prospect in db.Prospect
                           join contato in db.TelefoneSGF on prospect.cd_pessoa_fisica equals contato.cd_pessoa
                           where contato.dc_fone_mail == email
                                 && (cdEscola == 0 || prospect.cd_pessoa_escola == cdEscola)
                                 && contato.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL 
                                 && prospect.cd_prospect != cdProspect
                           select new ProspectSearchUI
                           {
                               cd_midia = prospect.cd_midia,
                               no_pessoa = prospect.PessoaFisica.no_pessoa,
                               cd_prospect = prospect.cd_prospect,
                               no_escola = prospect.Escola.dc_reduzido_pessoa,
                               cd_pessoa_escola = prospect.cd_pessoa_escola,
                               email = contato.dc_fone_mail,
                               dt_nascimento_prospect = prospect.PessoaFisica.dt_nascimento,
                               nm_sexo = prospect.PessoaFisica.nm_sexo,
                               TelefonePessoa = (from telefone in db.TelefoneSGF
                                                 where telefone.cd_pessoa == prospect.cd_pessoa_fisica
                                                 select new
                                                 {
                                                     cd_telefone = telefone.cd_telefone,
                                                     cd_operadora = telefone.cd_operadora,
                                                     cd_classe_telefone = telefone.cd_classe_telefone,
                                                     cd_endereco = telefone.cd_endereco,
                                                     cd_tipo_telefone = telefone.cd_tipo_telefone,
                                                     dc_fone_mail = telefone.dc_fone_mail,
                                                     des_tipo_contato = telefone.TelefoneTipo.no_tipo_telefone,
                                                     no_classe = telefone.ClasseTelefone.dc_classe_telefone,
                                                     no_pessoa = telefone.PessoaTelefone.no_pessoa,
                                                     id_telefone_principal = telefone.id_telefone_principal
                                                 }).ToList().Select(x => new TelefoneUI
                                                 {
                                                     cd_telefone = x.cd_telefone,
                                                     cd_operadora = x.cd_operadora,
                                                     cd_classe_telefone = x.cd_classe_telefone,
                                                     cd_endereco = x.cd_endereco,
                                                     cd_tipo_telefone = x.cd_tipo_telefone,
                                                     dc_fone_mail = x.dc_fone_mail,
                                                     id_telefone_principal = x.id_telefone_principal
                                                 }).ToList(),
                               enderecoUI = (from e in db.EnderecoSGF
                                                    where e.cd_pessoa == prospect.cd_pessoa_fisica && prospect.PessoaFisica.cd_endereco_principal == e.cd_endereco
                                                    join p in db.PessoaSGF on e.cd_pessoa equals p.cd_pessoa
                                                    select new
                                                    {
                                                        cd_endereco = e.cd_endereco,
                                                        cd_loc_estado = e.cd_loc_estado,
                                                        cd_loc_cidade = e.cd_loc_cidade,
                                                        cd_loc_bairro = e.cd_loc_bairro,
                                                        cd_loc_logradouro = e.cd_loc_logradouro,
                                                        cd_tipo_endereco = e.cd_tipo_endereco,
                                                        cd_tipo_logradouro = e.cd_tipo_logradouro,
                                                        noLocCidade = e.Cidade.no_localidade,
                                                        noLocBairro = e.Bairro.no_localidade,
                                                        noLocRua = e.Logradouro.no_localidade,
                                                        dc_num_cep = e.Logradouro.dc_num_cep,
                                                        numero = e.dc_num_endereco,
                                                        dc_complemento = e.dc_compl_endereco,
                                                        cd_loc_pais = e.cd_loc_pais
                                                    }).ToList().Select(x => new EnderecoUI
                                                    {
                                                        cd_endereco = x.cd_endereco,
                                                        cd_loc_estado = x.cd_loc_estado,
                                                        cd_loc_cidade = x.cd_loc_cidade,
                                                        cd_loc_bairro = x.cd_loc_bairro,
                                                        cd_loc_logradouro = x.cd_loc_logradouro,
                                                        cd_tipo_endereco = x.cd_tipo_endereco,
                                                        cd_tipo_logradouro = x.cd_tipo_logradouro,
                                                        noLocCidade = x.noLocCidade,
                                                        noLocBairro = x.noLocBairro,
                                                        noLocRua = x.noLocRua,
                                                        num_cep = x.dc_num_cep,
                                                        dc_num_endereco = x.numero,
                                                        cd_loc_pais = x.cd_loc_pais,
                                                        dc_compl_endereco = x.dc_complemento,
                                                    }).FirstOrDefault()


                           }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public ProspectSearchUI getExistsProspectEmailENome(string email, int cdEscola, string nomme)
        {
            try
            {
                var sql = (from prospect in db.Prospect
                           join contato in db.TelefoneSGF on prospect.cd_pessoa_fisica equals contato.cd_pessoa
                           where contato.dc_fone_mail == email
                                 && (cdEscola == 0 || prospect.cd_pessoa_escola == cdEscola)
                                 && contato.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL &&
                                 prospect.PessoaFisica.no_pessoa == nomme
                           select new ProspectSearchUI
                           {
                               no_pessoa = prospect.PessoaFisica.no_pessoa,
                               cd_prospect = prospect.cd_prospect,
                               no_escola = prospect.Escola.dc_reduzido_pessoa,
                               cd_pessoa_escola = prospect.cd_pessoa_escola,
                               dt_nascimento_prospect = prospect.PessoaFisica.dt_nascimento
                           }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Prospect getProspectAllData(int cdProspect, int cdEscola)
        {
            try
            {
                var sql = (from prospect in db.Prospect
                           where prospect.cd_pessoa_escola == cdEscola && prospect.cd_prospect == cdProspect
                           select prospect).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        // Metodo responsavel por verificar se pessoa fisica não é prospect e/ou aluno através do email
        public PessoaFisicaSGF verificarPessoaFisicaEmail(string email)
        {
            try
            {
                var sql = (from pessoa in db.PessoaSGF.OfType<PessoaFisicaSGF>()
                           join contato in db.TelefoneSGF on pessoa.cd_pessoa equals contato.cd_pessoa
                           where contato.dc_fone_mail == email
                           && !pessoa.Prospect.Where(p => p.cd_pessoa_fisica == pessoa.cd_pessoa).Any()
                           && !(from a in db.Aluno where a.cd_pessoa_aluno == pessoa.cd_pessoa select a.cd_pessoa_aluno).Any()
                           select pessoa).FirstOrDefault();
                if (sql != null)
                    sql.TelefonePessoa = (from telefone in db.TelefoneSGF
                                          where telefone.cd_pessoa == sql.cd_pessoa
                                          select telefone).ToList();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public PessoaFisicaSGF verificarPessoaFisicaEmailCadProspect(string email)
        {
            try
            {
                var sql = (from pessoa in db.PessoaSGF.OfType<PessoaFisicaSGF>()
                           join contato in db.TelefoneSGF on pessoa.cd_pessoa equals contato.cd_pessoa
                           where contato.dc_fone_mail == email
                                 && pessoa.Prospect.Where(p => p.cd_pessoa_fisica == pessoa.cd_pessoa).Any()
                           select pessoa).FirstOrDefault();
                if (sql != null)
                    sql.TelefonePessoa = (from telefone in db.TelefoneSGF
                                          where telefone.cd_pessoa == sql.cd_pessoa
                                          select telefone).ToList();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Prospect getProspectForAluno(int cdProspect, int cdEscola)
        {
            try
            {
                var sql = (from prospect in db.Prospect
                           where prospect.cd_pessoa_escola == cdEscola &&
                                 prospect.cd_prospect == cdProspect
                           select new
                           {
                               prospect.cd_prospect,
                               prospect.PessoaFisica.no_pessoa,
                               prospect.cd_midia,
                               prospect.PessoaFisica.nm_sexo,
                               prospect.cd_pessoa_fisica,
                               prospect.PessoaFisica.EnderecoPrincipal
                           }).ToList().Select(x => new Prospect
                           {
                               cd_prospect = x.cd_prospect,
                               cd_pessoa_fisica = x.cd_pessoa_fisica,
                               cd_midia = x.cd_midia,
                               PessoaFisica = new PessoaFisicaSGF
                               {
                                   cd_pessoa = x.cd_pessoa_fisica,
                                   no_pessoa = x.no_pessoa,
                                   nm_sexo = x.nm_sexo,
                                   EnderecoPrincipal = x.EnderecoPrincipal
                               }
                           }).FirstOrDefault();
                if (sql != null)
                {
                    if (sql.PessoaFisica != null)
                        sql.PessoaFisica.TelefonePessoa = (from telefone in db.TelefoneSGF
                                                           where telefone.cd_pessoa == sql.PessoaFisica.cd_pessoa && telefone.id_telefone_principal
                                                           select new
                                                           {
                                                               cd_telefone = telefone.cd_telefone,
                                                               cd_operadora = telefone.cd_operadora,
                                                               cd_tipo_telefone = telefone.cd_tipo_telefone,
                                                               dc_fone_mail = telefone.dc_fone_mail,
                                                               id_telefone_principal = telefone.id_telefone_principal
                                                           }).ToList().Select(x => new TelefoneSGF
                                                           {
                                                               cd_telefone = x.cd_telefone,
                                                               cd_operadora = x.cd_operadora,
                                                               cd_tipo_telefone = x.cd_tipo_telefone,
                                                               dc_fone_mail = x.dc_fone_mail,
                                                               id_telefone_principal = x.id_telefone_principal
                                                           }).ToList();
                    sql.PessoaFisica.EnderecoPrincipal = (from e in db.EnderecoSGF
                                                          where e.cd_pessoa == sql.cd_pessoa_fisica
                                                          select new
                                                          {
                                                              cd_endereco = e.cd_endereco,
                                                              cd_loc_estado = e.cd_loc_estado,
                                                              cd_loc_cidade = e.cd_loc_cidade,
                                                              cd_loc_bairro = e.cd_loc_bairro,
                                                              cd_loc_logradouro = e.cd_loc_logradouro,
                                                              cd_tipo_endereco = e.cd_tipo_endereco,
                                                              cd_tipo_logradouro = e.cd_tipo_logradouro,
                                                              noLocCidade = e.Cidade.no_localidade,
                                                              noLocBairro = e.Bairro.no_localidade,
                                                              noLocRua = e.Logradouro.no_localidade,
                                                              dc_num_cep = e.Logradouro.dc_num_cep,
                                                              numero = e.dc_num_endereco,
                                                              dc_complemento = e.dc_compl_endereco,
                                                              cd_loc_pais = e.cd_loc_pais
                                                          }).ToList().Select(x => new EnderecoSGF
                                                          {
                                                              cd_endereco = x.cd_endereco,
                                                              cd_loc_estado = x.cd_loc_estado,
                                                              cd_loc_cidade = x.cd_loc_cidade,
                                                              cd_loc_bairro = x.cd_loc_bairro,
                                                              cd_loc_logradouro = x.cd_loc_logradouro,
                                                              cd_tipo_endereco = x.cd_tipo_endereco,
                                                              cd_tipo_logradouro = x.cd_tipo_logradouro,
                                                              noLocCidade = x.noLocCidade,
                                                              noLocBairro = x.noLocBairro,
                                                              noLocRua = x.noLocRua,
                                                              num_cep = x.dc_num_cep,
                                                              dc_num_endereco = x.numero,
                                                              cd_loc_pais = x.cd_loc_pais,
                                                              dc_compl_endereco = x.dc_complemento,
                                                          }).FirstOrDefault();
                    sql.ProspectFollowUp = (from f in db.FollowUp
                                            where f.cd_prospect == cdProspect && f.FollowUpProspect.cd_pessoa_escola == cdEscola
                                            select new
                                            {
                                                f.cd_usuario,
                                                dt_follow_up = f.dt_follow_up,
                                                no_login = f.FollowUpUsuario.no_login,
                                                dc_assunto = f.dc_assunto
                                            }).ToList().Select(x => new FollowUp
                                            {
                                                cd_usuario = x.cd_usuario,
                                                dt_follow_up = x.dt_follow_up,
                                                FollowUpUsuario = new UsuarioWebSGF { no_login = x.no_login },
                                                dc_assunto = x.dc_assunto
                                            }).ToList();


                }
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public int? getBaixaFinanceira(int cd_prospect, int cd_empresa) {
            SGFWebContext dbComp = new SGFWebContext();
            try {
                int origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Prospect"].ToString());
                var retorno = from p in db.Prospect
                              join titulo in db.Titulo on p.cd_prospect equals titulo.cd_origem_titulo
                              //join baixa in db.BaixaTitulo on titulo.cd_titulo equals baixa.cd_titulo
                              where p.cd_prospect == cd_prospect
                                    && p.cd_pessoa_escola == cd_empresa
                                    && titulo.id_origem_titulo == origem
                              select titulo.BaixaTitulo.FirstOrDefault();
                if(retorno.FirstOrDefault() != null)
                    return retorno.FirstOrDefault().cd_baixa_titulo;
                else
                    return null;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<ReportProspect> getProspectAtendido(int cd_escola, int cdMotivoNaoMatricula, int cFuncionario, int cdProduto, DateTime? pDtaI, DateTime? pDtaF, int cd_midia, List<int> periodos, int cd_faixa_etaria)
        {
            try
            {
                var sql = from p in db.Prospect
                          where p.cd_pessoa_escola == cd_escola
                          select p;

                if (cdMotivoNaoMatricula > 0)
                    sql = from t in sql
                          where t.ProspectMotivoNaomatricula.Where(a => a.cd_motivo_nao_matricula == cdMotivoNaoMatricula).Any()
                          select t;

                if (cFuncionario > 0)
                    sql = from t in sql
                          where t.SysUsuario.cd_pessoa == cFuncionario
                          select t;

                if (cdProduto > 0)
                    sql = from t in sql
                          where t.ProspectProduto.Where(x => x.cd_produto == cdProduto).Any()
                          select t;

                if (pDtaI.HasValue)
                    sql = from t in sql
                          where DbFunctions.TruncateTime(t.PessoaFisica.dt_cadastramento) >= pDtaI
                          select t;

                if (pDtaF.HasValue)
                    sql = from t in sql
                          where DbFunctions.TruncateTime(t.PessoaFisica.dt_cadastramento) <= pDtaF
                          select t;

                if (cd_midia > 0)
                    sql = from p in sql
                          where p.cd_midia == cd_midia
                          select p;

                if (cd_faixa_etaria > 0)
                    sql = from p in sql
                          where p.id_faixa_etaria == cd_faixa_etaria
                          select p;

                if (periodos.Count() > 0)
                    sql = from t in sql
                          where t.ProspectPeriodo.Any(x=> periodos.Contains(x.id_periodo))
                          select t;

                var retorno = (from p in sql
                               select new ReportProspect
                               {
                                   cd_pessoa_fisica = p.cd_pessoa_fisica,
                                   cd_usuario = p.cd_usuario,
                                   no_pessoa = p.PessoaFisica.no_pessoa,
                                   no_usuario_funcionario = p.SysUsuario.PessoaFisica.no_pessoa,
                                   dt_cadastramento = p.PessoaFisica.dt_cadastramento,
                                   listaProdutos = db.Produto.Where(x => x.ProdutoProspect.Where(pp => pp.cd_produto == x.cd_produto && pp.cd_prospect == p.cd_prospect).Any()),
                                   listaMotivosNaoMatricula = db.MotivoNaoMatricula.Where(x => x.MotivoNaoMatriculaProspect.Where(pp => pp.cd_prospect == p.cd_prospect).Any()),
                                   listaPeriodos = p.ProspectPeriodo,
                                   telefone = p.PessoaFisica.Telefone.dc_fone_mail,
                                   email = db.TelefoneSGF.Where(telefone => telefone.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL && 
                                                                            telefone.cd_pessoa == p.cd_pessoa_fisica).FirstOrDefault().dc_fone_mail,
                                   celular = db.TelefoneSGF.Where(telefone => telefone.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.CELULAR &&
                                                                  telefone.cd_pessoa == p.cd_pessoa_fisica).FirstOrDefault().dc_fone_mail,
                                   no_faixa_etaria = db.Modalidade.Where(m => m.cd_modalidade == p.id_faixa_etaria).FirstOrDefault().no_modalidade
                               });
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<ReportProspect> getProspectAtendidoMatricula(int cd_escola, int cdMotivoNaoMatricula, int cFuncionario, int cdProduto, DateTime? pDtaI, DateTime? pDtaF, int cd_midia, List<int> periodos)
        {
            try
            {
                var sql = from p in db.Prospect
                          where p.cd_pessoa_escola == cd_escola
                          select p;
                if (cdMotivoNaoMatricula > 0)
                    sql = from t in sql
                          where t.ProspectMotivoNaomatricula.Where(a => a.cd_motivo_nao_matricula == cdMotivoNaoMatricula).Any()
                          select t;
                if (cFuncionario > 0)
                    sql = from t in sql
                          where t.SysUsuario.cd_pessoa == cFuncionario
                          select t;
                if (cdProduto > 0)
                    sql = from t in sql
                          where t.ProspectProduto.Where(x => x.cd_produto == cdProduto).Any()
                          select t;
                if (pDtaI.HasValue)
                    sql = from t in sql
                          where DbFunctions.TruncateTime(t.PessoaFisica.dt_cadastramento) >= pDtaI
                          select t;

                if (pDtaF.HasValue)
                    sql = from t in sql
                          where DbFunctions.TruncateTime(t.PessoaFisica.dt_cadastramento) <= pDtaF
                          select t;
                if (cd_midia > 0)
                    sql = from p in sql
                          where p.cd_midia == cd_midia
                          select p;
                if (periodos.Count() > 0)
                    sql = from t in sql
                          where t.ProspectPeriodo.Any(x => periodos.Contains(x.id_periodo))
                          select t;
                var retorno = (from p in sql
                               select new ReportProspect
                               {
                                   cd_pessoa_fisica = p.cd_pessoa_fisica,
                                   cd_usuario = p.cd_usuario,
                                   no_usuario_funcionario = p.SysUsuario.PessoaFisica.no_pessoa,
                                   no_pessoa = p.PessoaFisica.no_pessoa,
                                   dt_cadastramento = p.PessoaFisica.dt_cadastramento,
                                   dt_matricula = (from c in db.Contrato
                                                   where
                                                       c.Aluno.cd_pessoa_aluno == p.cd_pessoa_fisica
                                                   orderby c.dt_matricula_contrato ascending
                                                   select c.dt_matricula_contrato).FirstOrDefault()
                               });
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<ReportProspect> getComparativoProspectAtendido(int cd_escola, int cdMotivoNaoMatricula, int cFuncionario, int cdProduto, DateTime? pDtaI, DateTime? pDtaF, int cd_midia, List<int> periodos)
        {
            try
            {
                var sql = from p in db.Prospect
                          where p.cd_pessoa_escola == cd_escola
                          select p;
                if (cdMotivoNaoMatricula > 0)
                    sql = from t in sql
                          where t.ProspectMotivoNaomatricula.Where(a => a.cd_motivo_nao_matricula == cdMotivoNaoMatricula).Any()
                          select t;
                if (cFuncionario > 0)
                    sql = from t in sql
                          where t.SysUsuario.cd_pessoa == cFuncionario
                          select t;
                if (cdProduto > 0)
                    sql = from t in sql
                          where t.ProspectProduto.Where(x => x.cd_produto == cdProduto).Any()
                          select t;
                if (pDtaI.HasValue)
                    sql = from t in sql
                          where DbFunctions.TruncateTime(t.PessoaFisica.dt_cadastramento) >= pDtaI
                          select t;

                if (pDtaF.HasValue)
                    sql = from t in sql
                          where DbFunctions.TruncateTime(t.PessoaFisica.dt_cadastramento) <= pDtaF
                          select t;
                if (cd_midia > 0)
                    sql = from p in sql
                          where p.cd_midia == cd_midia
                          select p;
                if (periodos.Count() > 0)
                    sql = from t in sql
                          where t.ProspectPeriodo.Any(x => periodos.Contains(x.id_periodo))
                          select t;
                var retorno = (from p in sql
                               join pp in db.ProspectProduto on p.cd_prospect equals pp.cd_prospect
                               select new ReportProspect
                               {
                                   cd_produto = pp.cd_produto,
                                   no_produto = pp.Produto.no_produto,
                                   cd_prospect = p.cd_prospect

                               });
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public string getNomeAtendente(int cdUsuario, int cdEscola)
        {
            try
            {
                var sql = (from p in db.UsuarioWebSGF
                           where p.cd_usuario == cdUsuario
                           select p);

                if (sql.FirstOrDefault() != null)
                    return sql.FirstOrDefault().no_login;
                return "";
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex);
            }

        }

        public Prospect getProspectByPessoaFisica(int cd_pessoa_fisica) 
        {
            try
            {
                var sql = (from p in db.Prospect
                      where p.cd_pessoa_fisica == cd_pessoa_fisica
                      select p).FirstOrDefault();

                return sql;
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex);
            }
        }

        public PromocaoIntercambioParams findProspectApiPromocaoIntercambio(int prospectCdProspect)
        {
            try
            {

                var sql = (from p in db.Prospect
                       join pf in db.PessoaSGF.OfType<PessoaFisicaSGF>().AsNoTracking() on p.cd_pessoa_fisica equals pf.cd_pessoa
                       where p.cd_prospect == prospectCdProspect 
                       select new
                       {
                           cpf = "",
                           email = ((from t in db.TelefoneSGF where t.cd_pessoa == p.cd_pessoa_fisica && t.cd_tipo_telefone == 4 && t.id_telefone_principal == true select t).FirstOrDefault() != null ?
                               (from t in db.TelefoneSGF where t.cd_pessoa == p.cd_pessoa_fisica && t.cd_tipo_telefone == 4 && t.id_telefone_principal == true select t).FirstOrDefault().dc_fone_mail : ""),
                           telefone = pf.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.TELEFONE).Any() ? pf.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.TELEFONE).FirstOrDefault().dc_fone_mail : pf.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.CELULAR).FirstOrDefault().dc_fone_mail,
                           tipo = "1",
                           id_unidade = (p.Escola.cd_empresa_coligada == null ? p.Escola.nm_cliente_integracao : (from e in db.PessoaSGF.OfType<Escola>() where e.cd_pessoa == p.Escola.cd_empresa_coligada select e.nm_cliente_integracao).FirstOrDefault()),

                       }).ToList().Select(x => new PromocaoIntercambioParams()
                       {
                           cpf = x.cpf,
                           email = x.email,
                           telefone = x.telefone,
                           tipo = x.tipo,
                           unidade = x.id_unidade != null ? x.id_unidade.ToString() : ""

                       }).FirstOrDefault();
                return sql;
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex);
            }
        }
    }
}


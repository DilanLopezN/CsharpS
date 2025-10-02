using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.GenericModel.Partial;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net.Sockets;
using FundacaoFisk.SGF.Web.Services.Pessoa.DataAccess;
using System.Data;
using DALC4NET;
using System.Reflection;
using System.Data.SqlClient;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess
{
    public class AlunoDataAccess : GenericRepository<Aluno>, IAlunoDataAccess
    {
        public enum TipoConsultaAlunoEnum
        {
            HAS_ATIVO = 0,
            HAS_POLITICADESCONTO = 1,
            DESISTENCIA = 10,
            CANCELA_DESISTENCIA = 11,
            ALUNO_CLIENTE = 12,
            ALUNO = 13,
            HAS_ALUNO_FOLLOWUP = 2,
            HAS_PESQ_NOTA_MATERIAL = 3,
            VENDA_MATERIAL = 4
        }
        public enum TipoPessoaEnum
        {
            PESSOA_RELACIONADA = 1,
            PESSOA_GERAL = 2
        }

        public enum OrigemConsultaEnum
        {
            AULAREPOSICAO = 28,
            ENVIARTRANSFERENCIA = 30, 
            ENVIARTRANSFERENCIACAD = 31,
            ALUNOSEMAULA = 33,
            DESISTENCIA_CARGA = 50,
            CARGA_HORARIA = 42,
            CARGA_HORARIA_COM_ESCOLA = 43,
            ALUNO_SEARCH_PERDA_MATERIAL = 44,
            ALUNO_FK_MOVIMENTO = 45,
        }

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        #region Pessoa

        public IEnumerable<FundacaoFisk.SGF.Web.Services.Secretaria.Model.PessoaSGFSearchUI> GetPessoaSearch(SearchParameters parametros, string nome, string apelido, int tipoPessoa, string cnpjCpf, int? papel, int sexo, bool inicio, int cdEscola, TipoPessoaEnum tipo)
        {
            try
            {
                IEntitySorter<FundacaoFisk.SGF.Web.Services.Secretaria.Model.PessoaSGFSearchUI> sorter = EntitySorter<FundacaoFisk.SGF.Web.Services.Secretaria.Model.PessoaSGFSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<FundacaoFisk.SGF.Web.Services.Secretaria.Model.PessoaSGFSearchUI> sql;

                sql = from c in db.PessoaSGF.AsNoTracking()
                      where //c.id_pessoa_empresa == false &&
                             (!papel.HasValue || c.PessoaFilhoRelacionamento.Where(r => r.RelacionamentoFilhoPapel.cd_papel == papel && (papel != (int)PapelSGF.TipoPapelSGF.FRANQUEADO || (r.cd_pessoa_pai == cdEscola))).Any() || c.PessoaPaiRelacionamento.Where(rp => rp.RelacionamentoPaiPapel.cd_papel == papel).Any())
                            && c.PessoaEmpresa.Where(r => r.cd_escola == cdEscola).Any()
                      select new FundacaoFisk.SGF.Web.Services.Secretaria.Model.PessoaSGFSearchUI
                      {
                          cd_pessoa = c.cd_pessoa,
                          no_pessoa = c.no_pessoa,
                          nm_natureza_pessoa = c.nm_natureza_pessoa,
                          dc_reduzido_pessoa = c.dc_reduzido_pessoa,
                          dt_cadastramento = c.dt_cadastramento,
                          nm_cpf_cgc = c.nm_natureza_pessoa == 1 ?//Pessoa Fisica
                                        db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().cd_pessoa_cpf != null ?
                                           db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().PessoaSGFQueUsoOCpf.nm_cpf :
                                                db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf :
                          db.PessoaSGF.OfType<PessoaJuridicaSGF>().Where(pj => pj.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaJuridicaSGF>().dc_num_cgc,
                          no_pessoa_dependente = c.nm_natureza_pessoa == 1 ?//Pessoa Fisica
                                           db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().PessoaSGFQueUsoOCpf.no_pessoa : "",
                          papeisFilhos = (from pf in db.PapelSGF
                                          where pf.PapelFilhoRelacionamento.Where(r => r.cd_pessoa_filho == c.cd_pessoa).Any()
                                          select pf),
                          papeisPai = //c.PessoaPaiRelacionamento.Select(r => r.RelacionamentoPaiPapel)
                                      (from pf in db.PapelSGF
                                       where pf.PapelPaiRelacionamento.Where(r => r.cd_pessoa_pai == c.cd_pessoa).Any()
                                       select pf)
                      };
                if (tipoPessoa.Equals(1))
                {
                    sql = from p in db.PessoaSGF.OfType<PessoaFisicaSGF>().AsNoTracking()
                          where (!papel.HasValue || p.PessoaFilhoRelacionamento.Where(r => r.RelacionamentoFilhoPapel.cd_papel == papel).Any() ||
                                  p.PessoaPaiRelacionamento.Where(r => r.RelacionamentoPaiPapel.cd_papel == papel).Any()) &&
                                  p.PessoaEmpresa.Where(r => r.cd_escola == cdEscola).Any() &&
                                  (!string.IsNullOrEmpty(cnpjCpf) ? (p.PessoaSGFQueUsoOCpf.nm_cpf == cnpjCpf || p.nm_cpf == cnpjCpf) : true)
                          select new FundacaoFisk.SGF.Web.Services.Secretaria.Model.PessoaSGFSearchUI
                          {
                              cd_pessoa = p.cd_pessoa,
                              no_pessoa = p.no_pessoa,
                              nm_natureza_pessoa = p.nm_natureza_pessoa,
                              dc_reduzido_pessoa = p.dc_reduzido_pessoa,
                              dt_cadastramento = p.dt_cadastramento,
                              nm_cpf_cgc = p.cd_pessoa_cpf != null ? p.PessoaSGFQueUsoOCpf.nm_cpf : p.nm_cpf,
                              nm_sexo = p.nm_sexo,
                              papeisFilhos = //c.PessoaFilhoRelacionamento.Select(r => r.RelacionamentoFilhoPapel),
                                              (from pf in db.PapelSGF
                                               where pf.PapelFilhoRelacionamento.Where(r => r.cd_pessoa_filho == p.cd_pessoa).Any()
                                               select pf),
                              papeisPai = //c.PessoaPaiRelacionamento.Select(r => r.RelacionamentoPaiPapel)
                                            (from pf in db.PapelSGF
                                             where pf.PapelPaiRelacionamento.Where(r => r.cd_pessoa_pai == p.cd_pessoa).Any()
                                             select pf),
                              no_pessoa_dependente = p.PessoaSGFQueUsoOCpf.no_pessoa,
                          };

                    if (sexo > 0)
                        sql = from p in sql
                              where p.nm_sexo == sexo
                              select p;
                }

                else if (tipoPessoa.Equals(2))
                    sql = from c in db.PessoaSGF.OfType<PessoaJuridicaSGF>().AsNoTracking()
                          where !string.IsNullOrEmpty(cnpjCpf) ? c.dc_num_cgc == cnpjCpf : string.IsNullOrEmpty(cnpjCpf) //&& c.id_pessoa_empresa == false
                            && (!papel.HasValue || c.PessoaFilhoRelacionamento.Where(r => r.RelacionamentoFilhoPapel.cd_papel == papel).Any() || c.PessoaPaiRelacionamento.Where(r => r.RelacionamentoPaiPapel.cd_papel == papel).Any())
                            && c.PessoaEmpresa.Where(r => r.cd_escola == cdEscola).Any()
                          select new FundacaoFisk.SGF.Web.Services.Secretaria.Model.PessoaSGFSearchUI
                          {
                              cd_pessoa = c.cd_pessoa,
                              no_pessoa = c.no_pessoa,
                              nm_natureza_pessoa = c.nm_natureza_pessoa,
                              dc_reduzido_pessoa = c.dc_reduzido_pessoa,
                              dt_cadastramento = c.dt_cadastramento,
                              nm_cpf_cgc = c.dc_num_cgc,
                              papeisFilhos = //c.PessoaFilhoRelacionamento.Select(r => r.RelacionamentoFilhoPapel),
                                                 (from pf in db.PapelSGF
                                                  where pf.PapelFilhoRelacionamento.Where(r => r.cd_pessoa_filho == c.cd_pessoa).Any()
                                                  select pf),
                              papeisPai = //c.PessoaPaiRelacionamento.Select(r => r.RelacionamentoPaiPapel)
                                            (from pf in db.PapelSGF
                                             where pf.PapelPaiRelacionamento.Where(r => r.cd_pessoa_pai == c.cd_pessoa).Any()
                                             select pf)
                          };
                if (tipo == TipoPessoaEnum.PESSOA_RELACIONADA)
                    sql = from c in sql
                          where !(from f in db.FuncionarioSGF where f.cd_pessoa_funcionario == c.cd_pessoa && f.cd_pessoa_empresa == cdEscola select f.cd_pessoa_funcionario).Any()
                            && !(from f in db.Aluno where f.cd_pessoa_aluno == c.cd_pessoa && f.cd_pessoa_escola == cdEscola select f.cd_pessoa_aluno).Any()
                          select c;

                sql = sorter.Sort(sql);

                if (!string.IsNullOrEmpty(nome))
                    if (inicio == true)
                        sql = from c in sql
                              where c.no_pessoa.StartsWith(nome)
                              select c;
                    else
                        sql = from c in sql
                              where c.no_pessoa.Contains(nome)
                              select c;

                if (!string.IsNullOrEmpty(apelido))
                    if (inicio == true)
                        sql = from c in sql
                              where c.dc_reduzido_pessoa.StartsWith(apelido)
                              select c;
                    else
                        sql = from c in sql
                              where c.dc_reduzido_pessoa.Contains(apelido)
                              select c;

                int limite = sql.Count();

                parametros.ajustaParametrosPesquisa(limite);
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite).AsQueryable();
                parametros.qtd_limite = limite;

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<FundacaoFisk.SGF.Web.Services.Secretaria.Model.PessoaSGFSearchUI> getPessoaUsuarioSearch(SearchParameters parametros, string nome, string apelido, string cnpjCpf, int sexo, bool inicio, int cdEscola, int cd_pessoa_usuario)
        {
            try
            {
                IEntitySorter<FundacaoFisk.SGF.Web.Services.Secretaria.Model.PessoaSGFSearchUI> sorter = EntitySorter<FundacaoFisk.SGF.Web.Services.Secretaria.Model.PessoaSGFSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<FundacaoFisk.SGF.Web.Services.Secretaria.Model.PessoaSGFSearchUI> sql;

                if (cd_pessoa_usuario == 0)
                    sql = from p in db.PessoaSGF.OfType<PessoaFisicaSGF>().AsNoTracking()
                          where (!string.IsNullOrEmpty(cnpjCpf) ? (p.PessoaSGFQueUsoOCpf.nm_cpf == cnpjCpf || p.nm_cpf == cnpjCpf) : true)
                             && p.UsuarioWebSGF.Where(u => u.Empresas.Where(ue => ue.cd_pessoa_empresa == cdEscola).Any()
                             && u.id_usuario_ativo == true).Any()
                          select new FundacaoFisk.SGF.Web.Services.Secretaria.Model.PessoaSGFSearchUI
                          {
                              cd_pessoa = p.cd_pessoa,
                              no_pessoa = p.no_pessoa,
                              nm_natureza_pessoa = p.nm_natureza_pessoa,
                              dc_reduzido_pessoa = p.dc_reduzido_pessoa,
                              dt_cadastramento = p.dt_cadastramento,
                              nm_cpf_cgc = p.cd_pessoa_cpf != null ? p.PessoaSGFQueUsoOCpf.nm_cpf : p.nm_cpf,
                              nm_sexo = p.nm_sexo,
                              no_pessoa_dependente = p.PessoaSGFQueUsoOCpf.no_pessoa
                          };
                else
                    sql = from p in db.PessoaSGF.OfType<PessoaFisicaSGF>().AsNoTracking()
                          where (!string.IsNullOrEmpty(cnpjCpf) ? (p.PessoaSGFQueUsoOCpf.nm_cpf == cnpjCpf || p.nm_cpf == cnpjCpf) : true)
                             && p.UsuarioWebSGF.Where(u => u.Empresas.Where(ue => ue.cd_pessoa_empresa == cdEscola).Any()
                             && u.id_usuario_ativo == true).Any()
                             && p.UsuarioWebSGF.Any(u => u.cd_pessoa == cd_pessoa_usuario)
                          select new FundacaoFisk.SGF.Web.Services.Secretaria.Model.PessoaSGFSearchUI
                          {
                              cd_pessoa = p.cd_pessoa,
                              no_pessoa = p.no_pessoa,
                              nm_natureza_pessoa = p.nm_natureza_pessoa,
                              dc_reduzido_pessoa = p.dc_reduzido_pessoa,
                              dt_cadastramento = p.dt_cadastramento,
                              nm_cpf_cgc = p.cd_pessoa_cpf != null ? p.PessoaSGFQueUsoOCpf.nm_cpf : p.nm_cpf,
                              nm_sexo = p.nm_sexo,
                              no_pessoa_dependente = p.PessoaSGFQueUsoOCpf.no_pessoa
                          };
                if (sexo > 0)
                    sql = from p in sql
                          where p.nm_sexo == sexo
                          select p;

                sql = sorter.Sort(sql);

                if (!string.IsNullOrEmpty(nome))
                    if (inicio == true)
                        sql = from c in sql
                              where c.no_pessoa.StartsWith(nome)
                              select c;
                    else
                        sql = from c in sql
                              where c.no_pessoa.Contains(nome)
                              select c;

                if (!string.IsNullOrEmpty(apelido))
                    if (inicio == true)
                        sql = from c in sql
                              where c.dc_reduzido_pessoa.StartsWith(apelido)
                              select c;
                    else
                        sql = from c in sql
                              where c.dc_reduzido_pessoa.Contains(apelido)
                              select c;

                int limite = sql.Count();

                parametros.ajustaParametrosPesquisa(limite);
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite).AsQueryable();
                parametros.qtd_limite = limite;

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<PessoaSGFSearchUI> getPessoaMovimentoSearch(SearchParameters parametros, string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int sexo, int tipoMovimento, int cd_empresa)
        {
            try
            {
                IEntitySorter<PessoaSGFSearchUI> sorter = EntitySorter<PessoaSGFSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<PessoaSGFSearchUI> sql;

                if (tipoPessoa.Equals((int)PessoaSGF.TipoPessoa.FISICA))
                    sql = from p in db.PessoaSGF.OfType<PessoaFisicaSGF>().AsNoTracking()
                          where (!string.IsNullOrEmpty(cnpjCpf) ? (p.PessoaSGFQueUsoOCpf.nm_cpf == cnpjCpf || p.nm_cpf == cnpjCpf) : true)
                            && p.MovimentosPessoa.Any(m => m.id_tipo_movimento == tipoMovimento && m.cd_pessoa_empresa == cd_empresa)
                          select new PessoaSGFSearchUI
                          {
                              cd_pessoa = p.cd_pessoa,
                              no_pessoa = p.no_pessoa,
                              dc_num_pessoa = p.dc_num_pessoa,
                              nm_natureza_pessoa = p.nm_natureza_pessoa,
                              dc_reduzido_pessoa = p.dc_reduzido_pessoa,
                              dt_cadastramento = p.dt_cadastramento,
                              nm_cpf_cgc = p.cd_pessoa_cpf != null ? p.PessoaSGFQueUsoOCpf.nm_cpf : p.nm_cpf,
                              no_pessoa_dependente = p.PessoaSGFQueUsoOCpf.no_pessoa,
                              nm_sexo = p.nm_sexo
                          };

                else if (tipoPessoa.Equals((int)PessoaSGF.TipoPessoa.JURIDICA))
                    sql = from c in db.PessoaSGF.OfType<PessoaJuridicaSGF>().AsNoTracking()
                          where !string.IsNullOrEmpty(cnpjCpf) ? c.dc_num_cgc == cnpjCpf : string.IsNullOrEmpty(cnpjCpf) //&& c.id_pessoa_empresa == false
                           && c.MovimentosPessoa.Any(m => m.id_tipo_movimento == tipoMovimento && m.cd_pessoa_empresa == cd_empresa)
                          select new PessoaSGFSearchUI
                          {
                              cd_pessoa = c.cd_pessoa,
                              no_pessoa = c.no_pessoa,
                              dc_num_pessoa = c.dc_num_pessoa,
                              nm_natureza_pessoa = c.nm_natureza_pessoa,
                              dc_reduzido_pessoa = c.dc_reduzido_pessoa,
                              dt_cadastramento = c.dt_cadastramento,
                              nm_cpf_cgc = c.dc_num_cgc,
                              nm_sexo = null
                          };
                else
                    sql = from c in db.PessoaSGF.AsNoTracking()
                          where //c.id_pessoa_empresa == false &&
                           c.MovimentosPessoa.Any(m => m.id_tipo_movimento == tipoMovimento && m.cd_pessoa_empresa == cd_empresa)
                          select new PessoaSGFSearchUI
                          {
                              cd_pessoa = c.cd_pessoa,
                              no_pessoa = c.no_pessoa,
                              dc_num_pessoa = c.dc_num_pessoa,
                              nm_natureza_pessoa = c.nm_natureza_pessoa,
                              dc_reduzido_pessoa = c.dc_reduzido_pessoa,
                              dt_cadastramento = c.dt_cadastramento,
                              nm_cpf_cgc = c.nm_natureza_pessoa == 1 ?//Pessoa Fisica
                                       db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().cd_pessoa_cpf != null ?
                                          db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().PessoaSGFQueUsoOCpf.nm_cpf :
                                               db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf :
                                                  db.PessoaSGF.OfType<PessoaJuridicaSGF>().Where(pj => pj.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaJuridicaSGF>().dc_num_cgc,
                              no_pessoa_dependente = c.nm_natureza_pessoa == 1 ?//Pessoa Fisica
                                          db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().PessoaSGFQueUsoOCpf.no_pessoa : "",
                              nm_sexo = c.nm_natureza_pessoa == 1 ? db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_sexo : null
                          };

                sql = sorter.Sort(sql);

                if (!string.IsNullOrEmpty(nome))
                    if (inicio)
                        sql = from c in sql
                              where c.no_pessoa.StartsWith(nome)
                              select c;
                    else
                        sql = from c in sql
                              where c.no_pessoa.Contains(nome)
                              select c;

                if (!string.IsNullOrEmpty(apelido))
                    if (inicio)
                        sql = from c in sql
                              where c.dc_reduzido_pessoa.StartsWith(apelido)
                              select c;
                    else
                        sql = from c in sql
                              where c.dc_reduzido_pessoa.Contains(apelido)
                              select c;
                if (sexo > 0)
                    sql = from c in sql
                          where c.nm_sexo == sexo
                          select c;

                int limite = sql.Count();

                parametros.ajustaParametrosPesquisa(limite);
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<PessoaSearchUI> getPessoaRelacionadaEscola(SearchParameters parametros, string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int sexo, int cd_empresa)
        {
            try
            {
                IEntitySorter<PessoaSearchUI> sorter = EntitySorter<PessoaSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<PessoaSearchUI> sql;

                if (tipoPessoa.Equals((int)PessoaSGF.TipoPessoa.FISICA))
                    sql = from p in db.PessoaSGF.OfType<PessoaFisicaSGF>().AsNoTracking()
                          where (!string.IsNullOrEmpty(cnpjCpf) ? p.nm_cpf == cnpjCpf : true) //&& !p.id_pessoa_empresa
                            && (
                                p.PessoaEmpresa.Where(pe => pe.cd_escola == cd_empresa).Any()
                                || p.PessoaFisicaFuncioanrio.Where(pff => pff.cd_pessoa_empresa == cd_empresa).Any()
                                || p.Alunos.Where(a => a.cd_pessoa_escola == cd_empresa).Any()
                            )
                          select new PessoaSearchUI
                          {
                              cd_pessoa = p.cd_pessoa,
                              no_pessoa = p.no_pessoa,
                              dc_num_pessoa = p.dc_num_pessoa,
                              nm_natureza_pessoa = p.nm_natureza_pessoa,
                              dc_reduzido_pessoa = p.dc_reduzido_pessoa,
                              dt_cadastramento = p.dt_cadastramento,
                              nm_cpf_cgc = p.nm_cpf,
                              nm_sexo = p.nm_sexo
                          };

                else if (tipoPessoa.Equals((int)PessoaSGF.TipoPessoa.JURIDICA))
                    sql = from c in db.PessoaSGF.OfType<PessoaJuridicaSGF>().AsNoTracking()
                          where (!string.IsNullOrEmpty(cnpjCpf) ? c.dc_num_cgc == cnpjCpf : true) //&& c.id_pessoa_empresa == false
                            && c.PessoaEmpresa.Where(pe => pe.cd_escola == cd_empresa).Any()
                          select new PessoaSearchUI
                          {
                              cd_pessoa = c.cd_pessoa,
                              no_pessoa = c.no_pessoa,
                              dc_num_pessoa = c.dc_num_pessoa,
                              nm_natureza_pessoa = c.nm_natureza_pessoa,
                              dc_reduzido_pessoa = c.dc_reduzido_pessoa,
                              dt_cadastramento = c.dt_cadastramento,
                              nm_cpf_cgc = c.dc_num_cgc,
                              nm_sexo = null
                          };
                else
                    sql = from c in db.PessoaSGF.AsNoTracking()
                          where (c.nm_natureza_pessoa == 1 ?
                                    (from p in db.FuncionarioSGF where p.cd_pessoa_funcionario == c.cd_pessoa && p.cd_pessoa_empresa == cd_empresa select p.cd_pessoa_funcionario).Any() ||
                                    (from a in db.Aluno where a.cd_pessoa_aluno == c.cd_pessoa && a.cd_pessoa_escola == cd_empresa select a.cd_pessoa_aluno).Any() ||
                                     c.PessoaEmpresa.Where(pe => pe.cd_escola == cd_empresa).Any()
                                : c.PessoaEmpresa.Where(pe => pe.cd_escola == cd_empresa).Any())

                          select new PessoaSearchUI
                          {
                              cd_pessoa = c.cd_pessoa,
                              no_pessoa = c.no_pessoa,
                              dc_num_pessoa = c.dc_num_pessoa,
                              nm_natureza_pessoa = c.nm_natureza_pessoa,
                              dc_reduzido_pessoa = c.dc_reduzido_pessoa,
                              dt_cadastramento = c.dt_cadastramento,
                              nm_cpf_cgc = c.nm_natureza_pessoa == 1 ? db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf : db.PessoaSGF.OfType<PessoaJuridicaSGF>().Where(pj => pj.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaJuridicaSGF>().dc_num_cgc,
                              nm_sexo = c.nm_natureza_pessoa == 1 ? db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_sexo : null
                          };

                if (!string.IsNullOrEmpty(nome))
                    if (inicio)
                        sql = from c in sql
                              where c.no_pessoa.StartsWith(nome)
                              select c;
                    else
                        sql = from c in sql
                              where c.no_pessoa.Contains(nome)
                              select c;

                if (!string.IsNullOrEmpty(apelido))
                    if (inicio)
                        sql = from c in sql
                              where c.dc_reduzido_pessoa.StartsWith(apelido)
                              select c;
                    else
                        sql = from c in sql
                              where c.dc_reduzido_pessoa.Contains(apelido)
                              select c;

                if (sexo > 0)
                    sql = from c in sql
                          where c.nm_sexo == sexo
                          select c;
                sql = sorter.Sort(sql);
                int limite = sql.Count();

                parametros.ajustaParametrosPesquisa(limite);
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<PessoaSGFSearchUI> getPessoaTituloSearch(SearchParameters parametros, string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int sexo, int cd_empresa, bool responsavel)
        {
            try
            {
                IEntitySorter<PessoaSGFSearchUI> sorter = EntitySorter<PessoaSGFSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<PessoaSGFSearchUI> sql;

                 if (tipoPessoa.Equals((int)PessoaSGF.TipoPessoa.FISICA))
                 {
                     var sql1 = (from c in db.PessoaSGF.OfType<PessoaFisicaSGF>()
                                 where
                                 (!string.IsNullOrEmpty(cnpjCpf) ? c.nm_cpf == cnpjCpf : true) &&
                                 (c.PessoaEmpresa.Where(pe => pe.cd_escola == cd_empresa).Any()
                                 || c.PessoaFisicaFuncioanrio.Where(pff => pff.cd_pessoa_empresa == cd_empresa).Any()
                                 || c.Alunos.Where(a => a.cd_pessoa_escola == cd_empresa).Any())
                                 select c);
                     sql = from c in sql1
                           where
                          (responsavel ? c.TituloResponsavel.Any(x => x.cd_pessoa_empresa == cd_empresa /*&& x.cd_pessoa_responsavel != x.cd_pessoa_titulo*/) :
                                              c.TituloPessoa.Any(x => x.cd_pessoa_empresa == cd_empresa))
                           select new PessoaSGFSearchUI
                           {
                               cd_pessoa = c.cd_pessoa,
                               no_pessoa = c.no_pessoa,
                               dc_num_pessoa = c.dc_num_pessoa,
                               nm_natureza_pessoa = c.nm_natureza_pessoa,
                               dc_reduzido_pessoa = c.dc_reduzido_pessoa,
                               dt_cadastramento = c.dt_cadastramento,
                               nm_cpf_cgc = c.cd_pessoa_cpf != null ? c.PessoaSGFQueUsoOCpf.nm_cpf : c.nm_cpf,
                               nm_sexo = c.nm_sexo
                           };
                 }
                 else if (tipoPessoa.Equals((int)PessoaSGF.TipoPessoa.JURIDICA)){
                    var sql2 = (from c in db.PessoaSGF.OfType<PessoaJuridicaSGF>()
                            where
                            (!string.IsNullOrEmpty(cnpjCpf) ? c.dc_num_cgc == cnpjCpf : true) &&
                            c.PessoaEmpresa.Where(pe => pe.cd_escola == cd_empresa).Any()
                            select c);
                    sql = from c in sql2
                          where
                       (responsavel ? c.TituloResponsavel.Any(x => x.cd_pessoa_empresa == cd_empresa && x.cd_pessoa_responsavel != x.cd_pessoa_titulo) :
                                                           c.TituloPessoa.Any(x => x.cd_pessoa_empresa == cd_empresa))

                          select new PessoaSGFSearchUI
                          {
                              cd_pessoa = c.cd_pessoa,
                              no_pessoa = c.no_pessoa,
                              dc_num_pessoa = c.dc_num_pessoa,
                              nm_natureza_pessoa = c.nm_natureza_pessoa,
                              dc_reduzido_pessoa = c.dc_reduzido_pessoa,
                              dt_cadastramento = c.dt_cadastramento,
                              nm_cpf_cgc = c.dc_num_cgc,
                              nm_sexo = null
                          };
                 }
                 else
                 {
                     var sql1 = (from c in db.PessoaSGF
                                 where
                                 c.nm_natureza_pessoa == (int)PessoaSGF.TipoPessoa.FISICA ?
                                (from pf in db.PessoaSGF.OfType<PessoaFisicaSGF>()
                                 where
                                 (!string.IsNullOrEmpty(cnpjCpf) ? pf.nm_cpf == cnpjCpf : true) &&
                                 (pf.PessoaEmpresa.Where(pe => pe.cd_escola == cd_empresa).Any()
                                 || pf.PessoaFisicaFuncioanrio.Where(pff => pff.cd_pessoa_empresa == cd_empresa).Any()
                                 || pf.Alunos.Where(a => a.cd_pessoa_escola == cd_empresa).Any())
                                 select c).Any() :
                                 (from pj in db.PessoaSGF.OfType<PessoaJuridicaSGF>()
                                    where
                                    (!string.IsNullOrEmpty(cnpjCpf) ? pj.dc_num_cgc == cnpjCpf : true) &&
                                    pj.PessoaEmpresa.Where(pe => pe.cd_escola == cd_empresa).Any()
                                    select c).Any()

                                 //c.PessoaEmpresa.Where(pe => pe.cd_escola == cd_empresa).Any()
                                 //|| db.FuncionarioSGF.Where(pff => pff.cd_pessoa_empresa == cd_empresa && pff.cd_pessoa_funcionario == c.cd_pessoa).Any()
                                 //|| db.Aluno.Where(a => a.cd_pessoa_escola == cd_empresa && a.cd_pessoa_aluno == c.cd_pessoa).Any()
                                 //|| db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.cd_pessoa).Any()
                                 //|| db.PessoaSGF.OfType<PessoaJuridicaSGF>().Where(pj => pj.cd_pessoa == c.cd_pessoa).Any()
                                 select c);

                     sql = from c in sql1
                           where
                          (responsavel ? c.TituloResponsavel.Any(x => x.cd_pessoa_empresa == cd_empresa && x.cd_pessoa_responsavel != x.cd_pessoa_titulo) :
                                                              c.TituloPessoa.Any(x => x.cd_pessoa_empresa == cd_empresa))
                           select new PessoaSGFSearchUI
                           {
                               cd_pessoa = c.cd_pessoa,
                               no_pessoa = c.no_pessoa,
                               dc_num_pessoa = c.dc_num_pessoa,
                               nm_natureza_pessoa = c.nm_natureza_pessoa,
                               dc_reduzido_pessoa = c.dc_reduzido_pessoa,
                               dt_cadastramento = c.dt_cadastramento,
                               nm_cpf_cgc = c.nm_natureza_pessoa == (int)PessoaSGF.TipoPessoa.FISICA ? db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf :
                                   db.PessoaSGF.OfType<PessoaJuridicaSGF>().Where(pj => pj.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaJuridicaSGF>().dc_num_cgc,
                               nm_sexo = c.nm_natureza_pessoa == (int)PessoaSGF.TipoPessoa.FISICA ? db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_sexo : null
                           };
                 }

                sql = sorter.Sort(sql);

                if (!string.IsNullOrEmpty(nome))
                    if (inicio)
                        sql = from c in sql
                              where c.no_pessoa.StartsWith(nome)
                              select c;
                    else
                        sql = from c in sql
                              where c.no_pessoa.Contains(nome)
                              select c;

                if (!string.IsNullOrEmpty(apelido))
                    if (inicio)
                        sql = from c in sql
                              where c.dc_reduzido_pessoa.StartsWith(apelido)
                              select c;
                    else
                        sql = from c in sql
                              where c.dc_reduzido_pessoa.Contains(apelido)
                              select c;
                if (sexo > 0)
                    sql = from c in sql
                          where c.nm_sexo == sexo
                          select c;

                int limite = sql.Count();

                parametros.ajustaParametrosPesquisa(limite);
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<PessoaSearchUI> getPessoaSearchEscolaWithCPFCNPJ(SearchParameters parametros, string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int sexo, int cd_empresa, int papel)
        {
            try
            {
                IEntitySorter<PessoaSearchUI> sorter = EntitySorter<PessoaSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<PessoaSearchUI> sql;
                //Trazer somente as pessoas que tenham CPF ou CNPJ (donas).
                if (!tipoPessoa.Equals((int)PessoaSGF.TipoPessoa.JURIDICA))
                    sql = from p in db.PessoaSGF.OfType<PessoaFisicaSGF>().AsNoTracking()
                      where !string.IsNullOrEmpty(cnpjCpf) ? p.nm_cpf == cnpjCpf : p.cd_pessoa_cpf == null && p.nm_cpf != null &&
                        (
                            p.PessoaEmpresa.Where(pe => pe.cd_escola == cd_empresa).Any()
                            || p.PessoaFisicaFuncioanrio.Where(pff => pff.cd_pessoa_empresa == cd_empresa).Any()
                            || p.Alunos.Where(a => a.cd_pessoa_escola == cd_empresa).Any()
                        )
                      select new PessoaSearchUI
                      {
                          cd_pessoa = p.cd_pessoa,
                          no_pessoa = p.no_pessoa,
                          dc_num_pessoa = p.dc_num_pessoa,
                          nm_natureza_pessoa = p.nm_natureza_pessoa,
                          dc_reduzido_pessoa = p.dc_reduzido_pessoa,
                          dt_cadastramento = p.dt_cadastramento,
                          id_pessoa_empresa = p.id_pessoa_empresa,
                          nm_cpf_cgc = p.cd_pessoa_cpf != null ? p.PessoaSGFQueUsoOCpf.nm_cpf : p.nm_cpf,
                          no_pessoa_dependente = p.PessoaSGFQueUsoOCpf.no_pessoa,
                          nm_sexo = p.nm_natureza_pessoa == 1 ? db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == p.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_sexo : null,
                          telefone = p.Telefone.dc_fone_mail
                      };

                else
                    sql = from p in db.PessoaSGF.OfType<PessoaJuridicaSGF>().AsNoTracking()
                          where !string.IsNullOrEmpty(cnpjCpf) ? p.dc_num_cgc == cnpjCpf : p.dc_num_cgc != null 
                          && (((p.PessoaEmpresa.Where(pe => pe.cd_escola == cd_empresa).Any()) && papel != (int)PapelSGF.TipoPapelSGF.FRANQUEADO) || 
                              ((db.PessoaSGF.OfType<Escola>().Where(pe => pe.cd_pessoa == p.cd_pessoa).Any()) && papel == (int)PapelSGF.TipoPapelSGF.FRANQUEADO))
                          select new PessoaSearchUI
                          {
                              cd_pessoa = p.cd_pessoa,
                              no_pessoa = p.no_pessoa,
                              dc_num_pessoa = p.dc_num_pessoa,
                              nm_natureza_pessoa = p.nm_natureza_pessoa,
                              dc_reduzido_pessoa = p.dc_reduzido_pessoa,
                              dt_cadastramento = p.dt_cadastramento,
                              id_pessoa_empresa = p.id_pessoa_empresa,
                              nm_cpf_cgc = p.dc_num_cgc,
                              telefone = p.Telefone.dc_fone_mail
                          };

                sql = sorter.Sort(sql);

                if (!string.IsNullOrEmpty(nome))
                    if (inicio)
                        sql = from c in sql
                              where c.no_pessoa.StartsWith(nome)
                              select c;
                    else
                        sql = from c in sql
                              where c.no_pessoa.Contains(nome)
                              select c;

                if (!string.IsNullOrEmpty(apelido))
                    if (inicio)
                        sql = from c in sql
                              where c.dc_reduzido_pessoa.StartsWith(apelido)
                              select c;
                    else
                        sql = from c in sql
                              where c.dc_reduzido_pessoa.Contains(apelido)
                              select c;
                if (sexo > 0)
                    sql = from c in sql
                          where c.nm_sexo == sexo
                          select c;
                int limite = sql.Count();

                parametros.ajustaParametrosPesquisa(limite);
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<PessoaSearchUI> getPessoaResponsavelCPFSearchEscola(SearchParameters parametros, string nome, string apelido, bool inicio, string cnpjCpf, int sexo, int cd_empresa)
        {
            try
            {
                IEntitySorter<PessoaSearchUI> sorter = EntitySorter<PessoaSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<PessoaSearchUI> sql;

                sql = from p in db.PessoaSGF.OfType<PessoaFisicaSGF>().AsNoTracking()
                      where !string.IsNullOrEmpty(cnpjCpf) ? p.nm_cpf == cnpjCpf : true && p.cd_pessoa_cpf == null && p.nm_cpf != null //&& !p.id_pessoa_empresa
                            && (
                                p.PessoaEmpresa.Where(pe => pe.cd_escola == cd_empresa).Any()
                                || p.PessoaFisicaFuncioanrio.Where(pff => pff.cd_pessoa_empresa == cd_empresa).Any()
                                || p.Alunos.Where(a => a.cd_pessoa_escola == cd_empresa).Any()
                            )
                      select new PessoaSearchUI
                      {
                          cd_pessoa = p.cd_pessoa,
                          no_pessoa = p.no_pessoa,
                          dc_num_pessoa = p.dc_num_pessoa,
                          nm_natureza_pessoa = p.nm_natureza_pessoa,
                          dc_reduzido_pessoa = p.dc_reduzido_pessoa,
                          dt_cadastramento = p.dt_cadastramento,
                          id_pessoa_empresa = p.id_pessoa_empresa,
                          nm_cpf_cgc = p.nm_cpf,
                          nm_sexo = p.nm_sexo
                      };

                sql = sorter.Sort(sql);

                if (!string.IsNullOrEmpty(nome))
                    if (inicio)
                        sql = from c in sql
                              where c.no_pessoa.StartsWith(nome)
                              select c;
                    else
                        sql = from c in sql
                              where c.no_pessoa.Contains(nome)
                              select c;

                if (!string.IsNullOrEmpty(apelido))
                    if (inicio)
                        sql = from c in sql
                              where c.dc_reduzido_pessoa.StartsWith(apelido)
                              select c;
                    else
                        sql = from c in sql
                              where c.dc_reduzido_pessoa.Contains(apelido)
                              select c;
                if (sexo > 0)
                    sql = from p in sql
                          where p.nm_sexo == sexo
                          select p;


                int limite = sql.Count();

                parametros.ajustaParametrosPesquisa(limite);
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<FundacaoFisk.SGF.Web.Services.Secretaria.Model.PessoaSGFSearchUI> getPessoaPapelSearchWithCPFCNPJ(SearchParameters parametros, string nome, string apelido, int tipoPessoa, string cnpjCpf, int? papel, int sexo, bool inicio, int cdEscola)
        {
            try
            {
                IEntitySorter<FundacaoFisk.SGF.Web.Services.Secretaria.Model.PessoaSGFSearchUI> sorter = EntitySorter<FundacaoFisk.SGF.Web.Services.Secretaria.Model.PessoaSGFSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<FundacaoFisk.SGF.Web.Services.Secretaria.Model.PessoaSGFSearchUI> sql;

                sql = from c in db.PessoaSGF.AsNoTracking()
                      where //c.id_pessoa_empresa == false &&
                             (!papel.HasValue || c.PessoaFilhoRelacionamento.Where(r => r.RelacionamentoFilhoPapel.cd_papel == papel).Any() || c.PessoaPaiRelacionamento.Where(rp => rp.RelacionamentoPaiPapel.cd_papel == papel).Any())
                            && c.PessoaEmpresa.Where(r => r.cd_escola == cdEscola).Any()
                      select new FundacaoFisk.SGF.Web.Services.Secretaria.Model.PessoaSGFSearchUI
                      {
                          cd_pessoa = c.cd_pessoa,
                          no_pessoa = c.no_pessoa,
                          nm_natureza_pessoa = c.nm_natureza_pessoa,
                          dc_reduzido_pessoa = c.dc_reduzido_pessoa,
                          dt_cadastramento = c.dt_cadastramento,
                          nm_cpf_cgc = c.nm_natureza_pessoa == 1 ?//Pessoa Fisica
                                        db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().cd_pessoa_cpf != null ?
                                           db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().PessoaSGFQueUsoOCpf.nm_cpf :
                                                db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf :
                          db.PessoaSGF.OfType<PessoaJuridicaSGF>().Where(pj => pj.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaJuridicaSGF>().dc_num_cgc,
                          no_pessoa_dependente = c.nm_natureza_pessoa == 1 ?//Pessoa Fisica
                                           db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().PessoaSGFQueUsoOCpf.no_pessoa : "",
                          papeisFilhos = (from pf in db.PapelSGF
                                          where pf.PapelFilhoRelacionamento.Where(r => r.cd_pessoa_filho == c.cd_pessoa).Any()
                                          select pf),
                          papeisPai = //c.PessoaPaiRelacionamento.Select(r => r.RelacionamentoPaiPapel)
                                      (from pf in db.PapelSGF
                                       where pf.PapelPaiRelacionamento.Where(r => r.cd_pessoa_pai == c.cd_pessoa).Any()
                                       select pf)
                      };
                if (tipoPessoa.Equals(1))
                {
                    sql = from p in db.PessoaSGF.OfType<PessoaFisicaSGF>().AsNoTracking()
                          where !string.IsNullOrEmpty(cnpjCpf) ? (p.PessoaSGFQueUsoOCpf.nm_cpf == cnpjCpf || p.nm_cpf == cnpjCpf) : string.IsNullOrEmpty(cnpjCpf) && //!p.id_pessoa_empresa &&
                                (!papel.HasValue || p.PessoaFilhoRelacionamento.Where(r => r.RelacionamentoFilhoPapel.cd_papel == papel).Any() ||
                                  p.PessoaPaiRelacionamento.Where(r => r.RelacionamentoPaiPapel.cd_papel == papel).Any()) &&
                                  p.PessoaEmpresa.Where(r => r.cd_escola == cdEscola).Any()
                          select new FundacaoFisk.SGF.Web.Services.Secretaria.Model.PessoaSGFSearchUI
                          {
                              cd_pessoa = p.cd_pessoa,
                              no_pessoa = p.no_pessoa,
                              nm_natureza_pessoa = p.nm_natureza_pessoa,
                              dc_reduzido_pessoa = p.dc_reduzido_pessoa,
                              dt_cadastramento = p.dt_cadastramento,
                              nm_cpf_cgc = p.cd_pessoa_cpf != null ? p.PessoaSGFQueUsoOCpf.nm_cpf : p.nm_cpf,
                              nm_sexo = p.nm_sexo,
                              papeisFilhos = //c.PessoaFilhoRelacionamento.Select(r => r.RelacionamentoFilhoPapel),
                                              (from pf in db.PapelSGF
                                               where pf.PapelFilhoRelacionamento.Where(r => r.cd_pessoa_filho == p.cd_pessoa).Any()
                                               select pf),
                              papeisPai = //c.PessoaPaiRelacionamento.Select(r => r.RelacionamentoPaiPapel)
                                            (from pf in db.PapelSGF
                                             where pf.PapelPaiRelacionamento.Where(r => r.cd_pessoa_pai == p.cd_pessoa).Any()
                                             select pf),
                              no_pessoa_dependente = p.PessoaSGFQueUsoOCpf.no_pessoa,
                          };

                    if (sexo > 0)
                        sql = from p in sql
                              where p.nm_sexo == sexo
                              select p;
                }

                else if (tipoPessoa.Equals(2))
                    sql = from c in db.PessoaSGF.OfType<PessoaJuridicaSGF>().AsNoTracking()
                          where !string.IsNullOrEmpty(cnpjCpf) ? c.dc_num_cgc == cnpjCpf : string.IsNullOrEmpty(cnpjCpf) //&& !c.id_pessoa_empresa
                            && (!papel.HasValue || c.PessoaFilhoRelacionamento.Where(r => r.RelacionamentoFilhoPapel.cd_papel == papel).Any() || c.PessoaPaiRelacionamento.Where(r => r.RelacionamentoPaiPapel.cd_papel == papel).Any())
                            && c.PessoaEmpresa.Where(r => r.cd_escola == cdEscola).Any()
                          select new FundacaoFisk.SGF.Web.Services.Secretaria.Model.PessoaSGFSearchUI
                          {
                              cd_pessoa = c.cd_pessoa,
                              no_pessoa = c.no_pessoa,
                              nm_natureza_pessoa = c.nm_natureza_pessoa,
                              dc_reduzido_pessoa = c.dc_reduzido_pessoa,
                              dt_cadastramento = c.dt_cadastramento,
                              nm_cpf_cgc = c.dc_num_cgc,
                              papeisFilhos = //c.PessoaFilhoRelacionamento.Select(r => r.RelacionamentoFilhoPapel),
                                                 (from pf in db.PapelSGF
                                                  where pf.PapelFilhoRelacionamento.Where(r => r.cd_pessoa_filho == c.cd_pessoa).Any()
                                                  select pf),
                              papeisPai = //c.PessoaPaiRelacionamento.Select(r => r.RelacionamentoPaiPapel)
                                            (from pf in db.PapelSGF
                                             where pf.PapelPaiRelacionamento.Where(r => r.cd_pessoa_pai == c.cd_pessoa).Any()
                                             select pf)
                          };
                sql = sorter.Sort(sql);

                if (!string.IsNullOrEmpty(nome))
                    if (inicio == true)
                        sql = from c in sql
                              where c.no_pessoa.StartsWith(nome)
                              select c;
                    else
                        sql = from c in sql
                              where c.no_pessoa.Contains(nome)
                              select c;

                if (!string.IsNullOrEmpty(apelido))
                    if (inicio == true)
                        sql = from c in sql
                              where c.dc_reduzido_pessoa.StartsWith(apelido)
                              select c;
                    else
                        sql = from c in sql
                              where c.dc_reduzido_pessoa.Contains(apelido)
                              select c;

                int limite = sql.Count();

                parametros.ajustaParametrosPesquisa(limite);
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite).AsQueryable();
                parametros.qtd_limite = limite;

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public PessoaSGF getPessoaById(int cd_pessoa)
        {
            try
            {
                PessoaSGF retorno = (from p in db.PessoaSGF
                                     where p.cd_pessoa == cd_pessoa
                                     select new
                                     {
                                         cd_pessoa = p.cd_pessoa,
                                         no_pessoa = p.no_pessoa
                                     }).ToList().Select(x => new PessoaSGF
                                  {
                                      cd_pessoa = x.cd_pessoa,
                                      no_pessoa = x.no_pessoa
                                  }).FirstOrDefault();
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public PessoaSGF verificaExistePessoaEmpresaByCpf(string cpf, int cd_empresa)
        {
            try
            {
                PessoaSGF retorno = (from p in db.PessoaSGF.OfType<PessoaFisicaSGF>()
                                     where p.nm_cpf == cpf && p.PessoaEmpresa.Where(x => x.cd_escola == cd_empresa).Any()
                                     select new
                                     {
                                         cd_pessoa = p.cd_pessoa,
                                         no_pessoa = p.no_pessoa
                                     }).ToList().Select(x => new PessoaSGF
                                     {
                                         cd_pessoa = x.cd_pessoa,
                                         no_pessoa = x.no_pessoa
                                     }).FirstOrDefault();
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public PessoaSGF verificaExistePessoaJuridicaEmpresaByCpf(string cnpj, int cd_empresa)
        {
            try
            {

                PessoaSGF retorno = (from p in db.PessoaSGF.OfType<PessoaJuridicaSGF>()
                                     where p.dc_num_cgc == cnpj && p.PessoaEmpresa.Where(x => x.cd_escola == cd_empresa).Any()
                                     select new
                                     {
                                         cd_pessoa = p.cd_pessoa,
                                         no_pessoa = p.no_pessoa
                                     }).ToList().Select(x => new PessoaSGF
                                     {
                                         cd_pessoa = x.cd_pessoa,
                                         no_pessoa = x.no_pessoa
                                     }).FirstOrDefault();
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<PessoaSGF> getListaAniversariantes(int cd_escola, int tipo, int cd_turma, int mes, int dia)
        {
            try
            {
                var sql = from p in db.PessoaSGF.OfType<PessoaFisicaSGF>()
                          where p.dt_nascimento != null
                          select p;
                switch (tipo)
                {
                    case (int)PessoaSGF.TipoRelatorioAniversariantes.ALUNOS_ATIVOS:
                        sql = from p in sql
                              where p.Alunos.Any(x => x.id_aluno_ativo && x.cd_pessoa_escola == cd_escola &&
                                                      (cd_turma == 0 || x.AlunoTurma.Where(at => at.cd_turma == cd_turma || at.Turma.cd_turma_ppt == cd_turma).Any()) &&
                                                       x.HistoricoAluno.Any(h => (h.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                                                  h.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado) &&
                                                                                  (cd_turma == 0 || (h.cd_turma == cd_turma || h.Turma.cd_turma_ppt == cd_turma)) &&
                                                                               !db.HistoricoAluno.Any(ha => ha.cd_turma == h.cd_turma && ha.cd_aluno == h.cd_aluno && ha.cd_produto == h.cd_produto && ha.nm_sequencia > h.nm_sequencia))
                                                                               )
                              select p;
                        break;
                    case (int)PessoaSGF.TipoRelatorioAniversariantes.ALUNOS_DESISTENTES:
                        sql = from p in sql
                              where p.Alunos.Any(x => x.cd_pessoa_escola == cd_escola && x.Contratos.Any() &&
                                                      x.HistoricoAluno.Any(h => h.id_situacao_historico == (byte)FundacaoFisk.SGF.GenericModel.HistoricoAluno.SituacaoHistorico.DESISTENTE &&
                                                                                (cd_turma == 0 || (h.cd_turma == cd_turma || h.Turma.cd_turma_ppt == cd_turma)) &&
                                                                               !db.HistoricoAluno.Any(ha => (cd_turma == 0 || (ha.cd_turma == cd_turma || ha.Turma.cd_turma_ppt == cd_turma)) && ha.cd_aluno == h.cd_aluno && ha.cd_produto == h.cd_produto && ha.nm_sequencia > h.nm_sequencia)))
                              select p;
                        break;
                    case (int)PessoaSGF.TipoRelatorioAniversariantes.CLIENTES:
                        sql = from p in sql
                              where p.Alunos.Where(x => x.cd_pessoa_escola == cd_escola && !x.Contratos.Any()).Any()
                              select p;
                        break;
                    case (int)PessoaSGF.TipoRelatorioAniversariantes.EX_ALUNOS:
                        DateTime dataAnterior = new DateTime(DateTime.Now.Year - 1, DateTime.Now.Month, DateTime.Now.Day);
                        DateTime dataAtual = DateTime.Now.Date;
                        sql = from p in sql
                              where p.Alunos.Any(x => x.cd_pessoa_escola == cd_escola && x.HistoricoAluno.Any() &&
                                    !x.HistoricoAluno.Where(ha => (cd_turma == 0 || (ha.cd_turma == cd_turma || ha.Turma.cd_turma_ppt == cd_turma)) &&
                                                                   (ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                                   ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado) &&
                                                                   DbFunctions.TruncateTime(ha.dt_historico) <= dataAnterior.Date &&
                                                                   ha.nm_sequencia >= x.HistoricoAluno.Where(han => DbFunctions.TruncateTime(han.dt_historico) <= dataAnterior.Date &&
                                                                   (cd_turma == 0 || (han.cd_turma == cd_turma || han.Turma.cd_turma_ppt == cd_turma))).Max(m => m.nm_sequencia)
                                                           ).Any() &&
                                    !x.HistoricoAluno.Where(h => (cd_turma == 0 || (h.cd_turma == cd_turma || h.Turma.cd_turma_ppt == cd_turma)) &&
                                                                 DbFunctions.TruncateTime(h.dt_historico) >= dataAnterior &&
                                                                 DbFunctions.TruncateTime(h.dt_historico) <= DbFunctions.TruncateTime(dataAtual) &&
                                                                 (h.id_situacao_historico == (byte)FundacaoFisk.SGF.GenericModel.HistoricoAluno.SituacaoHistorico.ATIVO ||
                                                                 h.id_situacao_historico == (byte)FundacaoFisk.SGF.GenericModel.HistoricoAluno.SituacaoHistorico.REMATRICULADO)).Any())
                              select p;
                        break;
                    case (int)PessoaSGF.TipoRelatorioAniversariantes.FUNCIONÁRIOS_PROFESSOR:
                        sql = from p in sql
                              where p.PessoaFisicaFuncioanrio.Any(x => x.cd_pessoa_empresa == cd_escola && x.id_funcionario_ativo)
                              //db.FuncionarioSGF.OfType<Professor>().Any(x => x.cd_pessoa_funcionario == p.cd_pessoa && x.cd_pessoa_empresa == cd_escola && x.id_funcionario_ativo)
                              select p;
                        break;
                    default:
                        sql = from p in sql
                              where p.PessoaEmpresa.Any(pe => pe.cd_escola == cd_escola) ||
                                    p.Alunos.Any(a => a.cd_pessoa_escola == cd_escola) ||
                                    p.PessoaFisicaFuncioanrio.Any(x => x.cd_pessoa_empresa == cd_escola)
                              select p;
                        break;

                }
                if (cd_turma > 0)
                    sql = from p in sql
                          where p.Alunos.Where(x => x.AlunoTurma.Where(a => a.cd_turma == cd_turma || a.Turma.cd_turma_ppt == cd_turma).Any()).Any()
                          select p;
                if (mes > 0)
                    sql = from p in sql
                          where p.dt_nascimento.Value.Month == mes
                          select p;

                if (dia > 0)
                    sql = from p in sql
                          where p.dt_nascimento.Value.Day == dia
                          select p;
                var retorno = (from p in sql
                               orderby p.dt_nascimento.Value.Day ascending, p.dt_nascimento.Value.Month ascending
                               select new
                               {
                                   p.dt_nascimento,
                                   p.no_pessoa
                               }).ToList().Select(x => new PessoaSGF
                              {
                                  dt_cadastramento = x.dt_nascimento.HasValue ? (DateTime)x.dt_nascimento : DateTime.MinValue,
                                  no_pessoa = x.no_pessoa
                              }).ToList();
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<PessoaSearchUI> getTdsPessoaSearchEscola(SearchParameters parametros, string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int sexo, int cd_empresa)
        {
            try
            {
                IEntitySorter<PessoaSearchUI> sorter = EntitySorter<PessoaSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<PessoaSearchUI> sql;
                //Trazer somente as pessoas que tenham CPF ou CNPJ (donas).
                sql = from c in db.PessoaSGF.AsNoTracking()
                      where (c.nm_natureza_pessoa == (int)PessoaSGF.TipoPessoa.FISICA ?
                                (from p in db.FuncionarioSGF where p.cd_pessoa_funcionario == c.cd_pessoa && p.cd_pessoa_empresa == cd_empresa select p.cd_pessoa_funcionario).Any() ||
                                (from a in db.Aluno where a.cd_pessoa_aluno == c.cd_pessoa && a.cd_pessoa_escola == cd_empresa select a.cd_pessoa_aluno).Any() ||
                                 c.PessoaEmpresa.Where(pe => pe.cd_escola == cd_empresa).Any()
                            : c.PessoaEmpresa.Where(pe => pe.cd_escola == cd_empresa).Any())

                      select new PessoaSearchUI
                      {
                          cd_pessoa = c.cd_pessoa,
                          no_pessoa = c.no_pessoa,
                          dc_num_pessoa = c.dc_num_pessoa,
                          nm_natureza_pessoa = c.nm_natureza_pessoa,
                          dc_reduzido_pessoa = c.dc_reduzido_pessoa,
                          dt_cadastramento = c.dt_cadastramento,
                          nm_cpf_cgc = c.nm_natureza_pessoa == (int)PessoaSGF.TipoPessoa.FISICA ? db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf :
                          db.PessoaSGF.OfType<PessoaJuridicaSGF>().Where(pj => pj.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaJuridicaSGF>().dc_num_cgc,
                          nm_sexo = c.nm_natureza_pessoa == (int)PessoaSGF.TipoPessoa.FISICA ? db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_sexo : null
                      };

                if (tipoPessoa.Equals((int)PessoaSGF.TipoPessoa.FISICA))
                    sql = from p in db.PessoaSGF.OfType<PessoaFisicaSGF>().AsNoTracking()
                          where !string.IsNullOrEmpty(cnpjCpf) ? p.nm_cpf == cnpjCpf : //p.id_pessoa_empresa == false &&
                            (
                                p.PessoaEmpresa.Where(pe => pe.cd_escola == cd_empresa).Any()
                                || p.PessoaFisicaFuncioanrio.Where(pff => pff.cd_pessoa_empresa == cd_empresa).Any()
                                || p.Alunos.Where(a => a.cd_pessoa_escola == cd_empresa).Any()
                            )
                          select new PessoaSearchUI
                          {
                              cd_pessoa = p.cd_pessoa,
                              no_pessoa = p.no_pessoa,
                              dc_num_pessoa = p.dc_num_pessoa,
                              nm_natureza_pessoa = p.nm_natureza_pessoa,
                              dc_reduzido_pessoa = p.dc_reduzido_pessoa,
                              dt_cadastramento = p.dt_cadastramento,
                              id_pessoa_empresa = p.id_pessoa_empresa,
                              nm_cpf_cgc = p.cd_pessoa_cpf != null ? p.PessoaSGFQueUsoOCpf.nm_cpf : p.nm_cpf,
                              no_pessoa_dependente = p.PessoaSGFQueUsoOCpf.no_pessoa,
                              nm_sexo = p.nm_natureza_pessoa == 1 ? db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == p.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_sexo : null
                          };

                if (tipoPessoa.Equals((int)PessoaSGF.TipoPessoa.JURIDICA))
                    sql = from p in db.PessoaSGF.OfType<PessoaJuridicaSGF>().AsNoTracking()
                          where !string.IsNullOrEmpty(cnpjCpf) ? p.dc_num_cgc == cnpjCpf : p.dc_num_cgc != null && //p.id_pessoa_empresa == false &&
                          p.PessoaEmpresa.Where(pe => pe.cd_escola == cd_empresa).Any()
                          select new PessoaSearchUI
                          {
                              cd_pessoa = p.cd_pessoa,
                              no_pessoa = p.no_pessoa,
                              dc_num_pessoa = p.dc_num_pessoa,
                              nm_natureza_pessoa = p.nm_natureza_pessoa,
                              dc_reduzido_pessoa = p.dc_reduzido_pessoa,
                              dt_cadastramento = p.dt_cadastramento,
                              id_pessoa_empresa = p.id_pessoa_empresa,
                              nm_cpf_cgc = p.dc_num_cgc
                          };

                sql = sorter.Sort(sql);

                if (!string.IsNullOrEmpty(nome))
                    if (inicio)
                        sql = from c in sql
                              where c.no_pessoa.StartsWith(nome)
                              select c;
                    else
                        sql = from c in sql
                              where c.no_pessoa.Contains(nome)
                              select c;

                if (!string.IsNullOrEmpty(apelido))
                    if (inicio)
                        sql = from c in sql
                              where c.dc_reduzido_pessoa.StartsWith(apelido)
                              select c;
                    else
                        sql = from c in sql
                              where c.dc_reduzido_pessoa.Contains(apelido)
                              select c;
                if (sexo > 0)
                    sql = from c in sql
                          where c.nm_sexo == sexo
                          select c;
                int limite = sql.Count();

                parametros.ajustaParametrosPesquisa(limite);
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Contrato getMatriculaByTurmaAlunoHistorico(int cd_escola, int cd_aluno, int cd_contrato)
        {
            try
            {

                Contrato sql = (from c in db.Contrato
                                where c.cd_pessoa_escola == cd_escola && c.cd_aluno == cd_aluno && c.cd_contrato == cd_contrato
                                select new { c.cd_contrato, c.dt_matricula_contrato }).ToList()
                                .Select(x => new Contrato { cd_contrato = x.cd_contrato, dt_matricula_contrato = x.dt_matricula_contrato }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        #endregion

        #region Aluno

        public IEnumerable<AlunoSearchUI> getAlunoSearchFollowUp(Componentes.Utils.SearchParameters parametros, string desc, bool inicio, int cdEscola, string email, string telefone, TipoConsultaAlunoEnum tipo)
        {
            try
            {
                IEntitySorter<AlunoSearchUI> sorter = EntitySorter<AlunoSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<AlunoSearchUI> sql;

                var sql1 = from aluno in db.Aluno.AsNoTracking()
                           where aluno.cd_pessoa_escola == cdEscola && aluno.id_aluno_ativo
                           select aluno;

                if (tipo == TipoConsultaAlunoEnum.HAS_ALUNO_FOLLOWUP)
                    sql1 = from aluno in db.Aluno.AsNoTracking()
                           where aluno.AlunoFollowUp.Any()
                           select aluno;
                if (!String.IsNullOrEmpty(email))
                {
                    if (inicio)
                        sql1 = from aluno in sql1
                               where aluno.AlunoPessoaFisica.TelefonePessoa.Any(t => t.dc_fone_mail.StartsWith(email))
                               select aluno;
                    else
                        sql1 = from aluno in sql1
                               where aluno.AlunoPessoaFisica.TelefonePessoa.Any(t => t.dc_fone_mail.Contains(email))
                               select aluno;
                }

                if (!String.IsNullOrEmpty(telefone))
                {
                    sql1 = from aluno in sql1
                           where aluno.AlunoPessoaFisica.TelefonePessoa.Where(pf => pf.dc_fone_mail.Contains(telefone)).Any()
                           select aluno;
                }

                sql = from aluno in sql1
                      where aluno.cd_pessoa_escola == cdEscola
                      select new AlunoSearchUI
                      {
                          cd_aluno = aluno.cd_aluno,
                          cd_pessoa_aluno = aluno.cd_aluno,
                          no_pessoa = aluno.AlunoPessoaFisica.no_pessoa,
                          email = db.TelefoneSGF.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL && t.cd_pessoa == aluno.cd_pessoa_aluno).FirstOrDefault().dc_fone_mail,
                          telefone = aluno.AlunoPessoaFisica.Telefone.dc_fone_mail,
                          celular = db.TelefoneSGF.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.CELULAR && t.cd_pessoa == aluno.cd_pessoa_aluno).FirstOrDefault().dc_fone_mail
                      };

                sql = sorter.Sort(sql);
                var retorno = from aluno in sql
                              select aluno;
                if (!String.IsNullOrEmpty(desc))
                {
                    if (inicio)
                    {
                        retorno = from aluno in sql
                                  where aluno.no_pessoa.StartsWith(desc)
                                  select aluno;
                    }//end if
                    else
                    {
                        retorno = from aluno in sql
                                  where aluno.no_pessoa.Contains(desc)
                                  select aluno;
                    }//end else

                }

                int limite = retorno.Count();

                parametros.ajustaParametrosPesquisa(limite);
                retorno = retorno.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<AlunoSearchUI> findAluno(int cdEscola)
        {
            try
            {
                IQueryable<AlunoSearchUI> retorno = null;
                retorno = from aluno in db.Aluno
                          join pessoa in db.PessoaSGF
                          on aluno.cd_pessoa_aluno equals pessoa.cd_pessoa
                          where aluno.cd_pessoa_escola == cdEscola
                          select new AlunoSearchUI
                          {
                              cd_aluno = aluno.cd_aluno,
                              cd_pessoa_escola = aluno.cd_pessoa_escola,
                              cd_pessoa_aluno = pessoa.cd_pessoa,
                              no_pessoa = pessoa.no_pessoa,
                              id_aluno_ativo = aluno.id_aluno_ativo
                          };
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Aluno getAlunoById(int idAluno, int cdEscola)
        {
            try
            {
                var sql = (from c in db.Aluno
                           where c.cd_aluno == idAluno &&
                                 c.cd_pessoa_escola == cdEscola
                           select c).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Aluno firstOrDefault(int cdEscola)
        {
            try
            {
                var sql = (from c in db.Aluno
                           where c.cd_pessoa_escola == cdEscola
                           select c).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<AlunoSearchUI> getAlunoFKReajusteSearch(SearchParameters parametros, string nome, string email, bool inicio, bool? status, int cdEscola, string cpf, List<int> cdSituacao, int sexo, bool semTurma, bool movido, int tipoAluno, DateTime? dt_inicial, DateTime? dt_final)
        {
            try
            {
                IEntitySorter<AlunoSearchUI> sorter = EntitySorter<AlunoSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<AlunoSearchUI> retorno;
                var sql = from c in db.Aluno.AsNoTracking()
                          where c.cd_pessoa_escola == cdEscola
                          select c;

                if (dt_inicial.HasValue && dt_final.HasValue)
                {
                    //os alunos  que possuem títulos abertos, que não possuem baixas e nem Cnab emitido, no período selecionado
                    sql = from c in sql
                          join pf in db.PessoaSGF.OfType<PessoaFisicaSGF>() on c.cd_pessoa_aluno equals pf.cd_pessoa
                          where pf.TituloPessoa.Where(tp => tp.id_status_titulo == (byte)FundacaoFisk.SGF.GenericModel.Titulo.StatusTitulo.ABERTO
                                                      && tp.id_status_cnab == (byte)FundacaoFisk.SGF.GenericModel.Titulo.StatusCnabTitulo.INICIAL
                                                      && tp.dt_vcto_titulo >= dt_inicial.Value
                                                      && tp.dt_vcto_titulo <= dt_final.Value
                                                      && !tp.BaixaTitulo.Any(x => x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA &&
                                                                                  x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO)
                                  ).Any()
                          //&& !pf.TituloPessoa.Where(tp => tp.id_status_titulo == (byte)FundacaoFisk.SGF.GenericModel.Titulo.StatusTitulo.FECHADO
                          //          && tp.dt_vcto_titulo >= dt_inicial.Value
                          //          && tp.dt_vcto_titulo <= dt_final.Value
                          //).Any()
                          //&& !pf.TituloPessoa.Where(tp => tp.id_status_cnab != (byte)FundacaoFisk.SGF.GenericModel.Titulo.StatusCnabTitulo.INICIAL
                          //          && tp.dt_vcto_titulo >= dt_inicial.Value
                          //          && tp.dt_vcto_titulo <= dt_final.Value
                          //).Any()
                          select c;
                }
                if (sexo > 0)
                    sql = from a in sql
                          where a.AlunoPessoaFisica.nm_sexo == sexo
                          select a;
                if (!string.IsNullOrEmpty(nome))
                {
                    if (inicio)
                        sql = from a in sql
                              where a.AlunoPessoaFisica.no_pessoa.StartsWith(nome)
                              select a;
                    else
                        sql = from c in sql
                              where c.AlunoPessoaFisica.no_pessoa.Contains(nome)
                              select c;
                }
                if (!string.IsNullOrEmpty(cpf))
                    sql = from c in sql
                          where c.AlunoPessoaFisica.nm_cpf == cpf
                          select c;
                if (!string.IsNullOrEmpty(email))
                    if (inicio)
                        sql = from a in sql
                              where (from t in db.TelefoneSGF
                                     where t.cd_pessoa == a.cd_pessoa_aluno &&
                                         t.cd_tipo_telefone == 4 && t.id_telefone_principal == true && t.dc_fone_mail.StartsWith(email)
                                     select t.cd_telefone).Any()
                              select a;
                    else
                        sql = from a in sql
                              where (from t in db.TelefoneSGF
                                     where t.cd_pessoa == a.cd_pessoa_aluno &&
                                         t.cd_tipo_telefone == 4 && t.id_telefone_principal == true && t.dc_fone_mail.Contains(email)
                                     select t.cd_telefone).Any()
                              select a;

                if (status != null)
                    sql = from a in sql
                          where a.id_aluno_ativo == status
                          select a;
                if (!cdSituacao.Contains(100))
                    sql = from c in sql
                          where db.AlunoTurma.Where(a => a.cd_aluno == c.cd_aluno && a.cd_situacao_aluno_turma != null && cdSituacao.Contains((int)a.cd_situacao_aluno_turma)).Any()
                          select c;
                if (movido)
                    sql = from s in sql
                          where db.AlunoTurma.Where(a => a.cd_aluno == s.cd_aluno && a.cd_contrato == null && a.cd_turma_origem > 0).Any()
                          select s;
                if (semTurma)
                    sql = from s in sql
                          where !db.AlunoTurma.Where(a => a.cd_aluno == s.cd_aluno).Any()
                          select s;
                if (tipoAluno > 0)
                    switch (tipoAluno)
                    {
                        case (int)TipoConsultaAlunoEnum.ALUNO:
                            sql = from a in sql
                                  where a.Contratos.Any()
                                  select a;
                            break;
                        case (int)TipoConsultaAlunoEnum.ALUNO_CLIENTE:
                            sql = from a in sql
                                  where !a.Contratos.Any()
                                  select a;
                            break;
                    }
                retorno = from c in sql
                          join pf in db.PessoaSGF.OfType<PessoaFisicaSGF>().AsNoTracking() on c.cd_pessoa_aluno equals pf.cd_pessoa
                          where c.cd_pessoa_escola == cdEscola
                          select new AlunoSearchUI
                          {
                              cd_aluno = c.cd_aluno,
                              cd_pessoa_aluno = pf.cd_pessoa,
                              cd_pessoa_escola = c.cd_pessoa_escola,
                              cd_pessoa = pf.cd_pessoa,
                              no_pessoa = pf.no_pessoa,
                              dc_reduzido_pessoa = pf.dc_reduzido_pessoa,
                              dt_cadastramento = pf.dt_cadastramento,
                              id_aluno_ativo = c.id_aluno_ativo,
                              nm_cpf = pf.cd_pessoa_cpf > 0 ? pf.PessoaSGFQueUsoOCpf.nm_cpf : pf.nm_cpf,
                              no_pessoa_dependente = pf.PessoaSGFQueUsoOCpf.no_pessoa,
                              cd_pessoa_dependente = pf.PessoaSGFQueUsoOCpf.cd_pessoa,
                              ext_img_pessoa = pf.ext_img_pessoa,
                              no_atividade = pf.Atividade.no_atividade,
                              email = pf.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL).FirstOrDefault().dc_fone_mail,
                              telefone = pf.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.TELEFONE).Any() ? pf.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.TELEFONE).FirstOrDefault().dc_fone_mail : pf.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.CELULAR).FirstOrDefault().dc_fone_mail,
                              ativExtra = c.AtividadeExtraAluno.Where(at => at.ind_participacao == false).Count(),
                              pc_bolsa = c.Bolsa.pc_bolsa,
                              pc_bolsa_material = c.Bolsa.pc_bolsa_material,
                              dt_inicio_bolsa = c.Bolsa.dt_inicio_bolsa
                          };

                retorno = sorter.Sort(retorno);

                int limite = retorno.Count();

                parametros.ajustaParametrosPesquisa(limite);
                retorno = retorno.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;

                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<AlunoSearchUI> getAlunoSearch(SearchParameters parametros, string nome, string email, bool inicio, bool? status, int cdEscola,
            string cpf, List<int> cdSituacao, int sexo, bool semTurma, bool movido, int tipoAluno, bool matriculasem, bool matricula)
        {
            try
            {
                IEntitySorter<AlunoSearchUI> sorter = EntitySorter<AlunoSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<AlunoSearchUI> retorno;
                var sql = from a in db.Aluno
                          join pf in db.PessoaSGF.OfType<PessoaFisicaSGF>() on a.cd_pessoa_aluno equals pf.cd_pessoa
                          where a.cd_pessoa_escola == cdEscola
                          select a;

                if (sexo > 0)
                    sql = from a in sql
                          where a.AlunoPessoaFisica.nm_sexo == sexo
                          select a;
                if (!string.IsNullOrEmpty(nome))
                {
                    if (inicio)
                        sql = from a in sql
                              where a.AlunoPessoaFisica.no_pessoa.StartsWith(nome)
                              select a;
                    else
                        sql = from c in sql
                              where c.AlunoPessoaFisica.no_pessoa.Contains(nome)
                              select c;
                }
                if (!string.IsNullOrEmpty(cpf))
                    sql = from c in sql
                          where c.AlunoPessoaFisica.nm_cpf == cpf
                          select c;
                if (!string.IsNullOrEmpty(email))
                    if (inicio)
                        sql = from a in sql
                              where (from t in db.TelefoneSGF
                                     where t.cd_pessoa == a.cd_pessoa_aluno &&
                                         t.cd_tipo_telefone == 4 && t.id_telefone_principal == true && t.dc_fone_mail.StartsWith(email)
                                     select t.cd_telefone).Any()
                              select a;
                    else
                        sql = from a in sql
                              where (from t in db.TelefoneSGF
                                     where t.cd_pessoa == a.cd_pessoa_aluno &&
                                         t.cd_tipo_telefone == 4 && t.id_telefone_principal == true && t.dc_fone_mail.Contains(email)
                                     select t.cd_telefone).Any()
                              select a;

                if (status != null)
                    sql = from a in sql
                          where a.id_aluno_ativo == status
                          select a;
                if (!cdSituacao.Contains(100))
                    sql = from c in sql
                          where (db.AlunoTurma.Where(a => a.cd_aluno == c.cd_aluno && a.cd_situacao_aluno_turma != null && cdSituacao.Contains((int)a.cd_situacao_aluno_turma)).Any() ||
                                    (matriculasem &&
                                        ((db.Contrato.Where(a => a.cd_aluno == c.cd_aluno  && !db.AlunoTurma.Where(at=>at.cd_aluno == c.cd_aluno && at.cd_contrato == a.cd_contrato).Any()))).Any() &&
                                        (cdSituacao.Contains((int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Ativo) &&
                                         cdSituacao.Contains((int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Rematriculado))))
                          select c;
                if (movido)
                    sql = from s in sql
                          where db.AlunoTurma.Where(a => a.cd_aluno == s.cd_aluno && a.cd_contrato == null && a.cd_turma_origem > 0).Any()
                          select s;
                if (semTurma)
                    sql = from s in sql
                          where !db.AlunoTurma.Where(a => a.cd_aluno == s.cd_aluno).Any()
                          select s;
                if (tipoAluno > 0)
                    switch (tipoAluno)
                    {
                        case (int)TipoConsultaAlunoEnum.ALUNO:
                            sql = from a in sql
                                  where a.Contratos.Any()
                                  select a;
                            break;
                        case (int)TipoConsultaAlunoEnum.ALUNO_CLIENTE:
                            sql = from a in sql
                                  where !a.Contratos.Any()
                                  select a;
                            break;
                    }
                if (cdSituacao.Contains(100) && matriculasem)
                    sql = from s in sql
                          where db.Contrato.Where(a => a.cd_aluno == s.cd_aluno && !db.AlunoTurma.Where(at=>at.cd_aluno == s.cd_aluno && at.cd_contrato == a.cd_contrato).Any()).Any()
                          select s;

                if (cdSituacao.Contains(100) && matricula)
                    sql = from s in sql
                          where !db.AlunoTurma.Where(at => at.cd_aluno == s.cd_aluno && at.cd_situacao_aluno_turma ==
                            (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Transferido).Any()
                          select s;

                retorno = from a in sql
                          join pf in db.PessoaSGF.OfType<PessoaFisicaSGF>() on a.cd_pessoa_aluno equals pf.cd_pessoa
                          where a.cd_pessoa_escola == cdEscola
                          select new AlunoSearchUI
                          {
                              cd_aluno = a.cd_aluno,
                              cd_pessoa_aluno = pf.cd_pessoa,
                              cd_pessoa_escola = a.cd_pessoa_escola,
                              cd_pessoa = pf.cd_pessoa,
                              no_pessoa = pf.no_pessoa,
                              dc_reduzido_pessoa = pf.dc_reduzido_pessoa,
                              dt_cadastramento = pf.dt_cadastramento,
                              id_aluno_ativo = a.id_aluno_ativo,
                              nm_cpf = pf.cd_pessoa_cpf > 0 ? pf.PessoaSGFQueUsoOCpf.nm_cpf : pf.nm_cpf,
                              no_pessoa_dependente = !String.IsNullOrEmpty(pf.PessoaSGFQueUsoOCpf.no_pessoa) ? pf.PessoaSGFQueUsoOCpf.no_pessoa : (from rl in db.RelacionamentoSGF from pp in db.PessoaSGF where rl.cd_pessoa_pai == a.cd_pessoa_aluno && rl.cd_pessoa_filho == pp.cd_pessoa && rl.cd_papel_pai == (int)RelacionamentoDataAccess.TipoRelacionamento.ALUNO_RESPONSAVEL && rl.cd_papel_filho == (int)RelacionamentoDataAccess.TipoRelacionamento.RESPONSAVEL_FINANCEIRO select pp.no_pessoa).FirstOrDefault(),
                              cd_pessoa_dependente = pf.PessoaSGFQueUsoOCpf.cd_pessoa != null ? pf.PessoaSGFQueUsoOCpf.cd_pessoa : (from rl in db.RelacionamentoSGF from pp in db.PessoaSGF where rl.cd_pessoa_pai == a.cd_pessoa_aluno && rl.cd_pessoa_filho == pp.cd_pessoa && rl.cd_papel_pai == (int)RelacionamentoDataAccess.TipoRelacionamento.ALUNO_RESPONSAVEL && rl.cd_papel_filho == (int)RelacionamentoDataAccess.TipoRelacionamento.RESPONSAVEL_FINANCEIRO select pp.cd_pessoa).FirstOrDefault(),
                              ext_img_pessoa = pf.ext_img_pessoa,
                              no_atividade = pf.Atividade.no_atividade,
                              email = pf.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL).FirstOrDefault().dc_fone_mail,
                              telefone = pf.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.TELEFONE).Any() ? pf.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.TELEFONE).FirstOrDefault().dc_fone_mail : pf.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.CELULAR).FirstOrDefault().dc_fone_mail,
                              ativExtra = a.AtividadeExtraAluno.Where(at => at.ind_participacao == false).Count(),
                              pc_bolsa = a.Bolsa.pc_bolsa,
                              pc_bolsa_material = a.Bolsa.pc_bolsa_material,
                              dt_inicio_bolsa = a.Bolsa.dt_inicio_bolsa,
                              vl_abatimento_matricula = a.Contratos.Where(x => x.vl_pre_matricula > 0).Count() > 0 ? 0 : a.Prospect != null ? a.Prospect.vl_matricula_prospect : 0
                          };

                retorno = sorter.Sort(retorno);

                int limite = sql.Count();

                parametros.ajustaParametrosPesquisa(limite);
                retorno = retorno.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;

                return retorno.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<AlunoSearchUI> getAlunoSearchFKPesquisas(SearchParameters parametros, string nome, string email, bool inicio, bool? status, int cdEscola, int origemFK, string cpf,
            List<int> cdSituacao, int sexo, bool semTurma, bool movido, int tipoAluno, int cd_pessoa_responsavel, TipoConsultaAlunoEnum tipoConsulta)
        {
            int? cdEmpresaLig = (from empresa in db.PessoaSGF.OfType<Escola>() where empresa.cd_empresa_coligada == cdEscola select empresa.cd_pessoa).FirstOrDefault();
            int cd_empresa = cdEscola;
            cdEscola = (int)((cdEmpresaLig == null || cdEmpresaLig == 0) ? cdEscola : cdEmpresaLig);
            try
            {
                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                IEntitySorter<AlunoSearchUI> sorter = EntitySorter<AlunoSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<AlunoSearchUI> retorno;
                var sql = from c in db.Aluno.AsNoTracking()
                          join pf in db.PessoaSGF.OfType<PessoaFisicaSGF>().AsNoTracking() on c.cd_pessoa_aluno equals pf.cd_pessoa
                          where (c.cd_pessoa_escola == cdEscola || origemFK == (int)OrigemConsultaEnum.ALUNOSEMAULA)
                          select c;
                if (origemFK == (int)OrigemConsultaEnum.AULAREPOSICAO)
                {
                    sql = from t in sql
                          join ar in db.AlunoAulaReposicao on t.cd_aluno equals ar.cd_aluno
                          select t;
                }
                if (origemFK == (int)OrigemConsultaEnum.ALUNOSEMAULA)
                {
                    sql = from t in sql
                          where
                            (from ta in db.vi_aluno_sem_aula where ta.cd_aluno == t.cd_aluno select ta).Any()
                          select t;
                }
                if (origemFK == (int)OrigemConsultaEnum.ENVIARTRANSFERENCIACAD)
                {
                    sql = from t in sql
                          where
                            (from pr in db.PessoaRaf where pr.cd_pessoa == t.cd_pessoa_aluno select pr).Any()
                            //LBM Eliminando Proteção && !(from ta in db.TransferenciaAluno where ta.cd_aluno_origem == t.cd_aluno && ta.cd_escola_origem == cdEscola && ta.id_status_transferencia != (int)TransferenciaAluno.StatusTransferenciaAluno.RECUSADA select ta).Any()
                          select t;
                }
                if (origemFK == (int)OrigemConsultaEnum.ENVIARTRANSFERENCIA)
                {
                    sql = from t in sql
                          where
                            (from pr in db.PessoaRaf where pr.cd_pessoa == t.cd_pessoa_aluno select pr).Any() &&
                            (from ta in db.TransferenciaAluno where ta.cd_aluno_origem == t.cd_aluno select ta).Any()
                          select t;

                }
                if (origemFK == (int)OrigemConsultaEnum.DESISTENCIA_CARGA)
                {
                    sql = from t in sql
                          where
                            (from ta in db.vi_desistencia_carga where ta.cd_aluno == t.cd_aluno select ta).Any()
                          select t;
                }
                if (origemFK == (int)OrigemConsultaEnum.CARGA_HORARIA)
                {
                    sql = from t in sql
                          where
                            (from ta in db.vi_carga_horaria where ta.cd_aluno == t.cd_aluno select ta).Any()
                          select t;
                }
                if (origemFK == (int)OrigemConsultaEnum.CARGA_HORARIA_COM_ESCOLA)
                {
                    sql = from t in sql
                          where
                              (from ta in db.vi_carga_horaria where ta.cd_aluno == t.cd_aluno && ta.cd_escola == cdEscola select ta).Any()
                          select t;
                }
                if (origemFK == (int)OrigemConsultaEnum.ALUNO_SEARCH_PERDA_MATERIAL)
                {
                    sql = from t in sql
                          where
                              (from pm in db.PerdaMaterial where pm.Movimento.cd_aluno == t.cd_aluno && pm.Movimento.cd_pessoa_empresa == cdEscola select pm).Any()
                          select t;
                }
                
                if (origemFK == (int)OrigemConsultaEnum.ALUNO_SEARCH_PERDA_MATERIAL)
                {
                    sql = from t in sql
                          where
                              (from mm in db.Movimento where mm.cd_aluno == t.cd_aluno && mm.cd_pessoa_empresa == cdEscola && mm.id_material_didatico == true select mm).Any()
                          select t;
                }

                if (tipoConsulta >= 0)
                    switch (tipoConsulta)
                    {
                        case TipoConsultaAlunoEnum.HAS_PESQ_NOTA_MATERIAL:
                            sql = from c in sql
                                  where c.Movimento.Any(m => m.id_origem_movimento == cd_origem)
                                  select c;
                            break;

                        case TipoConsultaAlunoEnum.VENDA_MATERIAL:
                            sql = from c in sql
                                  where c.Movimento.Any()
                                  select c;
                            break;
                    }

                if (sexo > 0)
                    sql = from a in sql
                          where a.AlunoPessoaFisica.nm_sexo == sexo
                          select a;
                if (!string.IsNullOrEmpty(nome))
                {
                    if (inicio)
                        sql = from a in sql
                              where a.AlunoPessoaFisica.no_pessoa.StartsWith(nome)
                              select a;
                    else
                        sql = from c in sql
                              where c.AlunoPessoaFisica.no_pessoa.Contains(nome)
                              select c;
                }
                if (!string.IsNullOrEmpty(cpf))
                    sql = from c in sql
                          where c.AlunoPessoaFisica.nm_cpf == cpf
                          select c;
                if (!string.IsNullOrEmpty(email))
                    if (inicio)
                        sql = from a in sql
                              where (from t in db.TelefoneSGF
                                     where t.cd_pessoa == a.cd_pessoa_aluno &&
                                         t.cd_tipo_telefone == 4 && t.id_telefone_principal == true && t.dc_fone_mail.StartsWith(email)
                                     select t.cd_telefone).Any()
                              select a;
                    else
                        sql = from a in sql
                              where (from t in db.TelefoneSGF
                                     where t.cd_pessoa == a.cd_pessoa_aluno &&
                                         t.cd_tipo_telefone == 4 && t.id_telefone_principal == true && t.dc_fone_mail.Contains(email)
                                     select t.cd_telefone).Any()
                              select a;

                if (status != null)
                    sql = from a in sql
                          where a.id_aluno_ativo == status
                          select a;
                if (!cdSituacao.Contains(100))
                    sql = from c in sql
                          where db.AlunoTurma.Where(a => a.cd_aluno == c.cd_aluno && a.cd_situacao_aluno_turma != null && cdSituacao.Contains((int)a.cd_situacao_aluno_turma)).Any()
                          select c;
                if (movido)
                    sql = from s in sql
                          where db.AlunoTurma.Where(a => a.cd_aluno == s.cd_aluno && a.cd_contrato == null && a.cd_turma_origem > 0).Any()
                          select s;
                if (semTurma)
                    sql = from s in sql
                          where !db.AlunoTurma.Where(a => a.cd_aluno == s.cd_aluno).Any()
                          select s;
                if (tipoAluno > 0)
                    switch (tipoAluno)
                    {
                        case (int)TipoConsultaAlunoEnum.ALUNO:
                            sql = from a in sql
                                  where a.Contratos.Any()
                                  select a;
                            break;
                        case (int)TipoConsultaAlunoEnum.ALUNO_CLIENTE:
                            sql = from a in sql
                                  where !a.Contratos.Any()
                                  select a;
                            break;
                    }
                if (cd_pessoa_responsavel > 0)
                {
                    List<int> papeis = new List<int>() { 1, 3, 7 };
                    sql = from a in sql
                          where a.AlunoPessoaFisica.PessoaPaiRelacionamento.Any(r =>
                                                                     papeis.Contains(r.cd_papel_filho) &&
                                                                     r.cd_pessoa_filho == cd_pessoa_responsavel)
                          select a;
                }

                retorno = from c in sql
                          join pf in db.PessoaSGF.OfType<PessoaFisicaSGF>().AsNoTracking() on c.cd_pessoa_aluno equals pf.cd_pessoa
                          where (c.cd_pessoa_escola == cdEscola || origemFK == (int)OrigemConsultaEnum.ALUNOSEMAULA)
                          select new AlunoSearchUI
                          {
                              cd_aluno = c.cd_aluno,
                              cd_pessoa_aluno = pf.cd_pessoa,
                              cd_pessoa_escola = c.cd_pessoa_escola,
                              dc_reduzido_pessoa_escola = c.EscolaAluno.dc_reduzido_pessoa,
                              cd_pessoa = pf.cd_pessoa,
                              no_pessoa = pf.no_pessoa,
                              dc_reduzido_pessoa = pf.dc_reduzido_pessoa,
                              dt_cadastramento = pf.dt_cadastramento,
                              id_aluno_ativo = c.id_aluno_ativo,
                              nm_cpf = pf.cd_pessoa_cpf > 0 ? pf.PessoaSGFQueUsoOCpf.nm_cpf : pf.nm_cpf,
                              no_pessoa_dependente = pf.PessoaSGFQueUsoOCpf.no_pessoa,
                              cd_pessoa_dependente = pf.PessoaSGFQueUsoOCpf.cd_pessoa,
                              ext_img_pessoa = pf.ext_img_pessoa,
                              no_atividade = pf.Atividade.no_atividade,
                              email = pf.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL).FirstOrDefault().dc_fone_mail,
                              telefone = pf.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.TELEFONE).Any() ? pf.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.TELEFONE).FirstOrDefault().dc_fone_mail : pf.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.CELULAR).FirstOrDefault().dc_fone_mail,
                              ativExtra = c.AtividadeExtraAluno.Where(at => at.ind_participacao == false).Count()
                          };

                retorno = sorter.Sort(retorno);

                int limite = sql.Count();

                parametros.ajustaParametrosPesquisa(limite);
                retorno = retorno.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }


        public IEnumerable<AlunoSearchUI> getAlunoSearchFKPesquisasAtividade(SearchParameters parametros, string nome, string email, bool inicio, bool? status, int cdEscola, int origemFK, string cpf,
            List<int> cdSituacao, int sexo, bool semTurma, bool movido, int tipoAluno, int cd_pessoa_responsavel, TipoConsultaAlunoEnum tipoConsulta, int cd_escola_combo_fk, List<int> cds_escolas_atividade)
        {
            try
            {
                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                IEntitySorter<AlunoSearchUI> sorter = EntitySorter<AlunoSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<AlunoSearchUI> retorno;
                IQueryable<Aluno> sql;
                
                sql = from c in db.Aluno.AsNoTracking()
                      join pf in db.PessoaSGF.OfType<PessoaFisicaSGF>().AsNoTracking() on c.cd_pessoa_aluno equals pf.cd_pessoa
                      where (c.cd_pessoa_escola == cdEscola)
                      select c;

                if (origemFK == 28)
                {
                    sql = from t in sql
                          join ar in db.AlunoAulaReposicao on t.cd_aluno equals ar.cd_aluno
                          select t;
                }

                if (tipoConsulta >= 0)
                    switch (tipoConsulta)
                    {
                        case TipoConsultaAlunoEnum.HAS_PESQ_NOTA_MATERIAL:
                            sql = from c in sql
                                  where c.Movimento.Any(m => m.id_origem_movimento == cd_origem)
                                  select c;
                            break;

                        case TipoConsultaAlunoEnum.VENDA_MATERIAL:
                            sql = from c in sql
                                  where c.Movimento.Any()
                                  select c;
                            break;
                    }

                if (sexo > 0)
                    sql = from a in sql
                          where a.AlunoPessoaFisica.nm_sexo == sexo
                          select a;
                if (!string.IsNullOrEmpty(nome))
                {
                    if (inicio)
                        sql = from a in sql
                              where a.AlunoPessoaFisica.no_pessoa.StartsWith(nome)
                              select a;
                    else
                        sql = from c in sql
                              where c.AlunoPessoaFisica.no_pessoa.Contains(nome)
                              select c;
                }
                if (!string.IsNullOrEmpty(cpf))
                    sql = from c in sql
                          where c.AlunoPessoaFisica.nm_cpf == cpf
                          select c;
                if (!string.IsNullOrEmpty(email))
                    if (inicio)
                        sql = from a in sql
                              where (from t in db.TelefoneSGF
                                     where t.cd_pessoa == a.cd_pessoa_aluno &&
                                         t.cd_tipo_telefone == 4 && t.id_telefone_principal == true && t.dc_fone_mail.StartsWith(email)
                                     select t.cd_telefone).Any()
                              select a;
                    else
                        sql = from a in sql
                              where (from t in db.TelefoneSGF
                                     where t.cd_pessoa == a.cd_pessoa_aluno &&
                                         t.cd_tipo_telefone == 4 && t.id_telefone_principal == true && t.dc_fone_mail.Contains(email)
                                     select t.cd_telefone).Any()
                              select a;

                if (status != null)
                    sql = from a in sql
                          where a.id_aluno_ativo == status
                          select a;
                if (!cdSituacao.Contains(100))
                    sql = from c in sql
                          where db.AlunoTurma.Where(a => a.cd_aluno == c.cd_aluno && a.cd_situacao_aluno_turma != null && cdSituacao.Contains((int)a.cd_situacao_aluno_turma)).Any()
                          select c;
                if (movido)
                    sql = from s in sql
                          where db.AlunoTurma.Where(a => a.cd_aluno == s.cd_aluno && a.cd_contrato == null && a.cd_turma_origem > 0).Any()
                          select s;
                if (semTurma)
                    sql = from s in sql
                          where !db.AlunoTurma.Where(a => a.cd_aluno == s.cd_aluno).Any()
                          select s;
                if (tipoAluno > 0)
                    switch (tipoAluno)
                    {
                        case (int)TipoConsultaAlunoEnum.ALUNO:
                            sql = from a in sql
                                  where a.Contratos.Any()
                                  select a;
                            break;
                        case (int)TipoConsultaAlunoEnum.ALUNO_CLIENTE:
                            sql = from a in sql
                                  where !a.Contratos.Any()
                                  select a;
                            break;
                    }
                if (cd_pessoa_responsavel > 0)
                {
                    List<int> papeis = new List<int>() { 1, 3, 7 };
                    sql = from a in sql
                          where a.AlunoPessoaFisica.PessoaPaiRelacionamento.Any(r =>
                                                                     papeis.Contains(r.cd_papel_filho) &&
                                                                     r.cd_pessoa_filho == cd_pessoa_responsavel)
                          select a;
                }

                
                retorno = from c in sql
                          join pf in db.PessoaSGF.OfType<PessoaFisicaSGF>().AsNoTracking() on c.cd_pessoa_aluno equals pf.cd_pessoa
                          where (c.cd_pessoa_escola == cdEscola)
                          select new AlunoSearchUI
                          {
                              cd_aluno = c.cd_aluno,
                              cd_pessoa_aluno = pf.cd_pessoa,
                              cd_pessoa_escola = c.cd_pessoa_escola,
                              dc_reduzido_pessoa_escola = c.EscolaAluno.dc_reduzido_pessoa,
                              cd_pessoa = pf.cd_pessoa,
                              no_pessoa = pf.no_pessoa,
                              dc_reduzido_pessoa = pf.dc_reduzido_pessoa,
                              dt_cadastramento = pf.dt_cadastramento,
                              id_aluno_ativo = c.id_aluno_ativo,
                              nm_cpf = pf.cd_pessoa_cpf > 0 ? pf.PessoaSGFQueUsoOCpf.nm_cpf : pf.nm_cpf,
                              no_pessoa_dependente = pf.PessoaSGFQueUsoOCpf.no_pessoa,
                              cd_pessoa_dependente = pf.PessoaSGFQueUsoOCpf.cd_pessoa,
                              ext_img_pessoa = pf.ext_img_pessoa,
                              no_atividade = pf.Atividade.no_atividade,
                              email = pf.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL).FirstOrDefault().dc_fone_mail,
                              telefone = pf.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.TELEFONE).Any() ? pf.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.TELEFONE).FirstOrDefault().dc_fone_mail : pf.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.CELULAR).FirstOrDefault().dc_fone_mail,
                              ativExtra = c.AtividadeExtraAluno.Where(at => at.ind_participacao == false).Count()
                          };
                


                retorno = sorter.Sort(retorno);

                int limite = sql.Count();

                parametros.ajustaParametrosPesquisa(limite);
                retorno = retorno.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<AlunoSearchUI> getAlunoSearchFKPesquisasAtividadeExtra(SearchParameters parametros, string nome, string email, bool inicio, bool? status, int cdEscola, string cpf,
            List<int> cdSituacao, int sexo, bool semTurma, bool movido, int tipoAluno, int cd_pessoa_responsavel, TipoConsultaAlunoEnum tipoConsulta, int cd_escola_combo_fk, List<int> cursos, List<int> cds_escolas_atividade) 
        {
            try
            {
                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                IEntitySorter<AlunoSearchUI> sorter = EntitySorter<AlunoSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<AlunoSearchUI> retorno;
                IQueryable<Aluno> sql;
                
  
                    sql = from c in db.Aluno.AsNoTracking()
                          join pf in db.PessoaSGF.OfType<PessoaFisicaSGF>().AsNoTracking() on c.cd_pessoa_aluno equals pf.cd_pessoa
                          where (c.cd_pessoa_escola == cdEscola)
                          select c;

                

                if (!cursos.Contains(-1) && cursos.Count > 0)
                {
                    List<int> listSituacoesAluno = new List<int>();
                    listSituacoesAluno.Add((int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Ativo);
                    listSituacoesAluno.Add((int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Rematriculado);
                    listSituacoesAluno.Add((int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Aguardando);

                    sql = from a in sql
                          join ta in db.AlunoTurma on a.cd_aluno equals ta.cd_aluno
                          join t in db.Turma on ta.cd_turma equals t.cd_turma
                          where ta.cd_aluno == a.cd_aluno && cursos.Contains((Int32)ta.Turma.cd_curso)
                          && listSituacoesAluno.Contains((Int32)ta.cd_situacao_aluno_turma)
                          select a;
                }

                if (tipoConsulta >= 0)
                    switch (tipoConsulta)
                    {
                        case TipoConsultaAlunoEnum.HAS_PESQ_NOTA_MATERIAL:
                            sql = from c in sql
                                  where c.Movimento.Any(m => m.id_origem_movimento == cd_origem)
                                  select c;
                            break;

                        case TipoConsultaAlunoEnum.VENDA_MATERIAL:
                            sql = from c in sql
                                  where c.Movimento.Any()
                                  select c;
                            break;
                    }

                if (sexo > 0)
                    sql = from a in sql
                          where a.AlunoPessoaFisica.nm_sexo == sexo
                          select a;
                if (!string.IsNullOrEmpty(nome))
                {
                    if (inicio)
                        sql = from a in sql
                              where a.AlunoPessoaFisica.no_pessoa.StartsWith(nome)
                              select a;
                    else
                        sql = from c in sql
                              where c.AlunoPessoaFisica.no_pessoa.Contains(nome)
                              select c;
                }
                if (!string.IsNullOrEmpty(cpf))
                    sql = from c in sql
                          where c.AlunoPessoaFisica.nm_cpf == cpf
                          select c;
                if (!string.IsNullOrEmpty(email))
                    if (inicio)
                        sql = from a in sql
                              where (from t in db.TelefoneSGF
                                     where t.cd_pessoa == a.cd_pessoa_aluno &&
                                         t.cd_tipo_telefone == 4 && t.id_telefone_principal == true && t.dc_fone_mail.StartsWith(email)
                                     select t.cd_telefone).Any()
                              select a;
                    else
                        sql = from a in sql
                              where (from t in db.TelefoneSGF
                                     where t.cd_pessoa == a.cd_pessoa_aluno &&
                                         t.cd_tipo_telefone == 4 && t.id_telefone_principal == true && t.dc_fone_mail.Contains(email)
                                     select t.cd_telefone).Any()
                              select a;

                if (status != null)
                    sql = from a in sql
                          where a.id_aluno_ativo == status
                          select a;
                if (!cdSituacao.Contains(100))
                    sql = from c in sql
                          where db.AlunoTurma.Where(a => a.cd_aluno == c.cd_aluno && a.cd_situacao_aluno_turma != null && cdSituacao.Contains((int)a.cd_situacao_aluno_turma)).Any()
                          select c;
                if (movido)
                    sql = from s in sql
                          where db.AlunoTurma.Where(a => a.cd_aluno == s.cd_aluno && a.cd_contrato == null && a.cd_turma_origem > 0).Any()
                          select s;
                if (semTurma)
                    sql = from s in sql
                          where !db.AlunoTurma.Where(a => a.cd_aluno == s.cd_aluno).Any()
                          select s;
                if (tipoAluno > 0)
                    switch (tipoAluno)
                    {
                        case (int)TipoConsultaAlunoEnum.ALUNO:
                            sql = from a in sql
                                  where a.Contratos.Any()
                                  select a;
                            break;
                        case (int)TipoConsultaAlunoEnum.ALUNO_CLIENTE:
                            sql = from a in sql
                                  where !a.Contratos.Any()
                                  select a;
                            break;
                    }
                if (cd_pessoa_responsavel > 0)
                {
                    List<int> papeis = new List<int>() { 1, 3, 7 };
                    sql = from a in sql
                          where a.AlunoPessoaFisica.PessoaPaiRelacionamento.Any(r =>
                                                                     papeis.Contains(r.cd_papel_filho) &&
                                                                     r.cd_pessoa_filho == cd_pessoa_responsavel)
                          select a;
                }

                
                retorno = from c in sql
                          join pf in db.PessoaSGF.OfType<PessoaFisicaSGF>().AsNoTracking() on c.cd_pessoa_aluno equals pf.cd_pessoa
                          where (c.cd_pessoa_escola == cdEscola)
                          select new AlunoSearchUI
                          {
                              cd_aluno = c.cd_aluno,
                              cd_pessoa_aluno = pf.cd_pessoa,
                              cd_pessoa_escola = c.cd_pessoa_escola,
                              dc_reduzido_pessoa_escola = c.EscolaAluno.dc_reduzido_pessoa,
                              cd_pessoa = pf.cd_pessoa,
                              no_pessoa = pf.no_pessoa,
                              dc_reduzido_pessoa = pf.dc_reduzido_pessoa,
                              dt_cadastramento = pf.dt_cadastramento,
                              id_aluno_ativo = c.id_aluno_ativo,
                              nm_cpf = pf.cd_pessoa_cpf > 0 ? pf.PessoaSGFQueUsoOCpf.nm_cpf : pf.nm_cpf,
                              no_pessoa_dependente = pf.PessoaSGFQueUsoOCpf.no_pessoa,
                              cd_pessoa_dependente = pf.PessoaSGFQueUsoOCpf.cd_pessoa,
                              ext_img_pessoa = pf.ext_img_pessoa,
                              no_atividade = pf.Atividade.no_atividade,
                              email = pf.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL).FirstOrDefault().dc_fone_mail,
                              telefone = pf.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.TELEFONE).Any() ? pf.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.TELEFONE).FirstOrDefault().dc_fone_mail : pf.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.CELULAR).FirstOrDefault().dc_fone_mail,
                              ativExtra = c.AtividadeExtraAluno.Where(at => at.ind_participacao == false).Count()
                          };


                retorno = sorter.Sort(retorno);

                int limite = sql.Count();

                parametros.ajustaParametrosPesquisa(limite);
                retorno = retorno.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;

                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<AlunoSearchUI> getAlunoSearchTurma(SearchParameters parametros, string nome, string email, bool inicio, int origemFK, bool? status, int cdEscola, string cpf, int cdSituacao, int sexo)
        {
            try
            {
                IEntitySorter<AlunoSearchUI> sorter = EntitySorter<AlunoSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<AlunoSearchUI> sql;

                sql = from c in db.Aluno
                      join pf in db.PessoaSGF.OfType<PessoaFisicaSGF>().AsNoTracking() on c.cd_pessoa_aluno equals pf.cd_pessoa
                      where c.cd_pessoa_escola == cdEscola &&
                            c.AlunoTurma.Where(at => at.cd_aluno == c.cd_aluno).Any()
                      select new AlunoSearchUI
                      {
                          cd_aluno = c.cd_aluno,
                          cd_pessoa_aluno = pf.cd_pessoa,
                          cd_pessoa_escola = c.cd_pessoa_escola,
                          cd_pessoa = pf.cd_pessoa,
                          no_pessoa = pf.no_pessoa,
                          dc_reduzido_pessoa = pf.dc_reduzido_pessoa,
                          dt_cadastramento = pf.dt_cadastramento,
                          id_aluno_ativo = c.id_aluno_ativo,
                          nm_cpf = pf.cd_pessoa_cpf > 0 ? pf.PessoaSGFQueUsoOCpf.nm_cpf : pf.nm_cpf,
                          cd_pessoa_dependente = pf.PessoaSGFQueUsoOCpf.cd_pessoa,
                          no_pessoa_dependente = pf.PessoaSGFQueUsoOCpf.no_pessoa,
                          ext_img_pessoa = pf.ext_img_pessoa,
                          no_atividade = pf.Atividade.no_atividade,
                          ativExtra = c.AtividadeExtraAluno.Where(at => at.ind_participacao == false).Count()
                      };

                if (sexo > 0)
                {
                    sql = from c in db.Aluno.AsNoTracking()
                          join pf in db.PessoaSGF.OfType<PessoaFisicaSGF>().AsNoTracking() on c.cd_pessoa_aluno equals pf.cd_pessoa
                          where c.cd_pessoa_escola == cdEscola &&
                                c.AlunoPessoaFisica.nm_sexo == sexo
                          select new AlunoSearchUI
                          {
                              cd_aluno = c.cd_aluno,
                              cd_pessoa_aluno = pf.cd_pessoa,
                              cd_pessoa_escola = c.cd_pessoa_escola,
                              cd_pessoa = pf.cd_pessoa,
                              no_pessoa = pf.no_pessoa,
                              dc_reduzido_pessoa = pf.dc_reduzido_pessoa,
                              dt_cadastramento = pf.dt_cadastramento,
                              id_aluno_ativo = c.id_aluno_ativo,
                              nm_cpf = pf.cd_pessoa_cpf > 0 ? pf.PessoaSGFQueUsoOCpf.nm_cpf : pf.nm_cpf,
                              cd_pessoa_dependente = pf.PessoaSGFQueUsoOCpf.cd_pessoa,
                              no_pessoa_dependente = pf.PessoaSGFQueUsoOCpf.no_pessoa,
                              ext_img_pessoa = pf.ext_img_pessoa,
                              no_atividade = pf.Atividade.no_atividade,
                              ativExtra = c.AtividadeExtraAluno.Where(at => at.ind_participacao == false).Count()
                          };
                }
                else
                {
                    sql = from c in db.Aluno.AsNoTracking()
                          join pf in db.PessoaSGF.OfType<PessoaFisicaSGF>().AsNoTracking() on c.cd_pessoa_aluno equals pf.cd_pessoa
                          where c.cd_pessoa_escola == cdEscola
                          select new AlunoSearchUI
                          {
                              cd_aluno = c.cd_aluno,
                              cd_pessoa_aluno = pf.cd_pessoa,
                              cd_pessoa_escola = c.cd_pessoa_escola,
                              cd_pessoa = pf.cd_pessoa,
                              no_pessoa = pf.no_pessoa,
                              dc_reduzido_pessoa = pf.dc_reduzido_pessoa,
                              dt_cadastramento = pf.dt_cadastramento,
                              id_aluno_ativo = c.id_aluno_ativo,
                              nm_cpf = pf.cd_pessoa_cpf > 0 ? pf.PessoaSGFQueUsoOCpf.nm_cpf : pf.nm_cpf,
                              cd_pessoa_dependente = pf.PessoaSGFQueUsoOCpf.cd_pessoa,
                              no_pessoa_dependente = pf.PessoaSGFQueUsoOCpf.no_pessoa,
                              ext_img_pessoa = pf.ext_img_pessoa,
                              no_atividade = pf.Atividade.no_atividade,
                              //email = pf.TelefonePessoa.Where(t => t.cd_tipo_telefone == 4).FirstOrDefault().dc_fone_mail,
                              ativExtra = c.AtividadeExtraAluno.Where(at => at.ind_participacao == false).Count()
                          };
                }
                if (origemFK == 28)
                {
                    sql = from t in sql
                          join ar in db.AlunoAulaReposicao on t.cd_aluno equals ar.cd_aluno
                          select t;
                }

                if (!string.IsNullOrEmpty(nome))
                {
                    if (inicio)
                        sql = from func in sql
                              where func.no_pessoa.StartsWith(nome)
                              select func;
                    else
                        sql = from c in sql
                              where c.no_pessoa.Contains(nome)
                              select c;
                }
                if (!string.IsNullOrEmpty(cpf))
                    sql = from c in sql
                          where c.nm_cpf == cpf
                          select c;
                //todo
                if (!string.IsNullOrEmpty(email))
                    if (inicio)
                        sql = from c in sql
                              where (from t in db.TelefoneSGF
                                     where t.cd_pessoa == c.cd_pessoa &&
                                         t.cd_tipo_telefone == 4 && t.id_telefone_principal == true && t.dc_fone_mail.StartsWith(email)
                                     select t.cd_telefone).Any()
                              select c;
                    else
                        sql = from c in sql
                              where (from t in db.TelefoneSGF
                                     where t.cd_pessoa == c.cd_pessoa &&
                                         t.cd_tipo_telefone == 4 && t.id_telefone_principal == true && t.dc_fone_mail.Contains(email)
                                     select t.cd_telefone).Any()
                              select c;

                if (status != null)
                    sql = from c in sql
                          where c.id_aluno_ativo == status
                          select c;
                if (cdSituacao < 100)
                    sql = from c in sql
                          where db.AlunoTurma.Where(at => at.cd_situacao_aluno_turma == cdSituacao).Any()
                          select c;
                sql = sorter.Sort(sql);
                int limite = sql.Count();

                parametros.ajustaParametrosPesquisa(limite);
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Aluno getAllDataAlunoById(int cdAluno, int cdEscola, bool editView)
        {
            try
            {
                var sql = (from aluno in db.Aluno
                           where aluno.cd_aluno == cdAluno && aluno.cd_pessoa_escola == cdEscola
                           select new
                           {
                               cd_aluno = aluno.cd_aluno,
                               cd_pessoa_escola = aluno.cd_pessoa_escola,
                               cd_pessoa_aluno = aluno.cd_pessoa_aluno,
                               cd_midia = aluno.cd_midia,
                               cd_escolaridade = aluno.cd_escolaridade,
                               cd_usuario_atendente = aluno.cd_usuario_atendente,
                               id_aluno_ativo = aluno.id_aluno_ativo,
                               cd_prospect = aluno.cd_prospect,
                               no_prospect = aluno.Prospect.PessoaFisica.no_pessoa,
                               txt_obs_pessoa = aluno.AlunoPessoaFisica.txt_obs_pessoa,
                               bolsa = aluno.Bolsa,
                               fichaSaudeAluno = (from fs in db.FichaSaude where fs.cd_aluno == aluno.cd_aluno select fs).FirstOrDefault()
                           }).ToList().Select(x => new Aluno
                           {
                               cd_aluno = x.cd_aluno,
                               cd_pessoa_escola = x.cd_pessoa_escola,
                               cd_pessoa_aluno = x.cd_pessoa_aluno,
                               cd_midia = x.cd_midia,
                               cd_escolaridade = x.cd_escolaridade,
                               cd_usuario_atendente = x.cd_usuario_atendente,
                               id_aluno_ativo = x.id_aluno_ativo,
                               cd_prospect = x.cd_prospect,
                               no_prospect = x.no_prospect,
                               txt_obs_pessoa = x.txt_obs_pessoa,
                               Bolsa = x.bolsa,
                               fichaSaudeAluno = x.fichaSaudeAluno
                           }).FirstOrDefault();
                if (sql != null)
                    if (editView)
                        sql.MotivosMatricula = (from a in db.MotivoMatricula
                                                where a.AlunosMotivosMatricula.Where(am => am.cd_aluno == sql.cd_aluno).Any()
                                                select a).ToList();
                    else
                        sql.AlunosMotivosMatricula = (from a in db.AlunoMotivoMatricula
                                                      where a.cd_aluno == sql.cd_aluno
                                                      select a).ToList();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Aluno getAlunoByCpf(string cpf, string email, string nome, int cd_pessoa_cpf, int cdEscola)
        {
            try
            {
                IQueryable<Aluno> sql = null;

                if (!string.IsNullOrEmpty(cpf))
                {
                    sql = (from aluno in db.Aluno
                            where aluno.AlunoPessoaFisica.nm_cpf == cpf && aluno.cd_pessoa_escola == cdEscola
                            select aluno);
                }
                if (!string.IsNullOrEmpty(email))
                {
                    sql = (from aluno in db.Aluno
                            join contato in db.TelefoneSGF on aluno.cd_pessoa_aluno equals contato.cd_pessoa
                            where aluno.cd_pessoa_escola == cdEscola
                                    && contato.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL
                                    && contato.dc_fone_mail == email
                            select aluno);
                }

                if (!string.IsNullOrEmpty(nome) && cd_pessoa_cpf > 0)
                {
                    sql = (from aluno in db.Aluno
                            where aluno.AlunoPessoaFisica.no_pessoa == nome && aluno.AlunoPessoaFisica.cd_pessoa_cpf == cd_pessoa_cpf && aluno.cd_pessoa_escola == cdEscola
                            select aluno);
                }

                var alunoContexto = (from aluno in sql
                           select new
                           {
                               aluno.cd_aluno,
                               aluno.AlunoPessoaFisica.no_pessoa,
                               aluno.id_aluno_ativo,
                               aluno.cd_pessoa_escola
                           }).ToList().Select(x => new Aluno
                           {
                               cd_aluno = x.cd_aluno,
                               id_aluno_ativo = x.id_aluno_ativo,
                               AlunoPessoaFisica = new PessoaFisicaSGF
                               {
                                   no_pessoa = x.no_pessoa
                               },
                               cd_pessoa_escola = x.cd_pessoa_escola
                           }).FirstOrDefault();
                return alunoContexto;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Aluno getAlunoByCpfAluno(string cpf, string email, string nome, int cd_pessoa_cpf, int cdEscola)
        {
            try
            {
                IQueryable<Aluno> sql = null;
                Aluno alunoContexto = null;

                if (!string.IsNullOrEmpty(cpf))
                {
                    sql = (from aluno in db.Aluno
                           where aluno.AlunoPessoaFisica.nm_cpf == cpf && aluno.cd_pessoa_escola != cdEscola
                           select aluno);
                }

                if (sql == null || sql.Count() == 0)
                {
                    if (!string.IsNullOrEmpty(cpf))
                    {
                        sql = (from aluno in db.Aluno
                               where aluno.AlunoPessoaFisica.nm_cpf == cpf && aluno.cd_pessoa_escola == cdEscola
                               select aluno);
                    }
                    if (!string.IsNullOrEmpty(email))
                    {
                        sql = (from aluno in db.Aluno
                               join contato in db.TelefoneSGF on aluno.cd_pessoa_aluno equals contato.cd_pessoa
                               where aluno.cd_pessoa_escola == cdEscola
                                     && contato.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL
                                     && contato.dc_fone_mail == email
                               select aluno);
                    }

                    if (!string.IsNullOrEmpty(nome) && cd_pessoa_cpf > 0)
                    {
                        sql = (from aluno in db.Aluno
                               where aluno.AlunoPessoaFisica.no_pessoa == nome && aluno.AlunoPessoaFisica.cd_pessoa_cpf == cd_pessoa_cpf && aluno.cd_pessoa_escola == cdEscola
                               select aluno);
                    }
                }
                if(sql != null)
                 alunoContexto = (from aluno in sql
                                     select new
                                     {
                                         aluno.cd_aluno,
                                         aluno.AlunoPessoaFisica.no_pessoa,
                                         aluno.id_aluno_ativo,
                                         aluno.cd_pessoa_escola
                                     }).ToList().Select(x => new Aluno
                                     {
                                         cd_aluno = x.cd_aluno,
                                         id_aluno_ativo = x.id_aluno_ativo,
                                         AlunoPessoaFisica = new PessoaFisicaSGF
                                         {
                                             no_pessoa = x.no_pessoa
                                         },
                                         cd_pessoa_escola = x.cd_pessoa_escola
                                     }).FirstOrDefault();
                return alunoContexto;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<AlunoSearchUI> getAlunoByEscola(int cdEscola, bool? status, int? cd_aluno)
        {
            try
            {
                var retorno = from aluno in db.Aluno
                              join pessoa in db.PessoaSGF
                              on aluno.cd_pessoa_aluno equals pessoa.cd_pessoa
                              where aluno.cd_pessoa_escola == cdEscola
                                    && (status == null || aluno.id_aluno_ativo == status)
                                    && (cd_aluno == 0 || aluno.cd_aluno == cd_aluno)
                              select new AlunoSearchUI
                              {
                                  cd_aluno = aluno.cd_aluno,
                                  no_pessoa = pessoa.no_pessoa
                              };

                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public AlunoSearchUI verificarAlunoExistEmail(string email, int cdEscola, int cd_prospect)
        {
            try
            {
                var sql = (from aluno in db.Aluno
                           join contato in db.TelefoneSGF on aluno.cd_pessoa_aluno equals contato.cd_pessoa
                           where contato.dc_fone_mail == email
                                 && aluno.cd_pessoa_escola == cdEscola
                                 && contato.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL
                                 && (cd_prospect == 0 || aluno.cd_prospect != cd_prospect)
                           select new AlunoSearchUI
                           {
                               cd_aluno = aluno.cd_aluno,
                               no_pessoa = aluno.AlunoPessoaFisica.no_pessoa
                           }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Aluno buscarAlunoExistEmail(string email, string nome, int cd_pessoa_cpf)
        {
            try
            {
                IQueryable<Aluno> sql = null;

                if (!string.IsNullOrEmpty(nome) && cd_pessoa_cpf > 0)
                {
                    sql = (from aluno in db.Aluno
                           where aluno.AlunoPessoaFisica.cd_pessoa_cpf == cd_pessoa_cpf && aluno.AlunoPessoaFisica.no_pessoa == nome
                        select aluno);

                }
                else if (!string.IsNullOrEmpty(email))
                {
                    sql = (from aluno in db.Aluno
                        where aluno.AlunoPessoaFisica.TelefonePessoa.Where(t =>
                            t.cd_tipo_telefone == (int) TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL &&
                            t.dc_fone_mail == email).Any()
                            select aluno);
                }

                var alunoContexto = (from aluno in sql
                    select new
                           {
                               //cd_pessoa_cpf = aluno.AlunoPessoaFisica.cd_pessoa_cpf,
                               //no_prospect = aluno.AlunoPessoaFisica.no_pessoa,

                               cd_aluno = aluno.cd_aluno,
                               cd_pessoa_escola = aluno.cd_pessoa_escola,
                               cd_pessoa_aluno = aluno.cd_pessoa_aluno,
                               cd_midia = aluno.cd_midia,
                               cd_escolaridade = aluno.cd_escolaridade,
                               cd_usuario_atendente = aluno.cd_usuario_atendente,
                               cd_prospect = aluno.cd_prospect,
                               no_prospect = aluno.Prospect.PessoaFisica.no_pessoa,
                               txt_obs_pessoa = aluno.AlunoPessoaFisica.txt_obs_pessoa,
                               id_prospect_ativo = aluno.Prospect == null ? false : aluno.Prospect.id_prospect_ativo,
                               id_aluno_ativo = aluno.id_aluno_ativo,
                               bolsa = aluno.Bolsa
                           }).ToList().Select(x => new Aluno
                           {
                               //cd_pessoa_cpf = x.cd_pessoa_cpf,
                               no_prospect = x.no_prospect,

                               cd_aluno = x.cd_aluno,
                               cd_pessoa_escola = x.cd_pessoa_escola,
                               cd_pessoa_aluno = x.cd_pessoa_aluno,
                               cd_midia = x.cd_midia,
                               cd_escolaridade = x.cd_escolaridade,
                               cd_usuario_atendente = x.cd_usuario_atendente,
                               id_aluno_ativo = x.id_aluno_ativo,
                               cd_prospect = x.cd_prospect,
                               txt_obs_pessoa = x.txt_obs_pessoa,
                               Bolsa = x.bolsa
                           }).FirstOrDefault();

                if (alunoContexto != null)
                    alunoContexto.MotivosMatricula = (from a in db.MotivoMatricula
                                            where a.AlunosMotivosMatricula.Where(am => am.cd_aluno == alunoContexto.cd_aluno).Any()
                                            select a).ToList();

                return alunoContexto;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<Aluno> getAlunosByCod(int[] cdAlunos, int cdEscola)
        {
            try
            {
                var result = from aluno in db.Aluno
                             where
                                cdAlunos.Contains(aluno.cd_aluno) &&
                                aluno.cd_pessoa_escola == cdEscola
                             select aluno;
                return result.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        //Método responsavel por buscar todos alunos que estejam disponíveis nos horários de uma determinada turma (e não estão alocados no mesmos horários para outra turma).
        public IEnumerable<AlunoSearchUI> getAlunosDisponiveisFaixaHorario(SearchParameters parametros, string desc, string email, bool inicio, bool? status, string cpf, int sexo,
               List<Horario> horariosTurma, int cd_escola, int cd_turma, int cd_produto, bool id_turma_PPT, DateTime? dt_inicio_aula, int cd_curso, int cd_duracao)
        {
            IEntitySorter<AlunoSearchUI> sorter = EntitySorter<AlunoSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
            IQueryable<AlunoSearchUI> sql;
            try
            {
                IQueryable<Aluno> alunos = null;
                IQueryable<Aluno> alunosT = null;

               
                //if (id_turma_PPT)
                    alunos = from aluno in db.Aluno.AsNoTracking()
                             where aluno.cd_pessoa_escola == cd_escola  &&
                                   aluno.id_aluno_ativo &&
                                  (cd_produto == 0 || !aluno.AlunoTurma.Any(al => (al.Turma.cd_produto == cd_produto)) || 
                                    aluno.AlunoTurma.Where(al => (al.Turma.cd_produto == cd_produto )//&&
                                        //al.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Aguardando ||
                                        //al.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                        //al.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Rematriculado) //&&
                                      ).Any())
                             select aluno;
                // Pegar detalhes da turma onde está tentando inserir o aluno
                var tl = (
                    from tr in db.Curso.AsNoTracking()
                    where tr.cd_curso == cd_curso    
                    select new
                    {
                        id_permitir_matricula = tr.id_permitir_matricula
                    }
                ).FirstOrDefault();
                
                var id_permitir_matricula = tl.id_permitir_matricula;
                
                        if (horariosTurma != null && horariosTurma.Count() > 0)
                            foreach (var h in horariosTurma)
                            {
                                alunosT = from aluno in alunos
                                         where
                                        (
                                            (
                                                (from ht in db.Horario
                                                    join turma in db.Turma on ht.cd_registro equals turma.cd_turma
                                                    where
                                                    ht.id_origem == (int)Horario.Origem.TURMA &&
                                                    turma.cd_turma != cd_turma
                                                    && turma.cd_pessoa_escola == cd_escola
                                                    && turma.id_turma_ppt == false
                                                    && turma.dt_termino_turma == null 
                                                    && turma.TurmaAluno.Where(al => al.cd_aluno == aluno.cd_aluno &&
                                                     (al.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                     al.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Rematriculado) //&&
                                                    ).Any() &&
                                                    (
                                                        (
                                                            (!turma.Curso.id_permitir_matricula && !id_permitir_matricula && cd_curso != turma.cd_curso) &&
                                                            (!turma.dt_final_aula.HasValue && dt_inicio_aula > DbFunctions.AddDays(turma.dt_inicio_aula, turma.Duracao.nm_duracao == 0 ? 0 : (int)(turma.Curso.nm_carga_horaria / (double)turma.Duracao.nm_duracao * 7.0)) ||
                                                            turma.dt_final_aula.HasValue && dt_inicio_aula > turma.dt_final_aula) 
                                                        ) ||
                                                        (ht.id_dia_semana != h.id_dia_semana || (
                                                            !((h.dt_hora_ini <= ht.dt_hora_ini && ht.dt_hora_ini < h.dt_hora_fim)
                                                            || (h.dt_hora_ini < ht.dt_hora_fim && ht.dt_hora_fim <= h.dt_hora_fim)
                                                            || (h.dt_hora_ini <= ht.dt_hora_ini && ht.dt_hora_fim <= h.dt_hora_fim)
                                                            || (h.dt_hora_ini >= ht.dt_hora_ini && ht.dt_hora_fim >= h.dt_hora_fim)))
                                                        )
                                                        &&
                                                        (
                                                            (turma.cd_produto != cd_produto) ||
                                                              //ATENÇÃO: Se data final das turmas ocupadas não estiverem definidas será acrescentada 
                                                              //o número de dias da carga horária à data inicial da turma.
                                                              //A verificação para contratos sem turma será feita no postmatricula
                                                            (turma.cd_produto == cd_produto &&
                                                                (
                                                                    (!turma.dt_final_aula.HasValue && dt_inicio_aula <= DbFunctions.AddDays(turma.dt_inicio_aula, turma.Duracao.nm_duracao == 0 ? 0 : (int)(turma.Curso.nm_carga_horaria / (double)turma.Duracao.nm_duracao * 7.0)) ||
                                                                    turma.dt_final_aula.HasValue && dt_inicio_aula <= turma.dt_final_aula) &&
                                                                    (
                                                                        //Aceleração só para personalizadas no mesmo período
                                                                        (!turma.Curso.id_permitir_matricula && !id_permitir_matricula && cd_curso == turma.cd_curso &&
                                                                         id_turma_PPT && turma.cd_turma_ppt != null) || 
                                                                        (!turma.Curso.id_permitir_matricula && id_permitir_matricula) ||
                                                                        (turma.Curso.id_permitir_matricula)
                                                                    )
                                                                ) 
                                                            )
                                                        )
                                                    )
                                                    select ht.cd_horario
                                                ).Any()
                                            )
                                            ||
                                            (cd_produto == 0 || !aluno.AlunoTurma.Any(al => (al.Turma.cd_produto == cd_produto)) ||
                                             aluno.AlunoTurma.Any(a => a.Turma.cd_produto == cd_produto))
                                        ) 
                                        &&
                                        // Não exista horários disponiveis para esse aluno.
                                        (   !(from ht in db.Horario
                                             where ht.cd_registro == aluno.cd_aluno && ht.id_origem == (int)Horario.Origem.ALUNO &&
                                                  ht.cd_pessoa_escola == cd_escola
                                             select ht.cd_horario).Any()
                                              ||
                                             //Que exista horários disponiveis.
                                            (from ht in db.Horario
                                            where (ht.cd_registro == aluno.cd_aluno && ht.id_origem == (int)Horario.Origem.ALUNO &&
                                                ht.cd_pessoa_escola == cd_escola && ht.id_dia_semana == h.id_dia_semana &&
                                                h.dt_hora_ini >= ht.dt_hora_ini && h.dt_hora_fim <= ht.dt_hora_fim)
                                           select ht.cd_horario).Any()
                                        )
                                        select aluno;
                            }

                if (alunosT != null)
                {
                    alunos = from a in alunos
                        where
                            (alunosT.Any(al => al.cd_aluno == a.cd_aluno))
                        select a;
                }        
        

                if (sexo > 0)
                    alunos = from a in alunos
                             where a.AlunoPessoaFisica.nm_sexo == sexo
                             select a;

                sql = from aluno in alunos
                      select new AlunoSearchUI
                  {
                      cd_aluno = aluno.cd_aluno,
                      cd_pessoa_aluno = aluno.AlunoPessoaFisica.cd_pessoa,
                      cd_pessoa_escola = aluno.cd_pessoa_escola,
                      cd_pessoa = aluno.AlunoPessoaFisica.cd_pessoa,
                      no_pessoa = aluno.AlunoPessoaFisica.no_pessoa,
                      dc_reduzido_pessoa = aluno.AlunoPessoaFisica.dc_reduzido_pessoa,
                      nm_cpf = aluno.AlunoPessoaFisica.cd_pessoa_cpf > 0 ? aluno.AlunoPessoaFisica.PessoaSGFQueUsoOCpf.nm_cpf : aluno.AlunoPessoaFisica.nm_cpf,
                      no_pessoa_dependente = aluno.AlunoPessoaFisica.PessoaSGFQueUsoOCpf.no_pessoa,
                      email = aluno.AlunoPessoaFisica.TelefonePessoa.Where(t => t.cd_tipo_telefone == 4).FirstOrDefault().dc_fone_mail,
                      dt_cadastramento = aluno.AlunoPessoaFisica.dt_cadastramento,
                      id_aluno_ativo = aluno.id_aluno_ativo,
                      cd_pessoa_dependente = aluno.AlunoPessoaFisica.PessoaSGFQueUsoOCpf == null ? 0 : aluno.AlunoPessoaFisica.PessoaSGFQueUsoOCpf.cd_pessoa,
                      vl_abatimento_matricula = aluno.Contratos.Any(x => x.vl_pre_matricula > 0) ? 0 : aluno.Prospect.vl_matricula_prospect,
                      PessoaSGFQueUsoOCpf = aluno.AlunoPessoaFisica.PessoaSGFQueUsoOCpf == null ? null : aluno.AlunoPessoaFisica.PessoaSGFQueUsoOCpf,
                      dc_reduzido_pessoa_escola = aluno.EscolaAluno.dc_reduzido_pessoa,
                      cd_pessoa_escola_aluno = aluno.EscolaAluno.cd_pessoa,
                      pc_bolsa = aluno.Bolsa.pc_bolsa,
                      dt_inicio_bolsa = aluno.Bolsa.dt_inicio_bolsa,
                      pc_bolsa_material = aluno.Bolsa.pc_bolsa_material
                      //ativExtra = aluno.AtividadeExtraAluno.Where(at => at.ind_participacao == false).Count()
                  };
                sql = sorter.Sort(sql);

                if (!string.IsNullOrEmpty(desc))
                {
                    if (inicio)
                        sql = from func in sql
                              where func.no_pessoa.StartsWith(desc)
                              select func;
                    else
                        sql = from c in sql
                              where c.no_pessoa.Contains(desc)
                              select c;
                }
                if (!string.IsNullOrEmpty(cpf))
                    sql = from c in sql
                          where c.nm_cpf == cpf
                          select c;
                //todo
                if (!string.IsNullOrEmpty(email))
                    if (inicio)
                        sql = from c in sql
                              where (from t in db.TelefoneSGF
                                     where t.cd_pessoa == c.cd_pessoa &&
                                         t.cd_tipo_telefone == 4 && t.id_telefone_principal == true && t.dc_fone_mail.StartsWith(email)
                                     select t.cd_telefone).Any()
                              select c;
                    else
                        sql = from c in sql
                              where (from t in db.TelefoneSGF
                                     where t.cd_pessoa == c.cd_pessoa &&
                                         t.cd_tipo_telefone == 4 && t.id_telefone_principal == true && t.dc_fone_mail.Contains(email)
                                     select t.cd_telefone).Any()
                              select c;

                if (status != null)
                    sql = from c in sql
                          where c.id_aluno_ativo == status
                          select c;

                int limite = sql.Count();

                parametros.ajustaParametrosPesquisa(limite);
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Aluno> getAlunosMatriculadosRematriculados(int cd_turma, int? cd_turma_ppt, int cd_escola, List<Aluno> alunos)
        {
            try
            {
                List<int> cdTurmas = new List<int>();
                List<int> cdAlunos = new List<int>();
                if (cd_turma > 0)
                    cdTurmas.Add(cd_turma);
                if (alunos != null && alunos.Count() > 0)
                {
                    for (int i = 0; i < alunos.Count(); i++)
                        cdAlunos.Add(alunos[i].cd_aluno);
                    if (cd_turma == 0)
                        for (int i = 0; i < alunos.Count(); i++)
                            cdTurmas.Add(alunos[i].cd_turma);
                }

                IEnumerable<Aluno> alunosT;

                if (cd_turma > 0 && cd_turma_ppt == null )//regular
                {
                    alunosT = from aluno in db.Aluno
                              join at in db.AlunoTurma on aluno.cd_aluno equals at.cd_aluno
                              where aluno.cd_pessoa_escola == cd_escola
                                   && aluno.AlunoTurma.Any(a => a.cd_aluno == aluno.cd_aluno)
                                   && cdAlunos.Contains(aluno.cd_aluno) 
                                   && at.Turma.id_turma_ppt == false
                                   && at.Turma.cd_turma == cd_turma
                                   && aluno.id_aluno_ativo == true && aluno.AlunoTurma.Where(al =>
                                       al.cd_aluno == aluno.cd_aluno &&
                                       (al.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma
                                            .SituacaoAlunoTurma.Ativo ||
                                        al.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma
                                            .SituacaoAlunoTurma.Rematriculado && at.Turma.cd_turma == cd_turma)).Any()

                              select aluno;

                }
                else if (cd_turma == 0 && cd_turma_ppt > 0  )//ppt pai
                {

                    alunosT = from aluno in db.Aluno
                        join at in db.AlunoTurma on aluno.cd_aluno equals at.cd_aluno
                        where aluno.cd_pessoa_escola == cd_escola
                              && aluno.AlunoTurma.Any(a => a.cd_aluno == aluno.cd_aluno)
                              && cdAlunos.Contains(aluno.cd_aluno)
                              && at.Turma.id_turma_ppt == false
                              && at.Turma.cd_turma_ppt == cd_turma_ppt
                              && aluno.id_aluno_ativo == true && aluno.AlunoTurma.Where(al =>
                                  al.cd_aluno == aluno.cd_aluno &&
                                  (al.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma
                                       .SituacaoAlunoTurma.Ativo ||
                                   al.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma
                                       .SituacaoAlunoTurma.Rematriculado && al.Turma.cd_turma_ppt == cd_turma_ppt)).Any()

                        select aluno;
                }
                else
                {
                  
                    alunosT = from aluno in db.Aluno //ppt filha
                        join at in db.AlunoTurma on aluno.cd_aluno equals at.cd_aluno
                        where aluno.cd_pessoa_escola == cd_escola
                              && aluno.AlunoTurma.Any(a => a.cd_aluno == aluno.cd_aluno)
                              && cdAlunos.Contains(aluno.cd_aluno)
                              && at.Turma.id_turma_ppt == false
                              && cdTurmas.Contains(at.Turma.cd_turma)
                              && aluno.id_aluno_ativo == true && aluno.AlunoTurma.Where(al =>
                                  al.cd_aluno == aluno.cd_aluno &&
                                  (al.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma
                                       .SituacaoAlunoTurma.Ativo ||
                                   al.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma
                                       .SituacaoAlunoTurma.Rematriculado )).Any()

                        select aluno;
                }


               
                
                return alunosT;

            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Aluno> getAlunosDisponiveisFaixaHorario(int cd_turma, int? cd_turma_ppt, int cd_escola, Horario h, List<Aluno> alunos)
        {
            try
            {
                List<int> cdTurmas = new List<int>();
                List<int> cdAlunos = new List<int>();
                if (cd_turma > 0)
                    cdTurmas.Add(cd_turma);
                if (alunos != null && alunos.Count() > 0)
                {
                    for (int i = 0; i < alunos.Count(); i++)
                        cdAlunos.Add(alunos[i].cd_aluno);
                    if (cd_turma == 0)
                        for (int i = 0; i < alunos.Count(); i++)
                            cdTurmas.Add(alunos[i].cd_turma);
                }

                var alunosT = from aluno in db.Aluno
                              where aluno.cd_pessoa_escola == cd_escola
                                   && aluno.AlunoTurma.Any(a => a.cd_aluno == aluno.cd_aluno)
                                   && cdAlunos.Contains(aluno.cd_aluno)
                                   && aluno.id_aluno_ativo == true

                              select aluno;
                /*
                alunosT = from aluno in alunosT
                          where
                              // Não exista horário alocado em outras turmas.
                          !(from ht in db.Horario
                            join turmas in db.Turma on ht.cd_registro equals turmas.cd_turma
                            where
                                // Desconsidera a própria tuma (sendo filha ou normal):
                            !cdTurmas.Contains(turmas.cd_turma)
                            && turmas.dt_termino_turma == null

                            // Desconsidera qualquer turma pai:
                            && !turmas.id_turma_ppt
                            && turmas.TurmaAluno.Where(t => t.cd_aluno == aluno.cd_aluno &&
                                                             (t.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                              t.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Rematriculado)).Any()
                            && ht.id_origem == (int)Horario.Origem.TURMA
                            && ht.id_dia_semana == h.id_dia_semana
                            && ((ht.dt_hora_ini <= h.dt_hora_ini && h.dt_hora_ini < ht.dt_hora_fim)
                                       || (ht.dt_hora_ini < h.dt_hora_fim && h.dt_hora_fim <= ht.dt_hora_fim)
                                       || (ht.dt_hora_ini <= h.dt_hora_ini && h.dt_hora_fim <= ht.dt_hora_fim)
                                       || (ht.dt_hora_ini >= h.dt_hora_ini && h.dt_hora_fim >= ht.dt_hora_fim)
                                       )
                            select ht.cd_horario).Any()
                          // &&
                          //    //Que exista horários disponiveis.
                          //((from ht in db.Horario
                          //  where (ht.cd_registro == aluno.cd_aluno && ht.id_origem == (int)Horario.Origem.ALUNO &&
                          //       ht.cd_pessoa_escola == cd_escola && ht.id_dia_semana == h.id_dia_semana &&
                          //       h.dt_hora_ini >= ht.dt_hora_ini && h.dt_hora_fim <= ht.dt_hora_fim)
                          //  select ht.cd_horario).Any()
                          //||
                          //    // Não exista horários disponiveis para esse aluno.
                          //!(from ht in db.Horario
                          //  where ht.cd_registro == aluno.cd_aluno && ht.id_origem == (int)Horario.Origem.ALUNO &&
                          //       ht.cd_pessoa_escola == cd_escola
                          //  select ht.cd_horario).Any()
                          //)*/
                        alunosT = from aluno in alunosT
                            where
                        (!(from ht in db.Horario
                                    join turma in db.Turma on ht.cd_registro equals turma.cd_turma
                                    where
                                    turma.cd_turma != cd_turma
                                    && turma.cd_pessoa_escola == cd_escola
                                    && turma.id_turma_ppt == false
                                    && turma.dt_termino_turma == null
                                    && turma.TurmaAluno.Where(al => al.cd_aluno == aluno.cd_aluno && (al.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                                     al.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Rematriculado)).Any()
                                    && ht.id_origem == (int)Horario.Origem.TURMA
                                        //Produtos iguais, tem que verificar a interseção
                                    && ((!turma.TurmaAluno.Where(al => al.Turma.Produto.cd_produto == turma.Produto.cd_produto).Any() && ht.id_dia_semana == h.id_dia_semana && (
                                           ((ht.dt_hora_ini <= h.dt_hora_ini && h.dt_hora_ini < ht.dt_hora_fim)
                                        || (ht.dt_hora_ini < h.dt_hora_fim && h.dt_hora_fim <= ht.dt_hora_fim)
                                        || (ht.dt_hora_ini <= h.dt_hora_ini && h.dt_hora_fim <= ht.dt_hora_fim)
                                        || (ht.dt_hora_ini >= h.dt_hora_ini && h.dt_hora_fim >= ht.dt_hora_fim)))
                                       ) ||
                                       
                                       (turma.TurmaAluno.Where(al => al.Turma.Produto.cd_produto == turma.Produto.cd_produto).Any() && (turma.dt_final_aula != null && turma.dt_final_aula.HasValue ) && (
                                         (turma.TurmaAluno.Where(ta => ta.Turma.cd_turma_ppt > 0).Any()  && 
                                            ht.id_dia_semana == h.id_dia_semana && (
                                                 //Verificação para turmas que tem horários diferentes.
                                                 ((ht.dt_hora_ini != h.dt_hora_ini || h.dt_hora_fim != ht.dt_hora_fim) &&
                                                    // Verificação básica de interseção/ excluindo quem está fora dos horários.
                                                  (((ht.dt_hora_ini <= h.dt_hora_ini && h.dt_hora_ini < ht.dt_hora_fim) ||
                                                   (ht.dt_hora_ini < h.dt_hora_fim && h.dt_hora_fim <= ht.dt_hora_fim) ||
                                                   (ht.dt_hora_ini <= h.dt_hora_ini && h.dt_hora_fim <= ht.dt_hora_fim) ||
                                                   (ht.dt_hora_ini >= h.dt_hora_ini && h.dt_hora_fim >= ht.dt_hora_fim))
                                                      // Verificação de rematricula antecipada.
                                                      || (turma.dt_final_aula != null && (
                                                                                          // Verificar somente se houver interseção
                                                                                          ((ht.dt_hora_ini <= h.dt_hora_ini && h.dt_hora_ini < ht.dt_hora_fim) ||
                                                                                           (ht.dt_hora_ini < h.dt_hora_fim && h.dt_hora_fim <= ht.dt_hora_fim) ||
                                                                                           (ht.dt_hora_ini <= h.dt_hora_ini && h.dt_hora_fim <= ht.dt_hora_fim) ||
                                                                                           (ht.dt_hora_ini >= h.dt_hora_ini && h.dt_hora_fim >= ht.dt_hora_fim)) 
                                                                                          )
                                                         )
                                                  )
                                                 )
                                                 ||
                                                 //Verificação para turmas que tiverem horário igual.
                                                 //Verificação básica de interseção/ excluindo quem está fora dos horários.
                                                 (ht.dt_hora_ini == h.dt_hora_ini && h.dt_hora_fim == ht.dt_hora_fim &&
                                                  (turma.dt_final_aula != null && turma.dt_final_aula.HasValue ))
                                          )))
                                     ))
                                    select ht.cd_horario).Any()
                                   ||
                                   !(from ht in db.Horario
                                     join turma in db.Turma on ht.cd_registro equals turma.cd_turma
                                     where
                                     turma.cd_turma != cd_turma
                                     && turma.cd_pessoa_escola == cd_escola
                                     && turma.id_turma_ppt == false
                                     && turma.dt_termino_turma == null
                                     && turma.TurmaAluno.Where(al => al.cd_aluno == aluno.cd_aluno && (al.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                                      al.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Rematriculado)).Any()
                                     && ht.id_origem == (int)Horario.Origem.TURMA
                                     select ht.cd_horario).Any()
                                   )

                                   &&

                                   // Não exista horários disponiveis para esse aluno.
                                  (!(from ht in db.Horario
                                     where ht.cd_registro == aluno.cd_aluno && ht.id_origem == (int)Horario.Origem.ALUNO &&
                                          ht.cd_pessoa_escola == cd_escola
                                     select ht.cd_horario).Any()
                                      ||
                                     //Que exista horários disponiveis.
                                  (from ht in db.Horario
                                   where (ht.cd_registro == aluno.cd_aluno && ht.id_origem == (int)Horario.Origem.ALUNO &&
                                        ht.cd_pessoa_escola == cd_escola && ht.id_dia_semana == h.id_dia_semana &&
                                        h.dt_hora_ini >= ht.dt_hora_ini && h.dt_hora_fim <= ht.dt_hora_fim)
                                   select ht.cd_horario).Any()
                                   )
                          select aluno;
                return alunosT;

            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<int> getAlunosDisponiveisHistCancelEnc(int cd_turma, int cd_escola)
        {
            try
            {
                List<int> alunosT = (from aluno in db.Aluno
                                     where aluno.cd_pessoa_escola == cd_escola
                                          && ((aluno.AlunoTurma.Any(a => a.cd_turma == cd_turma && a.cd_contrato > 0)
                                          && aluno.HistoricoAluno.Any(ha => ha.cd_turma == cd_turma && DbFunctions.TruncateTime(ha.dt_historico) <= DbFunctions.TruncateTime(ha.Turma.dt_termino_turma) &&
                                                                       (ha.id_situacao_historico == (byte)FundacaoFisk.SGF.GenericModel.HistoricoAluno.SituacaoHistorico.ATIVO ||
                                                                        ha.id_situacao_historico == (byte)FundacaoFisk.SGF.GenericModel.HistoricoAluno.SituacaoHistorico.REMATRICULADO) &&
                                                                       !db.HistoricoAluno.Any(ht => ht.cd_turma == cd_turma && aluno.cd_aluno == ht.cd_aluno &&
                                                                                                 (ht.id_situacao_historico != (byte)FundacaoFisk.SGF.GenericModel.HistoricoAluno.SituacaoHistorico.ATIVO &&
                                                                                                  ht.id_situacao_historico != (byte)FundacaoFisk.SGF.GenericModel.HistoricoAluno.SituacaoHistorico.REMATRICULADO &&
                                                                                                  ht.id_situacao_historico != (byte)FundacaoFisk.SGF.GenericModel.HistoricoAluno.SituacaoHistorico.ENCERRADO
                                                                                                  && ht.cd_historico_aluno > ha.cd_historico_aluno && ht.nm_sequencia > ha.nm_sequencia)
                                                                                                 )
                                              ))
                                         || aluno.AlunoTurma.Where(a => a.cd_turma == cd_turma && a.cd_contrato == null).Any())

                                     select aluno.cd_aluno).ToList();
                return alunosT;

            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Aluno> getAlunosDisponiveisFaixaHorarioCancelEnc(int cd_turma, int cd_escola, Horario h, List<int> cdAlunos)
        {
            try
            {
                List<int> cdTurmas = new List<int>();

                if (cd_turma > 0)
                    cdTurmas.Add(cd_turma);

                var alunosT = from aluno in db.Aluno
                              where aluno.cd_pessoa_escola == cd_escola
                                   && aluno.AlunoTurma.Any(a => a.cd_aluno == aluno.cd_aluno && a.cd_turma == cd_turma)
                                   && cdAlunos.Contains(aluno.cd_aluno)
                              select aluno;

                alunosT = from aluno in alunosT
                          where
                              // Não exista horário alocado em outras turmas.
                          !(from ht in db.Horario
                            join turmas in db.Turma on ht.cd_registro equals turmas.cd_turma
                            where
                                // Desconsidera a própria tuma (sendo filha ou normal):
                            !cdTurmas.Contains(turmas.cd_turma)
                            && turmas.dt_termino_turma == null

                            // Desconsidera qualquer turma pai:
                            && !turmas.id_turma_ppt
                            && turmas.TurmaAluno.Where(t => t.cd_aluno == aluno.cd_aluno &&
                                                             (t.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                              t.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Rematriculado)).Any()
                            && ht.id_origem == (int)Horario.Origem.TURMA
                            && ht.id_dia_semana == h.id_dia_semana
                            && ((ht.dt_hora_ini <= h.dt_hora_ini && h.dt_hora_ini < ht.dt_hora_fim)
                                       || (ht.dt_hora_ini < h.dt_hora_fim && h.dt_hora_fim <= ht.dt_hora_fim)
                                       || (ht.dt_hora_ini <= h.dt_hora_ini && h.dt_hora_fim <= ht.dt_hora_fim)
                                       || (ht.dt_hora_ini >= h.dt_hora_ini && h.dt_hora_fim >= ht.dt_hora_fim)
                                       )
                            select ht.cd_horario).Any()
                            &&
                              //Que exista horários disponiveis.
                           ((from ht in db.Horario
                             where (ht.cd_registro == aluno.cd_aluno && ht.id_origem == (int)Horario.Origem.ALUNO &&
                                  ht.cd_pessoa_escola == cd_escola && ht.id_dia_semana == h.id_dia_semana &&
                                  h.dt_hora_ini >= ht.dt_hora_ini && h.dt_hora_fim <= ht.dt_hora_fim)
                             select ht.cd_horario).Any()
                            ||
                              // Não exista horários disponiveis para esse aluno.
                            !(from ht in db.Horario
                              where ht.cd_registro == aluno.cd_aluno && ht.id_origem == (int)Horario.Origem.ALUNO &&
                                   ht.cd_pessoa_escola == cd_escola
                              select ht.cd_horario).Any()
                           )
                          select aluno;
                return alunosT;

            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        //Busca os alunos que já foram matriculados na turma, que não possui avaliação do aluno para alguma avaliação da turma:
        public IEnumerable<Aluno> returnAlunosSemAvalicacaoByTurma(int cd_escola, int cd_turma)
        {
            try
            {
                var sql = (from alunos in db.Aluno
                           join alunoTuma in db.AlunoTurma on alunos.cd_aluno equals alunoTuma.cd_aluno
                           join avaliacaoTurma in db.AvaliacaoTurma on alunoTuma.cd_turma equals avaliacaoTurma.cd_turma
                           where alunos.cd_pessoa_escola == cd_escola
                               && alunoTuma.cd_contrato.HasValue
                               && avaliacaoTurma.cd_turma == cd_turma
                               && (
                                     !(from avalAluno in db.AvaliacaoAluno
                                       where avalAluno.cd_avaliacao_turma == avaliacaoTurma.cd_avaliacao_turma
                                       select avalAluno).Any()
                                     ||
                                     !(from avalAluno in db.AvaliacaoAluno
                                       join avaliacaoTurma2 in db.AvaliacaoTurma on avalAluno.cd_avaliacao_turma equals avaliacaoTurma2.cd_avaliacao_turma
                                       where avaliacaoTurma2.cd_turma == cd_turma
                                            && avalAluno.cd_aluno == alunos.cd_aluno
                                       select avalAluno).Any()
                                  )
                           select alunos).Distinct();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<AlunoSearchUI> getAlunoPolitica(int cdEscola)
        {
            try
            {
                var retorno = from aluno in db.Aluno
                              join polAluno in db.PoliticaAluno
                              on aluno.cd_aluno equals polAluno.cd_aluno
                              join pessoa in db.PessoaSGF
                              on aluno.cd_pessoa_aluno equals pessoa.cd_pessoa
                              where aluno.cd_pessoa_escola == cdEscola
                              orderby pessoa.no_pessoa
                              select new AlunoSearchUI
                              {
                                  cd_aluno = aluno.cd_aluno,
                                  no_pessoa = pessoa.no_pessoa
                              };

                return retorno;

            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<AlunoSearchUI> getAlunoSelecionado(int cdPolitica, int cdEscola)
        {
            try
            {
                var retorno = from aluno in db.Aluno
                              join politica in db.PoliticaAluno
                              on aluno.cd_aluno equals politica.cd_aluno
                              where
                                politica.cd_politica_desconto == cdPolitica &&
                                aluno.cd_pessoa_escola == cdEscola
                              orderby aluno.cd_aluno
                              select new AlunoSearchUI
                              {
                                  cd_aluno = aluno.cd_aluno,
                                  cd_pessoa_aluno = aluno.cd_aluno,
                                  cd_pessoa_escola = aluno.cd_pessoa_escola,
                                  no_pessoa = aluno.AlunoPessoaFisica.no_pessoa,
                                  id_aluno_ativo = aluno.id_aluno_ativo
                              };
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Aluno> getAllAlunosTurmasFilhasPPT(int cd_turma_ppt, int cd_escola)
        {
            try
            {
                var sql = (from aluno in db.Aluno
                           join at in db.AlunoTurma on aluno.cd_aluno equals at.cd_aluno
                           where aluno.cd_pessoa_escola == cd_escola &&
                                 at.Turma.cd_turma_ppt == cd_turma_ppt &&
                                 (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                  at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                  at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Aguardando)
                           select new
                           {
                               cd_aluno = at.cd_aluno,
                               cd_turma = at.cd_turma
                           }).ToList().Select(x => new Aluno
                           {
                               cd_aluno = x.cd_aluno,
                               cd_turma = x.cd_turma
                           });
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Aluno findAlunoById(int cd_aluno, int cd_escola)
        {
            try
            {
                var sql = (from a in db.Aluno
                           where a.cd_pessoa_escola == cd_escola && a.cd_aluno == cd_aluno
                           select a).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public Aluno getAlunoById(int cd_aluno)
        {
            try
            {
                var sql = (from a in db.Aluno.Include(x => x.AlunoPessoaFisica.PessoaSGFQueUsoOCpf)
                           where a.cd_aluno == cd_aluno
                           select a).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public AlunoApiCyberBdUI FindAlunoByCdAluno(int cd_aluno, int cd_escola)
        {
            try
            {
                var sql = (from a in db.Aluno.Include(x => x.AlunoPessoaFisica)
                    where a.cd_pessoa_escola == cd_escola && a.cd_aluno == cd_aluno
                    select new
                    {
                        nome = a.AlunoPessoaFisica.no_pessoa,
                        codigo = a.cd_aluno,
                        email = ((from t in db.TelefoneSGF where t.cd_pessoa == a.cd_pessoa_aluno && t.cd_tipo_telefone == 4 && t.id_telefone_principal == true select t).FirstOrDefault()!= null ?
                            (from t in db.TelefoneSGF where t.cd_pessoa == a.cd_pessoa_aluno && t.cd_tipo_telefone == 4 && t.id_telefone_principal == true select t).FirstOrDefault().dc_fone_mail : ""),
                        aluno_ativo = a.id_aluno_ativo

                    }).ToList().Select(x => new AlunoApiCyberBdUI
                    {
                        nome = x.nome,
                        codigo = x.codigo,
                        email = x.email,
                        aluno_ativo = x.aluno_ativo
                    }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Aluno> getAlunosTurmaAtivosDiarioAula(int cd_turma, int cd_pessoa_escola, DateTime? dt_inicial, DateTime dt_final, AlunoTurma.FiltroSituacaoAlunoTurma situacao_aluno)
        {
            try
            {
                IEnumerable<Aluno> sql;
                DateTime data_inicial = DateTime.MinValue;
                if (dt_inicial == null)
                {
                    dt_inicial = DateTime.MinValue;
                }
                List<int> cds_turmas_escolas = new List<int>();
                List<TurmaEscola> turmasEscola = new List<TurmaEscola>();

                int cdPessoaEscola = (from t in db.Turma
                                      where (t.cd_turma == cd_turma || t.cd_turma_ppt == cd_turma)
                                        select t.cd_pessoa_escola).FirstOrDefault();


                turmasEscola = (from t in db.Turma
                    join te in db.TurmaEscola on t.cd_turma equals te.cd_turma
                    where t.cd_turma == cd_turma 
                    select te).ToList();

                cds_turmas_escolas = turmasEscola.Select(x => x.cd_escola).ToList();

                sql = (from a in db.Aluno
                       orderby a.AlunoPessoaFisica.no_pessoa
                       where ((a.cd_pessoa_escola == cd_pessoa_escola && (cdPessoaEscola == 0 || cdPessoaEscola != cd_pessoa_escola)) ||
                                (a.cd_pessoa_escola == cd_pessoa_escola || cds_turmas_escolas.Contains(a.cd_pessoa_escola) && cdPessoaEscola == cd_pessoa_escola)) &&
                             a.AlunoTurma.Any(x => (x.cd_turma == cd_turma || x.Turma.cd_turma_ppt == cd_turma) &&
                                                    (from pt in db.ProgramacaoTurma
                                                     where pt.cd_turma == x.Turma.cd_turma && pt.id_prog_cancelada == false &&
                                                           (pt.cd_feriado == null || pt.cd_feriado_desconsiderado != null) && (dt_inicial.HasValue && pt.dta_programacao_turma >= ((DateTime)dt_inicial).Date) && pt.dta_programacao_turma <= dt_final.Date 
                                                     select pt).Count() > 0 
                                                   && (situacao_aluno == AlunoTurma.FiltroSituacaoAlunoTurma.Nao_Encerrado ? x.Turma.dt_termino_turma == null : true)) &&
                             a.HistoricoAluno.Where(ha =>
                                                    (ha.cd_turma == cd_turma || ha.Turma.cd_turma_ppt == cd_turma) &&
                                                    (DbFunctions.TruncateTime(ha.dt_historico) <= dt_final.Date &&
                                                           (ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                            ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                            ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Encerrado)
                                                            && ha.nm_sequencia == a.HistoricoAluno.Where(han => (han.cd_turma == ha.cd_turma)
                                                                                           && DbFunctions.TruncateTime(han.dt_historico) <= dt_final.Date).Max(x => x.nm_sequencia)
                                                        )
                                                    ).Any()
                       select new
                       {
                           cd_aluno = a.cd_aluno,
                           cd_pessoa = a.cd_pessoa_aluno,
                           no_aluno = a.AlunoPessoaFisica.no_pessoa,
                           qtd_faltas = a.AlunoTurma.Where(x => x.cd_turma == cd_turma || x.Turma.cd_turma_ppt == cd_turma).FirstOrDefault().nm_faltas,
                           cd_pessoa_escola = a.cd_pessoa_escola
                       }).ToList().Select(x => new Aluno
                       {
                           cd_aluno = x.cd_aluno,
                           cd_pessoa_aluno = x.cd_pessoa,
                           cd_pessoa_escola = x.cd_pessoa_escola,
                           nomeAluno = x.no_aluno,
                           nm_faltas = x.qtd_faltas
                       });

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Aluno> getAlunosTurmaAtivosDiarioAulaDiario(int cd_turma, int cd_pessoa_escola, DateTime? dt_inicial, DateTime dt_final, AlunoTurma.FiltroSituacaoAlunoTurma situacao_aluno)
        {
            try
            {
                IEnumerable<Aluno> sql;
                DateTime data_inicial = DateTime.MinValue;
                if (dt_inicial == null)
                {
                    dt_inicial = DateTime.MinValue;
                }
                List<int> cds_turmas_escolas = new List<int>();
                List<TurmaEscola> turmasEscola = new List<TurmaEscola>();

                int cdPessoaEscola = (from t in db.Turma
                                      where (t.cd_turma == cd_turma || t.cd_turma_ppt == cd_turma)
                                      select t.cd_pessoa_escola).FirstOrDefault();


                turmasEscola = (from t in db.Turma
                                join te in db.TurmaEscola on t.cd_turma equals te.cd_turma
                                where t.cd_turma == cd_turma
                                select te).ToList();

                cds_turmas_escolas = turmasEscola.Select(x => x.cd_escola).ToList();

                sql = (from a in db.Aluno
                       orderby a.AlunoPessoaFisica.no_pessoa
                       where ((a.cd_pessoa_escola == cd_pessoa_escola && (cdPessoaEscola == 0 || cdPessoaEscola != cd_pessoa_escola)) ||
                                (a.cd_pessoa_escola == cd_pessoa_escola || cds_turmas_escolas.Contains(a.cd_pessoa_escola) && cdPessoaEscola == cd_pessoa_escola)) &&
                             a.AlunoTurma.Any(x => (x.cd_turma == cd_turma || x.Turma.cd_turma_ppt == cd_turma)
                                                   && (situacao_aluno == AlunoTurma.FiltroSituacaoAlunoTurma.Nao_Encerrado ? x.Turma.dt_termino_turma == null : true)) &&
                             a.HistoricoAluno.Where(ha =>
                                                    (ha.cd_turma == cd_turma || ha.Turma.cd_turma_ppt == cd_turma) &&
                                                    (DbFunctions.TruncateTime(ha.dt_historico) <= dt_final.Date &&
                                                           (ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                            ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                            ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Encerrado ||
                                                            ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial)
                                                            && ha.nm_sequencia == a.HistoricoAluno.Where(han => (han.cd_turma == ha.cd_turma)
                                                                                           && DbFunctions.TruncateTime(han.dt_historico) <= dt_final.Date).Max(x => x.nm_sequencia)
                                                        )
                                                    ).Any()
                       select new
                       {
                           cd_aluno = a.cd_aluno,
                           cd_pessoa = a.cd_pessoa_aluno,
                           no_aluno = a.AlunoPessoaFisica.no_pessoa,
                           qtd_faltas = a.AlunoTurma.Where(x => x.cd_turma == cd_turma || x.Turma.cd_turma_ppt == cd_turma).FirstOrDefault().nm_faltas,
                           cd_pessoa_escola = a.cd_pessoa_escola
                       }).ToList().Select(x => new Aluno
                       {
                           cd_aluno = x.cd_aluno,
                           cd_pessoa_aluno = x.cd_pessoa,
                           cd_pessoa_escola = x.cd_pessoa_escola,
                           nomeAluno = x.no_aluno,
                           nm_faltas = x.qtd_faltas
                       });

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Aluno> getAlunosTurmaAtivosDiarioAulaCarga(int cd_turma, int cd_pessoa_escola, DateTime? dt_inicial, DateTime dt_final)
        {
            try
            {
                IEnumerable<Aluno> sql;
                DateTime data_inicial = DateTime.MinValue;
                List<int> cds_turmas_escolas = new List<int>();
                List<TurmaEscola> turmasEscola = new List<TurmaEscola>();

                int cdPessoaEscola = (from t in db.Turma
                                      where (t.cd_turma == cd_turma || t.cd_turma_ppt == cd_turma)
                                      select t.cd_pessoa_escola).FirstOrDefault();


                turmasEscola = (from t in db.Turma
                                join te in db.TurmaEscola on t.cd_turma equals te.cd_turma
                                where t.cd_turma == cd_turma
                                select te).ToList();

                cds_turmas_escolas = turmasEscola.Select(x => x.cd_escola).ToList();
                int? cdCurso = (from t in db.Turma where t.cd_turma == cd_turma select t.cd_curso).FirstOrDefault();


                sql = (from a in db.Aluno
                       join v in db.vi_carga_aluno on a.cd_aluno equals v.cd_aluno
                       orderby a.AlunoPessoaFisica.no_pessoa
                       where ((a.cd_pessoa_escola == cd_pessoa_escola && (cdPessoaEscola == 0 || cdPessoaEscola != cd_pessoa_escola)) ||
                                (a.cd_pessoa_escola == cd_pessoa_escola || cds_turmas_escolas.Contains(a.cd_pessoa_escola) && cdPessoaEscola == cd_pessoa_escola)) &&
                             v.cd_curso == cdCurso &&
                             a.AlunoTurma.Any(x => (x.cd_turma == cd_turma || x.Turma.cd_turma_ppt == cd_turma) ) &&
                             a.HistoricoAluno.Where(ha =>
                                                    (ha.cd_turma == cd_turma || ha.Turma.cd_turma_ppt == cd_turma) &&
                                                    (DbFunctions.TruncateTime(ha.dt_historico) <= dt_final.Date &&
                                                           (ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                            ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                            ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Encerrado)
                                                            && ha.nm_sequencia == a.HistoricoAluno.Where(han => (han.cd_turma == ha.cd_turma)
                                                                                           && DbFunctions.TruncateTime(han.dt_historico) <= dt_final.Date).Max(x => x.nm_sequencia)
                                                        )
                                                    ).Any()
                       select new
                       {
                           cd_aluno = a.cd_aluno,
                           cd_pessoa = a.cd_pessoa_aluno,
                           no_aluno = a.AlunoPessoaFisica.no_pessoa,
                           qtd_faltas = a.AlunoTurma.Where(x => x.cd_turma == cd_turma || x.Turma.cd_turma_ppt == cd_turma).FirstOrDefault().nm_faltas,
                           nm_carga = v.nm_carga,
                           nm_maxima = v.nm_maxima,
                           cd_pessoa_escola = a.cd_pessoa_escola
                       }).ToList().Select(x => new Aluno
                       {
                           cd_aluno = x.cd_aluno,
                           cd_pessoa_aluno = x.cd_pessoa,
                           cd_pessoa_escola = x.cd_pessoa_escola,
                           nomeAluno = x.no_aluno,
                           nm_faltas = x.qtd_faltas,
                           nm_carga = x.nm_carga,
                           nm_maxima = x.nm_maxima
                       }).Union(
                        (from a in db.Aluno
                         orderby a.AlunoPessoaFisica.no_pessoa
                         where ((a.cd_pessoa_escola == cd_pessoa_escola && (cdPessoaEscola == 0 || cdPessoaEscola != cd_pessoa_escola)) ||
                                  (a.cd_pessoa_escola == cd_pessoa_escola || cds_turmas_escolas.Contains(a.cd_pessoa_escola) && cdPessoaEscola == cd_pessoa_escola)) &&
                               a.AlunoTurma.Any(x => (x.cd_turma == cd_turma || x.Turma.cd_turma_ppt == cd_turma)) &&
                               !db.vi_carga_aluno.Any(x => x.cd_aluno == a.cd_aluno && x.cd_curso == cdCurso) &&
                               a.HistoricoAluno.Where(ha =>
                                                      (ha.cd_turma == cd_turma || ha.Turma.cd_turma_ppt == cd_turma) &&
                                                      (DbFunctions.TruncateTime(ha.dt_historico) <= dt_final.Date &&
                                                             (ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                              ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                              ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Encerrado)
                                                              && ha.nm_sequencia == a.HistoricoAluno.Where(han => (han.cd_turma == ha.cd_turma)
                                                                                             && DbFunctions.TruncateTime(han.dt_historico) <= dt_final.Date).Max(x => x.nm_sequencia)
                                                          )
                                                      ).Any()
                         select new
                         {
                             cd_aluno = a.cd_aluno,
                             cd_pessoa = a.cd_pessoa_aluno,
                             no_aluno = a.AlunoPessoaFisica.no_pessoa,
                             qtd_faltas = a.AlunoTurma.Where(x => x.cd_turma == cd_turma || x.Turma.cd_turma_ppt == cd_turma).FirstOrDefault().nm_faltas,
                             nm_carga = 0,
                             nm_maxima = 0,
                             cd_pessoa_escola = a.cd_pessoa_escola
                         }).ToList().Select(x => new Aluno
                         {
                             cd_aluno = x.cd_aluno,
                             cd_pessoa_aluno = x.cd_pessoa,
                             cd_pessoa_escola = x.cd_pessoa_escola,
                             nomeAluno = x.no_aluno,
                             nm_faltas = x.qtd_faltas,
                             nm_carga = x.nm_carga,
                             nm_maxima = x.nm_maxima
                         }
                       ));

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Aluno> getAlunosTurmaAtivosDiarioAulaReport(int cd_turma, int cd_pessoa_escola, DateTime? dt_inicial, DateTime dt_final)
        {
            try
            {
                List<TurmaEscola> turmasEscola = new List<TurmaEscola>();
                List<int> cds_turmas_escolas = new List<int>();

                int cdPessoaEscola = (from t in db.Turma
                                      where (t.cd_turma == cd_turma || t.cd_turma_ppt == cd_turma)
                                      select t.cd_pessoa_escola).FirstOrDefault();


                turmasEscola = (from t in db.Turma
                                join te in db.TurmaEscola on t.cd_turma equals te.cd_turma
                                where t.cd_turma == cd_turma
                                select te).ToList();

                cds_turmas_escolas = turmasEscola.Select(x => x.cd_escola).ToList();

                IEnumerable<Aluno> sql;
                DateTime data_inicial = dt_inicial.Value.Date;

                sql = (from a in db.Aluno
                    join aa in db.AvaliacaoAluno on a.cd_aluno equals aa.cd_aluno into x
                    from avaA in x.DefaultIfEmpty()
                    join at in db.AvaliacaoTurma on avaA.cd_avaliacao_turma equals at.cd_avaliacao_turma into y
                    from avaT in
                        (from avaTD in y
                            where (avaTD.cd_turma == cd_turma || avaTD.Turma.cd_turma_ppt == cd_turma)
                            select avaTD).DefaultIfEmpty()
                    //where avaT.cd_turma == cd_turma
                    join av in db.Avaliacao on avaT.cd_avaliacao equals av.cd_avaliacao into z
                    from ava in z.DefaultIfEmpty()
                    join ta in db.TipoAvaliacao on ava.cd_tipo_avaliacao equals ta.cd_tipo_avaliacao into w
                    from tiA in w.DefaultIfEmpty()
                    //orderby a.AlunoPessoaFisica.no_pessoa
                    where ((a.cd_pessoa_escola == cd_pessoa_escola && (cdPessoaEscola == 0 || cdPessoaEscola != cd_pessoa_escola)) ||
                           (a.cd_pessoa_escola == cd_pessoa_escola || cds_turmas_escolas.Contains(a.cd_pessoa_escola) && cdPessoaEscola == cd_pessoa_escola)) &&
                          a.AlunoTurma.Where(alt =>
                              (alt.cd_turma == cd_turma || alt.Turma.cd_turma_ppt == cd_turma) /*&&
                              alt.Turma.dt_termino_turma == null*/).Any()
                              //&& (avaT == null || avaT.cd_turma == cd_turma)
                              //&& ((a.cd_pessoa_escola == cd_pessoa_escola && (cdPessoaEscola == 0 || cdPessoaEscola != cd_pessoa_escola)) ||
                              //    (a.cd_pessoa_escola == cd_pessoa_escola || cds_turmas_escolas.Contains(a.cd_pessoa_escola) && cdPessoaEscola == cd_pessoa_escola))

                          && a.HistoricoAluno.Where(ha =>
                              (ha.cd_turma == cd_turma || ha.Turma.cd_turma_ppt == cd_turma)
                              &&
                              (
                                  //(DbFunctions.TruncateTime(ha.dt_historico) <= dt_final.Date
                                  ////&& (!dt_inicial.HasValue || DbFunctions.TruncateTime(ha.dt_historico) >= data_inicial)
                                  //&& (ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Ativo
                                  // || ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado
                                  // || ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Encerrado)

                                  // // Pega o aluno ativo rematriculado ou encerrado que não teve outra situação que não seja esta logo após ter sido matriculado, ativo ou encerrado.
                                  //&& !a.HistoricoAluno.Where(han =>
                                  //            (han.cd_turma == cd_turma || han.Turma.cd_turma_ppt == cd_turma) &&
                                  //          !(han.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Ativo 
                                  //          || han.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado)
                                  //          || han.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Encerrado
                                  //           && DbFunctions.TruncateTime(han.dt_historico) <= dt_final.Date
                                  //                //&& (!dt_inicial.HasValue || DbFunctions.TruncateTime(ha.dt_historico) >= data_inicial)
                                  //           && DbFunctions.TruncateTime(han.dt_historico) >= DbFunctions.TruncateTime(ha.dt_historico) && han.nm_sequencia > ha.nm_sequencia
                                  //       ).Any()
                                  //)
                                  ///---------------------------------------------------------------------------------
                                  (
                                  (DbFunctions.TruncateTime(ha.dt_historico) <= dt_final.Date
                                  && (ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Ativo
                                  || ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado
                                  || ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Movido)

                                  // Pega o aluno ativo rematriculado ou encerrado que não teve outra situação que não seja esta logo após ter sido matriculado, ativo ou encerrado.
                                  && !a.HistoricoAluno.Where(han =>
                                              ((han.cd_turma == cd_turma || han.Turma.cd_turma_ppt == cd_turma) && han.cd_turma == ha.cd_turma) &&
                                              !(han.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Ativo
                                              || han.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado
                                              || ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Encerrado)
                                              && DbFunctions.TruncateTime(han.dt_historico) <= dt_final.Date
                                              && DbFunctions.TruncateTime(han.dt_historico) >= DbFunctions.TruncateTime(ha.dt_historico) && han.nm_sequencia > ha.nm_sequencia
                                              ).Any()
                                          )
                                  )


                                  ||
                                      ((!dt_inicial.HasValue || DbFunctions.TruncateTime(ha.dt_historico) >= data_inicial)
                                      && ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Movido)
                                  )


                          ).Any()
                        group avaA by new
                       {
                           cd_aluno = a.cd_aluno,
                           no_aluno = a.AlunoPessoaFisica.no_pessoa,
                           dt_nascimento = a.AlunoPessoaFisica.dt_nascimento,
                           pc_bolsa = a.Bolsa.pc_bolsa,
                           //nm_faltas = (byte?)a.AlunoTurma.Where(at => at.cd_turma == cd_turma || at.Turma.cd_turma_ppt == cd_turma ).FirstOrDefault().nm_faltas,
                           nm_faltas = (byte?)a.AlunoTurma.FirstOrDefault().nm_faltas,
                           cd_tipo_avaliacao = (int?)tiA.cd_tipo_avaliacao,
                           dc_tipo_avaliacao = tiA.dc_tipo_avaliacao
                       } into g
                       select new
                       {
                           cd_aluno = g.Key.cd_aluno,
                           no_aluno = g.Key.no_aluno,
                           dt_nascimento = g.Key.dt_nascimento,
                           pc_bolsa = g.Key.pc_bolsa,
                           nm_faltas = g.Key.nm_faltas,
                           cd_tipo_avaliacao = g.Key.cd_tipo_avaliacao,
                           dc_tipo_avaliacao = g.Key.dc_tipo_avaliacao,
                           //nm_nota_aluno = g.Sum(s => s.nm_nota_aluno * (double)s.AvaliacaoTurma.Avaliacao.nm_peso_avaliacao) / g.Count(s => s.nm_nota_aluno.HasValue && !s.AvaliacaoTurma.Avaliacao.CriterioAvaliacao.id_conceito) //Soma da nota do aluno agrupado por aluno e tipo de avaliação.
                           nm_nota_aluno = 0.0
                       }).ToList().Select(x => new Aluno
                       {
                           cd_aluno = x.cd_aluno,
                           nomeAluno = x.no_aluno,
                           AlunoPessoaFisica = new PessoaFisicaSGF()
                           {
                               dt_nascimento = x.dt_nascimento
                           },
                           Bolsa = new AlunoBolsa()
                           {
                               pc_bolsa = x.pc_bolsa
                           },
                           nm_faltas = x.nm_faltas,
                           nm_nota_aluno = null,
                           cd_tipo_avaliacao = x.cd_tipo_avaliacao,
                           dc_tipo_avaliacao = x.dc_tipo_avaliacao,
                           aluno_dt_nascimento = x.dt_nascimento
                       });

                List<Aluno> alunos = new List<Aluno>();
                foreach (var aluno in sql)
                {

                    IEnumerable<HistoricoAluno> historico = getHistoricoTurmas(aluno.cd_aluno, cd_pessoa_escola);
                    HistoricoAluno his_aux = historico.Where(h => ((h.Turma.cd_turma == cd_turma) || (h.Turma.cd_turma_ppt == cd_turma)) && 
                        (h.id_situacao_historico == (int) AlunoTurma.SituacaoAlunoTurma.Ativo ||
                    h.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                    h.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Movido)
                    ).OrderByDescending(x=> x.nm_sequencia).FirstOrDefault();
                    Turma turma = getTurmasAvaliacoes(aluno.cd_aluno, cd_pessoa_escola).Where(a => a.cd_turma == his_aux.Turma.cd_turma).FirstOrDefault();

                    IEnumerable<AvaliacaoAluno> avaliacao = getNotasAvaliacaoTurma(aluno.cd_aluno, turma.cd_turma, cd_pessoa_escola);
                    List<TipoAvaliacao> qtd_avaliacoes_curso = getTiposAvaliacaoComQtdAvaliacao(cd_turma, avaliacao.GroupBy(x => x.AvaliacaoTurma.Avaliacao.cd_tipo_avaliacao).Select(x => x.Key).ToList());

                    var mediaAvaliacao = processarMediaAvaliacaoTurma(avaliacao.ToList(), qtd_avaliacoes_curso);

                    aluno.nm_faltas = turma.nm_faltas;
                    foreach (var tipoAvaliacao in mediaAvaliacao.avaliacoesMedia)
                    {
                        if (aluno.cd_tipo_avaliacao != null &&
                            tipoAvaliacao.cd_tipo_avaliacao == aluno.cd_tipo_avaliacao)
                        {
                            aluno.nm_nota_aluno = tipoAvaliacao.vl_media;
                        }
                    }
                    alunos.Add(aluno);
                }

                return alunos.AsEnumerable();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<HistoricoAluno> getHistoricoTurmas(int cd_aluno, int cd_escola)
        {
            try
            {
                return (from h in db.HistoricoAluno
                    where h.cd_aluno == cd_aluno
                          && h.Turma.cd_pessoa_escola == cd_escola
                    orderby h.nm_sequencia
                    select new
                    {
                        cd_produto = h.Produto.cd_produto,
                        no_produto = h.Produto.no_produto,
                        cd_turma = h.Turma.cd_turma,
                        no_turma = h.Turma.no_turma,
                        nm_sequencia = h.nm_sequencia,
                        dt_historico = h.dt_historico,
                        nm_contrato = h.Contrato.nm_contrato,
                        id_situacao_historico = h.id_situacao_historico,
                        id_tipo_movimento = h.id_tipo_movimento,
                        dt_cadastro = h.dt_cadastro,
                        h.SysUsuario.no_login,
                        cd_turma_ppt = h.Turma.cd_turma_ppt,
                        id_turma_ppt = h.Turma.id_turma_ppt
                    }).ToList().Select(x => new HistoricoAluno
                {
                    Produto = new Produto { cd_produto = x.cd_produto, no_produto = x.no_produto },
                    Turma = new Turma { no_turma = x.no_turma, cd_turma = x.cd_turma, cd_turma_ppt = x.cd_turma_ppt, id_turma_ppt = x.id_turma_ppt},
                    nm_sequencia = x.nm_sequencia,
                    dt_historico = x.dt_historico,
                    Contrato = new Contrato { nm_contrato = x.nm_contrato },
                    id_situacao_historico = x.id_situacao_historico,
                    id_tipo_movimento = x.id_tipo_movimento,
                    dt_cadastro = x.dt_cadastro,
                    SysUsuario = new UsuarioWebSGF { no_login = x.no_login }
                });
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Turma> getTurmasAvaliacoes(int cd_aluno, int cd_escola)
        {
            try
            {
                var sql = (from t in db.Turma
                           where t.cd_pessoa_escola == cd_escola
                           && t.TurmaAluno.Where(at => at.cd_aluno == cd_aluno && at.cd_contrato > 0).Any()
                           orderby t.Produto.no_produto, t.no_turma
                           select new
                           {
                               no_turma = t.no_turma,
                               cd_turma = t.cd_turma,
                               no_produto = t.Produto.no_produto,
                               no_sala = t.Sala.no_sala,
                               no_professor = t.TurmaProfessorTurma.Where(p => p.id_professor_ativo == true && p.cd_turma == t.cd_turma).FirstOrDefault().Professor.FuncionarioPessoaFisica.dc_reduzido_pessoa,
                               dt_inicio_aula = t.dt_inicio_aula,
                               dt_final_aula = t.dt_final_aula,
                               dt_termino_turma = t.dt_termino_turma,
                               nm_aulas_dadas = t.TurmaAluno.Where(at => at.cd_aluno == cd_aluno).FirstOrDefault().nm_aulas_dadas,
                               nm_faltas = t.TurmaAluno.Where(at => at.cd_aluno == cd_aluno).FirstOrDefault().nm_faltas,
                               nm_aulas_contratadas = t.ProgramacaoTurma.Where(pt => !pt.cd_feriado.HasValue || pt.cd_feriado_desconsiderado.HasValue).Count()
                           }).ToList().Select(x => new Turma
                           {
                               no_turma = x.no_turma,
                               cd_turma = x.cd_turma,
                               Produto = new Produto() { no_produto = x.no_produto },
                               no_sala = x.no_sala,
                               no_professor = x.no_professor,
                               dt_inicio_aula = x.dt_inicio_aula,
                               dt_final_aula = x.dt_final_aula,
                               dt_termino_turma = x.dt_termino_turma,
                               nm_aulas_dadas = x.nm_aulas_dadas,
                               nm_faltas = x.nm_faltas,
                               nm_aulas_contratadas = x.nm_aulas_contratadas
                           });

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<AvaliacaoAluno> getNotasAvaliacaoTurma(int cd_aluno, int cd_turma, int cd_escola)
        {
            try
            {
                var sql = (from a in db.AvaliacaoAluno
                           where a.AvaliacaoTurma.cd_turma == cd_turma
                             && a.AvaliacaoTurma.Turma.cd_pessoa_escola == cd_escola
                             && a.cd_aluno == cd_aluno
                             && a.nm_nota_aluno.HasValue
                           orderby a.AvaliacaoTurma.Avaliacao.TipoAvaliacao.dc_tipo_avaliacao, a.AvaliacaoTurma.Avaliacao.CriterioAvaliacao.dc_criterio_avaliacao, a.AvaliacaoTurma.dt_avaliacao_turma
                           select new
                           {
                               cd_avaliacao_aluno = a.cd_avaliacao_aluno,
                               cd_avaliacao_turma = a.cd_avaliacao_turma,
                               cd_aluno = a.cd_aluno,
                               cd_turma = a.AvaliacaoTurma.Turma.cd_turma,
                               no_turma = a.AvaliacaoTurma.Turma.no_turma,
                               nm_nota_aluno = a.nm_nota_aluno,
                               dt_avaliacao_turma = a.AvaliacaoTurma.dt_avaliacao_turma,
                               dc_criterio_avaliacao = a.AvaliacaoTurma.Avaliacao.CriterioAvaliacao.dc_criterio_avaliacao,
                               dc_tipo_avaliacao = a.AvaliacaoTurma.Avaliacao.TipoAvaliacao.dc_tipo_avaliacao,
                               vl_nota = a.AvaliacaoTurma.Avaliacao.vl_nota,
                               nm_peso_avaliacao = a.AvaliacaoTurma.Avaliacao.nm_peso_avaliacao,
                               cd_tipo_avaliacao = a.AvaliacaoTurma.Avaliacao.cd_tipo_avaliacao,
                               vl_total_nota = a.AvaliacaoTurma.Avaliacao.TipoAvaliacao.vl_total_nota
                           }).ToList().Select(x => new AvaliacaoAluno
                           {
                               cd_avaliacao_aluno = x.cd_avaliacao_aluno,
                               cd_avaliacao_turma = x.cd_avaliacao_turma,
                               cd_aluno = x.cd_aluno,
                               cd_turma = x.cd_turma,
                               no_turma = x.no_turma,
                               nm_nota_aluno = x.nm_nota_aluno,
                               dt_avaliacao_turma = x.dt_avaliacao_turma,
                               dc_criterio_avaliacao = x.dc_criterio_avaliacao,
                               dc_tipo_avaliacao = x.dc_tipo_avaliacao,
                               vl_nota = x.vl_nota,
                               nm_peso_avaliacao = x.nm_peso_avaliacao,
                               AvaliacaoTurma = new AvaliacaoTurma() { Avaliacao = new Avaliacao() { cd_tipo_avaliacao = x.cd_tipo_avaliacao, TipoAvaliacao = new TipoAvaliacao() { vl_total_nota = x.vl_total_nota } } }
                           });
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<TipoAvaliacao> getTiposAvaliacaoComQtdAvaliacao(int cd_turma, List<int> cdsTiposAvaliacao)
        {
            try
            {
                var retorno = (from t in db.TipoAvaliacao
                    where cdsTiposAvaliacao.Contains(t.cd_tipo_avaliacao)
                    select new
                    {
                        t.cd_tipo_avaliacao,
                        qtd_avaliacoes = t.Avaliacao.Count()
                    }).ToList().Select(x => new TipoAvaliacao
                {
                    cd_tipo_avaliacao = x.cd_tipo_avaliacao,
                    nm_avaliacoes = x.qtd_avaliacoes
                }).ToList();
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        private AvaliacaoAlunoMediaUI processarMediaAvaliacaoTurma(List<AvaliacaoAluno> listaAvaliacaoNota, List<TipoAvaliacao> qtd_avaliacoes_curso)
        {
            List<TipoAvaliacao> avaliacoesMedia = new List<TipoAvaliacao>();
            AvaliacaoAlunoMediaUI retorno = new AvaliacaoAlunoMediaUI();
            //Ininicializa as variaveis:
            retorno.vl_maximo = 0;
            retorno.vl_total = 0;
            var nm_total_avaliacoes = 0;

            for (int i = 0; i < listaAvaliacaoNota.Count; i++)
            {
                TipoAvaliacao tipoAvaliacao = avaliacoesMedia.Where(a => a.cd_tipo_avaliacao == listaAvaliacaoNota[i].AvaliacaoTurma.Avaliacao.cd_tipo_avaliacao).FirstOrDefault();

                if (tipoAvaliacao != null)
                {
                    tipoAvaliacao.vl_soma += listaAvaliacaoNota[i].vl_nota_corrigida.Value;
                    //tipoAvaliacao.nm_avaliacoes += 1;
                }
                else
                {
                    tipoAvaliacao = new TipoAvaliacao();
                    tipoAvaliacao.nm_avaliacoes = qtd_avaliacoes_curso.Where(x => x.cd_tipo_avaliacao == listaAvaliacaoNota[i].AvaliacaoTurma.Avaliacao.cd_tipo_avaliacao).FirstOrDefault().nm_avaliacoes;
                    tipoAvaliacao.vl_soma = listaAvaliacaoNota[i].vl_nota_corrigida.Value;
                    //tipoAvaliacao.nm_avaliacoes = 1;
                    nm_total_avaliacoes += tipoAvaliacao.nm_avaliacoes;
                    tipoAvaliacao.dc_tipo_avaliacao = listaAvaliacaoNota[i].dc_tipo_avaliacao;
                    tipoAvaliacao.cd_tipo_avaliacao = listaAvaliacaoNota[i].AvaliacaoTurma.Avaliacao.cd_tipo_avaliacao;
                    tipoAvaliacao.vl_total_nota = listaAvaliacaoNota[i].AvaliacaoTurma.Avaliacao.TipoAvaliacao.vl_total_nota;
                    tipoAvaliacao.cd_turma = listaAvaliacaoNota[i].cd_turma;
                    tipoAvaliacao.no_turma = listaAvaliacaoNota[i].no_turma;

                    avaliacoesMedia.Add(tipoAvaliacao);

                    if (tipoAvaliacao.vl_total_nota.HasValue)
                        retorno.vl_maximo += (double)tipoAvaliacao.vl_total_nota;
                }
                retorno.vl_total += listaAvaliacaoNota[i].vl_nota_corrigida.Value;
            }

            retorno.avaliacoesNota = listaAvaliacaoNota;
            retorno.avaliacoesMedia = avaliacoesMedia;
            if (avaliacoesMedia != null && avaliacoesMedia.Count > 0)
            {
                //Valores do campo Aproveitamento total do aluno:
                retorno.vl_media_final = Math.Round(avaliacoesMedia.Sum(x => x.vl_soma) / nm_total_avaliacoes, 1, MidpointRounding.AwayFromZero);
                //A Média Parcial será calculada pelo número de avaliações realizadas.
                retorno.vl_media_parcial = Math.Round(avaliacoesMedia.Sum(x => x.vl_soma) / listaAvaliacaoNota.Count, 1, MidpointRounding.AwayFromZero);
                retorno.vl_aproveitamento_total = Math.Round(retorno.vl_media_final / retorno.vl_media_parcial * 100, 2, MidpointRounding.AwayFromZero);

                if (avaliacoesMedia.Count == 1 && (avaliacoesMedia.Sum(x => x.nm_avaliacoes) / 2) == listaAvaliacaoNota.Count)
                    retorno.vl_aproveitamento_total = 50;
                //retorno.vl_media_final = Math.Round(avaliacoesMedia.Sum(x => x.vl_media), 2) / avaliacoesMedia.Count;
                //retorno.vl_aproveitamento_total = Math.Round(retorno.vl_media_final * 10, 2);
            }
            return retorno;
        }

        public IEnumerable<Aluno> getAlunosPorEventoDiario(int cd_turma, int cd_pessoa_escola, DateTime dtAula, int cd_evento, int cd_diario_aula)
        {
            try
            {
                List<int> cds_turmas_escolas = new List<int>();
                List<TurmaEscola> turmasEscola = new List<TurmaEscola>();

                int cdPessoaEscola = (from t in db.Turma
                    where (t.cd_turma == cd_turma || t.cd_turma_ppt == cd_turma)
                    select t.cd_pessoa_escola).FirstOrDefault();

                turmasEscola = (from t in db.Turma
                    join te in db.TurmaEscola on t.cd_turma equals te.cd_turma
                    where t.cd_turma == cd_turma
                    select te).ToList();

                cds_turmas_escolas = turmasEscola.Select(x => x.cd_escola).ToList();
                int cdCurso = (int)((from t in db.Turma where t.cd_turma == cd_turma select t.cd_curso).FirstOrDefault());

                var sql = (from a in db.Aluno
                           orderby a.AlunoPessoaFisica.no_pessoa
                           where ((a.cd_pessoa_escola == cd_pessoa_escola && (cdPessoaEscola == 0 || cdPessoaEscola != cd_pessoa_escola)) ||
                                  (a.cd_pessoa_escola == cd_pessoa_escola || cds_turmas_escolas.Contains(a.cd_pessoa_escola) && cdPessoaEscola == cd_pessoa_escola)) &&
                                a.HistoricoAluno.Where(ha =>
                                                      ha.cd_turma == cd_turma &&
                                                       (DbFunctions.TruncateTime(ha.dt_historico) <= dtAula.Date &&
                                                        (ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                         ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                         ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Encerrado)
                                                         && ha.nm_sequencia == a.HistoricoAluno.Where(han => han.cd_turma == cd_turma
                                                                                           && DbFunctions.TruncateTime(han.dt_historico) <= dtAula.Date).Max(x => x.nm_sequencia)
                                                        )
                                                      ).Any()
                           select new
                           {
                               cd_aluno = a.cd_aluno,
                               no_aluno = a.AlunoPessoaFisica.no_pessoa,
                               ehselecionado = a.AlunoEvento.Where(e => e.cd_diario_aula == cd_diario_aula &&
                                                                        e.cd_evento == cd_evento).Any(),
                               qtd_faltas = a.AlunoTurma.Where(x => x.cd_turma == cd_turma).FirstOrDefault().nm_faltas
                           }).ToList().Select(x => new Aluno
                           {
                               cd_aluno = x.cd_aluno,
                               nomeAluno = x.no_aluno,
                               selecionadoAluno = x.ehselecionado,
                               nm_faltas = x.qtd_faltas
                           });
                return sql;

            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Aluno> getAlunosPorEvento(int cd_turma, int cd_pessoa_escola, int cd_evento, int cd_diario_aula)
        {
            try
            {
                var sql = (from a in db.Aluno
                           where a.AlunoEvento.Where(ea => ea.cd_aluno == a.cd_aluno &&
                                 ea.DiarioAula.cd_pessoa_empresa == cd_pessoa_escola &
                                 ea.DiarioAula.cd_diario_aula == cd_diario_aula &&
                                 ea.cd_evento == cd_evento).Any()
                           select new
                           {
                               cd_aluno = a.cd_aluno,
                               no_aluno = a.AlunoPessoaFisica.no_pessoa,
                               ehselecionado = true
                           }).ToList().Select(x => new Aluno
                           {
                               cd_aluno = x.cd_aluno,
                               nomeAluno = x.no_aluno,
                               selecionadoAluno = x.ehselecionado
                           });
                return sql;

            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Aluno> getAlunoPorTurma(int cdTurma, int cdEscola, int opcao)
        {
            try
            {
                IEnumerable<Aluno> retorno;
                if (opcao == (int)FundacaoFisk.SGF.Web.Services.Secretaria.Model.MudancasInternas.OpcoesMudanca.RetornarTurmaOri)
                    retorno = (from a in db.Aluno
                               where a.AlunoTurma.Where(at => at.cd_aluno == a.cd_aluno && at.cd_turma == cdTurma &&
                                                at.cd_turma_origem > 0 && at.cd_situacao_aluno_origem > 0 &&
                                                (at.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                 at.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                 at.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Encerrado ||
                                                 at.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Aguardando)).Any()

                               select new
                             {
                                 cd_pessoa_aluno = a.cd_pessoa_aluno,
                                 cd_aluno = a.cd_aluno,
                                 no_aluno = a.AlunoPessoaFisica.no_pessoa,
                                 alunoPessoaFisica = a.AlunoPessoaFisica,
                                 aluno_dt_nascimento = a.AlunoPessoaFisica != null ? a.AlunoPessoaFisica.dt_nascimento : null,
                                 AlunoTurma = a.AlunoTurma
                             }).ToList().Select(x => new Aluno
                             {
                                 cd_pessoa_aluno = x.cd_pessoa_aluno,
                                 cd_contrato = x.cd_aluno > 0 ? x.AlunoTurma.Where(at => at.cd_aluno == x.cd_aluno && at.cd_turma == cdTurma).FirstOrDefault().cd_contrato : 0,
                                 cd_aluno = x.cd_aluno,
                                 nomeAluno = x.no_aluno,
                                 AlunoPessoaFisica = x.alunoPessoaFisica,
                                 aluno_dt_nascimento = x.aluno_dt_nascimento
                             });
                else
                    if (opcao == (int)FundacaoFisk.SGF.Web.Services.Secretaria.Model.MudancasInternas.OpcoesMudanca.MudarTurma)
                        retorno = (from a in db.Aluno
                                   where a.AlunoTurma.Where(at => at.cd_aluno == a.cd_aluno && at.cd_turma == cdTurma &&
                                                    (at.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                     at.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                     at.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Encerrado)).Any()

                                   select new
                                 {
                                     cd_pessoa_aluno = a.cd_pessoa_aluno,
                                     cd_aluno = a.cd_aluno,
                                     no_aluno = a.AlunoPessoaFisica.no_pessoa,
                                     alunoPessoaFisica = a.AlunoPessoaFisica,
                                     aluno_dt_nascimento = a.AlunoPessoaFisica != null ? a.AlunoPessoaFisica.dt_nascimento : null,
                                     AlunoTurma = a.AlunoTurma
                                 }).ToList().Select(x => new Aluno
                                 {
                                     cd_pessoa_aluno = x.cd_pessoa_aluno,
                                     cd_contrato = x.cd_aluno > 0 ? x.AlunoTurma.Where(at => at.cd_aluno == x.cd_aluno && at.cd_turma == cdTurma).FirstOrDefault().cd_contrato : 0,
                                     cd_aluno = x.cd_aluno,
                                     nomeAluno = x.no_aluno,
                                     AlunoPessoaFisica = x.alunoPessoaFisica,
                                     aluno_dt_nascimento = x.aluno_dt_nascimento
                                 });
                    else
                        retorno = (from a in db.Aluno
                                   where a.AlunoTurma.Where(at => at.cd_aluno == a.cd_aluno && at.cd_turma == cdTurma &&
                                                    (at.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                     at.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                     at.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Encerrado)).Any()

                                   select new
                                   {
                                       cd_pessoa_aluno = a.cd_pessoa_aluno,
                                       cd_aluno = a.cd_aluno,
                                       no_aluno = a.AlunoPessoaFisica.no_pessoa,
                                       alunoPessoaFisica = a.AlunoPessoaFisica,
                                       aluno_dt_nascimento = a.AlunoPessoaFisica != null ? a.AlunoPessoaFisica.dt_nascimento : null,
                                       AlunoTurma = a.AlunoTurma
                                   }).ToList().Select(x => new Aluno
                                   {
                                       cd_pessoa_aluno = x.cd_pessoa_aluno,
                                       cd_contrato = x.cd_aluno > 0 ? x.AlunoTurma.Where(at => at.cd_aluno == x.cd_aluno && at.cd_turma == cdTurma).FirstOrDefault().cd_contrato : 0,
                                       cd_aluno = x.cd_aluno,
                                       nomeAluno = x.no_aluno,
                                       AlunoPessoaFisica = x.alunoPessoaFisica,
                                       aluno_dt_nascimento = x.aluno_dt_nascimento
                                   });
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<AlunoSearchUI> getAlunoDesistente(SearchParameters parametros, string nome, string email, bool inicio, bool? status, int cdEscola, string cpf, int cdSituacao, int sexo)
        {
            try
            {
                IEntitySorter<AlunoSearchUI> sorter = EntitySorter<AlunoSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<AlunoSearchUI> sql;

                if (sexo > 0)
                {
                    sql = from c in db.Aluno.AsNoTracking()
                          join pf in db.PessoaSGF.OfType<PessoaFisicaSGF>().AsNoTracking() on c.cd_pessoa_aluno equals pf.cd_pessoa
                          where c.cd_pessoa_escola == cdEscola &&
                                c.AlunoPessoaFisica.nm_sexo == sexo &&
                                c.AlunoTurma.Where(at => at.cd_aluno == c.cd_aluno &&
                                    at.Desistencia.Where(d => d.cd_aluno_turma == at.cd_aluno_turma).Any()).Any()
                          select new AlunoSearchUI
                          {
                              cd_aluno = c.cd_aluno,
                              cd_pessoa_aluno = pf.cd_pessoa,
                              cd_pessoa_escola = c.cd_pessoa_escola,
                              cd_pessoa = pf.cd_pessoa,
                              no_pessoa = pf.no_pessoa,
                              dc_reduzido_pessoa = pf.dc_reduzido_pessoa,
                              dt_cadastramento = pf.dt_cadastramento,
                              id_aluno_ativo = c.id_aluno_ativo,
                              nm_cpf = pf.cd_pessoa_cpf > 0 ? pf.PessoaSGFQueUsoOCpf.nm_cpf : pf.nm_cpf,
                              no_pessoa_dependente = pf.PessoaSGFQueUsoOCpf.no_pessoa,
                              ext_img_pessoa = pf.ext_img_pessoa,
                              no_atividade = pf.Atividade.no_atividade,
                              //email = pf.TelefonePessoa.Where(t=> t.cd_tipo_telefone == 4).FirstOrDefault().dc_fone_mail,
                              ativExtra = c.AtividadeExtraAluno.Where(at => at.ind_participacao == false).Count()
                          };
                }
                else
                {
                    sql = from c in db.Aluno.AsNoTracking()
                          join pf in db.PessoaSGF.OfType<PessoaFisicaSGF>().AsNoTracking() on c.cd_pessoa_aluno equals pf.cd_pessoa
                          where c.cd_pessoa_escola == cdEscola &&
                                c.AlunoTurma.Where(at => at.cd_aluno == c.cd_aluno &&
                                    at.Desistencia.Where(d => d.cd_aluno_turma == at.cd_aluno_turma).Any()).Any()
                          select new AlunoSearchUI
                          {
                              cd_aluno = c.cd_aluno,
                              cd_pessoa_aluno = pf.cd_pessoa,
                              cd_pessoa_escola = c.cd_pessoa_escola,
                              cd_pessoa = pf.cd_pessoa,
                              no_pessoa = pf.no_pessoa,
                              dc_reduzido_pessoa = pf.dc_reduzido_pessoa,
                              dt_cadastramento = pf.dt_cadastramento,
                              id_aluno_ativo = c.id_aluno_ativo,
                              nm_cpf = pf.cd_pessoa_cpf > 0 ? pf.PessoaSGFQueUsoOCpf.nm_cpf : pf.nm_cpf,
                              no_pessoa_dependente = pf.PessoaSGFQueUsoOCpf.no_pessoa,
                              ext_img_pessoa = pf.ext_img_pessoa,
                              no_atividade = pf.Atividade.no_atividade,
                              //email = pf.TelefonePessoa.Where(t => t.cd_tipo_telefone == 4).FirstOrDefault().dc_fone_mail,
                              ativExtra = c.AtividadeExtraAluno.Where(at => at.ind_participacao == false).Count()
                          };
                }

                sql = sorter.Sort(sql);

                if (!string.IsNullOrEmpty(nome))
                {
                    if (inicio)
                        sql = from func in sql
                              where func.no_pessoa.StartsWith(nome)
                              select func;
                    else
                        sql = from c in sql
                              where c.no_pessoa.Contains(nome)
                              select c;
                }
                if (!string.IsNullOrEmpty(cpf))
                    sql = from c in sql
                          where c.nm_cpf == cpf
                          select c;
                //todo
                if (!string.IsNullOrEmpty(email))
                    if (inicio)
                        sql = from c in sql
                              where (from t in db.TelefoneSGF
                                     where t.cd_pessoa == c.cd_pessoa &&
                                         t.cd_tipo_telefone == 4 && t.id_telefone_principal == true && t.dc_fone_mail.StartsWith(email)
                                     select t.cd_telefone).Any()
                              select c;
                    else
                        sql = from c in sql
                              where (from t in db.TelefoneSGF
                                     where t.cd_pessoa == c.cd_pessoa &&
                                         t.cd_tipo_telefone == 4 && t.id_telefone_principal == true && t.dc_fone_mail.Contains(email)
                                     select t.cd_telefone).Any()
                              select c;

                if (status != null)
                    sql = from c in sql
                          where c.id_aluno_ativo == status
                          select c;
                if (cdSituacao > 0)
                    sql = from c in sql
                          where db.AlunoTurma.Where(at => at.cd_situacao_aluno_turma == cdSituacao).Any()
                          select c;

                int limite = sql.Count();

                parametros.ajustaParametrosPesquisa(limite);
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<AlunoSearchUI> getAlunoPorTurmaSearch(SearchParameters parametros, string nome, string email, bool inicio, int origemFK, bool? status, int cdEscola, string cpf, int cdSituacao, int sexo, int cdTurma, int opcao)
        {
            try
            {
                IEntitySorter<AlunoSearchUI> sorter = EntitySorter<AlunoSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<AlunoSearchUI> sql;
                IQueryable<Aluno> retorno;

                retorno = from c in db.Aluno.AsNoTracking()
                          where c.cd_pessoa_escola == cdEscola
                          select c;

                if (origemFK == 28)
                {
                    retorno = from t in retorno
                          join ar in db.AlunoAulaReposicao on t.cd_aluno equals ar.cd_aluno
                          select t;
                }
                if (origemFK == 29)
                {
                    opcao =  (int)TipoConsultaAlunoEnum.DESISTENCIA;
                }

                if (sexo > 0)
                    retorno = from c in retorno
                              where c.AlunoPessoaFisica.nm_sexo == sexo
                              select c;
                if (opcao == (int)TipoConsultaAlunoEnum.DESISTENCIA)
                    retorno = from c in retorno
                              where c.AlunoTurma.Where(at => at.cd_aluno == c.cd_aluno && (at.cd_turma == cdTurma || cdTurma == 0) && at.cd_contrato > 0 &&
                                                    (at.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                     at.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Rematriculado)).Any()
                              select c;
                if (opcao == (int)TipoConsultaAlunoEnum.CANCELA_DESISTENCIA)
                    retorno = from c in retorno
                              where c.AlunoTurma.Where(at => at.cd_aluno == c.cd_aluno && (at.cd_turma == cdTurma || cdTurma == 0) && at.cd_contrato > 0 &&
                                                       at.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Desistente).Any()
                              select c;
                if (opcao == (int)TipoConsultaAlunoEnum.HAS_ATIVO && cdTurma > 0)
                    retorno = from c in retorno
                        where c.AlunoTurma.Where(at => at.cd_aluno == c.cd_aluno && (at.cd_turma == cdTurma || cdTurma == 0) && at.cd_contrato > 0 ).Any()
                        select c;

                sql = from c in retorno
                      join pf in db.PessoaSGF.OfType<PessoaFisicaSGF>().AsNoTracking() on c.cd_pessoa_aluno equals pf.cd_pessoa
                      select new AlunoSearchUI
                      {
                          cd_aluno = c.cd_aluno,
                          cd_pessoa_aluno = pf.cd_pessoa,
                          cd_pessoa_escola = c.cd_pessoa_escola,
                          cd_pessoa = pf.cd_pessoa,
                          no_pessoa = pf.no_pessoa,
                          dc_reduzido_pessoa = pf.dc_reduzido_pessoa,
                          dt_cadastramento = pf.dt_cadastramento,
                          id_aluno_ativo = c.id_aluno_ativo,
                          nm_cpf = pf.cd_pessoa_cpf > 0 ? pf.PessoaSGFQueUsoOCpf.nm_cpf : pf.nm_cpf,
                          no_pessoa_dependente = pf.PessoaSGFQueUsoOCpf.no_pessoa,
                          ext_img_pessoa = pf.ext_img_pessoa,
                          no_atividade = pf.Atividade.no_atividade,
                          ativExtra = c.AtividadeExtraAluno.Where(at => at.ind_participacao == false).Count(),
                          cd_aluno_turma = cdTurma > 0 ? c.AlunoTurma.Where(at => at.cd_aluno == c.cd_aluno && at.cd_turma == cdTurma).FirstOrDefault().cd_aluno_turma : 0,
                          cd_contrato = cdTurma > 0 ? c.Contratos.Where(ct => ct.cd_aluno == c.cd_aluno && ct.AlunoTurma.Any(at => at.cd_turma == cdTurma)).FirstOrDefault().cd_contrato : 0
                      };
                sql = sorter.Sort(sql);

                if (!string.IsNullOrEmpty(nome))
                {
                    if (inicio)
                        sql = from func in sql
                              where func.no_pessoa.StartsWith(nome)
                              select func;
                    else
                        sql = from c in sql
                              where c.no_pessoa.Contains(nome)
                              select c;
                }
                if (!string.IsNullOrEmpty(cpf))
                    sql = from c in sql
                          where c.nm_cpf == cpf
                          select c;
                //todo
                if (!string.IsNullOrEmpty(email))
                    if (inicio)
                        sql = from c in sql
                              where (from t in db.TelefoneSGF
                                     where t.cd_pessoa == c.cd_pessoa &&
                                         t.cd_tipo_telefone == 4 && t.id_telefone_principal == true && t.dc_fone_mail.StartsWith(email)
                                     select t.cd_telefone).Any()
                              select c;
                    else
                        sql = from c in sql
                              where (from t in db.TelefoneSGF
                                     where t.cd_pessoa == c.cd_pessoa &&
                                         t.cd_tipo_telefone == 4 && t.id_telefone_principal == true && t.dc_fone_mail.Contains(email)
                                     select t.cd_telefone).Any()
                              select c;

                if (status != null)
                    sql = from c in sql
                          where c.id_aluno_ativo == status
                          select c;
                if (cdSituacao > 0)
                    sql = from c in sql
                          where db.AlunoTurma.Where(at => at.cd_situacao_aluno_turma == cdSituacao).Any()
                          select c;

                int limite = sql.Count();

                parametros.ajustaParametrosPesquisa(limite);
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<AlunoSearchUI> getAlunoPorTurmaSearchAulaReposicao(SearchParameters parametros, string nome, string email, bool inicio, int origemFK, bool? status, int cdEscola, string cpf, int cdSituacao, int sexo, int cdTurma, int opcao)
        {
            try
            {
                IEntitySorter<AlunoSearchUI> sorter = EntitySorter<AlunoSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<AlunoSearchUI> sql;
                IQueryable<Aluno> retorno;

                retorno = from c in db.Aluno
                        .Include(x => x.AlunoPessoaFisica)
                        .Include(x => x.AlunoPessoaFisica.PessoaSGFQueUsoOCpf)
                        .Include(x => x.AlunoPessoaFisica.Atividade).AsNoTracking()
                          where c.cd_pessoa_escola == cdEscola
                          select c;

                if (origemFK == 28)
                {
                    retorno = from t in retorno
                              where (from ar in db.AlunoAulaReposicao where ar.cd_aluno == t.cd_aluno select ar).Any()
                              select t;
                }

                if (origemFK == 28)
                {
                    retorno = from t in retorno
                              join ar in db.AlunoAulaReposicao on t.cd_aluno equals ar.cd_aluno
                              select t;
                }

                if (sexo > 0)
                    retorno = from c in retorno
                              where c.AlunoPessoaFisica.nm_sexo == sexo
                              select c;

                if (opcao == (int)TipoConsultaAlunoEnum.HAS_ATIVO && cdTurma > 0)
                    retorno = from c in retorno
                              where c.AlunoTurma.Where(at => at.cd_aluno == c.cd_aluno && (at.cd_turma == cdTurma || (at.cd_turma != cdTurma && at.Turma.cd_turma_ppt == cdTurma) || cdTurma == 0) && at.cd_contrato > 0).Any()
                              select c;

                sql = from c in retorno
                      select new AlunoSearchUI
                      {
                          cd_aluno = c.cd_aluno,
                          cd_pessoa_aluno = c.AlunoPessoaFisica.cd_pessoa,
                          cd_pessoa_escola = c.cd_pessoa_escola,
                          cd_pessoa = c.AlunoPessoaFisica.cd_pessoa,
                          no_pessoa = c.AlunoPessoaFisica.no_pessoa,
                          dc_reduzido_pessoa = c.AlunoPessoaFisica.dc_reduzido_pessoa,
                          dt_cadastramento = c.AlunoPessoaFisica.dt_cadastramento,
                          id_aluno_ativo = c.id_aluno_ativo,
                          nm_cpf = c.AlunoPessoaFisica.cd_pessoa_cpf > 0 ? c.AlunoPessoaFisica.PessoaSGFQueUsoOCpf.nm_cpf : c.AlunoPessoaFisica.nm_cpf,
                          no_pessoa_dependente = c.AlunoPessoaFisica.PessoaSGFQueUsoOCpf.no_pessoa,
                          ext_img_pessoa = c.AlunoPessoaFisica.ext_img_pessoa,
                          no_atividade = c.AlunoPessoaFisica.Atividade.no_atividade,
                          ativExtra = c.AtividadeExtraAluno.Where(at => at.ind_participacao == false).Count(),
                          cd_aluno_turma = cdTurma > 0 ? c.AlunoTurma.Where(at => at.cd_aluno == c.cd_aluno && at.cd_turma == cdTurma || (at.cd_turma != cdTurma && at.Turma.cd_turma_ppt == cdTurma)).FirstOrDefault().cd_aluno_turma : 0,
                          cd_contrato = cdTurma > 0 ? c.Contratos.Where(ct => ct.cd_aluno == c.cd_aluno && ct.AlunoTurma.Any(at => at.cd_turma == cdTurma || (at.cd_turma != cdTurma && at.Turma.cd_turma_ppt == cdTurma))).FirstOrDefault().cd_contrato : 0
                      };
                sql = sorter.Sort(sql);

                if (!string.IsNullOrEmpty(nome))
                {
                    if (inicio)
                        sql = from func in sql
                              where func.no_pessoa.StartsWith(nome)
                              select func;
                    else
                        sql = from c in sql
                              where c.no_pessoa.Contains(nome)
                              select c;
                }
                if (!string.IsNullOrEmpty(cpf))
                    sql = from c in sql
                          where c.nm_cpf == cpf
                          select c;
                //todo
                if (!string.IsNullOrEmpty(email))
                    if (inicio)
                        sql = from c in sql
                              where (from t in db.TelefoneSGF
                                     where t.cd_pessoa == c.cd_pessoa &&
                                         t.cd_tipo_telefone == 4 && t.id_telefone_principal == true && t.dc_fone_mail.StartsWith(email)
                                     select t.cd_telefone).Any()
                              select c;
                    else
                        sql = from c in sql
                              where (from t in db.TelefoneSGF
                                     where t.cd_pessoa == c.cd_pessoa &&
                                         t.cd_tipo_telefone == 4 && t.id_telefone_principal == true && t.dc_fone_mail.Contains(email)
                                     select t.cd_telefone).Any()
                              select c;

                if (status != null)
                    sql = from c in sql
                          where c.id_aluno_ativo == status
                          select c;
                if (cdSituacao > 0)
                    sql = from c in sql
                          where db.AlunoTurma.Where(at => at.cd_situacao_aluno_turma == cdSituacao).Any()
                          select c;

                int limite = sql.Count();

                parametros.ajustaParametrosPesquisa(limite);
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<AlunoSearchUI> getAlunoPorTurmaControleFaltaSearch(SearchParameters parametros, List<int> cdAlunos, string nome, string email, bool inicio, bool? status, int cdEscola, string cpf, int cdSituacao, int sexo, int cdTurma, int opcao, DateTime? dataFinalHistorico)
        {
            try
            {
                IEntitySorter<AlunoSearchUI> sorter = EntitySorter<AlunoSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<AlunoSearchUI> sql;
                IQueryable<Aluno> retorno;

                retorno = from c in db.Aluno.AsNoTracking()
                          where c.cd_pessoa_escola == cdEscola
                          select c;

                if (sexo > 0)
                    retorno = from c in retorno
                              where c.AlunoPessoaFisica.nm_sexo == sexo
                              select c;
                if (opcao == (int)TipoConsultaAlunoEnum.DESISTENCIA)
                    retorno = from c in retorno
                              where c.AlunoTurma.Where(at => at.cd_aluno == c.cd_aluno && (at.cd_turma == cdTurma || cdTurma == 0) && at.cd_contrato > 0 &&
                                                    (at.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                     at.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Rematriculado)).Any()
                              select c;
                if (opcao == (int)TipoConsultaAlunoEnum.CANCELA_DESISTENCIA)
                    retorno = from c in retorno
                              where c.AlunoTurma.Where(at => at.cd_aluno == c.cd_aluno && (at.cd_turma == cdTurma || cdTurma == 0) && at.cd_contrato > 0 &&
                                                       at.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Desistente).Any()
                              select c;
                if (opcao == (int)TipoConsultaAlunoEnum.HAS_ATIVO && cdTurma > 0)
                    retorno = from c in retorno
                              where c.AlunoTurma.Where(at => at.cd_aluno == c.cd_aluno && (at.cd_turma == cdTurma || cdTurma == 0) && at.cd_contrato > 0).Any()
                              select c;

                if (dataFinalHistorico.HasValue)
                {
                    retorno = from c in retorno
                        where (c.AlunoTurma.Where(x => x.cd_turma == cdTurma || x.Turma.cd_turma_ppt == cdTurma)
                        .FirstOrDefault().nm_faltas > 0) &&
                            c.HistoricoAluno.Any(ha =>
                            (ha.cd_turma == cdTurma || ha.Turma.cd_turma_ppt == cdTurma) &&
                            (DbFunctions.TruncateTime(ha.dt_historico) <= ((DateTime)dataFinalHistorico).Date
                             && ha.nm_sequencia == c.HistoricoAluno.Where(han =>
                                     (han.cd_turma == cdTurma || han.Turma.cd_turma_ppt == cdTurma)
                                     && DbFunctions.TruncateTime(han.dt_historico) <= ((DateTime)dataFinalHistorico).Date)
                                 .Max(x => x.nm_sequencia)
                            )) 
                        select c;
                }

                sql = from c in retorno
                      join pf in db.PessoaSGF.OfType<PessoaFisicaSGF>().AsNoTracking() on c.cd_pessoa_aluno equals pf.cd_pessoa
                      select new AlunoSearchUI
                      {
                          cd_aluno = c.cd_aluno,
                          cd_pessoa_aluno = pf.cd_pessoa,
                          cd_pessoa_escola = c.cd_pessoa_escola,
                          cd_pessoa = pf.cd_pessoa,
                          no_pessoa = pf.no_pessoa,
                          dc_reduzido_pessoa = pf.dc_reduzido_pessoa,
                          dt_cadastramento = pf.dt_cadastramento,
                          id_aluno_ativo = c.id_aluno_ativo,
                          nm_cpf = pf.cd_pessoa_cpf > 0 ? pf.PessoaSGFQueUsoOCpf.nm_cpf : pf.nm_cpf,
                          no_pessoa_dependente = pf.PessoaSGFQueUsoOCpf.no_pessoa,
                          ext_img_pessoa = pf.ext_img_pessoa,
                          no_atividade = pf.Atividade.no_atividade,
                          ativExtra = c.AtividadeExtraAluno.Where(at => at.ind_participacao == false).Count(),
                          cd_aluno_turma = cdTurma > 0 ? c.AlunoTurma.Where(at => at.cd_aluno == c.cd_aluno && at.cd_turma == cdTurma).FirstOrDefault().cd_aluno_turma : 0,
                          cd_contrato = cdTurma > 0 ? c.Contratos.Where(ct => ct.cd_aluno == c.cd_aluno && ct.AlunoTurma.Any(at => at.cd_turma == cdTurma)).FirstOrDefault().cd_contrato : 0
                      };
                sql = sorter.Sort(sql);

                if (!string.IsNullOrEmpty(nome))
                {
                    if (inicio)
                        sql = from func in sql
                              where func.no_pessoa.StartsWith(nome)
                              select func;
                    else
                        sql = from c in sql
                              where c.no_pessoa.Contains(nome)
                              select c;
                }
                if (!string.IsNullOrEmpty(cpf))
                    sql = from c in sql
                          where c.nm_cpf == cpf
                          select c;
                //todo
                if (!string.IsNullOrEmpty(email))
                    if (inicio)
                        sql = from c in sql
                              where (from t in db.TelefoneSGF
                                     where t.cd_pessoa == c.cd_pessoa &&
                                         t.cd_tipo_telefone == 4 && t.id_telefone_principal == true && t.dc_fone_mail.StartsWith(email)
                                     select t.cd_telefone).Any()
                              select c;
                    else
                        sql = from c in sql
                              where (from t in db.TelefoneSGF
                                     where t.cd_pessoa == c.cd_pessoa &&
                                         t.cd_tipo_telefone == 4 && t.id_telefone_principal == true && t.dc_fone_mail.Contains(email)
                                     select t.cd_telefone).Any()
                              select c;

                if (status != null)
                    sql = from c in sql
                          where c.id_aluno_ativo == status
                          select c;
                if (cdSituacao > 0)
                    sql = from c in sql
                          where db.AlunoTurma.Where(at => at.cd_situacao_aluno_turma == cdSituacao).Any()
                          select c;

                if (cdAlunos.Count > 0)
                {
                    sql = from c in sql
                        where !cdAlunos.Contains(c.cd_aluno)
                        select c;
                }

               

                int limite = sql.Count();

                parametros.ajustaParametrosPesquisa(limite);
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }



        public bool existeAlunoMatOrRemEscola(int cdEscola)
        {
            var sql = from aluno in db.Aluno
                      where
                        aluno.AlunoTurma.Any(a => a.cd_aluno == aluno.cd_aluno
                                               && (a.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo
                                               || a.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado))
                                               && aluno.cd_pessoa_escola == cdEscola
                      select aluno;
            return sql.Count() > 0;
        }

        public AlunoSearchUI getAlunoByCodForGrid(int cd_aluno, int cd_empresa)
        {
            try
            {
                var sql = (from a in db.Aluno
                           where a.cd_pessoa_aluno == cd_aluno && a.cd_pessoa_escola == cd_empresa
                           select new AlunoSearchUI
                           {
                               cd_aluno = a.cd_aluno,
                               cd_pessoa_aluno = a.cd_pessoa_aluno,
                               cd_pessoa_escola = a.cd_pessoa_escola,
                               cd_pessoa = a.cd_pessoa_aluno,
                               no_pessoa = a.AlunoPessoaFisica.no_pessoa,
                               dc_reduzido_pessoa = a.AlunoPessoaFisica.dc_reduzido_pessoa,
                               dt_cadastramento = a.AlunoPessoaFisica.dt_cadastramento,
                               id_aluno_ativo = a.id_aluno_ativo,
                               nm_cpf = a.AlunoPessoaFisica.cd_pessoa_cpf > 0 ? a.AlunoPessoaFisica.PessoaSGFQueUsoOCpf.nm_cpf : a.AlunoPessoaFisica.nm_cpf,
                               no_pessoa_dependente = a.AlunoPessoaFisica.PessoaSGFQueUsoOCpf.no_pessoa,
                               ext_img_pessoa = a.AlunoPessoaFisica.ext_img_pessoa,
                               no_atividade = a.AlunoPessoaFisica.Atividade.no_atividade,
                               cd_pessoa_dependente = a.AlunoPessoaFisica.PessoaSGFQueUsoOCpf.cd_pessoa,
                               //email = pf.TelefonePessoa.Where(t => t.cd_tipo_telefone == 4).FirstOrDefault().dc_fone_mail,
                               ativExtra = a.AtividadeExtraAluno.Where(at => at.ind_participacao == false).Count(),
                               pc_bolsa = a.Bolsa.pc_bolsa,
                               dt_inicio_bolsa = a.Bolsa.dt_inicio_bolsa,
                               vl_abatimento_matricula = a.Contratos.Any(x => x.vl_pre_matricula > 0) ? 0 : a.Prospect.vl_matricula_prospect
                           }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public AlunoSearchUI getAlunoPorTurmaPPTFilha(int cdEscola, int cdTurma, int cdTurmaPai, int opcao)
        {
            try
            {
                AlunoSearchUI sql;
                IQueryable<Aluno> retorno;

                retorno = from c in db.Aluno
                          where c.cd_pessoa_escola == cdEscola &&
                          c.AlunoTurma.Where(at => at.Turma.cd_turma_ppt == cdTurmaPai).Any()
                          select c;


                if (opcao == (int)TipoConsultaAlunoEnum.DESISTENCIA)
                    retorno = from c in retorno
                              where c.AlunoTurma.Where(at => at.cd_aluno == c.cd_aluno && at.cd_turma == cdTurma &&
                                                    (at.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                     at.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Rematriculado)).Any()
                              select c;
                if (opcao == (int)TipoConsultaAlunoEnum.CANCELA_DESISTENCIA)
                    retorno = from c in retorno
                              where c.AlunoTurma.Where(at => at.cd_aluno == c.cd_aluno && at.cd_turma == cdTurma &&
                                                       at.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Desistente).Any()
                              select c;


                sql = (from c in retorno
                       join pf in db.PessoaSGF.OfType<PessoaFisicaSGF>() on c.cd_pessoa_aluno equals pf.cd_pessoa
                       select new AlunoSearchUI
                       {
                           cd_aluno = c.cd_aluno,
                           cd_pessoa_aluno = pf.cd_pessoa,
                           cd_pessoa_escola = c.cd_pessoa_escola,
                           cd_pessoa = pf.cd_pessoa,
                           no_pessoa = pf.no_pessoa,
                           dc_reduzido_pessoa = pf.dc_reduzido_pessoa,
                           dt_cadastramento = pf.dt_cadastramento,
                           id_aluno_ativo = c.id_aluno_ativo,
                           nm_cpf = pf.cd_pessoa_cpf > 0 ? pf.PessoaSGFQueUsoOCpf.nm_cpf : pf.nm_cpf,
                           no_pessoa_dependente = pf.PessoaSGFQueUsoOCpf.no_pessoa,
                           ext_img_pessoa = pf.ext_img_pessoa,
                           no_atividade = pf.Atividade.no_atividade,
                           ativExtra = c.AtividadeExtraAluno.Where(at => at.ind_participacao == false).Count(),
                           cd_aluno_turma = c.AlunoTurma.Where(at => at.cd_aluno == c.cd_aluno && at.cd_turma == cdTurma).FirstOrDefault().cd_aluno_turma,
                           cd_contrato = c.Contratos.Where(ct => ct.cd_aluno == c.cd_aluno && ct.AlunoTurma.Any(at => at.cd_turma == cdTurma)).FirstOrDefault().cd_contrato
                       }).FirstOrDefault();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<AlunoRel> getRelAluno(string nome, int cdResp, string telefone, string email, bool? status, int cdEscola, DateTime? dtaIni, DateTime? dtaFinal, int cd_midia, List<int> cdSituacaoA, bool exibirEnderecos)
        {
            try
            {
                DateTime DataIni = DateTime.MinValue;
                DateTime DataFim = DateTime.MaxValue;
                if (dtaIni.HasValue) DataIni = dtaIni.Value.Date;
                if (dtaFinal.HasValue) DataFim = dtaFinal.Value.Date.AddDays(1).AddSeconds(-1);
                IEnumerable<AlunoRel> sql;
                var sql1 = from c in db.Aluno
                           where c.cd_pessoa_escola == cdEscola
                           select c;
                if (cd_midia > 0)
                    sql1 = from a in sql1
                           where a.cd_midia == cd_midia
                           select a;
                if (cdResp > 0)
                    sql1 = from c in sql1
                           join pf in db.PessoaSGF.OfType<PessoaFisicaSGF>() on c.cd_pessoa_aluno equals pf.cd_pessoa
                           where pf.PessoaPaiRelacionamento.Where(pr => pr.cd_pessoa_filho == cdResp).Any()
                           select c;
                if (exibirEnderecos)
                {
                    sql1 = from c in sql1
                           join pf in db.PessoaSGF.OfType<PessoaFisicaSGF>() on c.cd_pessoa_aluno equals pf.cd_pessoa
                           where pf.EnderecoPrincipal != null
                                select c;
                }

                if (!cdSituacaoA.Contains(100))
                    sql1 = from c in sql1
                          where (db.AlunoTurma.Where(a => a.cd_aluno == c.cd_aluno && a.cd_situacao_aluno_turma != null && cdSituacaoA.Contains((int)a.cd_situacao_aluno_turma)).Any())
                          select c;

                sql = (from c in sql1
                       join pf in db.PessoaSGF.OfType<PessoaFisicaSGF>() on c.cd_pessoa_aluno equals pf.cd_pessoa
                       select new //AlunoRel
                       {
                           cd_aluno = c.cd_aluno,
                           cd_pessoa_aluno = c.cd_pessoa_aluno,
                           no_aluno = pf.no_pessoa,
                           no_resp = db.PessoaSGF.Where(pr => pr.PessoaFilhoRelacionamento.Where(r => r.cd_pessoa_pai == c.cd_pessoa_aluno).Any()).FirstOrDefault().no_pessoa,
                           dt_cadastramento = pf.dt_cadastramento,
                           nm_telefone = pf.TelefonePessoa.Where(tel => tel.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.TELEFONE).FirstOrDefault().dc_fone_mail,
                           nm_celular = pf.TelefonePessoa.Where(tel => tel.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.CELULAR).FirstOrDefault().dc_fone_mail,
                           email_aluno = pf.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL).FirstOrDefault().dc_fone_mail,
                           id_aluno_ativo = c.id_aluno_ativo,
                           exibir_coluna = exibirEnderecos,
                           enderecoAluno = //pf.EnderecoPrincipal
                                pf.EnderecoPrincipal == null ? "" :
                                pf.EnderecoPrincipal.Logradouro.no_localidade == null ? "" :
                                pf.EnderecoPrincipal.TipoLogradouro.no_tipo_logradouro == null ? "" :
                                pf.EnderecoPrincipal.TipoLogradouro.no_tipo_logradouro + " " + pf.EnderecoPrincipal.Logradouro.no_localidade +
                                (pf.EnderecoPrincipal.dc_num_endereco == null || pf.EnderecoPrincipal.dc_num_endereco == "" ? "" : " Nº " + pf.EnderecoPrincipal.dc_num_endereco) +
                                (pf.EnderecoPrincipal.dc_compl_endereco == null || pf.EnderecoPrincipal.dc_compl_endereco == "" ? "" : " / " + pf.EnderecoPrincipal.dc_compl_endereco) +
                                (pf.EnderecoPrincipal.Bairro.no_localidade == null ? "" : ", Bairro: " + pf.EnderecoPrincipal.Bairro.no_localidade) +
                                (pf.EnderecoPrincipal.Logradouro.dc_num_cep == null ? "" : ", CEP: " + pf.EnderecoPrincipal.Logradouro.dc_num_cep) +
                                (pf.EnderecoPrincipal.Cidade.no_localidade == null ? "" : ", Cidade: " + pf.EnderecoPrincipal.Cidade.no_localidade) +
                                (pf.EnderecoPrincipal.Estado.no_localidade == null ? "" : " - " + pf.EnderecoPrincipal.Estado.Estado.sg_estado),
                           raf = (from pr in db.PessoaRaf where pr.cd_pessoa == c.cd_pessoa_aluno select pr).FirstOrDefault()

                       }).ToList().Select(x => new AlunoRel
                       {
                           cd_aluno = x.cd_aluno,
                           cd_pessoa_aluno = x.cd_pessoa_aluno,
                           no_aluno = x.no_aluno,
                           no_resp = x.no_resp,
                           dt_cadastramento = x.dt_cadastramento,
                           nm_telefone = x.nm_telefone,
                           nm_celular = x.nm_celular,
                           email_aluno = x.email_aluno,
                           id_aluno_ativo = x.id_aluno_ativo,
                           exibir_coluna = x.exibir_coluna,
                           dc_endereco = x.enderecoAluno,
                           nm_raf = x.raf != null ? x.raf.nm_raf : "",
                           id_bloqueado = x.raf != null ? (x.raf.id_bloqueado == true) ? "Sim" : "Não" : "",
                           id_raf_liberado = x.raf != null ? (x.raf.id_raf_liberado == true) ? "Sim" : "Não" : "",

                       }).OrderBy(mtr => mtr.no_aluno);

                if (!string.IsNullOrEmpty(nome))
                    sql = from c in sql
                          where c.no_aluno.ToUpper().Contains(nome.ToUpper())
                          select c;

                if (!string.IsNullOrEmpty(email))
                    sql = from c in sql
                          where (from t in db.TelefoneSGF
                                 where t.cd_pessoa == c.cd_pessoa_aluno &&
                                     t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL &&
                                     t.id_telefone_principal == true &&
                                     t.dc_fone_mail.StartsWith(email)
                                 select t.cd_telefone).Any()
                          select c;

                if (!string.IsNullOrEmpty(telefone))
                    sql = from c in sql
                          where (from t in db.TelefoneSGF
                                 where t.cd_pessoa == c.cd_pessoa_aluno &&
                                     (t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.TELEFONE ||
                                      t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.CELULAR) &&
                                     t.id_telefone_principal == true &&
                                     t.dc_fone_mail == telefone
                                 select t.cd_telefone).Any()
                          select c;

                if (dtaIni.HasValue)
                    sql = from c in sql
                          where c.dt_cadastramento >= DataIni
                          select c;
                if (dtaFinal.HasValue)
                    sql = from c in sql
                          where c.dt_cadastramento <= DataFim
                          select c;

                if (status != null)
                    sql = from c in sql
                          where c.id_aluno_ativo == status
                          select c;

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

        public IEnumerable<AlunoSearchUI> getRptAlunosTurma(int cd_turma, bool id_turma_ppt, List<int> cd_situacao_aluno_turma, int cdEscolaAluno)
        {
            try
            {
                var sql = from at in db.AlunoTurma
                         where at.Aluno.cd_pessoa_escola == cdEscolaAluno
                          select at;
                if (id_turma_ppt)
                    sql = from at in sql
                          where at.Turma.cd_turma_ppt == cd_turma && (cd_situacao_aluno_turma.Contains(100) || (at.cd_situacao_aluno_turma.HasValue && cd_situacao_aluno_turma.Contains(at.cd_situacao_aluno_turma.Value)))
                          select at;
                else
                    sql = from at in sql
                          where at.cd_turma == cd_turma && (cd_situacao_aluno_turma.Contains(100) || (at.cd_situacao_aluno_turma.HasValue && cd_situacao_aluno_turma.Contains(at.cd_situacao_aluno_turma.Value)))
                          select at;
                //c.PessoaFilhoRelacionamento.Where(r => r.RelacionamentoFilhoPapel.cd_papel == papel).Any() || c.PessoaPaiRelacionamento.Where(rp => rp.RelacionamentoPaiPapel.cd_papel == papel).Any())

                var retorno = (from a in sql
                               select new
                               {
                                   cd_aluno = a.cd_aluno,
                                   cd_aluno_turma = a.cd_aluno_turma,
                                   cd_situacao_aluno_turma = a.cd_situacao_aluno_turma,
                                   no_pessoa = a.Aluno.AlunoPessoaFisica.no_pessoa,
                                   dt_nascimento = a.Aluno.AlunoPessoaFisica.dt_nascimento,
                                   celular = a.Aluno.AlunoPessoaFisica.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.CELULAR && t.id_telefone_principal).FirstOrDefault().dc_fone_mail,
                                   telefone = a.Aluno.AlunoPessoaFisica.Telefone.dc_fone_mail,
                                   no_pessoa_dependente = (from p in db.PessoaSGF
                                                           where p.PessoaFilhoRelacionamento.Where(r => r.cd_papel_filho == (int)PapelSGF.TipoPapelSGF.RESPONSAVEL &&
                                                                r.cd_pessoa_pai == a.Aluno.cd_pessoa_aluno).Any()
                                                           select p.no_pessoa).FirstOrDefault(),
                                  a.Contrato.AnoEscolar.dc_ano_escolar,
                                  a.Contrato.AnoEscolar.Escolaridade.no_escolaridade
                               }).ToList().Select(x => new AlunoSearchUI
                               {
                                   cd_aluno = x.cd_aluno,
                                   cd_aluno_turma = x.cd_aluno_turma,
                                   no_pessoa = x.no_pessoa,
                                   dt_nascimento = x.dt_nascimento,
                                   celular = x.celular,
                                   telefone = x.telefone,
                                   no_pessoa_dependente = x.no_pessoa_dependente,
                                   situacaoAluno = AlunoTurma.getSituacaoAlunoTurma(x.cd_situacao_aluno_turma),
                                   dc_ano_escolar = !string.IsNullOrEmpty(x.no_escolaridade) && !string.IsNullOrEmpty(x.dc_ano_escolar) ? x.no_escolaridade + " - " + x.dc_ano_escolar :
                                                         !string.IsNullOrEmpty(x.dc_ano_escolar) ? x.dc_ano_escolar : ""
                               });
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<AlunoSearchUI> getRptAlunosTurmaEncerrar(int cd_turma, bool id_turma_ppt, DateTime dtaIniAula, DateTime dtaFim, int cdEscolaAluno)
        {
            try
            {
                var sql = from at in db.AlunoTurma
                    where at.Aluno.cd_pessoa_escola == cdEscolaAluno
                          select at;

                if (id_turma_ppt)
                    sql = from at in sql
                           
                          join tf in db.Turma on at.cd_turma equals tf.cd_turma
                          join tPai in db.Turma on tf.cd_turma_ppt equals tPai.cd_turma
                          join a in db.Aluno on at.cd_aluno equals a.cd_aluno
                          join pa in db.PessoaSGF on a.cd_pessoa_aluno equals pa.cd_pessoa
                          where 
                             tPai.cd_turma == cd_turma &&
                             (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                              at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado)
                          && tf.dt_termino_turma == null && (tf.dt_final_aula != null && tf.dt_final_aula >= dtaIniAula && (dtaFim == null || tf.dt_final_aula <= dtaFim))
                          
                          select at;
                else
                    sql = from at in sql
                          where at.cd_turma == cd_turma &&
                                at.Turma.cd_turma_ppt == null &&
                                at.Turma.dt_termino_turma == null && (at.Turma.dt_final_aula != null && at.Turma.dt_final_aula >= dtaIniAula && (dtaFim == null || at.Turma.dt_final_aula <= dtaFim)) &&
                                (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                 at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado)
                          select at;
                //c.PessoaFilhoRelacionamento.Where(r => r.RelacionamentoFilhoPapel.cd_papel == papel).Any() || c.PessoaPaiRelacionamento.Where(rp => rp.RelacionamentoPaiPapel.cd_papel == papel).Any())

                var retorno = (from a in sql
                               select new
                               {
                                   cd_aluno = a.cd_aluno,
                                   cd_aluno_turma = a.cd_aluno_turma,
                                   cd_situacao_aluno_turma = a.cd_situacao_aluno_turma,
                                   no_pessoa = a.Aluno.AlunoPessoaFisica.no_pessoa,
                                   dt_nascimento = a.Aluno.AlunoPessoaFisica.dt_nascimento,
                                   celular = a.Aluno.AlunoPessoaFisica.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.CELULAR && t.id_telefone_principal).FirstOrDefault().dc_fone_mail,
                                   telefone = a.Aluno.AlunoPessoaFisica.Telefone.dc_fone_mail,
                                   dt_inicio_aula = a.Turma.dt_inicio_aula,
                                   dt_final_aula = a.Turma.id_turma_ppt == true ?
                                   ((from at in db.AlunoTurma
                           
                                    join tf in db.Turma on at.cd_turma equals tf.cd_turma
                                    join tPai in db.Turma on tf.cd_turma_ppt equals tPai.cd_turma
                                    join al in db.Aluno on at.cd_aluno equals al.cd_aluno
                                    join pa in db.PessoaSGF on al.cd_pessoa_aluno equals pa.cd_pessoa
                                    where 
                                        tPai.cd_turma == cd_turma && a.cd_aluno == al.cd_aluno && 
                                        (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                        at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado)
                                    && tf.dt_termino_turma == null && (tf.dt_final_aula != null && tf.dt_final_aula >= dtaIniAula && (dtaFim == null || tf.dt_final_aula <= dtaFim))
                          
                                    select tf).FirstOrDefault().dt_final_aula) :
                                    a.Turma.dt_final_aula,
                                   
                                   no_turma = a.Turma.no_turma,
                                    
                                   //no_turma = a.Turma.id_turma_ppt == true ? 
                                    //a.Turma.TurmaFilhasPPT.FirstOrDefault().no_turma: a.Turma.no_turma,
                                   no_pessoa_dependente = (from p in db.PessoaSGF
                                                           where p.PessoaFilhoRelacionamento.Where(r => r.cd_papel_filho == (int)PapelSGF.TipoPapelSGF.RESPONSAVEL &&
                                                                r.cd_pessoa_pai == a.Aluno.cd_pessoa_aluno).Any()
                                                           select p.no_pessoa).FirstOrDefault(),
                                   a.Contrato.AnoEscolar.dc_ano_escolar,
                                   a.Contrato.AnoEscolar.Escolaridade.no_escolaridade
                               }).ToList().Select(x => new AlunoSearchUI
                               {
                                   cd_aluno = x.cd_aluno,
                                   cd_aluno_turma = x.cd_aluno_turma,
                                   no_pessoa = x.no_pessoa,
                                   dt_nascimento = x.dt_nascimento,
                                   celular = x.celular,
                                   telefone = x.telefone,
                                   //dt_inicio_aula = DateTime.ParseExact(x.dt_inicio_aula, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture),
                                   //dt_final_aula = DateTime.ParseExact(x.dt_final_aula, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture),
                                   dt_inicio_aula = x.dt_inicio_aula,
                                   //dt_final_aula = x.dt_final_aula == null ? x.dt_final_aula : (DateTime?)null,
                                   dt_final_aula = x.dt_final_aula,
                                   no_turma = x.no_turma,
                                   no_pessoa_dependente = x.no_pessoa_dependente,
                                   situacaoAluno = AlunoTurma.getSituacaoAlunoTurma(x.cd_situacao_aluno_turma),
                                   dc_ano_escolar = !string.IsNullOrEmpty(x.no_escolaridade) && !string.IsNullOrEmpty(x.dc_ano_escolar) ? x.no_escolaridade + " - " + x.dc_ano_escolar :
                                                         !string.IsNullOrEmpty(x.dc_ano_escolar) ? x.dc_ano_escolar : ""
                               });
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }


        public IEnumerable<AlunoSearchUI> getRptAlunosTurmaNova(int cd_turma, bool id_turma_ppt, DateTime dtaIniAula, Nullable<DateTime> dtaFim, int cdEscolaAluno)
        {
            try
            {
                var sql = from at in db.AlunoTurma
                            where at.Aluno.cd_pessoa_escola == cdEscolaAluno
                          select at;

                if (id_turma_ppt)
                    sql = from at in sql
                          join tf in db.Turma on at.cd_turma equals tf.cd_turma
                          join tPai in db.Turma on tf.cd_turma_ppt equals tPai.cd_turma
                          join a in db.Aluno on at.cd_aluno equals a.cd_aluno
                          join pa in db.PessoaSGF on a.cd_pessoa_aluno equals pa.cd_pessoa
                          where
                              tPai.cd_turma == cd_turma &&
                             (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                              at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado)
                              && tf.dt_termino_turma == null  && tf.cd_turma_enc == null &&
                              !tf.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada) &&
                              (tf.dt_inicio_aula != null && tf.dt_inicio_aula >= dtaIniAula && (dtaFim == null || tf.dt_inicio_aula <= dtaFim))
                          select at;
                else
                    sql = from at in sql
                          where at.cd_turma == cd_turma &&
                                at.Turma.cd_turma_ppt == null &&
                                //at.Turma.dt_termino_turma == null && (at.Turma.dt_final_aula != null && at.Turma.dt_final_aula >= dtaIniAula && (dtaFim == null || at.Turma.dt_final_aula <= dtaFim)) &&
                                //at.Turma.dt_termino_turma == null && 
                                (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                 at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado)
                          select at;
                //c.PessoaFilhoRelacionamento.Where(r => r.RelacionamentoFilhoPapel.cd_papel == papel).Any() || c.PessoaPaiRelacionamento.Where(rp => rp.RelacionamentoPaiPapel.cd_papel == papel).Any())

                var retorno = (from a in sql
                               select new
                               {
                                   cd_aluno = a.cd_aluno,
                                   cd_aluno_turma = a.cd_aluno_turma,
                                   cd_situacao_aluno_turma = a.cd_situacao_aluno_turma,
                                   no_pessoa = a.Aluno.AlunoPessoaFisica.no_pessoa,
                                   dt_nascimento = a.Aluno.AlunoPessoaFisica.dt_nascimento,
                                   celular = a.Aluno.AlunoPessoaFisica.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.CELULAR && t.id_telefone_principal).FirstOrDefault().dc_fone_mail,
                                   telefone = a.Aluno.AlunoPessoaFisica.Telefone.dc_fone_mail,
                                   dt_inicio_aula = a.Turma.dt_inicio_aula,
                                   dt_final_aula = a.Turma != null ?
                                   ((from at in db.AlunoTurma
                                     join tf in db.Turma on at.cd_turma equals tf.cd_turma
                                     join tPai in db.Turma on tf.cd_turma_ppt equals tPai.cd_turma
                                     join al in db.Aluno on at.cd_aluno equals al.cd_aluno
                                     join pa in db.PessoaSGF on al.cd_pessoa_aluno equals pa.cd_pessoa
                                     where
                                         tPai.cd_turma == cd_turma && a.cd_aluno == al.cd_aluno &&
                                         (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                         at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado)
                                     select tf).FirstOrDefault().dt_final_aula) : a.Turma.dt_final_aula,
                                   no_turma = a.Turma.no_turma,
                                   no_pessoa_dependente = (from p in db.PessoaSGF
                                                           where p.PessoaFilhoRelacionamento.Where(r => r.cd_papel_filho == (int)PapelSGF.TipoPapelSGF.RESPONSAVEL &&
                                                                r.cd_pessoa_pai == a.Aluno.cd_pessoa_aluno).Any()
                                                           select p.no_pessoa).FirstOrDefault(),
                                   a.Contrato.AnoEscolar.dc_ano_escolar,
                                   a.Contrato.AnoEscolar.Escolaridade.no_escolaridade
                               }).ToList().Select(x => new AlunoSearchUI
                               {
                                   cd_aluno = x.cd_aluno,
                                   cd_aluno_turma = x.cd_aluno_turma,
                                   no_pessoa = x.no_pessoa,
                                   dt_nascimento = x.dt_nascimento,
                                   celular = x.celular,
                                   telefone = x.telefone,
                                   //dt_inicio_aula = DateTime.ParseExact(x.dt_inicio_aula, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture),
                                   //dt_final_aula = DateTime.ParseExact(x.dt_final_aula, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture),
                                   dt_inicio_aula = x.dt_inicio_aula,
                                   //dt_final_aula = x.dt_final_aula == null ? x.dt_final_aula : (DateTime?)null,
                                   dt_final_aula = x.dt_final_aula,
                                   no_turma = x.no_turma,
                                   no_pessoa_dependente = x.no_pessoa_dependente,
                                   situacaoAluno = AlunoTurma.getSituacaoAlunoTurma(x.cd_situacao_aluno_turma),
                                   dc_ano_escolar = !string.IsNullOrEmpty(x.no_escolaridade) && !string.IsNullOrEmpty(x.dc_ano_escolar) ? x.no_escolaridade + " - " + x.dc_ano_escolar :
                                                         !string.IsNullOrEmpty(x.dc_ano_escolar) ? x.dc_ano_escolar : ""
                               });
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool existeProdutoAlunoMat(int cd_escola, bool turmaFilha, int cd_produto, List<int> cdAlunos, int cd_turma)
        {
            try
            {
                bool existe = false;
                if (turmaFilha)
                    existe = (from at in db.AlunoTurma
                              where at.Turma.cd_pessoa_escola == cd_escola && cdAlunos.Contains(at.cd_aluno) && at.cd_turma != cd_turma && (at.Turma.cd_turma_enc != cd_turma || !at.Turma.cd_turma_enc.HasValue) &&
                                       (at.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                        at.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                        at.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Aguardando) &&
                                      ((at.Turma.cd_turma_ppt == null && at.Turma.cd_produto == cd_produto && at.Turma.dt_termino_turma == null) &&
                                      !(at.Turma.cd_turma_ppt != null && at.Turma.cd_produto == cd_produto && at.Turma.dt_termino_turma == null)
                                      )
                              select at.cd_aluno).Any();

                else
                    existe = (from at in db.AlunoTurma
                              where at.Turma.cd_pessoa_escola == cd_escola &&
                              cdAlunos.Contains(at.cd_aluno) &&
                              at.cd_turma != cd_turma && (at.Turma.cd_turma_enc != cd_turma || !at.Turma.cd_turma_enc.HasValue) &&
                              (
                                (at.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                 at.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                        at.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Aguardando) &&
                                (at.Turma.cd_produto == cd_produto && at.Turma.dt_termino_turma == null)
                              )
                              select at.cd_aluno).Any();
                return existe;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public DataTable getRptMediaAlunos(int cd_pessoa_escola, int cd_turma, int tipoTurma, int cdCurso, int cdProduto, int pesOpcao, int pesTipoAluno,
                decimal? vl_media, DateTime dt_inicial, DateTime dt_final)
        {
            try
            {
                if (vl_media == null) vl_media = 0;
                DataTable dtReportData = new DataTable();

                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringReportProcedure"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(@"sp_getRptMediaAlunos", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandTimeout = 180;
                        command.Parameters.AddWithValue("@cd_pessoa_escola", cd_pessoa_escola);
                        command.Parameters.AddWithValue("@cd_turma", cd_turma);
                        command.Parameters.AddWithValue("@cdCurso", cdCurso);
                        command.Parameters.AddWithValue("@cdProduto", cdProduto);
                        command.Parameters.AddWithValue("@tipoTurma", tipoTurma);
                        command.Parameters.AddWithValue("@pesOpcao", pesOpcao);
                        command.Parameters.AddWithValue("@pesTipoAluno", pesTipoAluno);
                        command.Parameters.AddWithValue("@vl_media", vl_media);
                        command.Parameters.AddWithValue("@dt_inicial", dt_inicial);
                        command.Parameters.AddWithValue("@dt_final", dt_final);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dtReportData);
                            return dtReportData;
                        }
                    }

                }




                //                db.Database.Connection.Open();
                //                var command = db.Database.Connection.CreateCommand();
                //command.CommandType = System.Data.CommandType.StoredProcedure;
                //command.CommandText = @"sp_getRptMediaAlunos";
                //command.CommandTimeout = 180;

                //var sqlParameters = new List<System.Data.SqlClient.SqlParameter>();

                //sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@cd_pessoa_escola", cd_pessoa_escola));

                //sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@cd_turma", cd_turma));

                //sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@cdCurso", cdCurso));

                //sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@cdProduto", cdProduto));
                //sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@tipoTurma", tipoTurma));
                //sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@pesOpcao", pesOpcao));
                //sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@pesTipoAluno", pesTipoAluno));
                //sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@vl_media", vl_media));
                //sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@dt_inicial", dt_inicial));
                //sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@dt_final", dt_final));


                //var parameter = new System.Data.SqlClient.SqlParameter("@result", System.Data.SqlDbType.Int);
                //parameter.Direction = System.Data.ParameterDirection.ReturnValue;

                //sqlParameters.Add(parameter);

                //command.Parameters.AddRange(sqlParameters.ToArray());

                //                var teste = command.ExecuteReader();

                //db.Database.Connection.Close();
                //return teste;


                //string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringReportProcedure"].ConnectionString;
                //string providerName = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringReportProcedure"].ProviderName;


                //              DataTable dtReportData = new DataTable();

                //DBHelper dbSql = new DBHelper(connectionString, providerName);
                //if (vl_media == null) vl_media = 0;
                //DBParameter param1 = new DBParameter("@cd_pessoa_escola", cd_pessoa_escola, DbType.Int32);
                //DBParameter param2 = new DBParameter("@cd_turma", cd_turma, DbType.Int32);
                //DBParameter param3 = new DBParameter("@cdCurso", cdCurso, DbType.Int32);
                //DBParameter param4 = new DBParameter("@cdProduto", cdProduto, DbType.Int32);
                //DBParameter param5 = new DBParameter("@tipoTurma", tipoTurma, DbType.Int32);
                //DBParameter param6 = new DBParameter("@pesOpcao", pesOpcao, DbType.Int32);
                //DBParameter param7 = new DBParameter("@pesTipoAluno", pesTipoAluno, DbType.Int32);
                //DBParameter param8 = new DBParameter("@vl_media", vl_media, DbType.Double);
                //DBParameter param9 = new DBParameter("@dt_inicial", dt_inicial, DbType.DateTime);
                //DBParameter param10 = new DBParameter("@dt_final", dt_final, DbType.DateTime);

                //DBParameterCollection paramCollection = new DBParameterCollection();
                //paramCollection.Add(param1);
                //paramCollection.Add(param2);
                //paramCollection.Add(param3);
                //paramCollection.Add(param4);
                //paramCollection.Add(param5);
                //paramCollection.Add(param6);
                //paramCollection.Add(param7);
                //paramCollection.Add(param8);
                //paramCollection.Add(param9);
                //paramCollection.Add(param10);

                //dtReportData = dbSql.ExecuteDataTable("sp_getRptMediaAlunos", paramCollection, CommandType.StoredProcedure);
                //return dtReportData;

                //                ObjectResult<sp_getRptMediaAlunos_Result> result = db.sp_getRptMediaAlunos(cd_pessoa_escola, cd_turma, tipoTurma, cdCurso, cdProduto, (byte?)pesOpcao, (byte?)pesTipoAluno, (double?)vl_media, dt_inicial, dt_final);
                //                return result;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<AlunoSearchUI> GetAlunoSearchAulaPer(SearchParameters parametros, string nome, string email, bool inicio, bool? status, int cdEscola, string cpf, int cdSituacao, int sexo,
                                                                bool semTurma, bool movido, int tipoAluno, DateTime dtaAula)
        {
            try
            {
                IEntitySorter<AlunoSearchUI> sorter = EntitySorter<AlunoSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<AlunoSearchUI> retorno;
                var sql = from c in db.Aluno.AsNoTracking()
                          join pf in db.PessoaSGF.OfType<PessoaFisicaSGF>().AsNoTracking() on c.cd_pessoa_aluno equals pf.cd_pessoa
                          where c.cd_pessoa_escola == cdEscola &&
                          c.AlunoTurma.Where(at => at.Turma.cd_turma_ppt > 0 && (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                                                 at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado) &&
                                                                                 at.Turma.dt_inicio_aula <= dtaAula && (!at.Turma.dt_final_aula.HasValue || at.Turma.dt_final_aula.Value >= dtaAula)).Any()
                          select c;
                if (sexo > 0)
                    sql = from a in sql
                          where a.AlunoPessoaFisica.nm_sexo == sexo
                          select a;
                if (!string.IsNullOrEmpty(nome))
                {
                    if (inicio)
                        sql = from a in sql
                              where a.AlunoPessoaFisica.no_pessoa.StartsWith(nome)
                              select a;
                    else
                        sql = from c in sql
                              where c.AlunoPessoaFisica.no_pessoa.Contains(nome)
                              select c;
                }
                if (!string.IsNullOrEmpty(cpf))
                    sql = from c in sql
                          where c.AlunoPessoaFisica.nm_cpf == cpf
                          select c;
                if (!string.IsNullOrEmpty(email))
                    if (inicio)
                        sql = from a in sql
                              where (from t in db.TelefoneSGF
                                     where t.cd_pessoa == a.cd_pessoa_aluno &&
                                         t.cd_tipo_telefone == 4 && t.id_telefone_principal == true && t.dc_fone_mail.StartsWith(email)
                                     select t.cd_telefone).Any()
                              select a;
                    else
                        sql = from a in sql
                              where (from t in db.TelefoneSGF
                                     where t.cd_pessoa == a.cd_pessoa_aluno &&
                                         t.cd_tipo_telefone == 4 && t.id_telefone_principal == true && t.dc_fone_mail.Contains(email)
                                     select t.cd_telefone).Any()
                              select a;

                if (status != null)
                    sql = from a in sql
                          where a.id_aluno_ativo == status
                          select a;
                if (cdSituacao < 100)
                    sql = from c in sql
                          where db.AlunoTurma.Where(a => a.cd_aluno == c.cd_aluno && a.cd_situacao_aluno_turma == cdSituacao).Any()
                          select c;
                if (movido)
                    sql = from s in sql
                          where db.AlunoTurma.Where(a => a.cd_aluno == s.cd_aluno && a.cd_contrato == null && a.cd_turma_origem > 0).Any()
                          select s;
                if (semTurma)
                    sql = from s in sql
                          where !db.AlunoTurma.Where(a => a.cd_aluno == s.cd_aluno).Any()
                          select s;
                if (tipoAluno > 0)
                    switch (tipoAluno)
                    {
                        case (int)TipoConsultaAlunoEnum.ALUNO:
                            sql = from a in sql
                                  where a.Contratos.Any()
                                  select a;
                            break;
                        case (int)TipoConsultaAlunoEnum.ALUNO_CLIENTE:
                            sql = from a in sql
                                  where !a.Contratos.Any()
                                  select a;
                            break;
                    }
                retorno = from c in sql
                          join pf in db.PessoaSGF.OfType<PessoaFisicaSGF>().AsNoTracking() on c.cd_pessoa_aluno equals pf.cd_pessoa
                          where c.cd_pessoa_escola == cdEscola
                          select new AlunoSearchUI
                          {
                              cd_aluno = c.cd_aluno,
                              cd_pessoa_aluno = pf.cd_pessoa,
                              cd_pessoa_escola = c.cd_pessoa_escola,
                              cd_pessoa = pf.cd_pessoa,
                              no_pessoa = pf.no_pessoa,
                              dc_reduzido_pessoa = pf.dc_reduzido_pessoa,
                              dt_cadastramento = pf.dt_cadastramento,
                              id_aluno_ativo = c.id_aluno_ativo,
                              nm_cpf = pf.cd_pessoa_cpf > 0 ? pf.PessoaSGFQueUsoOCpf.nm_cpf : pf.nm_cpf,
                              no_pessoa_dependente = pf.PessoaSGFQueUsoOCpf.no_pessoa,
                              cd_pessoa_dependente = pf.PessoaSGFQueUsoOCpf.cd_pessoa,
                              ext_img_pessoa = pf.ext_img_pessoa,
                              no_atividade = pf.Atividade.no_atividade,
                              email = pf.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL).FirstOrDefault().dc_fone_mail,
                              telefone = pf.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.TELEFONE).Any() ? pf.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.TELEFONE).FirstOrDefault().dc_fone_mail : pf.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.CELULAR).FirstOrDefault().dc_fone_mail,
                              ativExtra = c.AtividadeExtraAluno.Where(at => at.ind_participacao == false).Count(),
                              pc_bolsa = c.Bolsa.pc_bolsa,
                              dt_inicio_bolsa = c.Bolsa.dt_inicio_bolsa
                          };

                retorno = sorter.Sort(retorno);

                int limite = sql.Count();

                parametros.ajustaParametrosPesquisa(limite);
                retorno = retorno.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;

                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<ReportPercentualTerminoEstagio> getRptAlunosProximoCurso(int cd_escola, int cd_turma)
        {
            try
            {
                int? cd_curso = null;
                var sqlResult = db.Curso.Where(x => db.Curso.Any(e => e.Turma.Any(t => t.cd_turma == cd_turma && t.cd_pessoa_escola == cd_escola && e.cd_proximo_curso == x.cd_curso))).FirstOrDefault() ?? null;
                if (sqlResult != null) cd_curso = sqlResult.cd_curso;

                //var cd_produto = db.Turma.Where(t=> t.cd_turma == cd_turma).FirstOrDefault().cd_turma;
                var sql = (from a in db.Aluno
                           where a.cd_pessoa_escola == cd_escola && a.AlunoTurma.Where(at => at.cd_turma == cd_turma).Any() &&
                                 a.HistoricoAluno.Where(ha => ha.cd_turma == cd_turma && ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Encerrado &&
                                                              ha.nm_sequencia == a.HistoricoAluno.Where(han => han.cd_turma == cd_turma).Max(x => x.nm_sequencia) &&
                                                               a.HistoricoAluno.Where(hap => hap.cd_produto == ha.cd_produto && hap.nm_sequencia > ha.nm_sequencia &&
                                                                                      (hap.id_situacao_historico == (int)HistoricoAluno.SituacaoHistorico.ATIVO ||
                                                                                       hap.id_situacao_historico == (int)HistoricoAluno.SituacaoHistorico.REMATRICULADO)).Any()).Any()
                           select new
                           {
                               a.cd_aluno,
                               a.AlunoPessoaFisica.no_pessoa,
                               material = a.Movimento.Any(m => m.ItensMovimento.Any(im => im.Item.Cursos.Any(c => c.cd_curso == cd_curso))) ? true : false,
                               no_turma = a.HistoricoAluno.Where(hap => hap.Turma.cd_curso == cd_curso &&
                                                                        (hap.id_situacao_historico == (int)HistoricoAluno.SituacaoHistorico.ATIVO ||
                                                                         hap.id_situacao_historico == (int)HistoricoAluno.SituacaoHistorico.REMATRICULADO)).FirstOrDefault().Turma.no_turma

                           }).ToList().Select(x => new ReportPercentualTerminoEstagio
                           {
                               cd_aluno = x.cd_aluno,
                               no_aluno = x.no_pessoa,
                               no_turma = x.no_turma,
                               id_material = x.material
                           }).ToList();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public string getObservacaoAluno(int cdAluno, int cdEscola)
        {
            var sql = (from aluno in db.Aluno
                       where aluno.cd_aluno == cdAluno /*&& aluno.cd_pessoa_escola == cdEscola*/
                       select new
                       {
                           txt_obs_pessoa = aluno.AlunoPessoaFisica.txt_obs_pessoa,
                       }).ToList().Select(x => new Aluno
                       {
                           txt_obs_pessoa = x.txt_obs_pessoa,
                       }).FirstOrDefault();
            return sql.txt_obs_pessoa;
        }

        public IEnumerable<AlunoSearchUI> getRptAlunosTurmaEncerrar(List<int> cdTurmas, DateTime? pDtaFimI, DateTime? pDtaFimF, int cd_escola, int tipoTurma)
        {
            try
            {
                SGFWebContext dbContext = new SGFWebContext();
                int cd_origem = Int32.Parse(dbContext.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                var sql = from at in db.AlunoTurma
                          where at.Turma.cd_pessoa_escola == cd_escola && db.Titulo.Where(ti => ti.cd_pessoa_empresa == cd_escola && at.cd_contrato == ti.cd_origem_titulo &&
                                                   ti.id_origem_titulo == cd_origem &&
                                                   at.Aluno.cd_pessoa_aluno == ti.cd_pessoa_titulo &&
                                                   (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                    at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado) &&
                                                   ti.id_status_titulo == (byte)FundacaoFisk.SGF.GenericModel.Titulo.StatusTitulo.ABERTO &&
                                                   //ti.id_status_cnab == (byte)FundacaoFisk.SGF.GenericModel.Titulo.StatusCnabTitulo.INICIAL &&
                                                   //&& ti.dt_vcto_titulo >= pDtaFimI.Value && (pDtaFimF == null || ti.dt_vcto_titulo <= pDtaFimF.Value)
                                                    !ti.BaixaTitulo.Any(x => x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA &&
                                                                                x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO)
                                  ).Any()
                          select at;


                if (tipoTurma > 0)
                {
                    if (tipoTurma == (int)Turma.TipoTurma.PPT)
                    {
                        sql = from t in sql
                              where t.Turma.cd_turma_ppt != null && cdTurmas.Contains((int)t.Turma.cd_turma_ppt) &&
                                    t.Turma.dt_termino_turma == null && (t.Turma.dt_final_aula != null && t.Turma.dt_final_aula >= pDtaFimI && (pDtaFimF == null || t.Turma.dt_final_aula <= pDtaFimF)) &&
                                    t.Turma.ProgramacaoTurma.Any()
                              select t;
                    }

                    if (tipoTurma == (int)Turma.TipoTurma.NORMAL)
                    {
                        sql = from t in sql
                              where cdTurmas.Contains(t.Turma.cd_turma) &&
                                    t.Turma.dt_termino_turma == null && (t.Turma.dt_final_aula != null && t.Turma.dt_final_aula >= pDtaFimI && (pDtaFimF == null || t.Turma.dt_final_aula <= pDtaFimF)) &&
                                    t.Turma.ProgramacaoTurma.Any()
                              select t;
                    }
                }
                else
                {
                    sql = from t in sql
                          where ((t.Turma.cd_turma_ppt != null && cdTurmas.Contains((int)t.Turma.cd_turma_ppt)) ||
                                 (cdTurmas.Contains(t.Turma.cd_turma))) &&
                                 t.Turma.dt_termino_turma == null && (t.Turma.dt_final_aula != null && t.Turma.dt_final_aula >= pDtaFimI && (pDtaFimF == null || t.Turma.dt_final_aula <= pDtaFimF)) &&
                                 t.Turma.ProgramacaoTurma.Any()
                          select t;
                }

                var retorno = (from a in sql
                               select new
                               {
                                   cd_aluno = a.cd_aluno,
                                   cd_aluno_turma = a.cd_aluno_turma,
                                   no_pessoa = a.Aluno.AlunoPessoaFisica.no_pessoa,
                                   no_turma = a.Turma.no_turma,
                                   cd_contrato = a.Contrato.nm_contrato,
                                   titulos = db.Titulo.Where(ti => ti.cd_pessoa_empresa == cd_escola && a.cd_contrato == ti.cd_origem_titulo &&
                                                   ti.id_origem_titulo == cd_origem &&
                                                   a.Aluno.cd_pessoa_aluno == ti.cd_pessoa_titulo &&
                                                   (a.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                    a.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado) &&
                                                   ti.id_status_titulo == (byte)FundacaoFisk.SGF.GenericModel.Titulo.StatusTitulo.ABERTO &&
                                       //ti.id_status_cnab == (byte)FundacaoFisk.SGF.GenericModel.Titulo.StatusCnabTitulo.INICIAL &&
                                       //&& ti.dt_vcto_titulo >= pDtaFimI.Value && (pDtaFimF == null || ti.dt_vcto_titulo <= pDtaFimF.Value)
                                                    !ti.BaixaTitulo.Any(x => x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA &&
                                                                                x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO)),

                                   dt_inicio_aula = a.Turma.dt_inicio_aula,
                                   dt_final_aula = a.Turma.dt_final_aula
                               }).ToList().Select(x => new AlunoSearchUI
                               {
                                   cd_aluno = x.cd_aluno,
                                   cd_aluno_turma = x.cd_aluno_turma,
                                   no_pessoa = x.no_pessoa,
                                   no_turma = x.no_turma,
                                   cd_contrato = x.cd_contrato,
                                   titulos = x.titulos,
                                   dt_inicio_aula = x.dt_inicio_aula,
                                   dt_final_aula = x.dt_final_aula
                               });
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<AlunoSearchUI> getRptAlunosTurmasNovas(List<int> cdTurmas, DateTime? pDtaI, DateTime? pDtaF, int cd_escola, int tipoTurma)
        {
            try
            {
                var sql = from at in db.AlunoTurma
                          select at;

                if (tipoTurma == (int)FundacaoFisk.SGF.GenericModel.Turma.TipoTurma.NORMAL)
                {
                    sql = from at in sql  
                          where at.Aluno.cd_pessoa_escola == cd_escola &&
                          (at.Turma.dt_inicio_aula >= pDtaI && (pDtaF == null || at.Turma.dt_inicio_aula <= pDtaF)) &&
                          (at.Contrato != null && at.Contrato.pc_desconto_bolsa > 0) &&
                          (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                           at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado) &&
                          (cdTurmas.Contains(at.Turma.cd_turma))
                          select at;
                }
                else
                {
                    sql = from at in sql
                        join tf in db.Turma on at.cd_turma equals tf.cd_turma
                        join tPai in db.Turma on tf.cd_turma_ppt equals tPai.cd_turma
                        join a in db.Aluno on at.cd_aluno equals a.cd_aluno
                        join pa in db.PessoaSGF on a.cd_pessoa_aluno equals pa.cd_pessoa
                        where
                            at.Aluno.cd_pessoa_escola == cd_escola &&
                            tf.dt_termino_turma == null && tf.cd_turma_enc == null &&
                            !tf.DiarioAula.Any(x => x.id_status_aula == (int) DiarioAula.StatusDiarioAula.Efetivada) &&
                            (at.Contrato != null && at.Contrato.pc_desconto_bolsa > 0) &&
                            (at.cd_situacao_aluno_turma == (int) AlunoTurma.SituacaoAlunoTurma.Ativo ||
                             at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado) &&
                            (at.Turma.dt_inicio_aula >= pDtaI && (pDtaF == null || at.Turma.dt_inicio_aula <= pDtaF)) &&
                            (tf.cd_turma_ppt != null && cdTurmas.Contains((int)tf.cd_turma_ppt) )
                        select at;
                }

               

                IEnumerable<AlunoSearchUI> retorno = (from at in sql                                       
                                                     select new
                                                     {
                                                         cd_aluno = at.cd_aluno,
                                                         cd_contrato = at.Contrato.nm_contrato,
                                                         no_turma = at.Turma.no_turma,
                                                         no_aluno = at.Aluno.AlunoPessoaFisica.no_pessoa,
                                                         pc_bolsa = at.Aluno.Bolsa.pc_bolsa,
                                                         pc_bolsa_material = at.Aluno.Bolsa.pc_bolsa_material,
                                                         dc_motivo_bolsa = at.Aluno.Bolsa.MotivoBolsa.dc_motivo_bolsa,

                                                         dt_inicial_contrato = at.Contrato.dt_inicial_contrato,
                                                         dt_final_contrato = at.Contrato.dt_final_contrato 
                                                     }).ToList().Select(x => new AlunoSearchUI
                                                     {
                                                         cd_aluno = x.cd_aluno,
                                                         cd_contrato = x.cd_contrato,
                                                         no_turma = x.no_turma,
                                                         no_aluno = x.no_aluno,
                                                         pc_bolsa = x.pc_bolsa,
                                                         pc_bolsa_material = x.pc_bolsa_material,
                                                         dc_motivo_bolsa = x.dc_motivo_bolsa,

                                                         dt_inicio_aula = (DateTime)x.dt_inicial_contrato,
                                                         dt_final_aula = x.dt_final_contrato
                                                     });

                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        #endregion

        public IEnumerable<sp_RptCartaQuitacao_Result> findAlunoCartaQuitacao(SearchParameters parametros, int cdEscola, int ano, int cdPessoa)
        {
            try
            {
                IEntitySorter<sp_RptCartaQuitacao_Result> sorter = EntitySorter<sp_RptCartaQuitacao_Result>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                DateTime dtInicial = new DateTime(ano, 01, 01).Date;
                DateTime dtFinal = new DateTime(ano, 12, 31).Date;
                IQueryable<sp_RptCartaQuitacao_Result> sql;

                List<sp_RptCartaQuitacao_Result> retorno = new List<sp_RptCartaQuitacao_Result>();

                ObjectResult<sp_RptCartaQuitacao_Result> result = db.sp_RptCartaQuitacao(cdEscola, (short)ano, cdPessoa);
                retorno = result.ToList();
                sql = sorter.Sort(retorno.AsQueryable());

                int limite = sql.Count();

                parametros.ajustaParametrosPesquisa(limite);
                parametros.qtd_limite = limite;
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<AlunosemAulaUI> getAlunosemAula(SearchParameters parametros, int cd_aluno, int cd_item, int cdEscola, DateTime? dtaInicial, DateTime? dtaFinal, bool idMovimento, bool idHistorico, byte idSituacao)
        {
            IEntitySorter<AlunosemAulaUI> sorter = EntitySorter<AlunosemAulaUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
            IQueryable<AlunosemAulaUI> sql;
            //IEnumerable<AlunosemAulaUI> retorno;
            List<AlunosemAulaUI> retorno = new List<AlunosemAulaUI>();
            try
            {
                sql = from a in db.vi_aluno_sem_aula.AsNoTracking()
                          select new AlunosemAulaUI
                          {
                              cd_aluno = (int)a.cd_aluno,
                              cd_item = a.cd_item,
                              dc_reduzido_pessoa = a.dc_reduzido_pessoa,
                              no_pessoa = a.no_pessoa,
                              cd_pessoa_empresa = a.cd_pessoa_empresa,
                              cd_movimento = a.cd_movimento,
                              nm_movimento = a.nm_movimento,
                              dt_emissao_movimento = a.dt_emissao_movimento,
                              no_item = a.no_item,
                              cd_item_movimento = a.cd_item_movimento,
                              qt_item_movimento = a.qt_item_movimento,
                              vl_unitario_item = a.vl_unitario_item,
                              no_turma = a.no_turma,
                              Movimentacao = a.Movimentacao,
                              dt_historico = a.dt_historico,
                              id_situacao_historico = a.id_situacao_historico
                          };
                if (cdEscola != 0)
                    sql = from a in sql
                           where a.cd_pessoa_empresa == cdEscola
                           select a;
                if (cd_aluno != 0)
                    sql = from a in sql
                           where a.cd_aluno == cd_aluno
                           select a;
                if (cd_item != 0)
                    sql = from a in sql
                           where a.cd_item == cd_item
                           select a;
                if (idSituacao != 0)
                    sql = from a in sql
                           where a.id_situacao_historico == idSituacao
                           select a;
                if (dtaInicial != null) {
                    if (idMovimento)
                        sql = from a in sql
                              where a.dt_emissao_movimento >= dtaInicial
                              select a;
                    if (idHistorico)
                        sql = from a in sql
                              where a.dt_historico >= dtaInicial
                              select a;
                }

                if (dtaFinal != null)
                {
                    if (idMovimento)
                        sql = from a in sql
                              where a.dt_emissao_movimento <= dtaFinal
                              select a;
                    if (idHistorico)
                        sql = from a in sql
                              where a.dt_historico <= dtaFinal
                              select a;
                }
                sql = sorter.Sort(sql);
                //retorno = (from a in sql
                //           select new AlunosemAulaUI
                //           {
                //               cd_aluno = (int)a.cd_aluno,
                //               cd_item = a.cd_item,
                //               dc_reduzido_pessoa = a.dc_reduzido_pessoa,
                //               no_pessoa = a.no_pessoa,
                //               cd_pessoa_empresa = a.cd_pessoa_empresa,
                //               cd_movimento = a.cd_movimento,
                //               nm_movimento = a.nm_movimento,
                //               dt_emissao_movimento = a.dt_emissao_movimento,
                //               no_item = a.no_item,
                //               cd_item_movimento = a.cd_item_movimento,
                //               qt_item_movimento = a.qt_item_movimento,
                //               vl_unitario_item = a.vl_unitario_item,
                //               no_turma = a.no_turma,
                //               Movimentacao = a.Movimentacao,
                //               dt_historico = a.dt_historico,
                //               id_situacao_historico = a.id_situacao_historico
                //           }).ToList();


                int limite = sql.Count();

                parametros.ajustaParametrosPesquisa(limite);
                parametros.qtd_limite = limite;
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);

                return sql;

            }
            catch (Exception exe)
            {

                throw new DataAccessException(exe);
            }
        }
        public IEnumerable<AlunosemAulaUI> getNotasDevolvidas(SearchParameters parametros, int cd_aluno, int cd_item, int cdEscola, DateTime? dtaInicial, DateTime? dtaFinal, bool idMovimento, bool idHistorico, byte idSituacao)
        {
            IEntitySorter<AlunosemAulaUI> sorter = EntitySorter<AlunosemAulaUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
            IQueryable<AlunosemAulaUI> sql;
            //IEnumerable<AlunosemAulaUI> retorno;
            List<AlunosemAulaUI> retorno = new List<AlunosemAulaUI>();
            try
            {
                sql = from a in db.vi_livro_devolvido.AsNoTracking()
                      select new AlunosemAulaUI
                      {
                          cd_aluno = (int)a.cd_aluno,
                          cd_item = a.cd_item,
                          dc_reduzido_pessoa = a.dc_reduzido_pessoa,
                          no_pessoa = a.no_pessoa,
                          cd_pessoa_empresa = a.cd_pessoa_empresa,
                          cd_movimento = a.cd_movimento,
                          nm_movimento = a.nm_movimento,
                          dt_emissao_movimento = a.dt_kardex,
                          no_item = a.no_item,
                          cd_item_movimento = a.cd_item_movimento,
                          dt_historico = a.dt_historico,
                          nm_raf = a.nm_raf
                      };
                if (cdEscola != 0)
                    sql = from a in sql
                          where a.cd_pessoa_empresa == cdEscola
                          select a;
                if (cd_aluno != 0)
                    sql = from a in sql
                          where a.cd_aluno == cd_aluno
                          select a;
                if (cd_item != 0)
                    sql = from a in sql
                          where a.cd_item == cd_item
                          select a;
                if (dtaInicial != null)
                {
                    if (idMovimento)
                        sql = from a in sql
                              where a.dt_emissao_movimento >= dtaInicial
                              select a;
                    if (idHistorico)
                        sql = from a in sql
                              where a.dt_historico >= dtaInicial
                              select a;
                }

                if (dtaFinal != null)
                {
                    if (idMovimento)
                        sql = from a in sql
                              where a.dt_emissao_movimento <= dtaFinal
                              select a;
                    if (idHistorico)
                        sql = from a in sql
                              where a.dt_historico <= dtaFinal
                              select a;
                }
                sql = sorter.Sort(sql);

                int limite = sql.Count();

                parametros.ajustaParametrosPesquisa(limite);
                parametros.qtd_limite = limite;
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);

                return sql;

            }
            catch (Exception exe)
            {

                throw new DataAccessException(exe);
            }
        }

        public string postGerarKardexEntrada(Nullable<int> cd_item_movimento, Nullable<int> cd_usuario, Nullable<int> fuso)
        {
            try
            {

                db.Database.Connection.Open();
                var command = db.Database.Connection.CreateCommand();
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = @"sp_gerar_kardex_sem_aula";
                command.CommandTimeout = 180;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>();

                if (cd_item_movimento != null)
                    sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@cd_item_movimento", cd_item_movimento));
                else
                    sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@cd_item_movimento", DBNull.Value));

                if (cd_usuario != null)
                    sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@cd_usuario", cd_usuario));
                else
                    sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@cd_usuario", DBNull.Value));

                if (fuso != null)
                    sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@fuso", fuso));
                else
                    sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@fuso", DBNull.Value));

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

        public IEnumerable<AlunosCargaHorariaUI> getAlunosCargaHoraria(SearchParameters parametros, int cd_aluno, int cd_turma, int cdEscola, DateTime? dtaInicial, DateTime? dtaFinal)
        {
            IEntitySorter<AlunosCargaHorariaUI> sorter = EntitySorter<AlunosCargaHorariaUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
            IQueryable<AlunosCargaHorariaUI> sql;
            try
            {
                sql = from a in db.vi_desistencia_carga.AsNoTracking()
                      select new AlunosCargaHorariaUI
                      {
                          cd_aluno = (int)a.cd_aluno,
                          cd_turma = a.cd_turma,
                          dc_reduzido_pessoa = a.dc_reduzido_pessoa,
                          no_aluno = a.no_aluno,
                          cd_pessoa_escola = a.cd_pessoa_escola,
                          cd_desistencia = a.cd_desistencia,
                          dt_desistencia = a.dt_desistencia,
                          cd_contrato = a.cd_contrato,
                          cd_pessoa_aluno = a.cd_pessoa_aluno,
                          no_turma = a.no_turma,
                          cd_curso = a.cd_curso,
                          no_curso = a.no_curso,
                          tx_obs_desistencia = a.tx_obs_desistencia,
                          nm_raf = a.nm_raf
                      };
                if (cdEscola != 0)
                    sql = from a in sql
                          where a.cd_pessoa_escola == cdEscola
                          select a;
                if (cd_aluno != 0)
                    sql = from a in sql
                          where a.cd_aluno == cd_aluno
                          select a;
                if (cd_turma != 0)
                    sql = from a in sql
                          where a.cd_turma == cd_turma
                          select a;
                if (dtaInicial != null)
                {
                        sql = from a in sql
                              where a.dt_desistencia >= dtaInicial
                              select a;
                }

                if (dtaFinal != null)
                {
                        sql = from a in sql
                              where a.dt_desistencia <= dtaFinal
                              select a;
                }
                sql = sorter.Sort(sql);


                int limite = sql.Count();

                parametros.ajustaParametrosPesquisa(limite);
                parametros.qtd_limite = limite;
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);

                return sql;

            }
            catch (Exception exe)
            {

                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<CargaHorariaUI> getCargaHoraria(SearchParameters parametros, int cd_aluno, int cd_turma, int cd_curso, int cdEscola, int cd_professor, bool totasEscolas, int nm_aulas_vencimento)
        {
            IEntitySorter<CargaHorariaUI> sorter = EntitySorter<CargaHorariaUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
            IQueryable<CargaHorariaUI> sql;
            try
            {
                sql = from a in db.vi_carga_horaria.AsNoTracking()
                      from p in db.PessoaSGF
                      where p.cd_pessoa == a.cd_escola
                      select new CargaHorariaUI
                      {
                          cd_pessoa_escola = a.cd_escola,
                          no_escola = p.dc_reduzido_pessoa,
                          nm_raf = a.nm_raf,
                          cd_aluno = a.cd_aluno,
                          no_aluno = a.no_aluno,
                          no_curso = a.no_curso,
                          cd_curso = a.cd_curso,
                          cd_turma = a.cd_turma,
                          no_turma = a.no_turma,
                          nm_carga = a.nm_carga,
                          nm_carga_maxima = a.nm_carga_maxima,
                          qt_voucher = a.qt_voucher,
                          dt_ultima_aula = a.dt_ultima_aula,
                          cd_turma_ppt = a.cd_turma_ppt
                          
                          
                      };
                if (!totasEscolas)
                    sql = from a in sql
                          where a.cd_pessoa_escola == cdEscola
                          select a;
                if (cd_aluno != 0)
                    sql = from a in sql
                          where a.cd_aluno == cd_aluno
                          select a;
                if (cd_turma != 0)
                    sql = from a in sql
                          where a.cd_turma == cd_turma || a.cd_turma_ppt == cd_turma
                          select a;
                if (cd_curso != 0)
                    sql = from a in sql
                          where a.cd_curso == cd_curso
                          select a;
                if (cd_professor != 0)
                    sql = from a in sql
                          where db.DiarioAula.Where(d => d.cd_turma == a.cd_turma && d.cd_professor == cd_professor).Any()
                          select a;

                if (nm_aulas_vencimento > 0)
                {
                    sql = from a in sql
                          where (a.nm_carga_maxima + a.qt_voucher - a.nm_carga) >= 0 &&
                                (a.nm_carga_maxima + a.qt_voucher - a.nm_carga) <= nm_aulas_vencimento
                          select a;
                }


                sql = sorter.Sort(sql);


                int limite = sql.Count();

                parametros.ajustaParametrosPesquisa(limite);
                parametros.qtd_limite = limite;
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);

                return sql;

            }
            catch (Exception exe)
            {

                throw new DataAccessException(exe);
            }
        }

        public string postGerarNotaVoucher(int cd_desistencia, int cd_usuario, int fusoHorario, int itemVoucher)
        {
            try
            {

                db.Database.Connection.Open();
                var command = db.Database.Connection.CreateCommand();
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = @"sp_gerar_nota_servico_voucher";
                command.CommandTimeout = 180;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>();

                sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@cd_desistencia", cd_desistencia));

                sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@cd_usuario", cd_usuario));

                sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@fuso", fusoHorario));

                sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@itemVoucher", itemVoucher));

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

        public bool getParametroEscolaInternacional(int cdEscola)
        {
            try
            {
                bool retorno = false;
                retorno = (from e in db.PessoaSGF.OfType<Escola>()
                          where e.cd_pessoa == cdEscola
                          select e.id_empresa_internacional).FirstOrDefault();
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Aluno findAlunoByCdPessoaAlunoAndCdEscolaDestino(int alunoBdCdPessoaAluno, int cdEscolaDestino)
        {
            try
            {
                
               var retorno = (from a in db.Aluno
                           where a.cd_pessoa_aluno == alunoBdCdPessoaAluno &&
                                 a.cd_pessoa_escola == cdEscolaDestino
                           select a).FirstOrDefault();
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        
        }

        public ProfessorCargaHorariaMaximaResultUI getExisteCargaHorariaProximaMaxima(int cdPessoaUsuario, int cdEscola)
        {
            try
            {
                
                FuncionarioSGF funcionario = (from f in db.FuncionarioSGF
                                              where f.id_professor == true && 
                                                    f.cd_pessoa_funcionario == cdPessoaUsuario &&
                                                    f.cd_pessoa_empresa == cdEscola 
                               select f).FirstOrDefault();
                bool hasAlunoCargaMaxima = false;

                if (funcionario != null)
                {
                    hasAlunoCargaMaxima = (from a in db.vi_carga_horaria
                                           where a.cd_escola == cdEscola &&
                                                 db.DiarioAula.Where(d => d.cd_turma == a.cd_turma && d.cd_professor == funcionario.cd_funcionario).Any() &&
                                                 a.dt_termino_turma == null &&
                                                 (a.nm_carga_maxima + a.qt_voucher - a.nm_carga) >= 0 &&
                                                 (a.nm_carga_maxima + a.qt_voucher - a.nm_carga) <= 5
                                           select a).Any();

                }

                ProfessorCargaHorariaMaximaResultUI professorCarga = new ProfessorCargaHorariaMaximaResultUI();
                professorCarga.cd_professor = funcionario != null ? funcionario.cd_funcionario : 0;
                professorCarga.existe_alunos_carga_maxima = hasAlunoCargaMaxima;


                return professorCarga;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public IList<T> ConvertTo<T>(DataTable table)
        {
            if (table == null)
            {
                return null;
            }

            List<DataRow> rows = new List<DataRow>();

            foreach (DataRow row in table.Rows)
            {
                rows.Add(row);
            }

            return ConvertToData<T>(rows);
        }
        public IList<T> ConvertToData<T>(IList<DataRow> rows)
        {
            IList<T> list = null;

            if (rows != null)
            {
                list = new List<T>();

                foreach (DataRow row in rows)
                {
                    T item = CreateItem<T>(row);
                    list.Add(item);
                }
            }

            return list;
        }

        public T CreateItem<T>(DataRow row)
        {
            T obj = default(T);
            if (row != null)
            {
                obj = Activator.CreateInstance<T>();

                foreach (DataColumn column in row.Table.Columns)
                {
                    PropertyInfo prop = obj.GetType().GetProperty(column.ColumnName);
                    try
                    {
                        object value = row[column.ColumnName];
                        if (value != DBNull.Value)
                        {
                            //object value = row[column.ColumnName];
                            prop.SetValue(obj, value, null);
                        }
                    }
                    catch
                    {
                        // You can log something here
                        throw;
                    }
                }
            }

            return obj;
        }

    }
}

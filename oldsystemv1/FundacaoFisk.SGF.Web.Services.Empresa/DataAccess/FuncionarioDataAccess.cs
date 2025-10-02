using System;
using System.Collections.Generic;
using System.Linq;
using Componentes.GenericDataAccess.GenericRepository;
using System.Data.Entity;
using Componentes.Utils;
using Componentes.GenericDataAccess;
using System.Data;
using Componentes.GenericDataAccess.GenericException;
using Componentes.Utils.Messages;
using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IDataAccess;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.GenericModel.Partial;

namespace FundacaoFisk.SGF.Web.Services.Empresa.DataAccess {
    using FundacaoFisk.SGF.Web.Services.Empresa.Model;

    public class FuncionarioDataAccess : GenericRepository<FuncionarioSGF>, IFuncionarioDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<FuncionarioSearchUI> getSearchFuncionario(SearchParameters parametros, string nome, string apelido, bool? status, string cpf, bool inicio, byte tipo, int cdEscola, int sexo, int cdAtividade)
        {
            try
            {
                IEntitySorter<FuncionarioSearchUI> sorter = EntitySorter<FuncionarioSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<FuncionarioSearchUI> sql;

                //sql = from p in db.Pessoa select p;
                if (sexo > 0)
                {
                    sql = from c in db.FuncionarioSGF.AsNoTracking()
                          where c.cd_pessoa_empresa == cdEscola &&
                                c.FuncionarioPessoaFisica.nm_sexo == sexo
                          select new FuncionarioSearchUI
                          {
                              cd_funcionario = c.cd_funcionario,
                              cd_pessoa_funcionario = c.FuncionarioPessoaFisica.cd_pessoa,
                              no_pessoa = c.FuncionarioPessoaFisica.no_pessoa,
                              dc_num_pessoa = c.FuncionarioPessoaFisica.dc_num_pessoa,
                              nm_natureza_pessoa = c.FuncionarioPessoaFisica.nm_natureza_pessoa,
                              dc_reduzido_pessoa = c.FuncionarioPessoaFisica.dc_reduzido_pessoa,
                              dt_cadastramento = c.FuncionarioPessoaFisica.dt_cadastramento,
                              id_funcionario_ativo = c.id_funcionario_ativo,
                              nm_cpf_cgc = c.FuncionarioPessoaFisica.cd_pessoa_cpf > 0 ? c.FuncionarioPessoaFisica.PessoaSGFQueUsoOCpf.nm_cpf : c.FuncionarioPessoaFisica.nm_cpf,
                              no_pessoa_dependente = c.FuncionarioPessoaFisica.PessoaSGFQueUsoOCpf.no_pessoa,
                              ext_img_pessoa = c.FuncionarioPessoaFisica.ext_img_pessoa,
                              //no_atividade = c.PessoaFisica.AtividadePessoa.no_atividade,
                              //cd_atividade = c.PessoaFisica.cd_atividade_principal,
                              id_professor = c.id_professor,
                              nm_atividade_principal = c.FuncionarioPessoaFisica.cd_atividade_principal,
                              nm_endereco_principal = c.FuncionarioPessoaFisica.cd_endereco_principal,
                              nm_telefone_principal = c.FuncionarioPessoaFisica.cd_telefone_principal,
                              cd_cargo = c.cd_cargo,
                              des_cargo = c.FuncionarioAtividade.no_atividade
                          };
                }
                else
                {

                    sql = from c in db.FuncionarioSGF.AsNoTracking()
                          where c.cd_pessoa_empresa == cdEscola
                          select new FuncionarioSearchUI
                          {
                              cd_funcionario = c.cd_funcionario,
                              cd_pessoa_funcionario = c.FuncionarioPessoaFisica.cd_pessoa,
                              no_pessoa = c.FuncionarioPessoaFisica.no_pessoa,
                              dc_num_pessoa = c.FuncionarioPessoaFisica.dc_num_pessoa,
                              nm_natureza_pessoa = c.FuncionarioPessoaFisica.nm_natureza_pessoa,
                              dc_reduzido_pessoa = c.FuncionarioPessoaFisica.dc_reduzido_pessoa,
                              dt_cadastramento = c.FuncionarioPessoaFisica.dt_cadastramento,
                              id_funcionario_ativo = c.id_funcionario_ativo,
                              nm_cpf_cgc = c.FuncionarioPessoaFisica.cd_pessoa_cpf > 0 ? c.FuncionarioPessoaFisica.PessoaSGFQueUsoOCpf.nm_cpf : c.FuncionarioPessoaFisica.nm_cpf,
                              no_pessoa_dependente = c.FuncionarioPessoaFisica.PessoaSGFQueUsoOCpf.no_pessoa,
                              ext_img_pessoa = c.FuncionarioPessoaFisica.ext_img_pessoa,
                              //no_atividade = c.PessoaFisica.AtividadePessoa.no_atividade,
                              //cd_atividade = c.PessoaFisica.cd_atividade_principal,
                              id_professor = c.id_professor,
                              nm_atividade_principal = c.FuncionarioPessoaFisica.cd_atividade_principal,
                              nm_endereco_principal = c.FuncionarioPessoaFisica.cd_endereco_principal,
                              nm_telefone_principal = c.FuncionarioPessoaFisica.cd_telefone_principal,
                              cd_cargo =c.cd_cargo,
                              des_cargo = c.FuncionarioAtividade.no_atividade
                          };

                }
                sql = sorter.Sort(sql);

                if (!string.IsNullOrEmpty(nome))
                {
                    if (inicio)
                    {
                        sql = from func in sql
                              where func.no_pessoa.StartsWith(nome)
                              select func;
                    }//end if
                    else
                    {
                        sql = from c in sql
                              where c.no_pessoa.Contains(nome)
                              select c;
                    }//end else


                }
                if (!string.IsNullOrEmpty(cpf))
                {
                    sql = from c in sql
                          where c.nm_cpf_cgc == cpf
                          select c;
                }

                if (!string.IsNullOrEmpty(apelido))
                {
                    if (inicio)
                    {
                        sql = from c in sql
                              where c.dc_reduzido_pessoa.StartsWith(apelido)
                              select c;
                    }
                    else
                    {
                        sql = from c in sql
                              where c.dc_reduzido_pessoa.Contains(apelido)
                              select c;
                    }
                }

                if (status != null)
                    sql = from c in sql
                          where c.id_funcionario_ativo == status
                          select c;
                if (tipo > (byte)FuncionarioSGF.TipoFuncionario.TODOS)
                {
                    if (tipo == (byte)FuncionarioSGF.TipoFuncionario.FUNCIONARIO)
                        sql = from c in sql
                              where !c.id_professor
                              select c;
                    else
                        sql = from c in sql
                              where c.id_professor
                              select c;
                }

                if(cdAtividade > 0)
                    sql = from c in sql
                          where c.cd_cargo == cdAtividade
                          select c;

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

        public IEnumerable<FuncionarioSearchUI> getSearchFuncionarioComAtividadeExtra(SearchParameters parametros, string nome, string apelido, bool? status, string cpf, bool inicio, byte tipo, int cdEscola, int sexo, int cdAtividade)
        {
            try
            {
                IEntitySorter<FuncionarioSearchUI> sorter = EntitySorter<FuncionarioSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<FuncionarioSearchUI> sql;

                //sql = from p in db.Pessoa select p;
                if (sexo > 0)
                {
                    sql = from c in db.FuncionarioSGF.AsNoTracking().Distinct()
                          join p in db.PessoaSGF.OfType<PessoaFisicaSGF>() on c.cd_pessoa_funcionario equals p.cd_pessoa
                          join at in db.AtividadeExtra on c.cd_funcionario equals at.cd_funcionario
                          where (c.cd_pessoa_empresa == cdEscola) && 
                                (c.FuncionarioPessoaFisica.nm_sexo == sexo) 
                          //orderby c.FuncionarioPessoaFisica.no_pessoa
                          select new FuncionarioSearchUI
                          {
                              cd_funcionario = c.cd_funcionario,
                              cd_pessoa_funcionario = c.FuncionarioPessoaFisica.cd_pessoa,
                              no_pessoa = c.FuncionarioPessoaFisica.no_pessoa,
                              dc_num_pessoa = c.FuncionarioPessoaFisica.dc_num_pessoa,
                              nm_natureza_pessoa = c.FuncionarioPessoaFisica.nm_natureza_pessoa,
                              dc_reduzido_pessoa = c.FuncionarioPessoaFisica.dc_reduzido_pessoa,
                              dt_cadastramento = c.FuncionarioPessoaFisica.dt_cadastramento,
                              id_funcionario_ativo = c.id_funcionario_ativo,
                              nm_cpf_cgc = c.FuncionarioPessoaFisica.cd_pessoa_cpf > 0 ? c.FuncionarioPessoaFisica.PessoaSGFQueUsoOCpf.nm_cpf : c.FuncionarioPessoaFisica.nm_cpf,
                              no_pessoa_dependente = c.FuncionarioPessoaFisica.PessoaSGFQueUsoOCpf.no_pessoa,
                              ext_img_pessoa = c.FuncionarioPessoaFisica.ext_img_pessoa,
                              //no_atividade = c.PessoaFisica.AtividadePessoa.no_atividade,
                              //cd_atividade = c.PessoaFisica.cd_atividade_principal,
                              id_professor = c.id_professor,
                              nm_atividade_principal = c.FuncionarioPessoaFisica.cd_atividade_principal,
                              nm_endereco_principal = c.FuncionarioPessoaFisica.cd_endereco_principal,
                              nm_telefone_principal = c.FuncionarioPessoaFisica.cd_telefone_principal,
                              cd_cargo = c.cd_cargo,
                              des_cargo = c.FuncionarioAtividade.no_atividade
                          };
                }
                else
                {

                    sql = from professor in db.FuncionarioSGF
                        join pessoa in db.PessoaSGF.OfType<PessoaFisicaSGF>() on professor.cd_pessoa_funcionario equals pessoa.cd_pessoa
                        join atividadeExtra in db.AtividadeExtra on professor.cd_funcionario equals atividadeExtra.cd_funcionario
                        where (professor.cd_pessoa_empresa == cdEscola)
                             
                          //orderby c.FuncionarioPessoaFisica.no_pessoa
                          select new FuncionarioSearchUI
                          {
                              cd_funcionario = professor.cd_funcionario,
                              cd_pessoa_funcionario = professor.FuncionarioPessoaFisica.cd_pessoa,
                              no_pessoa = professor.FuncionarioPessoaFisica.no_pessoa,
                              dc_num_pessoa = professor.FuncionarioPessoaFisica.dc_num_pessoa,
                              nm_natureza_pessoa = professor.FuncionarioPessoaFisica.nm_natureza_pessoa,
                              dc_reduzido_pessoa = professor.FuncionarioPessoaFisica.dc_reduzido_pessoa,
                              dt_cadastramento = professor.FuncionarioPessoaFisica.dt_cadastramento,
                              id_funcionario_ativo = professor.id_funcionario_ativo,
                              nm_cpf_cgc = professor.FuncionarioPessoaFisica.cd_pessoa_cpf > 0 ? professor.FuncionarioPessoaFisica.PessoaSGFQueUsoOCpf.nm_cpf : professor.FuncionarioPessoaFisica.nm_cpf,
                              no_pessoa_dependente = professor.FuncionarioPessoaFisica.PessoaSGFQueUsoOCpf.no_pessoa,
                              ext_img_pessoa = professor.FuncionarioPessoaFisica.ext_img_pessoa,
                              //no_atividade = professor.PessoaFisica.AtividadePessoa.no_atividade,
                              //cd_atividade = professor.PessoaFisica.cd_atividade_principal,
                              id_professor = professor.id_professor,
                              nm_atividade_principal = professor.FuncionarioPessoaFisica.cd_atividade_principal,
                              nm_endereco_principal = professor.FuncionarioPessoaFisica.cd_endereco_principal,
                              nm_telefone_principal = professor.FuncionarioPessoaFisica.cd_telefone_principal,
                              cd_cargo = professor.cd_cargo,
                              des_cargo = professor.FuncionarioAtividade.no_atividade
                          };

                }
                sql = sorter.Sort(sql);

                if (!string.IsNullOrEmpty(nome))
                {
                    if (inicio)
                    {
                        sql = from func in sql
                              where func.no_pessoa.StartsWith(nome)
                              select func;
                    }//end if
                    else
                    {
                        sql = from c in sql
                              where c.no_pessoa.Contains(nome)
                              select c;
                    }//end else


                }
                if (!string.IsNullOrEmpty(cpf))
                {
                    sql = from c in sql
                          where c.nm_cpf_cgc == cpf
                          select c;
                }

                if (!string.IsNullOrEmpty(apelido))
                {
                    if (inicio)
                    {
                        sql = from c in sql
                              where c.dc_reduzido_pessoa.StartsWith(apelido)
                              select c;
                    }
                    else
                    {
                        sql = from c in sql
                              where c.dc_reduzido_pessoa.Contains(apelido)
                              select c;
                    }
                }

                if (status != null)
                    sql = from c in sql
                          where c.id_funcionario_ativo == status
                          select c;
                if (tipo > (byte)FuncionarioSGF.TipoFuncionario.TODOS)
                {
                    if (tipo == (byte)FuncionarioSGF.TipoFuncionario.FUNCIONARIO)
                        sql = from c in sql
                              where !c.id_professor
                              select c;
                    else
                        sql = from c in sql
                              where c.id_professor
                              select c;
                }

                if (cdAtividade > 0)
                    sql = from c in sql
                          where c.cd_cargo == cdAtividade
                          select c;

                int limite = sql.Count();

                parametros.ajustaParametrosPesquisa(limite);
                parametros.qtd_limite = limite;
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);

                return sql.Distinct().OrderBy(p => p.no_pessoa); ;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public FuncionarioSGF getFuncionarioByCpf(string cpf, int cdEscola)
        {
            try
            {
                var sql = (from func in db.FuncionarioSGF
                           //join pessoa in db.PessoaSGF.OfType<PessoaFisicaSGF>() on func.cd_pessoa_funcionario equals pessoa.cd_pessoa
                           where func.FuncionarioPessoaFisica.nm_cpf == cpf && func.cd_pessoa_empresa == cdEscola
                           select new { func.cd_funcionario, func.FuncionarioPessoaFisica.no_pessoa }).ToList()
                           .Select(x => new FuncionarioSGF { cd_funcionario = x.cd_funcionario, FuncionarioPessoaFisica = new PessoaFisicaSGF { no_pessoa = x.no_pessoa } }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool addEmpresaPessoa(PessoaEscola pessoaEmpresa)
        {
            try
            {
                int ret = 0;
                var exist = from pe in db.PessoaEscola
                            where
                              pe.cd_pessoa == pessoaEmpresa.cd_pessoa &&
                              pe.cd_escola == pessoaEmpresa.cd_escola
                            select 1;
                if (exist.Count() <= 0)
                {
                    db.PessoaEscola.Add(pessoaEmpresa);
                    ret = db.SaveChanges();
                }

                return ret > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public FuncionarioSearchUI getFuncionarioSearchUIById(int cd_funcionario, int cd_empresa)
        {
            try
            {
                var sql = (from func in db.FuncionarioSGF
                           where func.cd_funcionario == cd_funcionario && func.cd_pessoa_empresa == cd_empresa
                           select new FuncionarioSearchUI {
                               cd_funcionario = func.cd_funcionario,
                               cd_pessoa_funcionario = func.FuncionarioPessoaFisica.cd_pessoa,
                               no_pessoa = func.FuncionarioPessoaFisica.no_pessoa,
                               dt_cadastramento = func.FuncionarioPessoaFisica.dt_cadastramento,
                               dc_reduzido_pessoa = func.FuncionarioPessoaFisica.dc_reduzido_pessoa,
                               nm_atividade_principal = func.FuncionarioPessoaFisica.cd_atividade_principal,
                               nm_endereco_principal = func.FuncionarioPessoaFisica.cd_endereco_principal,
                               nm_telefone_principal = func.FuncionarioPessoaFisica.cd_telefone_principal,
                               nm_natureza_pessoa = func.FuncionarioPessoaFisica.nm_natureza_pessoa,
                               dc_num_pessoa = func.FuncionarioPessoaFisica.dc_num_pessoa,
                               nm_cpf_cgc = func.FuncionarioPessoaFisica.cd_pessoa_cpf > 0 ? func.FuncionarioPessoaFisica.PessoaSGFQueUsoOCpf.nm_cpf : func.FuncionarioPessoaFisica.nm_cpf,
                               no_pessoa_dependente = func.FuncionarioPessoaFisica.PessoaSGFQueUsoOCpf.no_pessoa,
                               id_funcionario_ativo = func.id_funcionario_ativo,
                               ext_img_pessoa = func.FuncionarioPessoaFisica.ext_img_pessoa,
                               no_atividade = func.FuncionarioPessoaFisica.Atividade != null ? func.FuncionarioPessoaFisica.Atividade.no_atividade : "",
                               id_professor = func.id_professor,
                               cd_cargo = func.cd_cargo,
                               des_cargo = func.FuncionarioAtividade.no_atividade
                           }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public FuncionarioCyberBdUI findFuncionarioByCdFuncionario(int cd_funcionario, int cd_empresa)
        {
            try
            {
                var sql = (from func in db.FuncionarioSGF
                           where func.cd_funcionario == cd_funcionario && func.cd_pessoa_empresa == cd_empresa
                           select new
                           {
                               codigo = func.cd_funcionario,
                               nome = func.FuncionarioPessoaFisica.no_pessoa,
                               email = ((from t in db.TelefoneSGF where t.cd_pessoa == func.FuncionarioPessoaFisica.cd_pessoa && t.cd_tipo_telefone == 4 && t.id_telefone_principal == true select t).FirstOrDefault() != null ?
                                   (from t in db.TelefoneSGF where t.cd_pessoa == func.FuncionarioPessoaFisica.cd_pessoa && t.cd_tipo_telefone == 4 && t.id_telefone_principal == true select t).FirstOrDefault().dc_fone_mail : ""),
                               id_unidade = (func.Empresa.cd_empresa_coligada == null ? func.Empresa.nm_cliente_integracao : (from e in db.PessoaSGF.OfType<Escola>() where e.cd_pessoa == func.Empresa.cd_empresa_coligada select e.nm_cliente_integracao).FirstOrDefault()),
                               funcionario_ativo = func.id_funcionario_ativo,
                               tipoFuncionario = (!func.id_professor && func.id_colaborador_cyber == false && db.FuncionarioSGF.OfType<Professor>().Any(x => x.cd_funcionario == func.cd_funcionario && x.id_coordenador == false)) ? (byte)FuncionarioSGF.TipoAlunoFuncionario.FUNCIONARIO :
                                   (func.id_professor && db.FuncionarioSGF.OfType<Professor>().Any(x => x.cd_funcionario == func.cd_funcionario && x.id_coordenador == false)) ? (byte)FuncionarioSGF.TipoAlunoFuncionario.PROFESSOR :
                                   (func.id_professor && db.FuncionarioSGF.OfType<Professor>().Any(x => x.cd_funcionario == func.cd_funcionario && x.id_coordenador == true)) ? (byte)FuncionarioSGF.TipoAlunoFuncionario.COORDENADOR :
                                   (!func.id_professor && func.id_colaborador_cyber == true) ? (byte)FuncionarioSGF.TipoAlunoFuncionario.COLABORADOR :
                                   (byte)FuncionarioSGF.TipoAlunoFuncionario.NENHUM

                           }).ToList().Select(x => new FuncionarioCyberBdUI
                           {
                               codigo = x.codigo,
                               nome = x.nome,
                               email = x.email,
                               id_unidade = x.id_unidade,
                               funcionario_ativo = x.funcionario_ativo,
                               tipo_funcionario = x.tipoFuncionario

                           }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public FuncionarioSGF fidFuncionarioById(int cd_func, int cd_empresa)
        {
            try
            {
                var sql = (from func in db.FuncionarioSGF
                           where func.cd_funcionario == cd_func && func.cd_pessoa_empresa == cd_empresa
                           select func).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<FuncionarioSearchUI> getFuncionarios(int cd_pessoa_empresa, int? cd_funcionario, FuncionarioSGF.TipoConsultaFuncionarioEnum tipo)
        {
            try
            {
                IQueryable<FuncionarioSearchUI> sql = null;
                switch (tipo)
                {
                    case FuncionarioSGF.TipoConsultaFuncionarioEnum.HAS_COMISSIONADO:
                        sql = from funcionario in db.FuncionarioSGF
                              join pessoa in db.PessoaSGF
                              on funcionario.cd_pessoa_funcionario equals pessoa.cd_pessoa
                              where funcionario.cd_pessoa_empresa == cd_pessoa_empresa && funcionario.id_funcionario_ativo == true &&
                                    funcionario.id_comissionado == true
                              orderby funcionario.FuncionarioPessoaFisica.no_pessoa
                              select new FuncionarioSearchUI
                              {
                                  cd_pessoa_funcionario = funcionario.cd_funcionario,
                                  no_pessoa = funcionario.FuncionarioPessoaFisica.no_pessoa,
                                  no_fantasia = funcionario.FuncionarioPessoaFisica.dc_reduzido_pessoa
                              };
                        break;
                }
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public int getFuncionarioByIdPessoa(int cd_pessoa)
        {
            try
            {
                var sql = (from func in db.FuncionarioSGF
                            where func.cd_pessoa_funcionario == cd_pessoa
                            select func).FirstOrDefault().cd_funcionario;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
            
        }


    }
}

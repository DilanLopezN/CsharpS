using System;
using System.Collections.Generic;
using System.Linq;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.Web.Services.EmailMarketing.Comum.IDataAccess;
using Componentes.GenericDataAccess.GenericException;
using FundacaoFisk.SGF.Web.Service.EmailMarketing.Model;
using FundacaoFisk.SGF.GenericModel;
using System.Data.Entity;

namespace FundacaoFisk.SGF.Web.Service.EmailMarketing.DataAccess
{
    public class ListaEnderecoMalaDataAccess: GenericRepository<ListaEnderecoMala>, IListaEnderecoMalaDataAccess
    {
        public enum TipoSituacaoAluno
        {
            INATIVO = 20
        }

        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<ListaEnderecoMala> getListagemEnderecosComporMensagemProspect(MalaDireta mala_direta)
        {
            try
            {
                IEnumerable<ListaEnderecoMala> retorno;
                List<int> listaProdutos = new List<int>();
                List<byte> listaPeriodo = new List<byte>();
                var dataAtual = DateTime.Now.Date;

                if (mala_direta.MalasDiretaProduto != null && mala_direta.MalasDiretaProduto.Count > 0)
                    listaProdutos = mala_direta.MalasDiretaProduto.Select(x => x.cd_produto).ToList();
                if (mala_direta.MalasDiretaPeriodo != null && mala_direta.MalasDiretaPeriodo.Count > 0)
                    listaPeriodo = mala_direta.MalasDiretaPeriodo.Select(x => x.id_periodo).ToList();

                var sql = from p in db.PessoaSGF.OfType<PessoaFisicaSGF>()
                          select p;
                
                var retorno1 = from p in sql
                               where p.Prospect.Where(pr => 
                                   pr.cd_pessoa_escola == mala_direta.cd_escola && pr.id_prospect_ativo == true
                                   && !pr.Aluno.Any()
                                   && (listaProdutos.Count <= 0 || pr.ProspectProduto.Where(pp => listaProdutos.Contains(pp.cd_produto)).Any())
                                   && (listaPeriodo.Count <= 0 || pr.ProspectPeriodo.Where(pe => listaPeriodo.Contains(pe.id_periodo)).Any())
                                   ).Any()
                               select p;
                if (mala_direta.id_prospect_inativo)
                {
                    retorno1 = from p in sql
                        where p.Prospect.Where(pr =>
                                pr.cd_pessoa_escola == mala_direta.cd_escola && pr.id_prospect_ativo == false
                                && !pr.Aluno.Any()
                                && (listaProdutos.Count <= 0 || pr.ProspectProduto.Where(pp => listaProdutos.Contains(pp.cd_produto)).Any())
                                && (listaPeriodo.Count <= 0 || pr.ProspectPeriodo.Where(pe => listaPeriodo.Contains(pe.id_periodo)).Any())).Any()
                        select p;
                }
                else
                {
                    retorno1 = from p in sql
                               where p.Prospect.Where(pr =>
                                       pr.cd_pessoa_escola == mala_direta.cd_escola && pr.id_prospect_ativo == true
                                       && !pr.Aluno.Any()
                                       && (listaProdutos.Count <= 0 || pr.ProspectProduto.Where(pp => listaProdutos.Contains(pp.cd_produto)).Any())
                                       && (listaPeriodo.Count <= 0 || pr.ProspectPeriodo.Where(pe => listaPeriodo.Contains(pe.id_periodo)).Any())).Any()
                               select p;
                }
                

                if (!mala_direta.dtFimPeriodo.HasValue)
                    mala_direta.dtFimPeriodo = dataAtual;

                if (mala_direta.dtIniPeriodo.HasValue && mala_direta.dtFimPeriodo.HasValue)
                {
                    retorno1 = from p in retorno1
                               where p.Prospect.Any(x =>
                                   x.PessoaFisica.dt_cadastramento >= mala_direta.dtIniPeriodo.Value
                                   && x.PessoaFisica.dt_cadastramento <= mala_direta.dtFimPeriodo.Value)
                               select p;
                }

                if (mala_direta.id_sexo.HasValue)
                    retorno1 = from p in retorno1
                                where p.nm_sexo == mala_direta.id_sexo
                                select p;

                if (mala_direta.nm_mes_nascimento.HasValue)
                    retorno1 = from p in retorno1
                               where p.dt_nascimento.Value.Month == mala_direta.nm_mes_nascimento.Value
                               select p;

                if (mala_direta.cd_midia > 0)
                    retorno1 = from p in retorno1
                               where p.Prospect.Any(x => x.cd_midia == mala_direta.cd_midia)
                               select p;

                if (mala_direta.cd_faixa_etaria > 0)
                    retorno1 = from p in retorno1
                               where p.Prospect.Any(c => c.id_faixa_etaria == mala_direta.cd_faixa_etaria)
                               select p;

                if (mala_direta.cd_dia_semana.Count > 0)
                    retorno1 = from p in retorno1
                               where p.Prospect.Any(x => x.ProspectDia.Any(y => mala_direta.cd_dia_semana.Contains(y.id_dia_semana)))
                               select p;

                if(!string.IsNullOrEmpty(mala_direta.nome))
                    retorno1 = from p in retorno1
                               where p.Prospect.Any(x => x.PessoaFisica.no_pessoa.Contains(mala_direta.nome))
                               select p;


                if (mala_direta.cd_estado > 0)
                    retorno1 = from p in retorno1
                        where p.Prospect.Any(x => x.PessoaFisica.EnderecoPrincipal.cd_loc_estado == mala_direta.cd_estado)
                        select p;

                if (mala_direta.cd_cidade > 0)
                    retorno1 = from p in retorno1
                        where p.Prospect.Any(x => x.PessoaFisica.EnderecoPrincipal.cd_loc_cidade == mala_direta.cd_cidade)
                        select p;

                if (mala_direta.cd_localidade > 0)
                    retorno1 = from p in retorno1
                               where p.Prospect.Any(x => x.PessoaFisica.EnderecoPrincipal.cd_loc_bairro == mala_direta.cd_localidade)
                               select p;


                retorno = (from p in retorno1
                           select new
                           {
                               p.cd_pessoa,
                               p.no_pessoa,
                               id_cadastro = (byte)MalaDireta.TipoCadastro.PROSPECT,
                               email = p.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL).FirstOrDefault().dc_fone_mail
                           }).ToList().Select(x => new ListaEnderecoMala
                           {
                               cd_cadastro = x.cd_pessoa,
                               no_pessoa = x.no_pessoa,
                               dc_email_cadastro = x.email,
                               id_cadastro = x.id_cadastro
                           });

                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<ListaEnderecoMala> getListagemEnderecosComporMensagemAluno(MalaDireta mala_direta)
        {
            try
            {
                IEnumerable<ListaEnderecoMala> retorno;
                List<int> listaProdutos = new List<int>();
                List<int> listaCurso = new List<int>();
                var dataAtual = DateTime.Now.Date;

                int situacaoAlunoInativo = 0;

                //o tipo inativo pode vir junto na lista de situações do aluno
                //para nao interferir na funcionalidade das situações, (quando o tipo inativo vem na lista) o mesmo é removido a lista(cd_situacao_aluno) e colocado na variavel situacaoAlunoInativo para ser aplicado o filtro
                if (mala_direta.cd_situacao_aluno != null && mala_direta.cd_situacao_aluno.Count > 0 
                        && mala_direta.cd_situacao_aluno.Contains((int) TipoSituacaoAluno.INATIVO))
                {
                    situacaoAlunoInativo = (int) TipoSituacaoAluno.INATIVO;
                    mala_direta.cd_situacao_aluno.Remove((int) TipoSituacaoAluno.INATIVO);
                }

                if (mala_direta.MalasDiretaProduto != null && mala_direta.MalasDiretaProduto.Count > 0)
                    listaProdutos = mala_direta.MalasDiretaProduto.Select(x => x.cd_produto).ToList();
                if (mala_direta.MalasDiretaProduto != null && mala_direta.MalasDiretaProduto.Count > 0)
                    listaCurso = mala_direta.MalasDiretaCurso.Select(x => x.cd_curso).ToList();

                //var sql = from p in db.Aluno
                //          select p;

                var retorno1 = from a in db.Aluno
                               where  a.cd_pessoa_escola == mala_direta.cd_escola 
                                   && (situacaoAlunoInativo == (int)TipoSituacaoAluno.INATIVO ? a.id_aluno_ativo == false : a.id_aluno_ativo == true)
                                   && a.Contratos.Any()
                                   && a.Contratos.Where(co => co.Aluno.cd_pessoa_aluno == a.cd_pessoa_aluno && (listaProdutos.Count <= 0 || listaProdutos.Contains(co.cd_produto_atual))).Any()
                                   && a.Contratos.Where(co => co.Aluno.cd_pessoa_aluno == a.cd_pessoa_aluno && (listaCurso.Count <= 0 || listaCurso.Contains(co.cd_curso_atual))).Any()
                                   && db.AlunoTurma.Where(al => al.cd_aluno == a.cd_aluno && al.cd_situacao_aluno_turma != null && (mala_direta.cd_situacao_aluno.Count == 0 || mala_direta.cd_situacao_aluno.Contains((int)al.cd_situacao_aluno_turma))).Any()
                                   && (!mala_direta.hh_inicial.HasValue 
                                        || (from ht in db.Horario
                                             where a.Contratos.Where(c => c.AlunoTurma.Where(at =>
                                                 ht.cd_registro == at.Turma.cd_turma
                                                 && ht.id_origem == (int)Horario.Origem.TURMA
                                                 && !at.Turma.id_turma_ppt
                                                     // A hora inicial e final devem estar contidas no horário da turma:
                                                 && (
                                                        ht.dt_hora_ini <= mala_direta.hh_inicial &&
                                                        ht.dt_hora_fim >= mala_direta.hh_final
                                                    )
                                                 ).Any()).Any()
                                             select ht.cd_registro
                                            ).Any()
                                        )
                               select a;

                if (!mala_direta.dtFimPeriodo.HasValue)
                    mala_direta.dtFimPeriodo = dataAtual;

                if (mala_direta.dtIniPeriodo.HasValue && mala_direta.dtFimPeriodo.HasValue)
                {
                    retorno1 = from a in retorno1
                               where a.AlunoPessoaFisica.dt_cadastramento >= mala_direta.dtIniPeriodo.Value
                               && a.AlunoPessoaFisica.dt_cadastramento <= mala_direta.dtFimPeriodo.Value
                               select a;
                }

                if (mala_direta.cd_estado > 0)
                    retorno1 = from p in retorno1
                               where p.AlunoPessoaFisica.EnderecoPrincipal.cd_loc_estado == mala_direta.cd_estado
                        select p;

                if (mala_direta.cd_cidade > 0)
                    retorno1 = from p in retorno1
                               where p.AlunoPessoaFisica.EnderecoPrincipal.cd_loc_cidade == mala_direta.cd_cidade
                        select p;


                if(mala_direta.cd_localidade > 0)
                    retorno1 = from p in retorno1
                               where p.AlunoPessoaFisica.EnderecoPrincipal.cd_loc_bairro == mala_direta.cd_localidade
                               select p;

                if (mala_direta.id_sexo.HasValue)
                    retorno1 = from p in retorno1
                               where p.AlunoPessoaFisica.nm_sexo == mala_direta.id_sexo
                               select p;

                if (mala_direta.nm_mes_nascimento.HasValue)
                    retorno1 = from p in retorno1
                               where p.AlunoPessoaFisica.dt_nascimento.Value.Month == mala_direta.nm_mes_nascimento.Value
                               select p;

                if (mala_direta.dt_matricula != null)
                    retorno1 = from p in retorno1
                               where p.Contratos.Any(y => y.dt_matricula_contrato == DbFunctions.TruncateTime(mala_direta.dt_matricula))
                               select p;

                if (mala_direta.dt_inicio_turma != null)
                    retorno1 = from p in retorno1
                               where p.AlunoTurma.Any(at => at.Turma.dt_inicio_aula == mala_direta.dt_inicio_turma)
                               select p;              

                if (mala_direta.cd_dia_semana.Count > 0)
                    retorno1 = from a in retorno1
                               where (from ht in db.Horario
                                    where a.Contratos.Where(c => c.AlunoTurma.Where(at =>
                                        ht.cd_registro == at.Turma.cd_turma
                                        && ht.id_origem == (int)Horario.Origem.TURMA
                                        && !at.Turma.id_turma_ppt
                                        && mala_direta.cd_dia_semana.Contains(ht.id_dia_semana)).Any()).Any()
                                    select ht.cd_registro).Any()
                               select a;

                if (mala_direta.cd_midia > 0)
                    retorno1 = from p in retorno1
                               where p.cd_midia == mala_direta.cd_midia
                               select p;

                if (!string.IsNullOrEmpty(mala_direta.nome))
                    retorno1 = from a in retorno1
                               where a.AlunoPessoaFisica.no_pessoa.Contains(mala_direta.nome)
                               select a;

                if (mala_direta.cd_escolaridade > 0)
                    retorno1 = from a in retorno1
                               where a.cd_escolaridade == mala_direta.cd_escolaridade
                               select a;

                if (mala_direta.idade > 0)
                {
                    retorno1 = from a in retorno1
                               where (a.AlunoPessoaFisica.dt_nascimento.HasValue &&
                                      ((DateTime.Now.Month < a.AlunoPessoaFisica.dt_nascimento.Value.Month ||
                                        (DateTime.Now.Month == a.AlunoPessoaFisica.dt_nascimento.Value.Month &&
                                         DateTime.Now.Day < a.AlunoPessoaFisica.dt_nascimento.Value.Day)) ?
                                          ((DateTime.Now.Year - a.AlunoPessoaFisica.dt_nascimento.Value.Year) - 1) == mala_direta.idade :
                                          (DateTime.Now.Year - a.AlunoPessoaFisica.dt_nascimento.Value.Year) == mala_direta.idade))
                               select a;
                }

                if (mala_direta.dtIniPeriodo.HasValue)
                    retorno1 = from a in retorno1
                               where a.AlunoPessoaFisica.dt_cadastramento >=
                               DbFunctions.TruncateTime(mala_direta.dtIniPeriodo.Value)                              
                               select a;

                if (mala_direta.dtFimPeriodo.HasValue)
                    retorno1 = from a in retorno1
                               where a.AlunoPessoaFisica.dt_cadastramento <=
                               DbFunctions.TruncateTime(mala_direta.dtFimPeriodo.Value)
                               select a;

                if (mala_direta.cd_turma > 0)
                {
                    retorno1 = from r in retorno1
                               where r.Contratos.Any(c => c.AlunoTurma.Any(at => at.Turma.cd_turma == mala_direta.cd_turma || at.Turma.cd_turma_ppt == mala_direta.cd_turma))
                               select r;
                }
                else
                {
                    if (mala_direta.cd_tipo_turma > 0)
                    {
                        if (mala_direta.cd_tipo_turma == (int)Turma.TipoTurma.PPT)
                        {
                            if (!mala_direta.existe_turmas_filhas && mala_direta.cd_situacao_turma > 0)
                            {
                                var ativo = true;
                                if (mala_direta.cd_situacao_turma == 2)
                                    ativo = false;

                                retorno1 = from r in retorno1
                                           where r.AlunoTurma.Any(at => at.Turma.cd_turma_ppt > 0 && at.Turma.TurmaPai.id_turma_ativa == ativo)
                                           select r;

                            }
                            if (mala_direta.existe_turmas_filhas)
                            {
                                retorno1 = from t in retorno1
                                           where t.Contratos.Any(c => c.AlunoTurma.Any(at => at.Turma.cd_turma_ppt != null && at.Turma.id_turma_ppt == false))
                                           select t;

                                if (mala_direta.cd_situacao_turma > 0)
                                {
                                    if (mala_direta.cd_situacao_turma == (int)Turma.SituacaoTurma.TURMASEMANDAMENTO)
                                        retorno1 = from r in retorno1
                                                   where r.AlunoTurma.Any(at => at.Turma.dt_termino_turma == null && at.Turma.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada))
                                                   select r;

                                    if (mala_direta.cd_situacao_turma == (int)Turma.SituacaoTurma.TURMASENCERRADAS)
                                        retorno1 = from r in retorno1
                                                   where r.AlunoTurma.Any(at => at.Turma.dt_termino_turma != null)
                                                   select r;

                                    if (mala_direta.cd_situacao_turma == (int)Turma.SituacaoTurma.TURMASFORMACAO)
                                        retorno1 = from r in retorno1
                                                   where r.AlunoTurma.Any(at => at.Turma.dt_termino_turma == null && !at.Turma.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada))
                                                   select r;
                                }
                            }
                               

                                //retorno1 = from r in retorno1
                                //           where r.AlunoTurma.Any(at => at.Turma.id_turma_ppt == true && at.Turma.cd_turma_ppt == null)
                                //           select r;

                        }
                        if (mala_direta.cd_tipo_turma == (int)Turma.TipoTurma.NORMAL)
                        {
                            retorno1 = from r in retorno1
                                       where r.AlunoTurma.Any(at => at.Turma.id_turma_ppt == false && at.Turma.cd_turma_ppt == null)
                                       select r;

                            if (mala_direta.cd_situacao_turma > 0)
                            {
                            
                                if (mala_direta.cd_situacao_turma == (int)Turma.SituacaoTurma.TURMASEMANDAMENTO)
                                    retorno1 = from r in retorno1
                                               where r.AlunoTurma.Any(at => at.Turma.dt_termino_turma == null && at.Turma.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada))
                                               select r;

                                if (mala_direta.cd_situacao_turma == (int)Turma.SituacaoTurma.TURMASENCERRADAS)
                                    retorno1 = from r in retorno1
                                               where r.Contratos.Any(c => c.AlunoTurma.Any(at => at.Turma.dt_termino_turma != null))
                                               select r;

                                if (mala_direta.cd_situacao_turma == (int)Turma.SituacaoTurma.TURMASFORMACAO)
                                    retorno1 = from r in retorno1
                                               where r.AlunoTurma.Any(at => at.Turma.dt_termino_turma == null && !at.Turma.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada))
                                               select r;
                            }

                            
                        }
                    }
                    else
                    {
                        retorno1 = from r in retorno1
                                   where r.AlunoTurma.Any(at => !at.Turma.id_turma_ppt)
                                   select r;

                        if (mala_direta.cd_situacao_turma == (int)Turma.SituacaoTurma.TURMASEMANDAMENTO)
                            retorno1 = from r in retorno1
                                       where r.AlunoTurma.Any(at => at.Turma.dt_termino_turma == null && at.Turma.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada))
                                       select r;

                        if (mala_direta.cd_situacao_turma == (int)Turma.SituacaoTurma.TURMASENCERRADAS)
                            retorno1 = from r in retorno1
                                       where r.AlunoTurma.Any(at => at.Turma.dt_termino_turma != null)
                                       select r;

                        if (mala_direta.cd_situacao_turma == (int)Turma.SituacaoTurma.TURMASFORMACAO)
                            retorno1 = from r in retorno1
                                       where r.AlunoTurma.Any(at => at.Turma.dt_termino_turma == null && !at.Turma.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada))
                                       select r;
                    }
                }
                
                retorno = (from p in retorno1
                           select new
                           {
                               p.cd_pessoa_aluno,
                               p.AlunoPessoaFisica.no_pessoa ,
                               id_cadastro = (byte)MalaDireta.TipoCadastro.ALUNO,
                               email = p.AlunoPessoaFisica.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL).FirstOrDefault().dc_fone_mail
                           }).ToList().Select(x => new ListaEnderecoMala
                           {
                               cd_cadastro = x.cd_pessoa_aluno,
                               no_pessoa = x.no_pessoa,
                               dc_email_cadastro = x.email,
                               id_cadastro = x.id_cadastro
                           });

                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<ListaEnderecoMala> getListagemEnderecosComporMensagemCliente(MalaDireta mala_direta)
        {
            try
            {
                IEnumerable<ListaEnderecoMala> retorno;
                var dataAtual = DateTime.Now.Date;

                var sql = from p in db.PessoaSGF.OfType<PessoaFisicaSGF>()
                          select p;

                var retorno1 = from p in sql
                               where p.Alunos.Any(a => a.cd_pessoa_escola == mala_direta.cd_escola && !a.Contratos.Any()) 
                               select p;

                if (!mala_direta.dtFimPeriodo.HasValue)
                    mala_direta.dtFimPeriodo = dataAtual;

                if (mala_direta.dtIniPeriodo.HasValue && mala_direta.dtFimPeriodo.HasValue)
                {
                    retorno1 = from al in retorno1
                               where (al.Alunos.Any(a => 
                                   a.AlunoPessoaFisica.dt_cadastramento >= mala_direta.dtIniPeriodo.Value
                                   && a.AlunoPessoaFisica.dt_cadastramento  <= mala_direta.dtFimPeriodo.Value))
                               select al;
                }

                if (mala_direta.cd_estado > 0)
                    retorno1 = from p in retorno1
                               where p.Alunos.Any(x => x.AlunoPessoaFisica.EnderecoPrincipal.cd_loc_estado == mala_direta.cd_estado)
                        select p;

                if (mala_direta.cd_cidade > 0)
                    retorno1 = from p in retorno1
                               where p.Alunos.Any(x => x.AlunoPessoaFisica.EnderecoPrincipal.cd_loc_cidade == mala_direta.cd_cidade)
                        select p;


                if (mala_direta.cd_localidade > 0)
                    retorno1 = from a in retorno1
                               where a.Alunos.Any(p => p.AlunoPessoaFisica.EnderecoPrincipal.cd_loc_bairro == mala_direta.cd_localidade)
                               select a;

                if (mala_direta.id_sexo.HasValue)
                    retorno1 = from p in retorno1
                               where p.nm_sexo == mala_direta.id_sexo
                               select p;

                if (mala_direta.nm_mes_nascimento.HasValue)
                    retorno1 = from p in retorno1
                               where p.dt_nascimento.Value.Month == mala_direta.nm_mes_nascimento.Value
                               select p;

                if (!string.IsNullOrEmpty(mala_direta.nome))
                    retorno1 = from a in retorno1
                               where a.no_pessoa.Contains(mala_direta.nome)
                               select a;

                if (mala_direta.cd_escolaridade > 0)
                    retorno1 = from al in retorno1
                               where al.Alunos.Any(a => a.cd_escolaridade == mala_direta.cd_escolaridade)
                               select al;

                if (mala_direta.idade > 0)
                {
                    retorno1 = from al in retorno1
                               where (al.Alunos.Any(a => a.AlunoPessoaFisica.dt_nascimento.HasValue &&
                                             ((DateTime.Now.Month < a.AlunoPessoaFisica.dt_nascimento.Value.Month ||
                                               (DateTime.Now.Month == a.AlunoPessoaFisica.dt_nascimento.Value.Month &&
                                                DateTime.Now.Day < a.AlunoPessoaFisica.dt_nascimento.Value.Day)) ?
                                                 ((DateTime.Now.Year - a.AlunoPessoaFisica.dt_nascimento.Value.Year) - 1) == mala_direta.idade :
                                                 (DateTime.Now.Year - a.AlunoPessoaFisica.dt_nascimento.Value.Year) == mala_direta.idade)))
                                 select al;
                }

                retorno = (from p in retorno1
                           select new
                           {
                               p.cd_pessoa,
                               p.no_pessoa,
                               id_cadastro = (byte)MalaDireta.TipoCadastro.CLIENTE,
                               email = p.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL).FirstOrDefault().dc_fone_mail
                           }).ToList().Select(x => new ListaEnderecoMala
                           {
                               cd_cadastro = x.cd_pessoa,
                               no_pessoa = x.no_pessoa,
                               dc_email_cadastro = x.email,
                               id_cadastro = x.id_cadastro
                           });

                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<ListaEnderecoMala> getListagemEnderecosComporMensagemPessoaRelacionada(MalaDireta mala_direta, bool filtro_empresa)
        {
            try
            {
                IEnumerable<ListaEnderecoMala> retorno;
                var dataAtual = DateTime.Now.Date;

                int situacaoAlunoInativo = 0;

                var sql = from p in db.PessoaSGF.OfType<PessoaFisicaSGF>()
                          select p;

                if (mala_direta.cd_situacao_aluno != null && mala_direta.cd_situacao_aluno.Count > 0
                                                          && mala_direta.cd_situacao_aluno.Contains((int)TipoSituacaoAluno.INATIVO))
                {
                    situacaoAlunoInativo = (int)TipoSituacaoAluno.INATIVO;
                    mala_direta.cd_situacao_aluno.Remove((int)TipoSituacaoAluno.INATIVO);
                }

                var retorno1 = from p in sql
                               where (p.PessoaEmpresa.Where(pe => pe.cd_escola == mala_direta.cd_escola).Any() 
                                && !p.PessoaFisicaFuncioanrio.Where(pff => pff.cd_pessoa_empresa == mala_direta.cd_escola).Any())
                                && !p.Prospect.Any(pp => pp.cd_pessoa_escola == mala_direta.cd_escola)
                                && !p.Alunos.Any(a => a.cd_pessoa_escola == mala_direta.cd_escola)

                               select p;

                

                if (!mala_direta.dtFimPeriodo.HasValue)
                    mala_direta.dtFimPeriodo = dataAtual;

                if (mala_direta.dtIniPeriodo.HasValue && mala_direta.dtFimPeriodo.HasValue)
                {
                    retorno1 = from al in retorno1
                               where al.dt_cadastramento >= mala_direta.dtIniPeriodo.Value
                               && al.dt_cadastramento <= mala_direta.dtFimPeriodo.Value
                               select al;
                }

                if (mala_direta.cd_estado > 0)
                    retorno1 = from p in retorno1
                        where p.EnderecoPrincipal.cd_loc_estado == mala_direta.cd_estado
                        select p;

                if (mala_direta.cd_cidade > 0)
                    retorno1 = from p in retorno1
                        where p.EnderecoPrincipal.cd_loc_cidade == mala_direta.cd_cidade
                        select p;


                if (mala_direta.cd_localidade > 0)
                    retorno1 = from p in retorno1
                               where p.EnderecoPrincipal.cd_loc_bairro == mala_direta.cd_localidade
                               select p;

                if (mala_direta.id_sexo.HasValue)
                    retorno1 = from p in retorno1
                               where p.nm_sexo == mala_direta.id_sexo
                               select p;

                if (mala_direta.nm_mes_nascimento.HasValue)
                    retorno1 = from p in retorno1
                               where p.dt_nascimento.Value.Month == mala_direta.nm_mes_nascimento.Value
                               select p;

                if (mala_direta.cd_tipo_papel.Count > 0)
                    retorno1 = from p in retorno1
                               where (p.PessoaFilhoRelacionamento.Where(r => mala_direta.cd_tipo_papel.Contains(r.RelacionamentoFilhoPapel.cd_papel)).Any() ||
                                        p.PessoaPaiRelacionamento.Where(rp => mala_direta.cd_tipo_papel.Contains(rp.RelacionamentoPaiPapel.cd_papel)).Any())
                               select p;

                if (mala_direta.cd_grau_parentesco.Count > 0)
                    retorno1 = from r in retorno1
                               where (r.PessoaFilhoRelacionamento.Where(rp => mala_direta.cd_grau_parentesco.Contains((int)rp.cd_qualif_relacionamento)).Any())
                               select r;

                if (!string.IsNullOrEmpty(mala_direta.nome))
                    retorno1 = from p in retorno1
                               where p.no_pessoa.Contains(mala_direta.nome)
                               select p;

               
                if (mala_direta.idade > 0)
                {
                    retorno1 = from p in retorno1
                               where p.dt_nascimento.HasValue &&
                               ((DateTime.Now.Month < p.dt_nascimento.Value.Month || 
                                 (DateTime.Now.Month == p.dt_nascimento.Value.Month &&
                                  DateTime.Now.Day < p.dt_nascimento.Value.Day)) ?
                                    ((DateTime.Now.Year - p.dt_nascimento.Value.Year) - 1) == mala_direta.idade :
                                    (DateTime.Now.Year - p.dt_nascimento.Value.Year) == mala_direta.idade)
                               select p;
                }

                

                // Filtrar empresa
                var sqlJuridica = from p in db.PessoaSGF.OfType<PessoaJuridicaSGF>()
                          select p;

                var juridica = from p in sqlJuridica
                               where (p.PessoaEmpresa.Where(pe => pe.cd_escola == mala_direta.cd_escola).Any())                                
                               select p;

                if (mala_direta.cd_tipo_papel.Count > 0)
                    juridica = from p in juridica
                               where (p.PessoaFilhoRelacionamento.Where(r => mala_direta.cd_tipo_papel.Contains(r.RelacionamentoFilhoPapel.cd_papel)).Any() ||
                                        p.PessoaPaiRelacionamento.Where(rp => mala_direta.cd_tipo_papel.Contains(rp.RelacionamentoPaiPapel.cd_papel)).Any())
                               select p;

               

                if (!filtro_empresa)
                    retorno = PessoaFisicaMalaDireta(retorno1);
                else
                    retorno = retornoPessoaFisicaJuridica(retorno1, juridica);

                if (mala_direta.cd_situacao_aluno != null && mala_direta.cd_situacao_aluno.Count > 0)
                {
                    retorno = from p in retorno
                              join pe in db.PessoaEscola on p.cd_cadastro equals pe.cd_pessoa
                              where
                                  pe.cd_escola == mala_direta.cd_escola &&
                                  (from r in db.RelacionamentoSGF
                                   where p.cd_cadastro == r.cd_pessoa_filho &&
                                         r.cd_papel_pai == (int)PapelSGF.TipoPapelSGF.ALUNORESPONSAVEL && r.cd_papel_filho == (int)PapelSGF.TipoPapelSGF.RESPONSAVEL &&
                                         ( //Alunos ativos com as situações selecionadas  
                                             (from a in db.Aluno
                                               where
                                              a.cd_pessoa_aluno == r.cd_pessoa_pai &&  a.id_aluno_ativo == true &&  
                                              (from at in db.AlunoTurma
                                               where a.cd_aluno == at.cd_aluno && at.Aluno.cd_pessoa_escola == mala_direta.cd_escola && mala_direta.cd_situacao_aluno.Contains((int)at.cd_situacao_aluno_turma)
                                               select at.cd_aluno).Any()
                                                select a.cd_aluno).Any() ||
                                              //quando marcado inativo junto as situações do aluno, busca os inativos + os ativos com as situações selecionadas
                                              (from a in db.Aluno
                                               where
                                                   a.cd_pessoa_escola == mala_direta.cd_escola && a.cd_pessoa_aluno == r.cd_pessoa_pai && 
                                                   ((situacaoAlunoInativo == (int)TipoSituacaoAluno.INATIVO) && a.id_aluno_ativo == false)
                                               select a.cd_aluno).Any() ||
                                             //quando as situações matriculado ou rematriculado estiver marcada traz também os alunos matriculados sem turma
                                             (from a in db.Aluno
                                              where a.cd_pessoa_escola == mala_direta.cd_escola && a.cd_pessoa_aluno == r.cd_pessoa_pai && a.id_aluno_ativo == true && 
                                                   (from c in db.Contrato where c.cd_aluno == a.cd_aluno && 
                                                             !(from at in db.AlunoTurma where at.cd_aluno == c.cd_aluno && c.cd_contrato == at.cd_contrato select at).Any()
                                                    select c).Any() && 
                                                    (
                                                       mala_direta.cd_situacao_aluno.Contains((int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Ativo)||
                                                    mala_direta.cd_situacao_aluno.Contains((int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Rematriculado))
                                              select a).Any())

                                   select r).Any()
                              select p;
                }else
                {
                    retorno = from p in retorno
                              join pe in db.PessoaEscola on p.cd_cadastro equals pe.cd_pessoa
                              where
                                  pe.cd_escola == mala_direta.cd_escola &&
                                  (from r in db.RelacionamentoSGF
                                   where p.cd_cadastro == r.cd_pessoa_filho &&
                                         r.cd_papel_pai == (int)PapelSGF.TipoPapelSGF.ALUNORESPONSAVEL && r.cd_papel_filho == (int)PapelSGF.TipoPapelSGF.RESPONSAVEL &&
                                         ((from a in db.Aluno
                                           where
                                               a.cd_pessoa_aluno == r.cd_pessoa_pai && a.id_aluno_ativo == (situacaoAlunoInativo == (int)TipoSituacaoAluno.INATIVO ? false : true) &&
                                               (from at in db.AlunoTurma
                                                where a.cd_aluno == at.cd_aluno && at.Aluno.cd_pessoa_escola == mala_direta.cd_escola
                                                select at.cd_aluno).Any()
                                           select a.cd_aluno).Any() ||
                                          //quando não marcar nada também traz os alunos matriculados sem turma
                                          (from a in db.Aluno
                                           where a.cd_pessoa_escola == mala_direta.cd_escola && a.cd_pessoa_aluno == r.cd_pessoa_pai && a.id_aluno_ativo == true &&
                                                 situacaoAlunoInativo != (int)TipoSituacaoAluno.INATIVO &&
                                                 (from c in db.Contrato
                                                  where c.cd_aluno == a.cd_aluno &&
                                                        !(from at in db.AlunoTurma where at.cd_aluno == c.cd_aluno && c.cd_contrato == at.cd_contrato select at).Any()
                                                  select c).Any() 
                                           select a).Any())

                                   select r).Any()
                              select p;
                }




                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        private IEnumerable<ListaEnderecoMala> PessoaFisicaMalaDireta(IQueryable<PessoaFisicaSGF> pessoaFisica)
        {
            var listaEnderecoMala = new List<ListaEnderecoMala>();

            listaEnderecoMala.AddRange((from p in pessoaFisica
                                     select new
                                     {
                                         p.cd_pessoa,
                                         p.no_pessoa,
                                         id_cadastro = (byte)MalaDireta.TipoCadastro.PESSOARELACIONADA,
                                         email = p.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL).FirstOrDefault().dc_fone_mail
                                     }).ToList().Select(x => new ListaEnderecoMala
                                     {
                                         cd_cadastro = x.cd_pessoa,
                                         no_pessoa = x.no_pessoa,
                                         dc_email_cadastro = x.email,
                                         id_cadastro = x.id_cadastro
                                     }));

            return listaEnderecoMala;
        }

        private IEnumerable<ListaEnderecoMala> retornoPessoaFisicaJuridica(IQueryable<PessoaFisicaSGF> pessoaFisica, IQueryable<PessoaJuridicaSGF> PessoaJuridica)
        {
            var fisicaJuridica = new List<ListaEnderecoMala>();

            fisicaJuridica.AddRange((from p in pessoaFisica
                                     select new
                                     {
                                         p.cd_pessoa,
                                         p.no_pessoa,
                                         id_cadastro = (byte)MalaDireta.TipoCadastro.PESSOARELACIONADA,
                                         email = p.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL).FirstOrDefault().dc_fone_mail
                                     }).ToList().Select(x => new ListaEnderecoMala
                           {
                               cd_cadastro = x.cd_pessoa,
                               no_pessoa = x.no_pessoa,
                               dc_email_cadastro = x.email,
                               id_cadastro = x.id_cadastro
                           }));

            fisicaJuridica.AddRange((from p in PessoaJuridica
                                     select new
                                     {
                                         p.cd_pessoa,
                                         p.no_pessoa,
                                         id_cadastro = (byte)MalaDireta.TipoCadastro.PESSOARELACIONADA,
                                         email = p.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL).FirstOrDefault().dc_fone_mail
                                     }).ToList().Select(x => new ListaEnderecoMala
                                     {
                                         cd_cadastro = x.cd_pessoa,
                                         no_pessoa = x.no_pessoa,
                                         dc_email_cadastro = x.email,
                                         id_cadastro = x.id_cadastro
                                     }));
            return fisicaJuridica;
        }

        public IEnumerable<ListaEnderecoMala> getListEndComporMsgFuncProfissao(MalaDireta mala_direta)
        {
            try
            {
                IEnumerable<ListaEnderecoMala> retorno;
                var dataAtual = DateTime.Now.Date;

                var sql = from p in db.PessoaSGF.OfType<PessoaFisicaSGF>()
                          where p.PessoaFisicaFuncioanrio.Any(a => a.cd_pessoa_empresa == mala_direta.cd_escola && a.id_funcionario_ativo == true)
                          select p;

                var retorno1 = sql;

                if (mala_direta.id_tipo_profissao > 0)            
                {
                    switch (mala_direta.id_tipo_profissao)
                    {
                        case (int)MalaDireta.TipoProfissao.COORDENADOR:
                            retorno1 = from c in sql
                                       where db.FuncionarioSGF.OfType<Professor>().Any(x => x.cd_pessoa_empresa == mala_direta.cd_escola && x.cd_pessoa_funcionario == c.cd_pessoa && x.id_coordenador == true)
                                       select c;
                            break;

                        case (int)MalaDireta.TipoProfissao.CYBER:
                            retorno1 = from p in sql
                                       where p.PessoaFisicaFuncioanrio.Any(a => a.cd_pessoa_empresa == mala_direta.cd_escola && a.id_colaborador_cyber == true && a.cd_pessoa_funcionario == p.cd_pessoa)
                                       select p;
                            break;

                        case (int)MalaDireta.TipoProfissao.PROFESSOR:
                            retorno1 = from p in sql
                                       where p.PessoaFisicaFuncioanrio.Any(a => a.cd_pessoa_empresa == mala_direta.cd_escola && a.id_professor == true && a.cd_pessoa_funcionario == p.cd_pessoa)
                                       select p;
                            break;

                        case (int)MalaDireta.TipoProfissao.FUNCIONARIO_PROFESSOR:
                            retorno1 = from p in sql
                                       where p.PessoaFisicaFuncioanrio.Any(a => a.cd_pessoa_empresa == mala_direta.cd_escola && a.cd_pessoa_funcionario == p.cd_pessoa)
                                       select p;
                            break;
                    }
                }

                if (!mala_direta.dtFimPeriodo.HasValue)
                    mala_direta.dtFimPeriodo = dataAtual;

                if (mala_direta.dtIniPeriodo.HasValue && mala_direta.dtFimPeriodo.HasValue)
                {
                    retorno1 = from al in retorno1
                               where al.dt_cadastramento >= mala_direta.dtIniPeriodo.Value
                               && al.dt_cadastramento  <= mala_direta.dtFimPeriodo.Value
                               select al;
                }

                if (mala_direta.id_sexo.HasValue)
                    retorno1 = from p in retorno1
                               where p.nm_sexo == mala_direta.id_sexo
                               select p;

                if (mala_direta.nm_mes_nascimento.HasValue)
                    retorno1 = from p in retorno1
                               where p.dt_nascimento.Value.Month == mala_direta.nm_mes_nascimento.Value
                               select p;

                if (!string.IsNullOrEmpty(mala_direta.nome))
                    retorno1 = from p in retorno1
                               where p.no_pessoa.Contains(mala_direta.nome)
                               select p;

                if (mala_direta.idade > 0)
                {
                    retorno1 = from p in retorno1
                               where p.dt_nascimento.HasValue &&
                                     ((DateTime.Now.Month < p.dt_nascimento.Value.Month ||
                                       (DateTime.Now.Month == p.dt_nascimento.Value.Month &&
                                        DateTime.Now.Day < p.dt_nascimento.Value.Day)) ?
                                         ((DateTime.Now.Year - p.dt_nascimento.Value.Year) - 1) == mala_direta.idade :
                                         (DateTime.Now.Year - p.dt_nascimento.Value.Year) == mala_direta.idade)
                               select p;
                }

                if (mala_direta.dtIniPeriodo.HasValue)
                    retorno1 = from al in retorno1
                               where al.dt_cadastramento >=
                               DbFunctions.TruncateTime(mala_direta.dtIniPeriodo.Value)
                               select al;

                if (mala_direta.dtFimPeriodo.HasValue)
                    retorno1 = from al in retorno1
                               where al.dt_cadastramento <=
                               DbFunctions.TruncateTime(mala_direta.dtFimPeriodo.Value)
                               select al;

                retorno = (from p in retorno1
                           select new
                           {
                               p.cd_pessoa,
                               p.no_pessoa,
                               id_cadastro = (byte)MalaDireta.TipoCadastro.FUNCIONARIOPROFESSOR,
                               email = p.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL).FirstOrDefault().dc_fone_mail
                           }).ToList().Select(x => new ListaEnderecoMala
                           {
                               cd_cadastro = x.cd_pessoa,
                               no_pessoa = x.no_pessoa,
                               dc_email_cadastro = x.email,
                               id_cadastro = x.id_cadastro
                           });

                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<ListaNaoInscrito> getListagemEnderecosMalaDireta(int cd_empresa, int cd_mala_direta)
        {
            try
            {
                var sql = (from l in db.ListaEnderecoMala
                           join p in db.PessoaSGF on l.cd_cadastro equals p.cd_pessoa
                           where l.MalaDireta.cd_escola == cd_empresa && l.cd_mala_direta == cd_mala_direta
                           select new
                           {
                               l.cd_cadastro,
                               l.id_cadastro,
                               p.no_pessoa,
                               email = p.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL).FirstOrDefault().dc_fone_mail//,
                           }).ToList().Select(x => new ListaNaoInscrito
                           {
                               cd_cadastro = x.cd_cadastro,
                               id_cadastro = x.id_cadastro,
                               no_pessoa = x.no_pessoa,
                               dc_email_cadastro = x.email
                           });
                return sql;

            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool existeEmailListagemEscola(int cd_empresa, int cd_cadastro, int id_cadastro)
        {
            try
            {
                var sql = from p in db.PessoaSGF.OfType<PessoaFisicaSGF>()
                          where p.cd_pessoa == cd_cadastro
                          select p;
                switch (id_cadastro)
                {
                    case (int)MalaDireta.TipoCadastro.PROSPECT:
                        sql = from p in sql
                              where
                              p.Prospect.Any(pp => pp.cd_pessoa_escola == cd_empresa)
                              select p;
                        break;
                    case (int)MalaDireta.TipoCadastro.ALUNO:
                    case (int)MalaDireta.TipoCadastro.CLIENTE:
                        sql = from p in sql
                              where
                              p.Alunos.Any(a => a.cd_pessoa_escola == cd_empresa)
                              select p;
                        break;
                    case (int)MalaDireta.TipoCadastro.PESSOARELACIONADA:
                        sql = from p in sql
                              where
                             p.PessoaEmpresa.Where(pe => pe.cd_escola == cd_empresa).Any()
                              select p;
                        break;
                }
                bool retorno = (from p in sql
                                select p.cd_pessoa).Any();

                return retorno;

            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<ListaNaoInscrito> getListagemEnderecos(int cd_empresa, string no_pessoa, int status, string email, byte id_tipo_cadastro)
        {
            try
            {
                IEnumerable<ListaNaoInscrito> retorno;
                var sql = from p in db.PessoaSGF.OfType<PessoaFisicaSGF>()
                          where (
                            ((id_tipo_cadastro == 0 || id_tipo_cadastro == 1) && p.Prospect.Where(x => x.cd_pessoa_escola == cd_empresa).Any()) ||
                            ((id_tipo_cadastro == 0 || id_tipo_cadastro == 2) && p.Alunos.Where(x => x.cd_pessoa_escola == cd_empresa).Any()) ||
                            ((id_tipo_cadastro == 0 || id_tipo_cadastro == 5) && p.PessoaFisicaFuncioanrio.Where(x => x.cd_pessoa_empresa == cd_empresa).Any()) ||
                            ((id_tipo_cadastro == 0 || id_tipo_cadastro == 3 || id_tipo_cadastro == 4) && p.PessoaEmpresa.Where(x => x.cd_escola == cd_empresa).Any())
                            ) &&
                          p.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL).Any()
                          select p;
                if (!string.IsNullOrEmpty(no_pessoa))
                    sql = from p in sql
                          where p.no_pessoa.Contains(no_pessoa)
                          select p;
                if (!String.IsNullOrEmpty(email))
                {
                    sql = from p in sql
                          where p.TelefonePessoa.Any(t => t.dc_fone_mail.Contains(email))
                          select p;
                }
                if (id_tipo_cadastro == 0)
                {
                    /*
                    retorno = (from p in sql
                               where
                               p.Prospect.Any(pp => pp.cd_pessoa_escola == cd_empresa && !pp.Aluno.Any()) ||
                               p.Alunos.Any(a => a.cd_pessoa_escola == cd_empresa && a.Contratos.Any()) ||
                               p.Alunos.Any(a => a.cd_pessoa_escola == cd_empresa && !a.Contratos.Any()) ||
                               (p.PessoaEmpresa.Where(pe => pe.cd_escola == cd_empresa).Any() && !p.PessoaFisicaFuncioanrio.Where(pff => pff.cd_pessoa_empresa == cd_empresa).Any())
                               orderby p.no_pessoa descending
                               select new
                               {
                                   p.cd_pessoa,
                                   p.no_pessoa,
                                   id_cadastro = p.Prospect.Any(pp => pp.cd_pessoa_escola == cd_empresa && !pp.Aluno.Any()) ? (byte)MalaDireta.TipoCadastro.PROSPECT :
                                                   p.Alunos.Any(a => a.cd_pessoa_escola == cd_empresa && a.Contratos.Any()) ? (byte)MalaDireta.TipoCadastro.ALUNO :
                                                     p.Alunos.Any(a => a.cd_pessoa_escola == cd_empresa && !a.Contratos.Any()) ? (byte)MalaDireta.TipoCadastro.CLIENTE :
                                                       (byte)MalaDireta.TipoCadastro.PESSOARELACIONADA,
                                   email = p.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL).FirstOrDefault().dc_fone_mail,
                                   inscrito = db.ListaNaoInscrito.Any(x => x.cd_escola == cd_empresa && x.cd_cadastro == p.cd_pessoa && x.id_cadastro == (p.Prospect.Any(pp => pp.cd_pessoa_escola == cd_empresa && !pp.Aluno.Any()) ? (byte)MalaDireta.TipoCadastro.PROSPECT :
                                                   p.Alunos.Any(a => a.cd_pessoa_escola == cd_empresa && a.Contratos.Any()) ? (byte)MalaDireta.TipoCadastro.ALUNO :
                                                     p.Alunos.Any(a => a.cd_pessoa_escola == cd_empresa && !a.Contratos.Any()) ? (byte)MalaDireta.TipoCadastro.CLIENTE :
                                                       (byte)MalaDireta.TipoCadastro.PESSOARELACIONADA))
                               }).ToList().Select(x => new ListaNaoInscrito
                               {
                                   cd_cadastro = x.cd_pessoa,
                                   no_pessoa = x.no_pessoa,
                                   dc_email_cadastro = x.email,
                                   id_cadastro = x.id_cadastro,
                                   id_inscrito = x.inscrito
                                   //id_inscrito = x.ListaNaoInscrito.ToList().Select(i => i.id_cadastro == x.id_cadastro).FirstOrDefault()
                               });*/

                     retorno = (from p in sql
                                 where
                                 p.Prospect.Any(pp => pp.cd_pessoa_escola == cd_empresa && !pp.Aluno.Any()) 
                                       orderby p.no_pessoa descending
                                  select new
                                  {
                                     p.cd_pessoa,
                                     p.no_pessoa,
                                     id_cadastro = (byte)MalaDireta.TipoCadastro.PROSPECT,
                                     email = p.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL).FirstOrDefault().dc_fone_mail,
                                     inscrito = db.ListaNaoInscrito.Any(x => x.cd_escola == cd_empresa && x.cd_cadastro == p.cd_pessoa && x.id_cadastro == (byte)MalaDireta.TipoCadastro.PROSPECT)
                                   }).Union(    
                                (from p in sql
                                     where
                                     p.Alunos.Any(a => a.cd_pessoa_escola == cd_empresa && a.Contratos.Any())
                                     orderby p.no_pessoa descending
                                     select new
                                     {
                                         p.cd_pessoa,
                                         p.no_pessoa,
                                         id_cadastro = (byte)MalaDireta.TipoCadastro.ALUNO,
                                         email = p.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL).FirstOrDefault().dc_fone_mail,
                                         inscrito = db.ListaNaoInscrito.Any(x => x.cd_escola == cd_empresa && x.cd_cadastro == p.cd_pessoa && x.id_cadastro == (byte)MalaDireta.TipoCadastro.ALUNO)
                                     })).Union(
                                    (from p in sql
                                        where
                                        p.Alunos.Any(a => a.cd_pessoa_escola == cd_empresa && !a.Contratos.Any()) 
                                        orderby p.no_pessoa descending
                                        select new
                                        {
                                            p.cd_pessoa,
                                            p.no_pessoa,
                                            id_cadastro = (byte)MalaDireta.TipoCadastro.CLIENTE,
                                            email = p.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL).FirstOrDefault().dc_fone_mail,
                                            inscrito = db.ListaNaoInscrito.Any(x => x.cd_escola == cd_empresa && x.cd_cadastro == p.cd_pessoa && x.id_cadastro == (byte)MalaDireta.TipoCadastro.CLIENTE)
                                        }).Union(
                                        (from p in sql
                                              where
                                              (p.PessoaEmpresa.Where(pe => pe.cd_escola == cd_empresa).Any() &&
                                               !p.Prospect.Any(pp => pp.cd_pessoa_escola == cd_empresa && !p.Alunos.Any()) &&
                                               !p.PessoaFisicaFuncioanrio.Where(pff => pff.cd_pessoa_empresa == cd_empresa).Any() &&
                                               !p.Alunos.Any(a => a.cd_pessoa_escola == cd_empresa))
                                              orderby p.no_pessoa descending
                                              select new
                                              {
                                                  p.cd_pessoa,
                                                  p.no_pessoa,
                                                  id_cadastro = (byte)MalaDireta.TipoCadastro.PESSOARELACIONADA,
                                                  email = p.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL).FirstOrDefault().dc_fone_mail,
                                                  inscrito = db.ListaNaoInscrito.Any(x => x.cd_escola == cd_empresa && x.cd_cadastro == p.cd_pessoa && x.id_cadastro == (byte)MalaDireta.TipoCadastro.PESSOARELACIONADA)
                                              })
                                            )
                                        ).Union(
                                    (from p in sql
                                     where
                                     p.Alunos.Any(a => a.cd_pessoa_escola == cd_empresa && !a.Contratos.Any())
                                     orderby p.no_pessoa descending
                                     select new
                                     {
                                         p.cd_pessoa,
                                         p.no_pessoa,
                                         id_cadastro = (byte)MalaDireta.TipoCadastro.CLIENTE,
                                         email = p.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL).FirstOrDefault().dc_fone_mail,
                                         inscrito = db.ListaNaoInscrito.Any(x => x.cd_escola == cd_empresa && x.cd_cadastro == p.cd_pessoa && x.id_cadastro == (byte)MalaDireta.TipoCadastro.CLIENTE)
                                     }).Union(
                                        (from p in sql
                                         where
                                         (p.PessoaFisicaFuncioanrio.Where(pff => pff.cd_pessoa_empresa == cd_empresa).Any())
                                         orderby p.no_pessoa descending
                                         select new
                                         {
                                             p.cd_pessoa,
                                             p.no_pessoa,
                                             id_cadastro = (byte)MalaDireta.TipoCadastro.FUNCIONARIOPROFESSOR,
                                             email = p.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL).FirstOrDefault().dc_fone_mail,
                                             inscrito = db.ListaNaoInscrito.Any(x => x.cd_escola == cd_empresa && x.cd_cadastro == p.cd_pessoa && x.id_cadastro == (byte)MalaDireta.TipoCadastro.FUNCIONARIOPROFESSOR)
                                         })
                                        )
                                        ).ToList().Select(x => new ListaNaoInscrito
                                         {
                                             cd_cadastro = x.cd_pessoa,
                                             no_pessoa = x.no_pessoa,
                                             dc_email_cadastro = x.email,
                                             id_cadastro = x.id_cadastro,
                                             id_inscrito = x.inscrito
                                        }).OrderByDescending(x => x.no_pessoa);
                    if (status > 0)
                        if (status == (int)ListaEnderecoMala.TipoEnderecoMala.INSCRITO)
                            retorno = from p in retorno
                                      where !p.id_inscrito
                                      select p;
                        else
                            retorno = (from p in retorno
                                       where p.id_inscrito
                                       select p).ToList();

                }
                else
                {
                    switch (id_tipo_cadastro)
                    {
                        case (int)MalaDireta.TipoCadastro.PROSPECT:
                            sql = from p in sql
                                  where
                                  p.Prospect.Any(pp => pp.cd_pessoa_escola == cd_empresa && !pp.Aluno.Any())
                                  select p;
                            break;
                        case (int)MalaDireta.TipoCadastro.ALUNO:
                            sql = from p in sql
                                  where
                                  p.Alunos.Any(a => a.cd_pessoa_escola == cd_empresa && a.Contratos.Any())
                                  select p;
                            break;
                        case (int)MalaDireta.TipoCadastro.CLIENTE:
                            sql = from p in sql
                                  where
                                  p.Alunos.Any(a => a.cd_pessoa_escola == cd_empresa && !a.Contratos.Any())
                                  select p;
                            break;
                        case (int)MalaDireta.TipoCadastro.PESSOARELACIONADA:
                            sql = from p in sql
                                  where
                                 p.PessoaEmpresa.Where(pe => pe.cd_escola == cd_empresa).Any() &&
                                 !p.Prospect.Any(pp => pp.cd_pessoa_escola == cd_empresa && !p.Alunos.Any()) &&
                                 !p.Alunos.Any(a => a.cd_pessoa_escola == cd_empresa) &&
                                 !p.PessoaFisicaFuncioanrio.Where(pff => pff.cd_pessoa_empresa == cd_empresa).Any()
                                  select p;
                            break;
                        case (int)MalaDireta.TipoCadastro.FUNCIONARIOPROFESSOR:
                            sql = from p in sql
                                where
                                    p.PessoaFisicaFuncioanrio.Where(pff => pff.cd_pessoa_empresa == cd_empresa).Any()
                                select p;
                            break;
                    }
                    if (status > 0)
                        if (status == (int)ListaEnderecoMala.TipoEnderecoMala.INSCRITO)
                            sql = from p in sql
                                  where !db.ListaNaoInscrito.Any(x => x.cd_escola == cd_empresa && x.cd_cadastro == p.cd_pessoa && x.id_cadastro == id_tipo_cadastro)
                                  select p;
                        else
                            sql = from p in sql
                                  where db.ListaNaoInscrito.Any(x => x.cd_escola == cd_empresa && x.cd_cadastro == p.cd_pessoa && x.id_cadastro == id_tipo_cadastro)
                                  select p;
                    retorno = (from p in sql
                               orderby p.no_pessoa descending
                               select new
                               {
                                   cd_cadastro = p.cd_pessoa,
                                   no_pessoa = p.no_pessoa,
                                   id_cadastro = id_tipo_cadastro,
                                   email = p.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL).FirstOrDefault().dc_fone_mail//,
                                   //inscrito = db.ListaNaoInscrito.Where(x => x.cd_escola == cd_empresa && x.cd_cadastro == p.cd_pessoa && x.id_cadastro == id_tipo_cadastro).Any()
                               }).ToList().Select(x => new ListaNaoInscrito
                               {
                                   cd_cadastro = x.cd_cadastro,
                                   no_pessoa = x.no_pessoa,
                                   dc_email_cadastro = x.email,
                                   id_cadastro = x.id_cadastro,
                                   //id_inscrito = x.inscrito
                               });
                }

                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<RptListagemEndereco> getRptListagemEnderecos(int cd_empresa, string no_pessoa, int status, string email, byte id_tipo_cadastro)
        {
            try
            {
                IEnumerable<RptListagemEndereco> retorno;
                var sql = from p in db.PessoaSGF.OfType<PessoaFisicaSGF>()
                          where p.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL).Any()
                          select p;
                if (!string.IsNullOrEmpty(no_pessoa))
                    sql = from p in sql
                          where p.no_pessoa.Contains(no_pessoa)
                          select p;
                if (!String.IsNullOrEmpty(email))
                {
                    sql = from p in sql
                          where p.TelefonePessoa.Any(t => t.dc_fone_mail.Contains(email))
                          select p;
                }
                if (id_tipo_cadastro == 0)
                {
                    retorno = (from p in sql
                               where
                               p.Prospect.Any(pp => pp.cd_pessoa_escola == cd_empresa && !pp.Aluno.Any()) ||
                               p.Alunos.Any(a => a.cd_pessoa_escola == cd_empresa && a.Contratos.Any()) ||
                               p.Alunos.Any(a => a.cd_pessoa_escola == cd_empresa && !a.Contratos.Any()) ||
                               (p.PessoaEmpresa.Where(pe => pe.cd_escola == cd_empresa).Any() && !p.PessoaFisicaFuncioanrio.Where(pff => pff.cd_pessoa_empresa == cd_empresa).Any())
                               orderby p.no_pessoa descending
                               select new
                               {
                                   p.cd_pessoa,
                                   p.no_pessoa,
                                   no_bairro = p.EnderecoPrincipal.Bairro.no_localidade,
                                   no_cidade = p.EnderecoPrincipal.Cidade.no_localidade,
                                   dc_num_cep = p.EnderecoPrincipal.Logradouro.dc_num_cep,
                                   sg_estado = p.EnderecoPrincipal.Estado.Estado.sg_estado,
                                   no_localidade = p.EnderecoPrincipal.Logradouro.no_localidade,
                                   dc_compl_endereco = p.EnderecoPrincipal.dc_compl_endereco,
                                   no_tipo_logradouro = p.EnderecoPrincipal.TipoLogradouro.no_tipo_logradouro,
                                   dc_num_endereco = p.EnderecoPrincipal.dc_num_endereco,
                                   email = p.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL).FirstOrDefault().dc_fone_mail,
                                   id_cadastro = p.Prospect.Any(pp => pp.cd_pessoa_escola == cd_empresa && !pp.Aluno.Any()) ? (byte)MalaDireta.TipoCadastro.PROSPECT :
                                                      p.Alunos.Any(a => a.cd_pessoa_escola == cd_empresa && a.Contratos.Any()) ? (byte)MalaDireta.TipoCadastro.ALUNO :
                                                        p.Alunos.Any(a => a.cd_pessoa_escola == cd_empresa && !a.Contratos.Any()) ? (byte)MalaDireta.TipoCadastro.CLIENTE :
                                                          (byte)MalaDireta.TipoCadastro.PESSOARELACIONADA,
                                   inscrito = db.ListaNaoInscrito.Any(x => x.cd_escola == cd_empresa && x.cd_cadastro == p.cd_pessoa && x.id_cadastro == (p.Prospect.Any(pp => pp.cd_pessoa_escola == cd_empresa && !pp.Aluno.Any()) ? (byte)MalaDireta.TipoCadastro.PROSPECT :
                                                    p.Alunos.Any(a => a.cd_pessoa_escola == cd_empresa && a.Contratos.Any()) ? (byte)MalaDireta.TipoCadastro.ALUNO :
                                                        p.Alunos.Any(a => a.cd_pessoa_escola == cd_empresa && !a.Contratos.Any()) ? (byte)MalaDireta.TipoCadastro.CLIENTE :(byte)MalaDireta.TipoCadastro.PESSOARELACIONADA))
                               }).ToList().Select(x => new RptListagemEndereco
                               {
                                   no_pessoa = x.no_pessoa,
                                   email = x.email,
                                   no_bairro = x.no_bairro,
                                   no_cidade = x.no_cidade,
                                   dc_num_cep = x.dc_num_cep,
                                   sg_estado = x.sg_estado,
                                   no_localidade = x.no_localidade,
                                   dc_compl_endereco = x.dc_compl_endereco,
                                   no_tipo_logradouro = x.no_tipo_logradouro,
                                   dc_num_endereco = x.dc_num_endereco,
                                   id_inscrito = x.inscrito
                               });
                    if (status > 0)
                        if (status == (int)ListaEnderecoMala.TipoEnderecoMala.INSCRITO)
                            retorno = from p in retorno
                                      where !p.id_inscrito
                                      select p;
                        else
                            retorno = (from p in retorno
                                       where p.id_inscrito
                                       select p).ToList();

                }
                else
                {
                    switch (id_tipo_cadastro)
                    {
                        case (int)MalaDireta.TipoCadastro.PROSPECT:
                            sql = from p in sql
                                  where
                                  p.Prospect.Any(pp => pp.cd_pessoa_escola == cd_empresa && !pp.Aluno.Any())
                                  select p;
                            break;
                        case (int)MalaDireta.TipoCadastro.ALUNO:
                            sql = from p in sql
                                  where
                                  p.Alunos.Any(a => a.cd_pessoa_escola == cd_empresa && a.Contratos.Any())
                                  select p;
                            break;
                        case (int)MalaDireta.TipoCadastro.CLIENTE:
                            sql = from p in sql
                                  where
                                  p.Alunos.Any(a => a.cd_pessoa_escola == cd_empresa && !a.Contratos.Any())
                                  select p;
                            break;
                        case (int)MalaDireta.TipoCadastro.PESSOARELACIONADA:
                            sql = from p in sql
                                  where
                                 p.PessoaEmpresa.Where(pe => pe.cd_escola == cd_empresa).Any() &&
                                 !p.Prospect.Any(pp => pp.cd_pessoa_escola == cd_empresa && !p.Alunos.Any()) &&
                                 !p.Alunos.Any(a => a.cd_pessoa_escola == cd_empresa) &&
                                 !p.PessoaFisicaFuncioanrio.Where(pff => pff.cd_pessoa_empresa == cd_empresa).Any()
                                  select p;
                            break;
                    }
                    if (status > 0)
                        if (status == (int)ListaEnderecoMala.TipoEnderecoMala.INSCRITO)
                            sql = from p in sql
                                  where !db.ListaNaoInscrito.Any(x => x.cd_escola == cd_empresa && x.cd_cadastro == p.cd_pessoa && x.id_cadastro == id_tipo_cadastro)
                                  select p;
                        else
                            sql = from p in sql
                                  where db.ListaNaoInscrito.Any(x => x.cd_escola == cd_empresa && x.cd_cadastro == p.cd_pessoa && x.id_cadastro == id_tipo_cadastro)
                                  select p;
                    retorno = (from p in sql
                               orderby p.no_pessoa descending
                               select new
                               {
                                   cd_cadastro = p.cd_pessoa,
                                   no_pessoa = p.no_pessoa,
                                   id_cadastro = id_tipo_cadastro,
                                   no_bairro = p.EnderecoPrincipal.Bairro.no_localidade,
                                   no_cidade = p.EnderecoPrincipal.Cidade.no_localidade,
                                   dc_num_cep = p.EnderecoPrincipal.Logradouro.dc_num_cep,
                                   sg_estado = p.EnderecoPrincipal.Estado.Estado.sg_estado,
                                   no_localidade = p.EnderecoPrincipal.Logradouro.no_localidade,
                                   dc_compl_endereco = p.EnderecoPrincipal.dc_compl_endereco,
                                   no_tipo_logradouro = p.EnderecoPrincipal.TipoLogradouro.no_tipo_logradouro,
                                   dc_num_endereco = p.EnderecoPrincipal.dc_num_endereco,
                                   email = p.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL).FirstOrDefault().dc_fone_mail//,
                                   //inscrito = db.ListaNaoInscrito.Where(x => x.cd_escola == cd_empresa && x.cd_cadastro == p.cd_pessoa && x.id_cadastro == id_tipo_cadastro).Any()
                               }).ToList().Select(x => new RptListagemEndereco
                               {
                                   no_pessoa = x.no_pessoa,
                                   email = x.email,
                                   no_bairro = x.no_bairro,
                                   no_cidade = x.no_cidade,
                                   dc_num_cep = x.dc_num_cep,
                                   sg_estado = x.sg_estado,
                                   no_localidade = x.no_localidade,
                                   dc_compl_endereco = x.dc_compl_endereco,
                                   no_tipo_logradouro = x.no_tipo_logradouro,
                                   dc_num_endereco = x.dc_num_endereco
                               });
                }

                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool existEnderecoPrincipalPessoa(int cd_pessoa)
        {
            try
            {
                var existEndereco = (from endereco in db.EnderecoSGF 
                                     where endereco.cd_pessoa == cd_pessoa && endereco.Pessoa.cd_endereco_principal != null 
                                     select endereco).Any();
                return existEndereco;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<RptListagemEndereco> getRptListagemEnderecosMalaDireta(int cd_empresa, int cd_mala_direta)
        {
            try
            {
                var sql = (from l in db.ListaEnderecoMala
                           join p in db.PessoaSGF on l.cd_cadastro equals p.cd_pessoa
                           where l.MalaDireta.cd_escola == cd_empresa && l.cd_mala_direta == cd_mala_direta
                           select new
                           {
                               p.no_pessoa,
                               email = p.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL).FirstOrDefault().dc_fone_mail,
                               no_bairro = p.EnderecoPrincipal.Bairro.no_localidade,
                               no_cidade = p.EnderecoPrincipal.Cidade.no_localidade,
                               dc_num_cep = p.EnderecoPrincipal.Logradouro.dc_num_cep,
                               sg_estado = p.EnderecoPrincipal.Estado.Estado.sg_estado,
                               no_localidade = p.EnderecoPrincipal.Logradouro.no_localidade,
                               dc_compl_endereco = p.EnderecoPrincipal.dc_compl_endereco,
                               no_tipo_logradouro = p.EnderecoPrincipal.TipoLogradouro.no_tipo_logradouro,
                               dc_num_endereco = p.EnderecoPrincipal.dc_num_endereco,
                           }).ToList().Select(x => new RptListagemEndereco
                           {
                               no_pessoa = x.no_pessoa,
                               email = x.email,
                               no_bairro = x.no_bairro,
                               no_cidade = x.no_cidade,
                               dc_num_cep = x.dc_num_cep,
                               sg_estado = x.sg_estado,
                               no_localidade = x.no_localidade,
                               dc_compl_endereco = x.dc_compl_endereco,
                               no_tipo_logradouro = x.no_tipo_logradouro,
                               dc_num_endereco = x.dc_num_endereco
                           });
                return sql;

            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<ListaEnderecoMala> getListagemEndAlunosInadimplentes(MalaDireta mala_direta)
        {
            try
            {
                IEnumerable<ListaEnderecoMala> retorno;
                List<int> listaProdutos = new List<int>();
                var dataAtual = DateTime.Now.Date;

                var retorno1 = from a in db.Aluno
                               join pf in db.PessoaSGF.OfType<PessoaFisicaSGF>() on a.cd_pessoa_aluno equals pf.cd_pessoa
                               where a.cd_pessoa_escola == mala_direta.cd_escola 
                                                      && pf.TituloPessoa.Where(tp => tp.id_status_titulo == (byte)FundacaoFisk.SGF.GenericModel.Titulo.StatusTitulo.ABERTO
                                                      && tp.id_status_cnab == (byte)FundacaoFisk.SGF.GenericModel.Titulo.StatusCnabTitulo.INICIAL
                                                      && tp.dt_vcto_titulo <= dataAtual).Any()
                               select a;

                if (!mala_direta.dtFimPeriodo.HasValue)
                    mala_direta.dtFimPeriodo = dataAtual;

                if (mala_direta.dtIniPeriodo.HasValue && mala_direta.dtFimPeriodo.HasValue)
                {
                    //os alunos  que possuem títulos abertos, que não possuem baixas e nem Cnab emitido, no período selecionado
                    retorno1 = from a in retorno1
                               join pf in db.PessoaSGF.OfType<PessoaFisicaSGF>() on a.cd_pessoa_aluno equals pf.cd_pessoa
                               where a.cd_pessoa_escola == mala_direta.cd_escola
                                                      && pf.TituloPessoa.Where(tp => tp.id_status_titulo == (byte)FundacaoFisk.SGF.GenericModel.Titulo.StatusTitulo.ABERTO
                                                      && tp.id_status_cnab == (byte)FundacaoFisk.SGF.GenericModel.Titulo.StatusCnabTitulo.INICIAL
                                                      && tp.dt_vcto_titulo >= mala_direta.dtIniPeriodo.Value
                                                      && tp.dt_vcto_titulo <= mala_direta.dtFimPeriodo.Value
                                  ).Any()
                          select a;
                }

                if (mala_direta.MalasDiretaProduto != null && mala_direta.MalasDiretaProduto.Count > 0)
                {
                    listaProdutos = mala_direta.MalasDiretaProduto.Select(x => x.cd_produto).ToList();
                    retorno1 = from a in retorno1
                               where a.AlunoTurma.Where(t => listaProdutos.Contains(t.Turma.cd_produto)).Any()
                               select a;
                }

                if (!string.IsNullOrEmpty(mala_direta.nome))
                    retorno1 = from a in retorno1
                               where a.AlunoPessoaFisica.no_pessoa.Contains(mala_direta.nome)
                               select a;

                if (mala_direta.idade > 0)
                {
                    retorno1 = from a in retorno1
                               where (a.AlunoPessoaFisica.dt_nascimento.HasValue &&
                                      ((DateTime.Now.Month < a.AlunoPessoaFisica.dt_nascimento.Value.Month ||
                                        (DateTime.Now.Month == a.AlunoPessoaFisica.dt_nascimento.Value.Month &&
                                         DateTime.Now.Day < a.AlunoPessoaFisica.dt_nascimento.Value.Day)) ?
                                          ((DateTime.Now.Year - a.AlunoPessoaFisica.dt_nascimento.Value.Year) - 1) == mala_direta.idade :
                                          (DateTime.Now.Year - a.AlunoPessoaFisica.dt_nascimento.Value.Year) == mala_direta.idade))
                               select a;
                }               

                if (mala_direta.cd_pessoa_responsavel > 0)
                    retorno1 = from a in retorno1
                               join pf in db.PessoaSGF.OfType<PessoaFisicaSGF>() on a.cd_pessoa_aluno equals pf.cd_pessoa
                               where pf.PessoaPaiRelacionamento.Any(p => p.cd_pessoa_filho == mala_direta.cd_pessoa_responsavel)
                               select a;

                retorno = (from p in retorno1
                           select new
                           {
                               p.cd_pessoa_aluno,
                               p.AlunoPessoaFisica.no_pessoa,
                               id_cadastro = (byte)MalaDireta.TipoCadastro.ALUNO,
                               email = p.AlunoPessoaFisica.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL).FirstOrDefault().dc_fone_mail
                           }).ToList().Select(x => new ListaEnderecoMala
                           {
                               cd_cadastro = x.cd_pessoa_aluno,
                               no_pessoa = x.no_pessoa,
                               dc_email_cadastro = x.email,
                               id_cadastro = x.id_cadastro
                           });

                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}

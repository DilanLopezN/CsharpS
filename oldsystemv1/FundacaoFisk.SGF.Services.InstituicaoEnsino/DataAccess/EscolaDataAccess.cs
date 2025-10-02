using System;
using System.Collections.Generic;
using System.Linq;
using FundacaoFisk.SGF.GenericModel;
using Componentes.GenericDataAccess.GenericRepository;
using System.Data.Entity;
using Componentes.Utils;
using Componentes.GenericDataAccess;
using FundacaoFisk.SGF.Services.InstituicaoEnsino.Model;
using System.Data;
using Componentes.GenericModel;
using FundacaoFisk.SGF.Web.Services.InstituicaoEnsino.Comum.IDataAccess;
using Componentes.GenericDataAccess.GenericException;
using Componentes.Utils.Messages;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using FundacaoFisk.SGF.Web.Services.Usuario.Model;
using System.Data.Common;
using DALC4NET;

namespace FundacaoFisk.SGF.Web.Services.InstituicaoEnsino.DataAccess
{
    public class EscolaDataAccess : GenericRepository<Escola>, IEscolaDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }
       
        //retorna escolas com item
        public IEnumerable<EscolaUI> findEscolasSecionadas(int cdItem, int cd_usuario, bool masterGeral)
        {
            try{
                IQueryable<EscolaUI> sql;
                if (masterGeral == false)
                {
                    sql = from escola in db.PessoaSGF.OfType<Escola>()
                          where escola.EscolaItem.Where(es => es.cd_item == cdItem).Any()
                                && escola.Usuarios.Where(usu => usu.cd_usuario == cd_usuario).Any()
                          orderby escola.cd_pessoa
                          select new EscolaUI
                          {
                              cd_pessoa = escola.cd_pessoa,
                              no_pessoa = escola.no_pessoa
                          };
                }
                else
                {
                    sql = from escola in db.PessoaSGF.OfType<Escola>()
                          where escola.EscolaItem.Where(es => es.cd_item == cdItem).Any()
                          orderby escola.cd_pessoa
                          select new EscolaUI
                          {
                              cd_pessoa = escola.cd_pessoa,
                              no_pessoa = escola.no_pessoa
                          };
                }
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<EscolaUI> getEscolaByDesc(SearchParameters parametros, string desc, bool inicio, bool? ativo, string cnpj, string fantasia, int cdUsuario)
        {
            try{
                IEntitySorter<EscolaUI> sorter = EntitySorter<EscolaUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<EscolaUI> sql;

                var ret = from escola in db.PessoaSGF.OfType<Escola>().AsNoTracking()
                          // join enderecoPes in db.EnderecoSGF 
                          // on escola.cd_endereco_principal equals enderecoPes.cd_endereco 
                          // into end from enderecoPes in end.DefaultIfEmpty()
                           where !string.IsNullOrEmpty(cnpj) ? escola.dc_num_cgc == cnpj : string.IsNullOrEmpty(cnpj)
                                 && !string.IsNullOrEmpty(fantasia) ? escola.dc_reduzido_pessoa.Contains(fantasia) : string.IsNullOrEmpty(fantasia)
                           select escola;

                if (cdUsuario > 0)
                    ret = from escola in ret
                          where escola.Usuarios.Where(u => u.cd_usuario == cdUsuario).Any()
                          select escola;


                if (ativo != null)
                {
                    ret = from escola in ret
                          where escola.id_escola_ativa == ativo
                          select escola;
                }

                sql = from escola in ret
                      orderby escola.no_pessoa
                      select new EscolaUI
                      {
                          cd_pessoa = escola.cd_pessoa,
                          dc_num_cgc = escola.dc_num_cgc,
                          dt_cadastramento = escola.dt_cadastramento,
                          dc_fone_email = escola.Telefone.dc_fone_mail,
                          no_pessoa = escola.no_pessoa,
                          dc_reduzido_pessoa = escola.dc_reduzido_pessoa,
                          id_pessoa_ativa = escola.id_escola_ativa,
                          ext_img_pessoa = escola.ext_img_pessoa,
                          cd_empresa_coligada = escola.cd_empresa_coligada
                      };
                sql = sorter.Sort(sql);

                var retorno = (from escola in sql
                               select escola);

                if (!String.IsNullOrEmpty(desc))
                {
                    if (inicio)
                        retorno = from escola in retorno
                                  where escola.no_pessoa.StartsWith(desc)
                                  select escola;
                    else
                        retorno = from escola in retorno
                                  where escola.no_pessoa.Contains(desc)
                                  select escola;
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

        public EscolaUI getEscolaForEdit(int cd_pessoa_empresa)
        {
            try
            {
                IQueryable<EscolaUI> sql;

                var ret = from escola in db.PessoaSGF.OfType<Escola>()
                          where escola.cd_pessoa == cd_pessoa_empresa
                          select escola;

                sql = from escola in ret
                      orderby escola.no_pessoa
                      select new EscolaUI
                      {
                          cd_pessoa = escola.cd_pessoa,
                          dc_num_cgc = escola.dc_num_cgc,
                          dt_cadastramento = escola.dt_cadastramento,
                          hr_final = escola.hr_final,
                          hr_inicial = escola.hr_inicial,
                          dc_fone_email = escola.Telefone.dc_fone_mail,
                          no_pessoa = escola.no_pessoa,
                          nm_cliente_integracao = escola.nm_cliente_integracao,
                          nm_escola_integracao = escola.nm_empresa_integracao,
                          id_pessoa_ativa = escola.id_escola_ativa,
                          parametro = escola.EscolaParametro,
                          ext_img_pessoa = escola.ext_img_pessoa,
                          cd_operadora = escola.Telefone.cd_operadora,
                          dc_reduzido_pessoa = escola.dc_reduzido_pessoa,
                          dt_inicio = escola.dt_inicio,
                          dt_abertura = escola.dt_abertura,
                          cd_empresa = escola.cd_empresa,
                          cd_empresa_coligada = escola.cd_empresa_coligada,

                          dc_plano_mat = escola.EscolaParametro.ParametroPlanoContaMatricula.PlanoContaSubgrupo.no_subgrupo_conta,
                          dc_plano_taxa = escola.EscolaParametro.ParametroPlanoContaTaxa.PlanoContaSubgrupo.no_subgrupo_conta,
                          dc_plano_juros = escola.EscolaParametro.ParametroPlanoContaJuros.PlanoContaSubgrupo.no_subgrupo_conta,
                          dc_plano_multa = escola.EscolaParametro.ParametroPlanoContaMulta.PlanoContaSubgrupo.no_subgrupo_conta,
                          dc_plano_desconto = escola.EscolaParametro.ParametroPlanoContaDesconto.PlanoContaSubgrupo.no_subgrupo_conta,
                          dc_plano_taxa_bco = escola.EscolaParametro.ParametroPlanoContaTaxaBco.PlanoContaSubgrupo.no_subgrupo_conta,
                          dc_plano_material = escola.EscolaParametro.ParametroPlanoContaMaterial.PlanoContaSubgrupo.no_subgrupo_conta,
                          dc_item_taxa_mat = escola.EscolaParametro.ItemTaxaMens != null ? escola.EscolaParametro.ItemTaxaMens.no_item : "",
                          dc_item_mensalidade = escola.EscolaParametro.ItemMensalidade != null ? escola.EscolaParametro.ItemMensalidade.no_item : "",
                          dc_item_biblioteca = escola.EscolaParametro.ItemBiblioteca != null ? escola.EscolaParametro.ItemBiblioteca.no_item : "",
                          dc_tpnf_biblioteca = escola.EscolaParametro.TpNFBiblioteca != null ? escola.EscolaParametro.TpNFBiblioteca.dc_tipo_nota_fiscal : "",
                          dc_tpnf_taxa_mensalidade = escola.EscolaParametro.TpNFMatricula != null ? escola.EscolaParametro.TpNFMatricula.dc_tipo_nota_fiscal : "",
                          dc_tpnf_material = escola.EscolaParametro.TpNFMaterial != null ? escola.EscolaParametro.TpNFMaterial.dc_tipo_nota_fiscal : "",
                          dc_tpnf_material_saida = escola.EscolaParametro.TpNFMaterialSaida != null ? escola.EscolaParametro.TpNFMaterialSaida.dc_tipo_nota_fiscal : "",
                          dc_pol_comercial = escola.EscolaParametro.PoliticaComercial != null ? escola.EscolaParametro.PoliticaComercial.dc_politica_comercial : "",
                          id_baixa_automatica_cartao = escola.EscolaParametro.id_baixa_automatica_cartao,
                          id_baixa_automatica_cheque = escola.EscolaParametro.id_baixa_automatica_cheque,
                          id_empresa_propria = escola.id_empresa_propria,
                          nm_dia_gerar_nfs = escola.nm_dia_gerar_nfs,
                          nm_investimento = escola.nm_investimento,
                          nm_patrimonio = escola.nm_patrimonio,
                          nome_assinatura_certificado = escola.nome_assinatura_certificado,
                          dc_item_servico = escola.EscolaParametro.ItemServico != null ? escola.EscolaParametro.ItemServico.no_item : "",
                          dc_tp_nf_servico = escola.EscolaParametro.TipoNotaFiscalServico != null ? escola.EscolaParametro.TipoNotaFiscalServico.dc_tipo_nota_fiscal : "",
                          dc_plano_contas_servico = escola.EscolaParametro.PlanoContaServico != null ? escola.EscolaParametro.PlanoContaServico.PlanoContaSubgrupo.no_subgrupo_conta : "",
                          dc_pol_comercial_servico = escola.EscolaParametro.PoliticaComercialServico != null ? escola.EscolaParametro.PoliticaComercialServico.dc_politica_comercial : "",
                          nm_dia_gerar_nf_servico = escola.nm_dia_gerar_nf_servico,
                          id_empresa_internacional = escola.id_empresa_internacional,

                          empresaValorServico = (from evs in db.EmpresaValorServico where evs.cd_pessoa_empresa == escola.cd_pessoa select evs).ToList()
                      };
              
                return sql.FirstOrDefault();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<PessoaSearchUI> getSearchEscolas(SearchParameters parametros, string desc, string cnpj, string fantasia, bool inicio)
        {
            try
            {
                IEntitySorter<PessoaSearchUI> sorter = EntitySorter<PessoaSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<PessoaSearchUI> sql;

                var ret = from escola in db.PessoaSGF.OfType<Escola>().AsNoTracking()
                          where escola.id_escola_ativa == true
                          select escola;

                if (!String.IsNullOrEmpty(desc))
                    if (inicio)
                        ret = from escola in ret
                              where escola.no_pessoa.StartsWith(desc)
                              select escola;
                    else
                        ret = from escola in ret
                              where escola.no_pessoa.Contains(desc)
                              select escola;
                if (!string.IsNullOrEmpty(cnpj))
                    ret = from escola in ret
                          where escola.dc_num_cgc == cnpj
                          select escola;
                if (!string.IsNullOrEmpty(fantasia))
                    if (inicio)
                        ret = from escola in ret
                              where escola.dc_reduzido_pessoa.StartsWith(fantasia)
                              select escola;
                    else
                        ret = from escola in ret
                              where escola.dc_reduzido_pessoa.Contains(fantasia)
                              select escola;

                sql = from escola in ret
                      orderby escola.no_pessoa
                      select new PessoaSearchUI
                      {
                          cd_pessoa = escola.cd_pessoa,
                          nm_cpf_cgc = escola.dc_num_cgc,
                          dt_cadastramento = escola.dt_cadastramento,
                          no_pessoa = escola.no_pessoa,
                          dc_reduzido_pessoa = escola.dc_reduzido_pessoa
                      };

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

        public IEnumerable<PessoaSearchUI> getEscolaNotWithItem(SearchParameters parametros, string desc, string cnpj, string fantasia, int cd_item, bool inicio)
        {
            try
            {
                IEntitySorter<PessoaSearchUI> sorter = EntitySorter<PessoaSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<PessoaSearchUI> sql;
                IQueryable<Escola> ret;

                if (cd_item > 0)
                    ret = from escola in db.PessoaSGF.OfType<Escola>().AsNoTracking()
                          where escola.id_escola_ativa == true
                         // && !escola.EscolaItem.Any(ex => ex.cd_pessoa_escola == escola.cd_pessoa && ex.cd_item == cd_item)
                          select escola;
                else
                    ret = from escola in db.PessoaSGF.OfType<Escola>().AsNoTracking()
                          where escola.id_escola_ativa == true
                          select escola;
               
                if (!String.IsNullOrEmpty(desc))
                    if (inicio)
                        ret = from escola in ret
                              where escola.no_pessoa.StartsWith(desc)
                              select escola;
                    else
                        ret = from escola in ret
                              where escola.no_pessoa.Contains(desc)
                              select escola;
                if (!string.IsNullOrEmpty(cnpj))
                    ret = from escola in ret
                          where escola.dc_num_cgc == cnpj
                          select escola;
                if (!string.IsNullOrEmpty(fantasia))
                    if (inicio)
                        ret = from escola in ret
                              where escola.dc_reduzido_pessoa.StartsWith(fantasia)
                              select escola;
                    else
                        ret = from escola in ret
                              where escola.dc_reduzido_pessoa.Contains(fantasia)
                              select escola;

                sql = from escola in ret
                      orderby escola.no_pessoa
                      select new PessoaSearchUI
                      {
                          cd_pessoa = escola.cd_pessoa,
                          nm_cpf_cgc = escola.dc_num_cgc,
                          dt_cadastramento = escola.dt_cadastramento,
                          no_pessoa = escola.no_pessoa,
                          dc_reduzido_pessoa = escola.dc_reduzido_pessoa
                      };

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

        public IEnumerable<PessoaSearchUI> getEscolaNotWithKit(SearchParameters parametros, string desc, List<int> empresas, string cnpj, string fantasia, int cd_item, bool inicio)
        {
            try
            {
                IEntitySorter<PessoaSearchUI> sorter = EntitySorter<PessoaSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<PessoaSearchUI> sql;
                IQueryable<Escola> ret;

                if (cd_item > 0)
                    ret = from escola in db.PessoaSGF.OfType<Escola>().AsNoTracking()
                          where escola.id_escola_ativa == true
                           //&& !escola.EscolaItem.Any(ex => ex.cd_pessoa_escola == escola.cd_pessoa && ex.cd_item == cd_item)
                          select escola;
                else
                    ret = from escola in db.PessoaSGF.OfType<Escola>().AsNoTracking()
                          where escola.id_escola_ativa == true
                          select escola;

                if (!String.IsNullOrEmpty(desc))
                    if (inicio)
                        ret = from escola in ret
                              where escola.no_pessoa.StartsWith(desc)
                              select escola;
                    else
                        ret = from escola in ret
                              where escola.no_pessoa.Contains(desc)
                              select escola;
                if (!string.IsNullOrEmpty(cnpj))
                    ret = from escola in ret
                          where escola.dc_num_cgc == cnpj
                          select escola;
                if (!string.IsNullOrEmpty(fantasia))
                    if (inicio)
                        ret = from escola in ret
                              where escola.dc_reduzido_pessoa.StartsWith(fantasia)
                              select escola;
                    else
                        ret = from escola in ret
                              where escola.dc_reduzido_pessoa.Contains(fantasia)
                              select escola;

                ret = from escola in ret
                    where !empresas.Contains(escola.cd_pessoa)
                      select escola;

                sql = from escola in ret
                      orderby escola.dc_reduzido_pessoa, escola.no_pessoa
                      select new PessoaSearchUI
                      {
                          cd_pessoa = escola.cd_pessoa,
                          nm_cpf_cgc = escola.dc_num_cgc,
                          dt_cadastramento = escola.dt_cadastramento,
                          no_pessoa = escola.no_pessoa,
                          dc_reduzido_pessoa = escola.dc_reduzido_pessoa
                      };

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

        public int? getCodigoFranquia(int cd_escola, int id_aplicacao){
            try {
                var sql = (from escola in db.PessoaSGF.OfType<Escola>()
                           where escola.cd_pessoa == cd_escola
                           select 
                             id_aplicacao != 1 ?
                               (escola.cd_empresa_coligada == null ? 
                                  escola.nm_cliente_integracao : 
                                 (from e in db.PessoaSGF.OfType<Escola>() where e.cd_pessoa == escola.cd_empresa_coligada select e.nm_cliente_integracao).FirstOrDefault()) :
                              escola.nm_cliente_integracao
                          ).FirstOrDefault();

                return sql;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<PessoaSearchUI> getEscolaHasItem(int cd_item)
        {
            try
            {
                var sql = from escola in db.PessoaSGF.OfType<Escola>()
                          where escola.EscolaItem.Any(ex => ex.cd_item == cd_item)
                          orderby escola.no_pessoa
                          select new PessoaSearchUI
                          {
                              cd_pessoa = escola.cd_pessoa,
                              no_pessoa = escola.no_pessoa,
                              dc_reduzido_pessoa = escola.dc_reduzido_pessoa
                          };

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public IEnumerable<PessoaSearchUI> getEscolaHasTpDesc(int cdTpDesc)
        {
            try
            {
                var sql = from escola in db.PessoaSGF.OfType<Escola>()
                          where escola.TiposDescontoEscola.Any(ex => ex.cd_tipo_desconto == cdTpDesc)
                          orderby escola.no_pessoa
                          select new PessoaSearchUI
                          {
                              cd_pessoa = escola.cd_pessoa,
                              no_pessoa = escola.no_pessoa,
                              dc_reduzido_pessoa = escola.dc_reduzido_pessoa
                          };

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool verificaHorarioOcupado(int cd_pessoa_escola, TimeSpan hr_ini, TimeSpan hr_fim)
        {
            try
            {
                //var sql = (from escola in db.PessoaSGF.OfType<Escola>()
                //          where escola.cd_pessoa == cd_pessoa_escola
                //             && (db.Horario.Any(h => h.cd_pessoa_escola == escola.cd_pessoa && (hr_ini > h.dt_hora_ini || hr_fim < h.dt_hora_fim)))
                //          select escola.cd_empresa).Any();

                var sql = false;
                byte horarioRet = (from horario in db.Horario
                                   orderby horario.id_origem
                                   where horario.cd_pessoa_escola == cd_pessoa_escola &&
                                   (hr_ini > horario.dt_hora_ini || hr_fim < horario.dt_hora_fim)
                                   select horario.id_origem).FirstOrDefault();
                switch (horarioRet)
                {
                    case (byte)Horario.Origem.ALUNO:
                        {
                            
                            sql = (from horario in db.Horario
                                    join aluno in db.Aluno on horario.cd_registro equals aluno.cd_pessoa_aluno
                                    where horario.cd_pessoa_escola == cd_pessoa_escola &&
                                    aluno.id_aluno_ativo &&
                                    horario.id_origem == (byte)Horario.Origem.ALUNO &&
                                   (hr_ini > horario.dt_hora_ini || hr_fim < horario.dt_hora_fim)
                                    select aluno.AlunoPessoaFisica.no_pessoa).Any();
                            break;
                        }
                    case (byte)Horario.Origem.PROFESSOR:
                        {
                            sql = (from horario in db.Horario
                                    join prof in db.FuncionarioSGF on horario.cd_registro equals prof.cd_funcionario
                                    where horario.cd_pessoa_escola == cd_pessoa_escola &&
                                    prof.id_funcionario_ativo &&
                                    horario.id_origem == (byte)Horario.Origem.PROFESSOR &&
                                   (hr_ini > horario.dt_hora_ini || hr_fim < horario.dt_hora_fim)
                                    select prof.FuncionarioPessoaFisica.no_pessoa).Any();
                            break;
                        }
                    case (byte)Horario.Origem.TURMA:
                        {
                            sql = (from horario in db.Horario
                                    join turma in db.Turma on horario.cd_registro equals turma.cd_turma
                                    where horario.cd_pessoa_escola == cd_pessoa_escola &&
                                    horario.id_origem == (byte)Horario.Origem.TURMA &&
                                   (hr_ini > horario.dt_hora_ini || hr_fim < horario.dt_hora_fim)
                                    select turma.no_turma).Any();

                            break;
                        }
                    case (byte)Horario.Origem.USUARIO:
                        {
                            sql = (
                                from horario in db.Horario
                                join usuario in db.UsuarioWebSGF on horario.cd_registro equals usuario.cd_usuario
                                join pessoa in db.PessoaSGF.OfType<PessoaFisicaSGF>() on usuario.cd_pessoa equals pessoa.cd_pessoa
                                where horario.cd_pessoa_escola == cd_pessoa_escola && usuario.id_usuario_ativo &&
                                horario.id_origem == (byte)Horario.Origem.USUARIO &&
                                (hr_ini > horario.dt_hora_ini || hr_fim < horario.dt_hora_fim)
                                select usuario).Any();

                            break;
                        }
                }

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool? verifcaEstadoEscAluno(int cd_pessoa_escola, int cd_aluno)
        {
            try
            {
                int estadoEsc = (from p in db.PessoaSGF
                                 where p.cd_pessoa == cd_pessoa_escola && p.EnderecoPrincipal != null
                                 select p.EnderecoPrincipal.cd_loc_estado).FirstOrDefault();
                int estadoAl = (from p in db.PessoaSGF
                                where p.cd_pessoa == cd_aluno && p.EnderecoPrincipal != null
                                select p.EnderecoPrincipal.cd_loc_estado).FirstOrDefault();
                bool? igual = null;
                if (estadoAl > 0 && estadoEsc > 0)
                {
                    if (estadoEsc != estadoAl)
                        igual = false;
                    else
                        igual = true;
                }
                return igual;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<UsuarioWebSGF> getUsuariosById(List<int> cdUsers)
        {
            try
            {
                var sql = from u in db.UsuarioWebSGF
                          where cdUsers.Contains(u.cd_usuario)
                          select u;
                return sql;
            }

            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public void deleteUsuarioContext(UsuarioWebSGF user)
        {
            try
            {
                db.UsuarioWebSGF.Remove(user);
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool getEmpresaPropria(int cd_escola)
        {
            try
            {
                var retorno = (from escola in db.PessoaSGF.OfType<Escola>()
                           where escola.cd_pessoa == cd_escola
                           select escola.id_empresa_propria).FirstOrDefault();

                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        #region Follow-Up

        public IEnumerable<UsuarioUISearch> getUsuarioSearchFKFollowUp(SearchParameters parametros, string descricao, string nome, bool inicio, int cd_empresa, 
                                                               int cd_usuario_logado, int tipoPesq, string usuariologado, Int32[] codEscolas)
        {
            try
            {
                IEntitySorter<UsuarioUISearch> sorter = EntitySorter<UsuarioUISearch>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<UsuarioUISearch> sql;
                IQueryable<UsuarioWebSGF> sqlInicial;

                sqlInicial = from usuario in db.UsuarioWebSGF.AsNoTracking()
                             orderby usuario.no_login
                             select usuario;
                switch (tipoPesq)
                {
                    case (int)UsuarioUI.TipoPesquisaUsuarioEnum.HAS_USUARIO_ORIGEM:
                        sqlInicial = from usuario in sqlInicial
                                          where (usuario.Empresas.Where(ep => ep.cd_pessoa_empresa == cd_empresa).Any() || !usuario.Empresas.Any()) &&
                                                 usuario.UsuarioFollowUp.Any()
                                     orderby usuario.no_login
                                     select usuario;
                        break;
                    case (int)UsuarioUI.TipoPesquisaUsuarioEnum.HAS_USUARIO_DESTINO:
                        sqlInicial = from usuario in sqlInicial
                                     where (usuario.Empresas.Where(ep => ep.cd_pessoa_empresa == cd_empresa).Any() || !usuario.Empresas.Any()) && usuario.FollowUpDestino.Any()
                                     orderby usuario.no_login
                                     select usuario;
                        break;
                    case (int)UsuarioUI.TipoPesquisaUsuarioEnum.HAS_USUARIO_MASTER:
                        sqlInicial = from usuario in sqlInicial
                                     where (usuario.Empresas.Where(ep => ep.cd_pessoa_empresa == cd_empresa).Any() || !usuario.Empresas.Any()) && 
                                            usuario.id_master && usuario.UsuarioFollowUp.Where(x => x.cd_escola == null).Any()
                                     orderby usuario.no_login
                                     select usuario;
                        break;
                    case (int)UsuarioUI.TipoPesquisaUsuarioEnum.HAS_USUARIOS_ATIVOS_ESCOLA:
                        sqlInicial = from usuario in sqlInicial     //Antes estava var sqlInicial2
                                     where usuario.id_usuario_ativo && usuario.Empresas.Where(ep => ep.cd_pessoa_empresa == cd_empresa).Any() 
                                     orderby usuario.no_login
                                     select usuario;
                    

                        // Eliinado a pedido do Eduardo em 25/06/2021
                        //if (codEscolas != null && codEscolas.Count() > 0)
                        //{
                        //    sqlInicial = from s in sqlInicial2
                        //                 where
                        //                 !(from empresas in s.Empresas where !codEscolas.Contains(empresas.cd_pessoa_empresa) select empresas.cd_pessoa_empresa).Any()
                        //                 select s;
                        //}

                        break;
                    case (int)UsuarioUI.TipoPesquisaUsuarioEnum.HAS_USUARIOS_CAD_PROSPECT_ALUNO:
                        sqlInicial = from usuario in sqlInicial     //Antes estava var sqlInicial2
                                     where usuario.id_usuario_ativo && usuario.Empresas.Where(ep => ep.cd_pessoa_empresa == cd_empresa).Any()
                                     orderby usuario.no_login
                                     select usuario;
                        break;

                }

                if (!String.IsNullOrEmpty(nome))
                    if (inicio)
                        sqlInicial = from usuarioWeb in sqlInicial
                                     where usuarioWeb.PessoaFisica.no_pessoa.StartsWith(nome)
                                     select usuarioWeb;
                    else
                        sqlInicial = from usuarioWeb in sqlInicial
                                     where usuarioWeb.PessoaFisica.no_pessoa.Contains(nome)
                                     select usuarioWeb;

                if (!String.IsNullOrEmpty(descricao))
                    if (inicio)
                        sqlInicial = from usuarioWeb in sqlInicial
                                     where usuarioWeb.no_login.StartsWith(descricao)
                                     select usuarioWeb;
                    else
                        sqlInicial = from usuarioWeb in sqlInicial
                                     where usuarioWeb.no_login.Contains(descricao)
                                     select usuarioWeb;

                sql = from usuario in sqlInicial
                      select new UsuarioUISearch
                      {
                          cd_pessoa = usuario.cd_pessoa,
                          cd_usuario = usuario.cd_usuario,
                          id_master = usuario.id_master,
                          no_login = usuario.no_login,
                          id_usuario_ativo = usuario.id_usuario_ativo,
                          no_pessoa = db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == usuario.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().no_pessoa,
                          //Empresas = (from e in db.Pessoa.OfType<Empresa>() where e.Usuarios.Where( u => u.cd_usuario == usuario.cd_usuario).Any() select e).ToList(),
                          id_admin = usuario.id_admin,
                          id_administrador = usuario.id_administrador
                      };

                sql = sorter.Sort(sql);
                int limite = sql.Count();
                parametros.ajustaParametrosPesquisa(limite);
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);
                var retorno = sql.ToList();
                parametros.qtd_limite = limite;
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<PessoaSearchUI> getEscolasFollowUp(int cd_follow_up, int cd_escola)
        {
            try
            {
                var sql = from escola in db.PessoaSGF.OfType<Escola>()
                          where escola.id_escola_ativa && escola.FollowUpsEscola.Where(fe => fe.cd_follow_up == cd_follow_up && fe.FollowUp.cd_escola == cd_escola).Any()
                          select new PessoaSearchUI
                          {
                              cd_pessoa = escola.cd_pessoa,
                              nm_cpf_cgc = escola.dc_num_cgc,
                              dt_cadastramento = escola.dt_cadastramento,
                              no_pessoa = escola.no_pessoa,
                              dc_reduzido_pessoa = escola.dc_reduzido_pessoa
                          };

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<PessoaSearchUI> getSearchEscolasFKFollowUp(SearchParameters parametros, string desc, string cnpj, string fantasia,
            bool inicio, List<int> cdsEmpresa, bool masterGeral, int? cd_estado, int? cd_cidade)
        {
            try
            {
                IEntitySorter<PessoaSearchUI> sorter = EntitySorter<PessoaSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<PessoaSearchUI> sql;

                var ret = from escola in db.PessoaSGF.OfType<Escola>().AsNoTracking()
                          where escola.id_escola_ativa == true
                          select escola;
                if (!masterGeral)
                    ret = from escola in ret
                          where cdsEmpresa.Contains(escola.cd_pessoa)
                          select escola;
                if (cd_estado.HasValue)
                    ret = from escola in ret
                          where escola.EnderecoPrincipal.cd_loc_estado == cd_estado.Value
                          select escola;
                if (cd_cidade.HasValue && cd_cidade != 0)
                    ret = from escola in ret
                          where escola.EnderecoPrincipal.cd_loc_cidade == cd_cidade.Value
                          select escola;
                if (!String.IsNullOrEmpty(desc))
                    if (inicio)
                        ret = from escola in ret
                              where escola.no_pessoa.StartsWith(desc)
                              select escola;
                    else
                        ret = from escola in ret
                              where escola.no_pessoa.Contains(desc)
                              select escola;
                if (!string.IsNullOrEmpty(cnpj))
                    ret = from escola in ret
                          where escola.dc_num_cgc == cnpj
                          select escola;
                if (!string.IsNullOrEmpty(fantasia))
                    if (inicio)
                        ret = from escola in ret
                              where escola.dc_reduzido_pessoa.StartsWith(fantasia)
                              select escola;
                    else
                        ret = from escola in ret
                              where escola.dc_reduzido_pessoa.Contains(fantasia)
                              select escola;

                sql = from escola in ret
                      orderby escola.no_pessoa
                      select new PessoaSearchUI
                      {
                          cd_pessoa = escola.cd_pessoa,
                          nm_cpf_cgc = escola.dc_num_cgc,
                          dt_cadastramento = escola.dt_cadastramento,
                          no_pessoa = escola.no_pessoa,
                          dc_reduzido_pessoa = escola.dc_reduzido_pessoa
                      };

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

        #endregion
        public DataTable getLoginEscola(DateTime dt_analise, bool id_login, byte id_matricula)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringReportProcedure"].ConnectionString;
            string providerName = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringReportProcedure"].ProviderName;


            DataTable dtReportData = new DataTable();

            DBHelper dbSql = new DBHelper(connectionString, providerName);

            DBParameter param1 = new DBParameter("@dt_analise", dt_analise, DbType.DateTime);
            DBParameter param2 = new DBParameter("@id_login", id_login, DbType.Boolean);
            DBParameter param3 = new DBParameter("@id_matricula", id_matricula, DbType.Byte);

            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(param1);
            paramCollection.Add(param2);
            paramCollection.Add(param3);

            dtReportData = dbSql.ExecuteDataTable("sp_login_escolas", paramCollection, CommandType.StoredProcedure);
            return dtReportData;
        }

    }
}

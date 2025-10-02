using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using FundacaoFisk.SGF.Web.Services.Usuario.Model;
using FundacaoFisk.SGF.Web.Services.Usuario.Comum;
using FundacaoFisk.SGF.Web.Services.Usuario.Business;
using FundacaoFisk.SGF.Web.Services.Usuario.DataAccess;
using Componentes.Utils;
using System.Text;
using System.Transactions;
using Componentes.Utils.Messages;
using log4net;
using System.Data.Entity;
using Componentes.GenericBusiness;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Usuario.Business
{
    public class PermissaoBusiness : IPermissaoBusiness
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(PermissaoBusiness));
        private IMenuDataAccess MenuDataAccess { get; set; }
        public IGrupoDataAccess DataAccessGrupo { get; set; }
        public ISysDireitoGrupoDataAccess DataAccessDireitoGrupo { get; set; }
        public ISysGrupoUsuarioDataAccess DataAccessGrupoUsuario { get; set; }

        public PermissaoBusiness(IMenuDataAccess dataAccessMenu, IGrupoDataAccess grupoDataAccess,
                                ISysDireitoGrupoDataAccess dataAccessDireitoGrupo, ISysGrupoUsuarioDataAccess dataAccessGrupoUsuario)
        {
            if (dataAccessMenu == null || grupoDataAccess == null || dataAccessDireitoGrupo == null || dataAccessGrupoUsuario == null)
            {
                throw new ArgumentNullException("repository");
            }
            MenuDataAccess = dataAccessMenu;
            DataAccessGrupo = grupoDataAccess;
            DataAccessDireitoGrupo = dataAccessDireitoGrupo;
            DataAccessGrupoUsuario = dataAccessGrupoUsuario;
        }

        public void configuraUsuario(int cdUsuario, int cd_empresa)
        {
            // Configura os codigos do usuário para auditorias dos DataAccess:
            ((SGFWebContext)this.MenuDataAccess.DB()).IdUsuario = ((SGFWebContext)this.DataAccessGrupo.DB()).IdUsuario = cdUsuario;
            ((SGFWebContext)this.MenuDataAccess.DB()).cd_empresa = ((SGFWebContext)this.DataAccessGrupo.DB()).cd_empresa = cd_empresa;
        }

        public void sincronizaContexto(DbContext db)
        {
            //this.MenuDataAccess.sincronizaContexto(db);
            //this.DataAccessGrupo.sincronizaContexto(db);
            //this.DataAccessDireitoGrupo.sincronizaContexto(db);
        }

        #region Permissões
        public IEnumerable<SysMenuUI> GetMenusUsuario(int? codEscola, int cd_usuario, bool masterGeral)
        {
            IEnumerable<SysMenuUI> retorno = new List<SysMenuUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = MenuDataAccess.GetMenusUsuario(codEscola, cd_usuario, masterGeral);
                transaction.Complete();
            }
            return retorno;
        }

        public string GetFuncionalidadesUsuario(string login, bool eh_master_geral, int? cd_escola, int cd_usuario, bool eh_master)
        {
            //UsuarioWeb usuario = UsuarioDataAccess.GetUsuarioByLogin(login).FirstOrDefault<UsuarioWeb>();

            IEnumerable<SysMenu> permissoesMenu = MenuDataAccess.GetFuncionalidadesUsuario(cd_escola, cd_usuario, eh_master);
            string permissoes = "";

            permissoes = GetFuncionalidadesUsuario(cd_usuario, eh_master, permissoes, permissoesMenu, eh_master_geral);

            // Remove a última barra, se ela existir:
            if (permissoes.Length > 0)
                permissoes = permissoes.Substring(0, permissoes.Length - 1);

            return permissoes;
        }
        public List<SysMenu> GetFuncionalidadesGrupoArvore(int? cdGrupo)
        {
            return MenuDataAccess.GetFuncionalidadesGrupoArvore(cdGrupo);
        }

        public List<SysMenu> GetFuncionalidadesUsuarioArvore(int? cdUsuario)
        {
            return MenuDataAccess.GetFuncionalidadesUsuarioArvore(cdUsuario);
        }

        private string GetFuncionalidadesUsuario(int cd_usuario, bool eh_master, string permissoes, IEnumerable<SysMenu> permissoesMenu, bool eh_master_geral)
        {
            IEnumerator<SysMenu> enumerator = permissoesMenu.GetEnumerator();

            while (enumerator.MoveNext() && enumerator.Current != null)
            {
                if (!string.IsNullOrEmpty(enumerator.Current.dc_menu))
                    permissoes += enumerator.Current.dc_menu + "|";
                List<SysDireitoGrupo> listaPermGrupo = enumerator.Current.SysDireitosGrupos.ToList();
                List<SysDireitoUsuario> listaPermUsuario = enumerator.Current.Direitos.ToList();
                int menuEspecial = MenuDataAccess.GetMenuEspecial();
                // Adiciona todas as permissões para o administrador:
                if (eh_master)
                {
                    if (eh_master_geral)
                        permissoes += enumerator.Current.dc_menu + ".a|" + enumerator.Current.dc_menu + ".i|" + enumerator.Current.dc_menu + ".e|";
                    else if (enumerator.Current.id_permissao_visivel || enumerator.Current.cd_menu_pai == menuEspecial)
                    {
                        permissoes += enumerator.Current.dc_menu + ".a|" + enumerator.Current.dc_menu + ".i|" + enumerator.Current.dc_menu + ".e|";
                    }
                }
                else
                {
                    // Adiciona as permissões configuradas pelo grupo de usuarios:
                    for (int i = 0; i < listaPermGrupo.Count; i++)
                    {
                        if (listaPermGrupo[i].id_alterar_grupo)
                            permissoes += enumerator.Current.dc_menu + ".a|";
                        if (listaPermGrupo[i].id_inserir_grupo)
                            permissoes += enumerator.Current.dc_menu + ".i|";
                        if (listaPermGrupo[i].id_excluir_grupo)
                            permissoes += enumerator.Current.dc_menu + ".e|";
                    }
                    // Adiciona as permissões configuradas do usuario:
                    for (int i = 0; i < listaPermUsuario.Count; i++)
                    {
                        if (listaPermUsuario[i].id_alterar)
                            permissoes += enumerator.Current.dc_menu + ".a|";
                        if (listaPermUsuario[i].id_inserir)
                            permissoes += enumerator.Current.dc_menu + ".i|";
                        if (listaPermUsuario[i].id_deletar)
                            permissoes += enumerator.Current.dc_menu + ".e|";
                    }
                }
            }
            return permissoes;
        }

        #endregion

        #region Grupo

        public IEnumerable<SysGrupo> getSysGrupoByCdUsuario(int cdUsuario, int cd_empresa)
        {
            return DataAccessGrupo.getSysGrupoByCdUsuario(cdUsuario, cd_empresa);
        }

        public SysGrupo GetGrupoById(int id)
        {
            return DataAccessGrupo.findById(id, false);
        }

        //Para ususarios da escola
        public SysGrupo PostGrupo(SysGrupo grupo)
        {
            // Salva os grupos com as permissões e com os usuários
            DataAccessGrupo.addGrupo(grupo);
            return grupo;
        }

        public SysGrupo PutGrupo(SysGrupo grupo)
        {
            if (grupo != null)
            {
                SysGrupo grupoContext = DataAccessGrupo.GetGrupoById(grupo.cd_grupo, grupo.cd_pessoa_empresa);
                if (grupoContext != null)
                {
                    grupoContext.no_grupo = grupo.no_grupo;
                    grupoContext.id_atualizar_grupo = grupo.id_atualizar_grupo;

                    DataAccessGrupo.saveChanges(false);

                    if (grupo.alteraDireito)
                    {
                        //Adicionando ou Excluindo Direito Grupo
                        IEnumerable<SysDireitoGrupo> direitoDeleted = null;
                        if (grupoContext.SysDireitoGrupo != null)
                            direitoDeleted = grupoContext.SysDireitoGrupo.Where(tc => !grupo.SysDireitoGrupo.Any(tv => tc.cd_menu == tv.cd_menu));


                        if (direitoDeleted != null && grupoContext.SysDireitoGrupo != null)
                        {
                            var SysDireitoGrupoCopy = new List<SysDireitoGrupo>();
                            SysDireitoGrupoCopy = grupoContext.SysDireitoGrupo.ToList().cloneList().ToList();

                            foreach (var itemDireito in SysDireitoGrupoCopy)
                            {
                                if (itemDireito != null)
                                {
                                    var direito = DataAccessDireitoGrupo.findGrupoDireito(grupo.cd_grupo, itemDireito.cd_menu);
                                    if (direito != null && direito.cd_direito_grupo > 0)
                                        DataAccessDireitoGrupo.delete(direito, false);
                                }
                            }
                        }
                        if (grupo.SysDireitoGrupo != null)
                            foreach (var itemDireito in grupo.SysDireitoGrupo)
                            {
                                if (itemDireito != null)
                                {
                                    var direito = DataAccessDireitoGrupo.findGrupoDireito(grupo.cd_grupo, itemDireito.cd_menu);
                                    if (direito == null)
                                        DataAccessDireitoGrupo.add(itemDireito, false);
                                }
                            }
                    }
                    //Adicionando ou Excluindo Usuario Grupo
                    IEnumerable<SysGrupoUsuario> usuarioDeleted = null;
                    if (grupoContext.Usuarios != null)
                        usuarioDeleted = grupoContext.Usuarios.Where(tc => !grupo.Usuarios.Any(tv => tc.cd_usuario == tv.cd_usuario)).ToList(); 

                    if (usuarioDeleted != null)
                        foreach (var item in usuarioDeleted)
                        {
                            if (item != null)
                            {
                                var usuario = DataAccessGrupoUsuario.findGrupoUsuario(grupo.cd_grupo, item.cd_usuario);
                                if (usuario != null)
                                    DataAccessGrupoUsuario.delete(usuario, false);
                            }
                        }
                    if (grupo.Usuarios != null)
                        foreach (var itemUsu in grupo.Usuarios)
                        {
                            if (itemUsu != null)
                            {
                                var usuario = DataAccessGrupoUsuario.findGrupoUsuario(grupo.cd_grupo, itemUsu.cd_usuario);
                                if (usuario == null)
                                    DataAccessGrupoUsuario.add(itemUsu, false);
                            }
                        }
                    grupo.cd_grupo_master = grupoContext.cd_grupo_master;
                }
            }
            return grupo;
        }

        public bool DeleteGrupo(List<SysGrupo> grupos)
        {
            //bool deletado = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                //deletado = DataAccessGrupo.DeleteGrupo(grupos);
                foreach (var grupo in grupos)
                {
                    var grupoDeletar = DataAccessGrupo.findById(grupo.cd_grupo, false);
                    DataAccessGrupo.delete(grupoDeletar, false);
                }
                DataAccessGrupo.saveChanges(false);
                transaction.Complete();
            }
            //return deletado;
            return true;
        }

        #region Crud grupo Master

        public SysGrupo postGrupoEscolas(SysGrupo grupo)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, null, TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                grupo.cd_pessoa_empresa = null;
                grupo.id_atualizar_grupo = false;

                SysGrupo newGrupo = DataAccessGrupo.addContext(grupo, false);

                DataAccessGrupo.saveChanges(false);

                transaction.Complete();
            }
            return grupo;
        }

        public SysGrupo editarGrupoEscola(SysGrupo grupo, List<Permissao> permissoes)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, null, TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                DataAccessDireitoGrupo.sincronizaContexto(DataAccessGrupo.DB());
                List<SysDireitoGrupo> listDireitosAlterados = new List<SysDireitoGrupo>();
                if (grupo.alteraDireito)
                {
                    //Grupos filhos com o flag atualizar marcado  e que não tennha usuarios
                    List<SysGrupo> sysGrupoFilhoBase = DataAccessGrupo.getGrupoFilhoById(grupo.cd_grupo, false).ToList();

                    //Grupos filhos com flag usuarios e que não usuarios
                    List<SysGrupo> sysGrupoFilhoBaseComUsuario = DataAccessGrupo.getGrupoFilhoById(grupo.cd_grupo, true).ToList();

                    //Grupos filhos com o flag atualizar desmarcado
                    List<SysGrupo> sysGrupoFilhoDesmarcados = DataAccessGrupo.getGrupoFilhoDesmarcados(grupo.cd_grupo, false).ToList();

                    //Grupos filhos novos
                    List<SysGrupo> gruposFilhosVW = grupo.SysGrupoFilho.Where(g => g.cd_grupo == 0
                                                                                  && !sysGrupoFilhoDesmarcados.Any(gu => gu.cd_pessoa_empresa == g.cd_pessoa_empresa)
                                                                                  && !sysGrupoFilhoBase.Any(gg => gg.cd_pessoa_empresa == g.cd_pessoa_empresa)).ToList();

                    List<SysGrupo> sysGrupoFilhosCopy = new List<SysGrupo>();

                    sysGrupoFilhosCopy = sysGrupoFilhoBase.cloneList().ToList();

                    //Deleta todos os grupos filhos id_atualizar_grupo marcado
                    for (int i = 0; i < sysGrupoFilhoBase.Count(); i++)
                    {
                        if (sysGrupoFilhoBase[i].id_atualizar_grupo && sysGrupoFilhoBase[i].cd_grupo > 0)
                        {
                            sysGrupoFilhoBase[i].SysDireitoGrupo = null;
                            DataAccessGrupo.delete(sysGrupoFilhoBase[i], false);
                        }
                    }

                    //Atualiza a chave cd_grupo_master para null, dessa forma o pai poderá ser deletado
                    for (int i = 0; i < sysGrupoFilhoDesmarcados.Count(); i++)
                        sysGrupoFilhoDesmarcados[i].cd_grupo_master = null;

                    for (int i = 0; i < sysGrupoFilhoBaseComUsuario.Count(); i++)
                        sysGrupoFilhoBaseComUsuario[i].cd_grupo_master = null;

                    //Adciona os novos filhos para poder inserir na base
                    for (int i = 0; i < gruposFilhosVW.Count(); i++)
                        sysGrupoFilhosCopy.Add(new SysGrupo
                        {
                            cd_pessoa_empresa = gruposFilhosVW[i].cd_pessoa_empresa,
                            id_atualizar_grupo = true,
                            no_grupo = grupo.no_grupo,
                            cd_grupo_master = grupo.cd_grupo
                        });

                    //Monta as permisões para os filhos
                    grupo.SysGrupoFilho = GrupoPermissao.montarGruposPermissoesFilhos(sysGrupoFilhosCopy, permissoes, grupo.no_grupo);

                    SysGrupo grupoDelete = DataAccessGrupo.getGruposMasterById(grupo.cd_grupo);
                    grupoDelete.cd_pessoa_empresa = null;

                    DataAccessGrupo.deleteContext(grupoDelete, false);
                    DataAccessGrupo.saveChanges(false);

                    grupo.cd_pessoa_empresa = null;
                    SysGrupo grupoNovo = DataAccessGrupo.addContext(grupo, false);
                    DataAccessGrupo.saveChanges(false);

                    //Atualiza os pais para os filhos
                    for (int i = 0; i < sysGrupoFilhoDesmarcados.Count(); i++)
                        sysGrupoFilhoDesmarcados[i].cd_grupo_master = grupoNovo.cd_grupo;

                    DataAccessGrupo.saveChanges(false);
                    for (int i = 0; i < sysGrupoFilhoBaseComUsuario.Count(); i++)
                    {
                        sysGrupoFilhoBaseComUsuario[i].no_grupo = grupoNovo.no_grupo;
                        sysGrupoFilhoBaseComUsuario[i].cd_grupo_master = grupoNovo.cd_grupo;

                        List<SysDireitoGrupo> direitoGrupoFilho = sysGrupoFilhoBaseComUsuario[i].SysDireitoGrupo.ToList();

                        for (int h = 0; h < direitoGrupoFilho.Count(); h++)
                            DataAccessDireitoGrupo.deleteContext(direitoGrupoFilho[h], false);

                        DataAccessDireitoGrupo.saveChanges(false);

                        List<SysDireitoGrupo> direitosGrupoPai = grupoNovo.SysDireitoGrupo.ToList();
                       
                        List<SysDireitoGrupo> sysDireitosF = new List<SysDireitoGrupo>();
                        
                        for (int h = 0; h < direitosGrupoPai.Count(); h++)
                        {
                            sysDireitosF.Add(new SysDireitoGrupo
                            {
                                id_alterar_grupo = direitosGrupoPai[h].id_alterar_grupo,
                                id_excluir_grupo = direitosGrupoPai[h].id_excluir_grupo,
                                id_inserir_grupo = direitosGrupoPai[h].id_inserir_grupo,
                                cd_menu = direitosGrupoPai[h].cd_menu,
                                cd_direito_grupo = 0,
                                cd_grupo = sysGrupoFilhoBaseComUsuario[i].cd_grupo
                            });
                        }
                        sysGrupoFilhoBaseComUsuario[i].SysDireitoGrupo = sysDireitosF;
                        DataAccessDireitoGrupo.saveChanges(false);

                    }                 

                    //crudSysDireitoGrupo(grupo.SysDireitoGrupo.ToList(), grupo);
                }
                else
                {
                    SysGrupo grupoContext = DataAccessGrupo.getGruposMasterById(grupo.cd_grupo);
                    grupoContext.no_grupo = grupo.no_grupo;

                    //Recupera os grupos filhos da view
                    ICollection<SysGrupo> gruposFilhosVW = grupo.SysGrupoFilho;
                    ICollection<SysGrupo> gruposFilhosBD = DataAccessGrupo.getGrupoFilhosSearch(grupo.cd_grupo).ToList();

                    foreach (var item in gruposFilhosBD)
                        if (item.cd_grupo > 0 && item.id_atualizar_grupo == true)
                            item.no_grupo = grupo.no_grupo;

                    if (gruposFilhosVW != null)
                    {
                        List<SysGrupo> gruposFilhoInterar = gruposFilhosVW.Where(gf => gf.cd_grupo == 0).ToList();
                        
                        if (gruposFilhoInterar != null)
                        {
                            foreach (var gruposFilhos in gruposFilhoInterar)
                            {
                                gruposFilhos.cd_grupo_master = grupo.cd_grupo;
                                gruposFilhos.no_grupo = grupo.no_grupo;
                                List<SysDireitoGrupo> direitosFilhos = DataAccessDireitoGrupo.findGrupoDireito(grupo.cd_grupo).ToList();

                                List<SysDireitoGrupo> sysDireitosF = new List<SysDireitoGrupo>();

                                if (gruposFilhos.cd_grupo == 0)
                                {
                                    for (int i = 0; i < direitosFilhos.Count(); i++)
                                    {
                                        sysDireitosF.Add(new SysDireitoGrupo
                                        {
                                            id_alterar_grupo = direitosFilhos[i].id_alterar_grupo,
                                            id_excluir_grupo = direitosFilhos[i].id_excluir_grupo,
                                            id_inserir_grupo = direitosFilhos[i].id_inserir_grupo,
                                            cd_menu = direitosFilhos[i].cd_menu,
                                            cd_direito_grupo = 0,
                                            cd_grupo = 0
                                        });
                                    }

                                    gruposFilhos.SysDireitoGrupo = sysDireitosF;

                                    DataAccessGrupo.addContext(gruposFilhos, false);
                                    DataAccessGrupo.saveChanges(false);
                                } 
                            }
                        }

                        // Filtra as grupos que possuem código
                        IEnumerable<SysGrupo> gruposFilhosWithCD = gruposFilhosVW.Where(at => at.cd_grupo != 0);
                        IEnumerable<SysGrupo> gruposForDeleted = gruposFilhosBD.Where(atv => !gruposFilhosWithCD.Any(a => atv.cd_grupo == a.cd_grupo));

                        //Deleta o registro que esta na base mas não esta na view.
                        if (gruposForDeleted.Count() > 0)
                        {
                            foreach (var item in gruposForDeleted)
                            {
                                SysGrupo deletarGrupo = DataAccessGrupo.GetGrupoById(item.cd_grupo, item.cd_pessoa_empresa);

                                if ((deletarGrupo != null) && deletarGrupo.id_atualizar_grupo == true && deletarGrupo.Usuarios.Any(us => us.cd_grupo_usuario > 0 && us.cd_grupo == item.cd_grupo))
                                    throw new PermissaoBusinessException(Messages.msgGrupoUsadoEscola, null, PermissaoBusinessException.TipoErro.ERRO_GRUPO_MASTER, false);

                                bool deleted = deletarGrupo != null ? DataAccessGrupo.delete(deletarGrupo, false) : false;
                            }
                        }
                    }
                    else
                    {
                        //se não existir nenhum resgistro na view, significa que todos os grupos serão deletados.
                        if (gruposFilhosBD != null)
                        {
                            foreach (var grupoFilho in gruposFilhosBD)
                            {
                                SysGrupo deletarGrupoFilho = DataAccessGrupo.GetGrupoById(grupoFilho.cd_grupo, grupoFilho.cd_pessoa_empresa);

                                if ((deletarGrupoFilho != null) && deletarGrupoFilho.id_atualizar_grupo == true && deletarGrupoFilho.Usuarios.Any(us => us.cd_grupo_usuario > 0))
                                    throw new PermissaoBusinessException(Messages.msgGrupoUsadoEscola, null, PermissaoBusinessException.TipoErro.ERRO_GRUPO_MASTER, false);

                                bool deleted = deletarGrupoFilho != null ? DataAccessGrupo.delete(deletarGrupoFilho, false) : false;
                            }
                        }
                    }

                    DataAccessGrupo.saveChanges(false);
                }
                transaction.Complete();
            }
            return grupo;
        }

        public bool deleteGrupoEscola(List<SysGrupo> grupos)
        {
            bool deletado = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                for (int i = 0; i < grupos.Count(); i++)
                {
                    //Se não existir dependentes para o grupo pai, todos o filhos não terão mais a referência do pai.
                    List<SysGrupo> gruposFilhos = DataAccessGrupo.getGrupoFilhosSearch(grupos[i].cd_grupo).ToList();

                    if ((gruposFilhos != null) && gruposFilhos.Any(gf => gf.Usuarios.Any(u => u.cd_grupo_usuario > 0 && u.cd_grupo == gf.cd_grupo) && gf.id_atualizar_grupo))
                        throw new PermissaoBusinessException(Messages.msgGrupoUsadoEscola, null, PermissaoBusinessException.TipoErro.ERRO_GRUPO_MASTER, false);

                    for (int j = 0; j < gruposFilhos.Count(); j++)
                        if (gruposFilhos[j].id_atualizar_grupo)
                            DataAccessGrupo.deleteContext(gruposFilhos[j], false);

                    DataAccessGrupo.saveChanges(false);

                    SysGrupo grupoMaster = DataAccessGrupo.findById(grupos[i].cd_grupo, false);
                    deletado = DataAccessGrupo.delete(grupoMaster, false);
                }
                transaction.Complete();
            }
            return deletado;
        }

        #endregion

        public void DeletePermissaoGrupo(SysGrupo grupo)
        {
            DataAccessGrupo.DeletePermissaoGrupo(grupo);
        }

        public IEnumerable<SysGrupo> GetGrupoSearch(SearchParameters parametros, string descricao, bool inicio, int? cd_pessoa_escola, int tipoGrupo)
        {
            IEnumerable<SysGrupo> retorno = new List<SysGrupo>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_grupo";
                retorno = DataAccessGrupo.GetGrupoSearch(parametros, descricao, inicio, cd_pessoa_escola, tipoGrupo);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<SysGrupo> GetGrupoSearch(int cd_pessoa_escola)
        {
            return DataAccessGrupo.GetGrupoSearch(cd_pessoa_escola);
        }

        #endregion

        public void postSysDireitoGrupo(ICollection<SysDireitoGrupo> direitogrupo, SysGrupo grupo)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                SysGrupo grupoOld = DataAccessGrupo.GetGrupoById(grupo.cd_grupo, grupo.cd_pessoa_empresa);
                if (grupo.SysDireitoGrupo.Count() <= 0 || grupo.SysDireitoGrupo == null)
                    grupo.SysDireitoGrupo = grupoOld.SysDireitoGrupo;
                else
                    DataAccessGrupo.DeletePermissaoGrupo(grupo);

                DataAccessGrupo.DeleteUsuarioGrupo(grupo);
                DataAccessGrupo.editGrupo(grupo);

                transaction.Complete();
            }
        }

        public IEnumerable<SysGrupo> GetGrupoFilhoById(int cd_grupo_master, bool comDireito)
        {
            return DataAccessGrupo.getGrupoFilhoById(cd_grupo_master, comDireito);
        }

        public IEnumerable<SysDireitoGrupo> findAllGrupoDireito(int[] cdGrupos)
        {
            return DataAccessDireitoGrupo.findAllGrupoDireito(cdGrupos);
        }
    }
}
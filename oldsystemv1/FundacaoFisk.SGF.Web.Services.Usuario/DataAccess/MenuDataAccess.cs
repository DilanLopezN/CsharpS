using System.Linq;
using System.Collections.Generic;
using FundacaoFisk.SGF.Web.Services.Usuario.Comum;
using FundacaoFisk.SGF.Web.Services.Usuario.Model;
using Componentes.GenericDataAccess.GenericRepository;
using System.Data.Entity;
using System;
using Componentes.GenericDataAccess.GenericException;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Usuario.DataAccess
{
    public class MenuDataAccess : GenericRepository<SysMenu>, IMenuDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        // Método utilizado para buscar os menus de um determinado usuário.
        public IEnumerable<SysMenuUI> GetMenusUsuario(int? codEscola, int? codUsuario, bool administrador)
        {
            try{
                List<SysMenu> enumMenu = new List<SysMenu>();
                var param = from p in db.Parametro
                            where p.cd_pessoa_escola == codEscola
                            select new
                            {
                                p.id_baixa_automatica_cartao,
                                p.id_baixa_automatica_cheque,
                                cd_menu_cartao = (from m in db.SysMenu where m.dc_menu == "bautcar" select m.cd_menu).FirstOrDefault(),
                                cd_menu_cheque = (from m in db.SysMenu where m.dc_menu == "bautch" select m.cd_menu).FirstOrDefault(),
                                cd_menu_pai = (from m in db.SysMenu where m.dc_menu == "bautch" select m.cd_menu_pai).FirstOrDefault()
                            };

                var result1 = from m in db.SysMenu.Include(p => p.MenusFilhos)
                              where m.id_menu_visivel == true &&
                                  param.Where(x => ((x.id_baixa_automatica_cartao && x.id_baixa_automatica_cheque) ||
                                  (x.id_baixa_automatica_cheque && !x.id_baixa_automatica_cartao && x.cd_menu_cartao != m.cd_menu) ||
                                  (x.id_baixa_automatica_cartao && !x.id_baixa_automatica_cheque && x.cd_menu_cheque != m.cd_menu) ||
                                  (!x.id_baixa_automatica_cartao && !x.id_baixa_automatica_cheque && x.cd_menu_pai != m.cd_menu))).Any()
                              select m;
                
                if(!administrador){
                    var result2 = (from r in result1
                              where (r.Direitos.Where(sysDU => sysDU.cd_usuario == codUsuario ||codUsuario == null).Any()
                                        || r.SysDireitosGrupos.Where(sysDG => sysDG.SysGrupo.cd_pessoa_empresa == codEscola && sysDG.SysGrupo.Usuarios.Where(u => u.cd_usuario == codUsuario || codUsuario == null).Any()).Any())
                              orderby r.nm_ordem
                              select new {
                                  SysMenu = r,
                                  MenusFilhos = r.MenusFilhos.Where(mf => mf.id_menu_visivel == true &&
                                       param.Where(x =>
                                          ((x.id_baixa_automatica_cartao && x.id_baixa_automatica_cheque) ||
                                           (x.id_baixa_automatica_cheque && !x.id_baixa_automatica_cartao && x.cd_menu_cartao != mf.cd_menu) ||
                                           (x.id_baixa_automatica_cartao && !x.id_baixa_automatica_cheque && x.cd_menu_cheque != mf.cd_menu))).Any()
                                      && (mf.Direitos.Where(sysDU => sysDU.cd_usuario == codUsuario).Any()
                                        || mf.SysDireitosGrupos.Where(sysDG => sysDG.SysGrupo.cd_pessoa_empresa == codEscola && sysDG.SysGrupo.Usuarios.Where(u => u.cd_usuario == codUsuario).Any()).Any())).OrderBy(mf => mf.nm_ordem)
                              });

                    if(result2.Count() > 0)
                        foreach(var c in result2)
                            enumMenu.Add(c.SysMenu);
                } 
                else {
                    var result3 = (from m in result1
                                  orderby m.nm_ordem
                                  select new {
                                      SysMenu = m,
                                      MenusFilhos = m.MenusFilhos.Where(mf => mf.id_menu_visivel == true &&
                                       param.Where(x =>
                                          ((x.id_baixa_automatica_cartao && x.id_baixa_automatica_cheque) ||
                                           (x.id_baixa_automatica_cheque && !x.id_baixa_automatica_cartao && x.cd_menu_cartao != mf.cd_menu) ||
                                           (x.id_baixa_automatica_cartao && !x.id_baixa_automatica_cheque && x.cd_menu_cheque != mf.cd_menu))).Any()
                                      ).OrderBy(mf => mf.nm_ordem)
                                  }).ToList();
                    if(result3.Count() > 0)
                        foreach(var c in result3)
                            enumMenu.Add(c.SysMenu);
                }

                return (from r in enumMenu
                        where r.cd_menu_pai == null
                        select new SysMenuUI
                        {
                            cd_menu = r.cd_menu,
                            dc_url_menu = r.dc_url_menu,
                            id_separador = r.id_separador,
                            no_menu = r.no_menu,
                            MenusFilhos = reduzMenuRecursivo(r.MenusFilhos.ToList()).ToList()
                        });
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        private IEnumerable<SysMenuUI> reduzMenuRecursivo(IEnumerable<SysMenu> menus)
        {
            if (menus.Count() > 0)
            {
                List<SysMenuUI> listaMenus = new List<SysMenuUI>();
                foreach (SysMenu sysMenu in menus)
                {
                    SysMenuUI menuUI = new SysMenuUI() {
                        cd_menu = sysMenu.cd_menu,
                        dc_url_menu = sysMenu.dc_url_menu,
                        id_separador = sysMenu.id_separador,
                        no_menu = sysMenu.no_menu
                    };
                    if (sysMenu.MenusFilhos.Count > 0)
                        menuUI.MenusFilhos = reduzMenuRecursivo(sysMenu.MenusFilhos.ToList()).ToList();
                    else
                        menuUI.MenusFilhos = new List<SysMenuUI>();
                    listaMenus.Add(menuUI);
                }
                return listaMenus;
            }
            return new List<SysMenuUI>();
        }

        // Método utilizado para buscar as permissões de determinado usuário:
        public List<SysMenu> GetFuncionalidadesUsuario(int? codEscola, int codUsuario, bool administrador)
        {
            try{
                List<SysMenu> enumMenu = new List<SysMenu>();

                var result = from m in db.SysMenu.Include(p => p.MenusFilhos)
                              where m.dc_menu != null
                              select m;

                if(!administrador) {
                    var result2 = from m in result
                                  where (m.Direitos.Where(sysDU => sysDU.cd_usuario == codUsuario).Any()
                                        || m.SysDireitosGrupos.Where(sysDG => codEscola.HasValue && sysDG.SysGrupo.cd_pessoa_empresa == codEscola
                                                && sysDG.SysGrupo.Usuarios.Where(u => u.cd_usuario == codUsuario).Any()).Any())
                                  select new
                                  {
                                      SysMenu = m,
                                      SysDireitosUsuario = m.Direitos.Where(sysDU => sysDU.cd_usuario == codUsuario),
                                      SysDireitoGrupo = m.SysDireitosGrupos.Where(sysDG => codEscola.HasValue && sysDG.SysGrupo.cd_pessoa_empresa == codEscola
                                                && sysDG.SysGrupo.Usuarios.Where(u => u.cd_usuario == codUsuario).Any())
                                  };
                    if(result2.Count() > 0)
                        foreach(var c in result2)
                            enumMenu.Add(c.SysMenu);
                }
                else
                    enumMenu = result.ToList();

                return enumMenu;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        // Método que busca todas as permissões do sistema com as atribuições do usuário, caso as atribuições já existam.
        // Não é preciso buscar as funcionalidades do administrador, pois não será editável as permissões para o adm.
        public List<SysMenu> GetFuncionalidadesUsuarioArvore(int? cdUsuario) {
            try{
                var result = from m in db.SysMenu.Include(p => p.MenusFilhos).Include(sysDU => sysDU.Direitos)
                             //where m.id_permissao_visivel == true
                             orderby m.nm_ordem
                             select new {
                                 SysMenu = m,
                                 SysDireitosUsuario = m.Direitos.Where(sysDU => sysDU.cd_usuario == cdUsuario.Value || cdUsuario == null),
                                 SysDireitoGrupo = m.SysDireitosGrupos.Where(sysDG => sysDG.SysGrupo.Usuarios.Where(u => u.cd_usuario == cdUsuario.Value || cdUsuario == null).Any())
                             };
                
                result.ToList(); // Força a execução do sql para pegar os dados em estrutura de árvore;
                
                var result2 = (from r in result
                              where r.SysMenu.cd_menu_pai == null
                              orderby r.SysMenu.nm_ordem
                              select r);

                List<SysMenu> enumMenu = new List<SysMenu>();

                if(result2.Count() > 0)
                    foreach(var c in result2) 
                        enumMenu.Add(c.SysMenu);

                enumMenu = (from m in enumMenu
                            orderby m.nm_ordem
                            select m).ToList();

                return enumMenu;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        // Método que busca todas as permissões do sistema com as atribuições do Grupo, caso as atribuições já existam.
        // Não é preciso buscar as funcionalidades do administrador, pois não será editável as permissões para o adm.
        public List<SysMenu> GetFuncionalidadesGrupoArvore(int? cdGrupo)
        {
            try{
                var result = from m in db.SysMenu.Include(p => p.MenusFilhos).Include(sysDU => sysDU.SysDireitosGrupos)
                             //where m.id_permissao_visivel == true
                             orderby m.nm_ordem
                             select new
                             {
                                 SysMenu = m,
                                 SysDireitoGrupo = m.SysDireitosGrupos.Where(sysDG => sysDG.cd_grupo == cdGrupo)
                             };

                result.ToList(); // Força a execução do sql para pegar os dados em estrutura de árvore;

                var result2 = (from r in result
                               where r.SysMenu.cd_menu_pai == null
                               orderby r.SysMenu.nm_ordem
                               select r);

                List<SysMenu> enumMenu = new List<SysMenu>();

                if (result2.Count() > 0)
                    foreach (var c in result2)
                        enumMenu.Add(c.SysMenu);

                enumMenu = (from m in enumMenu
                            orderby m.nm_ordem
                            select m).ToList();

                return enumMenu;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public int GetMenuEspecial()
        {
            try
            {
                int result = (from m in db.SysMenu
                    where m.no_menu == "Especiais"
                    select m.cd_menu).FirstOrDefault();

                return result;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
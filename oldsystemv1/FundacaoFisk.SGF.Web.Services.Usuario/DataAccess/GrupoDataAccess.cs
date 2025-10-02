using System;
using System.Linq;
using System.Collections.Generic;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Usuario.Comum;
using FundacaoFisk.SGF.Web.Services.Usuario.Model;
using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericRepository;
using System.Data.Entity;
using Componentes.GenericDataAccess.GenericException;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Usuario.DataAccess
{
    public class GrupoDataAccess : GenericRepository<SysGrupo>, IGrupoDataAccess
    {
        //Constantes

        const int GRUPO = 0;

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }
        
       public bool DeletePermissaoGrupo(SysGrupo grupo)
        {
           try{

                bool retorno = true;
                var qtd = db.SysDireitoGrupo.Count(g => g.cd_grupo == grupo.cd_grupo);
                //Pega os que tiverem marcado no Banco de dados
                for (var i = 0; i < qtd; i++)
                {
                    db.SysDireitoGrupo.Remove(db.SysDireitoGrupo.FirstOrDefault(g => g.cd_grupo == grupo.cd_grupo));
                    retorno = retorno & db.SaveChanges() > 0;
                }
                return retorno;
           }
           catch (Exception exe)
           {
               throw new DataAccessException(exe);
           }
        }
       public bool DeleteUsuarioGrupo(SysGrupo grupo)
       {
           try{
               bool retorno = true;
               var usu = (from d in db.SysGrupo.Include(u => u.Usuarios)
                          where d.cd_grupo == grupo.cd_grupo
                          select d).FirstOrDefault();

               foreach (SysGrupoUsuario ed in usu.Usuarios.ToList())
                   retorno = retorno & usu.Usuarios.Remove(ed);
               return retorno;
           }
           catch (Exception exe)
           {
               throw new DataAccessException(exe);
           }

       }

       public bool DeleteGrupo(List<SysGrupo> grupos)
       {
           try{
                string strGrupos = "";
                if (grupos != null && grupos.Count > 0)
                    foreach (SysGrupo e in grupos)
                        strGrupos += e.cd_grupo + ",";

                // Remove o último ponto e virgula:
                if (strGrupos.Length > 0)
                    strGrupos = strGrupos.Substring(0, strGrupos.Length - 1);

                int retorno = db.Database.ExecuteSqlCommand("delete from t_sys_grupo where cd_grupo in(" + strGrupos + ")");

                return retorno > 0;
           }
           catch (Exception exe)
           {
               throw new DataAccessException(exe);
           }
       }

       public SysGrupo addGrupo(SysGrupo grupo)
       {
           try{
                // Prepara a lista de usuarios para criar o vínculo do usuario com o grupo:
                ICollection<UsuarioWebSGF> usuarios = new List<UsuarioWebSGF>();

                if(grupo.Usuarios != null)
                    foreach(SysGrupoUsuario usuario in grupo.Usuarios)
                        usuarios.Add(db.UsuarioWebSGF.Single(u => u.cd_usuario == usuario.cd_usuario));

                //grupo.Usuarios = usuarios;
                var newGrupo = db.SysGrupo.Add(grupo);
                db.SaveChanges();
                //base.add(newGrupo,false);
                return newGrupo;
           }
           catch (Exception exe)
           {
               throw new DataAccessException(exe);
           }
       }
       public SysGrupo editGrupo(SysGrupo grupo)
       {
           try{
                // Prepara a lista de usuarios para criar o vínculo do usuario com o grupo:
               // ICollection<UsuarioWeb> usuarios = new List<UsuarioWeb>();
               // ICollection<SysDireitoGrupo> direitosGrupo = new List<SysDireitoGrupo>();
              
               // SysGrupo grupoBase = base.findById(grupo.cd_grupo, false);


               // // Salva os dados do grupo:
               // grupoBase.no_grupo = grupo.no_grupo;
               ////////////////////////////
               ////// ICollection<SysGrupoUsuario> usuariosGrupo = new List<SysGrupoUsuario>();
               ////// foreach (SysGrupoUsuario u in grupo.Usuarios)
               //////     grupoBase.Usuarios.Add(u);

               ////// SysDireitoGrupo direito = new SysDireitoGrupo();
               ////// // Armazena os dados das relações do grupo:
               ////// if (grupo.SysDireitoGrupo != null)
               //////     foreach (SysDireitoGrupo direitoGrupo in grupo.SysDireitoGrupo)
               //////         direitosGrupo.Add(direitoGrupo);
               
               ////// grupoBase.SysDireitoGrupo = direitosGrupo;
               ////////////////////////////////

               ////// base.edit(grupoBase, false);

               ////// grupoBase = base.findById(grupo.cd_grupo, false);
               ////// //grupoBase.Usuarios = usuariosGrupo;

               grupo.Usuarios = null;
             //  grupo.SysDireitoGrupo = null;
                base.edit(grupo, false);

                return grupo;
           }
           catch (Exception exe)
           {
               throw new DataAccessException(exe);
           }
       }
       //retorna o primeiro registro ou o default
       public SysGrupo firstOrDefault()
       {
           try{
                var sql = (from grupo in db.SysGrupo
                           select grupo).FirstOrDefault();
                return sql;
           }
           catch (Exception exe)
           {
               throw new DataAccessException(exe);
           }
       }


       public List<SysGrupo> getGrupoByArray(int[] cdGrupos)
       {
           try{
                var sql = from c in db.SysGrupo
                          where (cdGrupos).Contains(c.cd_grupo)
                          select c;

                return sql.ToList();
           }
           catch (Exception exe)
           {
               throw new DataAccessException(exe);
           }
       }

       public SysGrupo GetGrupoById(int cdGrupo, int? cdEscola)
       {
           try{
                var sql = (from grupo in db.SysGrupo.Include(p => p.SysDireitoGrupo).Include(u => u.Usuarios)
                           where grupo.cd_grupo == cdGrupo &&
                           grupo.cd_pessoa_empresa == cdEscola
                           select grupo).FirstOrDefault();
                return sql;
           }
           catch (Exception exe)
           {
               throw new DataAccessException(exe);
           }
       }

       public IEnumerable<SysGrupo> GetGrupoSearch(SearchParameters parametros, string descricao, bool inicio, int? cd_pessoa_escola, int tipoGrupo)
       {
           try {

               IEntitySorter<SysGrupo> sorter = EntitySorter<SysGrupo>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
               IQueryable<SysGrupo> sql;

               sql = from grupo in db.SysGrupo.AsNoTracking()
                     where tipoGrupo == GRUPO ? grupo.cd_pessoa_empresa == cd_pessoa_escola
                                              : !grupo.cd_pessoa_empresa.HasValue
                         orderby grupo.no_grupo
                         select grupo;

               sql = sorter.Sort(sql);

               var retorno = from grupo in sql
                             select grupo;

               if(!String.IsNullOrEmpty(descricao))
                   if(inicio)
                       retorno = from grupo in sql
                                 where grupo.no_grupo.StartsWith(descricao)
                                 select grupo;
                   else
                       retorno = from grupo in sql
                                 where grupo.no_grupo.Contains(descricao)
                                 select grupo;
              

               int limite = retorno.Count();
               parametros.ajustaParametrosPesquisa(limite);
               retorno = retorno.Skip(parametros.from).Take(parametros.qtd_limite);

               parametros.qtd_limite = limite;

               return retorno;
           }
           catch(Exception exe) {
               throw new DataAccessException(exe);
           }
       }

       public IEnumerable<SysGrupo> getSysGrupoByCdUsuario(int cdUsuario, int cd_empresa) {
           var sql = from grupos in db.SysGrupo
                     where grupos.cd_pessoa_empresa == cd_empresa &&
                     (from user in grupos.Usuarios where user.cd_usuario == cdUsuario select user.cd_grupo).Any()
                     //(grupos.SysUsuario.Where(g => g.cd_usuario == cdUsuario).Any())
                     select grupos;
           return sql;
       }

       public IEnumerable<SysGrupo> GetGrupoSearch(int cd_pessoa_escola) {
           try {
              
               var sql = from grupo in db.SysGrupo
                     where grupo.cd_pessoa_empresa == cd_pessoa_escola 
                     orderby grupo.no_grupo
                     select grupo;

               return sql;
           }
           catch(Exception exe) {
               throw new DataAccessException(exe);
           }
       }

       public IEnumerable<SysGrupo> getGrupoFilhosSearch(int cd_grupo_pai)
       {
           try
           {
               var sql = from grupo in db.SysGrupo.Include(u => u.Usuarios)
                         where grupo.cd_grupo_master == cd_grupo_pai
                         orderby grupo.no_grupo
                         select grupo;
               return sql;
           }
           catch (Exception exe)
           {
               throw new DataAccessException(exe);
           }
       }

       public SysGrupo getGruposMasterById(int cdGrupo)
       {
           try
           {
               var sql = (from grupo in db.SysGrupo.Include(s => s.SysDireitoGrupo)
                          where grupo.cd_grupo == cdGrupo
                             && grupo.cd_pessoa_empresa == null
                          select grupo).FirstOrDefault();
               return sql;
           }
           catch (Exception exe)
           {
               throw new DataAccessException(exe);
           }
       }

        /// <summary>
       /// Retorna todos os grupos que tem um pai e o id_atualizar_grupo marcado
        /// </summary>
        /// <param name="cd_grupo_master"></param>
       /// <returns></returns>
       public IEnumerable<SysGrupo> getGrupoFilhoById(int cd_grupo_master, bool comDireito)
       {
           try
           {
               IQueryable<SysGrupo> sql;

               if (comDireito)
                   sql = from grupo in db.SysGrupo.Include(p => p.SysDireitoGrupo).Include(u => u.Usuarios)
                         where grupo.cd_grupo_master == cd_grupo_master
                            && grupo.cd_pessoa_empresa != null
                            && grupo.id_atualizar_grupo == true
                            && grupo.Usuarios.Any(gu => gu.cd_grupo == grupo.cd_grupo)
                         select grupo;
               else
                   sql = from grupo in db.SysGrupo
                         where grupo.cd_grupo_master == cd_grupo_master
                            && grupo.cd_pessoa_empresa != null
                            && grupo.id_atualizar_grupo == true
                            && !grupo.Usuarios.Any(gu => gu.cd_grupo == grupo.cd_grupo)
                         select grupo;

               return sql;
           }
           catch (Exception exe)
           {
               throw new DataAccessException(exe);
           }
       }

       /// <summary>
       /// Retorna todos os grupos que tem um pai e o id_atualizar_grupo desmarcado
       /// </summary>
       /// <param name="cd_grupo_master"></param>
       /// <returns></returns>
       public IEnumerable<SysGrupo> getGrupoFilhoDesmarcados(int cd_grupo_master, bool comDireito)
       {
           try
           {
               IQueryable<SysGrupo> sql;

               if (comDireito)
                   sql = from grupo in db.SysGrupo.Include(p => p.SysDireitoGrupo).Include(u => u.Usuarios)
                         where grupo.cd_grupo_master == cd_grupo_master
                            && grupo.cd_pessoa_empresa != null
                            && grupo.id_atualizar_grupo == false
                         select grupo;
               else
                   sql = from grupo in db.SysGrupo
                         where grupo.cd_grupo_master == cd_grupo_master
                            && grupo.cd_pessoa_empresa != null
                            && grupo.id_atualizar_grupo == false
                         select grupo;

               return sql;
           }
           catch (Exception exe)
           {
               throw new DataAccessException(exe);
           }
       }

       public SysGrupo GetGrupoMasterById(int cd_grupo)
       {
           try
           {
               var sql = (from grupo in db.SysGrupo.Include(s => s.SysDireitoGrupo).Include(s => s.SysGrupoFilho).Include(s => s.Usuarios)
                          where grupo.cd_grupo == cd_grupo 
                          select grupo).FirstOrDefault();
               return sql;
           }
           catch (Exception exe)
           {
               throw new DataAccessException(exe);
           }
       }
    }
}

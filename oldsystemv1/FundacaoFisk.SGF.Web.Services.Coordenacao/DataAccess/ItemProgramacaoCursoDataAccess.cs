using System;
using System.Collections.Generic;
using System.Linq;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum;
using Componentes.GenericModel;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using Componentes.GenericDataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao;
using System.Data.Entity;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using System.Data;
using Componentes.GenericDataAccess.GenericException;


namespace FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess
{
    public class ItemProgramacaoCursoDataAccess : GenericRepository<ItemProgramacao>, IItemProgramacaoCursoDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<ItemProgramacao> getCursoProg(int cdCurso, int cdDuracao, int? cd_escola)
        {
            try{
                var sql = from prog in db.ProgramacaoCurso
                          join item in db.ItemProgramacao
                          on prog.cd_programacao_curso equals item.cd_programacao_curso
                          join curso in db.Curso
                          on prog.cd_curso equals curso.cd_curso
                          join duracao in db.Duracao
                          on prog.cd_duracao equals duracao.cd_duracao
                          where 
                            curso.cd_curso == cdCurso &&
                            duracao.cd_duracao == cdDuracao //&&
                          //  prog.cd_regime == cdRegime
                          select item;

                if(cd_escola.HasValue)
                    sql = from s in sql
                          where s.ProgramacaoCurso.cd_escola == cd_escola.Value
                          select s;
                else
                    sql = from s in sql
                          where s.ProgramacaoCurso.cd_escola == null
                          select s;
                
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        
        public ItemProgramacao addItemProgramacao (ItemProgramacao itens){
            try{
                db.ItemProgramacao.Add(itens);
                db.SaveChanges();
                return itens;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public ItemProgramacao editItemProgramacao(ItemProgramacao item)
        {
            try{
                context.Entry(item).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                return item;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<ItemProgramacao> getItensProgramacaoCursoById(int cdProgramacao)
        {
            try{
                var sql = from prog in db.ProgramacaoCurso
                          join item in db.ItemProgramacao
                          on prog.cd_programacao_curso equals item.cd_programacao_curso
                          where prog.cd_programacao_curso == cdProgramacao
                          select item;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool deleteAllItemProgramacaoCurso(List<ItemProgramacao> itensProgramacao) {
            try {
                foreach(ItemProgramacao itemProgramacao in itensProgramacao) {
                    ItemProgramacao itemProgramacaoContext = this.findById(itemProgramacao.cd_item_programacao, false);
                    this.deleteContext(itemProgramacaoContext, false);
                }
                return this.saveChanges(false) > 0;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }
    }
}

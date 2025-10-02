using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess
{
    public class ItemEscolaDataAccess : GenericRepository<ItemEscola>, IItemEscolaDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public ItemEscola findItemEscolabyId(int cdItem, int cdPessoa)
        {
            try{
                var sql = (from itemEscola in db.ItemEscola
                           where itemEscola.cd_item == cdItem &&
                                 itemEscola.cd_pessoa_escola == cdPessoa
                           select itemEscola).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        
        public ICollection<ItemEscola> getItensWithEscola(int cdItem, int cdUsuario)
        {
            try{
                var sql = (from itemEscola in db.ItemEscola
                           where itemEscola.cd_item == cdItem
                           && itemEscola.Escola.Usuarios.Any(us => us.cd_usuario == cdUsuario)
                           select itemEscola).ToList();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }

        }

        public IEnumerable<int> getItensWithEscola(int cdItem)
        {
            try
            {
                var sql = (from itemEscola in db.ItemEscola
                           where itemEscola.cd_item == cdItem
                           select itemEscola.cd_pessoa_escola).Distinct();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public IEnumerable<ItemEscola> getItensEscolaByItem(int cdItem)
        {
            try
            {
                var sql = (from itemEscola in db.ItemEscola
                           where itemEscola.cd_item == cdItem
                           select itemEscola).Distinct();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public decimal getValorCusto(int cd_item, int cd_escola) {
            try {
                var sql = (from itemEscola in db.ItemEscola
                           where itemEscola.cd_item == cd_item && itemEscola.cd_pessoa_escola == cd_escola
                           select itemEscola.vl_custo).FirstOrDefault();
                return sql;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public int getQtdEstoque(int cd_item, int cd_escola){
            try {
                var sql = (from itemEscola in db.ItemEscola
                           where itemEscola.cd_item == cd_item && itemEscola.cd_pessoa_escola == cd_escola
                           select itemEscola.qt_estoque).FirstOrDefault();
                return sql;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }
        public IEnumerable<ItemEscola> getItemEscolaByItem(int cdItem)
        {
            try
            {
                var sql = (from itemEscola in db.ItemEscola
                           where itemEscola.cd_item == cdItem
                           select itemEscola).Distinct();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public IEnumerable<ItemEscola> getItemComSubgrupoByEscola(int cdSubGrupo, int cdEscola)
        {
            try
            {
                var sql = from itemEscola in db.ItemEscola
                          where (itemEscola.Item.cd_subgrupo_conta == cdSubGrupo || itemEscola.Item.cd_subgrupo_conta_2 == cdSubGrupo) &&
                                itemEscola.cd_pessoa_escola == cdEscola &&
                                itemEscola.cd_plano_conta == null
                          select itemEscola;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public bool getExisteItensEscolaByItem(int cdItem)
        {
            try
            {
                bool sql = (from itemEscola in db.ItemEscola
                           where itemEscola.cd_item == cdItem
                           select itemEscola.cd_item_escola).Any();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}

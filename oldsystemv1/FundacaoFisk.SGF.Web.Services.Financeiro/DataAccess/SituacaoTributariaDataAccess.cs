using System;
using System.Collections.Generic;
using System.Linq;
using Componentes.GenericModel;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using Componentes.GenericDataAccess;
using FundacaoFisk.SGF.Web.Services.Financeiro;
using System.Data.Entity;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess;
using Componentes.GenericDataAccess.GenericException;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess
{
    public class SituacaoTributariaDataAccess : GenericRepository<SituacaoTributaria>, ISituacaoTributariaDataAccess
    {

        public enum TipoConsultaSitTribEnum
        {
            HAS_ATIVO = 0
        }
        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<SituacaoTributaria> getSituacaoTributaria(TipoConsultaSitTribEnum tipo, List<int> cd_situacoes, int cdTpNF)
        {
            try{

                IEnumerable<SituacaoTributaria> sitTrib = from st in db.SituacaoTributaria
                                                          orderby st.dc_situacao_tributaria
                                                select st;
                switch (tipo)
                {
                    case TipoConsultaSitTribEnum.HAS_ATIVO:
                        sitTrib = from st in sitTrib
                                  where (st.id_situacao_ativa || (cd_situacoes.Count > 0  && cd_situacoes.Contains(st.cd_situacao_tributaria))) &&
                                  (st.id_regime_tributario == 0 || db.TipoNotaFiscal.Where(p => p.id_regime_tributario == st.id_regime_tributario && p.cd_tipo_nota_fiscal == cdTpNF).Any())
                                  select st;
                        break;
                }
                return sitTrib;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public SituacaoTributaria getSituacaoTributariaItem(int cd_grupo_estoque, int id_regime_tributario, int cdSitTrib)
        {
            try
            {
                //Seleciona a situação tributaria do grupo ou a situação do Tipo de NF
                SituacaoTributaria sitTrib = (from st in db.SituacaoTributaria
                                              join g in db.GrupoSituacao
                                             on st.cd_situacao_tributaria equals g.cd_situacao_tributaria
                                              where g.cd_grupo_estoque == cd_grupo_estoque &&
                                              g.id_regime_tributaria == id_regime_tributario
                                              select new
                                              {
                                                  cd_situacao_tributaria = st.cd_situacao_tributaria,
                                                  id_forma_tributacao = st.id_forma_tributacao
                                              }).ToList().Select(x => new SituacaoTributaria
                                              {
                                                  cd_situacao_tributaria = x.cd_situacao_tributaria,
                                                  id_forma_tributacao = x.id_forma_tributacao
                                              }).FirstOrDefault();
                if(sitTrib == null || sitTrib.cd_situacao_tributaria == 0)
                    sitTrib = (from st in db.SituacaoTributaria
                               where st.cd_situacao_tributaria == cdSitTrib
                               select new
                               {
                                   cd_situacao_tributaria = st.cd_situacao_tributaria,
                                   id_forma_tributacao = st.id_forma_tributacao
                               }).ToList().Select(x => new SituacaoTributaria
                               {
                                   cd_situacao_tributaria = x.cd_situacao_tributaria,
                                   id_forma_tributacao = x.id_forma_tributacao
                               }).FirstOrDefault();
                return sitTrib;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public IEnumerable<SituacaoTributaria> getSituacaoTributariaTipo(TipoConsultaSitTribEnum tipo, List<int> cd_situacoes, int tipoImp, int cd_escola, byte id_regime_trib, bool master_geral)
        {
            try  
            {
                IEnumerable<SituacaoTributaria> sitTrib;
                if (!master_geral)
                    sitTrib = from st in db.SituacaoTributaria
                              where (st.id_regime_tributario == 0 || db.Parametro.Where(p => p.cd_pessoa_escola == cd_escola && p.id_regime_tributario == st.id_regime_tributario).Any())
                              orderby st.dc_situacao_tributaria
                              select st;
                else
                    sitTrib = from st in db.SituacaoTributaria
                              where (st.id_regime_tributario == id_regime_trib || id_regime_trib == 0)
                              orderby st.dc_situacao_tributaria
                              select st;
                switch (tipo)
                {
                    case TipoConsultaSitTribEnum.HAS_ATIVO:
                        sitTrib = from st in sitTrib
                                  where (st.id_situacao_ativa || (cd_situacoes.Count > 0 && cd_situacoes.Contains(st.cd_situacao_tributaria))) &&
                                  st.id_tipo_imposto == tipoImp 
                                  select st;
                        break;
                }
                return sitTrib;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public SituacaoTributaria getSituacaoTributariaFormaTrib(int cd_situacao_trib)
        {
            try
            {
                SituacaoTributaria sitTrib = (from st in db.SituacaoTributaria
                                              where st.cd_situacao_tributaria == cd_situacao_trib
                                              select new
                                              {
                                                  cd_situacao_tributaria = st.cd_situacao_tributaria,
                                                  id_forma_tributacao = st.id_forma_tributacao
                                              }).ToList().Select(x => new SituacaoTributaria
                                              {
                                                  cd_situacao_tributaria = x.cd_situacao_tributaria,
                                                  id_forma_tributacao = x.id_forma_tributacao
                                              }).FirstOrDefault();
                
                return sitTrib;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}

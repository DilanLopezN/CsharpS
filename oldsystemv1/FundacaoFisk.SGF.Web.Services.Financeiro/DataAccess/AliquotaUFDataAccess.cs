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
    public class AliquotaUFDataAccess : GenericRepository<AliquotaUF>, IAliquotaUFDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<AliquotaUF> getAliquotaUFSearch(SearchParameters parametros, int cdEstadoOri, int cdEstadoDest, double? aliquota)
        {
            try
            {
                IEntitySorter<AliquotaUF> sorter = EntitySorter<AliquotaUF>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<AliquotaUF> sql;
                sql = from c in db.AliquotaUF.AsNoTracking()
                      orderby c.cd_localidade_estado_origem ascending
                      select c;

                if (cdEstadoOri > 0)
                    sql = from s in sql
                          where s.cd_localidade_estado_origem == cdEstadoOri
                          select s;
                if (cdEstadoDest > 0)
                    sql = from s in sql
                          where s.cd_localidade_estado_destino == cdEstadoDest
                          select s;
                if (aliquota != null)
                    sql = from s in sql
                          where s.pc_aliq_icms_padrao == aliquota
                          select s;

                sql = sorter.Sort(sql);

                IEnumerable<AliquotaUF> sql1 = (from s in sql
                                                select new
                                                {
                                                    cd_aliquota_uf = s.cd_aliquota_uf,
                                                    cd_localidade_estado_origem = s.cd_localidade_estado_origem,
                                                    cd_localidade_estado_destino = s.cd_localidade_estado_destino,
                                                    pc_aliq_icms_padrao = s.pc_aliq_icms_padrao,
                                                    no_localidade_des = s.EstadoDestino.Localidade.no_localidade,
                                                    no_localidade_ori = s.EstadoOrigem.Localidade.no_localidade
                                                }).ToList().Select(x => new AliquotaUF
                                                 {
                                                     cd_aliquota_uf = x.cd_aliquota_uf,
                                                     cd_localidade_estado_origem = x.cd_localidade_estado_origem,
                                                     cd_localidade_estado_destino = x.cd_localidade_estado_destino,
                                                     pc_aliq_icms_padrao = x.pc_aliq_icms_padrao,
                                                     EstadoOrigem = new EstadoSGF
                                                     {
                                                         Localidade = new LocalidadeSGF
                                                         {
                                                             no_localidade = x.no_localidade_ori
                                                         }
                                                     },
                                                     EstadoDestino = new EstadoSGF
                                                     {
                                                         Localidade = new LocalidadeSGF
                                                         {
                                                             no_localidade = x.no_localidade_des
                                                         }
                                                     }
                                                 });
                int limite = sql1.Count();

                parametros.ajustaParametrosPesquisa(limite);
                sql1 = sql1.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;
                return sql1;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public AliquotaUF getAliquotaUFById(int cdAliquota)
        {
            try
            {
                AliquotaUF sql = (from s in db.AliquotaUF
                                  where s.cd_aliquota_uf == cdAliquota
                                  select new
                                  {
                                      cd_aliquota_uf = s.cd_aliquota_uf,
                                      cd_localidade_estado_origem = s.cd_localidade_estado_origem,
                                      cd_localidade_estado_destino = s.cd_localidade_estado_destino,
                                      pc_aliq_icms_padrao = s.pc_aliq_icms_padrao,
                                      no_localidade_des = s.EstadoDestino.Localidade.no_localidade,
                                      no_localidade_ori = s.EstadoOrigem.Localidade.no_localidade
                                  }).ToList().Select(x => new AliquotaUF
                                               {
                                                   cd_aliquota_uf = x.cd_aliquota_uf,
                                                   cd_localidade_estado_origem = x.cd_localidade_estado_origem,
                                                   cd_localidade_estado_destino = x.cd_localidade_estado_destino,
                                                   pc_aliq_icms_padrao = x.pc_aliq_icms_padrao,
                                                   EstadoOrigem = new EstadoSGF
                                                   {
                                                       Localidade = new LocalidadeSGF
                                                       {
                                                           no_localidade = x.no_localidade_ori
                                                       }
                                                   },
                                                   EstadoDestino = new EstadoSGF
                                                   {
                                                       Localidade = new LocalidadeSGF
                                                       {
                                                           no_localidade = x.no_localidade_des
                                                       }
                                                   }
                                               }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public AliquotaUF getAliquotaUFByOriDes( int cdEstadoOri, int cdEstadoDest)
        {
            try
            {
                AliquotaUF sql = (from c in db.AliquotaUF
                                  where c.cd_localidade_estado_destino == cdEstadoDest &&
                                  c.cd_localidade_estado_origem == cdEstadoOri
                                  select c).FirstOrDefault();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public IEnumerable<EstadoSGF> getEstadoOri()
        {
            try
            {
                IEnumerable<EstadoSGF> sql = (from l in db.EstadoSGF
                                              where l.AliquotaUFOrigem.Any()
                                              select new {
                                                    cd_localidade_estado = l.cd_localidade_estado,
                                                    sg_estado = l.sg_estado,
                                                    no_localidade = l.Localidade.no_localidade
                                               }).ToList().Select(x => new EstadoSGF
                                               {
                                                  cd_localidade_estado = x.cd_localidade_estado,
                                                  sg_estado = x.sg_estado,
                                                  Localidade = new LocalidadeSGF
                                                  {
                                                      no_localidade = x.no_localidade
                                                  }
                                               });
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public IEnumerable<EstadoSGF> getEstadoDest()
        {
            try
            {
                IEnumerable<EstadoSGF> sql = (from l in db.EstadoSGF
                                              where l.AliquotaUFDestino.Any()
                                              select new {
                                                    cd_localidade_estado = l.cd_localidade_estado,
                                                    sg_estado = l.sg_estado,
                                                    no_localidade = l.Localidade.no_localidade
                                               }).ToList().Select(x => new EstadoSGF
                                               {
                                                  cd_localidade_estado = x.cd_localidade_estado,
                                                  sg_estado = x.sg_estado,
                                                  Localidade = new LocalidadeSGF
                                                  {
                                                      no_localidade = x.no_localidade
                                                  }
                                               });
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public AliquotaUF getAliquotaUFByEscDes(int cdEscola, int cdEstadoDest)
        {
            try
            {
                AliquotaUF sql = (from c in db.AliquotaUF
                                  where c.cd_localidade_estado_destino == cdEstadoDest &&
                                  db.EnderecoSGF.Where(e => e.cd_pessoa == cdEscola && e.cd_loc_estado == c.cd_localidade_estado_origem).Any()
                                  select c).FirstOrDefault();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public AliquotaUF getAliquotaUFPorEstadoPessoa(int cdEscola, int cd_pessoa_cliente)
        {
            try
            {
                AliquotaUF sql = (from c in db.AliquotaUF
                                  where db.EnderecoSGF.Where(ep => ep.cd_pessoa == cd_pessoa_cliente && 
                                                             ep.cd_loc_estado == c.cd_localidade_estado_destino).Any() &&
                                        db.EnderecoSGF.Where(e => e.cd_pessoa == cdEscola &&
                                                             e.cd_loc_estado == c.cd_localidade_estado_origem).Any()
                                  select c).FirstOrDefault();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}

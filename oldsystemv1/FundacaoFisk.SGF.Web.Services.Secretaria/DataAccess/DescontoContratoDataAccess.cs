using System;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Collections.Generic;
using FundacaoFisk.SGF.GenericModel;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess
{
    public class DescontoContratoDataAccess : GenericRepository<DescontoContrato>, IDescontoContratoDataAccess
    {
        

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<DescontoContrato> getDescontoContrato(int cd_contrato, int cd_pessoa_escola)
        {
            IEnumerable<DescontoContrato> sql;
            try
            {
                sql = ((from descontoContrato in db.DescontoContrato
                        where descontoContrato.cd_contrato == cd_contrato
                           && descontoContrato.Contratos.cd_pessoa_escola == cd_pessoa_escola
                        select new
                        {
                            dc_tipo_desconto = descontoContrato.dc_desconto_contrato,
                            pc_desconto = descontoContrato.pc_desconto_contrato,
                            vl_desconto_contrato = descontoContrato.vl_desconto_contrato,
                            id_incide_baixa = descontoContrato.id_incide_baixa,
                            id_desconto_ativo = descontoContrato.id_desconto_ativo,
                            cd_tipo_desconto = descontoContrato.cd_tipo_desconto,
                            cd_desconto_contrato = descontoContrato.cd_desconto_contrato,
                            nm_parcela_ini = descontoContrato.nm_parcela_ini,
                            nm_parcela_fim = descontoContrato.nm_parcela_fim
                        }).ToList().Select(x => new DescontoContrato
                         {
                             dc_tipo_desconto = x.dc_tipo_desconto,
                             pc_desconto = x.pc_desconto,
                             vl_desconto_contrato = x.vl_desconto_contrato,
                             id_incide_baixa = x.id_incide_baixa,
                             id_desconto_ativo = x.id_desconto_ativo,
                             cd_tipo_desconto = x.cd_tipo_desconto,
                             cd_desconto_contrato = x.cd_desconto_contrato,
                             nm_parcela_ini = x.nm_parcela_ini,
                             nm_parcela_fim = x.nm_parcela_fim
                         }));
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
            return sql;
        }

        public IEnumerable<DescontoContrato> getDescontoAditamento(int cd_contrato, int cd_aditamento, int cd_pessoa_escola)
        {
            try
            {
                IEnumerable<DescontoContrato> sql = from descontoContrato in db.DescontoContrato
                                                    where descontoContrato.cd_aditamento == cd_aditamento
                                                       && descontoContrato.Aditamento.Contrato.cd_pessoa_escola == cd_pessoa_escola
                                                       && descontoContrato.Aditamento.cd_contrato == cd_contrato
                                                    select descontoContrato;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }


        public IEnumerable<DescontoContrato> getDescontoContratoAllDados(int cd_contrato, int cd_pessoa_escola)
        {
            IEnumerable<DescontoContrato> sql;
            try
            {
                sql = from descontoContrato in db.DescontoContrato
                        where descontoContrato.cd_contrato == cd_contrato
                           && descontoContrato.Contratos.cd_pessoa_escola == cd_pessoa_escola
                        select descontoContrato;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
            return sql;
        }
    }
}

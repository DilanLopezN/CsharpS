using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Data;
using System.Data.Common;
using System.Net.Http;
using System.Net.Sockets;
using FundacaoFisk.SGF.Web.Services.Secretaria.Business;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess
{
    //public class DataAccessMensagemPadraoSms : GenericRepository<SmSComporMensagemPadrao>, ISmsMensagempadraoDataAccess
    //{

    //    private SGFWebContext db
    //    {
    //        get
    //        {
    //            return (SGFWebContext)base.DB();
    //        }
    //    }

    //    public IEnumerable<SmSComporMensagemPadrao> getListMensagensPadraoByEscola(int cdEscola)
    //    {
    //        IEnumerable<SmSComporMensagemPadrao> sql;
    //        try
    //        {
    //            sql = (from m in db.SmSComporMensagemPadrao
    //                where m.cd_escola == cdEscola
    //                select new
    //                {
    //                    cd_sms_padrao = m.cd_sms_padrao,
    //                    cd_escola = m.cd_escola,
    //                    motivo = m.motivo,
    //                    mensagem = m.mensagem,
    //                    dt_cadastro = m.dt_cadastro
    //                }).ToList().Select(x => new SmSComporMensagemPadrao()
    //            {
    //                cd_sms_padrao = x.cd_sms_padrao,
    //                cd_escola = x.cd_escola,
    //                motivo = x.motivo,
    //                mensagem = x.mensagem,
    //                dt_cadastro = x.dt_cadastro
    //            });
    //        }
    //        catch (Exception exe)
    //        {
    //            throw new DataAccessException(exe);
    //        }

    //        return sql;
    //    }

    //    public SmSComporMensagemPadrao getParamMensagemSms(int cd_escola, int motivo)
    //    {
    //        SmSComporMensagemPadrao sql;
    //        try
    //        {
    //            sql = (from p in db.SmSComporMensagemPadrao
    //                where p.cd_escola == cd_escola && p.motivo == motivo
    //                select p).FirstOrDefault();
          
    //        }
    //        catch (Exception exe)
    //        {
    //            throw new DataAccessException(exe);
    //        }
    //        return sql;
    //    }

    //    public SmSComporMensagemPadrao getParamMensagemSmsById(SmSComporMensagemPadrao smsComporMensagem)
    //    {
    //        SmSComporMensagemPadrao sql;
    //        try
    //        {
    //            sql = (from p in db.SmSComporMensagemPadrao
    //                where p.cd_escola == smsComporMensagem.cd_escola && p.motivo == smsComporMensagem.motivo
    //                select p).FirstOrDefault();
    //        }
    //        catch (Exception exe)
    //        {
    //            throw new DataAccessException(exe);
    //        }
    //        return sql;
    //    }
        
    //}
}
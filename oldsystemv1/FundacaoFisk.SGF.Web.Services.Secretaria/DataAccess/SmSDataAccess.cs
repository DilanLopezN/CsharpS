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

namespace FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess
{
    //public class DataAccessSms : GenericRepository<SmsParametrosEscola>, ISmsDataAcccess 
    //    {

    //    private SGFWebContext db
    //    {
    //        get
    //        {
    //            return (SGFWebContext)base.DB();
    //        }
    //    }

    //    public IEnumerable<SmsParametroUI> verificaParametros(int cdEscola)
    //    {
    //        SmsParametrosEscola sql;
    //        try
    //        {
    //            sql = (from p in db.SmsParametrosEscola
    //                   where p.cd_escola == cdEscola
    //                   select new
    //                   {
    //                       cod_escola = p.cd_escola,
    //                       num_usu = p.num_usu,
    //                       senha = p.senha,
    //                       seu_num = p.seu_num,
    //                       url_servico = p.url_servico,
    //                       id_automatico_devedores = p.id_automatico_devedores,
    //                       id_automatico_aniversario = p.id_automatico_aniversario
    //                   }).ToList().Select(x => new SmsParametrosEscola()
    //                   {
    //                       cd_escola = x.cod_escola,
    //                       num_usu = x.num_usu,
    //                       senha = x.senha,
    //                       seu_num = x.seu_num,
    //                       url_servico = x.url_servico,
    //                       id_automatico_devedores = x.id_automatico_devedores,
    //                       id_automatico_aniversario = x.id_automatico_aniversario
    //                   }).FirstOrDefault();
    //        }
    //        catch (Exception e)
    //        {
    //            throw new DataAccessException(e);
    //        }

    //        return (IEnumerable<SmsParametroUI>) sql;
    //    }

    //    public IEnumerable<SmsParametrosEscola> getListaEscolaComParametro(int cdEscola)
    //    {
    //        IEnumerable<SmsParametrosEscola> sql;
    //        try
    //        {
    //            sql = (from p in db.SmsParametrosEscola
    //                where p.cd_escola == cdEscola
    //            select new
    //            {
    //                num_usu = p.num_usu,
    //                senha = p.senha,
    //                seu_num = p.seu_num,
    //                url_servico = p.url_servico,
    //                id_automatico_devedores = p.id_automatico_devedores,
    //                id_automatico_aniversario = p.id_automatico_aniversario
    //           }).ToList().Select(x => new SmsParametrosEscola()
    //           {
    //               num_usu = x.num_usu,
    //               senha = x.senha,
    //               seu_num = x.seu_num,
    //               url_servico = x.url_servico,
    //               id_automatico_devedores = x.id_automatico_devedores,
    //               id_automatico_aniversario = x.id_automatico_aniversario
    //           });
    //        }
    //        catch (Exception exe)
    //        {
    //            throw new DataAccessException(exe);
    //        }
    //        return sql;
    //    }

    //    public SmsParametrosEscola getParamEscolaSms(int cdEscola)
    //    {
    //        SmsParametrosEscola sql;
    //        try
    //        {
    //            sql = (from p in db.SmsParametrosEscola
    //                where p.cd_escola == cdEscola
    //                select p).FirstOrDefault();
          
    //        }
    //        catch (Exception exe)
    //        {
    //            throw new DataAccessException(exe);
    //        }
    //        return sql;
    //    }

    //    public SmsParametrosEscola GetParametrosEscolaById(SmsParametrosEscola smsParametros)
    //    {
    //        SmsParametrosEscola sql;
    //        try
    //        {
    //            sql = (from p in db.SmsParametrosEscola
    //                where p.cd_escola == smsParametros.cd_escola
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

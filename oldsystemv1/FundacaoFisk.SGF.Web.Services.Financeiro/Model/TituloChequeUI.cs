using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;
namespace FundacaoFisk.SGF.Web.Services.Financeiro.Model
{
    public class TituloChequeUI : TO
    {
        public int cd_pessoa_empresa { get; set; }
        public int status_tit_baixa_aut { get; set; }

        public DateTime? dtInicial { get; set; }
        public DateTime? dtFinal { get; set; }

        public bool troca_local { get; set; }
        public int cd_local_movto_cheque { get; set; }
    }
}
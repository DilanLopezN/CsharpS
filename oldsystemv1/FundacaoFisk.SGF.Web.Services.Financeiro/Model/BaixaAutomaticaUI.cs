using System;
using System.Collections.Generic;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Model
{
    public class BaixaAutomaticaUI
    {
        public int cd_baixa_automatica { get; set; }
        public int cd_escola {get; set;}
        public int cd_local_movto { get; set; }
        public Nullable<int> cd_cartao_credito { get; set; }
        public int cd_usuario { get; set; }
        public Nullable<DateTime> dt_inicial { get; set; }
        public DateTime dt_final { get; set; }
        public DateTime dh_baixa_automatica { get; set; }
        public byte id_tipo { get; set; }
        public bool id_trocar_local { get; set; }
        public List<int> cds_titulos { get; set; }
    }
}
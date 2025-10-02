using System;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Model
{
    public class AlunosSemTituloGeradoParamsUI : TO
    {
        public string no_mes { get; set; }
        public int vl_mes { get; set; }
        public int ano { get; set; }
        public Nullable<int> cd_turma { get; set; }
        public string situacoes { get; set; }
    }
}
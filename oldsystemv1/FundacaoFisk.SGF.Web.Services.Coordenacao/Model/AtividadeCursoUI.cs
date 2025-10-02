using FundacaoFisk.SGF.GenericModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Componentes.GenericModel;


namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Model
{
    public class AtividadeCursoUI : TO
    {
        public int cd_atividade_curso { get; set; }
        public int cd_atividade_extra { get; set; }
        public int cd_curso { get; set; }
        public int cd_pessoa_escola { get; set; }

    }
}

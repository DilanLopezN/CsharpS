using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Model
{
    public class DeletaMatriculaUI : TO
    {
        public enum StatusProcedure
        {
            SUCESSO_EXECUCAO_PROCEDURE = 0,
            ERRO_EXECUCAO_PROCEDURE = 1
        }

        public Nullable<int> cd_contrato { get; set;}
        public Nullable<int> cd_usuario { get; set;}
        public Nullable<int> fuso { get; set; }
    }
}

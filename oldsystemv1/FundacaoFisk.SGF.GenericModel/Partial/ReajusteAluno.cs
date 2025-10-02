using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class ReajusteAluno
    {

        public string no_pessoa
        {
            get {
                if (this.Aluno != null)
                    return this.Aluno.AlunoPessoaFisica.no_pessoa;
                else
                    return "";
            }
        }
    }
}

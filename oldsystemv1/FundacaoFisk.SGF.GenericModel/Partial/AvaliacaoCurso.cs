using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class AvaliacaoCurso
    {
        public string no_produto
        {
            get
            {
                var retorno = "";
                if (Curso != null && Curso.Produto != null)
                    retorno = Curso.Produto.no_produto;
                return retorno;
            }
        }
        public string no_curso
        {
            get
            {
                var retorno = "";
                if (Curso != null)
                {
                    retorno = Curso.no_curso;
                    Curso = null;
                }
                return retorno;
            }
        }

    }
}

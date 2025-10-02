using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    using Componentes.GenericModel;
    public partial class PapelSGF : TO
    {
        public enum TipoPapelSGF
        {
            PAIS = 1,
            FILHOS = 2,
            RESPONSAVEL = 3,
            ALUNORESPONSAVEL = 9,
            FRANQUEADO = 18
        }
        public static PapelSGF toPapel(PapelSGF papelSGF)
        {
            PapelSGF retorno = new PapelSGF();
            retorno.copy(papelSGF);
            return retorno;
        }
        public static IEnumerable<PapelSGF> toListaPapel(IEnumerable<PapelSGF> listaPapelSGF)
        {
            List<PapelSGF> retorno = new List<PapelSGF>();
            List<PapelSGF> listPapelSGF = listaPapelSGF.ToList();
            for(int i = 0; listaPapelSGF!= null && i < listPapelSGF.Count(); i++){
                PapelSGF papel = new PapelSGF();
                papel.copy(listPapelSGF[i]);
                retorno.Add(papel);
          }
            return retorno;
        }
    }
}

using System;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel
{
    public class ContratoComboUI : TO
    {
        public int cd_contrato { get; set; }

        public int? nm_contrato { get; set; }

        public Nullable<int> nm_matricula_contrato { get; set; }

        public string no_contrato
        {
            get
            {
                if ((nm_contrato != null && nm_matricula_contrato != null && nm_contrato == nm_matricula_contrato) || (nm_contrato != null && nm_matricula_contrato == null))
                {
                    return String.Format("Contrato: {0}", nm_contrato);
                }
                else if (nm_contrato != null && nm_matricula_contrato != null && nm_contrato != nm_matricula_contrato)
                {
                    return String.Format("Contrato: {0}, Número: {1}", nm_contrato, nm_matricula_contrato);
                }
                else
                {
                    return null;
                }

            }
        }
    }
}
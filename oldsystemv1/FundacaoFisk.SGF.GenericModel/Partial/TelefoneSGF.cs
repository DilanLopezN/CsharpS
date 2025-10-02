using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class TelefoneSGF
    {
        public string nm_telefone_contatos
        {
            get {
                if(this.TelefoneTipo != null)
                    return this.TelefoneTipo.no_tipo_telefone + ": " + dc_fone_mail;
                else
                    return "";
            }
        }

        public static string getTelefones(List<TelefoneSGF> lista)
        {
            string nm_telefone_contatos = String.Empty;
            string no_tipo_telefone = String.Empty;
            for(int j = 0; j < lista.Count; j++)
                if (lista[j].TelefoneTipo == null || no_tipo_telefone.Equals(lista[j].TelefoneTipo.no_tipo_telefone))
                    nm_telefone_contatos = nm_telefone_contatos + " / " + lista[j].dc_fone_mail;
                else {
                    no_tipo_telefone = lista[j].TelefoneTipo.no_tipo_telefone;

                    if(String.Empty.Equals(nm_telefone_contatos))
                        nm_telefone_contatos = lista[j].nm_telefone_contatos;
                    else
                        nm_telefone_contatos = nm_telefone_contatos + ", " + lista[j].nm_telefone_contatos;
                }
            return nm_telefone_contatos;
        }
    }
}

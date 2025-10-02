using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.Model
{
    public class TelefoneUI
    {
        public int cd_telefone { get; set; }
        public int? cd_operadora { get; set; }
        public int cd_tipo_telefone { get; set; }
        public int cd_classe_telefone { get; set; }
        public string dc_fone_mail { get; set; }
        public Nullable<int> cd_endereco { get; set; }
        public string des_tipo_contato { get; set; }
        public string no_classe { get; set; }
        public string no_pessoa { get; set; }
        public bool id_telefone_principal { get; set; } 

        public static ContatosUI fromTelefoneforTelefoneUI(ICollection<TelefoneSGF> telefones)
        {
            ContatosUI contato = new ContatosUI();
            List<TelefoneUI> outrosContatos = new List<TelefoneUI>();
            List<TelefoneUI> contatosPrincipais = new List<TelefoneUI>();
            foreach (var telefone in telefones)
            {
                TelefoneUI telefoneUI = new TelefoneUI {
                    cd_telefone = telefone.cd_telefone,
                    cd_operadora = telefone.cd_operadora,
                    cd_classe_telefone = telefone.cd_classe_telefone,
                    cd_endereco = telefone.cd_endereco,
                    cd_tipo_telefone = telefone.cd_tipo_telefone,
                    dc_fone_mail = telefone.dc_fone_mail,
                    des_tipo_contato = telefone.TelefoneTipo.no_tipo_telefone,
                    no_classe = telefone.ClasseTelefone.dc_classe_telefone,
                    no_pessoa = telefone.PessoaTelefone.no_pessoa,
                    id_telefone_principal = telefone.id_telefone_principal
                };
                if (telefone.id_telefone_principal == true)
                    contatosPrincipais.Add(telefoneUI);
                else
                    outrosContatos.Add(telefoneUI);
            }
            contato.outrosContatos = outrosContatos;
            contato.contatosPrincipais = contatosPrincipais;
            //retorno.ativo = usuarioWeb.usuario_ativo;
            return contato;
        }
    }
}

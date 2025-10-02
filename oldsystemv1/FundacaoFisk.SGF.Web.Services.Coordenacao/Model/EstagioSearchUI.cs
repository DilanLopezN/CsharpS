using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Model
{
    public partial class EstagioSearchUI : TO
    {
        public int cd_estagio { get; set; }
        public int cd_produto { get; set; }
        public string no_estagio { get; set; }
        public Nullable<byte> nm_ordem_estagio { get; set; }
        public bool id_estagio_ativo { get; set; }
        public string no_produto { get; set; }
        public string no_estagio_abreviado { get; set; }
        public string cor_legenda { get; set; }
        public string estagio_ativo
        {
            get
            {
                return this.id_estagio_ativo ? "Sim" : "Não";
            }
        }

        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_estagio", "Código"));
                retorno.Add(new DefinicaoRelatorio("no_produto", "Produto"));
                retorno.Add(new DefinicaoRelatorio("no_estagio", "Estágio", AlinhamentoColuna.Left, "3.0000in"));
                retorno.Add(new DefinicaoRelatorio("no_estagio_abreviado", "Abreviado", AlinhamentoColuna.Left));
                retorno.Add(new DefinicaoRelatorio("ordem", "Ordem", AlinhamentoColuna.Right));
                retorno.Add(new DefinicaoRelatorio("estagio_ativo", "Ativo", AlinhamentoColuna.Center));

                return retorno;
            }
        }

        public string ordem {
            get {
                return this.nm_ordem_estagio == null ? "-" : this.nm_ordem_estagio.ToString();
            }
        }

        public static EstagioSearchUI  fromEstagio(Estagio estagio, String produto) {
            EstagioSearchUI estagioUI = new EstagioSearchUI();
            estagioUI.cd_estagio = estagio.cd_estagio;
            estagioUI.cd_produto = estagio.cd_produto;
            estagioUI.id_estagio_ativo = estagio.id_estagio_ativo;
            estagioUI.no_estagio = estagio.no_estagio;
            estagioUI.no_estagio_abreviado = estagio.no_estagio_abreviado;
            estagioUI.no_produto = produto;
            estagioUI.nm_ordem_estagio = estagio.nm_ordem_estagio;
            return estagioUI;
        }
    }
}

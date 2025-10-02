using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Model
{
    public partial class SalaSearchUI : TO
    {
        public int cd_sala { get; set; }
        public int cd_pessoa { get; set; }
        public string no_pessoa { get; set; }
        public string no_sala { get; set; }
        public int nm_vaga_sala { get; set; }
        public bool id_sala_ativa { get; set; }
        public bool? id_sala_online { get; set; }
        public string id_zoom { get; set; }
        public string dc_usuario_escola { get; set; }
        public string dc_senha_usuario_escola { get; set; }
        public string dc_usuario_adm{ get; set; }
        public string dc_senha_usuario_adm { get; set; }

        public string salaAtiva
        {
            get
            {
                return this.id_sala_ativa ? "Sim" : "Não";
            }
        }

        public List<DefinicaoRelatorio> ColunasRelatorio {
            get {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_sala", "Código"));
                retorno.Add(new DefinicaoRelatorio("no_sala", "Sala", AlinhamentoColuna.Left, "3.2000in"));
                retorno.Add(new DefinicaoRelatorio("nm_vaga_sala", "Vaga", AlinhamentoColuna.Right, "0.9000in"));
                retorno.Add(new DefinicaoRelatorio("salaAtiva", "Ativa", AlinhamentoColuna.Center, "0.9000in"));

                return retorno;
            }
        }
    }

    //TO DO
    public class SalaDisponivel
    {
        public int cd_sala { get; set; }
        public string no_sala { get; set; }
        public float totolHoras { get; set; }
        public float totalMinutos { get; set; }
    }
}

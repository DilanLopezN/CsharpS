using System;
using System.Collections.Generic;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;

namespace FundacaoFisk.SGF.Services.Coordenacao.Model
{
    public class MensagemAvaliacaoSearchUI : TO
    {
        public int cd_mensagem_avaliacao { get; set; }
        public int cd_produto { get; set; }
        public int cd_curso { get; set; }
        public bool id_mensagem_ativa { get; set; }
        public string tx_mensagem_avaliacao { get; set; }
        public string mensagemAtiva
        {
            get
            {
                return this.id_mensagem_ativa ? "Sim" : "Não";
            }
        }

        public string no_produto { get; set; }

        public string no_curso { get; set; }

        public ICollection<int> cursos { get; set; }

        public ICollection<Produto> listaProdutos { get; set; }
        public ICollection<Curso> listaCursos { get; set; }

        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_evento", "Código"));
                retorno.Add(new DefinicaoRelatorio("tx_mensagem_avaliacao", "Mensagem", AlinhamentoColuna.Left, "4.5000in"));
                retorno.Add(new DefinicaoRelatorio("no_produto", "Produto", AlinhamentoColuna.Left, "0.9000in"));
                retorno.Add(new DefinicaoRelatorio("no_curso", "Curso", AlinhamentoColuna.Left, "0.9000in"));
                retorno.Add(new DefinicaoRelatorio("mensagemAtiva", "Ativo", AlinhamentoColuna.Center, "0.9000in"));

                return retorno;
            }
        }

        public static MensagemAvaliacaoSearchUI fromEstagio(MensagemAvaliacao mensagemAvaliacao, String produto, String curso)
        {
            MensagemAvaliacaoSearchUI mensagemAvaliacaoUI = new MensagemAvaliacaoSearchUI();
            mensagemAvaliacaoUI.cd_mensagem_avaliacao = mensagemAvaliacao.cd_mensagem_avaliacao;
            mensagemAvaliacaoUI.cd_produto = mensagemAvaliacao.cd_produto;
            mensagemAvaliacaoUI.cd_curso = mensagemAvaliacao.cd_curso;
            mensagemAvaliacaoUI.id_mensagem_ativa = mensagemAvaliacao.id_mensagem_ativa;
            mensagemAvaliacaoUI.tx_mensagem_avaliacao = mensagemAvaliacao.tx_mensagem_avaliacao;
            mensagemAvaliacaoUI.no_produto = produto;
            mensagemAvaliacaoUI.no_curso = curso;
            
            return mensagemAvaliacaoUI;
        }

        

    }
}
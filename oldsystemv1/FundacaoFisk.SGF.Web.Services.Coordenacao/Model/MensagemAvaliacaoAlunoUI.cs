using System;
using System.Collections.Generic;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;

namespace FundacaoFisk.SGF.Services.Coordenacao.Model
{
    public class MensagemAvaliacaoAlunoUI : TO
    {
        public int cd_mensagem_avaliacao_aluno { get; set; }
        public int cd_aluno { get; set; }
        public int cd_tipo_avaliacao { get; set; }
        public int cd_produto { get; set; }
        public int cd_curso { get; set; }
        public bool id_mensagem_ativa { get; set; }
        public string tx_mensagem_avaliacao_aluno { get; set; }
        public string no_produto { get; set; }
        public string no_curso { get; set; }
        public int cd_mensagem_avaliacao { get; set; }
    }
}
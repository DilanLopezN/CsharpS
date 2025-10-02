using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Services.Coordenacao.Comum.IDataAccess;
using FundacaoFisk.SGF.Services.Coordenacao.Model;

namespace FundacaoFisk.SGF.Services.Coordenacao.DataAccess
{
    public class MensagemAvaliacaoAlunoDataAccess : GenericRepository<MensagemAvaliacaoAluno>, IMensagemAvaliacaoAlunoDataAccess
    {
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }
        public IEnumerable<MensagemAvaliacaoAlunoUI> findMsgAlunobyTipo(int cdTipoAvaliacao, int cdAluno, int cdProduto, int cdCurso)
        {
            IEnumerable<MensagemAvaliacaoAlunoUI> sql;
            sql = (from mal in db.MensagemAvaliacaoAluno
                   join ma in db.MensagemAvaliacao on mal.cd_mensagem_avaliacao equals ma.cd_mensagem_avaliacao
                   where mal.cd_tipo_avaliacao == cdTipoAvaliacao &&
                         mal.cd_aluno == cdAluno &&
                         ma.cd_produto == cdProduto &&
                         ma.cd_curso == cdCurso
                   select new MensagemAvaliacaoAlunoUI
                   {
                       cd_mensagem_avaliacao_aluno = mal.cd_mensagem_avaliacao_aluno,
                       tx_mensagem_avaliacao_aluno = mal.tx_mensagem_avaliacao_aluno,
                       cd_tipo_avaliacao = mal.cd_tipo_avaliacao,
                       cd_aluno = mal.cd_aluno,
                       id_mensagem_ativa = mal.id_mensagem_aluno_ativa,
                       cd_produto = ma.cd_produto,
                       no_produto = ma.Produto.no_produto,
                       cd_curso = ma.cd_curso,
                       no_curso = ma.Curso.no_curso,
                       cd_mensagem_avaliacao = mal.cd_mensagem_avaliacao
                   }).ToList();
            return sql;
        }
        public List<MensagemAvaliacaoAlunoUI> getMensagensAvaliacaoAlunoByIds(List<int> cdsMensagens)
        {
            try
            {
                List<MensagemAvaliacaoAlunoUI> listMensagem = (from mensagem in db.MensagemAvaliacaoAluno
                                                                where !cdsMensagens.Contains(mensagem.cd_mensagem_avaliacao_aluno) &&
                                                                      mensagem.id_mensagem_aluno_ativa
                                                                orderby mensagem.cd_aluno
                                                                select new MensagemAvaliacaoAlunoUI
                                                                {
                                                                    cd_mensagem_avaliacao_aluno = mensagem.cd_mensagem_avaliacao_aluno,
                                                                    tx_mensagem_avaliacao_aluno = mensagem.tx_mensagem_avaliacao_aluno,
                                                                    id_mensagem_ativa = mensagem.id_mensagem_aluno_ativa,
                                                                    cd_aluno = mensagem.cd_aluno,
                                                                    cd_tipo_avaliacao = mensagem.cd_tipo_avaliacao,
                                                                    cd_mensagem_avaliacao = mensagem.cd_mensagem_avaliacao
                                                                }).ToList();
                return listMensagem;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<MensagemAvaliacaoAlunoUI> getMensagensAvaliacaoAlunoById(int cd_mensagem_avaliacao)
        {
            try
            {
                List<MensagemAvaliacaoAlunoUI> listMensagem = (from mensagem in db.MensagemAvaliacaoAluno
                                                                where mensagem.cd_mensagem_avaliacao_aluno == cd_mensagem_avaliacao
                                                                orderby mensagem.cd_aluno
                                                                select new MensagemAvaliacaoAlunoUI
                                                                {
                                                                    cd_mensagem_avaliacao_aluno = mensagem.cd_mensagem_avaliacao_aluno,
                                                                    tx_mensagem_avaliacao_aluno = mensagem.tx_mensagem_avaliacao_aluno,
                                                                    id_mensagem_ativa = mensagem.id_mensagem_aluno_ativa,
                                                                    cd_aluno = mensagem.cd_aluno,
                                                                    cd_tipo_avaliacao = mensagem.cd_tipo_avaliacao,
                                                                    cd_mensagem_avaliacao = mensagem.cd_mensagem_avaliacao
                                                                }).ToList();
                return listMensagem;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}

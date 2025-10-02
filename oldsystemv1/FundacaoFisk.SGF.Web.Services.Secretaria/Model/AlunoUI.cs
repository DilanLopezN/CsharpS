using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.GenericModel.Partial;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Model {
    public partial class AlunoUI : TO {
        public Aluno aluno { get; set; }
        public PessoaFisicaUI pessoaFisicaUI {get;set;}
        public int cdUsuarioFollowUp { get; set; }
        public bool isMaster { get; set; }
        //Propriedades de UI para o link:
        public ContatosUI contatosUI { get; set; }
        public IEnumerable<EnderecoUI> enderecosUI { get; set; }
        public IEnumerable<TelefoneSGF> telefonesUI { get; set; }
        public IEnumerable<RelacionamentoUI> relacionamentoUI { get; set; }
        public ICollection<Horario> horarioSearchUI { get; set; }
        public bool atualizaHorarios { get; set; }
        public bool atualizaFollowUp { get; set; }
        public Prospect prospect { get; set; }
        public PessoaRafUI pessoaRaf { get; set; }
        public FichaSaudeUI fichaSaude { get; set; }
        public ICollection<MotivoMatricula> motivosMatriculaUI { get; set; }
        public ICollection<AlunoRestricao> alunosRestricaoUI { get; set; }
        public ICollection<FollowUp> followUpUI { get;set; }

        public static Aluno changeValuesAluno(Aluno alunoContext,Aluno alunoView){
            alunoContext.cd_escolaridade = alunoView.cd_escolaridade;
            alunoContext.cd_midia = alunoView.cd_midia;
            alunoContext.id_aluno_ativo = alunoView.id_aluno_ativo;
            return alunoContext;
        }
    }
}

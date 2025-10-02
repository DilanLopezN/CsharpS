using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class FollowUp
    {
        public string no_turma { get; set; }
        public Horario horarios { get; set; }
        public enum TipoFollowUp {
            INTERNO = 1,
            PROSPECT_ALUNO = 2,
            ADMINISTRACAO_GERAL = 3
        }

        public enum TipoAtendimento
        {
            PESSOAL = 1,
            TELEFONICO = 2,
            E_MAIL = 3
        }

        public string _dc_assunto { 
            get
            {
                if(String.IsNullOrEmpty(this.dc_assunto))
                    return "";
                else if(this.dc_assunto.Contains("<") && this.dc_assunto.Contains(">"))
                    return "...";
                else
                    return this.dc_assunto;
            } 
        }

        public string no_usuario_origem { get; set; }
        public string no_usuario_destino { get; set; }
        public string no_aluno_prospect { get; set; }
        public int cd_aluno_follow_up { get; set; }
        public int cd_prospect_follow_up { get; set; }
        public string dc_acao { get; set; }
        public List<AcaoFollowUp> acaoeFollowUp { get; set; }
        public List<FollowUpEscola> escolas { get; set; }
        public int nroOrdem { get; set; }
        public bool id_alterado { get; set; }
        public virtual ICollection<FollowUp> followUpsOrigem { get; set; }
        public virtual ICollection<FollowUp> followUpsTodasRepostas { get; set; }
        public int qtd_lido { get; set; }
        public string dc_mail_pessoa { get; set; }
        public string dc_telefone_pessoa { get; set; }
        public List<Produto> produtos { get; set; }
        public bool id_email_enviado_view { get; set; }

        public string no_usuario
        {
            get
            {   
                string retorno = "";
                if (FollowUpUsuario != null)
                    retorno = FollowUpUsuario.no_login;
                return retorno;
            }
        }

        public string dta_proximo_contato
        {
            get
            {
                if(dt_proximo_contato.HasValue)
                    return String.Format("{0:dd/MM/yyyy}", dt_proximo_contato);
                else
                    return "";
            }
        }

        public string dta_follow_up
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy HH:mm:ss}", dt_follow_up.ToLocalTime());
            }
        }

        public string dc_tipo_atendimento
        {
            get
            {
                string retorno = "";
                if(id_tipo_atendimento != null && id_tipo_atendimento > 0)
                    switch (id_tipo_atendimento)
                    {
                        case (int)TipoAtendimento.PESSOAL: retorno = "Pessoal";  break;
                        case (int)TipoAtendimento.TELEFONICO: retorno = "Telefônico";  break;
                        case (int)TipoAtendimento.E_MAIL: retorno = "E-Mail"; break;
                    }
                return retorno;
            }
        }

        public static FollowUp changeValuesFollowUp(FollowUp followUpContext, FollowUp followUpView)
        {
            switch (followUpContext.id_tipo_follow)
            {
                case (int)TipoFollowUp.INTERNO:
                    followUpContext.cd_usuario = followUpView.cd_usuario;
                    followUpContext.cd_usuario_destino = followUpView.cd_usuario_destino;
                    followUpContext.id_usuario_administrador = followUpView.id_usuario_administrador;
                    break;
                case (int)TipoFollowUp.PROSPECT_ALUNO:
                    followUpContext.cd_prospect = followUpView.cd_prospect;
                    followUpContext.cd_aluno = followUpView.cd_aluno;
                    followUpContext.cd_acao_follow_up = followUpView.cd_acao_follow_up;
                    followUpContext.dt_proximo_contato = followUpView.dt_proximo_contato;
                    followUpContext.id_tipo_atendimento = followUpView.id_tipo_atendimento;
                    followUpContext.cd_turma = followUpView.cd_turma;
                    followUpContext.cd_usuario_destino = followUpView.cd_usuario_destino;
                    break;
                case (int)TipoFollowUp.ADMINISTRACAO_GERAL:
                    followUpContext.id_usuario_administrador = followUpView.id_usuario_administrador;
                    break;
            }
            followUpContext.id_follow_lido = followUpView.id_follow_lido;
            followUpContext.id_follow_resolvido = followUpView.id_follow_resolvido;
            followUpContext.dt_follow_up = followUpView.dt_follow_up;
            followUpContext.dc_assunto = followUpView.dc_assunto;
            return followUpContext;
        }

        public static FollowUp setarCamposDefaultPorTipo(FollowUp followUpView, bool user_master_geral)
        {
            switch (followUpView.id_tipo_follow)
            {
                case (int)TipoFollowUp.INTERNO:
                    followUpView.cd_usuario = followUpView.cd_usuario;
                    followUpView.cd_usuario_destino = followUpView.cd_usuario_destino;
                    followUpView.id_usuario_administrador = followUpView.id_usuario_administrador;
                    followUpView.cd_prospect = null;
                    followUpView.cd_aluno = null;
                    followUpView.dt_proximo_contato = null;
                    followUpView.cd_acao_follow_up = null;
                    break;
                case (int)TipoFollowUp.PROSPECT_ALUNO:
                    followUpView.cd_usuario_destino = followUpView.cd_usuario_destino;
                    followUpView.id_usuario_administrador = false;
                    followUpView.escolas = null;
                    break;
                case (int)TipoFollowUp.ADMINISTRACAO_GERAL:
                    followUpView.cd_usuario_destino = null;
                    //followUpView.escolas = null;
                    followUpView.cd_prospect = null;
                    followUpView.cd_aluno = null;
                    followUpView.dt_proximo_contato = null;
                    followUpView.cd_acao_follow_up = null;
                    followUpView.cd_escola = null;
                    break;
            }
            return followUpView;
        }

        public static bool retornarStatusPesquisa(int selecao)
        {
            return selecao == 1 ? true : false ;
        }

    }
}

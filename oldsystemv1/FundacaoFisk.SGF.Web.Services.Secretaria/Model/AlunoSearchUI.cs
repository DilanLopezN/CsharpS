using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Model
{
    public class AlunoSearchUI : TO
    {
        public AlunoSearchUI() { }
        public int cd_aluno { get; set; }
        public int cd_aluno_turma { get; set; }
        public int cd_turma { get; set; }
        public int? cd_contrato{ get; set; }
        public int cd_pessoa_aluno { get; set; }
        public int cd_pessoa_escola { get; set; }
        public int cd_situacao_aluno_origem { get; set; }
        public string no_pessoa { get; set; }
        public System.DateTime dt_cadastramento { get; set; }
        public System.DateTime? dt_nascimento { get; set; }
        public string dc_reduzido_pessoa { get; set; }
        public string nm_cpf { get; set; }
        public bool id_aluno_ativo { get; set; }
        public string ext_img_pessoa { get; set; }
        public string no_atividade { get; set; }
        public int ativExtra { get; set; }
        public int cd_pessoa { get; set; }
        public string email { get; set; }
        public int? cd_pessoa_dependente { get; set; }    
        public string no_pessoa_dependente { get; set; }
        public string telefone { get; set; }
        public string celular { get; set; }
        public string situacaoAluno { get; set; }
        public Nullable<decimal> pc_bolsa { get; set; }
        public Nullable<decimal> pc_bolsa_material { get; set; }
        public Nullable<DateTime> dt_inicio_bolsa { get; set; }
        public decimal? vl_abatimento_matricula { get; set; }
		public PessoaFisicaSGF PessoaSGFQueUsoOCpf { get; set; }
        public string dc_ano_escolar { get; set; }
        public string no_turma { get; set; }        
        public IEnumerable<Titulo> titulos { get; set; }
        public DateTime dt_inicio_aula { get; set; }
        public DateTime? dt_final_aula { get; set; }
        public string no_aluno { get; set; }
        public string dc_motivo_bolsa { get; set; }
        public string dc_reduzido_pessoa_escola { get; set; }
        public int cd_pessoa_escola_aluno { get; set; }
        public bool alterou_status_ativo { get; set; }
        public int? idade
        {
            get
            {
                int? retorna = null;
                if (dt_nascimento != null)
                {
                    DateTime dataHoje = DateTime.Now;
                    retorna = dataHoje.Year - dt_nascimento.Value.Year;
                    if ((dataHoje.Month < dt_nascimento.Value.Month) || (dataHoje.Month == dt_nascimento.Value.Month && dataHoje.Day < dt_nascimento.Value.Day))
                        retorna -= 1;
                }
                return retorna;
            }
        }

        public string aluno_ativo
        {
            get
            {
                return this.id_aluno_ativo ? "Sim" : "Não";
            }
        }

        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_aluno", "Código", AlinhamentoColuna.Center));
                retorno.Add(new DefinicaoRelatorio("no_pessoa", "Nome", AlinhamentoColuna.Left, "3.6000in"));
                retorno.Add(new DefinicaoRelatorio("nm_cpf", "CPF", AlinhamentoColuna.Center, "1.2000in"));
                retorno.Add(new DefinicaoRelatorio("email", "E-mail", AlinhamentoColuna.Center, "1.7000in"));
                retorno.Add(new DefinicaoRelatorio("telefone", "Telefone", AlinhamentoColuna.Center, "1.2000in"));
                //retorno.Add(new DefinicaoRelatorio("dc_reduzido_pessoa", "Nome Reduzido"));
                retorno.Add(new DefinicaoRelatorio("dta_cadastro", "Data Cadastro", AlinhamentoColuna.Center));
                //retorno.Add(new DefinicaoRelatorio("situacao", "Situação", AlinhamentoColuna.Center, "1.4000in"));
                retorno.Add(new DefinicaoRelatorio("id_ativo", "Ativo", AlinhamentoColuna.Center));

                return retorno;
            }
        }

        public string dta_cadastro
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_cadastramento);
            }
        }

        public string dta_nascimento
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_nascimento);
            }
        }

        public string id_ativo
        {
            get
            {
                return this.id_aluno_ativo ? "Sim" : "Não";
            }
        }

        public string nm_cpf_dependente
        {
            get
            {
                string retorno = "";
                if (!String.IsNullOrEmpty(nm_cpf))
                {
                    if (!string.IsNullOrEmpty(no_pessoa_dependente) && !string.IsNullOrEmpty(nm_cpf))
                        retorno = nm_cpf + "(" + no_pessoa_dependente + ")";
                    else
                        if (!string.IsNullOrEmpty(nm_cpf))
                            retorno = nm_cpf;
                }
                return retorno;
            }
        }

        public string nm_parcela_titulo_data_vcto
        {
            get
            {
                string retorno = "";
                if (titulos != null)
                {
                    foreach (var titulo in titulos)
                    {
                        retorno += titulo.nm_parcela_titulo;
                        if (titulo.nm_parcela_titulo.HasValue)
                            retorno += "-" + " (" + String.Format("{0:dd/MM/yyyy}", titulo.dt_vcto_titulo) + ")\n";        
                    }
                }
                return retorno;
            }
        }

        public string dtaIniAula
        {
            get
            {
                if (dt_inicio_aula != null)
                    return String.Format("{0:dd/MM/yyyy}", dt_inicio_aula);
                else
                    return String.Empty;
            }
        }

        public string dtaFim
        {
            get
            {
                if (dt_final_aula != null)
                    return String.Format("{0:dd/MM/yyyy}", dt_final_aula);
                else
                    return String.Empty;
            }
        }

        public static AlunoSearchUI fromAluno(Aluno aluno) {
            AlunoSearchUI alunoSearchUI = new AlunoSearchUI();

            alunoSearchUI.ativExtra = aluno.AtividadeExtraAluno.Where(at => at.ind_participacao == false).Count();
            alunoSearchUI.copy(aluno);
            alunoSearchUI.copy(aluno.AlunoPessoaFisica);
            if (aluno.AlunoPessoaFisica.PessoaSGFQueUsoOCpf != null && aluno.AlunoPessoaFisica.PessoaSGFQueUsoOCpf.cd_pessoa > 0)
                alunoSearchUI.nm_cpf = aluno.AlunoPessoaFisica.PessoaSGFQueUsoOCpf.nm_cpf;
            return alunoSearchUI;
        }
    }
}

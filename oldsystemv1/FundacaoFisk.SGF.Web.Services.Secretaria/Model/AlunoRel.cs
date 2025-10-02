using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Model
{
    public class AlunoRel : TO
    {
        public AlunoRel() { }
        public int cd_aluno { get; set; }
        public int cd_pessoa_aluno { get; set; }
        public string no_aluno { get; set; }
        public DateTime dt_cadastramento { get; set; }
        public int cd_resp { get; set; }
        public string no_resp { get; set; }
        public string nm_telefone { get; set; }
        public string nm_celular { get; set; }
        public string email_aluno { get; set; }
        public bool id_aluno_ativo { get; set; }
        public bool exibir_coluna { get; set; }
        public string dc_endereco { get; set; }
        public EnderecoSGF enderecoAluno { get; set; }
        public string nm_raf { get; set; }
        public string id_raf_liberado { get; set; }
        public string id_bloqueado { get; set; }

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
                retorno.Add(new DefinicaoRelatorio("no_aluno", "Nome", AlinhamentoColuna.Left, "1.3000in"));
                retorno.Add(new DefinicaoRelatorio("dta_cadastro", "Data Cadastro", AlinhamentoColuna.Center, "1.2000in"));
                retorno.Add(new DefinicaoRelatorio("no_resp", "Nome do Responsável", AlinhamentoColuna.Left, "1.9000in"));
                retorno.Add(new DefinicaoRelatorio("nm_telefone", "Telefone", AlinhamentoColuna.Center, "1.0000in"));
                retorno.Add(new DefinicaoRelatorio("nm_celular", "Celular", AlinhamentoColuna.Center, "1.0000in"));
                retorno.Add(new DefinicaoRelatorio("email_aluno", "E-Mail", AlinhamentoColuna.Center, "1.0000in"));
                retorno.Add(new DefinicaoRelatorio("nm_raf", "Raf", AlinhamentoColuna.Center, "1.0000in"));
                retorno.Add(new DefinicaoRelatorio("id_raf_liberado", "Liberado", AlinhamentoColuna.Center, "1.0000in"));
                retorno.Add(new DefinicaoRelatorio("id_bloqueado", "Bloqueado", AlinhamentoColuna.Center, "1.0000in"));
                retorno.Add(new DefinicaoRelatorio("aluno_ativo", "Ativo", AlinhamentoColuna.Center, "0.5in"));
                if (exibir_coluna)
                    retorno.Add(new DefinicaoRelatorio("dc_endereco", "Endereço", AlinhamentoColuna.Left, "2.0000in"));
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


    }


}

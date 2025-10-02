using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class ReajusteAnual
    {
        public List<NomeContrato> nomesContrato { get; set; }
        public enum StatusReajuste
        {
            ABERTO = 1,
            FECHADO = 2
        }

        public int qtd_turmas {get; set;}
        public int qtd_cursos {get; set;}
        public int qtd_alunos {get; set;}
        public int qtd_titulos {get; set;}
        public string hr_cadastro { get; set; }
        public string dta_cadastro
        {
            get {
                if (dh_cadastro_reajuste != null)
                    return String.Format("{0:dd/MM/yyyy}", dh_cadastro_reajuste);
                else
                    return String.Empty;
            }
        }

        public string hh_cadastro
        {
            get
            {
                if (dh_cadastro_reajuste != null)
                    return String.Format("{0:HH:mm:ss}", dh_cadastro_reajuste);
                else
                    return String.Empty;
            }
        }

        public string dh_cadastro
        {
            get
            {
                if (dh_cadastro_reajuste != null)
                    return String.Format("{0:dd/MM/yyyy HH:mm:ss}", dh_cadastro_reajuste);
                else
                    return String.Empty;
            }
        }

        public string dt_inicial_vcto
        {
            get {
                if (dt_inicial_vencimento != null)
                    return String.Format("{0:dd/MM/yyyy}", dt_inicial_vencimento);
                else
                    return String.Empty;
            }
        }

        public string dt_final_vcto
        {
            get {
                if (dt_final_vencimento != null)
                    return String.Format("{0:dd/MM/yyyy}", dt_final_vencimento);
                else
                    return String.Empty;
            }
        }

        public string pc_reajuste
        {
            get
            {
                if (this.pc_reajuste_anual == 0)
                    return "";
                return string.Format("{0:#,0.0000}", this.pc_reajuste_anual);
            }
        }

        public string vl_reajuste
        {
            get
            {
                if (this.vl_reajuste_anual == 0)
                    return "";
                return string.Format("{0:#,0.00}", this.vl_reajuste_anual);
            }
        }

        public string dc_status
        {
            get
            {
                string descStatus = "";
                if (id_status_reajuste == (int)StatusReajuste.ABERTO)
                    descStatus = "Aberto";
                else
                    descStatus = "Fechado";
                return descStatus;
            }
        }
        
        public string no_login
        {
            get
            {
                if (this.SysUsuario != null)
                    return this.SysUsuario.no_login;
                else
                    return "";
            }
        }
        
        public List<DefinicaoRelatorio> ColunasRelatorio {
            get {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                retorno.Add(new DefinicaoRelatorio("dh_cadastro", "Data Cadastro", AlinhamentoColuna.Left, "3.0000in"));
                retorno.Add(new DefinicaoRelatorio("no_login", "Usuário", AlinhamentoColuna.Left));
                retorno.Add(new DefinicaoRelatorio("pc_reajuste", "Percentual", AlinhamentoColuna.Center));
                retorno.Add(new DefinicaoRelatorio("vl_reajuste", "Valor", AlinhamentoColuna.Center));
                retorno.Add(new DefinicaoRelatorio("dt_inicial_vcto", "Vcto. Inicial", AlinhamentoColuna.Center));
                retorno.Add(new DefinicaoRelatorio("dt_final_vcto", "Vcto. Final", AlinhamentoColuna.Center));
                retorno.Add(new DefinicaoRelatorio("qtd_turmas", "Turmas", AlinhamentoColuna.Center));
                retorno.Add(new DefinicaoRelatorio("qtd_cursos", "Cursos", AlinhamentoColuna.Center));
                retorno.Add(new DefinicaoRelatorio("qtd_alunos", "Alunos", AlinhamentoColuna.Center));
                retorno.Add(new DefinicaoRelatorio("qtd_titulos", "Títulos", AlinhamentoColuna.Center));
                retorno.Add(new DefinicaoRelatorio("dc_status", "Status", AlinhamentoColuna.Center));

                return retorno;
            }
        }
    }
}

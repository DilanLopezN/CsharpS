using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Model
{
    public partial class PessoaSGFSearchUI
    {
        public string dta_cadastro
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_cadastramento.ToLocalTime());
            }
        }

        public string natureza_pessoa
        {
            get
            {
                return this.nm_natureza_pessoa == 1 ? "Física" : "Jurídica";
            }
        }
        //public List<DefinicaoRelatorio> ColunasRelatorio
        //{
        //    get
        //    {
        //        List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

        //        // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
        //        retorno.Add(new DefinicaoRelatorio("cd_pessoa", "Código", AlinhamentoColuna.Center));
        //        retorno.Add(new DefinicaoRelatorio("no_pessoa", "Nome", AlinhamentoColuna.Left, "3.8000in"));
        //        retorno.Add(new DefinicaoRelatorio("dc_reduzido_pessoa", "Nome Reduzido"));
        //        retorno.Add(new DefinicaoRelatorio("nm_cpf_cgc", "CPF\\CNPJ", AlinhamentoColuna.Center));
        //        retorno.Add(new DefinicaoRelatorio("pessoa_ativa", "Ativo", AlinhamentoColuna.Center));
        //        retorno.Add(new DefinicaoRelatorio("natureza_pessoa", "Natureza", AlinhamentoColuna.Center));
        //        retorno.Add(new DefinicaoRelatorio("dta_cadastro", "Data Cadastro"));

        //        return retorno;
        //    }
        //}
    }
}

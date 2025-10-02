using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class TipoNotaFiscal
    {
        public enum Movto
        {
            ENTRADA = 1,
            SAIDA = 2,
            SERVICO = 9
        }
        public string devolucao
        {
            get
            {
                return this.id_devolucao ? "Sim" : "Não";
            }
        }
        public string mtvoFinanc
        {
            get
            {
                return this.id_movimenta_financeiro ? "Sim" : "Não";
            }
        }
        public string mtvoEstoque
        {
            get
            {
                return this.id_movimenta_estoque ? "Sim" : "Não";
            }
        }
        public string ativo
        {
            get
            {
                return this.id_tipo_ativo ? "Sim" : "Não";
            }
        }
        public string nmSitTrib
        {
            get
            {
                string ret = "";
                if(this.SituacaoTributaria != null)
                    ret = this.SituacaoTributaria.nm_situacao_tributaria;
                return ret;
            }
        }
        public string movimento
        {
            get
            {
                string retorna = "";

                switch (this.id_natureza_movimento)
                {
                    case (int)Movto.ENTRADA: retorna = "Entrada";
                        break;
                    case (int)Movto.SAIDA: retorna = "Saída";
                        break;
                    case (int)Movto.SERVICO: retorna = "Serviço";
                        break;
                }
                return retorna;
            }
        }
        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();
                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_grupo_estoque", "Código"));
                retorno.Add(new DefinicaoRelatorio("dc_tipo_nota_fiscal", "Descrição", AlinhamentoColuna.Left, "1.6000in"));
                retorno.Add(new DefinicaoRelatorio("dc_natureza_operacao", "Nat. Operação", AlinhamentoColuna.Left, "1.2000in"));
                retorno.Add(new DefinicaoRelatorio("dc_CFOP", "CFOP", AlinhamentoColuna.Left, "0.5000in"));
                //retorno.Add(new DefinicaoRelatorio("nm_codigo_integracao", "Cód. Integração", AlinhamentoColuna.Center, "1.4000in"));
                retorno.Add(new DefinicaoRelatorio("movimento", "Movimento", AlinhamentoColuna.Center, "0.9500in"));
                retorno.Add(new DefinicaoRelatorio("nmSitTrib", "Sit. Trib.", AlinhamentoColuna.Center, "0.8000in"));
                retorno.Add(new DefinicaoRelatorio("mtvoEstoque", "Mov. Estoque", AlinhamentoColuna.Center, "1.2000in"));
                retorno.Add(new DefinicaoRelatorio("mtvoFinanc", "Mov. Financeiro", AlinhamentoColuna.Center, "1.3500in"));
                retorno.Add(new DefinicaoRelatorio("devolucao", "Devolução", AlinhamentoColuna.Center, "0.9000in"));
                retorno.Add(new DefinicaoRelatorio("ativo", "Ativo", AlinhamentoColuna.Center, "0.6000in"));

                return retorno;
            }
        }
    }
}

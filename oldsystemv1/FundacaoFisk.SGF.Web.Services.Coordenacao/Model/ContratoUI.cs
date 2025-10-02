using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Model
{
    public class ContratoUI : TO
    {
        public Contrato contrato { get; set; }
        public IEnumerable<TurmaSearch> turmas { get; set; }
        public Valores valores { get; set; }
        public TaxaMatriculaSearchUI taxa { get; set; }
        public IEnumerable<DescontoContrato> descontos { get; set; }
        public Cheque cheque { get; set; }
        public List<Duracao> duracoes { get; set; }
        public List<Produto> produtos { get; set; }
        public List<Regime> regimes { get; set; }
        public List<NomeContrato> nomesContrato  { get; set; }
        public List<TipoLiquidacao> tipoLiquidacoes  { get; set; }
        public IEnumerable<Banco> bancos { get; set; }
        public List<LocalMovto> localMovto { get; set; }
        public AlunoTurma alunoTurma { get; set; }
        public IEnumerable<Curso> cursos { get; set; }
        public OpcoesPagamentoUI opcoesPagamento { get; set; }
        public Parametro parametro { get; set; }
    }    
}

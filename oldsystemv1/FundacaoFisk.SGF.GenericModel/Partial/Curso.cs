using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel {
    public partial class Curso {
        public bool selecionadoCurso = false;

        public string curso_ativo {
            get {
                return this.id_curso_ativo ? "Sim" : "Não";
            }
        }
        public long? cd_curso_ordem { get; set; }
        private string _no_produto;
        public string no_produto {
            get {
                if(this.Produto != null && this.Produto.no_produto != null)
                    return this.Produto.no_produto;
                return _no_produto;
            }
            set {
                _no_produto = value;
            }
        }

        private string _dc_nivel;
        public string dc_nivel
        {
            get
            {
                if (this.Nivel != null && this.Nivel.dc_nivel != null)
                    return this.Nivel.dc_nivel;
                return _dc_nivel;
            }
            set
            {
                _dc_nivel = value;
            }
        }

        private string _no_estagio;
        public string no_estagio {
            get {
                if(this.Estagio != null && this.Estagio.no_estagio != null)
                    return this.Estagio.no_estagio;
                return _no_estagio;
            }
            set {
                _no_estagio = value;
            }
        }

        private string _no_modalidade;
        public string no_modalidade {
            get {
                if(this.Modalidade != null && this.Modalidade.no_modalidade != null)
                    return this.Modalidade.no_modalidade;
                return _no_modalidade;
            }
            set {
                _no_modalidade = value;
            }
        }

        public List<DefinicaoRelatorio> ColunasRelatorio {
            get {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_curso", "Código"));
                retorno.Add(new DefinicaoRelatorio("no_curso", "Nome", AlinhamentoColuna.Left, "2.0000in"));
                retorno.Add(new DefinicaoRelatorio("no_produto", "Produto", AlinhamentoColuna.Left, "1.3000in"));
                retorno.Add(new DefinicaoRelatorio("no_estagio", "Estágio", AlinhamentoColuna.Left, "1.5000in"));
                retorno.Add(new DefinicaoRelatorio("no_modalidade", "Série", AlinhamentoColuna.Left, "1.3000in"));
                retorno.Add(new DefinicaoRelatorio("dc_nivel", "Nível", AlinhamentoColuna.Left, "1.3000in"));
                retorno.Add(new DefinicaoRelatorio("curso_ativo", "Ativo", AlinhamentoColuna.Center, "0.7000in"));

                return retorno;
            }
        }
    }
}

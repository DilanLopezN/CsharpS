using Componentes.GenericModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class CalendarioAcademico
    {
        enum TipoCalendarioAcademico
        {
            InicioAulas = 1,
            AtividadesExtras = 2,
            Programacao = 3
        }

        public string dc_tipo_calendario
        {
            get
            {
                if (cd_tipo_calendario == (byte)TipoCalendarioAcademico.InicioAulas)
                    return "Inicio das Aulas";
                if (cd_tipo_calendario == (byte)TipoCalendarioAcademico.AtividadesExtras)
                    return "Atividades Extras";
                if (cd_tipo_calendario == (byte)TipoCalendarioAcademico.Programacao)
                    return "Programaçao";
                return "";
            }
        }

        public string id_ativo_formatado
        {
            get
            {
                return this.id_ativo ? "Sim" : "Não";
            }
        }

        public string id_mostrar_todos_formatado
        {
            get
            {
                return this.id_mostrar_todos ? "Sim" : "Não";
            }
        }

        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_evento", "Código"));
                retorno.Add(new DefinicaoRelatorio("dc_tipo_calendario", "Tipo", AlinhamentoColuna.Left, "4.5000in"));
                retorno.Add(new DefinicaoRelatorio("id_mostrar_todos_formatado", "Mostrar Todos", AlinhamentoColuna.Center, "2.0000in"));
                retorno.Add(new DefinicaoRelatorio("id_ativo_formatado", "Ativo", AlinhamentoColuna.Center, "0.9000in"));

                return retorno;
            }
        }
    }
}

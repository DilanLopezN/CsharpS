using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class Prospect
    {
        public EnderecoSGF endereco { get; set; }
        public enum MotivoInativaProspect 
        {
            MATRICULA_ESCOLA_CONCORRENTE = 1,
            N_RECEBER_MALA_DIRETA_EMAIL_MKT = 2,
            N_TEM_INTERESSE_CURSO = 3,
        }

        public enum PeriodoProspect
        {
             MANHA = 1,
             TARDE = 2,
             NOITE = 3
        }

        public enum TipoRelatorio
        {
            PROSPECTS_ATENDIDOS = 1,
            PROSPECTS_ATENDIDOS_MOTIVO_NAO_MATRICULA = 2,
            PROSPECT_ATENDIDOS_MATRÍCULAS = 3,
            COMPARATIVO_PROSPECTS_ATENDIDOS = 4
        }

        public enum TipoStatusConsultado
        {
            CONSULTADO = 1,
            NAO_CONSULTADO = 0
        }

        public enum TipoOnline
        {
            TESTE_CLASSIFICACAO = 1,
            PRE_MATRICULA_ONLINE = 2
        }
    }
}

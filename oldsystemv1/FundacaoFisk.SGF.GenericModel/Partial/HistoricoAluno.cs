using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel {
   public partial class HistoricoAluno
   {
       public enum SituacaoHistorico {
           MOVIDO = 0,
           ATIVO = 1,
           DESISTENTE = 2,
           TRANSFERIDO = 3,
           ENCERRADO = 4,
           DEPENDENTE = 5,
           ADAPTACAO = 6,
           REMANEJADO = 7,
           REMATRICULADO = 8,
           AGUARDANDO = 9,
           MATRICULADOSMATERIAL = 10,
           CANCELADO = 11
        }

        public enum TipoMovimento {
           MATRICULA = 0,
           MUDANCA_INTERNA = 1,
           ENCERRADO = 2,
           PRIMEIRA_AULA = 3,
           ULTIMA_AULA = 4, //???
           DESISTENCIA = 5,
           REMATRICULA = 6,
           CANCELAR_DESISTENCIA = 7,
           CANCELA_ENCERRAMENTO = 8,
           TRANSFERENCIA = 9,
           CANCELAMENTO = 11,
           CANCELAR_CANCELAMENTO = 12
       }
       
       public string dtaCadastro {
           get {
               if(dt_cadastro.HasValue)
                   return String.Format("{0:dd/MM/yyyy HH:mm:ss}", dt_cadastro.Value.ToLocalTime());
               else
                   return String.Empty;
           }
       }

       public string dtaHistorico {
           get {
               return String.Format("{0:dd/MM/yyyy}", dt_historico);
           }
       }

       public string situacao_historico {
           get {
               string retorno = "";
               switch(this.id_situacao_historico) {
                   case (int) SituacaoHistorico.MOVIDO:
                       retorno = "Movido";
                       break;
                   case (int) SituacaoHistorico.ATIVO:
                       retorno = "Ativo";
                       break;
                   case (int) SituacaoHistorico.DESISTENTE:
                       retorno = "Desistente";
                       break;
                   case (int) SituacaoHistorico.TRANSFERIDO:
                       retorno = "Transferido";
                       break;
                   case (int) SituacaoHistorico.ENCERRADO:
                       retorno = "Encerrado";
                       break;
                   case (int) SituacaoHistorico.DEPENDENTE:
                       retorno = "Dependente";
                       break;
                   case (int) SituacaoHistorico.ADAPTACAO:
                       retorno = "Adaptação";
                       break;
                   case (int) SituacaoHistorico.REMATRICULADO:
                       retorno = "Rematriculado";
                       break;
                   case (int) SituacaoHistorico.AGUARDANDO:
                       retorno = "Aguardando";
                       break;
                    case (int)SituacaoHistorico.MATRICULADOSMATERIAL:
                        retorno = "Matriculado s/Material";
                        break;
                    case (int)SituacaoHistorico.CANCELADO:
                        retorno = "Matricula Cancelada";
                        break;
                }
                return retorno;
            }
       }

       public string tipo_movimento {
           get {
               string retorno = "";
               switch(this.id_tipo_movimento) {
                   case (int) TipoMovimento.MATRICULA:
                       retorno = "Matrícula";
                       break;
                   case (int) TipoMovimento.MUDANCA_INTERNA:
                       retorno = "Mudança Interna";
                       break;
                   case (int) TipoMovimento.PRIMEIRA_AULA:
                       retorno = "Primeira Aula";
                       break;
                   case (int) TipoMovimento.ULTIMA_AULA:
                       retorno = "Última Aula";
                       break;
                   case (int) TipoMovimento.DESISTENCIA:
                       retorno = "Desistência";
                       break;
                   case (int)TipoMovimento.CANCELAR_DESISTENCIA:
                       retorno = "Cancelamento de Desistência";
                       break;
                   case (int)TipoMovimento.REMATRICULA:
                       retorno = "Rematricula";
                       break;
                   case (int)TipoMovimento.CANCELA_ENCERRAMENTO:
                       retorno = "Cancelamento do Encerramento";
                       break;
                    case (int)TipoMovimento.TRANSFERENCIA:
                        retorno = "Transferência";
                        break;
                    case (int)TipoMovimento.CANCELAMENTO:
                        retorno = "Cancelamento";
                        break;
                    case (int)TipoMovimento.CANCELAR_CANCELAMENTO:
                        retorno = "Cancelar Cancelamento";
                        break;
                }
                return retorno;
           }
       }
    }
}

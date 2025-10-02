﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class MotivoDesistencia
    {
        public enum motivoDesistencia
        {
            TRANSFERENCIA = 1
        }
        public string motivoDesistenciaAtivo
        {
            get
            {
                return this.id_motivo_desistencia_ativo ? "Sim" : "Não";
            }
        }
        
        public string isCancelamento
        {
            get
            {
                return this.id_cancelamento ? "Sim" : "Não";
            }
        }

        public List<DefinicaoRelatorio> ColunasRelatorio {
            get {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_motivo_desistencia", "Código"));
                retorno.Add(new DefinicaoRelatorio("dc_motivo_desistencia", "Motivo Desistência", AlinhamentoColuna.Left, "3.8000in"));
                retorno.Add(new DefinicaoRelatorio("motivoDesistenciaAtivo", "Ativa", AlinhamentoColuna.Center));
                retorno.Add(new DefinicaoRelatorio("isCancelamento", "Cancelamento", AlinhamentoColuna.Center));
                return retorno;
            }
        }
    }
}

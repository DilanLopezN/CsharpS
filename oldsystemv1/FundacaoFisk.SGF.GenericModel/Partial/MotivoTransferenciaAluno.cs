﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class MotivoTransferenciaAluno : TO
    {
        public string motivo_transferencia_ativo
        {
            get
            {
                return this.id_motivo_transferencia_ativo ? "Sim" : "Não";
            }
        }
        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_motivo_nao_matricula", "Código"));
                retorno.Add(new DefinicaoRelatorio("dc_motivo_transferencia_aluno", "Descrição", AlinhamentoColuna.Left, "3.8000in"));
                retorno.Add(new DefinicaoRelatorio("motivo_transferencia_ativo", "Ativo", AlinhamentoColuna.Center));

                return retorno;
            }
        }

    }
}

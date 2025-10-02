using System;

namespace FundacaoFisk.SGF.Services.Coordenacao.Model
{
    public class TurmasHistoricoUI
    {
        public int cd_estagio { get; set; }
        public int cd_turma { get; set; }
        public int cd_produto { get; set; }
        public int nm_aulas_dadas { get; set; }
        public int nm_aulas_contratadas { get; set; }
        public byte nm_faltas { get; set; }
        public DateTime dt_aula_min { get; set; }
        public DateTime dt_aula_max { get; set; }
        public byte nm_sequencia { get; set; }
    }
}
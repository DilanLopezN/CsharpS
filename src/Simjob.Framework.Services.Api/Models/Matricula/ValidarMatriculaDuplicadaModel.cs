using System;
using System.Collections.Generic;

namespace Simjob.Framework.Services.Api.Models.Matricula
{
    public class ValidarMatriculaDuplicadaModel
    {
        public int cd_aluno { get; set; }
        public int cd_produto_atual { get; set; }
        public int cd_pessoa_escola { get; set; }
        public DateTime dt_inicial_contrato { get; set; }
        public DateTime? dt_final_contrato { get; set; } // OPCIONAL - será calculada se não fornecida
        public int cd_curso_atual { get; set; }
        public int? cd_duracao { get; set; } // Para cálculo automático da data final
        public int? cd_contrato_ignorar { get; set; } // Contrato a ser ignorado na validação (para edições)
        public List<CursoContratoValidacaoModel>? CursoContrato { get; set; }
    }

    public class CursoContratoValidacaoModel
    {
        public int cd_curso { get; set; }
    }
}
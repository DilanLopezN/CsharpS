using System;
using System.Collections.Generic;

namespace Simjob.Framework.Services.Api.Models.Matricula
{
    public class ValidarMatriculaInputModel
    {
        public string? cd_aluno { get; set; }
        public string? cd_produto { get; set; }
        public string? cd_curso { get; set; }
        public string? dt_inicial_matricula { get; set; }
    }

}
using System;
using System.Collections.Generic;

namespace Simjob.Framework.Services.Api.Models.FilaMatricula
{
    public class InsertFilaMatriculaModel
    {
        public int nm_sexo { get; set; }

        public string no_pessoa { get; set; }
        public int cd_pessoa_escola { get; set; }
        public string email { get; set; }
        public string telefone { get; set; }
        public string celular { get; set; }
        public int cd_acao { get; set; }
        public string nm_cpf { get; set; }
        public int id_status_fila { get; set; }
        public int? cd_produto { get; set; }
        public DateTime dt_programada_contato { get; set; }
        public int? cd_curso_recomendado { get; set; }
        //public object no_responsavel { get; set; }      
        public List<Dia> dias { get; set; } = new List<Dia>();
        public List<Periodo> periodos { get; set; } = new List<Periodo>();     

        public class Dia
        {
            public int id_dia_semana { get; set; }
        }

        public class Periodo
        {
            public int id_periodo { get; set; }
        }

    }
}

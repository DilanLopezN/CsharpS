using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Model {
    public class CursoSearch : TO{
        public int cd_curso { get; set; }
        public int cd_estagio { get; set; }
        public Nullable<int> cd_modalidade { get; set; }
        public int cd_produto { get; set; }
        public Nullable<int> cd_proximo_curso { get; set; }
        
        public Nullable<short> nm_carga_horaria { get; set; }
        public int nm_carga_maxima { get; set; }
        public string no_curso { get; set; }
        public string no_produto { get; set; }
        public string no_estagio { get; set; }
        public Nullable<int> cd_nivel { get; set; }
        public string dc_nivel { get; set; }
        public string no_modalidade { get; set; }
        public bool id_curso_ativo { get; set; }
        public bool id_permitir_matricula { get; set; }
        public bool id_certificado { get; set; }
        public Nullable<byte> nm_faixa_etaria_maxima { get; set; }
        public Nullable<short> nm_faixa_etaria_minima { get; set; }
        public Nullable<int> nm_total_nota { get; set; }
        public Nullable<byte> nm_vagas_curso { get; set; }
        public Nullable<decimal> pc_criterio_aprovacao { get; set; }
    }
}

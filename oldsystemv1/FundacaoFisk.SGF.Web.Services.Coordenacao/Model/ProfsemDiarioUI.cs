using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Model
{
    public class ProfsemDiarioUI : TO
    {
        public int cd_pessoa_empresa { get; set; }
        public string dc_reduzido_pessoa { get; set; }
        public string no_professor { get; set; }
        public string no_turma { get; set; }
        public int qtd_programacao { get; set; }
        public bool id_liberado { get; set; }
        public int cd_turma     { get; set; }
        public int cd_professor { get; set; }
        public string liberado
        {
            get
            {
                if (Convert.ToBoolean(this.id_liberado))
                    return "Sim";
                else
                    return "Não";
            }
        }
    }
}

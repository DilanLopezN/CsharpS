using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;
namespace FundacaoFisk.SGF.Services.Coordenacao.Model
{
    public class AtividadeExtraRecorrenciaUI : TO
    {
        public int cd_atividade_extra { get; set; }
        public int cd_tipo_atividade_extra { get; set; }
        public System.DateTime dt_atividade_extra { get; set; }
        public System.TimeSpan hh_inicial { get; set; }
        public System.TimeSpan hh_final { get; set; }
        public Nullable<int> nm_vagas { get; set; }
        public Nullable<int> cd_produto { get; set; }
        public int cd_funcionario { get; set; }
        public bool ind_carga_horaria { get; set; }
        public bool ind_pagar_professor { get; set; }
        public string tx_obs_atividade { get; set; }
        public int cd_usuario_atendente { get; set; }
        public Nullable<int> cd_sala { get; set; }
        public int cd_pessoa_escola { get; set; }
        public bool id_calendario_academico { get; set; }
        public Nullable<System.TimeSpan> hr_limite_academico { get; set; }
        public bool id_email_enviado { get; set; }
        public Nullable<int> cd_atividade_recorrrencia { get; set; }

        public  AtividadeRecorrencia AtividadeRecorrencia { get; set; }
        
    }
}
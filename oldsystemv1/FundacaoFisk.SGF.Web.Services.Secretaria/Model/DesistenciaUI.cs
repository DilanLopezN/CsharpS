using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Model
{
    public class DesistenciaUI : TO
    {
        public int cd_desistencia { get; set; }
        public int cd_pessoa { get; set; }
        public int cd_aluno { get; set; }
        public string no_pessoa { get; set; }
        public int cd_turma { get; set; }
        public string no_turma { get; set; }
        public Nullable<int> cd_motivo_desistencia { get; set; }
        public string dc_motivo_desistencia { get; set; }
        public System.DateTime dt_desistencia { get; set; }
        public byte id_tipo_desistencia { get; set; }
        public string dc_tipo { get; set; }
        public int cd_aluno_turma { get; set; }
        public string tx_obs_desistencia { get; set; }
        public int cd_usuario { get; set; }
        public int fuso { get; set; }
        public int? cd_contrato { get; set; }
        public int? cd_pessoa_responsavel_titulo { get; set; }
        public bool id_cancelamento { get; set; }
        public bool id_aluno_ativo { get; set; }
        public int cd_tipo_liquidacao { get; set; }
        public int cd_local_movto { get; set; }
        public int nm_ordem { get; set; }
        public string telefone { get; set; }
        public ICollection<Titulo> titulos { get; set; }
        public ChequeTransacao chequeTransacao { get; set; }

        public string dtaDesistencia
        {
            get
            {
                if (dt_desistencia != null)
                    return String.Format("{0:dd/MM/yyyy}", dt_desistencia);
                else
                    return String.Empty;
            }
            set { }
        }

        public static DesistenciaUI fromDesistenciaUI(DesistenciaUI desistenciaUI)
        {
            string tipo = "Cancelamento";
            if(desistenciaUI.id_tipo_desistencia == (byte)DesistenciaDataAccess.tipoDesistencia.DESISTENCIA)
                tipo = "Desistência";
            else
                if (desistenciaUI.id_tipo_desistencia == (byte)DesistenciaDataAccess.tipoDesistencia.NAOREMATRICULA)
                    tipo = "Não Rematrícula"; 
                else
                    if (desistenciaUI.id_tipo_desistencia == (byte)DesistenciaDataAccess.tipoDesistencia.CANCELAMENTONAOREMATRICULA)
                        tipo = "Cancelamento de Não Rematrícula"; 
            DesistenciaUI retornDesistenciaUI = new DesistenciaUI
            {
                
                 
                cd_desistencia = desistenciaUI.cd_desistencia,
                cd_aluno = desistenciaUI.cd_aluno,
                cd_motivo_desistencia = desistenciaUI.cd_motivo_desistencia,
                cd_pessoa = desistenciaUI.cd_aluno_turma,
                cd_turma = desistenciaUI.cd_turma,
                dc_motivo_desistencia = desistenciaUI.dc_motivo_desistencia,
                no_pessoa = desistenciaUI.no_pessoa,
                no_turma = desistenciaUI.no_turma,
                dt_desistencia = desistenciaUI.dt_desistencia,
                dc_tipo = tipo,
                id_tipo_desistencia = desistenciaUI.id_tipo_desistencia,
                cd_aluno_turma = desistenciaUI.cd_aluno_turma,
                tx_obs_desistencia = desistenciaUI.tx_obs_desistencia,
                dtaDesistencia = desistenciaUI.dtaDesistencia,
                cd_contrato = desistenciaUI.cd_contrato,
                cd_pessoa_responsavel_titulo = desistenciaUI.cd_pessoa_responsavel_titulo,
                id_cancelamento = desistenciaUI.id_cancelamento,
                id_aluno_ativo = desistenciaUI.id_aluno_ativo
            };
            return retornDesistenciaUI;
        }

        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_curso", "Código"));
                retorno.Add(new DefinicaoRelatorio("no_turma", "Turma", AlinhamentoColuna.Left, "2.6000in"));
                retorno.Add(new DefinicaoRelatorio("no_pessoa", "Aluno", AlinhamentoColuna.Left, "2.5000in"));
                retorno.Add(new DefinicaoRelatorio("telefone", "Telefone", AlinhamentoColuna.Left, "1.2000in"));
                retorno.Add(new DefinicaoRelatorio("dc_motivo_desistencia", "Motivo", AlinhamentoColuna.Left, "1.3000in"));
                retorno.Add(new DefinicaoRelatorio("dtaDesistencia", "Data", AlinhamentoColuna.Left, "0.9000in"));
                retorno.Add(new DefinicaoRelatorio("dc_tipo", "Tipo", AlinhamentoColuna.Left, "1.2000in"));

                return retorno;
            }
        }

    }
}

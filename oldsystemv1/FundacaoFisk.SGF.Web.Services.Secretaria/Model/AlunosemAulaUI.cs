using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Model
{
	public class AlunosemAulaUI : TO
	{
		public AlunosemAulaUI() { }
		public string dc_reduzido_pessoa { get; set; }
		public string no_pessoa { get; set; }
		public int cd_pessoa_empresa { get; set; }
		public int cd_movimento { get; set; }
		public int? nm_movimento { get; set; }
		public DateTime dt_emissao_movimento { get; set; }
		public int cd_item { get; set; }
		public string no_item { get; set; }
		public int cd_item_movimento { get; set; }
		public int qt_item_movimento { get; set; }
		public double vl_unitario_item { get; set; }
		public int cd_aluno { get; set; }
		public string no_turma { get; set; }
		public string Movimentacao { get; set; }
		public DateTime dt_historico { get; set; }
		public byte id_situacao_historico { get; set; }
		public string nm_raf { get; set; }
		public string dta_movimento
		{
			get
			{
				return String.Format("{0:dd/MM/yyyy}", this.dt_emissao_movimento);
			}
		}
		public string dta_historico
		{
			get
			{
				return String.Format("{0:dd/MM/yyyy}", this.dt_historico);
			}
		}

	}
}

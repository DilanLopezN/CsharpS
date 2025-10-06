using System.Collections.Generic;

namespace Simjob.Framework.Services.Api.Models.Turmas
{

  public class ItemProgramacaoModel
  {
    public int cd_item_programacao { get; set; }
    public int cd_programacao_curso { get; set; }
    public int nm_aula_programacao { get; set; }
    public string dc_aula_programacao { get; set; }
  }
  // Model para T_PROGRAMACAO_CURSO (cabeçalho)
  public class ProgramacaoCursoModel
  {
    public int cd_programacao_curso { get; set; }
    public int cd_curso { get; set; }
    public int cd_duracao { get; set; }
    public int? cd_escola { get; set; }
    public List<ItemProgramacaoModel> Itens { get; set; } = new List<ItemProgramacaoModel>();
  }

  // Model para T_ITEM_PROGRAMACAO (detalhes das aulas)

}

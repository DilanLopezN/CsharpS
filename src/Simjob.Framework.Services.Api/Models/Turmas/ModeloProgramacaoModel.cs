namespace Simjob.Framework.Services.Api.Models.Turmas
{
  public class ModeloProgramacaoModel
  {
    public int cd_modelo_programacao { get; set; }
    public int cd_curso { get; set; }
    public int cd_produto { get; set; }
    public int cd_regime { get; set; }
    public int cd_duracao { get; set; }
    public int cd_duracao_aula { get; set; }
    public string dia_semana { get; set; }
    public string hr_inicial { get; set; }
    public string hr_final { get; set; }
    public string dc_programacao { get; set; }
    public string nm_aula_auxiliar { get; set; }
    public bool id_modelo_ativo { get; set; }
  }
}

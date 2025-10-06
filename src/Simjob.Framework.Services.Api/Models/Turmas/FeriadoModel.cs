using System;

namespace Simjob.Framework.Services.Api.Models.Turmas
{
  public class FeriadoModel
  {
    public int cod_feriado { get; set; }
    public int? cd_pessoa_escola { get; set; }
    public int dd_feriado { get; set; }
    public int aa_feriado { get; set; }
    public int mm_feriado { get; set; }
    public string dc_feriado { get; set; }
    public bool id_feriado_financeiro { get; set; }
    public int? aa_feriado_fim { get; set; }
    public int? mm_feriado_fim { get; set; }
    public int? dd_feriado_fim { get; set; }
    public bool id_feriado_ativo { get; set; }
    public int? cd_ferias { get; set; }

    // Propriedade helper para facilitar uso
    public DateTime DataFeriado
    {
      get
      {
        try
        {
          return new DateTime(aa_feriado, mm_feriado, dd_feriado);
        }
        catch
        {
          return DateTime.MinValue;
        }
      }
    }
  }
}

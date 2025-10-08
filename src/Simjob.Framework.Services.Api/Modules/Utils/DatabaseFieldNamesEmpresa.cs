using System.Collections.Generic;

namespace Simjob.Framework.Services.Api.Constants
{
  /// <summary>
  /// Classe de constantes para padronizar os campos que referenciam empresa/escola no sistema.
  ///
  /// PROBLEMA RESOLVIDO:
  /// O sistema usa três nomenclaturas diferentes para o mesmo conceito (empresa/escola):
  /// - cd_pessoa_escola (maioria das tabelas)
  /// - cd_empresa (T_CAIXA, T_EMPRESA, T_PESSOA_EMPRESA e algumas views)
  ///
  /// SOLUÇÃO:
  /// Esta classe centraliza o mapeamento, permitindo que a API use sempre "cd_empresa"
  /// como parâmetro, enquanto internamente converte para o campo correto de cada tabela.
  /// </summary>
  public static class DatabaseFieldNames
  {
    #region Constantes Principais

    /// <summary>
    /// Campo padrão usado na maioria das tabelas para referenciar empresa/escola
    /// </summary>
    public const string EMPRESA_FIELD_DEFAULT = "cd_pessoa_escola";

    /// <summary>
    /// Campo alternativo usado em algumas tabelas específicas
    /// </summary>
    public const string EMPRESA_FIELD_ALTERNATIVE = "cd_empresa";

    #endregion

    #region Mapeamento Completo de Tabelas e Views

    /// <summary>
    /// Dicionário que mapeia cada tabela/view para o nome correto do campo que representa empresa/escola.
    ///
    /// REGRA:
    /// - cd_pessoa_escola: maioria das tabelas de negócio
    /// - cd_empresa: tabelas de infraestrutura (T_CAIXA, T_EMPRESA, T_PESSOA_EMPRESA) e algumas views
    /// </summary>
    public static readonly Dictionary<string, string> EmpresaFieldByTable = new Dictionary<string, string>
        {
            // ========== TABELAS (T_*) ==========

            // Tabelas que usam cd_pessoa_escola
            { "T_ACAO", "cd_pessoa_escola" },
            { "T_ALUNO", "cd_pessoa_escola" },
            { "T_ATIVIDADE_CURSO", "cd_pessoa_escola" },
            { "T_ATIVIDADE_EXTRA", "cd_pessoa_escola" },
            { "T_AULA_REPOSICAO", "cd_pessoa_escola" },
            { "T_BIBLIOTECA_SEC", "cd_pessoa_escola" },
            { "T_CALENDARIO_ACADEMICO", "cd_pessoa_escola" },
            { "T_CALENDARIO_EVENTO", "cd_pessoa_escola" },
            { "T_CONTATO", "cd_pessoa_escola" },
            { "T_CONTATO_ARQUIVO", "cd_pessoa_escola" },
            { "T_CONTRATO", "cd_pessoa_escola" },
            { "T_DESCONTO", "cd_pessoa_escola" },
            { "T_FERIADO", "cd_pessoa_escola" },
            { "T_FERIAS", "cd_pessoa_escola" },
            { "T_FILA_MATRICULA", "cd_pessoa_escola" },
            { "T_HISTORICO_PESSOA", "cd_pessoa_escola" },
            { "T_HORARIO", "cd_pessoa_escola" },
            { "T_ITEM_ESCOLA", "cd_pessoa_escola" },
            { "T_NOME_CONTRATO", "cd_pessoa_escola" },
            { "T_PARAMETRO", "cd_pessoa_escola" },
            { "T_POLITICA_DESCONTO", "cd_pessoa_escola" },
            { "T_PROSPECT", "cd_pessoa_escola" },
            { "T_REAJUSTE_ANUAL", "cd_pessoa_escola" },
            { "T_SALA", "cd_pessoa_escola" },
            { "T_TABELA_MODIFICADA", "cd_pessoa_escola" },
            { "T_TABELA_PRECO", "cd_pessoa_escola" },
            { "T_TABELA_PRECO_MATERIAL", "cd_pessoa_escola" },
            { "T_TIPO_DESCONTO_ESCOLA", "cd_pessoa_escola" },
            { "T_TURMA", "cd_pessoa_escola" },
            { "T_MODELO_PROGRAMACAO", "cd_pessoa_escola" },

            // Tabelas que usam cd_empresa (exceções)
            { "T_CAIXA", "cd_empresa" },
            { "T_EMPRESA", "cd_empresa" },
            { "T_PESSOA_EMPRESA", "cd_empresa" },

            // ========== VIEWS (V_*, vi_*) ==========

            // Views que usam cd_pessoa_escola
            { "V_aditamento", "cd_pessoa_escola" },
            { "v_aditamento_historico", "cd_pessoa_escola" },
            { "V_ALUNO", "cd_pessoa_escola" },
            { "V_ALUNO_BOLSA", "cd_pessoa_escola" },
            { "V_CONTRATO", "cd_pessoa_escola" },
            { "V_FILAMATRICULA", "cd_pessoa_escola" },
            { "vi_acao_listagem", "cd_pessoa_escola" },
            { "vi_acao_pipeline", "cd_pessoa_escola" },
            { "vi_aluno", "cd_pessoa_escola" },
            { "vi_bairro_contato", "cd_pessoa_escola" },
            { "vi_contato_listagem", "cd_pessoa_escola" },
            { "vi_contrato", "cd_pessoa_escola" },
            { "vi_contrato_grid_turma", "cd_pessoa_escola" },
            { "vi_contrato_id", "cd_pessoa_escola" },
            { "vi_contrato_titulos", "cd_pessoa_escola" },
            { "vi_contrato1", "cd_pessoa_escola" },
            { "vi_curso_aluno", "cd_pessoa_escola" },
            { "vi_curso_mensagem", "cd_pessoa_escola" },
            { "vi_desistencia_carga", "cd_pessoa_escola" },
            { "vi_duracao_turma", "cd_pessoa_escola" },
            { "vi_fila_matricula", "cd_pessoa_escola" },
            { "vi_funcionario", "cd_pessoa_escola" },
            { "vi_horario_turma", "cd_pessoa_escola" },
            { "vi_informacao_contrato", "cd_pessoa_escola" },
            { "vi_item_tabela_material", "cd_pessoa_escola" },
            { "vi_nome_turma", "cd_pessoa_escola" },
            { "vi_pre_turma", "cd_pessoa_escola" },
            { "vi_produto_aluno", "cd_pessoa_escola" },
            { "vi_produto_curso", "cd_pessoa_escola" },
            { "vi_produto_mensagem", "cd_pessoa_escola" },
            { "vi_professor_turma", "cd_pessoa_escola" },
            { "vi_raf_sem_diario", "cd_pessoa_escola" },
            { "vi_sala_turma", "cd_pessoa_escola" },
            { "vi_tabela_preco_curso", "cd_pessoa_escola" },
            { "vi_turma", "cd_pessoa_escola" },
            { "vi_turma_professor", "cd_pessoa_escola" },
            { "vi_usuario_pipeline", "cd_pessoa_escola" },

            // Views que usam cd_empresa (exceções)
            { "V_PESSOA", "cd_empresa" },
            { "vi_pessoa", "cd_empresa" },
            { "vi_professor", "cd_empresa" },

            // Views de programação (adicionadas recentemente)
            { "vi_programacao_completa", "cd_pessoa_escola" },
            { "vi_modelo_programacao_completa", "cd_pessoa_escola" }
        };

    #endregion

    #region Métodos Auxiliares

    /// <summary>
    /// Retorna o nome correto do campo que representa empresa/escola para uma tabela/view específica.
    /// </summary>
    /// <param name="tableName">Nome da tabela ou view (case-insensitive)</param>
    /// <returns>Nome do campo (cd_pessoa_escola ou cd_empresa)</returns>
    /// <example>
    /// <code>
    /// string campo = DatabaseFieldNames.GetEmpresaField("T_FILA_MATRICULA");
    /// // Retorna: "cd_pessoa_escola"
    ///
    /// string campo = DatabaseFieldNames.GetEmpresaField("T_CAIXA");
    /// // Retorna: "cd_empresa"
    /// </code>
    /// </example>
    public static string GetEmpresaField(string tableName)
    {
      if (string.IsNullOrWhiteSpace(tableName))
        return EMPRESA_FIELD_DEFAULT;

      // Busca case-insensitive
      var tableKey = tableName.Trim();

      // Tenta encontrar a chave exata (case-insensitive)
      foreach (var kvp in EmpresaFieldByTable)
      {
        if (string.Equals(kvp.Key, tableKey, System.StringComparison.OrdinalIgnoreCase))
        {
          return kvp.Value;
        }
      }

      // Se não encontrou, retorna o padrão
      return EMPRESA_FIELD_DEFAULT;
    }

    /// <summary>
    /// Verifica se uma tabela/view usa cd_empresa ao invés de cd_pessoa_escola
    /// </summary>
    /// <param name="tableName">Nome da tabela ou view</param>
    /// <returns>True se usa cd_empresa, False se usa cd_pessoa_escola</returns>
    public static bool UsesAlternativeField(string tableName)
    {
      return GetEmpresaField(tableName) == EMPRESA_FIELD_ALTERNATIVE;
    }

    /// <summary>
    /// Retorna uma lista de todas as tabelas que usam cd_empresa (exceções)
    /// </summary>
    public static List<string> GetTablesUsingCdEmpresa()
    {
      var tables = new List<string>();
      foreach (var kvp in EmpresaFieldByTable)
      {
        if (kvp.Value == EMPRESA_FIELD_ALTERNATIVE)
        {
          tables.Add(kvp.Key);
        }
      }
      return tables;
    }

    /// <summary>
    /// Retorna estatísticas sobre o uso dos campos
    /// </summary>
    public static (int totalTables, int usingCdPessoaEscola, int usingCdEmpresa) GetStatistics()
    {
      int total = EmpresaFieldByTable.Count;
      int cdEmpresa = 0;
      int cdPessoaEscola = 0;

      foreach (var kvp in EmpresaFieldByTable)
      {
        if (kvp.Value == EMPRESA_FIELD_ALTERNATIVE)
          cdEmpresa++;
        else
          cdPessoaEscola++;
      }

      return (total, cdPessoaEscola, cdEmpresa);
    }

    #endregion

    #region Documentação de Uso

    /// <summary>
    /// GUIA DE USO PARA DESENVOLVEDORES:
    ///
    /// 1. PARÂMETROS DE API:
    ///    - Sempre use "cd_empresa" (int) como parâmetro nos controllers
    ///    - Exemplo: public async Task<IActionResult> GetList(int cd_empresa)
    ///
    /// 2. QUERIES AO BANCO:
    ///    - Use GetEmpresaField() para obter o nome correto do campo
    ///    - Exemplo:
    ///      string campoEmpresa = DatabaseFieldNames.GetEmpresaField("T_FILA_MATRICULA");
    ///      // Retorna "cd_pessoa_escola"
    ///
    /// 3. INSERÇÃO/UPDATE:
    ///    - Sempre documente quando usar cd_empresa na T_PESSOA_EMPRESA
    ///    - Exemplo:
    ///      // T_PESSOA_EMPRESA: cd_empresa referencia o cd_pessoa da escola
    ///      var dict = new Dictionary<string, object>
    ///      {
    ///          { "cd_pessoa", cd_pessoa },
    ///          { "cd_empresa", cd_pessoa_escola }  // ID da escola
    ///      };
    ///
    /// 4. FILTROS:
    ///    - Use o campo retornado por GetEmpresaField() nos filtros
    ///    - Exemplo:
    ///      string campoFiltro = DatabaseFieldNames.GetEmpresaField(tableName);
    ///      var filtros = new List<(string, object)>
    ///      {
    ///          (campoFiltro, cd_empresa)
    ///      };
    ///
    /// 5. TABELAS ESPECIAIS (usam cd_empresa):
    ///    - T_CAIXA
    ///    - T_EMPRESA
    ///    - T_PESSOA_EMPRESA
    ///    - V_PESSOA / vi_pessoa
    ///    - vi_professor
    ///
    /// IMPORTANTE:
    /// - NUNCA hardcode "cd_pessoa_escola" ou "cd_empresa" diretamente
    /// - SEMPRE use esta classe para obter o nome do campo
    /// - DOCUMENTE quando o comportamento for diferente do padrão
    /// </summary>
    public const string USAGE_GUIDE = "Consulte a documentação XML desta classe para guia completo de uso";

    #endregion
  }
}

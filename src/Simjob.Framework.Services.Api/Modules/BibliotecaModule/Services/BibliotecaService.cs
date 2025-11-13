using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Simjob.Framework.Infra.Identity.Entities;

namespace Simjob.Framework.Services.Api.Modules.BibliotecaModule.Services
{
  public class BibliotecaService
  {
    private const int ORIGEM_EMPRESTIMO = 6; // cd_origem para Emprestimo na T_ORIGEM

    public async Task<List<Dictionary<string, object>>> GetPessoaBibliotecaSearch(
        Source source,
        string nome,
        string apelido,
        bool inicio,
        int tipoPessoa,
        string cnpjCpf,
        int sexo,
        int cd_empresa,
        int skip = 0,
        int take = 20,
        string sort = "no_pessoa",
        string sortDirection = "asc")
    {
      var connectionString = $"Server={source.Host};Database={source.DbName};User Id={source.User};Password={source.Password};MultipleActiveResultSets=True;";
      var results = new List<Dictionary<string, object>>();

      using (var connection = new SqlConnection(connectionString))
      {
        await connection.OpenAsync();

        var query = @"
                    SELECT DISTINCT
                        p.cd_pessoa,
                        p.no_pessoa,
                        p.dc_reduzido_pessoa,
                        CASE
                            WHEN pf.cd_pessoa_fisica IS NOT NULL THEN pf.cpf_pessoa_fisica
                            WHEN pj.cd_pessoa_juridica IS NOT NULL THEN pj.cnpj_pessoa_juridica
                            ELSE ''
                        END as documento,
                        CASE
                            WHEN pf.cd_pessoa_fisica IS NOT NULL THEN pf.nm_sexo
                            ELSE NULL
                        END as nm_sexo
                    FROM T_PESSOA p
                    LEFT JOIN T_PESSOA_FISICA pf ON p.cd_pessoa = pf.cd_pessoa_fisica
                    LEFT JOIN T_PESSOA_JURIDICA pj ON p.cd_pessoa = pj.cd_pessoa_juridica
                    WHERE p.id_ativo = 1";

        var parameters = new List<SqlParameter>();

        if (!string.IsNullOrEmpty(nome))
        {
          if (inicio)
            query += " AND p.no_pessoa LIKE @nome + '%'";
          else
            query += " AND p.no_pessoa LIKE '%' + @nome + '%'";
          parameters.Add(new SqlParameter("@nome", nome));
        }

        if (!string.IsNullOrEmpty(apelido))
        {
          if (inicio)
            query += " AND p.dc_reduzido_pessoa LIKE @apelido + '%'";
          else
            query += " AND p.dc_reduzido_pessoa LIKE '%' + @apelido + '%'";
          parameters.Add(new SqlParameter("@apelido", apelido));
        }

        if (!string.IsNullOrEmpty(cnpjCpf))
        {
          query += " AND (pf.cpf_pessoa_fisica = @doc OR pj.cnpj_pessoa_juridica = @doc)";
          parameters.Add(new SqlParameter("@doc", cnpjCpf));
        }

        if (sexo > 0)
        {
          query += " AND pf.nm_sexo = @sexo";
          parameters.Add(new SqlParameter("@sexo", sexo));
        }

        query += $" ORDER BY {sort} {sortDirection} OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY";
        parameters.Add(new SqlParameter("@skip", skip));
        parameters.Add(new SqlParameter("@take", take));

        using (var cmd = new SqlCommand(query, connection))
        {
          cmd.Parameters.AddRange(parameters.ToArray());
          using (var reader = await cmd.ExecuteReaderAsync())
          {
            while (await reader.ReadAsync())
            {
              var row = new Dictionary<string, object>();
              for (int i = 0; i < reader.FieldCount; i++)
              {
                row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
              }
              results.Add(row);
            }
          }
        }
      }

      return results;
    }

    public async Task<List<Dictionary<string, object>>> GetEmprestimoSearch(
        Source source,
        int? cd_pessoa,
        int? cd_item,
        bool? pendentes,
        DateTime? dt_inicial,
        DateTime? dt_final,
        bool? emprestimos,
        bool? devolucao,
        int cd_empresa,
        int skip = 0,
        int take = 20,
        string sort = "dt_emprestimo",
        string sortDirection = "desc")
    {
      var connectionString = $"Server={source.Host};Database={source.DbName};User Id={source.User};Password={source.Password};MultipleActiveResultSets=True;";
      var results = new List<Dictionary<string, object>>();

      using (var connection = new SqlConnection(connectionString))
      {
        await connection.OpenAsync();

        var query = @"
                    SELECT
                        b.cd_biblioteca,
                        b.cd_pessoa,
                        b.cd_item,
                        b.dt_emprestimo,
                        b.dt_prevista_devolucao,
                        b.dt_devolucao,
                        b.vl_taxa_emprestimo,
                        b.vl_multa_emprestimo,
                        b.tx_obs_biblioteca,
                        p.no_pessoa,
                        i.no_item
                    FROM T_BIBLIOTECA_SEC b
                    INNER JOIN T_PESSOA p ON b.cd_pessoa = p.cd_pessoa
                    INNER JOIN T_ITEM i ON b.cd_item = i.cd_item
                    INNER JOIN T_ITEM_ESCOLA ie ON i.cd_item = ie.cd_item
                    WHERE ie.cd_pessoa_escola = @cd_empresa";

        var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@cd_empresa", cd_empresa)
                };

        if (cd_pessoa.HasValue)
        {
          query += " AND b.cd_pessoa = @cd_pessoa";
          parameters.Add(new SqlParameter("@cd_pessoa", cd_pessoa.Value));
        }

        if (cd_item.HasValue)
        {
          query += " AND b.cd_item = @cd_item";
          parameters.Add(new SqlParameter("@cd_item", cd_item.Value));
        }

        if (pendentes.HasValue && pendentes.Value)
        {
          query += " AND b.dt_devolucao IS NULL";
        }

        if (emprestimos.HasValue && emprestimos.Value)
        {
          if (dt_inicial.HasValue)
          {
            query += " AND CAST(b.dt_emprestimo AS DATE) >= @dt_inicial";
            parameters.Add(new SqlParameter("@dt_inicial", dt_inicial.Value.Date));
          }
          if (dt_final.HasValue)
          {
            query += " AND CAST(b.dt_emprestimo AS DATE) <= @dt_final";
            parameters.Add(new SqlParameter("@dt_final", dt_final.Value.Date));
          }
        }

        if (devolucao.HasValue && devolucao.Value)
        {
          if (dt_inicial.HasValue)
          {
            query += " AND CAST(b.dt_devolucao AS DATE) >= @dt_inicial_dev";
            parameters.Add(new SqlParameter("@dt_inicial_dev", dt_inicial.Value.Date));
          }
          if (dt_final.HasValue)
          {
            query += " AND CAST(b.dt_devolucao AS DATE) <= @dt_final_dev";
            parameters.Add(new SqlParameter("@dt_final_dev", dt_final.Value.Date));
          }
        }

        query += $" ORDER BY {sort} {sortDirection} OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY";
        parameters.Add(new SqlParameter("@skip", skip));
        parameters.Add(new SqlParameter("@take", take));

        using (var cmd = new SqlCommand(query, connection))
        {
          cmd.Parameters.AddRange(parameters.ToArray());
          using (var reader = await cmd.ExecuteReaderAsync())
          {
            while (await reader.ReadAsync())
            {
              var row = new Dictionary<string, object>();
              for (int i = 0; i < reader.FieldCount; i++)
              {
                row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
              }
              results.Add(row);
            }
          }
        }
      }

      return results;
    }

    public async Task<Dictionary<string, object>> GetEmprestimo(
        Source source,
        int cd_biblioteca,
        int cd_empresa)
    {
      var connectionString = $"Server={source.Host};Database={source.DbName};User Id={source.User};Password={source.Password};MultipleActiveResultSets=True;";

      using (var connection = new SqlConnection(connectionString))
      {
        await connection.OpenAsync();

        var query = @"
                    SELECT
                        b.cd_biblioteca,
                        b.cd_pessoa,
                        b.cd_item,
                        b.dt_emprestimo,
                        b.dt_prevista_devolucao,
                        b.dt_devolucao,
                        b.vl_taxa_emprestimo,
                        b.vl_multa_emprestimo,
                        b.tx_obs_biblioteca,
                        b.cd_pessoa_escola,
                        p.no_pessoa,
                        i.no_item,
                        par.nm_dias_biblioteca,
                        par.id_bloquear_alt_dta_biblio,
                        par.pc_taxa_dia_biblioteca
                    FROM T_BIBLIOTECA_SEC b
                    INNER JOIN T_PESSOA p ON b.cd_pessoa = p.cd_pessoa
                    INNER JOIN T_ITEM i ON b.cd_item = i.cd_item
                    INNER JOIN T_ITEM_ESCOLA ie ON i.cd_item = ie.cd_item
                    INNER JOIN T_PARAMETRO par ON par.cd_pessoa_escola = ie.cd_pessoa_escola
                    WHERE b.cd_biblioteca = @cd_biblioteca
                    AND ie.cd_pessoa_escola = @cd_empresa";

        using (var cmd = new SqlCommand(query, connection))
        {
          cmd.Parameters.AddWithValue("@cd_biblioteca", cd_biblioteca);
          cmd.Parameters.AddWithValue("@cd_empresa", cd_empresa);

          using (var reader = await cmd.ExecuteReaderAsync())
          {
            if (await reader.ReadAsync())
            {
              var row = new Dictionary<string, object>();
              for (int i = 0; i < reader.FieldCount; i++)
              {
                row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
              }

              // Calcular multa se não tiver devolução
              if (row["dt_devolucao"] == null || row["dt_devolucao"] == DBNull.Value)
              {
                row["existe_devolucao"] = false;
                row["dt_devolucao"] = DateTime.UtcNow.Date;

                var dtPrevista = Convert.ToDateTime(row["dt_prevista_devolucao"]);
                var dtDevolucao = DateTime.UtcNow.Date;
                var diferencaDias = (dtDevolucao - dtPrevista).Days;

                if (diferencaDias > 0 && row["pc_taxa_dia_biblioteca"] != DBNull.Value)
                {
                  var taxaDia = Convert.ToDecimal(row["pc_taxa_dia_biblioteca"]);
                  row["vl_multa_emprestimo"] = Math.Round(diferencaDias * taxaDia, 2, MidpointRounding.AwayFromZero);
                }
              }
              else
              {
                row["existe_devolucao"] = true;
              }

              return row;
            }
          }
        }
      }

      return null;
    }

    public async Task<(bool success, string error, int cd_biblioteca)> AddEmprestimo(
        Source source,
        Dictionary<string, object> emprestimo,
        int cd_empresa,
        int saldo)
    {
      if (saldo <= 0)
      {
        return (false, $"Item {emprestimo["no_item"]} sem saldo em estoque", 0);
      }

      var dtEmprestimo = Convert.ToDateTime(emprestimo["dt_emprestimo"]);
      var dtPrevista = Convert.ToDateTime(emprestimo["dt_prevista_devolucao"]);

      if (dtPrevista < dtEmprestimo)
      {
        return (false, "Data prevista de devolução não pode ser menor que data de empréstimo", 0);
      }

      var connectionString = $"Server={source.Host};Database={source.DbName};User Id={source.User};Password={source.Password};MultipleActiveResultSets=True;";

      using (var connection = new SqlConnection(connectionString))
      {
        await connection.OpenAsync();

        using (var transaction = connection.BeginTransaction())
        {
          try
          {
            // Inserir empréstimo
            var insertQuery = @"
                            INSERT INTO T_BIBLIOTECA_SEC
                            (cd_pessoa, cd_item, dt_emprestimo, dt_prevista_devolucao, vl_taxa_emprestimo, tx_obs_biblioteca, cd_pessoa_escola)
                            VALUES
                            (@cd_pessoa, @cd_item, @dt_emprestimo, @dt_prevista_devolucao, @vl_taxa_emprestimo, @tx_obs_biblioteca, @cd_pessoa_escola);
                            SELECT CAST(SCOPE_IDENTITY() as int)";

            int cd_biblioteca;
            using (var cmd = new SqlCommand(insertQuery, connection, transaction))
            {
              cmd.Parameters.AddWithValue("@cd_pessoa", emprestimo["cd_pessoa"]);
              cmd.Parameters.AddWithValue("@cd_item", emprestimo["cd_item"]);
              cmd.Parameters.AddWithValue("@dt_emprestimo", dtEmprestimo);
              cmd.Parameters.AddWithValue("@dt_prevista_devolucao", dtPrevista);
              cmd.Parameters.AddWithValue("@vl_taxa_emprestimo", emprestimo.ContainsKey("vl_taxa_emprestimo") ? emprestimo["vl_taxa_emprestimo"] : 0);
              cmd.Parameters.AddWithValue("@tx_obs_biblioteca", emprestimo.ContainsKey("tx_obs_biblioteca") ? emprestimo["tx_obs_biblioteca"] ?? "" : "");
              cmd.Parameters.AddWithValue("@cd_pessoa_escola", cd_empresa);

              cd_biblioteca = (int)await cmd.ExecuteScalarAsync();
            }

            // Decrementar estoque
            var updateEstoqueQuery = @"
                            UPDATE T_ITEM_ESCOLA
                            SET qt_estoque = qt_estoque - 1
                            WHERE cd_item = @cd_item AND cd_pessoa_escola = @cd_empresa";

            using (var cmd = new SqlCommand(updateEstoqueQuery, connection, transaction))
            {
              cmd.Parameters.AddWithValue("@cd_item", emprestimo["cd_item"]);
              cmd.Parameters.AddWithValue("@cd_empresa", cd_empresa);
              await cmd.ExecuteNonQueryAsync();
            }

            // Buscar custo do item
            decimal vlKardex = 0;
            var custoQuery = "SELECT vl_custo FROM T_ITEM_ESCOLA WHERE cd_item = @cd_item AND cd_pessoa_escola = @cd_empresa";
            using (var cmd = new SqlCommand(custoQuery, connection, transaction))
            {
              cmd.Parameters.AddWithValue("@cd_item", emprestimo["cd_item"]);
              cmd.Parameters.AddWithValue("@cd_empresa", cd_empresa);
              var result = await cmd.ExecuteScalarAsync();
              if (result != null && result != DBNull.Value)
              {
                vlKardex = Convert.ToDecimal(result);
              }
            }

            // Inserir kardex (saída)
            var insertKardexQuery = @"
                            INSERT INTO T_KARDEX
                            (cd_pessoa_empresa, cd_item, cd_origem, cd_registro_origem, dt_kardex, id_tipo_movimento, qtd_kardex, nm_documento, tx_obs_kardex, vl_kardex)
                            VALUES
                            (@cd_pessoa_empresa, @cd_item, @cd_origem, @cd_registro_origem, @dt_kardex, @id_tipo_movimento, @qtd_kardex, @nm_documento, @tx_obs_kardex, @vl_kardex)";

            using (var cmd = new SqlCommand(insertKardexQuery, connection, transaction))
            {
              cmd.Parameters.AddWithValue("@cd_pessoa_empresa", cd_empresa);
              cmd.Parameters.AddWithValue("@cd_item", emprestimo["cd_item"]);
              cmd.Parameters.AddWithValue("@cd_origem", ORIGEM_EMPRESTIMO);
              cmd.Parameters.AddWithValue("@cd_registro_origem", cd_biblioteca);
              cmd.Parameters.AddWithValue("@dt_kardex", dtEmprestimo.Date);
              cmd.Parameters.AddWithValue("@id_tipo_movimento", 2); // SAIDA
              cmd.Parameters.AddWithValue("@qtd_kardex", 1);
              cmd.Parameters.AddWithValue("@nm_documento", cd_biblioteca.ToString());
              cmd.Parameters.AddWithValue("@tx_obs_kardex", $"Empréstimo de {emprestimo["no_pessoa"]}.");
              cmd.Parameters.AddWithValue("@vl_kardex", vlKardex);
              await cmd.ExecuteNonQueryAsync();
            }

            transaction.Commit();
            return (true, null, cd_biblioteca);
          }
          catch (Exception ex)
          {
            transaction.Rollback();
            return (false, ex.Message, 0);
          }
        }
      }
    }

    public async Task<(bool success, string error)> EditEmprestimo(
        Source source,
        Dictionary<string, object> emprestimo,
        int cd_empresa)
    {
      var dtEmprestimo = Convert.ToDateTime(emprestimo["dt_emprestimo"]);
      var dtPrevista = Convert.ToDateTime(emprestimo["dt_prevista_devolucao"]);
      var dtDevolucao = emprestimo.ContainsKey("dt_devolucao") && emprestimo["dt_devolucao"] != null
          ? Convert.ToDateTime(emprestimo["dt_devolucao"])
          : (DateTime?)null;

      if (dtDevolucao.HasValue && dtDevolucao.Value < dtEmprestimo)
      {
        return (false, "Data de devolução não pode ser menor que data de empréstimo");
      }

      if (dtPrevista < dtEmprestimo)
      {
        return (false, "Data prevista de devolução não pode ser menor que data de empréstimo");
      }

      var connectionString = $"Server={source.Host};Database={source.DbName};User Id={source.User};Password={source.Password};MultipleActiveResultSets=True;";

      using (var connection = new SqlConnection(connectionString))
      {
        await connection.OpenAsync();

        // Buscar empréstimo atual
        DateTime? dtDevolucaoAnterior = null;
        int cd_item = 0;
        int cd_pessoa = 0;
        string no_pessoa = "";

        var selectQuery = "SELECT dt_devolucao, cd_item, cd_pessoa, p.no_pessoa FROM T_BIBLIOTECA_SEC b INNER JOIN T_PESSOA p ON b.cd_pessoa = p.cd_pessoa WHERE cd_biblioteca = @cd_biblioteca";
        using (var cmd = new SqlCommand(selectQuery, connection))
        {
          cmd.Parameters.AddWithValue("@cd_biblioteca", emprestimo["cd_biblioteca"]);
          using (var reader = await cmd.ExecuteReaderAsync())
          {
            if (await reader.ReadAsync())
            {
              dtDevolucaoAnterior = reader.IsDBNull(0) ? null : reader.GetDateTime(0);
              cd_item = reader.GetInt32(1);
              cd_pessoa = reader.GetInt32(2);
              no_pessoa = reader.GetString(3);
            }
          }
        }

        using (var transaction = connection.BeginTransaction())
        {
          try
          {
            // Atualizar empréstimo
            var updateQuery = @"
                            UPDATE T_BIBLIOTECA_SEC
                            SET dt_emprestimo = @dt_emprestimo,
                                dt_prevista_devolucao = @dt_prevista_devolucao,
                                dt_devolucao = @dt_devolucao,
                                vl_taxa_emprestimo = @vl_taxa_emprestimo,
                                vl_multa_emprestimo = @vl_multa_emprestimo,
                                tx_obs_biblioteca = @tx_obs_biblioteca
                            WHERE cd_biblioteca = @cd_biblioteca";

            using (var cmd = new SqlCommand(updateQuery, connection, transaction))
            {
              cmd.Parameters.AddWithValue("@dt_emprestimo", dtEmprestimo);
              cmd.Parameters.AddWithValue("@dt_prevista_devolucao", dtPrevista);
              cmd.Parameters.AddWithValue("@dt_devolucao", (object)dtDevolucao ?? DBNull.Value);
              cmd.Parameters.AddWithValue("@vl_taxa_emprestimo", emprestimo.ContainsKey("vl_taxa_emprestimo") ? emprestimo["vl_taxa_emprestimo"] : 0);
              cmd.Parameters.AddWithValue("@vl_multa_emprestimo", emprestimo.ContainsKey("vl_multa_emprestimo") ? emprestimo["vl_multa_emprestimo"] ?? (object)DBNull.Value : DBNull.Value);
              cmd.Parameters.AddWithValue("@tx_obs_biblioteca", emprestimo.ContainsKey("tx_obs_biblioteca") ? emprestimo["tx_obs_biblioteca"] ?? "" : "");
              cmd.Parameters.AddWithValue("@cd_biblioteca", emprestimo["cd_biblioteca"]);
              await cmd.ExecuteNonQueryAsync();
            }

            // Se é primeira devolução
            if (dtDevolucao.HasValue && !dtDevolucaoAnterior.HasValue)
            {
              // Incrementar estoque
              var updateEstoqueQuery = @"
                                UPDATE T_ITEM_ESCOLA
                                SET qt_estoque = qt_estoque + 1
                                WHERE cd_item = @cd_item AND cd_pessoa_escola = @cd_empresa";

              using (var cmd = new SqlCommand(updateEstoqueQuery, connection, transaction))
              {
                cmd.Parameters.AddWithValue("@cd_item", cd_item);
                cmd.Parameters.AddWithValue("@cd_empresa", cd_empresa);
                await cmd.ExecuteNonQueryAsync();
              }

              // Buscar custo do item
              decimal vlKardex = 0;
              var custoQuery = "SELECT vl_custo FROM T_ITEM_ESCOLA WHERE cd_item = @cd_item AND cd_pessoa_escola = @cd_empresa";
              using (var cmd = new SqlCommand(custoQuery, connection, transaction))
              {
                cmd.Parameters.AddWithValue("@cd_item", cd_item);
                cmd.Parameters.AddWithValue("@cd_empresa", cd_empresa);
                var result = await cmd.ExecuteScalarAsync();
                if (result != null && result != DBNull.Value)
                {
                  vlKardex = Convert.ToDecimal(result);
                }
              }

              // Inserir kardex (entrada - devolução)
              var insertKardexQuery = @"
                                INSERT INTO T_KARDEX
                                (cd_pessoa_empresa, cd_item, cd_origem, cd_registro_origem, dt_kardex, id_tipo_movimento, qtd_kardex, nm_documento, tx_obs_kardex, vl_kardex)
                                VALUES
                                (@cd_pessoa_empresa, @cd_item, @cd_origem, @cd_registro_origem, @dt_kardex, @id_tipo_movimento, @qtd_kardex, @nm_documento, @tx_obs_kardex, @vl_kardex)";

              using (var cmd = new SqlCommand(insertKardexQuery, connection, transaction))
              {
                cmd.Parameters.AddWithValue("@cd_pessoa_empresa", cd_empresa);
                cmd.Parameters.AddWithValue("@cd_item", cd_item);
                cmd.Parameters.AddWithValue("@cd_origem", ORIGEM_EMPRESTIMO);
                cmd.Parameters.AddWithValue("@cd_registro_origem", emprestimo["cd_biblioteca"]);
                cmd.Parameters.AddWithValue("@dt_kardex", dtDevolucao.Value);
                cmd.Parameters.AddWithValue("@id_tipo_movimento", 1); // ENTRADA
                cmd.Parameters.AddWithValue("@qtd_kardex", 1);
                cmd.Parameters.AddWithValue("@nm_documento", emprestimo["cd_biblioteca"].ToString());
                cmd.Parameters.AddWithValue("@tx_obs_kardex", $"Devolução de {no_pessoa}.");
                cmd.Parameters.AddWithValue("@vl_kardex", vlKardex);
                await cmd.ExecuteNonQueryAsync();
              }
            }
            // Se alterou data de devolução existente
            else if (dtDevolucao.HasValue && dtDevolucaoAnterior.HasValue && dtDevolucao.Value != dtDevolucaoAnterior.Value)
            {
              // Atualizar data do kardex
              var updateKardexQuery = @"
                                UPDATE T_KARDEX
                                SET dt_kardex = @dt_kardex
                                WHERE cd_origem = @cd_origem
                                AND cd_registro_origem = @cd_registro_origem
                                AND id_tipo_movimento = 1";

              using (var cmd = new SqlCommand(updateKardexQuery, connection, transaction))
              {
                cmd.Parameters.AddWithValue("@dt_kardex", dtDevolucao.Value);
                cmd.Parameters.AddWithValue("@cd_origem", ORIGEM_EMPRESTIMO);
                cmd.Parameters.AddWithValue("@cd_registro_origem", emprestimo["cd_biblioteca"]);
                await cmd.ExecuteNonQueryAsync();
              }
            }

            transaction.Commit();
            return (true, null);
          }
          catch (Exception ex)
          {
            transaction.Rollback();
            return (false, ex.Message);
          }
        }
      }
    }

    public async Task<(bool success, string error)> DeleteEmprestimo(
        Source source,
        int cd_biblioteca,
        int cd_empresa)
    {
      var connectionString = $"Server={source.Host};Database={source.DbName};User Id={source.User};Password={source.Password};MultipleActiveResultSets=True;";

      using (var connection = new SqlConnection(connectionString))
      {
        await connection.OpenAsync();

        // Buscar empréstimo
        DateTime? dtDevolucao = null;
        int cd_item = 0;

        var selectQuery = "SELECT dt_devolucao, cd_item FROM T_BIBLIOTECA_SEC WHERE cd_biblioteca = @cd_biblioteca";
        using (var cmd = new SqlCommand(selectQuery, connection))
        {
          cmd.Parameters.AddWithValue("@cd_biblioteca", cd_biblioteca);
          using (var reader = await cmd.ExecuteReaderAsync())
          {
            if (await reader.ReadAsync())
            {
              dtDevolucao = reader.IsDBNull(0) ? null : reader.GetDateTime(0);
              cd_item = reader.GetInt32(1);
            }
            else
            {
              return (false, "Empréstimo não encontrado");
            }
          }
        }

        using (var transaction = connection.BeginTransaction())
        {
          try
          {
            // Se não teve devolução, incrementa estoque
            if (!dtDevolucao.HasValue)
            {
              var updateEstoqueQuery = @"
                                UPDATE T_ITEM_ESCOLA
                                SET qt_estoque = qt_estoque + 1
                                WHERE cd_item = @cd_item AND cd_pessoa_escola = @cd_empresa";

              using (var cmd = new SqlCommand(updateEstoqueQuery, connection, transaction))
              {
                cmd.Parameters.AddWithValue("@cd_item", cd_item);
                cmd.Parameters.AddWithValue("@cd_empresa", cd_empresa);
                await cmd.ExecuteNonQueryAsync();
              }
            }

            // Excluir kardex
            var deleteKardexQuery = @"
                            DELETE FROM T_KARDEX
                            WHERE cd_origem = @cd_origem
                            AND cd_registro_origem = @cd_registro_origem";

            using (var cmd = new SqlCommand(deleteKardexQuery, connection, transaction))
            {
              cmd.Parameters.AddWithValue("@cd_origem", ORIGEM_EMPRESTIMO);
              cmd.Parameters.AddWithValue("@cd_registro_origem", cd_biblioteca);
              await cmd.ExecuteNonQueryAsync();
            }

            // Excluir empréstimo
            var deleteQuery = "DELETE FROM T_BIBLIOTECA_SEC WHERE cd_biblioteca = @cd_biblioteca";
            using (var cmd = new SqlCommand(deleteQuery, connection, transaction))
            {
              cmd.Parameters.AddWithValue("@cd_biblioteca", cd_biblioteca);
              await cmd.ExecuteNonQueryAsync();
            }

            transaction.Commit();
            return (true, null);
          }
          catch (Exception ex)
          {
            transaction.Rollback();
            return (false, ex.Message);
          }
        }
      }
    }
  }
}

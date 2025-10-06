
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Simjob.Framework.Infra.Data.Models;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Services.Api.Enums;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Simjob.Framework.Services.Api.Services
{
    public static class SQLServerService
    {
        public static async Task RunSQLServerCommand(string connectionString, string sql)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand(sql, connection))
                {
                    await command.ExecuteNonQueryAsync();
                }
            }

        }

        public static async Task<bool> CheckIfTableExists(string connectionString, string tableName)
        {
            var sql = $"SELECT OBJECT_ID('{tableName}', 'U')";

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand(sql, connection))
                {
                    var result = await command.ExecuteScalarAsync();
                    return result != DBNull.Value && result != null;
                }
            }
        }

        //Tabelas
        public static string GenerateCreateTableSql(string tableName, Dictionary<string, object> jsonSchema)
        {
            if (!jsonSchema.TryGetValue("properties", out var propertiesObj))
                throw new Exception("Schema não contém 'properties'.");
            var autoIncrementResult = jsonSchema.TryGetValue("autoIncrement", out var result) ? result : false;

            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"CREATE TABLE {tableName} (");

            bool autoIncrement = false;
            if (autoIncrementResult is bool boolValue)
            {
                autoIncrement = boolValue;
            }

            if (autoIncrement)
            {
                var columnName = "id";
                sb.AppendLine($"  {columnName} BIGINT PRIMARY KEY IDENTITY(1,1),");
            }
            else
            {
                var columnName = "Id";
                sb.AppendLine($"  {columnName} UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),");
            }

            if (jsonSchema["properties"] is JObject properties)
            {
                foreach (var prop in properties)
                {
                    var columnName = prop.Key;
                    var fieldData = prop.Value;
                    string type = fieldData?["type"]?.ToString() ?? "undefined";
                    var sqlType = MapJsonTypeToSqlType(type);

                    sb.AppendLine($"  {columnName} {sqlType},");
                }
            }

            sb.Length -= 3;
            sb.AppendLine("\n);");

            return sb.ToString();
        }
        public static async Task AlterTableSqlServer(string conectionString, string tableName, Dictionary<string, object> jsonSchema, Dictionary<string, object> jsonSchemaUpdate)
        {
            if (!jsonSchema.TryGetValue("properties", out var propertiesObj))
                throw new Exception("Schema não contém 'properties'.");

            if (jsonSchema["properties"] is JObject properties)
            {
                properties.Properties().Select(p => p.Name).ToList();
            }

            if (!jsonSchemaUpdate.TryGetValue("properties", out var propertiesObjUpdate))
                throw new Exception("Schema não contém 'properties'.");

            var fieldsSchema = GetFields(jsonSchema);
            var fieldsJsonUpdate = GetFields(jsonSchemaUpdate);

            var camposRemover = fieldsSchema
                                .Where(p1 => !fieldsJsonUpdate.Any(p2 => p2.field == p1.field))
                                .ToList();

            var camposAdicionar = fieldsJsonUpdate
                                .Where(p1 => !fieldsSchema.Any(p2 => p2.field == p1.field))
                                .ToList();


            var sbAdd = new System.Text.StringBuilder();
            sbAdd.AppendLine($"ALTER TABLE {tableName} ADD ");

            foreach (var field in camposAdicionar)
            {
                sbAdd.AppendLine($" {field.field} {field.type},");
            }

            //sbAdd.Length -= 3;
            //sbAdd.AppendLine("\n);");

            var sqlAdicionar = sbAdd.ToString();

            var sbRemove = new System.Text.StringBuilder();
            sbRemove.AppendLine($"ALTER TABLE {tableName} DROP COLUMN ");

            sbRemove.AppendLine($"{string.Join(", ", camposRemover.Select(x => x.field))}");

            //sbRemove.Length -= 3;
            //sbRemove.AppendLine("\n);");

            await RunSQLServerCommand(conectionString, sbAdd.ToString());
            await RunSQLServerCommand(conectionString, sbRemove.ToString());
        }



        //Entidades
        public static async Task<(bool success, string? error, Dictionary<string, object>? inserted)> InsertWithResult(string schemaName, [FromBody] Dictionary<string, object> data, Source source)
        {
            if (string.IsNullOrWhiteSpace(schemaName) || data == null || data.Count == 0)
                return (false, "Parâmetros inválidos.", null);

            var columns = string.Join(", ", data.Keys.Select(k => $"[{k}]"));
            var values = string.Join(", ", data.Keys.Select(k => $"@{k}"));
            var insertSql = $"INSERT INTO [{schemaName}] ({columns}) OUTPUT INSERTED.* VALUES ({values});";

            var connectionString = $"Server={source.Host};Database={source.DbName};User Id={source.User};Password={source.Password};";

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(insertSql, connection))
                    {
                        foreach (var kvp in data)
                        {
                            var paramName = $"@{kvp.Key}";
                            object value = kvp.Value ?? DBNull.Value;

                            if (value is string strValue && kvp.Key == "img_pessoa") // Você precisa implementar isso
                            {
                                value = Convert.FromBase64String(strValue);
                            }
                            //command.Parameters.AddWithValue($"@{kvp.Key}", kvp.Value ?? DBNull.Value);
                            command.Parameters.AddWithValue(paramName, value);
                        }

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var inserted = new Dictionary<string, object>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    inserted[reader.GetName(i)] = await reader.IsDBNullAsync(i) ? null! : reader.GetValue(i);
                                }
                                return (true, null, inserted);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }

            return (false, "Nenhum dado retornado.", null);
        }

        public static async Task<(bool success, string? error)> Insert(string schemaName, [FromBody] Dictionary<string, object> data, Source source)
        {
            if (string.IsNullOrWhiteSpace(schemaName) || data == null || data.Count == 0)
                return (false, "Parâmetros inválidos.");

            var columns = string.Join(", ", data.Keys.Select(k => $"[{k}]"));
            var values = string.Join(", ", data.Keys.Select(k => $"@{k}"));

            var insertSql = $"INSERT INTO [{schemaName}] ({columns}) VALUES ({values});";

            var connectionString = $"Server={source.Host};Database={source.DbName};User Id={source.User};Password={source.Password};";
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(insertSql, connection))
                    {
                        foreach (var kvp in data)
                        {
                            command.Parameters.AddWithValue($"@{kvp.Key}", kvp.Value ?? DBNull.Value);
                        }

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }


            return (true, null);
        }


        public static async Task<(bool success, string? error)> Update(string schemaName, [FromBody] Dictionary<string, object> data, Source source, string field, object value)
        {
            if (string.IsNullOrWhiteSpace(schemaName) || data == null || data.Count == 0)
                return (false, "Parâmetros inválidos.");


            if (data.Count == 0)
                return (false, "Nenhum dado para atualizar.");

            var setClause = string.Join(", ", data.Keys.Select(k => $"[{k}] = @{k}"));
            var updateSql = $"UPDATE [{schemaName}] SET {setClause} WHERE [{field}] = {value};";

            var connectionString = $"Server={source.Host};Database={source.DbName};User Id={source.User};Password={source.Password};";
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(updateSql, connection))
                    {
                        foreach (var kvp in data)
                        {
                            command.Parameters.AddWithValue($"@{kvp.Key}", kvp.Value ?? DBNull.Value);
                        }
                        await command.ExecuteReaderAsync();
                        return (true, null);
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public static async Task<(bool success, string? error, Dictionary<string, object>? updatedData)> UpdateWithResult(
    string schemaName,
    [FromBody] Dictionary<string, object> data,
    Source source,
    string field,
    object value)
        {
            if (string.IsNullOrWhiteSpace(schemaName) || data == null || data.Count == 0)
                return (false, "Parâmetros inválidos.", null);

            var setClause = string.Join(", ", data.Keys.Select(k => $"[{k}] = @{k}"));
            var updateSql = $"UPDATE [{schemaName}] SET {setClause} WHERE [{field}] = @value;";
            var selectSql = $"SELECT * FROM [{schemaName}] WHERE [{field}] = @value;";

            var connectionString = $"Server={source.Host};Database={source.DbName};User Id={source.User};Password={source.Password};";

            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                // UPDATE
                using (var updateCommand = new SqlCommand(updateSql, connection))
                {
                    foreach (var kvp in data)
                    {
                        updateCommand.Parameters.AddWithValue($"@{kvp.Key}", kvp.Value ?? DBNull.Value);
                    }

                    updateCommand.Parameters.AddWithValue("@value", value);

                    var affectedRows = await updateCommand.ExecuteNonQueryAsync();
                    if (affectedRows == 0)
                        return (false, "Nenhuma linha foi atualizada.", null);
                }

                // SELECT atualizada
                using (var selectCommand = new SqlCommand(selectSql, connection))
                {
                    selectCommand.Parameters.AddWithValue("@value", value);

                    using var reader = await selectCommand.ExecuteReaderAsync();
                    if (await reader.ReadAsync())
                    {
                        var updatedData = new Dictionary<string, object>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            updatedData[reader.GetName(i)] = reader.IsDBNull(i) ? null! : reader.GetValue(i);
                        }

                        return (true, null, updatedData);
                    }
                    else
                    {
                        return (false, "Objeto não encontrado após atualização.", null);
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public static async Task<(bool success, string? error, Dictionary<string, object>? deleted)> Delete(string schemaName, string id, Source source)
        {
            if (string.IsNullOrWhiteSpace(schemaName) || string.IsNullOrWhiteSpace(id))
                return (false, "Parâmetros inválidos.", null);

            var deleteSql = $"DELETE FROM [{schemaName}] OUTPUT DELETED.* WHERE [Id] = @Id;";

            var connectionString = $"Server={source.Host};Database={source.DbName};User Id={source.User};Password={source.Password};";

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(deleteSql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var deleted = new Dictionary<string, object>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    deleted[reader.GetName(i)] = await reader.IsDBNullAsync(i) ? null! : reader.GetValue(i);
                                }

                                return (true, null, deleted);
                            }
                            else
                            {
                                return (false, "Nenhum registro excluído. Verifique o valor de 'Id'.", null);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public static async Task<(bool success, string? error)> Delete(string schemaName, string field, string value, Source source)
        {
            if (string.IsNullOrWhiteSpace(schemaName) || string.IsNullOrWhiteSpace(value) || string.IsNullOrEmpty(field))
                return (false, "Parâmetros inválidos.");

            var deleteSql = $"DELETE FROM [{schemaName}] WHERE [{field}] = @value;";

            var connectionString = $"Server={source.Host};Database={source.DbName};User Id={source.User};Password={source.Password};";

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(deleteSql, connection))
                    {
                        command.Parameters.AddWithValue("@value", value);
                        await command.ExecuteReaderAsync();
                        return (true, null);
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public static async Task<(bool success, string? error)> DeleteByTwoFields(string schemaName, string field1, string value1, string field2, string value2, Source source)
        {
            if (string.IsNullOrWhiteSpace(schemaName) ||
                string.IsNullOrWhiteSpace(field1) || string.IsNullOrWhiteSpace(value1) ||
                string.IsNullOrWhiteSpace(field2) || string.IsNullOrWhiteSpace(value2))
            {
                return (false, "Invalid parameters.");
            }

            var deleteSql = $"DELETE FROM [{schemaName}] WHERE [{field1}] = @value1 AND [{field2}] = @value2;";

            var connectionString = $"Server={source.Host};Database={source.DbName};User Id={source.User};Password={source.Password};";

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(deleteSql, connection))
                    {
                        command.Parameters.AddWithValue("@value1", value1);
                        command.Parameters.AddWithValue("@value2", value2);

                        // Use ExecuteNonQueryAsync for DELETE operations instead of ExecuteReaderAsync
                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        // Optional: You might want to check if any rows were actually deleted
                        if (rowsAffected == 0)
                        {
                            return (true, "No records found matching the criteria.");
                        }
                        return (true, null);
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public static async Task<(bool success, Dictionary<string, object>? data, string? error)> GetById(string schemaName, string pk, string id, Source source)
        {
            if (string.IsNullOrWhiteSpace(schemaName) || string.IsNullOrWhiteSpace(id))
                return (false, null, "Parâmetros inválidos.");

            var selectSql = $"SELECT * FROM [{schemaName}] WHERE [{pk}] = @Id;";

            var connectionString = $"Server={source.Host};Database={source.DbName};User Id={source.User};Password={source.Password};";

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(selectSql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var result = new Dictionary<string, object>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    var fieldName = reader.GetName(i);
                                    var fieldValue = await reader.IsDBNullAsync(i) ? null : reader.GetValue(i);
                                    result[fieldName] = fieldValue!;
                                }

                                return (true, result, null);
                            }
                            else
                            {
                                return (false, null, "Registro não encontrado.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, null, ex.Message);
            }
        }

        public static async Task<Dictionary<string, object>?> GetFirstByFields(Source source, string nomeTabela, List<(string campo, object valor)> filtros)
        {
            var connectionString = $"Server={source.Host};Database={source.DbName};User Id={source.User};Password={source.Password};";

            var whereClauses = new List<string>();
            var parametros = new List<SqlParameter>();

            for (int i = 0; i < filtros.Count; i++)
            {
                var paramName = $"@param{i}";
                whereClauses.Add($"[{filtros[i].campo}] = {paramName}");
                parametros.Add(new SqlParameter(paramName, filtros[i].valor ?? DBNull.Value));
            }

            var whereClause = whereClauses.Count > 0 ? "WHERE " + string.Join(" AND ", whereClauses) : "";

            var query = $"SELECT TOP 1 * FROM [{nomeTabela}] {whereClause}";

            await using var conexao = new SqlConnection(connectionString);
            await using var comando = new SqlCommand(query, conexao);

            foreach (var param in parametros)
            {
                comando.Parameters.Add(param);
            }

            await conexao.OpenAsync();
            await using var reader = await comando.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            var resultado = new Dictionary<string, object>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                resultado[reader.GetName(i)] = await reader.IsDBNullAsync(i) ? null! : reader.GetValue(i);
            }

            return resultado;
        }

        public static async Task<(bool success, List<Dictionary<string, object>>? data, int total, string? error)> GetListEntity(string schemaName, int? page, int? limit, string sortField = null, bool sortDesc = false, string ids = null, string searchFields = null, string value = null, Source source = null, SearchModeEnum mode = SearchModeEnum.Equals, string? cd_empresa_field = null, string? cd_empresa_value = null, Infra.Domain.Models.SchemaModel? schemaModel = null)
        
        {
            //trocar por string builder
            if (string.IsNullOrWhiteSpace(schemaName))
                return (false, null, 0, "Parâmetros inválidos.");

            var topClause = "";
            if (page == null) page = 1;
            if (limit == null) limit = 10;

            var offset = ((page ?? 1) - 1) * (limit ?? 10);
            topClause = $"OFFSET {offset} ROWS FETCH NEXT {limit ?? 10} ROWS ONLY";

            var whereConditions = new List<string>();
            
            if (!string.IsNullOrWhiteSpace(ids))
            {
                var idList = ids.Split(',').Select(id => $"'{id.Trim()}'");
                whereConditions.Add($"[{searchFields}] IN ({string.Join(",", idList)})");
            }

            if (!string.IsNullOrWhiteSpace(searchFields) && !string.IsNullOrWhiteSpace(value))
            {
                // Extrai cada "[item1,item2,...]" em listas separadas
                var fieldGroups = Regex.Matches(searchFields, @"\[(.*?)\]")
                                        .Cast<Match>()
                                        .Select(m => m.Groups[1].Value.Split(',')
                                                                          .Select(f => f.Trim())
                                                                          .ToList())
                                        .ToList();

                var valueGroups = Regex.Matches(value, @"\[(.*?)\]")
                                        .Cast<Match>()
                                        .Select(m => m.Groups[1].Value.Split(',')
                                                                          .Select(v => v.Trim())
                                                                          .ToList())
                                        .ToList();

                // Para cada grupo (até o menor número de grupos entre fields e values)
                var groupCount = Math.Min(fieldGroups.Count, valueGroups.Count);
                for (int i = 0; i < groupCount; i++)
                {
                    var fields = fieldGroups[i];
                    var vals = valueGroups[i];
                    var innerConds = new List<string>();

                    // Cross‑product: para cada campo e cada valor
                    foreach (var f in fields)
                    {
                        // Se o campo começa com dt_, tratar o valor completo como um range de data
                        if (f.StartsWith("dt_", StringComparison.OrdinalIgnoreCase))
                        {
                            // Para campos de data, usar o valor original completo (pode conter vírgula para range)
                            var originalValue = valueGroups[i].Count > 0 ? string.Join(",", valueGroups[i]) : "";
                            string cond = GenerateSearchCondition(f, originalValue, mode);
                            innerConds.Add(cond);
                        }
                        else
                        {
                            foreach (var v in vals)
                            {
                                string cond = GenerateSearchCondition(f, v, mode);
                                innerConds.Add(cond);
                            }
                        }
                    }

                    if (innerConds.Any())
                    {
                        // Agrupa com OR e envolve em parênteses
                        whereConditions.Add($"({string.Join(" OR ", innerConds)})");
                    }
                }
            }
            if (cd_empresa_field != null && cd_empresa_value != null) whereConditions.Add($"[{cd_empresa_field}] = '{cd_empresa_value.Replace("'", "''")}'");
            var whereClause = whereConditions.Count > 0 ? $"WHERE {string.Join(" AND ", whereConditions)}" : "";


            var orderClause = !string.IsNullOrWhiteSpace(sortField)
                ? $"ORDER BY [{sortField}] {(sortDesc ? "DESC" : "ASC")}"
                : "";

            var selectSql = $"SELECT * FROM [{schemaName}] {whereClause} {orderClause} {topClause};";

            var nomeTabela = "table_name";
            var joinsSql = "";
            if (schemaModel != null)
            {               

                var nomesCampos = schemaModel.Properties.Keys.Select(k => $"{k}").ToList();
                var campos = $"{nomeTabela}.*";

                orderClause = !string.IsNullOrWhiteSpace(sortField)
                ? $"ORDER BY {nomeTabela}.[{sortField}] {(sortDesc ? "DESC" : "ASC")}"
                : "";

                
                if (!schemaModel.InnerJoin.IsNullOrEmpty())
                {
                    foreach (var inner in schemaModel.InnerJoin)
                    {

                        var tableJoin = string.IsNullOrEmpty(inner.joinTable) ? nomeTabela : inner.joinTable;

                        joinsSql += $" INNER JOIN [{inner.table}] AS {inner.@as} ON {inner.@as}.[{inner.fk}] = {tableJoin}.[{inner.localField}]";


                        foreach (var campo in inner.fields)
                        {
                            if(!nomesCampos.Contains(campo))
                            {
                                if (campos.IsNullOrEmpty()) campos = $"{inner.@as}.{campo}";
                                else campos = $"{campos},{inner.@as}.{campo}";
                                nomesCampos.Add(campo);
                            }
                        }
                    }
                }

                if (!schemaModel.Where.IsNullOrEmpty())
                {
                    foreach (var w in schemaModel.Where)
                    {
                        whereConditions.Add($"{w.field} = '{w.value.ToString().Replace("'", "''")}'");
                    }
                    whereClause = whereConditions.Count > 0 ? $"WHERE {string.Join(" AND ", whereConditions)}" : "";
                }

                selectSql = $"SELECT {campos} FROM [{schemaName}] as {nomeTabela} {joinsSql} {whereClause} {orderClause} {topClause};";

            }
            var countSql = $"SELECT COUNT(*) FROM [{schemaName}] as {nomeTabela} {joinsSql} {whereClause};";

            var connectionString = $"Server={source.Host};Database={source.DbName};User Id={source.User};Password={source.Password};";

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    int total;
                    using (var countCmd = new SqlCommand(countSql, connection))
                    {
                        total = (int)await countCmd.ExecuteScalarAsync();
                    }

                    var results = new List<Dictionary<string, object>>();
                    using (var selectCmd = new SqlCommand(selectSql, connection))
                    using (var reader = await selectCmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                var fieldName = reader.GetName(i);
                                var fieldValue = await reader.IsDBNullAsync(i) ? null : reader.GetValue(i);
                                row[fieldName] = fieldValue!;
                            }
                            results.Add(row);
                        }
                    }

                    return (true, results, total, null);
                }
            }
            catch (Exception ex)
            {
                return (false, null, 0, $"{ex.Message}______{selectSql}");
            }
        }

        public static async Task<(bool success, List<Dictionary<string, object>>? data, int total, string? error)> GetList(string schemaName, int? page, int? limit, string sortField = null, bool sortDesc = false, string ids = null, string searchFields = null, string value = null, Source source = null, SearchModeEnum mode = SearchModeEnum.Equals, string? cd_empresa_field = null, string? cd_empresa_value = null)
        {
            if (string.IsNullOrWhiteSpace(schemaName))
                return (false, null, 0, "Parâmetros inválidos.");

            var topClause = "";
            if (page == null) page = 1;
            if (limit == null) limit = 10;

            var offset = ((page ?? 1) - 1) * (limit ?? 10);
            topClause = $"OFFSET {offset} ROWS FETCH NEXT {limit ?? 10} ROWS ONLY";

            var whereConditions = new List<string>();

            if (!string.IsNullOrWhiteSpace(ids))
            {
                var idList = ids.Split(',').Select(id => $"'{id.Trim()}'");
                whereConditions.Add($"[{searchFields}] IN ({string.Join(",", idList)})");
            }

            if (!string.IsNullOrWhiteSpace(searchFields) && !string.IsNullOrWhiteSpace(value))
            {
                // Extrai cada "[item1,item2,...]" em listas separadas
                var fieldGroups = Regex.Matches(searchFields, @"\[(.*?)\]")
                                        .Cast<Match>()
                                        .Select(m => m.Groups[1].Value.Split(',')
                                                                          .Select(f => f.Trim())
                                                                          .ToList())
                                        .ToList();

                var valueGroups = Regex.Matches(value, @"\[(.*?)\]")
                                        .Cast<Match>()
                                        .Select(m => m.Groups[1].Value.Split(',')
                                                                          .Select(v => v.Trim())
                                                                          .ToList())
                                        .ToList();

                // Para cada grupo (até o menor número de grupos entre fields e values)
                var groupCount = Math.Min(fieldGroups.Count, valueGroups.Count);
                for (int i = 0; i < groupCount; i++)
                {
                    var fields = fieldGroups[i];
                    var vals = valueGroups[i];
                    var innerConds = new List<string>();

                    // Cross‑product: para cada campo e cada valor
                    foreach (var f in fields)
                    {
                        // Se o campo começa com dt_, tratar o valor completo como um range de data
                        if (f.StartsWith("dt_", StringComparison.OrdinalIgnoreCase))
                        {
                            // Para campos de data, usar o valor original completo (pode conter vírgula para range)
                            var originalValue = valueGroups[i].Count > 0 ? string.Join(",", valueGroups[i]) : "";
                            string cond = GenerateSearchCondition(f, originalValue, mode);
                            innerConds.Add(cond);
                        }
                        else
                        {
                            foreach (var v in vals)
                            {
                                string cond = GenerateSearchCondition(f, v, mode);
                                innerConds.Add(cond);
                            }
                        }
                    }

                    if (innerConds.Any())
                    {
                        // Agrupa com OR e envolve em parênteses
                        whereConditions.Add($"({string.Join(" OR ", innerConds)})");
                    }
                }
            }
            if (cd_empresa_field != null && cd_empresa_value != null) whereConditions.Add($"[{cd_empresa_field}] = '{cd_empresa_value.Replace("'", "''")}'");
            var whereClause = whereConditions.Count > 0 ? $"WHERE {string.Join(" AND ", whereConditions)}" : "";


            var orderClause = !string.IsNullOrWhiteSpace(sortField)
                ? $"ORDER BY [{sortField}] {(sortDesc ? "DESC" : "ASC")}"
                : "ORDER BY [Id] ASC"; // Default ordering

            var selectSql = $"SELECT * FROM [{schemaName}] {whereClause} {orderClause} {topClause};";
            var countSql = $"SELECT COUNT(*) FROM [{schemaName}] {whereClause};";

            var connectionString = $"Server={source.Host};Database={source.DbName};User Id={source.User};Password={source.Password};";

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    int total;
                    using (var countCmd = new SqlCommand(countSql, connection))
                    {
                        total = (int)await countCmd.ExecuteScalarAsync();
                    }

                    var results = new List<Dictionary<string, object>>();
                    using (var selectCmd = new SqlCommand(selectSql, connection))
                    using (var reader = await selectCmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                var fieldName = reader.GetName(i);
                                var fieldValue = await reader.IsDBNullAsync(i) ? null : reader.GetValue(i);
                                row[fieldName] = fieldValue!;
                            }
                            results.Add(row);
                        }
                    }

                    return (true, results, total, null);
                }
            }
            catch (Exception ex)
            {
                return (false, null, 0, ex.Message);
            }
        }

        public static async Task<(bool success, List<Dictionary<string, object>>? data, int total, string? error)> GetList(
     string schemaName,
     string ids = null,
     string searchFields = null,
     string value = null,
     Source source = null,
     SearchModeEnum mode = SearchModeEnum.Equals)
        {
            if (string.IsNullOrWhiteSpace(schemaName))
                return (false, null, 0, "Parâmetros inválidos.");

            var whereConditions = new List<string>();

            if (!string.IsNullOrWhiteSpace(ids) && !string.IsNullOrWhiteSpace(searchFields))
            {
                var idList = ids.Split(',').Select(id => $"'{id.Trim()}'");
                whereConditions.Add($"[{searchFields}] IN ({string.Join(",", idList)})");
            }

            if (!string.IsNullOrWhiteSpace(searchFields) && !string.IsNullOrWhiteSpace(value))
            {
                // Extrai cada "[item1,item2,...]" em listas separadas
                var fieldGroups = Regex.Matches(searchFields, @"\[(.*?)\]")
                                        .Cast<Match>()
                                        .Select(m => m.Groups[1].Value.Split(',')
                                                                          .Select(f => f.Trim())
                                                                          .ToList())
                                        .ToList();

                var valueGroups = Regex.Matches(value, @"\[(.*?)\]")
                                        .Cast<Match>()
                                        .Select(m => m.Groups[1].Value.Split(',')
                                                                          .Select(v => v.Trim())
                                                                          .ToList())
                                        .ToList();

                // Para cada grupo (até o menor número de grupos entre fields e values)
                var groupCount = Math.Min(fieldGroups.Count, valueGroups.Count);
                for (int i = 0; i < groupCount; i++)
                {
                    var fields = fieldGroups[i];
                    var vals = valueGroups[i];
                    var innerConds = new List<string>();

                    // Cross‑product: para cada campo e cada valor
                    foreach (var f in fields)
                    {
                        // Se o campo começa com dt_, tratar o valor completo como um range de data
                        if (f.StartsWith("dt_", StringComparison.OrdinalIgnoreCase))
                        {
                            // Para campos de data, usar o valor original completo (pode conter vírgula para range)
                            var originalValue = valueGroups[i].Count > 0 ? string.Join(",", valueGroups[i]) : "";
                            string cond = GenerateSearchCondition(f, originalValue, mode);
                            innerConds.Add(cond);
                        }
                        else
                        {
                            foreach (var v in vals)
                            {
                                string cond = GenerateSearchCondition(f, v, mode);
                                innerConds.Add(cond);
                            }
                        }
                    }

                    if (innerConds.Any())
                    {
                        // Agrupa com OR e envolve em parênteses
                        whereConditions.Add($"({string.Join(" OR ", innerConds)})");
                    }
                }
            }

            var whereClause = whereConditions.Count > 0 ? $"WHERE {string.Join(" AND ", whereConditions)}" : "";



            var selectSql = $"SELECT * FROM [{schemaName}] {whereClause} ;";
            var countSql = $"SELECT COUNT(*) FROM [{schemaName}] {whereClause};";

            var connectionString = $"Server={source.Host};Database={source.DbName};User Id={source.User};Password={source.Password};";

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    int total;
                    using (var countCmd = new SqlCommand(countSql, connection))
                    {
                        total = (int)await countCmd.ExecuteScalarAsync();
                    }

                    var results = new List<Dictionary<string, object>>();
                    using (var selectCmd = new SqlCommand(selectSql, connection))
                    using (var reader = await selectCmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                var fieldName = reader.GetName(i);
                                var fieldValue = await reader.IsDBNullAsync(i) ? null : reader.GetValue(i);
                                row[fieldName] = fieldValue!;
                            }
                            results.Add(row);
                        }
                    }

                    return (true, results, total, null);
                }
            }
            catch (Exception ex)
            {
                return (false, null, 0, ex.Message);
            }
        }

        public static async Task<(bool success, List<Dictionary<string, object>>? data, int total, string? error)> GetListIn(string schemaName, int? page, int? limit, string sortField = null, bool sortDesc = false, string ids = null, string searchFields = null, string value = null, Source source = null, SearchModeEnum mode = SearchModeEnum.Equals, string? cd_empresa_field = null, string? cd_empresa_value = null, string fieldIn = null, List<string> valuesIn = null)
        {
            if (string.IsNullOrWhiteSpace(schemaName))
                return (false, null, 0, "Parâmetros inválidos.");

            var topClause = "";
            if (page == null) page = 1;
            if (limit == null) limit = 10;

            var offset = ((page ?? 1) - 1) * (limit ?? 10);
            topClause = $"OFFSET {offset} ROWS FETCH NEXT {limit ?? 10} ROWS ONLY";

            var whereConditions = new List<string>();

            if (!string.IsNullOrWhiteSpace(ids))
            {
                var idList = ids.Split(',').Select(id => $"'{id.Trim()}'");
                whereConditions.Add($"[{searchFields}] IN ({string.Join(",", idList)})");
            }

            if (!string.IsNullOrWhiteSpace(fieldIn) && !valuesIn.IsNullOrEmpty())
            {
                var valuesInList = valuesIn.Select(v => $"'{v.Trim()}'");
                whereConditions.Add($"[{fieldIn}] IN ({string.Join(",", valuesInList)})");
            }

            if (!string.IsNullOrWhiteSpace(searchFields) && !string.IsNullOrWhiteSpace(value))
            {
                // Extrai cada "[item1,item2,...]" em listas separadas
                var fieldGroups = Regex.Matches(searchFields, @"\[(.*?)\]")
                                        .Cast<Match>()
                                        .Select(m => m.Groups[1].Value.Split(',')
                                                                          .Select(f => f.Trim())
                                                                          .ToList())
                                        .ToList();

                var valueGroups = Regex.Matches(value, @"\[(.*?)\]")
                                        .Cast<Match>()
                                        .Select(m => m.Groups[1].Value.Split(',')
                                                                          .Select(v => v.Trim())
                                                                          .ToList())
                                        .ToList();

                // Para cada grupo (até o menor número de grupos entre fields e values)
                var groupCount = Math.Min(fieldGroups.Count, valueGroups.Count);
                for (int i = 0; i < groupCount; i++)
                {
                    var fields = fieldGroups[i];
                    var vals = valueGroups[i];
                    var innerConds = new List<string>();

                    // Cross‑product: para cada campo e cada valor
                    foreach (var f in fields)
                    {
                        // Se o campo começa com dt_, tratar o valor completo como um range de data
                        if (f.StartsWith("dt_", StringComparison.OrdinalIgnoreCase))
                        {
                            // Para campos de data, usar o valor original completo (pode conter vírgula para range)
                            var originalValue = valueGroups[i].Count > 0 ? string.Join(",", valueGroups[i]) : "";
                            string cond = GenerateSearchCondition(f, originalValue, mode);
                            innerConds.Add(cond);
                        }
                        else
                        {
                            foreach (var v in vals)
                            {
                                string cond = GenerateSearchCondition(f, v, mode);
                                innerConds.Add(cond);
                            }
                        }
                    }

                    if (innerConds.Any())
                    {
                        // Agrupa com OR e envolve em parênteses
                        whereConditions.Add($"({string.Join(" OR ", innerConds)})");
                    }
                }
            }
            if (cd_empresa_field != null && cd_empresa_value != null) whereConditions.Add($"[{cd_empresa_field}] = '{cd_empresa_value.Replace("'", "''")}'");
            var whereClause = whereConditions.Count > 0 ? $"WHERE {string.Join(" AND ", whereConditions)}" : "";


            var orderClause = !string.IsNullOrWhiteSpace(sortField)
                ? $"ORDER BY [{sortField}] {(sortDesc ? "DESC" : "ASC")}"
                : "ORDER BY [Id] ASC"; // Default ordering

            var selectSql = $"SELECT * FROM [{schemaName}] {whereClause} {orderClause} {topClause};";
            var countSql = $"SELECT COUNT(*) FROM [{schemaName}] {whereClause};";

            var connectionString = $"Server={source.Host};Database={source.DbName};User Id={source.User};Password={source.Password};";

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    int total;
                    using (var countCmd = new SqlCommand(countSql, connection))
                    {
                        total = (int)await countCmd.ExecuteScalarAsync();
                    }

                    var results = new List<Dictionary<string, object>>();
                    using (var selectCmd = new SqlCommand(selectSql, connection))
                    using (var reader = await selectCmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                var fieldName = reader.GetName(i);
                                var fieldValue = await reader.IsDBNullAsync(i) ? null : reader.GetValue(i);
                                row[fieldName] = fieldValue!;
                            }
                            results.Add(row);
                        }
                    }

                    return (true, results, total, null);
                }
            }
            catch (Exception ex)
            {
                return (false, null, 0, ex.Message);
            }
        }


        public static async Task<(bool success, List<Dictionary<string, object>>? data, int total, string? error)> GetListFiltroData(
    string schemaName,
    int? page,
    int? limit,
    string sortField = null,
    bool sortDesc = false,
    string ids = null,
    string idField = null,
    string searchFields = null,
    string value = null,
    Source source = null,
    SearchModeEnum mode = SearchModeEnum.Equals,
    string? cd_empresa_field = null,
    string? cd_empresa_value = null,
    string? dateFieldStart = null,
    string? dateFieldEnd = null,
    DateTime? dateStart = null,
    DateTime? dateEnd = null,
    string? dateField2 = null,
    DateTime? dateStart2 = null,
    DateTime? dateEnd2 = null,
    string? campoDiaAtual = null
)
        {
            if (string.IsNullOrWhiteSpace(schemaName))
                return (false, null, 0, "Parâmetros inválidos.");

            if (page == null) page = 1;
            if (limit == null) limit = 10;
            var offset = ((page ?? 1) - 1) * (limit ?? 10);
            var topClause = $"OFFSET {offset} ROWS FETCH NEXT {limit ?? 10} ROWS ONLY";

            var whereConditions = new List<string>();

            if (!string.IsNullOrWhiteSpace(ids))
            {
                var idList = ids.Split(',').Select(id => $"'{id.Trim()}'");
                whereConditions.Add($"[{idField}] IN ({string.Join(",", idList)})");
            }

            if (!string.IsNullOrWhiteSpace(searchFields) && !string.IsNullOrWhiteSpace(value))
            {
                // Extrai cada "[item1,item2,...]" em listas separadas
                var fieldGroups = Regex.Matches(searchFields, @"\[(.*?)\]")
                                        .Cast<Match>()
                                        .Select(m => m.Groups[1].Value.Split(',')
                                                                          .Select(f => f.Trim())
                                                                          .ToList())
                                        .ToList();

                var valueGroups = Regex.Matches(value, @"\[(.*?)\]")
                                        .Cast<Match>()
                                        .Select(m => m.Groups[1].Value.Split(',')
                                                                          .Select(v => v.Trim())
                                                                          .ToList())
                                        .ToList();

                // Para cada grupo (até o menor número de grupos entre fields e values)
                var groupCount = Math.Min(fieldGroups.Count, valueGroups.Count);
                for (int i = 0; i < groupCount; i++)
                {
                    var fields = fieldGroups[i];
                    var vals = valueGroups[i];
                    var innerConds = new List<string>();

                    // Cross‑product: para cada campo e cada valor
                    foreach (var f in fields)
                    {
                        // Se o campo começa com dt_, tratar o valor completo como um range de data
                        if (f.StartsWith("dt_", StringComparison.OrdinalIgnoreCase))
                        {
                            // Para campos de data, usar o valor original completo (pode conter vírgula para range)
                            var originalValue = valueGroups[i].Count > 0 ? string.Join(",", valueGroups[i]) : "";
                            string cond = GenerateSearchCondition(f, originalValue, mode);
                            innerConds.Add(cond);
                        }
                        else
                        {
                            foreach (var v in vals)
                            {
                                string cond = GenerateSearchCondition(f, v, mode);
                                innerConds.Add(cond);
                            }
                        }
                    }

                    if (innerConds.Any())
                    {
                        // Agrupa com OR e envolve em parênteses
                        whereConditions.Add($"({string.Join(" OR ", innerConds)})");
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(cd_empresa_field) && !string.IsNullOrWhiteSpace(cd_empresa_value))
                whereConditions.Add($"[{cd_empresa_field}] = '{cd_empresa_value.Replace("'", "''")}'");

            if (!string.IsNullOrWhiteSpace(dateFieldStart))
            {
                if (dateStart.HasValue) whereConditions.Add($"CAST([{dateFieldStart}] AS DATE) >= '{dateStart.Value:yyyy-MM-dd}'");
                if (dateEnd.HasValue) whereConditions.Add($"CAST([{dateFieldEnd}] AS DATE) <= '{dateEnd.Value:yyyy-MM-dd}'");

                if (schemaName == "vi_pipeline" && (dateStart.HasValue || dateEnd.HasValue))
                {
                    whereConditions.Add("[possui_atendente] = 0");
                }
            }

            if (!string.IsNullOrWhiteSpace(dateField2))
            {
                if (dateStart2.HasValue) whereConditions.Add($"CAST([{dateField2}] AS DATE) >= '{dateStart2.Value:yyyy-MM-dd}'");

                if (dateEnd2.HasValue) whereConditions.Add($"CAST([{dateField2}] AS DATE) <= '{dateEnd2.Value:yyyy-MM-dd}'");

                if (schemaName == "vi_pipeline" && (dateStart2.HasValue || dateEnd2.HasValue))
                {
                    whereConditions.Add("[dt_reprogramar] IS NOT NULL");
                }



            }
            if (!string.IsNullOrWhiteSpace(campoDiaAtual))
            {
                var dataAtual = DateTime.Now.Date;
                whereConditions.Add($"CAST([{campoDiaAtual}] AS DATE) >= '{dataAtual:yyyy-MM-dd}'");
                whereConditions.Add($"CAST([{campoDiaAtual}] AS DATE) <= '{dataAtual:yyyy-MM-dd}'");
            }

            var whereClause = whereConditions.Count > 0
                ? $"WHERE {string.Join(" AND ", whereConditions)}"
                : "";

            var orderClause = !string.IsNullOrWhiteSpace(sortField)
                ? $"ORDER BY [{sortField}] {(sortDesc ? "DESC" : "ASC")}"
                : "ORDER BY [Id] ASC";

            var selectSql = $"SELECT * FROM [{schemaName}] {whereClause} {orderClause} {topClause};";
            var countSql = $"SELECT COUNT(*) FROM [{schemaName}] {whereClause};";

            var connectionString = $"Server={source.Host};Database={source.DbName};User Id={source.User};Password={source.Password};";

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    int total;
                    using (var countCmd = new SqlCommand(countSql, connection))
                    {
                        total = (int)await countCmd.ExecuteScalarAsync();
                    }

                    var results = new List<Dictionary<string, object>>();
                    using (var selectCmd = new SqlCommand(selectSql, connection))
                    using (var reader = await selectCmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                var fieldName = reader.GetName(i);
                                var fieldValue = await reader.IsDBNullAsync(i) ? null : reader.GetValue(i);
                                row[fieldName] = fieldValue!;
                            }
                            results.Add(row);
                        }
                    }

                    return (true, results, total, null);
                }
            }
            catch (Exception ex)
            {
                return (false, null, 0, ex.Message);
            }
        }

        //Auxiliares
        static List<(string field, string type)> GetFields(Dictionary<string, object> json)
        {
            if (!json.TryGetValue("properties", out var propertiesObj))
                throw new Exception("Schema não contém 'properties'.");

            if (propertiesObj is not JObject properties)
                throw new Exception("'properties' não é um JObject.");
            List<(string field, string type)> fieldsReturn = new();

            foreach (var prop in properties)
            {
                var columnName = prop.Key;
                var fieldData = prop.Value;
                string type = fieldData?["type"]?.ToString() ?? "undefined";
                var sqlType = MapJsonTypeToSqlType(type);
                fieldsReturn.Add((columnName, sqlType));
            }
            return fieldsReturn;
        }
        static string MapJsonTypeToSqlType(string jsonType)
        {
            return jsonType switch
            {
                "string" => "VARCHAR(255)",
                "integer" => "INT",
                "number" => "DECIMAL(18, 2)",
                "boolean" => "BIT",
                _ => "TEXT"
            };
        }

        /// <summary>
        /// Gera condição de busca considerando que campos que iniciam com "cd_" sempre usam igualdade
        /// e campos que iniciam com "dt_" fazem busca por intervalo de datas
        /// </summary>
        private static string GenerateSearchCondition(string field, string value, SearchModeEnum mode)
        {
            // Se o campo inicia com "cd_", sempre usar igualdade, independente do mode
            if (field.StartsWith("cd_", StringComparison.OrdinalIgnoreCase))
            {
                return $"[{field}] = '{value.Replace("'", "''")}'";
            }

            // Se o campo inicia com "dt_", fazer busca por data/intervalo de datas
            if (field.StartsWith("dt_", StringComparison.OrdinalIgnoreCase))
            {
                var dates = value.Split(',');
                if (dates.Length == 2)
                {
                    // Busca entre duas datas
                    var dateStart = dates[0].Trim();
                    var dateEnd = dates[1].Trim();
                    return $"(CAST([{field}] AS DATE) >= '{dateStart}' AND CAST([{field}] AS DATE) <= '{dateEnd}')";
                }
                else
                {
                    // Busca por data específica
                    return $"CAST([{field}] AS DATE) = '{value.Trim()}'";
                }
            }

            return mode switch
            {
                SearchModeEnum.Contains => $"[{field}] LIKE '%{value.Replace("'", "''")}%'",
                SearchModeEnum.Equals => $"[{field}] = '{value.Replace("'", "''")}'",
                _ => throw new NotSupportedException($"Modo {mode} não suportado")
            };
        }



    }
}

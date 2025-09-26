using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Simjob.Framework.Application.Controllers;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Core.Utils;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using Simjob.Framework.Infra.Schemas.Entities;
using Simjob.Framework.Infra.Schemas.Interfaces;
using Simjob.Framework.Services.Api.Services;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Simjob.Framework.Services.Api.Controllers
{
    public class CompanySiteController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IEntityService _entityService;
        private readonly IRepository<SourceContext, Source> _sourceRepository;
        private readonly IRepository<MongoDbContext, Schema> _schemaRepository;

        public CompanySiteController(IMediatorHandler bus, INotificationHandler<DomainNotification> notifications, IUserService userService, IEntityService entityService, IRepository<SourceContext, Source> sourceRepository, IRepository<MongoDbContext, Schema> schemaRepository) : base(bus, notifications)
        {
            _userService = userService;
            _entityService = entityService;
            _sourceRepository = sourceRepository;
            _schemaRepository = schemaRepository;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetInfosToken()
        {
            string userId = "";
            var campo = "userId";
            var schemaName = "AsapTask";
            try
            {
                var accessToken = Request.Headers[HeaderNames.Authorization];

                var tokenInfo = Util.GetUserInfoFromToken(accessToken);

                if (tokenInfo.Count > 0)
                {
                    userId = tokenInfo["userid"];
                }
            }
            catch (Exception)
            {
                throw;
            }

            var user = _userService.GetUserById(userId);

            if (user == null) return BadRequest("usuario não encontrado");

            var companySiteIds = user.CompanySiteIds;

            if (companySiteIds.IsNullOrEmpty()) return NotFound("nenhum companyId encontrado");

            dynamic companies = await _entityService.GetAll("CompanySite", null, null, ids: string.Join(",", companySiteIds));

            return ResponseDefault(companies.Data);
        }


        [Authorize]
        [HttpGet("endereco")]
        public async Task<IActionResult> GetEnderecoEmpresa([FromQuery] int cd_empresa)
        {
            if (cd_empresa <= 0)
            {
                return BadRequest("Parâmetro cd_empresa é obrigatório e deve ser maior que zero");
            }

            try
            {
                // Obter schema e source usando T_Empresa
                var schemaName = "T_Empresa";
                if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
                var schema = _schemaRepository.GetSchemaByField("name", schemaName);
                var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
                var source = _sourceRepository.GetByField("description", schemaModel.Source);

                if (source == null || source.Active != true)
                {
                    return BadRequest("Fonte de dados não configurada ou inativa.");
                }

                // Executar a query personalizada
                string connectionString = $"Server={source.Host};Database={source.DbName};User Id={source.User};Password={source.Password};MultipleActiveResultSets=True;";
                
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    
                    var command = new SqlCommand(@"
                        SELECT 
                            e.cd_pessoa_empresa,
                            p.no_pessoa,
                            p.dc_reduzido_pessoa,
	                        pj.dc_num_cgc,
                            l5.no_localidade as pais,
                            l4.no_localidade as estado,
                            l3.no_localidade as cidade,
                            l2.no_localidade as bairro,
                            l1.no_localidade as logradouro,
                            en.dc_num_endereco as numero,
                            en.dc_compl_endereco as complemento,
                            en.dc_num_cep
                        FROM T_EMPRESA e 
                        INNER JOIN T_PESSOA p ON e.cd_pessoa_empresa = p.cd_pessoa 
                        INNER JOIN T_PESSOA_JURIDICA pj ON pj.cd_pessoa_juridica = p.cd_pessoa 
                        INNER JOIN T_ENDERECO en ON p.cd_endereco_principal = en.cd_endereco
                        INNER JOIN T_LOCALIDADE l1 ON en.cd_loc_logradouro = l1.cd_localidade
                        INNER JOIN T_LOCALIDADE l2 ON en.cd_loc_bairro = l2.cd_localidade
                        INNER JOIN T_LOCALIDADE l3 ON en.cd_loc_cidade = l3.cd_localidade
                        INNER JOIN T_LOCALIDADE l4 ON en.cd_loc_estado = l4.cd_localidade
                        INNER JOIN T_LOCALIDADE l5 ON en.cd_loc_pais = l5.cd_localidade
                        WHERE e.cd_pessoa_empresa = @cd_empresa", connection);
                    
                    command.Parameters.AddWithValue("@cd_empresa", cd_empresa);
                    
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var endereco = new
                            {
                                cd_pessoa_empresa = Convert.ToInt32(reader["cd_pessoa_empresa"]),
                                no_pessoa = reader["no_pessoa"]?.ToString(),
                                dc_reduzido_pessoa = reader["dc_reduzido_pessoa"]?.ToString(),
                                pais = reader["pais"]?.ToString(),
                                estado = reader["estado"]?.ToString(),
                                cidade = reader["cidade"]?.ToString(),
                                bairro = reader["bairro"]?.ToString(),
                                logradouro = reader["logradouro"]?.ToString(),
                                numero = reader["numero"]?.ToString(),
                                complemento = reader["complemento"]?.ToString(),
                                dc_num_cep = reader["dc_num_cep"]?.ToString()
                            };
                            
                            return ResponseDefault(endereco);
                        }
                        else
                        {
                            return NotFound("Empresa não encontrada ou sem endereço cadastrado");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro interno: {ex.Message}");
            }
        }
    }
}
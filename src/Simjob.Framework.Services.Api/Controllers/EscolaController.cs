using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Simjob.Framework.Application.Controllers;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Services.Api.Models;
using Simjob.Framework.Services.Api.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Simjob.Framework.Domain.Core.Utils;
using Simjob.Framework.Infra.Identity.Commands.AccessGroup;
using Simjob.Framework.Infra.Schemas.Commands.Entities;
using static Simjob.Framework.Infra.Identity.Entities.Notification;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
using System.Threading;
using Simjob.Framework.Infra.Schemas.Interfaces;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Schemas.Entities;
using Microsoft.Extensions.Configuration;
using Simjob.Framework.Infra.Identity.Interfaces;
using ApiEntities = Simjob.Framework.Services.Api.Entities;
using Newtonsoft.Json.Linq;
using System.Reflection;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Services.Api.Enums;



namespace Simjob.Framework.Services.Api.Controllers
{
  public class EscolaController : BaseController
  {
    private readonly IRepository<SourceContext, Source> _sourceRepository;
    private readonly ISchemaBuilder _schemaBuilder;
    private readonly IRepository<MongoDbContext, Schema> _schemaRepository;
    private readonly IRepository<MongoDbContext, ApiEntities.Action> _actionRepository;
    private readonly IUserService _userService;
    private readonly IEntityService _entityService;
    private readonly IGeneratorsService _generatorsService;
    private readonly IUserAccessService _userAccessService;
    private readonly IStatusFlowService _statusFlowService;
    private readonly IStatusFlowItemService _statusFlowItemService;
    private readonly IApproveLogService _approveLogService;
    private readonly INotificationService _notificationService;


    public EscolaController(
        IMediatorHandler bus,
        INotificationHandler<DomainNotification> notifications,

        ISchemaBuilder schemaBuilder,
        IRepository<MongoDbContext, Schema> schemaRepository,
        IRepository<MongoDbContext, ApiEntities.Action> actionRepository,
        IUserService userService,
        IEntityService entityService,
        IGeneratorsService generatorsService,
        IUserAccessService userAccessService,
        IStatusFlowService statusFlowService,
        IStatusFlowItemService statusFlowItemService,
        IApproveLogService approveLogService,
        INotificationService notificationService,
        IRepository<SourceContext, Source> sourceRepository) : base(bus, notifications)
    {
      _schemaBuilder = schemaBuilder;
      _schemaRepository = schemaRepository;
      _actionRepository = actionRepository;
      _userService = userService;
      _entityService = entityService;
      _generatorsService = generatorsService;
      _userAccessService = userAccessService;
      _statusFlowService = statusFlowService;
      _statusFlowItemService = statusFlowItemService;
      _approveLogService = approveLogService;
      _notificationService = notificationService;
      _sourceRepository = sourceRepository;
    }

    [Authorize]
    [HttpPost()]
    public async Task<IActionResult> Insert([FromBody] InsertEscolaModel command)
    {

      var schemaName = "T_Pessoa";
      if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
      var schema = _schemaRepository.GetSchemaByField("name", schemaName);
      var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
      var source = _sourceRepository.GetByField("description", schemaModel.Source);
      if (source != null && source.Active != null && source.Active == true)
      {
        var resultReturn = await ValidateCommand(command, source);
        var result = new
        {
          resultReturn.sucess,
          resultReturn.error
        };
        return resultReturn.sucess ? ResponseDefault(result) : BadRequest(result);

      }
      return BadRequest(new
      {
        error = "Fonte de dados não configurada ou inativa."
      });
    }

    async Task<(bool sucess, string error)> ValidateCommand(InsertEscolaModel command, Source source)
    {
      //valida cnpj
      if (command.pessoa.nm_cnpj_cgc != null && !string.IsNullOrEmpty(command.pessoa.nm_cnpj_cgc))
      {
        var filtros = new List<(string campo, object valor)> { new("dc_num_cnpj_cnab", command.pessoa.nm_cnpj_cgc) };
        var cnpjExist = await SQLServerService.GetFirstByFields(source, "T_PESSOA_JURIDICA", filtros);
        if (cnpjExist != null) return (false, $"Já existe um registro com este CNPJ({command.pessoa.nm_cnpj_cgc}) cadastrado");
      }

      //valida email
      if (command.pessoa.dc_email != null && !string.IsNullOrEmpty(command.pessoa.dc_email))
      {
        var filtros = new List<(string campo, object valor)> { new("dc_fone_mail", command.pessoa.dc_email) };
        var emailExist = await SQLServerService.GetFirstByFields(source, "T_TELEFONE", filtros);
        if (emailExist != null) return (false, $"Já existe um registro com este e-mail({command.pessoa.dc_email}) cadastrado");
      }

      try
      {

        var pessoaDict = new Dictionary<string, object>
                {
                    //{ "cd_pessoa", null },
                    { "no_pessoa", command.pessoa.no_pessoa },
                    { "dt_cadastramento", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") },
                    { "dc_reduzido_pessoa", command.pessoa.dc_reduzido_pessoa },
                    { "cd_atividade_principal", null },
                    { "cd_endereco_principal", null }, //cadastrar depois?
                    { "cd_telefone_principal", null }, //cadastrar depois?
                    { "id_pessoa_empresa",0 },
                    { "nm_natureza_pessoa",1 },
                    { "dc_num_pessoa", null },
                    { "id_exportado", 0 },
                    { "cd_papel_principal", null },
                    { "txt_obs_pessoa", null },
                    //{ "img_pessoa", null },
                    //{ "ext_img_pessoa", null }
                };

        var t_pessoa_insert = await SQLServerService.InsertWithResult("T_PESSOA", pessoaDict, source);
        if (!t_pessoa_insert.success) return new(t_pessoa_insert.success, t_pessoa_insert.error);
        var t_pessoa = t_pessoa_insert.inserted;
        var cd_pessoa = t_pessoa["cd_pessoa"];

        if (command.pessoa.dc_email != null)
        {
          //T_TELEFONE(email)
          var telefoneDictEmail = new Dictionary<string, object>
                    {
                        //{ "cd_telefone", null },
                        { "cd_pessoa", cd_pessoa },
                        { "cd_tipo_telefone", 4 },
                        { "cd_classe_telefone", 1 },
                        { "dc_fone_mail", command.pessoa.dc_email },
                        { "cd_endereco", null },
                        { "id_telefone_principal",1 },
                        { "cd_operadora", null }
                    };
          var t_telefone_email_insert = await SQLServerService.Insert("T_TELEFONE", telefoneDictEmail, source);
        }

        if (command.pessoa.telefone != null)
        {
          //T_TELEFONE(telefone)
          var telefoneDictTelefone = new Dictionary<string, object>
                    {
                        //{ "cd_telefone", null },
                        { "cd_pessoa", cd_pessoa },
                        { "cd_tipo_telefone", 1 },
                        { "cd_classe_telefone", 1 },
                        { "dc_fone_mail", command.pessoa.telefone },
                        { "cd_endereco", null },
                        { "id_telefone_principal", 1 },
                        { "cd_operadora", null }
                    };

          var t_telefone_telefone_insert = await SQLServerService.Insert("T_TELEFONE", telefoneDictTelefone, source);
        }
        //T_ENDERECO
        var enderecoDict = new Dictionary<string, object>
                {
                    //{ "cd_endereco", command.pessoa.endereco.cd_endereco },
                    { "cd_pessoa", cd_pessoa },
                    { "cd_loc_pais", command.pessoa.endereco.cd_loc_pais },
                    { "cd_loc_estado", command.pessoa.endereco.cd_loc_estado },
                    { "cd_loc_cidade", command.pessoa.endereco.cd_loc_cidade },
                    { "cd_tipo_endereco", command.pessoa.endereco.cd_tipo_endereco},
                    //{ "cd_loc_distrito", null },
                    { "cd_loc_bairro", command.pessoa.endereco.cd_loc_bairro },
                    { "cd_tipo_logradouro",command.pessoa.endereco.cd_tipo_logradouro},
                    { "cd_loc_logradouro", command.pessoa.endereco.cd_loc_logradouro },
                    //{ "nm_caixa_postal", command.pessoa.endereco.nm_caixa_postal },
                    //{ "nm_status_endereco", command.pessoa.endereco.nm_status_endereco },
                    { "dc_compl_endereco", command.pessoa.endereco.dc_compl_endereco },
                    { "id_exportado", 0 },
                    { "dc_num_cep", command.pessoa.endereco.dc_num_cep },
                    { "dc_num_endereco",  command.pessoa.endereco.dc_num_endereco },
                    //{ "dc_num_local_geografico", command.pessoa.endereco.dc_num_local_geografico}
                };
        var t_endereco_insert = await SQLServerService.Insert("T_ENDERECO", enderecoDict, source);

        var pessoa_juridica_dic = new Dictionary<string, object>
                    {
                        { "cd_pessoa_juridica", cd_pessoa },
                        { "cd_tipo_sociedade", !string.IsNullOrEmpty(command.pessoa.cd_tipo_sociedade) ? command.pessoa.cd_tipo_sociedade : "1" },
                        //{ "dc_num_cgc", "" },
                        //{ "dt_registro_junta_comercial", "2000-01-01" },
                        { "dc_num_insc_estadual", command.pessoa.dc_num_insc_estadual },
                        { "id_exportado", 0 },
                        { "dc_num_insc_municipal", command.pessoa.dc_num_insc_municipal },
                        { "dc_num_cnpj_cnab", command.pessoa.nm_cnpj_cgc },
                        //{ "dc_registro_junta_comercial", "" },
                        //{ "dt_baixa", "[NULO]" },
                        { "dc_nom_presidente", command.pessoa.no_pessoa }
                    };
        var t_pessoa_juridica_insert = await SQLServerService.Insert("T_PESSOA_JURIDICA", pessoa_juridica_dic, source);
        if (!t_pessoa_juridica_insert.success) return new(t_pessoa_juridica_insert.success, t_pessoa_juridica_insert.error);
        //T_PESSOA_EMPRESA

        var pessoa_empresa_dict = new Dictionary<string, object>
                {
                     { "cd_pessoa", cd_pessoa },
                     { "cd_empresa", command.cd_pessoa_escola }
                };
        var t_pessoa_empresa_insert = await SQLServerService.InsertWithResult("T_PESSOA_EMPRESA", pessoa_empresa_dict, source);
        if (!t_pessoa_empresa_insert.success) return new(t_pessoa_empresa_insert.success, t_pessoa_empresa_insert.error);
        var cd_pessoa_escola = t_pessoa_empresa_insert.inserted["cd_pessoa_empresa"];

        var dictcompanySite = new Dictionary<string, object>
                {
                     { "filial", command.pessoa.no_pessoa },
                     { "cnpj", command.pessoa.nm_cnpj_cgc },
                     { "cd_empresa", command.cd_pessoa_escola.ToString() }
                };


        await InsertCompanySite("CompanySite", null, null, null, null, null, dictcompanySite);




      }
      catch (Exception ex)
      {
        return (false, $"Erro: {ex.Message}");
      }


      return (true, string.Empty);
    }

    async Task InsertCompanySite(string schemaName, string latitude, string longitude, string ip, string userAgent, string description, [FromBody] Dictionary<string, object> data)
    {
      if (data.Count() == 0) return;

      var schema = _schemaRepository.GetSchemaByField("name", schemaName);

      var json = schema.JsonValue;
      string quote = "\"";
      schema.JsonValue = schema.JsonValue.Replace($"{quote}enum{quote}:[", $"{quote}enum{quote}:[{quote + quote},");
      var schemaProp = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(json).Properties;



      data = data.Where(kv => schemaProp.ContainsKey(kv.Key)).ToDictionary(kv => kv.Key, kv => kv.Value);
      string id = Guid.NewGuid().ToString();



      if (data.Keys.FirstOrDefault(key => key == "id") == null) data.Add("id", null);

      data["id"] = id;

      var beforeSave = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(json).BeforeSave;
      if (beforeSave != null)
      {
        var type = await _schemaBuilder.GetSchemaType(schemaName);
        string jsonSchema = JsonConvert.SerializeObject(data, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        var entityAction = JsonConvert.DeserializeObject(jsonSchema, type);
        var action = _actionRepository.GetByField("name", beforeSave);

        if (action != null)
        {
          ActionController x = new(null, null, _schemaRepository, _entityService, _actionRepository, null, _userService);
          ApiEntities.Action[] ac = x.GetBySchemaName(schemaName);

          var userHelper = _schemaRepository.GetUserHelper();
          var user = _userService.GetByUserName(userHelper.GetTenanty(), userHelper.GetUserName());

          string auth = Request.Headers["Authorization"];


          var result = await x.Run(action, new Dictionary<string, object>()
                {
                    { "params", null }
                }, auth, user, entityAction);
        }

      }
      var statusF = false;
      var field = "";

      foreach (var prop in schemaProp)
      {
        if (prop.Value.unique == true)
        {
          var FromData = data[prop.Key];
          var existe = _entityService.GetByFieldName(schemaName, prop.Key, FromData.ToString());
          statusF = prop.Value.statusF;
          if (existe.Result == true)
          {
            return;
          }

        }
        if (prop.Value.uniqueCompany == true && data.ContainsKey("companySiteId"))
        {
          var FromData = data[prop.Key];

          var fromDataCompany = data["companySiteId"];

          dynamic existe = await _entityService.GetByFieldNameCompany(schemaName, prop.Key, FromData.ToString(), fromDataCompany.ToString());

          statusF = prop.Value.statusF;

          if (existe != null)
          {
            return;
          }
        }
        if (prop.Value.statusF)
        {
          field = prop.Key;
          statusF = true;
        }
        if (prop.Value.@enum != null)
        {
          if (!data.ContainsKey(prop.Key))
          {
            data.Add(prop.Key, "");
          }
        }
        if (prop.Value.Type == "date" && data.ContainsKey(prop.Key))
        {
          if (data[prop.Key] != null && data[prop.Key].ToString() == "") data[prop.Key] = null;
          if (data[prop.Key] != null)
          {
            if (DateTime.TryParse(data[prop.Key].ToString(), out _)) data[prop.Key] = DateTime.Parse(data[prop.Key].ToString()).AddHours(3);

            else data[prop.Key] = null;
          }

        }
        if (prop.Value.AutoInc)
        {
          var autoIncField = prop.Key.Replace(prop.Key[0], char.ToUpper(prop.Key[0]));
          if (!data.ContainsKey(prop.Key))
          {
            var generatedAutoInc = await _generatorsService.GetAutoincAsync(schemaName, autoIncField, "");
            if (generatedAutoInc != null) data.Add(prop.Key, generatedAutoInc);
          }
        }
      }
      var userId = "";
      var userName = "";
      var tenanty = "";
      try
      {
        var accessToken = Request.Headers[HeaderNames.Authorization];

        var tokenInfo = Util.GetUserInfoFromToken(accessToken);

        if (tokenInfo.Count > 0)
        {
          userName = tokenInfo["username"];
          userId = tokenInfo["userid"];
          tenanty = tokenInfo["tenanty"];
        }

      }
      catch (Exception)
      {
        throw;
      }
      if (data.Keys.FirstOrDefault(key => key == "createBy") == null) data.Add("createBy", userName);
      var command = new InsertEntityCommand()
      {
        SchemaName = schemaName,
        SchemaJson = schema.JsonValue,
        Data = data
      };
      await SendCommand(command);
      var schemaModelData = JsonConvert.DeserializeObject<Simjob.Framework.Infra.Domain.Models.SchemaModel>(schema.JsonValue);
      var properties = schemaModelData.Properties;
      var fileFields = properties.Where(x => x.Value.file != null).ToList();
      if (fileFields.Count > 0) Thread.Sleep(3000);
      else
      {
        Thread.Sleep(1000);
      }


      var values = new List<object>(data.Values);
      var keys = new List<string>(data.Keys);
      var Lvalues = new List<string>();
      //Dictionary<string, string> dict = new Dictionary<string, string>();
      Dictionary<string, string> convertedDictionary = data
      .ToDictionary(
          kvp => kvp.Key,
          kvp => kvp.Value != null ? kvp.Value.ToString() : null
      );


      var newUserAccess = new UserAccess(userId, tenanty, userName, latitude, longitude, ip, userAgent, schemaName, description, id, convertedDictionary, null);
      await _userAccessService.Register(newUserAccess);

      await RunActionForSchema(schemaName, data["id"].ToString());


      if (statusF)
      {
        int? aprovMin = null;

        var listEmailAprov = new List<AprovEmailProperties>();
        var statusFlow = _statusFlowService.GetBySchemaAndField(schemaName, field, tenanty);
        if (statusFlow != null)
        {
          foreach (var prop in statusFlow.Properties)
          {
            var dataAprov = DateTime.Now;
            if (prop.Status == data[statusFlow.Field].ToString())
            {
              List<string> aprovadoresList = new List<string>();
              if (!prop.Action.IsNullOrEmpty() && prop.Type == "Action")
              {
                var statusFlowAction = _actionRepository.GetByField("name", prop.Action);

                if (statusFlowAction != null)
                {
                  ActionController x = new(null, null, _schemaRepository, _entityService, _actionRepository, null, _userService);
                  ApiEntities.Action[] ac = x.GetBySchemaName(schemaName);

                  var userHelper = _schemaRepository.GetUserHelper();
                  var user = _userService.GetByUserName(userHelper.GetTenanty(), userHelper.GetUserName());

                  string auth = Request.Headers["Authorization"];

                  dynamic result = await x.Run(statusFlowAction, new Dictionary<string, object>()
                                    {
                                        { "params", new { entity = data } }
                                    }, auth, user, null);

                  aprovadoresList = result.aprovadores.ToObject<List<string>>();
                  if (!aprovadoresList.IsNullOrEmpty())
                  {
                    if (!aprovadoresList.Contains("admin@admin.com")) aprovadoresList.Add("admin@admin.com");
                    aprovadoresList = aprovadoresList.Distinct().ToList();
                    _statusFlowItemService.RegisterAsync(new StatusFlowItem(statusFlow.Id, data["id"].ToString(), prop.Status, aprovadoresList.ToArray()));
                  }
                  //_statusFlowService.UpdateStatusFlowStatus(statusFlow, aprovadoresList, prop.Status);
                }
                else
                {
                  return;
                }
                aprovMin = prop.AprovMin;
                foreach (var email in aprovadoresList) listEmailAprov.Add(new AprovEmailProperties { email = email, aprov = null, view = false, data = null });
              }
              else
              {
                aprovMin = prop.AprovMin;
                var aprovEmails = prop.AprovEmail;
                var users = _userService.GetUsersByUserName(aprovEmails, tenanty);
                if (!users.IsNullOrEmpty())
                {
                  var userEmails = users.Select(x => x.UserName).ToList();
                  foreach (var email in userEmails) listEmailAprov.Add(new AprovEmailProperties { email = email, aprov = null, view = false, data = null });
                  if (!userEmails.IsNullOrEmpty())
                  {
                    if (!userEmails.Contains("admin@admin.com")) userEmails.Add("admin@admin.com");
                    _statusFlowItemService.RegisterAsync(new StatusFlowItem(statusFlow.Id, data["id"].ToString(), prop.Status, userEmails.ToArray()));
                  }
                }
              }

            }

          }
          if (aprovMin != null && !listEmailAprov.IsNullOrEmpty())
          {
            //var aprov = listEmail.Where(x => x.aprov == true).Count() >= aprovMin ? true : false;
            var newNotification = new Notification(id, schemaName, statusFlow.Field, data[statusFlow.Field].ToString(), null, "Mudança de status para " + data[statusFlow.Field].ToString(), null, null, false, (int)aprovMin, listEmailAprov.ToArray(), null);
            newNotification.CreateBy = userName;
            _notificationService.RegisterAsync(newNotification);

            // sendgrid
            foreach (var e in newNotification.AprovEmail)
            {
              var sendEmailCommand = new SendEmailCommand();
              sendEmailCommand.Subject = "Notificação";
              sendEmailCommand.To = e.email;
              sendEmailCommand.PlainTextContent = newNotification.Msg;
              sendEmailCommand.HtmlContent = newNotification.Msg;
              _userAccessService.SendEmail(sendEmailCommand);
            }
            if (listEmailAprov.Count > 1)
            {
              var newApproveLog = new ApproveLog(id, schemaName, statusFlow.Field, data[statusFlow.Field].ToString(), "Mudança de status para " + data[statusFlow.Field].ToString());
              _approveLogService.Register(newApproveLog);
            }
          }

        }

      }
      var schemaPropRelations = schemaProp.Where(x => !string.IsNullOrEmpty(x.Value.RelationSchema)).ToList();
      if (schemaPropRelations.Any())
      {
        var sendObject = schemaPropRelations
        .Select(x =>
        {
          var schemaChildren = _schemaRepository.GetSchemaByField("name", x.Value.RelationSchema);
          var jsonValue = schemaChildren.JsonValue; // Store JSON schema for access later
          string quoteChildren = "\"";
          jsonValue = jsonValue.Replace($"{quoteChildren}enum{quoteChildren}:[", $"{quoteChildren}enum{quoteChildren}:[{quoteChildren + quoteChildren},");
          var schemaPropChildren = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(jsonValue).Properties;

          if (!schemaPropChildren.ContainsKey("parentId"))
            return null; // Return null to filter later

          var matchingData = data.FirstOrDefault(y => y.Key == x.Key).Value;
          var childrenIds = ((JArray)matchingData)?.ToObject<List<string>>();

          return new
          {
            fieldName = x.Key,
            schemaName = x.Value.RelationSchema,
            childrenIds = childrenIds,
            jsonValue = jsonValue,
            parentId = id,
          };
        })
        .Where(x => x != null) // Remove null entries
        .ToList();

        if (sendObject.Any())
        {
          foreach (var item in sendObject)
          {
            dynamic entitiesOld = await _entityService.GetByIdsList(item.schemaName, item.childrenIds);

            List<Dictionary<string, object>> datas = ((IEnumerable<dynamic>)entitiesOld)
            .Cast<object>() // Cast to a non-dynamic type to resolve the issue
            .Select(entityUpdate =>
            {
              var properties = entityUpdate.GetType()
                              .GetProperties(BindingFlags.Instance | BindingFlags.Public);

              // Create dictionary with properties
              var dictionary = properties.ToDictionary(
                             prop => char.ToLowerInvariant(prop.Name[0]) + prop.Name.Substring(1), // Convert to camelCase
                             prop => prop.GetValue(entityUpdate, null)
                         );

              // Add parentId property
              dictionary["parentId"] = item.parentId; // Ensure `item.parentId` is accessible
              dictionary["updateAt"] = DateTime.Now;

              return dictionary;
            }).ToList();

            var commandChildrenUpdate = new UpdateManyEntityCommand()
            {
              Id = id,
              SchemaName = item.schemaName,
              SchemaJson = item.jsonValue,
              Datas = datas
            };

            await SendCommand(commandChildrenUpdate);
          }
        }
      }

      var entity = await _entityService.GetById(schemaName, id);
      //var afterSave = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(json).AfterSave;
      //if (afterSave != null)
      //{
      //    var action = _actionRepository.GetByField("name", afterSave);

      //    if (action != null)
      //    {
      //        ActionController x = new(null, null, _schemaRepository, _entityService, _actionRepository, null, _userService);
      //        ApiEntities.Action[] ac = x.GetBySchemaName(schemaName);

      //        var userHelper = _schemaRepository.GetUserHelper();
      //        var user = _userService.GetByUserName(userHelper.GetTenanty(), userHelper.GetUserName());

      //        string auth = Request.Headers["Authorization"];


      //        var result = await x.Run(action, new Dictionary<string, object>()
      //    {
      //        { "params", null }
      //    }, auth, user, entity);
      //    }

      //}
      return;
    }

    private async Task RunActionForSchema(string schemaName, string entityId = "")
    //#pragma warning restore CS8632
    {
      ActionController x = new(null, null, _schemaRepository, _entityService, _actionRepository, null, _userService);
      ApiEntities.Action[] ac = x.GetBySchemaName(schemaName);

      var userHelper = _schemaRepository.GetUserHelper();
      var user = _userService.GetByUserName(userHelper.GetTenanty(), userHelper.GetUserName());

      string auth = Request.Headers["Authorization"];

      object entity = null;
      if (!string.IsNullOrEmpty(entityId))
      {
        entity = await _entityService.GetById(schemaName, entityId);
        if (entity == null)
        {
          SendNotification("Run", "Entity not found by the provided entityId: " + entityId);
        }
      }

      foreach (var action in ac)
      {
        if (action.CallType == ApiEntities.ActionCallType.EventSchema)
          await x.Run(action, new Dictionary<string, object>()
                    {
                        { "params", null }
                    }, auth, user, entity);
      }
    }




    /// <summary>
    /// Busca horário de funcionamento de uma unidade específica
    /// </summary>
    private async Task<Dictionary<string, object>?> BuscarHorarioFuncionamentoUnidade(
        int cd_pessoa_escola,
        Source source)
    {
      try
      {
        // id_origem = 0 representa horário de funcionamento da ESCOLA/UNIDADE
        var horarios = await SQLServerService.GetList(
            "T_HORARIO",
            null,
            "[cd_pessoa_escola],[id_origem]",
            $"[{cd_pessoa_escola}],[0]",
            source,
            SearchModeEnum.Equals
        );

        if (horarios.data == null || !horarios.data.Any())
          return null;

        // Buscar informações da escola
        var escola = await SQLServerService.GetFirstByFields(
            source,
            "T_PESSOA",
            new List<(string campo, object valor)> { ("cd_pessoa", cd_pessoa_escola) }
        );

        // Organizar horários por dia da semana
        var horariosPorDia = horarios.data
            .Select(h => new
            {
              id_dia_semana = Convert.ToInt32(h["id_dia_semana"]),
              dia_semana_nome = ObterNomeDiaSemana(Convert.ToInt32(h["id_dia_semana"])),
              hora_inicio = h["dt_hora_ini"]?.ToString() ?? "",
              hora_fim = h["dt_hora_fim"]?.ToString() ?? "",
              id_disponivel = h.ContainsKey("id_disponivel")
                    ? Convert.ToBoolean(h["id_disponivel"])
                    : true
            })
            .OrderBy(h => h.id_dia_semana)
            .ToList();

        // Calcular horário geral (mais cedo e mais tarde)
        var horaInicioGeral = horariosPorDia
            .Where(h => !string.IsNullOrEmpty(h.hora_inicio))
            .Select(h => TimeSpan.Parse(h.hora_inicio))
            .OrderBy(h => h)
            .FirstOrDefault();

        var horaFimGeral = horariosPorDia
            .Where(h => !string.IsNullOrEmpty(h.hora_fim))
            .Select(h => TimeSpan.Parse(h.hora_fim))
            .OrderByDescending(h => h)
            .FirstOrDefault();

        var resultado = new Dictionary<string, object>
        {
          ["cd_pessoa_escola"] = cd_pessoa_escola,
          ["no_pessoa"] = escola?["no_pessoa"]?.ToString() ?? "",
          ["dc_reduzido_pessoa"] = escola?["dc_reduzido_pessoa"]?.ToString() ?? "",
          ["hora_inicio_geral"] = horaInicioGeral != default(TimeSpan)
                ? horaInicioGeral.ToString(@"hh\:mm")
                : "",
          ["hora_fim_geral"] = horaFimGeral != default(TimeSpan)
                ? horaFimGeral.ToString(@"hh\:mm")
                : "",
          ["horario_formatado"] = horaInicioGeral != default(TimeSpan) && horaFimGeral != default(TimeSpan)
                ? $"{horaInicioGeral:hh\\:mm} às {horaFimGeral:hh\\:mm}"
                : "",
          ["horarios_por_dia"] = horariosPorDia,
          ["total_dias_configurados"] = horariosPorDia.Count
        };

        return resultado;
      }
      catch (Exception ex)
      {
        // Log do erro se necessário
        return null;
      }
    }


    private (Source source, bool success, string error) ValidarFonteDados()
    {
      var schemaName = "Pessoa";

      // Remove prefixo T_ se existir
      if (schemaName.Contains("T_"))
        schemaName = schemaName.Replace("T_", "").Replace("_", "");

      // Busca o schema no repositório
      var schema = _schemaRepository.GetSchemaByField("name", schemaName);

      if (schema == null)
      {
        return (null, false, "Schema não encontrado.");
      }

      // Desserializa o schema
      var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);

      // Busca a fonte de dados
      var source = _sourceRepository.GetByField("description", schemaModel.Source);

      // Valida se a fonte está ativa
      if (source == null || source.Active != true)
      {
        return (null, false, "Fonte de dados não configurada ou inativa.");
      }

      return (source, true, null);
    }

    /// <summary>
    /// Método auxiliar para retornar resposta padrão
    /// </summary>
    private IActionResult ResponseDefault(object data = null)
    {
      return Ok(new
      {
        success = true,
        data = data
      });
    }


    /// <summary>
    /// Busca apenas o horário geral (início e fim) de uma unidade - formato simplificado
    /// GET: api/escola/horario-funcionamento/simples/123
    /// </summary>
    [Authorize]
    [HttpGet("horario-funcionamento/simples/{cd_pessoa_escola}")]
    public async Task<IActionResult> GetHorarioFuncionamentoSimples(int cd_pessoa_escola)
    {
      var fonteDados = ValidarFonteDados();
      if (!fonteDados.success)
      {
        return BadRequest(new
        {
          success = false,
          error = fonteDados.error
        });
      }

      try
      {
        var horario = await BuscarHorarioFuncionamentoUnidade(
            cd_pessoa_escola,
            fonteDados.source
        );

        if (horario == null)
        {
          return NotFound(new
          {
            success = false,
            message = "Horário não configurado"
          });
        }

        // Retorna apenas informações essenciais
        return Ok(new
        {
          success = true,
          data = new
          {
            cd_pessoa_escola = horario["cd_pessoa_escola"],
            hora_inicio = horario["hora_inicio_geral"],
            hora_fim = horario["hora_fim_geral"],
            horario_formatado = horario["horario_formatado"]
          }
        });
      }
      catch (Exception ex)
      {
        return BadRequest(new
        {
          success = false,
          message = $"Erro: {ex.Message}"
        });
      }
    }


    private string ObterNomeDiaSemana(int id_dia_semana)
    {
      // Se vier no formato 0-6 (JavaScript), converte para 1-7
      // 0 (Domingo) vira 1, 1 (Segunda) vira 2, etc.
      if (id_dia_semana == 0)
      {
        id_dia_semana = 1;
      }


      return id_dia_semana switch
      {
        1 => "Segunda-feira",
        2 => "Terça-feira",
        3 => "Quarta-feira",
        4 => "Quinta-feira",
        5 => "Sexta-feira",
        6 => "Sábado",
        7 => "Domingo",
        _ => $"Desconhecido ({id_dia_semana})" // Mostra o valor para debug
      };
    }

    /// <summary>
    /// Busca horários de funcionamento de TODAS as unidades
    /// </summary>
    private async Task<List<Dictionary<string, object>>> BuscarHorariosFuncionamentoTodasUnidades(
        Source source)
    {
      try
      {
        // Buscar todos os horários com id_origem = 0 (escola/unidade)
        var horarios = await SQLServerService.GetList(
            "T_HORARIO",
            1,
            1000000, // Limite alto para pegar todos
            "cd_pessoa_escola",
            true,
            null,
            "[id_origem]",
            "[0]",
            source,
            SearchModeEnum.Equals
        );

        if (horarios.data == null || !horarios.data.Any())
          return new List<Dictionary<string, object>>();

        // Agrupar por escola
        var escolasComHorario = horarios.data
            .GroupBy(h => Convert.ToInt32(h["cd_pessoa_escola"]))
            .ToList();

        var resultado = new List<Dictionary<string, object>>();

        foreach (var grupo in escolasComHorario)
        {
          var cd_pessoa_escola = grupo.Key;

          // Buscar dados da escola
          var escola = await SQLServerService.GetFirstByFields(
              source,
              "T_PESSOA",
              new List<(string campo, object valor)> { ("cd_pessoa", cd_pessoa_escola) }
          );

          // Organizar horários por dia
          var horariosPorDia = grupo
              .Select(h => new
              {
                id_dia_semana = Convert.ToInt32(h["id_dia_semana"]),
                dia_semana_nome = ObterNomeDiaSemana(Convert.ToInt32(h["id_dia_semana"])),
                hora_inicio = h["dt_hora_ini"]?.ToString() ?? "",
                hora_fim = h["dt_hora_fim"]?.ToString() ?? "",
                id_disponivel = h.ContainsKey("id_disponivel")
                      ? Convert.ToBoolean(h["id_disponivel"])
                      : true
              })
              .OrderBy(h => h.id_dia_semana)
              .ToList();

          // Calcular horário geral
          var horaInicioGeral = horariosPorDia
              .Where(h => !string.IsNullOrEmpty(h.hora_inicio))
              .Select(h => TimeSpan.Parse(h.hora_inicio))
              .OrderBy(h => h)
              .FirstOrDefault();

          var horaFimGeral = horariosPorDia
              .Where(h => !string.IsNullOrEmpty(h.hora_fim))
              .Select(h => TimeSpan.Parse(h.hora_fim))
              .OrderByDescending(h => h)
              .FirstOrDefault();

          resultado.Add(new Dictionary<string, object>
          {
            ["cd_pessoa_escola"] = cd_pessoa_escola,
            ["no_pessoa"] = escola?["no_pessoa"]?.ToString() ?? "",
            ["dc_reduzido_pessoa"] = escola?["dc_reduzido_pessoa"]?.ToString() ?? "",
            ["hora_inicio_geral"] = horaInicioGeral != default(TimeSpan)
                  ? horaInicioGeral.ToString(@"hh\:mm")
                  : "",
            ["hora_fim_geral"] = horaFimGeral != default(TimeSpan)
                  ? horaFimGeral.ToString(@"hh\:mm")
                  : "",
            ["horario_formatado"] = horaInicioGeral != default(TimeSpan) && horaFimGeral != default(TimeSpan)
                  ? $"{horaInicioGeral:hh\\:mm} às {horaFimGeral:hh\\:mm}"
                  : "",
            ["horarios_por_dia"] = horariosPorDia,
            ["total_dias_configurados"] = horariosPorDia.Count
          });
        }

        return resultado;
      }
      catch (Exception ex)
      {
        // Log do erro se necessário
        return new List<Dictionary<string, object>>();
      }
    }

    /// <summary>
    /// Busca horário de funcionamento de uma unidade específica ou todas
    /// GET: api/escola/horario-funcionamento
    /// GET: api/escola/horario-funcionamento?cd_pessoa_escola=123
    /// </summary>
    [Authorize]
    [HttpGet("horario-funcionamento")]
    public async Task<IActionResult> GetHorarioFuncionamento([FromQuery] int? cd_pessoa_escola)
    {
      var fonteDados = ValidarFonteDados();
      if (!fonteDados.success)
      {
        return BadRequest(new
        {
          success = false,
          error = fonteDados.error
        });
      }

      try
      {
        // Se passou cd_pessoa_escola, busca uma unidade específica
        if (cd_pessoa_escola.HasValue)
        {
          var horario = await BuscarHorarioFuncionamentoUnidade(
              cd_pessoa_escola.Value,
              fonteDados.source
          );

          if (horario == null)
          {
            return NotFound(new
            {
              success = false,
              message = "Horário de funcionamento não configurado para esta unidade",
              cd_pessoa_escola = cd_pessoa_escola.Value
            });
          }

          return Ok(new
          {
            success = true,
            data = horario
          });
        }
        // Se não passou, busca todas as unidades
        else
        {
          var horarios = await BuscarHorariosFuncionamentoTodasUnidades(
              fonteDados.source
          );

          return Ok(new
          {
            success = true,
            total = horarios.Count,
            data = horarios
          });
        }
      }
      catch (Exception ex)
      {
        return BadRequest(new
        {
          success = false,
          message = $"Erro ao buscar horário: {ex.Message}"
        });
      }
    }


  }
}

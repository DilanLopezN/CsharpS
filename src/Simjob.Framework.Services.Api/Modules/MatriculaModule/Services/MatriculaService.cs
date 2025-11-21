using Microsoft.IdentityModel.Tokens;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Services.Api.Enums;
using Simjob.Framework.Services.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Xceed.Words.NET;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Schemas.Entities;
using Microsoft.AspNetCore.Hosting;
using MongoDB.Bson.IO;
using Newtonsoft.Json;
using SendGrid.Helpers.Errors.Model;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;


namespace Simjob.Framework.Services.Api.Modules.TurmaModule.Services
{
  /// <summary>
  /// Serviço responsável pela lógica de negócio de Turmas
  /// Seguindo padrão de módulos similar ao NestJS
  /// </summary>
  public class MatriculaService
  {
    #region Dependências

    private readonly IRepository<SourceContext, Source> _sourceRepository;
    private readonly IRepository<MongoDbContext, Schema> _schemaRepository;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly SimulacaoBaixaService _simulacaoBaixaService;




    #endregion

    public MatriculaService(
      IRepository<SourceContext, Source> sourceRepository,
      IRepository<MongoDbContext, Schema> schemaRepository,
      IWebHostEnvironment webHostEnvironment,
      SimulacaoBaixaService simulacaoBaixaService
       )
    {
      _sourceRepository = sourceRepository;
      _schemaRepository = schemaRepository;
      _webHostEnvironment = webHostEnvironment;
      _simulacaoBaixaService = simulacaoBaixaService;

    }



    public async Task<(MemoryStream arquivo, string nomeContrato)> GerarContratoMatricula(int cdContrato, int cdPessoaEscola)
    {
      try
      {
        var schemaName = "T_Pessoa";
        if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
        var schema = _schemaRepository.GetSchemaByField("name", schemaName);
        var schemaModel = Newtonsoft.Json.JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
        var source = _sourceRepository.GetByField("description", schemaModel.Source);

        if (source == null || source.Active == null || source.Active == false)
        {
          throw new BadRequestException("Source não encontrado ou inativo.");
        }

        //valida se matricula existe
        var matriculaExists = await SQLServerService.GetFirstByFields(source, "T_CONTRATO", new List<(string campo, object valor)> { new("cd_contrato", cdContrato) });
        if (matriculaExists == null) throw new NotFoundException("Matrícula não encontrada.");

        var cd_nome_contrato = matriculaExists["cd_nome_contrato"];



        var cd_pessoa_escola = Convert.ToInt32(matriculaExists["cd_pessoa_escola"]);
        string nomeContrato = "";
        try
        {
          nomeContrato = await DeterminarNomeTemplate(matriculaExists, cd_pessoa_escola, source);
        }
        catch (Exception ex)
        {
          throw new NotFoundException(ex.Message);
        }
        #region ESCOLA
        //ESCOLA
        var pessoa_escola = await SQLServerService.GetFirstByFields(source, "T_PESSOA", new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa_escola) });
        var nomeEscola = pessoa_escola?["dc_reduzido_pessoa"]?.ToString() ?? "";
        var razaoSocialEscola = pessoa_escola?["no_pessoa"]?.ToString() ?? "";
        var pessoa_escola_juridica = await SQLServerService.GetFirstByFields(source, "T_PESSOA_JURIDICA", new List<(string campo, object valor)> { new("cd_pessoa_juridica", cd_pessoa_escola) });
        var cnpjEscola = pessoa_escola_juridica?["dc_num_cgc"]?.ToString() ?? "";
        var endereco_escola = await SQLServerService.GetFirstByFields(source, "T_ENDERECO", new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa_escola) });
        var enderecoEscolaMontado = "";
        var cidadeEstadoEscola = "";

        if (endereco_escola != null)
        {
          if (endereco_escola.ContainsKey("cd_loc_logradouro") && endereco_escola["cd_loc_logradouro"] != null)
          {
            var filtroLogradouro = new List<(string campo, object valor)> { new("cd_localidade", endereco_escola["cd_loc_logradouro"].ToString()) };
            var logradouroExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtroLogradouro);
            if (logradouroExists != null && logradouroExists.ContainsKey("no_localidade"))
            {
              var numEndereco = endereco_escola["dc_num_endereco"]?.ToString() ?? "";
              enderecoEscolaMontado = $"{logradouroExists["no_localidade"]?.ToString() ?? ""},{numEndereco}";
            }
          }

          if (endereco_escola.ContainsKey("cd_loc_estado") && endereco_escola["cd_loc_estado"] != null)
          {
            var filtroEstado = new List<(string campo, object valor)> { new("cd_localidade", endereco_escola["cd_loc_estado"].ToString()) };
            var estadoExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtroEstado);
            if (estadoExists != null)
            {
              if (endereco_escola.ContainsKey("cd_loc_cidade") && endereco_escola["cd_loc_cidade"] != null)
              {
                var filtroCidade = new List<(string campo, object valor)> { new("cd_localidade", endereco_escola["cd_loc_cidade"].ToString()) };
                var cidadeExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtroCidade);
                if (cidadeExists != null && cidadeExists.ContainsKey("no_localidade") && estadoExists.ContainsKey("no_localidade"))
                {
                  cidadeEstadoEscola = $"{cidadeExists["no_localidade"]?.ToString() ?? ""}/{estadoExists["no_localidade"]?.ToString() ?? ""}";
                }
              }
            }
          }
        }
        #endregion

        #region RESPONSAVEL
        // RESPONSAVEL
        var cd_responsavel = matriculaExists["cd_pessoa_responsavel"];
        var pessoa_responsavel = await SQLServerService.GetFirstByFields(source, "T_PESSOA", new List<(string campo, object valor)> { new("cd_pessoa", cd_responsavel) });
        var nomeResponsavel = pessoa_responsavel?["no_pessoa"]?.ToString() ?? "";
        var pessoa_responsavel_fisica = await SQLServerService.GetFirstByFields(source, "T_PESSOA_FISICA", new List<(string campo, object valor)> { new("cd_pessoa_fisica", cd_responsavel) });
        var rg_pessoa_responsavel = pessoa_responsavel_fisica?["nm_doc_identidade"]?.ToString() ?? "";
        var cpfResponsavel = pessoa_responsavel_fisica?["nm_cpf"]?.ToString() ?? "";
        var tituloRGResponsavel = pessoa_responsavel?["nm_natureza_pessoa"]?.ToString() == "1" ? "RG" : "";
        var tituloCPFouCNPJResponsavel = pessoa_responsavel?["nm_natureza_pessoa"]?.ToString() == "1" ? "CPF" : "CNPJ";
        var telefoneResponsavel = "";
        var telefone_responsavel = await SQLServerService.GetFirstByFields(source, "T_TELEFONE", new List<(string campo, object valor)> { new("cd_pessoa", cd_responsavel), new("cd_tipo_telefone", 1) });
        if (telefone_responsavel != null)
        {
          telefoneResponsavel = telefone_responsavel["dc_fone_mail"]?.ToString() ?? "";
        }
        var email_responsavel = await SQLServerService.GetFirstByFields(source, "T_TELEFONE", new List<(string campo, object valor)> { new("cd_pessoa", cd_responsavel), new("cd_tipo_telefone", 4) });
        var emailResponsavel = email_responsavel?["dc_fone_mail"]?.ToString() ?? "";

        var celular_responsavel = await SQLServerService.GetFirstByFields(source, "T_TELEFONE", new List<(string campo, object valor)> { new("cd_pessoa", cd_responsavel), new("cd_tipo_telefone", 3) });
        var celularResponsavel = celular_responsavel?["dc_fone_mail"]?.ToString() ?? "";

        var dataNascResponsavel = "";
        if (pessoa_responsavel_fisica != null && pessoa_responsavel_fisica.ContainsKey("dt_nascimento") && pessoa_responsavel_fisica["dt_nascimento"] != null)
        {
          if (DateTime.TryParse(pessoa_responsavel_fisica["dt_nascimento"].ToString(), out DateTime dt_nasc_resp))
          {
            dataNascResponsavel = dt_nasc_resp.ToString("dd/MM/yyyy");
          }
        }
        var endereco_responsavel = await SQLServerService.GetFirstByFields(source, "T_ENDERECO",
          new List<(string campo, object valor)> { new("cd_pessoa", cd_responsavel) });
        var enderecoResponsavel = "";

        if (endereco_responsavel != null)
        {
          if (endereco_responsavel.ContainsKey("cd_tipo_logradouro") &&
               endereco_responsavel["cd_tipo_logradouro"] != null)
          {
            var tipoLogradouroExists = await SQLServerService.GetFirstByFields(
              source,
              "T_TIPO_LOGRADOURO",  // ⬅️ TABELA CORRETA!
              new List<(string campo, object valor)> {
        new("cd_tipo_logradouro", endereco_responsavel["cd_tipo_logradouro"].ToString())
              });

            if (tipoLogradouroExists != null && tipoLogradouroExists.ContainsKey("no_tipo_logradouro"))
            {
              enderecoResponsavel = $"{tipoLogradouroExists["no_tipo_logradouro"]?.ToString() ?? ""} ";
            }
          }
          // Buscar nome do logradouro
          if (endereco_responsavel.ContainsKey("cd_loc_logradouro") &&
              endereco_responsavel["cd_loc_logradouro"] != null)
          {
            var filtroLogradouro = new List<(string campo, object valor)>
            { new("cd_localidade", endereco_responsavel["cd_loc_logradouro"].ToString()) };
            var logradouroExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtroLogradouro);
            if (logradouroExists != null && logradouroExists.ContainsKey("no_localidade"))
            {
              enderecoResponsavel += $"{logradouroExists["no_localidade"]?.ToString() ?? ""}";
            }
          }

          var numEndereco = endereco_responsavel["dc_num_endereco"]?.ToString() ?? "";
          if (!String.IsNullOrEmpty(numEndereco))
            enderecoResponsavel += ", Nº " + numEndereco;

          var complEndereco = endereco_responsavel["dc_compl_endereco"]?.ToString() ?? "";
          if (!String.IsNullOrEmpty(complEndereco))
            enderecoResponsavel += ", " + complEndereco;

          var cep = endereco_responsavel["dc_num_cep"]?.ToString() ?? "";
          if (!String.IsNullOrEmpty(cep))
            enderecoResponsavel += ", CEP: " + cep;

          if (endereco_responsavel.ContainsKey("cd_loc_bairro") && endereco_responsavel["cd_loc_bairro"] != null)
          {
            var cdBairro = endereco_responsavel["cd_loc_bairro"].ToString();
            if (!String.IsNullOrEmpty(cdBairro))
            {
              var filtroBairro = new List<(string campo, object valor)> { new("cd_localidade", cdBairro) };
              var bairroExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtroBairro);
              if (bairroExists != null && bairroExists.ContainsKey("no_localidade"))
              {
                enderecoResponsavel += ", Bairro: " + (bairroExists["no_localidade"]?.ToString() ?? "");
              }
            }
          }

          if (endereco_responsavel.ContainsKey("cd_loc_cidade") && endereco_responsavel["cd_loc_cidade"] != null)
          {
            var cdCidade = endereco_responsavel["cd_loc_cidade"].ToString();
            if (!String.IsNullOrEmpty(cdCidade))
            {
              var filtroCidade = new List<(string campo, object valor)> { new("cd_localidade", cdCidade) };
              var cidadeExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtroCidade);
              if (cidadeExists != null && cidadeExists.ContainsKey("no_localidade"))
              {
                enderecoResponsavel += ", Cidade: " + (cidadeExists["no_localidade"]?.ToString() ?? "");
              }
            }
          }

          if (endereco_responsavel.ContainsKey("cd_loc_estado") && endereco_responsavel["cd_loc_estado"] != null)
          {
            var cdEstado = endereco_responsavel["cd_loc_estado"].ToString();
            if (!String.IsNullOrEmpty(cdEstado))
            {
              var filtroEstado = new List<(string campo, object valor)> { new("cd_localidade", cdEstado) };
              var estadoExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtroEstado);
              if (estadoExists != null && estadoExists.ContainsKey("no_localidade"))
              {
                enderecoResponsavel += " - " + (estadoExists["no_localidade"]?.ToString() ?? "");
              }
            }
          }
        }
        #endregion

        #region ALUNO
        var cd_aluno = matriculaExists["cd_aluno"];
        var aluno = await SQLServerService.GetFirstByFields(source, "T_ALUNO", new List<(string campo, object valor)> { new("cd_aluno", cd_aluno) });
        if (aluno == null) throw new BadRequestException("aluno não encontrado");
        var cd_pessoa_aluno = aluno["cd_pessoa_aluno"];
        var pessoa_aluno = await SQLServerService.GetFirstByFields(source, "T_PESSOA", new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa_aluno) });
        if (pessoa_aluno == null) throw new BadRequestException("pessoa aluno não encontrada");
        var nomeAluno = pessoa_aluno["no_pessoa"]?.ToString() ?? "";
        var telefoneAluno = "";
        var telefone_aluno = await SQLServerService.GetFirstByFields(source, "T_TELEFONE", new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa_aluno), new("cd_tipo_telefone", 1) });
        if (telefone_aluno != null)
        {
          telefoneAluno = telefone_aluno["dc_fone_mail"]?.ToString() ?? "";
        }

        var pessoa_aluno_fisica = await SQLServerService.GetFirstByFields(source, "T_PESSOA_FISICA", new List<(string campo, object valor)> { new("cd_pessoa_fisica", cd_pessoa_aluno) });
        var rg_pessoa_aluno = pessoa_aluno_fisica?["nm_doc_identidade"]?.ToString() ?? "";
        var cpfAluno = pessoa_aluno_fisica?["nm_cpf"]?.ToString() ?? "";

        var estadoCivilAluno = "";
        if (pessoa_aluno_fisica != null && pessoa_aluno_fisica.ContainsKey("cd_estado_civil") && pessoa_aluno_fisica["cd_estado_civil"] != null)
        {
          var estado_civil_aluno = await SQLServerService.GetFirstByFields(source, "T_ESTADO_CIVIL", new List<(string campo, object valor)> { new("cd_estado_civil", pessoa_aluno_fisica["cd_estado_civil"].ToString()) });
          if (estado_civil_aluno != null)
          {
            estadoCivilAluno = estado_civil_aluno["dc_estado_civil_masc"]?.ToString() ?? "";
          }
        }

        var sexoAluno = pessoa_aluno_fisica?["nm_sexo"];
        var sexoFAluno = sexoAluno?.ToString() == "1" ? "X" : "";
        var sexoMAluno = sexoAluno?.ToString() == "2" ? "X" : "";

        var dataNascimentoAluno = "";
        if (pessoa_aluno_fisica != null && pessoa_aluno_fisica.ContainsKey("dt_nascimento") && pessoa_aluno_fisica["dt_nascimento"] != null)
        {
          if (DateTime.TryParse(pessoa_aluno_fisica["dt_nascimento"].ToString(), out DateTime dt_nasc))
          {
            dataNascimentoAluno = dt_nasc.ToString("dd/MM/yyyy");
          }
        }

        var endereco_aluno = await SQLServerService.GetFirstByFields(source, "T_ENDERECO", new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa_aluno) });
        var enderecoAluno = "";
        if (endereco_aluno != null)
        {
          if (endereco_aluno.ContainsKey("cd_loc_logradouro") && endereco_aluno["cd_loc_logradouro"] != null)
          {
            var filtroLogradouro = new List<(string campo, object valor)> { new("cd_localidade", endereco_aluno["cd_loc_logradouro"].ToString()) };
            var logradouroExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtroLogradouro);
            if (logradouroExists != null && logradouroExists.ContainsKey("no_localidade"))
            {
              enderecoAluno = $"{logradouroExists["no_localidade"]?.ToString() ?? ""} ";
            }
          }

          var numEnderecoAluno = endereco_aluno["dc_num_endereco"]?.ToString() ?? "";
          if (!String.IsNullOrEmpty(numEnderecoAluno))
            enderecoAluno += " Nº " + numEnderecoAluno;

          var complEnderecoAluno = endereco_aluno["dc_compl_endereco"]?.ToString() ?? "";
          if (!String.IsNullOrEmpty(complEnderecoAluno))
            enderecoAluno += " / " + complEnderecoAluno;

          var cepAluno = endereco_aluno["dc_num_cep"]?.ToString() ?? "";
          if (!String.IsNullOrEmpty(cepAluno))
            enderecoAluno += ", CEP: " + cepAluno;

          if (endereco_aluno.ContainsKey("cd_loc_bairro") && endereco_aluno["cd_loc_bairro"] != null)
          {
            var cdBairroAluno = endereco_aluno["cd_loc_bairro"].ToString();
            if (!String.IsNullOrEmpty(cdBairroAluno))
            {
              var filtroBairro = new List<(string campo, object valor)> { new("cd_localidade", cdBairroAluno) };
              var bairroExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtroBairro);
              if (bairroExists != null && bairroExists.ContainsKey("no_localidade"))
              {
                enderecoAluno += ", Bairro: " + (bairroExists["no_localidade"]?.ToString() ?? "");
              }
            }
          }

          if (endereco_aluno.ContainsKey("cd_loc_cidade") && endereco_aluno["cd_loc_cidade"] != null)
          {
            var cdCidadeAluno = endereco_aluno["cd_loc_cidade"].ToString();
            if (!String.IsNullOrEmpty(cdCidadeAluno))
            {
              var filtroCidade = new List<(string campo, object valor)> { new("cd_localidade", cdCidadeAluno) };
              var cidadeExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtroCidade);
              if (cidadeExists != null && cidadeExists.ContainsKey("no_localidade"))
              {
                enderecoAluno += ", Cidade: " + (cidadeExists["no_localidade"]?.ToString() ?? "");
              }
            }
          }

          if (endereco_aluno.ContainsKey("cd_loc_estado") && endereco_aluno["cd_loc_estado"] != null)
          {
            var cdEstadoAluno = endereco_aluno["cd_loc_estado"].ToString();
            if (!String.IsNullOrEmpty(cdEstadoAluno))
            {
              var filtroEstado = new List<(string campo, object valor)> { new("cd_localidade", cdEstadoAluno) };
              var estadoExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtroEstado);
              if (estadoExists != null && estadoExists.ContainsKey("no_localidade"))
              {
                enderecoAluno += " - " + (estadoExists["no_localidade"]?.ToString() ?? "");
              }
            }
          }
        }

        var celularAluno = "";
        var celular_aluno = await SQLServerService.GetFirstByFields(source, "T_TELEFONE", new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa_aluno), new("cd_tipo_telefone", 3) });
        if (celular_aluno != null)
        {
          celularAluno = celular_aluno["dc_fone_mail"]?.ToString() ?? "";
        }
        var emailAluno = "";
        var email_aluno = await SQLServerService.GetFirstByFields(source, "T_TELEFONE", new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa_aluno), new("cd_tipo_telefone", 4) });
        if (email_aluno != null)
        {
          emailAluno = email_aluno["dc_fone_mail"]?.ToString() ?? "";
        }
        #endregion

        #region CONTRATO
        var nomeCurso = "";
        var curso = await SQLServerService.GetFirstByFields(source, "T_CURSO", new List<(string campo, object valor)> { new("cd_curso", matriculaExists["cd_curso_atual"]) });
        if (curso != null)
        {
          nomeCurso = curso["no_curso"]?.ToString() ?? "";
        }
        var duracaoAula = "";
        var duracao = await SQLServerService.GetFirstByFields(source, "T_DURACAO", new List<(string campo, object valor)> { new("cd_duracao", matriculaExists["cd_duracao_atual"]) });
        if (duracao != null)
        {
          duracaoAula = $"{duracao["dc_duracao"]?.ToString() ?? ""}/aula";
        }

        var diasSemana = new Dictionary<int, string>
    {
      {1, "domingo"},
      {2, "segunda"},
      {3, "terça"},
      {4, "quarta"},
      {5, "quinta"},
      {6, "sexta"},
      {7, "sábado"}
    };

        var diasMontado = "";
        var horarioMontado = "";

        List<Dictionary<string, object>> horariosData = new List<Dictionary<string, object>>();
        var filtroAlunoTurma = new List<(string campo, object valor)> { new("cd_aluno", cd_aluno) };
        var alunoTurma = await SQLServerService.GetFirstByFields(source, "T_ALUNO_TURMA", filtroAlunoTurma);
        var cdTurma = Convert.ToInt32(alunoTurma["cd_turma"]);
        if (alunoTurma != null && alunoTurma.ContainsKey("cd_turma") && alunoTurma["cd_turma"] != null)
        {
          var horariosResult = await SQLServerService.GetList(
                      "T_HORARIO",
                      null,
                      "[cd_registro],[id_origem]",
                      $"[{cdTurma}],[19]",  // 19 = origem TURMA
                      source,
                      SearchModeEnum.Equals
                  );

          if (horariosResult.success && horariosResult.data != null && horariosResult.data.Any())
          {
            horariosData = horariosResult.data;

            var diasList = horariosData
                .Where(h => h.ContainsKey("id_dia_semana") && h["id_dia_semana"] != null)
                .Select(h => diasSemana.TryGetValue(Convert.ToInt32(h["id_dia_semana"]), out var dia) ? dia : "")
                .Where(d => !string.IsNullOrEmpty(d))
                .Distinct();

            diasMontado = string.Join(", ", diasList);

            var horariosList = horariosData
                .Where(h => h.ContainsKey("dt_hora_ini") && h.ContainsKey("dt_hora_fim"))
                .Select(h => $"{h["dt_hora_ini"]?.ToString() ?? ""} às {h["dt_hora_fim"]?.ToString() ?? ""}")
                .Where(t => !string.IsNullOrEmpty(t))
                .Distinct();
            horarioMontado = string.Join(", ", horariosList);
          }
        }

        var dataInicioAula = "";
        if (matriculaExists.ContainsKey("dt_inicial_contrato") && matriculaExists["dt_inicial_contrato"] != null)
        {
          if (DateTime.TryParse(matriculaExists["dt_inicial_contrato"].ToString(), out DateTime dt_inicio))
          {
            dataInicioAula = dt_inicio.ToString("dd/MM/yyyy");
          }
        }
        var dataFimAula = "";
        if (matriculaExists.ContainsKey("dt_final_contrato") && matriculaExists["dt_final_contrato"] != null)
        {
          if (DateTime.TryParse(matriculaExists["dt_final_contrato"].ToString(), out DateTime dt_fim))
          {
            dataFimAula = dt_fim.ToString("dd/MM/yyyy");
          }
        }

        var matriculaRematricula = "";

        var taxaMatricula = await SQLServerService.GetFirstByFields(source, "T_TAXA_MATRICULA", new List<(string campo, object valor)> {
    new("cd_contrato", cdContrato)
});

        if (taxaMatricula != null && taxaMatricula.ContainsKey("vl_matricula_taxa"))  // ✅ CORRETO
        {
          matriculaRematricula = string.Format("{0:#,0.00}", taxaMatricula["vl_matricula_taxa"]);  // ✅ CORRETO
        }
        else if (matriculaExists["vl_matricula_contrato"] != null)
        {
          matriculaRematricula = string.Format("{0:#,0.00}", matriculaExists["vl_matricula_contrato"]);
        }



        decimal vlMaterialMatricula = 0;
        decimal vlSemDesconto = Convert.ToDecimal(matriculaExists["vl_curso_contrato"] ?? 0) / Convert.ToDecimal(matriculaExists["nm_parcelas_mensalidade"] ?? 1);
        byte nm_parcelas_material = 0;

        if (Convert.ToInt32(matriculaExists["nm_parcelas_material"] ?? 0) > 0)
        {
          nm_parcelas_material = (byte)Convert.ToInt32(matriculaExists["nm_parcelas_material"]);
          vlMaterialMatricula = Convert.ToDecimal(matriculaExists["vl_material_contrato"] ?? 0);
          if (nm_parcelas_material > 0)
            vlSemDesconto = vlSemDesconto + vlMaterialMatricula / nm_parcelas_material;
        }

        #region Valor Com Desconto
        nm_parcelas_material = 0;
        string valor_com_desconto = "";
        vlMaterialMatricula = 0;

        var titulosAbertos = await SQLServerService.GetList("T_TITULO", null, "[cd_origem_titulo],[id_status_titulo]", $"[{cdContrato}],[1]", source, SearchModeEnum.Equals);
        var statusCnabTitulo = new List<int> { 0, 1 };

        var descontosContrato_result = await SQLServerService.GetList(
    "T_DESCONTO_CONTRATO",
    null,
    "cd_contrato",
    cdContrato.ToString(),
    source
);
        var descontosContrato = descontosContrato_result.success && descontosContrato_result.data != null
            ? descontosContrato_result.data.Where(d => Convert.ToBoolean(d["id_desconto_ativo"] ?? false)).ToList()
            : new List<Dictionary<string, object>>();
        var aditamentos_result = await SQLServerService.GetList("T_ADITAMENTO", null, "[cd_contrato]", $"[{cdContrato}]", source, SearchModeEnum.Equals);
        var aditamentos = aditamentos_result.success ? aditamentos_result.data : new List<Dictionary<string, object>>();

        if (Convert.ToDecimal(matriculaExists["vl_parcela_contrato"] ?? 0) > 0)
        {
          decimal valorbaixaDesc = 0;

          if (!titulosAbertos.success || titulosAbertos.data == null || titulosAbertos.data.Count == 0)
          {
            var parametrosEscola = await BuscarParametrosEscola(Convert.ToInt32(matriculaExists["cd_pessoa_escola"]), source);
            if (parametrosEscola != null)
            {
              var tituloSimulado = new Dictionary<string, object>
          {
            {"cd_titulo", 0},
            {"vl_titulo", matriculaExists["vl_parcela_contrato"]},
            {"vl_saldo", matriculaExists["vl_parcela_contrato"]},
            {"dt_vencimento", DateTime.Now},
            {"vl_material", 0},
            {"cd_pessoa_empresa", matriculaExists["cd_pessoa_escola"]},
            {"no_aluno", "Simulação"}
          };

              var simulacaoBaixa = await _simulacaoBaixaService.SimularBaixaTitulo(tituloSimulado, DateTime.Now, parametrosEscola, source);
              valorbaixaDesc = simulacaoBaixa.vl_liquidacao_baixa;
              Console.WriteLine("Valor baixa com desconto simulado: " + simulacaoBaixa);
            }
            else
            {
              valorbaixaDesc = Convert.ToDecimal(matriculaExists["vl_parcela_contrato"]);
            }
          }
          else
          {
            var titulosAbertosLista = titulosAbertos.data;
            var aditamento = aditamentos.OrderByDescending(a => Convert.ToDateTime(a["dt_aditamento"])).FirstOrDefault();

            Dictionary<string, object> tituloParaCalculo = null;

            if (aditamento == null || aditamentos.Count <= 0)
            {
              tituloParaCalculo = titulosAbertosLista.Where(x =>
                  Convert.ToInt32(x["id_status_titulo"]) == 1 &&
                  statusCnabTitulo.Contains(Convert.ToInt32(x["id_status_cnab"] ?? 0)) &&
                  x["dc_tipo_titulo"]?.ToString() == "ME"
              ).OrderBy(x => Convert.ToInt32(x["nm_parcela_titulo"] ?? 0)).FirstOrDefault();
            }
            else
            {
              var tipoAditamento = Convert.ToByte(aditamento["id_tipo_aditamento"] ?? 0);

              if (tipoAditamento != 3)
              {
                tituloParaCalculo = titulosAbertosLista.Where(x =>
                    Convert.ToInt32(x["id_status_titulo"]) == 1 &&
                    statusCnabTitulo.Contains(Convert.ToInt32(x["id_status_cnab"] ?? 0)) &&
                    x["dc_tipo_titulo"]?.ToString() == "ME"
                ).OrderBy(x => Convert.ToInt32(x["nm_parcela_titulo"] ?? 0)).FirstOrDefault();
              }
              else
              {
                tituloParaCalculo = titulosAbertosLista.Where(x =>
                    Convert.ToInt32(x["id_status_titulo"]) == 1 &&
                    statusCnabTitulo.Contains(Convert.ToInt32(x["id_status_cnab"] ?? 0)) &&
                    Convert.ToDecimal(x["vl_titulo"]) == Convert.ToDecimal(x["vl_saldo_titulo"] ?? x["vl_titulo"]) &&
                    (x["dc_tipo_titulo"]?.ToString() == "AD" || x["dc_tipo_titulo"]?.ToString() == "AA")
                ).OrderBy(x => Convert.ToInt32(x["nm_parcela_titulo"] ?? 0)).FirstOrDefault();
              }
            }

            if (tituloParaCalculo != null)
            {
              var parametrosEscola = await BuscarParametrosEscola(Convert.ToInt32(matriculaExists["cd_pessoa_escola"]), source);
              if (parametrosEscola != null)
              {
                var simulacaoBaixa = await _simulacaoBaixaService.SimularBaixaTitulo(tituloParaCalculo, DateTime.Now, parametrosEscola, source);
                valorbaixaDesc = simulacaoBaixa.vl_liquidacao_baixa;
                Console.WriteLine("Valor baixa com desconto simulado3: " + simulacaoBaixa);
              }
              else
              {
                valorbaixaDesc = Convert.ToDecimal(tituloParaCalculo["vl_saldo_titulo"] ?? tituloParaCalculo["vl_titulo"]);
              }
            }
            else
            {
              valorbaixaDesc = Convert.ToDecimal(matriculaExists["vl_parcela_contrato"]);
            }
          }

          valor_com_desconto = string.Format("{0:#,0.00}", decimal.Round(valorbaixaDesc, 2));
        }
        else
        {
          valor_com_desconto = "0,00";
        }
        #endregion

        var aditamentos_final = new List<Dictionary<string, object>>();
        var aditamentos_result_final = await SQLServerService.GetList("T_ADITAMENTO", null, "[cd_contrato]", $"[{cdContrato}]", source, SearchModeEnum.Equals);
        if (aditamentos_result_final.success)
        {
          aditamentos_final = aditamentos_result_final.data;
        }

        byte? dtaVctoAditamento = null;
        if (aditamentos_final.Count > 0)
        {
          var ultimoAditamento = aditamentos_final.OrderBy(a => Convert.ToDateTime(a["dt_aditamento"])).Last();
          if (ultimoAditamento.ContainsKey("nm_dia_vcto_desconto") && ultimoAditamento["nm_dia_vcto_desconto"] != null)
          {
            dtaVctoAditamento = Convert.ToByte(ultimoAditamento["nm_dia_vcto_desconto"]);
          }
        }
        string nroVencimentoComDesconto = !dtaVctoAditamento.HasValue ? "" : dtaVctoAditamento.ToString();
        #endregion

        var nomeProduto = "";
        var produto = await SQLServerService.GetFirstByFields(source, "T_PRODUTO", new List<(string campo, object valor)> { new("cd_produto", matriculaExists["cd_produto_atual"]) });
        if (produto != null)
        {
          nomeProduto = produto["no_produto"]?.ToString() ?? "";
        }

        List<string> listaProdutos = new List<string>();
        listaProdutos.Add("Inglês");
        listaProdutos.Add("Espanhol");
        var tituloCurso = listaProdutos.Contains(nomeProduto) ? "Estágio" : "Módulo";

        string complemento = "";
        if (nomeCurso.Contains("R60"))
          complemento += "TURMA DE 60 MINUTOS";

        var tipoFinanceiro = await SQLServerService.GetFirstByFields(source, "T_TIPO_FINANCEIRO", new List<(string campo, object valor)> { new("cd_tipo_financeiro", matriculaExists["cd_tipo_financeiro"]) });

        var aditamento_nmPrevisaoInicial = aditamentos.OrderByDescending(a => Convert.ToDateTime(a["dt_aditamento"])).FirstOrDefault();

        string desc_data_aditamento = "";
        string dataInicioAdtExtenso = "";
        if (aditamento_nmPrevisaoInicial != null)
        {
          desc_data_aditamento = aditamento_nmPrevisaoInicial["id_tipo_data_inicio"]?.ToString() switch
          {
            "1" => "Até 30 dias",
            "2" => "Até 60 dias",
            "3" => "Até 90 dias",
            "4" => FormatarData(aditamento_nmPrevisaoInicial["dt_inicio_aditamento"]),
            _ => ""
          };

          dataInicioAdtExtenso = FormatarData(aditamento_nmPrevisaoInicial["dt_inicio_aditamento"]);
        }

        string nroParcelas = "";
        if (aditamento_nmPrevisaoInicial == null || aditamentos.Count <= 0)
          nroParcelas = matriculaExists["nm_parcelas_mensalidade"]?.ToString() ?? "";
        else
        {
          int parcelasMensalidade = int.Parse(matriculaExists["nm_parcelas_mensalidade"]?.ToString() ?? "0");
          int titulosAditamento = int.Parse(aditamento_nmPrevisaoInicial["nm_titulos_aditamento"]?.ToString() ?? "0");
          nroParcelas = (parcelasMensalidade + titulosAditamento).ToString();
        }

        var datacorrenteextenso = DateTime.Now.ToString("dd 'de' MMMM 'de' yyyy", new System.Globalization.CultureInfo("pt-BR"));

        var regime = await SQLServerService.GetFirstByFields(source, "T_REGIME", new List<(string campo, object valor)> { new("cd_regime", matriculaExists["cd_regime_atual"]) });

        string vencimentosTitulosComDesc = "";
        decimal vlDescontoContrato = Convert.ToDecimal(matriculaExists["vl_desconto_contrato"] ?? 0);
        if (vlDescontoContrato > 0)
        {
          var ultimoAditamento = aditamentos?.OrderBy(a => a["dt_aditamento"]).LastOrDefault();

          if (ultimoAditamento == null || !aditamentos.Any())
          {
            var titulosParaConcatenar = titulosAbertos.success ? titulosAbertos.data
                .Where(x => Convert.ToInt32(x["id_status_titulo"]) == 1 &&
                           statusCnabTitulo.Contains(Convert.ToInt32(x["id_status_cnab"] ?? 0)))
                .OrderBy(x => Convert.ToDateTime(x["dt_vcto_titulo"]))
                .ToList() : new List<Dictionary<string, object>>();

            vencimentosTitulosComDesc = ConcatenarVencimentosTitulos(titulosParaConcatenar);
          }
          else
          {
            var titulosEmAbertoMatricula = new List<Dictionary<string, object>>();

            int? tipoAditamento = ultimoAditamento?["id_tipo_aditamento"] as int?;
            if (tipoAditamento.HasValue && tipoAditamento.Value != 4)
            {
              titulosEmAbertoMatricula = titulosAbertos.success ? titulosAbertos.data
                  .Where(x => Convert.ToInt32(x["id_status_titulo"]) == 1 &&
                             statusCnabTitulo.Contains(Convert.ToInt32(x["id_status_cnab"] ?? 0)) &&
                             Convert.ToDecimal(x["vl_titulo"]) == Convert.ToDecimal(x["vl_saldo_titulo"]) &&
                             x["dc_tipo_titulo"]?.ToString() != "TM" &&
                             x["dc_tipo_titulo"]?.ToString() != "TA" &&
                             x["dc_tipo_titulo"]?.ToString() != "AD" &&
                             x["dc_tipo_titulo"]?.ToString() != "AA")
                  .OrderBy(x => Convert.ToDateTime(x["dt_vcto_titulo"]))
                  .ToList() : new List<Dictionary<string, object>>();
            }
            else
            {
              titulosEmAbertoMatricula = titulosAbertos.success ? titulosAbertos.data
                  .Where(x => (x["dc_tipo_titulo"]?.ToString() == "AA" || x["dc_tipo_titulo"]?.ToString() == "AD") &&
                             Convert.ToInt32(x["id_status_titulo"]) == 1 &&
                             statusCnabTitulo.Contains(Convert.ToInt32(x["id_status_cnab"] ?? 0)) &&
                             Convert.ToDecimal(x["vl_titulo"]) == Convert.ToDecimal(x["vl_saldo_titulo"]))
                  .OrderBy(x => Convert.ToDateTime(x["dt_vcto_titulo"]))
                  .ToList() : new List<Dictionary<string, object>>();
            }

            vencimentosTitulosComDesc = ConcatenarVencimentosTitulos(titulosEmAbertoMatricula);
          }
        }

        string vencimentosTitulosSemDesc = "";
        decimal vlDescontoContratoSemDesc = Convert.ToDecimal(matriculaExists["vl_desconto_contrato"] ?? 0);
        if (vlDescontoContratoSemDesc <= 0)
        {
          var ultimoAditamentoSemDesc = aditamentos?.OrderBy(a => a["dt_aditamento"]).LastOrDefault();

          if (ultimoAditamentoSemDesc == null || !aditamentos.Any())
          {
            var titulosParaConcatenarSemDesc = titulosAbertos.success ? titulosAbertos.data
                .Where(x => Convert.ToInt32(x["id_status_titulo"]) == 1 &&
                           statusCnabTitulo.Contains(Convert.ToInt32(x["id_status_cnab"] ?? 0)))
                .OrderBy(x => Convert.ToDateTime(x["dt_vcto_titulo"]))
                .ToList() : new List<Dictionary<string, object>>();

            vencimentosTitulosSemDesc = ConcatenarVencimentosTitulos(titulosParaConcatenarSemDesc);
          }
          else
          {
            var titulosEmAbertoMatriculaSemDesc = new List<Dictionary<string, object>>();

            int? tipoAditamentoSemDesc = ultimoAditamentoSemDesc?["id_tipo_aditamento"] as int?;
            if (tipoAditamentoSemDesc.HasValue && tipoAditamentoSemDesc.Value != 4)
            {
              titulosEmAbertoMatriculaSemDesc = titulosAbertos.success ? titulosAbertos.data
                  .Where(x => Convert.ToInt32(x["id_status_titulo"]) == 1 &&
                             statusCnabTitulo.Contains(Convert.ToInt32(x["id_status_cnab"] ?? 0)) &&
                             Convert.ToDecimal(x["vl_titulo"]) == Convert.ToDecimal(x["vl_saldo_titulo"]) &&
                             x["dc_tipo_titulo"]?.ToString() != "TM" &&
                             x["dc_tipo_titulo"]?.ToString() != "TA" &&
                             x["dc_tipo_titulo"]?.ToString() != "AD" &&
                             x["dc_tipo_titulo"]?.ToString() != "AA")
                  .OrderBy(x => Convert.ToDateTime(x["dt_vcto_titulo"]))
                  .ToList() : new List<Dictionary<string, object>>();
            }
            else
            {
              titulosEmAbertoMatriculaSemDesc = titulosAbertos.success ? titulosAbertos.data
                  .Where(x => (x["dc_tipo_titulo"]?.ToString() == "AA" || x["dc_tipo_titulo"]?.ToString() == "AD") &&
                             Convert.ToInt32(x["id_status_titulo"]) == 1 &&
                             statusCnabTitulo.Contains(Convert.ToInt32(x["id_status_cnab"] ?? 0)) &&
                             Convert.ToDecimal(x["vl_titulo"]) == Convert.ToDecimal(x["vl_saldo_titulo"]))
                  .OrderBy(x => Convert.ToDateTime(x["dt_vcto_titulo"]))
                  .ToList() : new List<Dictionary<string, object>>();
            }

            vencimentosTitulosSemDesc = ConcatenarVencimentosTitulos(titulosEmAbertoMatriculaSemDesc);
          }
        }

        string diasHorariosCurso = "";
        if (horariosData != null && horariosData.Any())
        {
          diasHorariosCurso = GerarDescricaoCompletaHorarios(horariosData);
        }

        var dataFimContrato = FormatarData(matriculaExists["dt_final_contrato"]);
        var dataMatriculaContrato = FormatarData(matriculaExists["dt_matricula_contrato"]);

        var tipoMatriculaTexto = "";
        var idTipoMatricula = matriculaExists["id_tipo_matricula"];
        if (idTipoMatricula != null)
        {
          int tipoId = Convert.ToInt32(idTipoMatricula);
          tipoMatriculaTexto = tipoId switch
          {
            1 => "Matrícula",
            _ => "Rematrícula"
          };
        }

        decimal parcelaLiquida = await ObterValorParcelaLiquida(source, cdContrato);
        Console.WriteLine($"Valor final líquido da parcela: R$ {parcelaLiquida:N2}");


        var replacements = new Dictionary<string, string>
    {
      { "«NomeEscola»", nomeEscola },
      { "«RazaoSocial»", razaoSocialEscola },
      { "«CNPJEscola»", cnpjEscola },
      { "«EnderecoEscola»", enderecoEscolaMontado },
      { "«CidadeEstadoEscola»", cidadeEstadoEscola },
      { "«NomeResponsavel»", nomeResponsavel },
      { "«RGResponsavel»", rg_pessoa_responsavel },
      { "«CPFCNPJResponsavel»", cpfResponsavel },
      { "«TituloRGResponsavel»", tituloRGResponsavel },
      { "«TituloCPFouCNPJResponsavel»", tituloCPFouCNPJResponsavel },
      { "«TelefoneResponsavel»", telefoneResponsavel },
      { "«EnderecoResponsavel»", enderecoResponsavel },
      { "«EmailResponsavel»", emailResponsavel },
      { "«CelularResponsavel»", celularResponsavel },
      { "«DataNascResponsavel»", dataNascResponsavel },
      { "«NomeAluno»", nomeAluno },
      { "«TelelfoneAluno»", telefoneAluno },
      { "«TelefoneAluno»", telefoneAluno },
      { "«RGAluno»", rg_pessoa_aluno },
      { "«CPFAluno»", cpfAluno },
      { "«EstadoCivilAluno»", estadoCivilAluno },
      { "«DataNascimentoAluno»", dataNascimentoAluno },
      { "«EnderecoAluno»", enderecoAluno },
      { "«EmailAluno»", emailAluno },
      { "«CelularAluno»", celularAluno },
      { "«SexoF»", sexoFAluno },
      { "«SexoM»", sexoMAluno },
      { "«TituloCurso»", tituloCurso },
      { "«Curso»", nomeCurso },
      { "«Produto»", nomeProduto },
      { "«NomeProduto»", nomeProduto },
      { "«ComplementoCursoComMinutosTurma»", complemento },
      { "«DiasHorariosCurso»", diasHorariosCurso },
      { "«HorariosCurso»", diasHorariosCurso },
      { "«Dias»", diasMontado },
      { "«Horarios»", horarioMontado },
      { "«DuracaoAula»", duracaoAula },
      { "«DataInicioAulas»", dataInicioAula },
      { "«DataFimTurma»", dataFimAula },
      { "«DataFimContrato»", dataFimContrato },
      { "«DataMatriculaContrato»", dataMatriculaContrato },
      { "«DataInicioAdt»", desc_data_aditamento },
      { "«DataInicioAdtExtenso»", dataInicioAdtExtenso },
      { "«AnoCorrente»", DateTime.Now.Year.ToString() },
      { "«DataCorrenteExtenso»", datacorrenteextenso },
      { "«MatriculaRematricula»", $"R$ {matriculaRematricula}" },
      { "«ValorSemDesconto»", $"R$ {decimal.Round(vlSemDesconto, 2).ToString("N2")}" },
      { "«ValorComDesconto»", $"R$ {valor_com_desconto}" },
      { "«ValorCurso»", $"R$ {decimal.Parse(matriculaExists["vl_curso_contrato"]?.ToString() ?? "0").ToString("N2")}" },
      { "«ValorMaterial»", $"R$ {decimal.Parse(matriculaExists["vl_material_contrato"]?.ToString() ?? "0").ToString("N2")}" },
      { "«ValorComDescontoMaterial»", $"R$ {decimal.Parse(matriculaExists["vl_parcela_liq_material"]?.ToString() ?? "0").ToString("N2")}" },
     { "«ParcelaLiquida»", $"R$ {parcelaLiquida.ToString("N2")}" },
      { "«NroParcelas»", matriculaExists["nm_parcelas_mensalidade"]?.ToString() ?? "" },
      { "«NroParcelasTotal»", nroParcelas },
      { "«NroParcelasCurso»", matriculaExists["nm_parcelas_mensalidade"]?.ToString() ?? "" },
      { "«NroParcelasMaterial»", matriculaExists["nm_parcelas_material"]?.ToString() ?? "" },
      { "«NroVencimento»", matriculaExists["nm_dia_vcto"]?.ToString() ?? "" },
      { "«NroVencimentoComDesconto»", nroVencimentoComDesconto },
      { "«VencimentosTitulosComDesc»", vencimentosTitulosComDesc },
      { "«VencimentosTitulosSemDesc»", vencimentosTitulosSemDesc },
{ "«TipoAditamento»", aditamento_nmPrevisaoInicial != null && aditamento_nmPrevisaoInicial.ContainsKey("id_tipo_aditamento") && aditamento_nmPrevisaoInicial["id_tipo_aditamento"] != null ? aditamento_nmPrevisaoInicial["id_tipo_aditamento"].ToString() : "" },
      { "«NroPrevisaoDias»", aditamento_nmPrevisaoInicial?["nm_previsao_inicial"]?.ToString() ?? "" },
      { "«Observacao»", aditamento_nmPrevisaoInicial?["tx_obs_aditamento"]?.ToString() ?? "" },
      { "«NumeroContrato»", matriculaExists["nm_contrato"]?.ToString() ?? "" },
      { "«OpcoesPagamento»", tipoFinanceiro?["dc_tipo_financeiro"]?.ToString() ?? "" },
      { "«TipoFinanceiroTaxa»", tipoFinanceiro?["dc_tipo_financeiro"]?.ToString() ?? "" },
      { "«TipoMatricula»", tipoMatriculaTexto },
      { "«Modalidade»", regime?["no_regime"]?.ToString() ?? "" },
      { "«BolsaMaterial»", Convert.ToDecimal(matriculaExists["pc_bolsa_material"] ?? 0).ToString("0.##") + "%" },

    };

        var (success, arquivo, erro) = GerarContrato(nomeContrato, replacements, cdPessoaEscola);

        if (!success)
        {
          throw new Exception(erro);
        }

        // Buscar dados para as grades
        var cursosContrato = await ObterCursosDoContrato(source, cdContrato);
        var descontosAntecipacao = await ObterDescontosAntecipacao(source, cdContrato, cd_pessoa_escola);
        var parcelasTitulos = await ObterTitulosContrato(source, cdContrato, cd_pessoa_escola);


        Console.WriteLine("[DESCONTOS ANTECIPAÇÃO]", descontosAntecipacao);
        // Criar um novo MemoryStream que não será fechado automaticamente
        var novoArquivo = new MemoryStream();

        // Copiar o arquivo original para o novo stream
        arquivo.Position = 0;
        await arquivo.CopyToAsync(novoArquivo);
        novoArquivo.Position = 0;

        // Abrir documento e preencher grades
        using (var doc = WordprocessingDocument.Open(novoArquivo, true))
        {
          try
          {


            var descontosContratoCalculado = await ObterDescontosContrato(source, cdContrato, cd_pessoa_escola);
            PreencherGradeDescontosContrato(doc, descontosContratoCalculado);

            // Valores líquidos
            var parcelasComDesconto = await CalcularValoresLiquidos(source, parcelasTitulos, cdContrato, cd_pessoa_escola);
            PreencherGradeValoresLiquidos(doc, parcelasComDesconto);
            // Preencher grade de cursos
            PreencherGradeCursos(doc, cursosContrato);

            // Preencher grade de descontos
            PreencherGradeDescontosAntecipacao(doc, descontosAntecipacao);

            // Preencher grade de valores das parcelas (consolidando ME e MT por vencimento)
            if (parcelasTitulos != null && parcelasTitulos.Any())
            {
              var parcelasConsolidadas = ConsolidarTitulosPorVencimento(parcelasTitulos);
              PreencherGradeValoresParcelas(doc, parcelasConsolidadas);
            }

          }
          catch (Exception ex)
          {
            Console.WriteLine("[PreencherGradesErro]: " + ex);
          }

          try
          {
            ContratoFormatacao.AplicarFormatacaoPadrao(doc);
            doc.MainDocumentPart.Document.Save();
          }
          catch (Exception ex)
          {
            Console.WriteLine("[FormatarFinalErro]: " + ex);
          }
        }

        // Resetar posição para o início
        novoArquivo.Position = 0;

        return (novoArquivo, nomeContrato);
      }
      catch (Exception ex)
      {
        Console.WriteLine("[GerarContratoMatriculaError]: " + ex);
        throw;
      }
    }

    /// <summary>
    /// Calcula o valor líquido da parcela (mensalidade), aplicando todos os descontos possíveis:
    /// - Desconto base (pc_desconto_contrato)
    /// - Bolsa (pc_desconto_bolsa)
    /// - Descontos adicionais ativos da tabela T_DESCONTO_CONTRATO
    /// </summary>
    private async Task<decimal> ObterValorParcelaLiquida(Source source, int cdContrato)
    {
      try
      {
        Console.WriteLine($"[ObterValorParcelaLiquida] Iniciando cálculo para contrato {cdContrato}");

        // 🔹 1. Buscar contrato completo
        var matriculaExists = await SQLServerService.GetFirstByFields(
            source,
            "T_CONTRATO",
            new List<(string campo, object valor)> { new("cd_contrato", cdContrato) }
        );

        if (matriculaExists == null)
        {
          Console.WriteLine($"[ObterValorParcelaLiquida] Contrato {cdContrato} não encontrado.");
          return 0;
        }

        // 🔹 2. Tenta pegar o valor direto
        decimal parcelaLiquida = Convert.ToDecimal(matriculaExists["vl_parcela_liquida"] ?? 0);

        // 🔹 3. Se estiver zerado, calcular manualmente
        if (parcelaLiquida == 0)
        {
          decimal vlParcela = Convert.ToDecimal(matriculaExists["vl_parcela_contrato"] ?? 0);
          decimal pcDesconto = Convert.ToDecimal(matriculaExists["pc_desconto_contrato"] ?? 0);
          decimal pcBolsa = Convert.ToDecimal(matriculaExists["pc_desconto_bolsa"] ?? 0);

          // 🔹 Aplicar descontos base
          decimal vlDesconto = (vlParcela * pcDesconto) / 100;
          decimal vlBolsa = (vlParcela * pcBolsa) / 100;

          parcelaLiquida = vlParcela - vlDesconto - vlBolsa;

          Console.WriteLine($"[ObterValorParcelaLiquida] Base: {vlParcela:N2}, Desconto: {vlDesconto:N2}, Bolsa: {vlBolsa:N2} => Parcela líquida parcial: {parcelaLiquida:N2}");

          // 🔹 4. Aplicar descontos adicionais ativos de T_DESCONTO_CONTRATO
          var descontosContratoMensalidade = await SQLServerService.GetList(
              "T_DESCONTO_CONTRATO",
              null,
              null,
              null,
              false,
              null,
              "cd_contrato,id_desconto_ativo,id_incide_matricula",
              $"[{cdContrato}],[1],[0]", // Apenas descontos ativos que NÃO incidem sobre matrícula (mensalidade)
              source,
              SearchModeEnum.Equals,
              null,
              null
          );

          if (descontosContratoMensalidade.success &&
              descontosContratoMensalidade.data != null &&
              descontosContratoMensalidade.data.Any())
          {
            foreach (var desconto in descontosContratoMensalidade.data)
            {
              decimal pcDescontoAdicional = Convert.ToDecimal(desconto["pc_desconto_contrato"] ?? 0);
              decimal vlDescontoAdicional = Convert.ToDecimal(desconto["vl_desconto_contrato"] ?? 0);

              if (pcDescontoAdicional > 0)
              {
                decimal valorDescontoPerc = (parcelaLiquida * pcDescontoAdicional) / 100;
                parcelaLiquida -= valorDescontoPerc;
                Console.WriteLine($"[ObterValorParcelaLiquida] Desconto adicional {pcDescontoAdicional:N2}% aplicado => -R$ {valorDescontoPerc:N2}");
              }
              else if (vlDescontoAdicional > 0)
              {
                parcelaLiquida -= vlDescontoAdicional;
                Console.WriteLine($"[ObterValorParcelaLiquida] Desconto adicional fixo -R$ {vlDescontoAdicional:N2} aplicado");
              }
            }
          }

          Console.WriteLine($"[ObterValorParcelaLiquida] Valor final líquido calculado: R$ {parcelaLiquida:N2}");
        }
        else
        {
          Console.WriteLine($"[ObterValorParcelaLiquida] Valor líquido já definido no contrato: R$ {parcelaLiquida:N2}");
        }

        return parcelaLiquida;
      }
      catch (Exception ex)
      {
        Console.WriteLine($"[ObterValorParcelaLiquida] ERRO: {ex.Message}");
        Console.WriteLine($"[ObterValorParcelaLiquida] StackTrace: {ex.StackTrace}");
        return 0;
      }
    }

    private async Task<List<Dictionary<string, object>>> ObterTitulosContrato(Source source, int cdContrato, int cdPessoaEscola)
    {
      try
      {
        var titulos = await SQLServerService.GetList(
          "T_TITULO",
          null,
          null,
          "nm_parcela_titulo",
          false,
          null,
          "[cd_origem_titulo],[id_origem_titulo],[cd_pessoa_empresa]",
          $"[{cdContrato}],[22],[{cdPessoaEscola}]",
          source,
          SearchModeEnum.Equals,
          null,
          null
        );

        return titulos.success && titulos.data != null ? titulos.data : new List<Dictionary<string, object>>();
      }
      catch { return new List<Dictionary<string, object>>(); }
    }

    /// <summary>
    /// Consolida títulos de mensalidade (ME) e material (MT) agrupando por data de vencimento.
    /// Isso replica a lógica do sistema antigo onde cada linha da tabela representa um vencimento
    /// com os valores de material e mensalidade separados.
    /// </summary>
    private static List<Dictionary<string, object>> ConsolidarTitulosPorVencimento(List<Dictionary<string, object>> titulos)
    {
      var resultado = new List<Dictionary<string, object>>();

      // Agrupa títulos por data de vencimento
      var titulosPorVencimento = titulos
          .OrderBy(t => Convert.ToDateTime(t["dt_vcto_titulo"]))
          .ThenBy(t => Convert.ToInt32(t["nm_parcela_titulo"] ?? t["nm_parcela"] ?? 0))
          .GroupBy(t => Convert.ToDateTime(t["dt_vcto_titulo"]).Date);

      foreach (var grupo in titulosPorVencimento)
      {
        DateTime dtVcto = grupo.Key;

        // Separa valores de material e mensalidade
        decimal vlMaterial = grupo
            .Where(t => t["dc_tipo_titulo"]?.ToString() == "MT")
            .Sum(t => Convert.ToDecimal(t["vl_titulo"] ?? 0));

        decimal vlMensalidade = grupo
            .Where(t => t["dc_tipo_titulo"]?.ToString() == "ME")
            .Sum(t => Convert.ToDecimal(t["vl_titulo"] ?? 0));

        // Pega o número da parcela (pode vir como nm_parcela_titulo ou nm_parcela)
        var primeiroParcela = grupo.FirstOrDefault();
        int nmParcela = 0;
        if (primeiroParcela != null)
        {
          if (primeiroParcela.ContainsKey("nm_parcela_titulo") && primeiroParcela["nm_parcela_titulo"] != null)
            nmParcela = Convert.ToInt32(primeiroParcela["nm_parcela_titulo"]);
          else if (primeiroParcela.ContainsKey("nm_parcela") && primeiroParcela["nm_parcela"] != null)
            nmParcela = Convert.ToInt32(primeiroParcela["nm_parcela"]);
        }

        // Cria um dicionário consolidado para este vencimento
        var parcelaConsolidada = new Dictionary<string, object>
        {
            { "dt_vcto_titulo", dtVcto },
            { "nm_parcela", nmParcela },
            { "vl_material", vlMaterial },
            { "vl_mensalidade", vlMensalidade },
            { "vl_total", vlMaterial + vlMensalidade },
            { "dc_tipo_titulo", "CONSOLIDADO" } // Marca como consolidado
        };

        resultado.Add(parcelaConsolidada);
      }

      return resultado;
    }

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    private (bool success, MemoryStream? arquivo, string? erro) GerarContrato(string nomeContrato, Dictionary<string, string> replacements, int? cd_pessoa_escola = null)
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    {
      try
      {
        // Validações iniciais
        if (string.IsNullOrWhiteSpace(nomeContrato))
        {
          return (false, null, "Nome do contrato não pode ser nulo ou vazio.");
        }

        if (replacements == null)
        {
          return (false, null, "Dicionário de substituições não pode ser nulo.");
        }

        // Monta o caminho do template
        string webRootPath = _webHostEnvironment.WebRootPath;

        if (string.IsNullOrWhiteSpace(webRootPath))
        {
          return (false, null, "WebRootPath não está configurado.");
        }

        string caminhoPastaBase = Path.Combine(webRootPath, "Contratos");

        // Verifica se a pasta existe, se não existir, cria
        if (!Directory.Exists(caminhoPastaBase))
        {
          try
          {
            Directory.CreateDirectory(caminhoPastaBase);
            Console.WriteLine($"[INFO] Pasta de contratos criada: {caminhoPastaBase}");
          }
          catch (Exception ex)
          {
            return (false, null, $"Erro ao criar pasta de contratos: {ex.Message}");
          }
        }

        string caminhoArquivo = null;

        // ESTRATÉGIA 1: Buscar na pasta específica da escola (se informado)
        if (cd_pessoa_escola.HasValue && cd_pessoa_escola.Value > 0)
        {
          string caminhoPastaEscola = Path.Combine(caminhoPastaBase, cd_pessoa_escola.Value.ToString());

          if (Directory.Exists(caminhoPastaEscola))
          {
            Console.WriteLine($"[INFO] Buscando template na pasta da escola: {caminhoPastaEscola}");
            caminhoArquivo = BuscarArquivoRecursivamente(caminhoPastaEscola, nomeContrato);

            if (!string.IsNullOrEmpty(caminhoArquivo))
            {
              Console.WriteLine($"[SUCESSO] Template encontrado na pasta da escola!");
            }
          }
          else
          {
            Console.WriteLine($"[AVISO] Pasta da escola não existe: {caminhoPastaEscola}");
          }
        }

        // ESTRATÉGIA 2: Se não encontrou na pasta da escola, busca recursivamente em toda pasta Contratos
        if (string.IsNullOrEmpty(caminhoArquivo))
        {
          Console.WriteLine($"[INFO] Buscando template '{nomeContrato}' recursivamente em todas as pastas...");
          caminhoArquivo = BuscarArquivoRecursivamente(caminhoPastaBase, nomeContrato);
        }

        // ESTRATÉGIA 3: Se não encontrou, tenta com o template padrão na pasta da escola
        if (string.IsNullOrEmpty(caminhoArquivo) && cd_pessoa_escola.HasValue && cd_pessoa_escola.Value > 0)
        {
          string caminhoPastaEscola = Path.Combine(caminhoPastaBase, cd_pessoa_escola.Value.ToString());

          if (Directory.Exists(caminhoPastaEscola))
          {
            Console.WriteLine($"[AVISO] Template '{nomeContrato}' não encontrado. Tentando 'Contrato_Padrao' na pasta da escola...");
            caminhoArquivo = BuscarArquivoRecursivamente(caminhoPastaEscola, "Contrato_Padrao");
          }
        }

        // ESTRATÉGIA 4: Se ainda não encontrou, tenta template padrão em toda a estrutura
        if (string.IsNullOrEmpty(caminhoArquivo))
        {
          Console.WriteLine($"[AVISO] Template '{nomeContrato}' não encontrado. Tentando 'Contrato_Padrao' globalmente...");
          caminhoArquivo = BuscarArquivoRecursivamente(caminhoPastaBase, "Contrato_Padrao");
        }

        // Se ainda não encontrou, retorna erro
        if (string.IsNullOrEmpty(caminhoArquivo))
        {
          string mensagemErro = cd_pessoa_escola.HasValue
            ? $"Nenhum template encontrado para escola {cd_pessoa_escola.Value}. Tentou: '{nomeContrato}' e 'Contrato_Padrao'"
            : $"Nenhum template encontrado. Tentou: '{nomeContrato}' e 'Contrato_Padrao'";

          return (false, null, mensagemErro);
        }

        Console.WriteLine($"[INFO] Carregando template: {caminhoArquivo}");

        // Carrega o template (DOTX ou DOCX)
        using (var doc = DocX.Load(caminhoArquivo))
        {
          // Faz os replaces
          foreach (var campo in replacements)
          {
            if (!string.IsNullOrEmpty(campo.Key))
            {
              doc.ReplaceText(campo.Key, campo.Value ?? string.Empty);
            }
          }

          // Retorna como MemoryStream (sempre em DOCX)
          var memoryStream = new MemoryStream();
          doc.SaveAs(memoryStream);
          memoryStream.Position = 0;

          Console.WriteLine($"[SUCESSO] Contrato gerado. Tamanho: {memoryStream.Length} bytes");

          return (true, memoryStream, null);
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"[GerarContratoError]: {ex.Message}");
        Console.WriteLine($"[StackTrace]: {ex.StackTrace}");
        return (false, null, $"Erro ao processar template: {ex.Message}");
      }
    }


    /// <summary>
    /// Busca recursivamente por um arquivo de template (.docx ou .dotx) em uma pasta e suas subpastas
    /// </summary>
    /// <param name="caminhoPastaBase">Pasta base para iniciar a busca</param>
    /// <param name="nomeArquivo">Nome do arquivo (com ou sem extensão)</param>
    /// <returns>Caminho completo do arquivo encontrado ou null se não encontrar</returns>
    private string BuscarArquivoRecursivamente(string caminhoPastaBase, string nomeArquivo)
    {
      try
      {
        if (string.IsNullOrWhiteSpace(nomeArquivo) || !Directory.Exists(caminhoPastaBase))
        {
          return null;
        }

        // Remove extensão se tiver
        string nomeArquivoSemExtensao = Path.GetFileNameWithoutExtension(nomeArquivo);

        // Lista de extensões para tentar
        string[] extensoes = { ".dotx", ".docx" };

        // Buscar em todas as subpastas recursivamente
        foreach (string extensao in extensoes)
        {
          // Pattern de busca (case-insensitive através do SearchOption)
          string pattern = $"{nomeArquivoSemExtensao}{extensao}";

          try
          {
            // Busca recursiva em todas as subpastas
            var arquivosEncontrados = Directory.GetFiles(
              caminhoPastaBase,
              pattern,
              SearchOption.AllDirectories
            );

            // Se encontrou algum arquivo, retorna o primeiro
            if (arquivosEncontrados != null && arquivosEncontrados.Length > 0)
            {
              // Prioriza .dotx sobre .docx se houver ambos
              var arquivoDotx = arquivosEncontrados.FirstOrDefault(f => f.EndsWith(".dotx", StringComparison.OrdinalIgnoreCase));
              if (!string.IsNullOrEmpty(arquivoDotx))
              {
                Console.WriteLine($"[INFO] Template encontrado: {arquivoDotx}");
                return arquivoDotx;
              }

              Console.WriteLine($"[INFO] Template encontrado: {arquivosEncontrados[0]}");
              return arquivosEncontrados[0];
            }
          }
          catch (Exception ex)
          {
            Console.WriteLine($"[AVISO] Erro ao buscar {pattern}: {ex.Message}");
            // Continua tentando com outras extensões
          }
        }

        // Tentativa alternativa: busca case-insensitive manual
        try
        {
          var todosArquivos = Directory.GetFiles(caminhoPastaBase, "*.*", SearchOption.AllDirectories)
            .Where(f => f.EndsWith(".docx", StringComparison.OrdinalIgnoreCase) ||
                        f.EndsWith(".dotx", StringComparison.OrdinalIgnoreCase))
            .ToList();

          // Busca por nome exato (case-insensitive)
          var arquivoEncontrado = todosArquivos.FirstOrDefault(f =>
          {
            string nomeDoArquivo = Path.GetFileNameWithoutExtension(f);
            return nomeDoArquivo.Equals(nomeArquivoSemExtensao, StringComparison.OrdinalIgnoreCase);
          });

          if (!string.IsNullOrEmpty(arquivoEncontrado))
          {
            Console.WriteLine($"[INFO] Template encontrado (busca alternativa): {arquivoEncontrado}");
            return arquivoEncontrado;
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine($"[AVISO] Erro na busca alternativa: {ex.Message}");
        }

        Console.WriteLine($"[AVISO] Template '{nomeArquivo}' não encontrado em {caminhoPastaBase}");
        return null;
      }
      catch (Exception ex)
      {
        Console.WriteLine($"[ERRO] Erro ao buscar arquivo recursivamente: {ex.Message}");
        return null;
      }
    }





    private string FormatarData(object data)
    {
      if (data != null && DateTime.TryParse(data.ToString(), out DateTime dt))
        return dt.ToString("dd/MM/yyyy");
      return "";
    }

    private async Task<Dictionary<string, object>> BuscarParametrosEscola(int cd_empresa, Source source)
    {
      try
      {
        var filtroParametro = new List<(string campo, object valor)> { new("cd_pessoa_escola", cd_empresa) };
        return await SQLServerService.GetFirstByFields(source, "T_PARAMETRO", filtroParametro);
      }
      catch (Exception)
      {
        return null;
      }
    }

    private string ConcatenarVencimentosTitulos(List<Dictionary<string, object>> titulos)
    {
      string retorno = "";
      if (titulos != null && titulos.Any())
      {
        foreach (var titulo in titulos)
        {
          var dtVencimento = Convert.ToDateTime(titulo["dt_vcto_titulo"]).ToString("dd/MM/yyyy");
          retorno += ", " + dtVencimento;
        }
      }
      if (retorno.Length >= 1)
        retorno = retorno.Substring(2, retorno.Length - 2);
      return retorno;
    }

    private string GerarDescricaoCompletaHorarios(List<Dictionary<string, object>> horarios)
    {
      string diasHorarios = "";
      if (horarios != null && horarios.Any())
      {
        var horariosOrdenados = horarios
            .Select(h => new
            {
              DiasSemana = h.ContainsKey("diaSemana") ? h["diaSemana"]?.ToString() ?? "" : "",
              HoraInicio = h.ContainsKey("dt_hora_ini") ? h["dt_hora_ini"]?.ToString() ?? "" : "",
              HoraFim = h.ContainsKey("dt_hora_fim") ? h["dt_hora_fim"]?.ToString() ?? "" : "",
              IdDiaSemana = h.ContainsKey("id_dia_semana") ? Convert.ToInt32(h["id_dia_semana"]) : 0
            })
            .OrderBy(h => h.IdDiaSemana)
            .ThenBy(h => h.HoraInicio)
            .ToList();

        foreach (var horario in horariosOrdenados)
        {
          string diasFormatados = horario.DiasSemana;
          string horarioFormatado = $"{horario.HoraInicio} às {horario.HoraFim}";

          diasHorarios += "; " + diasFormatados + " das " + horarioFormatado;
        }

        if (diasHorarios.Length >= 2)
          diasHorarios = diasHorarios.Substring(2, diasHorarios.Length - 2);
      }
      return diasHorarios;
    }

    /// <summary>
    /// Determina qual template usar para gerar o contrato com sistema de fallback inteligente
    /// </summary>
    /// <param name="matriculaExists">Dados do contrato</param>
    /// <param name="cd_pessoa_escola">Código da escola</param>
    /// <param name="source">Source do banco de dados</param>
    /// <returns>Nome do arquivo template a ser usado</returns>
    private async Task<string> DeterminarNomeTemplate(Dictionary<string, object> matriculaExists, int cd_pessoa_escola, Source source)
    {
      try
      {
        var cd_nome_contrato = matriculaExists["cd_nome_contrato"];

        // ESTRATÉGIA 1: Template específico definido no contrato
        if (cd_nome_contrato != null &&
            cd_nome_contrato != DBNull.Value &&
            Convert.ToInt32(cd_nome_contrato) > 0)
        {

          var nome_contrato = await SQLServerService.GetFirstByFields(
            source,
            "T_NOME_CONTRATO",
            new List<(string campo, object valor)> {
          new("cd_nome_contrato", cd_nome_contrato)
            }
          );

          if (nome_contrato != null &&
              nome_contrato.ContainsKey("no_relatorio") &&
              nome_contrato["no_relatorio"] != null &&
              nome_contrato["no_relatorio"] != DBNull.Value)
          {
            var nomeRelatorio = nome_contrato["no_relatorio"].ToString();
            if (!string.IsNullOrWhiteSpace(nomeRelatorio))
            {
              return nomeRelatorio;
            }
          }

        }
        throw new Exception("Contrato não possui layout definido.");
      }
      catch (Exception ex)
      {
        throw new Exception("Erro ao determinar template do contrato: " + ex.Message);
      }
    }

    /// <summary>
    /// Método auxiliar para listar templates disponíveis de uma escola (útil para debug)
    /// </summary>

    public static class ContratoFormatacao
    {
      public const string FONTE_PADRAO = "Arial";
      public const int TAMANHO_FONTE_NORMAL = 10;

      public static void AplicarFormatacaoPadrao(WordprocessingDocument doc)
      {
        var body = doc.MainDocumentPart.Document.Body;

        // Formatar todos os runs do documento
        foreach (var run in body.Descendants<Run>())
        {
          if (run.RunProperties == null)
            run.RunProperties = new RunProperties();

          // Remove formatações antigas de fonte
          run.RunProperties.RemoveAllChildren<RunFonts>();
          run.RunProperties.RemoveAllChildren<FontSize>();

          // Aplica nova formatação
          run.RunProperties.Append(new RunFonts()
          {
            Ascii = FONTE_PADRAO,
            HighAnsi = FONTE_PADRAO,
            ComplexScript = FONTE_PADRAO
          });

          run.RunProperties.Append(new FontSize()
          {
            Val = (TAMANHO_FONTE_NORMAL * 2).ToString() // Word usa half-points
          });
        }

        doc.MainDocumentPart.Document.Save();
      }
    }

    private async Task<List<Dictionary<string, object>>> ObterCursosDoContrato(
       Source source,
      int cdContrato)
    {
      var query = @"
        SELECT
            CC.cd_curso_contrato,
            C.no_curso,
            D.dc_duracao,
            R.no_regime,
            CC.vl_curso_contrato,
            CC.nm_parcelas_mensalidade,
            CC.vl_parcela_contrato,
            CC.pc_desconto_contrato,
            CC.vl_desconto_contrato,
            CC.vl_parcela_liquida,
            CC.vl_curso_liquido
        FROM T_CURSO_CONTRATO CC
        INNER JOIN T_CURSO C ON C.cd_curso = CC.cd_curso
        INNER JOIN T_DURACAO D ON D.cd_duracao = CC.cd_duracao
        LEFT JOIN T_REGIME R ON R.cd_regime = CC.cd_regime
        WHERE CC.cd_contrato = @cdContrato
        ORDER BY CC.cd_curso_contrato";

      var parameters = new Dictionary<string, object>
    {
        { "@cdContrato", cdContrato }
    };

      var result = await SQLServerService.ExecuteQuery(source, query, parameters);

      Console.WriteLine($"[ObterCursosDoContrato] Result: Success={result.Success}, Count={result.Data?.Count}, Data={result.Data}");

      if (!result.Success) Console.WriteLine($"[ObterCursosDoContratoError]");
      return result.Data;
    }
    /// <summary>
    /// Obtém os descontos de antecipação - COM LOGS COMPLETOS
    /// </summary>
    private async Task<List<Dictionary<string, object>>> ObterDescontosAntecipacao(
        Source source, int cdContrato, int cdPessoaEscola)
    {
      try
      {
        Console.WriteLine("==========================================================");
        Console.WriteLine($"[ObterDescontosAntecipacao] INÍCIO");
        Console.WriteLine($"[ObterDescontosAntecipacao] Contrato: {cdContrato}");
        Console.WriteLine($"[ObterDescontosAntecipacao] Escola: {cdPessoaEscola}");
        Console.WriteLine("==========================================================");

        // 1. Buscar contrato
        Console.WriteLine("[ObterDescontosAntecipacao] STEP 1: Buscando contrato...");
        var contrato = await SQLServerService.GetFirstByFields(
            source,
            "T_CONTRATO",
            new List<(string campo, object valor)> { new("cd_contrato", cdContrato) }
        );

        if (contrato == null)
        {
          Console.WriteLine("[ObterDescontosAntecipacao] ❌ ERRO: Contrato não encontrado!");
          return new List<Dictionary<string, object>>();
        }
        Console.WriteLine($"[ObterDescontosAntecipacao] ✓ Contrato encontrado");
        Console.WriteLine($"[ObterDescontosAntecipacao] - cd_aluno: {contrato.GetValueOrDefault("cd_aluno", "NULL")}");
        Console.WriteLine($"[ObterDescontosAntecipacao] - vl_desconto_contrato: {contrato.GetValueOrDefault("vl_desconto_contrato", 0)}");

        // 2. Validar desconto
        Console.WriteLine("[ObterDescontosAntecipacao] STEP 2: Validando desconto do contrato...");
        var vlDescontoContrato = Convert.ToDecimal(contrato.GetValueOrDefault("vl_desconto_contrato", 0));
        if (vlDescontoContrato <= 0)
        {
          Console.WriteLine($"[ObterDescontosAntecipacao] ❌ Contrato sem desconto (valor: {vlDescontoContrato})");
          return new List<Dictionary<string, object>>();
        }
        Console.WriteLine($"[ObterDescontosAntecipacao] ✓ Contrato tem desconto: R$ {vlDescontoContrato:N2}");

        var cdAluno = Convert.ToInt32(contrato["cd_aluno"]);

        // 3. Buscar política
        Console.WriteLine("[ObterDescontosAntecipacao] STEP 3: Buscando política de desconto...");
        int? cdPoliticaDesconto = null;

        // 3.1 Tentar política do aluno
        Console.WriteLine("[ObterDescontosAntecipacao] - Tentando T_POLITICA_ALUNO...");
        var politicaAluno = await SQLServerService.GetFirstByFields(
            source,
            "T_POLITICA_ALUNO",
            new List<(string campo, object valor)> { new("cd_aluno", cdAluno) }
        );

        if (politicaAluno != null && politicaAluno.ContainsKey("cd_politica_desconto"))
        {
          cdPoliticaDesconto = Convert.ToInt32(politicaAluno["cd_politica_desconto"]);
          Console.WriteLine($"[ObterDescontosAntecipacao] ✓ Política do aluno encontrada: {cdPoliticaDesconto}");
        }
        else
        {
          Console.WriteLine("[ObterDescontosAntecipacao] - Política do aluno não encontrada");
        }

        // 3.2 Se não tem política do aluno, buscar da escola
        if (!cdPoliticaDesconto.HasValue)
        {
          Console.WriteLine("[ObterDescontosAntecipacao] - Tentando T_POLITICA_DESCONTO (escola)...");
          var query = @"
        SELECT TOP 1 cd_politica_desconto
        FROM T_POLITICA_DESCONTO
        WHERE cd_pessoa_escola = @cdEscola
          AND id_ativo = 1
          AND dt_inicial_politica <= GETDATE()
        ORDER BY dt_inicial_politica DESC";

          var parameters = new Dictionary<string, object>
      {
        { "@cdEscola", cdPessoaEscola }
      };

          var result = await SQLServerService.ExecuteQuery(source, query, parameters);

          if (result.Success && result.Data != null && result.Data.Any())
          {
            cdPoliticaDesconto = Convert.ToInt32(result.Data[0]["cd_politica_desconto"]);
            Console.WriteLine($"[ObterDescontosAntecipacao] ✓ Política da escola encontrada: {cdPoliticaDesconto}");
          }
          else
          {
            Console.WriteLine("[ObterDescontosAntecipacao] - Política da escola não encontrada");
          }
        }

        if (!cdPoliticaDesconto.HasValue)
        {
          Console.WriteLine("[ObterDescontosAntecipacao] ❌ NENHUMA política de desconto encontrada");
          return new List<Dictionary<string, object>>();
        }

        // 4. Buscar parâmetros
        Console.WriteLine("[ObterDescontosAntecipacao] STEP 4: Buscando parâmetros da escola...");
        var parametrosEscola = await BuscarParametrosEscola(cdPessoaEscola, source);
        if (parametrosEscola == null)
        {
          Console.WriteLine("[ObterDescontosAntecipacao] ❌ Parâmetros não encontrados");
          return new List<Dictionary<string, object>>();
        }
        Console.WriteLine("[ObterDescontosAntecipacao] ✓ Parâmetros encontrados");

        // 5. Buscar títulos
        Console.WriteLine("[ObterDescontosAntecipacao] STEP 5: Buscando títulos abertos...");
        var titulosAbertos = await BuscarTitulosAbertosParaSimulacao(source, cdContrato, cdPessoaEscola);

        Console.WriteLine($"[ObterDescontosAntecipacao] Total títulos retornados: {titulosAbertos.Count}");

        if (!titulosAbertos.Any())
        {
          Console.WriteLine("[ObterDescontosAntecipacao] ❌ Nenhum título aberto encontrado");
          Console.WriteLine("[ObterDescontosAntecipacao] FINALIZANDO SEM DADOS");
          Console.WriteLine("==========================================================");
          return new List<Dictionary<string, object>>();
        }

        Console.WriteLine($"[ObterDescontosAntecipacao] ✓ {titulosAbertos.Count} títulos abertos encontrados");
        foreach (var tit in titulosAbertos)
        {
          Console.WriteLine($"  - Título {tit.GetValueOrDefault("cd_titulo", "?")} | " +
                           $"Tipo: {tit.GetValueOrDefault("dc_tipo_titulo", "?")} | " +
                           $"Parcela: {tit.GetValueOrDefault("nm_parcela_titulo", "?")} | " +
                           $"Saldo: R$ {Convert.ToDecimal(tit.GetValueOrDefault("vl_saldo_titulo", 0)):N2}");
        }

        // 6. Simular baixas
        Console.WriteLine("[ObterDescontosAntecipacao] STEP 6: Simulando baixas dos títulos...");
        var descontosPoliticaTitulos = new List<Dictionary<string, object>>();

        int tituloIndex = 0;
        foreach (var titulo in titulosAbertos)
        {
          tituloIndex++;
          Console.WriteLine($"[ObterDescontosAntecipacao] - Simulando título {tituloIndex}/{titulosAbertos.Count} (cd: {titulo["cd_titulo"]})...");

          var simulacaoBaixa = await _simulacaoBaixaService.SimularBaixaTitulo(
              titulo,
              DateTime.Now,
              parametrosEscola,
              source
          );

          if (simulacaoBaixa.ExtraData != null && simulacaoBaixa.ExtraData.ContainsKey("diasPoliticaAntecipacao"))
          {
            var diasPolitica = simulacaoBaixa.ExtraData["diasPoliticaAntecipacao"] as List<Dictionary<string, object>>;
            if (diasPolitica != null && diasPolitica.Any())
            {
              Console.WriteLine($"[ObterDescontosAntecipacao]   ✓ {diasPolitica.Count} dias de política retornados");
              descontosPoliticaTitulos.AddRange(diasPolitica);
            }
            else
            {
              Console.WriteLine($"[ObterDescontosAntecipacao]   - Nenhum dia de política retornado");
            }
          }
          else
          {
            Console.WriteLine($"[ObterDescontosAntecipacao]   - ExtraData não contém diasPoliticaAntecipacao");
          }
        }

        Console.WriteLine($"[ObterDescontosAntecipacao] Total de registros de política coletados: {descontosPoliticaTitulos.Count}");

        // 7. Se não retornou dados da simulação, buscar diretamente
        if (!descontosPoliticaTitulos.Any())
        {
          Console.WriteLine("[ObterDescontosAntecipacao] STEP 7: Simulações não retornaram dados. Buscando dias da política diretamente...");
          Console.WriteLine($"[ObterDescontosAntecipacao] - cd_politica_desconto: {cdPoliticaDesconto}");

          var diasPoliticaResult = await SQLServerService.GetList(
              "T_DIAS_POLITICA",
              null,
              "[cd_politica_desconto]",
              $"[{cdPoliticaDesconto.Value}]",
              source
          );

          Console.WriteLine($"[ObterDescontosAntecipacao] - Query T_DIAS_POLITICA success: {diasPoliticaResult.success}");
          Console.WriteLine($"[ObterDescontosAntecipacao] - Registros retornados: {diasPoliticaResult.data?.Count ?? 0}");

          if (diasPoliticaResult.success && diasPoliticaResult.data != null && diasPoliticaResult.data.Any())
          {
            Console.WriteLine("[ObterDescontosAntecipacao] ✓ Dias de política encontrados:");

            foreach (var dia in diasPoliticaResult.data)
            {
              var nmDia = dia.GetValueOrDefault("nm_dia_limite_politica", "?");
              var pcDesc = dia.GetValueOrDefault("pc_desconto", "?");
              Console.WriteLine($"  - Dia {nmDia}: {pcDesc}%");
            }

            // Calcular valores manualmente
            var nmDiaVcto = Convert.ToInt32(contrato.GetValueOrDefault("nm_dia_vcto", 5));
            var nmMesVcto = Convert.ToInt32(contrato.GetValueOrDefault("nm_mes_vcto", DateTime.Now.Month));
            var nmAnoVcto = Convert.ToInt32(contrato.GetValueOrDefault("nm_ano_vcto", DateTime.Now.Year));

            DateTime dataVencimentoMatricula;
            try
            {
              dataVencimentoMatricula = new DateTime(nmAnoVcto, nmMesVcto, nmDiaVcto);
              Console.WriteLine($"[ObterDescontosAntecipacao] Data vencimento matrícula: {dataVencimentoMatricula:dd/MM/yyyy}");
            }
            catch
            {
              dataVencimentoMatricula = DateTime.Now;
              Console.WriteLine($"[ObterDescontosAntecipacao] ⚠ Erro ao calcular data vcto, usando hoje: {dataVencimentoMatricula:dd/MM/yyyy}");
            }

            foreach (var diaPolitica in diasPoliticaResult.data)
            {
              var nmDiaLimite = Convert.ToInt32(diaPolitica["nm_dia_limite_politica"]);
              var pcDesconto = Convert.ToDecimal(diaPolitica["pc_desconto"]);

              var dataPolitica = CalcularDataPolitica(dataVencimentoMatricula, nmDiaLimite);
              if (dataPolitica < dataVencimentoMatricula)
                dataPolitica = dataVencimentoMatricula;

              descontosPoliticaTitulos.Add(new Dictionary<string, object>
          {
            { "cd_politica_desconto", cdPoliticaDesconto.Value },
            { "nm_dia_limite_politica", nmDiaLimite },
            { "pc_desconto", pcDesconto },
            { "pc_pontualidade", pcDesconto },
            { "Data_politica", dataPolitica },
            { "dt_limite_desconto", dataPolitica }
          });
            }

            Console.WriteLine($"[ObterDescontosAntecipacao] ✓ {descontosPoliticaTitulos.Count} registros criados manualmente");
          }
          else
          {
            Console.WriteLine("[ObterDescontosAntecipacao] ❌ Nenhum dia de política encontrado em T_DIAS_POLITICA");
          }
        }

        if (!descontosPoliticaTitulos.Any())
        {
          Console.WriteLine("[ObterDescontosAntecipacao] ❌ NENHUM desconto de política disponível");
          Console.WriteLine("[ObterDescontosAntecipacao] FINALIZANDO SEM DADOS");
          Console.WriteLine("==========================================================");
          return new List<Dictionary<string, object>>();
        }

        // 8. Agrupar
        Console.WriteLine("[ObterDescontosAntecipacao] STEP 8: Agrupando descontos...");
        var grupoDescontosAntecipacao = descontosPoliticaTitulos
            .GroupBy(x => new
            {
              cd_politica = Convert.ToInt32(x["cd_politica_desconto"]),
              nm_dia = Convert.ToInt32(x["nm_dia_limite_politica"])
            })
            .Select(g =>
            {
              var primeiro = g.First();
              var dataPolitica = primeiro.ContainsKey("Data_politica")
                  ? Convert.ToDateTime(primeiro["Data_politica"])
                  : (primeiro.ContainsKey("dt_limite_desconto")
                      ? Convert.ToDateTime(primeiro["dt_limite_desconto"])
                      : DateTime.Now);

              var pcPontualidade = primeiro.ContainsKey("pc_pontualidade")
                  ? Convert.ToDecimal(primeiro["pc_pontualidade"])
                  : (primeiro.ContainsKey("pc_desconto")
                      ? Convert.ToDecimal(primeiro["pc_desconto"])
                      : 0m);

              return new Dictionary<string, object>
              {
            { "cd_politica_desconto", primeiro["cd_politica_desconto"] },
            { "nm_dia_limite_politica", primeiro["nm_dia_limite_politica"] },
            { "Data_politica", dataPolitica },
            { "pc_pontualidade", pcPontualidade },
            { "dt_limite_desconto", dataPolitica }
              };
            })
            .OrderBy(d => Convert.ToInt32(d["nm_dia_limite_politica"]))
            .ToList();

        Console.WriteLine($"[ObterDescontosAntecipacao] ✓ Agrupados em {grupoDescontosAntecipacao.Count} registros únicos");

        Console.WriteLine("[ObterDescontosAntecipacao] RESULTADO FINAL:");
        foreach (var desc in grupoDescontosAntecipacao)
        {
          Console.WriteLine($"  - Dia {desc["nm_dia_limite_politica"]}: " +
                           $"até {Convert.ToDateTime(desc["Data_politica"]):dd/MM/yyyy} - " +
                           $"{desc["pc_pontualidade"]}%");
        }

        Console.WriteLine("[ObterDescontosAntecipacao] FINALIZADO COM SUCESSO");
        Console.WriteLine("==========================================================");

        return grupoDescontosAntecipacao;
      }
      catch (Exception ex)
      {
        Console.WriteLine("==========================================================");
        Console.WriteLine($"[ObterDescontosAntecipacao] ❌❌❌ EXCEÇÃO CAPTURADA ❌❌❌");
        Console.WriteLine($"[ObterDescontosAntecipacao] Mensagem: {ex.Message}");
        Console.WriteLine($"[ObterDescontosAntecipacao] StackTrace: {ex.StackTrace}");
        Console.WriteLine("==========================================================");
        return new List<Dictionary<string, object>>();
      }
    }




    /// <summary>
    /// Busca títulos abertos - COM LOGS COMPLETOS
    /// </summary>
    private async Task<List<Dictionary<string, object>>> BuscarTitulosAbertosParaSimulacao(
        Source source, int cdContrato, int cdPessoaEscola)
    {
      try
      {
        Console.WriteLine("----------------------------------------------------------");
        Console.WriteLine($"[BuscarTitulosAbertosParaSimulacao] INÍCIO");
        Console.WriteLine($"[BuscarTitulosAbertosParaSimulacao] Contrato: {cdContrato}");
        Console.WriteLine($"[BuscarTitulosAbertosParaSimulacao] Escola: {cdPessoaEscola}");

        var query = @"
        SELECT
            t.cd_titulo,
            t.nm_parcela_titulo,
            t.dt_vcto_titulo,
            t.vl_titulo,
            t.vl_saldo_titulo,
            ISNULL(t.vl_material_titulo, 0) as vl_material_titulo,
            t.cd_origem_titulo,
            t.id_origem_titulo,
            t.cd_pessoa_empresa,
            t.dc_tipo_titulo,
            t.id_status_titulo,
            t.id_status_cnab
        FROM T_TITULO t
        WHERE t.cd_origem_titulo = @cdContrato
            AND t.id_origem_titulo = 22
            AND t.cd_pessoa_empresa = @cdEscola
            AND t.vl_saldo_titulo > 0
            AND t.id_status_titulo IN (1, 2)
        ORDER BY t.dt_vcto_titulo";

        Console.WriteLine($"[BuscarTitulosAbertosParaSimulacao] Query montada:");
        Console.WriteLine(query);
        Console.WriteLine($"[BuscarTitulosAbertosParaSimulacao] Parâmetros:");
        Console.WriteLine($"  @cdContrato = {cdContrato}");
        Console.WriteLine($"  @cdEscola = {cdPessoaEscola}");

        var parameters = new Dictionary<string, object>
    {
        { "@cdContrato", cdContrato },
        { "@cdEscola", cdPessoaEscola }
    };

        var result = await SQLServerService.ExecuteQuery(source, query, parameters);

        Console.WriteLine($"[BuscarTitulosAbertosParaSimulacao] Query executada");
        Console.WriteLine($"[BuscarTitulosAbertosParaSimulacao] Success: {result.Success}");
        Console.WriteLine($"[BuscarTitulosAbertosParaSimulacao] Data Count: {result.Data?.Count ?? 0}");

        if (result.Success && result.Data != null && result.Data.Any())
        {
          Console.WriteLine($"[BuscarTitulosAbertosParaSimulacao] ✓ {result.Data.Count} títulos encontrados");

          var tipos = result.Data
              .Select(t => t.ContainsKey("dc_tipo_titulo") ? t["dc_tipo_titulo"]?.ToString() : "NULL")
              .Distinct()
              .ToList();

          Console.WriteLine($"[BuscarTitulosAbertosParaSimulacao] Tipos de título encontrados: {string.Join(", ", tipos)}");

          Console.WriteLine($"[BuscarTitulosAbertosParaSimulacao] Detalhes dos títulos:");
          foreach (var t in result.Data)
          {
            Console.WriteLine($"  cd_titulo: {t.GetValueOrDefault("cd_titulo", "?")} | " +
                             $"tipo: {t.GetValueOrDefault("dc_tipo_titulo", "?")} | " +
                             $"parcela: {t.GetValueOrDefault("nm_parcela_titulo", "?")} | " +
                             $"status: {t.GetValueOrDefault("id_status_titulo", "?")} | " +
                             $"saldo: {t.GetValueOrDefault("vl_saldo_titulo", 0)}");
          }

          Console.WriteLine("----------------------------------------------------------");
          return result.Data;
        }

        Console.WriteLine($"[BuscarTitulosAbertosParaSimulacao] ❌ Nenhum título encontrado");
        Console.WriteLine("----------------------------------------------------------");
        return new List<Dictionary<string, object>>();
      }
      catch (Exception ex)
      {
        Console.WriteLine("----------------------------------------------------------");
        Console.WriteLine($"[BuscarTitulosAbertosParaSimulacao] ❌ EXCEÇÃO:");
        Console.WriteLine($"  Mensagem: {ex.Message}");
        Console.WriteLine($"  StackTrace: {ex.StackTrace}");
        Console.WriteLine("----------------------------------------------------------");
        return new List<Dictionary<string, object>>();
      }
    }

    /// <summary>
    /// Calcula a data da política com base no dia limite
    /// </summary>
    private DateTime CalcularDataPolitica(DateTime dtBase, int diaLimite)
    {
      try
      {
        return new DateTime(dtBase.Year, dtBase.Month, diaLimite);
      }
      catch (ArgumentOutOfRangeException)
      {
        // Se o dia não existe no mês, tenta dias anteriores
        for (int i = 1; i <= 3; i++)
        {
          try
          {
            return new DateTime(dtBase.Year, dtBase.Month, diaLimite - i);
          }
          catch
          {
            continue;
          }
        }
      }

      return dtBase;
    }

    /// <summary>
    /// Preenche grade - COM LOGS COMPLETOS
    /// </summary>
    public static void PreencherGradeDescontosAntecipacao(
        WordprocessingDocument doc,
        List<Dictionary<string, object>> descontos)
    {
      Console.WriteLine("**********************************************************");
      Console.WriteLine("[PreencherGradeDescontosAntecipacao] INÍCIO");
      Console.WriteLine($"[PreencherGradeDescontosAntecipacao] Descontos recebidos: {descontos?.Count ?? 0}");

      if (descontos != null && descontos.Any())
      {
        Console.WriteLine("[PreencherGradeDescontosAntecipacao] Detalhes dos descontos:");
        foreach (var d in descontos)
        {
          Console.WriteLine($"  - Dia: {d.GetValueOrDefault("nm_dia_limite_politica", "?")} | " +
                           $"Data: {d.GetValueOrDefault("Data_politica", "?")} | " +
                           $"%: {d.GetValueOrDefault("pc_pontualidade", "?")}");
        }
      }

      var body = doc.MainDocumentPart.Document.Body;
      string tag = "GradeValoresDescontosAntecipa";

      Console.WriteLine($"[PreencherGradeDescontosAntecipacao] Procurando tag: {tag}");

      var paragrafoComTag = body.Descendants<Paragraph>()
          .FirstOrDefault(p => p.InnerText.Contains($"«{tag}»") || p.InnerText.Contains($"<{tag}>"));

      if (paragrafoComTag != null)
      {
        Console.WriteLine("[PreencherGradeDescontosAntecipacao] ✓ Tag encontrada no documento");

        if (descontos == null || descontos.Count == 0)
        {
          Console.WriteLine("[PreencherGradeDescontosAntecipacao] Inserindo 'Não informado'");
          var paragrafoMensagem = new Paragraph(new Run(new Text("Não informado")));
          paragrafoComTag.InsertAfterSelf(paragrafoMensagem);
        }
        else
        {
          Console.WriteLine("[PreencherGradeDescontosAntecipacao] Criando tabela...");
          var tabela = CriarTabelaDescontosAntecipacao(descontos);
          paragrafoComTag.InsertAfterSelf(tabela);
          Console.WriteLine("[PreencherGradeDescontosAntecipacao] ✓ Tabela inserida");
        }

        paragrafoComTag.Remove();
        Console.WriteLine("[PreencherGradeDescontosAntecipacao] Tag removida");
      }
      else
      {
        Console.WriteLine("[PreencherGradeDescontosAntecipacao] ❌ Tag NÃO encontrada no documento");
      }

      doc.MainDocumentPart.Document.Save();
      Console.WriteLine("[PreencherGradeDescontosAntecipacao] Documento salvo");
      Console.WriteLine("[PreencherGradeDescontosAntecipacao] FINALIZADO");
      Console.WriteLine("**********************************************************");
    }
    /// <summary>
    /// Cria a tabela de descontos de antecipação (baseada no sistema legado)
    /// </summary>
    private static Table CriarTabelaDescontosAntecipacao(List<Dictionary<string, object>> descontos)
    {
      var table = new Table();

      // Propriedades da tabela
      var tableProperties = new TableProperties(
          new TableBorders(
              new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 6 },
              new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 6 },
              new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 6 },
              new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 6 },
              new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 6 },
              new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 6 }
          ),
          new TableWidth { Width = "5000", Type = TableWidthUnitValues.Pct }
      );
      table.AppendChild(tableProperties);

      // Linha de cabeçalho (igual ao sistema legado)
      var headerRow = new TableRow();
      headerRow.Append(
          CriarCelula("Dia Venc. Desconto por Antecipação", true),
          CriarCelula("DIA", true),
          CriarCelula("(%)", true)
      );
      table.Append(headerRow);

      // Linhas de dados
      foreach (var desconto in descontos)
      {
        var dtLimiteDesconto = desconto.ContainsKey("dt_limite_desconto")
            ? Convert.ToDateTime(desconto["dt_limite_desconto"])
            : DateTime.Now;

        var nmDiaLimite = desconto.ContainsKey("nm_dia_limite_politica")
            ? desconto["nm_dia_limite_politica"]?.ToString() ?? "0"
            : "0";

        var pcDesconto = desconto.ContainsKey("pc_desconto")
            ? Convert.ToDecimal(desconto["pc_desconto"])
            : 0m;

        // Formatar a primeira coluna igual ao sistema legado: "A partir de dd/MM/yyyy"
        var textoDataLimite = $"A partir de {dtLimiteDesconto:dd/MM/yyyy}";

        var dataRow = new TableRow();
        dataRow.Append(
            CriarCelula(textoDataLimite, false), // Centralizado
            CriarCelula(nmDiaLimite, false),      // Centralizado
            CriarCelula($"{pcDesconto:F2}", false) // Formatado com 2 casas decimais e centralizado
        );
        table.Append(dataRow);
      }

      return table;
    }


    private async Task<DateTime> ObterDataVencimentoContrato(Source source, int cdContrato)
    {
      var query = @"
        SELECT *
        FROM T_ADITAMENTO
        WHERE cd_contrato = @cdContrato
        ORDER BY dt_aditamento DESC";
      var parameters = new Dictionary<string, object> { { "@cdContrato", cdContrato } };
      var result = await SQLServerService.ExecuteQuery(source, query, parameters);
      return result.Success && result.Data.Any()
          ? Convert.ToDateTime(result.Data.First()["dt_vcto_aditamento"])
          : DateTime.MinValue;
    }
    private static Table CriarTabelaCursos(List<Dictionary<string, object>> cursos)
    {
      var tabela = new Table();

      // 🔹 Bordas simples
      var props = new TableProperties(
          new TableBorders(
              new TopBorder { Val = BorderValues.Single, Size = 6 },
              new BottomBorder { Val = BorderValues.Single, Size = 6 },
              new LeftBorder { Val = BorderValues.Single, Size = 6 },
              new RightBorder { Val = BorderValues.Single, Size = 6 },
              new InsideHorizontalBorder { Val = BorderValues.Single, Size = 6 },
              new InsideVerticalBorder { Val = BorderValues.Single, Size = 6 }
          )
      );

      tabela.AppendChild(props);

      // 🔹 Cabeçalho
      var headerRow = new TableRow();
      headerRow.Append(
          CriarCelula("Nome do Curso", true),
          CriarCelula("Duração", true),
          CriarCelula("Modalidade", true),
          CriarCelula("Valor do Curso", true),
          CriarCelula("Nº Parcelas", true),
          CriarCelula("Valor da Parcela", true),
          CriarCelula("% Desconto", true),
          CriarCelula("Parcela Líquida", true)
      );
      tabela.Append(headerRow);

      // 🔹 Linhas dos cursos
      foreach (var curso in cursos)
      {
        var row = new TableRow();

        row.Append(
            CriarCelula(curso["no_curso"]?.ToString() ?? ""),
            CriarCelula(curso["dc_duracao"]?.ToString() ?? ""),
            CriarCelula(curso["no_regime"]?.ToString() ?? ""),
            CriarCelula($"R$ {Convert.ToDecimal(curso["vl_curso_contrato"] ?? 0):N2}"),
            CriarCelula(curso["nm_parcelas_mensalidade"]?.ToString() ?? ""),
            CriarCelula($"R$ {Convert.ToDecimal(curso["vl_parcela_contrato"] ?? 0):N2}"),
            CriarCelula($"{Convert.ToDecimal(curso["pc_desconto_contrato"] ?? 0):N2}%"),
            CriarCelula($"R$ {Convert.ToDecimal(curso["vl_parcela_liquida"] ?? 0):N2}")
        );

        tabela.Append(row);
      }

      return tabela;
    }

    private static TableCell CriarCelula(string texto, bool negrito = false)
    {
      var run = new Run(new Text(texto));
      if (negrito)
        run.RunProperties = new RunProperties(new Bold());

      var cell = new TableCell(new Paragraph(run));
      cell.Append(new TableCellProperties(
          new TableCellWidth { Type = TableWidthUnitValues.Auto }));

      return cell;
    }



    // Método auxiliar para criar a tabela
    private static Table CriarTabelaDescontos(List<Dictionary<string, object>> descontos)
    {
      var table = new Table();

      // Propriedades da tabela
      var tableProperties = new TableProperties(
          new TableBorders(
              new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
              new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
              new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
              new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
              new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
              new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 }
          ),
          new TableWidth { Width = "5000", Type = TableWidthUnitValues.Pct }
      );
      table.AppendChild(tableProperties);

      // Linha de cabeçalho
      var headerRow = new TableRow();
      headerRow.Append(
          CriarCelula("Dias de Antecipação", true),
          CriarCelula("% Desconto", true),
          CriarCelula("Valor Desconto", true),
          CriarCelula("Valor com Desconto", true)
      );
      table.Append(headerRow);

      // Linhas de dados
      foreach (var desconto in descontos)
      {
        var nmDiaLimite = desconto.ContainsKey("nm_dia_limite_politica") ? desconto["nm_dia_limite_politica"]?.ToString() ?? "0" : "0";
        var pcDesconto = desconto.ContainsKey("pc_desconto") ? Convert.ToDecimal(desconto["pc_desconto"]).ToString("N2") : "0.00";
        var vlDesconto = desconto.ContainsKey("vl_desconto") ? Convert.ToDecimal(desconto["vl_desconto"]).ToString("N2") : "0.00";
        var vlComDesconto = desconto.ContainsKey("vl_com_desconto") ? Convert.ToDecimal(desconto["vl_com_desconto"]).ToString("N2") : "0.00";

        var dataRow = new TableRow();
        dataRow.Append(
            CriarCelula(nmDiaLimite),
            CriarCelula($"{pcDesconto}%"),
            CriarCelula($"R$ {vlDesconto}"),
            CriarCelula($"R$ {vlComDesconto}")
        );
        table.Append(dataRow);
      }

      return table;
    }
    public static void PreencherGradeCursos(WordprocessingDocument doc, List<Dictionary<string, object>> cursos)
    {

      {
        var body = doc.MainDocumentPart.Document.Body;
        string tag = "GradeCursos";
        bool tabelaInserida = false;

        // 🔹 Procura parágrafo com a TAG
        var paragrafoComTag = body.Descendants<Paragraph>()
            .FirstOrDefault(p => p.InnerText.Contains($"«{tag}»") || p.InnerText.Contains($"<{tag}>"));

        if (paragrafoComTag != null)
        {
          // 🔹 Cria a tabela
          var tabela = CriarTabelaCursos(cursos);

          // 🔹 Insere logo após o parágrafo da tag
          paragrafoComTag.InsertAfterSelf(tabela);

          // 🔹 Remove a tag
          paragrafoComTag.Remove();

          tabelaInserida = true;
        }

        // 🔹 Caso não exista a tag no documento, adiciona no final
        if (!tabelaInserida)
        {
          var tabela = CriarTabelaCursos(cursos);
          body.AppendChild(new Paragraph(new Run(new Text("Grade de Cursos:"))));
          body.AppendChild(tabela);
        }

        doc.MainDocumentPart.Document.Save();
      }
    }


    /// <summary>
    /// Preenche a grade de Valores das Parcelas (VALORES BRUTOS)
    /// </summary>
    public static void PreencherGradeValoresParcelas(
        WordprocessingDocument doc,
        List<Dictionary<string, object>> parcelas)
    {
      try
      {

        var body = doc.MainDocumentPart.Document.Body;
        string tag = "GradeValoresParcelas";

        // Log de todos os parágrafos para debug
        var todosParagrafos = body.Descendants<Paragraph>().ToList();


        // MÉTODO 1: Procurar pela tag como texto simples
        var paragrafoComTag = body.Descendants<Paragraph>()
            .FirstOrDefault(p => p.InnerText.Contains($"«{tag}»") ||
                                p.InnerText.Contains($"<{tag}>") ||
                                p.InnerText.Contains(tag));

        // MÉTODO 2: Se não encontrou, procurar em SimpleFields (campos do Word)
        if (paragrafoComTag == null)
        {

          // Buscar em SimpleField
          var campoSimples = body.Descendants<SimpleField>()
              .FirstOrDefault(f => f.Instruction?.Value?.Contains(tag) == true);

          if (campoSimples != null)
          {

            paragrafoComTag = campoSimples.Ancestors<Paragraph>().FirstOrDefault();
          }
        }

        // MÉTODO 3: Se ainda não encontrou, procurar em FieldCode (campos complexos)
        if (paragrafoComTag == null)
        {



          var campoComplexo = body.Descendants<FieldCode>()
              .FirstOrDefault(f => f.Text?.Contains(tag) == true);

          if (campoComplexo != null)
          {

            paragrafoComTag = campoComplexo.Ancestors<Paragraph>().FirstOrDefault();
          }
        }

        // MÉTODO 4: Busca case-insensitive em todo o texto
        if (paragrafoComTag == null)
        {


          paragrafoComTag = body.Descendants<Paragraph>()
              .FirstOrDefault(p => p.InnerText.ToLower().Contains(tag.ToLower()));

          if (paragrafoComTag != null)
          {

          }
        }

        if (paragrafoComTag != null)
        {


          if (parcelas == null || parcelas.Count == 0)
          {

            var paragrafoMensagem = new Paragraph(new Run(new Text("Não há parcelas a exibir.")));
            paragrafoComTag.InsertAfterSelf(paragrafoMensagem);
          }
          else
          {

            var tabela = CriarTabelaValoresParcelas(parcelas);
            paragrafoComTag.InsertAfterSelf(tabela);

          }

          // Remover o parágrafo inteiro (incluindo campos)
          paragrafoComTag.Remove();

        }
        else
        {


          // Log de debug dos primeiros parágrafos
          var primeiros = todosParagrafos.Take(20).Select(p => p.InnerText).ToList();

          for (int i = 0; i < primeiros.Count; i++)
          {
            var texto = primeiros[i];
            if (texto.Length > 150) texto = texto.Substring(0, 150) + "...";
            Console.WriteLine($"  [{i}]: {texto}");
          }

          // Log de campos encontrados
          var todosSimpleFields = body.Descendants<SimpleField>().ToList();
          var todosFieldCodes = body.Descendants<FieldCode>().ToList();


          if (todosSimpleFields.Any())
          {

            foreach (var sf in todosSimpleFields.Take(10))
            {
              Console.WriteLine($"  - {sf.Instruction?.Value}");
            }
          }
        }

        doc.MainDocumentPart.Document.Save();

      }
      catch (Exception ex)
      {
        Console.WriteLine($"[PreencherGradeValoresParcelas] ERRO: {ex.Message}");
        Console.WriteLine($"[PreencherGradeValoresParcelas] StackTrace: {ex.StackTrace}");
        throw;
      }
    }
    private static Table CriarTabelaValoresParcelas(List<Dictionary<string, object>> parcelas)
    {
      try
      {


        var table = new Table();

        var tableProperties = new TableProperties(
          new TableBorders(
            new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
            new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
            new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
            new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
            new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
            new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 }
          ),
          new TableWidth { Width = "5000", Type = TableWidthUnitValues.Pct }
        );
        table.AppendChild(tableProperties);

        // Cabeçalho igual ao legado
        var headerRow = new TableRow();
        headerRow.Append(
          CriarCelula("VENCIMENTO", true),
          CriarCelula("DIA", true),
          CriarCelula("MATERIAL (R$)", true),
          CriarCelula("PARCELA (R$)", true),
          CriarCelula("TOTAL (R$)", true)
        );
        table.Append(headerRow);


        // Dados
        int linhaCount = 0;
        foreach (var parcela in parcelas)
        {
          DateTime dtVcto = Convert.ToDateTime(parcela["dt_vcto_titulo"]);
          int dia = dtVcto.Day;

          // Verifica se é consolidado ou individual
          decimal vlMaterial = 0;
          decimal vlMensalidade = 0;

          string tipoParcela = parcela["dc_tipo_titulo"]?.ToString() ?? "DESCONHECIDO";
          if (tipoParcela == "CONSOLIDADO")
          {
            // Dados já consolidados
            vlMaterial = Convert.ToDecimal(parcela["vl_material"] ?? 0);
            vlMensalidade = Convert.ToDecimal(parcela["vl_mensalidade"] ?? 0);

          }
          else
          {
            // Dados individuais (mantém compatibilidade)
            vlMaterial = tipoParcela == "MT" ? Convert.ToDecimal(parcela["vl_titulo"] ?? 0) : 0;
            vlMensalidade = tipoParcela == "ME" ? Convert.ToDecimal(parcela["vl_titulo"] ?? 0) : 0;

          }

          decimal vlTotal = vlMaterial + vlMensalidade;

          var dataRow = new TableRow();
          dataRow.Append(
            CriarCelula(dtVcto.ToString("dd/MM/yyyy")),
            CriarCelula(dia.ToString()),
            CriarCelula($"R$ {vlMaterial:N2}"),
            CriarCelula($"R$ {vlMensalidade:N2}"),
            CriarCelula($"R$ {vlTotal:N2}")
          );
          table.Append(dataRow);
          linhaCount++;
        }


        return table;
      }
      catch (Exception ex)
      {
        Console.WriteLine($"[CriarTabelaValoresParcelas] ERRO: {ex.Message}");
        Console.WriteLine($"[CriarTabelaValoresParcelas] StackTrace: {ex.StackTrace}");
        throw;
      }
    }
    /// <summary>
    /// Preenche a grade de Descontos do Contrato (DESCONTOS APLICADOS)
    /// </summary>
    public static void PreencherGradeDescontosContrato(
        WordprocessingDocument doc,
        List<Dictionary<string, object>> descontos)
    {
      var body = doc.MainDocumentPart.Document.Body;
      string tag = "GradeDescontosContrato";

      var paragrafoComTag = body.Descendants<Paragraph>()
          .FirstOrDefault(p => p.InnerText.Contains($"«{tag}»") ||
                              p.InnerText.Contains($"<{tag}>"));

      if (paragrafoComTag != null)
      {
        // Se há descontos, cria a tabela
        if (descontos != null && descontos.Count > 0)
        {
          var tabela = CriarTabelaDescontosContrato(descontos);
          paragrafoComTag.InsertAfterSelf(tabela);
        }

        // SEMPRE remove a tag, independente se há ou não dados
        paragrafoComTag.Remove();
      }

      doc.MainDocumentPart.Document.Save();
    }
    private static Table CriarTabelaDescontosContrato(List<Dictionary<string, object>> descontos)
    {
      var table = new Table();

      var tableProperties = new TableProperties(
          new TableBorders(
              new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
              new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
              new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
              new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
              new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
              new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 }
          ),
          new TableWidth { Width = "5000", Type = TableWidthUnitValues.Pct }
      );
      table.AppendChild(tableProperties);

      // Cabeçalho
      var headerRow = new TableRow();
      headerRow.Append(
          CriarCelula("Descrição", true),
          CriarCelula("Percentual", true),
          CriarCelula("Valor", true),
          CriarCelula("Parcela Inicial", true),
          CriarCelula("Parcela Final", true)
      );
      table.Append(headerRow);

      // Dados
      foreach (var desconto in descontos)
      {
        string descricao = desconto["dc_desconto_contrato"]?.ToString() ?? "Desconto";
        decimal percentual = Convert.ToDecimal(desconto["pc_desconto_contrato"] ?? 0);
        decimal valor = Convert.ToDecimal(desconto["vl_desconto_contrato"] ?? 0);
        string parcelaIni = desconto["nm_parcela_ini"]?.ToString() ?? "-";
        string parcelaFim = desconto["nm_parcela_fim"]?.ToString() ?? "-";

        var dataRow = new TableRow();
        dataRow.Append(
            CriarCelula(descricao),
            CriarCelula($"{percentual.ToString("N2")}%"),
            CriarCelula($"R$ {valor.ToString("N2")}"),
            CriarCelula(parcelaIni),
            CriarCelula(parcelaFim)
        );
        table.Append(dataRow);
      }

      return table;
    }


    /// <summary>
    /// Preenche a grade de Valores Líquidos (VALORES COM DESCONTO)
    /// </summary>
    public static void PreencherGradeValoresLiquidos(
        WordprocessingDocument doc,
        List<Dictionary<string, object>> parcelas)
    {
      var body = doc.MainDocumentPart.Document.Body;
      string tag = "GradeValoresLiquidos";

      var paragrafoComTag = body.Descendants<Paragraph>()
          .FirstOrDefault(p => p.InnerText.Contains($"«{tag}»") ||
                              p.InnerText.Contains($"<{tag}>"));

      if (paragrafoComTag != null)
      {
        // Se há parcelas com desconto, cria a tabela
        if (parcelas != null && parcelas.Count > 0)
        {
          var tabela = CriarTabelaValoresLiquidos(parcelas);
          paragrafoComTag.InsertAfterSelf(tabela);
        }

        // SEMPRE remove a tag, independente se há ou não dados
        paragrafoComTag.Remove();
      }

      doc.MainDocumentPart.Document.Save();
    }

    private static Table CriarTabelaValoresLiquidos(List<Dictionary<string, object>> parcelas)
    {
      var table = new Table();

      var tableProperties = new TableProperties(
          new TableBorders(
              new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
              new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
              new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
              new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
              new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
              new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 }
          ),
          new TableWidth { Width = "5000", Type = TableWidthUnitValues.Pct }
      );
      table.AppendChild(tableProperties);

      // Cabeçalho
      var headerRow = new TableRow();
      headerRow.Append(
          CriarCelula("Parcela", true),
          CriarCelula("Vencimento", true),
          CriarCelula("Valor Líquido", true)
      );
      table.Append(headerRow);

      // Dados
      foreach (var parcela in parcelas)
      {
        decimal valorLiquido = Convert.ToDecimal(parcela["vl_saldo_titulo"] ??
                                                  parcela["vl_titulo"] ?? 0);
        DateTime dtVencimento = Convert.ToDateTime(parcela["dt_vcto_titulo"]);
        int numParcela = Convert.ToInt32(parcela["nm_parcela"] ?? 0);

        var dataRow = new TableRow();
        dataRow.Append(
            CriarCelula(numParcela.ToString()),
            CriarCelula(dtVencimento.ToString("dd/MM/yyyy")),
            CriarCelula($"R$ {valorLiquido.ToString("N2")}")
        );
        table.Append(dataRow);
      }

      return table;
    }


    /// <summary>
    /// Obtém os descontos aplicados ao contrato (não confundir com descontos de antecipação)
    /// Busca em T_DESCONTO_CONTRATO
    /// </summary>
    private async Task<List<Dictionary<string, object>>> ObterDescontosContrato(
        Source source,
        int cdContrato,
        int cdPessoaEscola)
    {
      try
      {



        // Query baseada no sistema legado (RelatorioController.cs linha ~68)
        var query = @"
      SELECT
        dc.cd_desconto_contrato,
        dc.cd_contrato,
        dc.cd_aditamento,
        dc.dc_desconto_contrato,
        dc.pc_desconto_contrato,
        dc.vl_desconto_contrato,
        dc.nm_parcela_ini,
        dc.nm_parcela_fim,
        dc.id_desconto_ativo,
        dc.id_incide_baixa
      FROM T_DESCONTO_CONTRATO dc
      INNER JOIN T_CONTRATO c ON dc.cd_contrato = c.cd_contrato
      WHERE dc.cd_contrato = @cdContrato
        AND c.cd_pessoa_escola = @cdPessoaEscola
        AND dc.id_desconto_ativo = 1
      ORDER BY dc.nm_parcela_ini";

        var parameters = new Dictionary<string, object>
    {
      { "@cdContrato", cdContrato },
      { "@cdPessoaEscola", cdPessoaEscola }
    };

        var result = await SQLServerService.ExecuteQuery(source, query, parameters);

        if (!result.Success)
        {

          return new List<Dictionary<string, object>>();
        }

        return result.Data ?? new List<Dictionary<string, object>>();
      }
      catch (Exception ex)
      {
        Console.WriteLine($"[ObterDescontosContrato] Erro: {ex.Message}");
        return new List<Dictionary<string, object>>();
      }
    }
    /// <summary>
    /// Calcula os valores líquidos das parcelas aplicando os descontos
    /// Retorna as parcelas com vl_saldo_titulo (valor líquido após descontos)
    /// </summary>
    private async Task<List<Dictionary<string, object>>> CalcularValoresLiquidos(
        Source source,
        List<Dictionary<string, object>> parcelasTitulos,
        int cdContrato,
        int cdPessoaEscola)
    {
      try
      {

        if (parcelasTitulos == null || !parcelasTitulos.Any())
          return new List<Dictionary<string, object>>();

        var parcelasComDesconto = new List<Dictionary<string, object>>();

        // Buscar descontos do contrato
        var descontosContrato = await ObterDescontosContrato(source, cdContrato, cdPessoaEscola);

        // Consolidar parcelas por vencimento
        var parcelasConsolidadas = ConsolidarTitulosPorVencimento(parcelasTitulos);

        foreach (var parcela in parcelasConsolidadas)
        {
          var parcelaComDesconto = new Dictionary<string, object>(parcela);

          decimal vlMaterial = parcela["vl_material"] == null || parcela["vl_material"] == DBNull.Value
       ? 0m : Convert.ToDecimal(parcela["vl_material"]);
          decimal vlMensalidade = parcela["vl_mensalidade"] == null || parcela["vl_mensalidade"] == DBNull.Value
              ? 0m : Convert.ToDecimal(parcela["vl_mensalidade"]);
          decimal vlTotal = vlMaterial + vlMensalidade;

          // Número da parcela
          int nmParcela = parcela["nm_parcela"] == null || parcela["nm_parcela"] == DBNull.Value
              ? 0 : Convert.ToInt32(parcela["nm_parcela"]);

          // Aplicar descontos que incidem nesta parcela
          decimal valorDesconto = 0;

          foreach (var desconto in descontosContrato)
          {
            int parcelaIni = desconto["nm_parcela_ini"] == null || desconto["nm_parcela_ini"] == DBNull.Value
                ? 0 : Convert.ToInt32(desconto["nm_parcela_ini"]);
            int parcelaFim = desconto["nm_parcela_fim"] == null || desconto["nm_parcela_fim"] == DBNull.Value
                ? 999 : Convert.ToInt32(desconto["nm_parcela_fim"]);

            // Verificar se o desconto se aplica a esta parcela
            if (nmParcela >= parcelaIni && nmParcela <= parcelaFim)
            {
              decimal pcDesconto = desconto["pc_desconto_contrato"] == null || desconto["pc_desconto_contrato"] == DBNull.Value
                  ? 0m : Convert.ToDecimal(desconto["pc_desconto_contrato"]);
              decimal vlDesconto = desconto["vl_desconto_contrato"] == null || desconto["vl_desconto_contrato"] == DBNull.Value
                  ? 0m : Convert.ToDecimal(desconto["vl_desconto_contrato"]);
              // Aplicar o desconto (percentual ou valor fixo)
              if (pcDesconto > 0)
              {
                valorDesconto += vlTotal * (pcDesconto / 100);
              }
              else if (vlDesconto > 0)
              {
                valorDesconto += vlDesconto;
              }
            }
          }

          // Calcular valor líquido
          decimal vlLiquido = vlTotal - valorDesconto;
          if (vlLiquido < 0) vlLiquido = 0;

          // Adicionar campos calculados
          parcelaComDesconto["vl_desconto_aplicado"] = valorDesconto;
          parcelaComDesconto["vl_saldo_titulo"] = vlLiquido;  // Campo usado pela grade
          parcelaComDesconto["vl_liquido"] = vlLiquido;

          parcelasComDesconto.Add(parcelaComDesconto);
        }


        return parcelasComDesconto;
      }
      catch (Exception ex)
      {
        Console.WriteLine($"[CalcularValoresLiquidos] Erro: {ex.Message}");
        return parcelasTitulos; // Retorna originais em caso de erro
      }
    }

  }
}

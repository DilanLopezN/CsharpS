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
      SimulacaoBaixaService simulacaoBaixaService)
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
        string nomeContrato = await DeterminarNomeTemplate(matriculaExists, cd_pessoa_escola, source);
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

        var endereco_responsavel = await SQLServerService.GetFirstByFields(source, "T_ENDERECO", new List<(string campo, object valor)> { new("cd_pessoa", cd_responsavel) });
        var enderecoResponsavel = "";
        if (endereco_responsavel != null)
        {
          if (endereco_responsavel.ContainsKey("cd_loc_logradouro") && endereco_responsavel["cd_loc_logradouro"] != null)
          {
            var filtroLogradouro = new List<(string campo, object valor)> { new("cd_localidade", endereco_responsavel["cd_loc_logradouro"].ToString()) };
            var logradouroExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtroLogradouro);
            if (logradouroExists != null && logradouroExists.ContainsKey("no_localidade"))
            {
              enderecoResponsavel = $"{logradouroExists["no_localidade"]?.ToString() ?? ""} ";
            }
          }

          var numEndereco = endereco_responsavel["dc_num_endereco"]?.ToString() ?? "";
          if (!String.IsNullOrEmpty(numEndereco))
            enderecoResponsavel += " Nº " + numEndereco;

          var complEndereco = endereco_responsavel["dc_compl_endereco"]?.ToString() ?? "";
          if (!String.IsNullOrEmpty(complEndereco))
            enderecoResponsavel += " / " + complEndereco;

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

        if (alunoTurma != null && alunoTurma.ContainsKey("cd_turma") && alunoTurma["cd_turma"] != null)
        {
          var horariosResult = await SQLServerService.GetList("vi_horario_turma", null, "[cd_turma]", $"[{alunoTurma["cd_turma"]}]", source, SearchModeEnum.Equals);

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
        if (matriculaExists.ContainsKey("vl_matricula_contrato") && matriculaExists["vl_matricula_contrato"] != null)
        {
          decimal vlMatriculaContrato = Convert.ToDecimal(matriculaExists["vl_matricula_contrato"]);
          matriculaRematricula = string.Format("{0:#,0.00}", vlMatriculaContrato);
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
      { "«ParcelaLiquida»", $"R$ {decimal.Parse(matriculaExists["vl_parcela_liquida"]?.ToString() ?? "0").ToString("N2")}" },
      { "«NroParcelas»", matriculaExists["nm_parcelas_mensalidade"]?.ToString() ?? "" },
      { "«NroParcelasTotal»", nroParcelas },
      { "«NroParcelasCurso»", matriculaExists["nm_parcelas_mensalidade"]?.ToString() ?? "" },
      { "«NroParcelasMaterial»", matriculaExists["nm_parcelas_material"]?.ToString() ?? "" },
      { "«NroVencimento»", matriculaExists["nm_dia_vcto"]?.ToString() ?? "" },
      { "«NroVencimentoComDesconto»", nroVencimentoComDesconto },
      { "«VencimentosTitulosComDesc»", vencimentosTitulosComDesc },
      { "«VencimentosTitulosSemDesc»", vencimentosTitulosSemDesc },
      { "«TipoAdiantamento»", aditamento_nmPrevisaoInicial?["id_tipo_aditamento"]?.ToString() ?? "" },
      { "«NroPrevisaoDias»", aditamento_nmPrevisaoInicial?["nm_previsao_inicial"]?.ToString() ?? "" },
      { "«Observacao»", aditamento_nmPrevisaoInicial?["tx_obs_aditamento"]?.ToString() ?? "" },
      { "«NumeroContrato»", matriculaExists["nm_contrato"]?.ToString() ?? "" },
      { "«OpcoesPagamento»", tipoFinanceiro?["dc_tipo_financeiro"]?.ToString() ?? "" },
      { "«TipoFinanceiroTaxa»", tipoFinanceiro?["dc_tipo_financeiro"]?.ToString() ?? "" },
      { "«TipoMatricula»", matriculaExists["id_tipo_matricula"]?.ToString() ?? "" },
      { "«Modalidade»", regime?["no_regime"]?.ToString() ?? "" },
      { "«BolsaMaterial»", decimal.Parse(matriculaExists["vl_material_contrato"]?.ToString() ?? "0").ToString("N2") },
      { "«GradeCursos»", "" },
      { "«GradeValoresParcelas»", "" },
      { "«GradeDescontosAntecip»", "" },
      { "«GradeDescontosContrato»", "" },
      { "«GradeValoresLiquidos»", "" }
    };

        Console.WriteLine("Replacements para o contrato:", replacements);
        var (success, arquivo, erro) = GerarContrato(nomeContrato, replacements, cdPessoaEscola);

        if (!success)
        {
          throw new Exception(erro);
        }

        return (arquivo, nomeContrato);
      }
      catch (Exception ex)
      {
        Console.WriteLine("[GerarContratoMatriculaError]: " + ex);
        throw;
      }
    }

    private (bool success, MemoryStream? arquivo, string? erro) GerarContrato(string nomeContrato, Dictionary<string, string> replacements, int? cd_pessoa_escola = null)
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



    // OPCIONAL: Método auxiliar para listar todos os templates disponíveis (útil para debug)
    private List<string> ListarTemplatesDisponiveis()
    {
      try
      {
        string webRootPath = _webHostEnvironment.WebRootPath;
        string caminhoPastaBase = Path.Combine(webRootPath, "Contratos");

        if (!Directory.Exists(caminhoPastaBase))
        {
          return new List<string>();
        }

        var templates = Directory.GetFiles(caminhoPastaBase, "*.*", SearchOption.AllDirectories)
          .Where(f => f.EndsWith(".docx", StringComparison.OrdinalIgnoreCase) ||
                      f.EndsWith(".dotx", StringComparison.OrdinalIgnoreCase))
          .Select(f => Path.GetRelativePath(caminhoPastaBase, f))
          .ToList();

        Console.WriteLine($"[INFO] Templates disponíveis: {templates.Count}");
        foreach (var template in templates)
        {
          Console.WriteLine($"  - {template}");
        }

        return templates;
      }
      catch (Exception ex)
      {
        Console.WriteLine($"[ERRO] Erro ao listar templates: {ex.Message}");
        return new List<string>();
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
          Console.WriteLine($"[INFO] Buscando template pelo cd_nome_contrato: {cd_nome_contrato}");

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
              Console.WriteLine($"[SUCESSO] Template encontrado: {nomeRelatorio}");
              return nomeRelatorio;
            }
          }

          Console.WriteLine($"[AVISO] cd_nome_contrato {cd_nome_contrato} não possui no_relatorio válido");
        }
        else
        {
          Console.WriteLine("[AVISO] Contrato sem cd_nome_contrato definido. Iniciando busca por template padrão...");
        }

        // ESTRATÉGIA 2: Template padrão ativo da escola específica
        Console.WriteLine($"[INFO] Buscando template padrão ativo para escola {cd_pessoa_escola}...");

        var templatesPadraoEscola = await SQLServerService.GetList(
          "T_NOME_CONTRATO",
          null,
          "[cd_pessoa_escola],[id_nome_ativo]",
          $"[{cd_pessoa_escola}],[1]",
          source,
          SearchModeEnum.Equals
        );

        if (templatesPadraoEscola.success &&
            templatesPadraoEscola.data != null &&
            templatesPadraoEscola.data.Any())
        {
          // Priorizar templates com nomes que indicam serem padrão
          var templatesPrioritarios = templatesPadraoEscola.data
            .Where(t => t.ContainsKey("no_relatorio") &&
                        t["no_relatorio"] != null &&
                        t["no_relatorio"] != DBNull.Value)
            .OrderBy(t =>
            {
              var nome = t["no_contrato"]?.ToString()?.ToLower() ?? "";
              var relatorio = t["no_relatorio"]?.ToString()?.ToLower() ?? "";

              // Prioridade 1: Contém "padrão" ou "default"
              if (nome.Contains("padrão") || nome.Contains("padrao") ||
                  nome.Contains("default") || relatorio.Contains("padrao"))
                return 0;

              // Prioridade 2: Contém "contrato" genérico
              if (nome.Contains("contrato") && !nome.Contains("teste"))
                return 1;

              // Prioridade 3: Contém "estágio" ou "estagio" (mais comum)
              if (nome.Contains("estágio") || nome.Contains("estagio") ||
                  relatorio.Contains("estagio"))
                return 2;

              // Prioridade 4: Qualquer outro ativo
              return 3;
            })
            .ThenBy(t => Convert.ToInt32(t["cd_nome_contrato"])) // Mais antigo primeiro
            .ToList();

          if (templatesPrioritarios.Any())
          {
            var templateEscolhido = templatesPrioritarios.First();
            var nomeRelatorio = templateEscolhido["no_relatorio"].ToString();
            var nomeContrato = templateEscolhido["no_contrato"]?.ToString() ?? "N/A";

            Console.WriteLine($"[INFO] Template padrão da escola encontrado:");
            Console.WriteLine($"  - Nome: {nomeContrato}");
            Console.WriteLine($"  - Arquivo: {nomeRelatorio}");
            Console.WriteLine($"  - cd_nome_contrato: {templateEscolhido["cd_nome_contrato"]}");

            return nomeRelatorio;
          }
        }

        // ESTRATÉGIA 3: Buscar template com nome "Contrato_Padrao" da escola
        Console.WriteLine("[INFO] Buscando template com nome 'Contrato_Padrao' da escola...");

        var templateComNomePadrao = await SQLServerService.GetList(
          "T_NOME_CONTRATO",
          null,
          "[cd_pessoa_escola]",
          $"[{cd_pessoa_escola}]",
          source,
          SearchModeEnum.Equals
        );

        if (templateComNomePadrao.success &&
            templateComNomePadrao.data != null &&
            templateComNomePadrao.data.Any())
        {
          var padrao = templateComNomePadrao.data.FirstOrDefault(t =>
          {
            var noContrato = t["no_contrato"]?.ToString()?.ToLower() ?? "";
            var noRelatorio = t["no_relatorio"]?.ToString()?.ToLower() ?? "";
            return noContrato.Contains("padrao") ||
                   noContrato.Contains("padrão") ||
                   noRelatorio.Contains("padrao");
          });

          if (padrao != null &&
              padrao.ContainsKey("no_relatorio") &&
              padrao["no_relatorio"] != null)
          {
            var nomeRelatorio = padrao["no_relatorio"].ToString();
            Console.WriteLine($"[INFO] Template 'Contrato_Padrao' encontrado: {nomeRelatorio}");
            return nomeRelatorio;
          }
        }

        // ESTRATÉGIA 4: Último recurso - nome fixo "Contrato_Padrao"
        Console.WriteLine("[AVISO] Nenhum template específico encontrado.");
        Console.WriteLine("[INFO] Usando nome de arquivo padrão: Contrato_Padrao");
        Console.WriteLine("[INFO] O sistema buscará recursivamente o arquivo Contrato_Padrao.dotx na pasta da escola");

        return "Contrato_Padrao";
      }
      catch (Exception ex)
      {
        Console.WriteLine($"[ERRO] Erro ao determinar template: {ex.Message}");
        Console.WriteLine("[INFO] Usando fallback: Contrato_Padrao");
        return "Contrato_Padrao";
      }
    }

    /// <summary>
    /// Método auxiliar para listar templates disponíveis de uma escola (útil para debug)
    /// </summary>
    private async Task<List<Dictionary<string, object>>> ListarTemplatesEscola(int cd_pessoa_escola, Source source)
    {
      try
      {
        var result = await SQLServerService.GetList(
          "T_NOME_CONTRATO",
          null,
          "[cd_pessoa_escola]",
          $"[{cd_pessoa_escola}]",
          source,
          SearchModeEnum.Equals
        );

        if (result.success && result.data != null)
        {
          Console.WriteLine($"\n=== TEMPLATES DISPONÍVEIS PARA ESCOLA {cd_pessoa_escola} ===");
          Console.WriteLine($"Total: {result.data.Count}");
          Console.WriteLine($"Ativos: {result.data.Count(t => Convert.ToInt32(t["id_nome_ativo"] ?? 0) == 1)}");
          Console.WriteLine($"Inativos: {result.data.Count(t => Convert.ToInt32(t["id_nome_ativo"] ?? 0) == 0)}");

          Console.WriteLine("\nTemplates Ativos:");
          foreach (var template in result.data.Where(t => Convert.ToInt32(t["id_nome_ativo"] ?? 0) == 1))
          {
            Console.WriteLine($"  [{template["cd_nome_contrato"]}] {template["no_contrato"]} → {template["no_relatorio"]}");
          }

          return result.data;
        }

        return new List<Dictionary<string, object>>();
      }
      catch (Exception ex)
      {
        Console.WriteLine($"[ERRO] Erro ao listar templates: {ex.Message}");
        return new List<Dictionary<string, object>>();
      }
    }

    private string GetDiaSemanaPorDia(string dia)
    {
      return dia.Trim() switch
      {
        "1" => "Segunda",
        "2" => "Terça",
        "3" => "Quarta",
        "4" => "Quinta",
        "5" => "Sexta",
        "6" => "Sábado",
        "7" => "Domingo",
        _ => dia
      };
    }

  }
}

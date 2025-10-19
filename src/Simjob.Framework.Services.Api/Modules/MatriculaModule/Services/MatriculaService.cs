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

    public async Task<(MemoryStream arquivo, string nomeContrato)> GerarContratoMatricula(int cdContrato)
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


        {
          //valida se matricula existe
          var matriculaExists = await SQLServerService.GetFirstByFields(source, "T_CONTRATO", new List<(string campo, object valor)> { new("cd_contrato", cdContrato) });
          if (matriculaExists == null) throw new NotFoundException("Matrícula não encontrada.");
          var cd_nome_contrato = matriculaExists["cd_nome_contrato"];
          if (cd_nome_contrato == null) throw new BadRequestException("Contrato não possui modelo de contrato definido.");
          //pegar nome do contrato
          var nome_contrato = await SQLServerService.GetFirstByFields(source, "T_NOME_CONTRATO", new List<(string campo, object valor)> { new("cd_nome_contrato", cd_nome_contrato) });
          if (nome_contrato == null) throw new NotFoundException("nome contrato não encontrado");

          var nomeContrato = nome_contrato["no_relatorio"]?.ToString();

          if (string.IsNullOrWhiteSpace(nomeContrato))
          {
            throw new BadRequestException("O campo 'no_relatorio' do contrato está vazio ou nulo.");
          }

          var cd_pessoa_escola = matriculaExists["cd_pessoa_escola"];

          #region ESCOLA
          //ESCOLA
          var pessoa_escola = await SQLServerService.GetFirstByFields(source, "T_PESSOA", new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa_escola) });
          var nomeEscola = pessoa_escola["dc_reduzido_pessoa"]?.ToString() ?? "";
          var razaoSocialEscola = pessoa_escola["no_pessoa"]?.ToString() ?? "";
          var pessoa_escola_juridica = await SQLServerService.GetFirstByFields(source, "T_PESSOA_JURIDICA", new List<(string campo, object valor)> { new("cd_pessoa_juridica", cd_pessoa_escola) });
          var cnpjEscola = pessoa_escola_juridica != null ? pessoa_escola_juridica["dc_num_cgc"]?.ToString() ?? pessoa_escola_juridica["dc_num_cgc"]?.ToString() ?? "" : "";
          var endereco_escola = await SQLServerService.GetFirstByFields(source, "T_ENDERECO", new List<(string campo, object valor)> { new("cd_pessoa", cd_pessoa_escola) });
          var enderecoEscolaMontado = "";
          var cidadeEstadoEscola = "";
          if (endereco_escola != null)
          {
            var filtroLogradouro = new List<(string campo, object valor)> { new("cd_localidade", endereco_escola["cd_loc_logradouro"].ToString()) };
            var logradouroExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtroLogradouro);
            if (logradouroExists != null)
            {
              enderecoEscolaMontado = $"{logradouroExists["no_localidade"]},{endereco_escola["dc_num_endereco"]}";
            }

            var filtroEstado = new List<(string campo, object valor)> { new("cd_localidade", endereco_escola["cd_loc_estado"].ToString()) };
            var estadoExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtroEstado);
            if (estadoExists != null)
            {

              var filtroCidade = new List<(string campo, object valor)> { new("cd_localidade", endereco_escola["cd_loc_cidade"].ToString()) };
              var cidadeExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtroCidade);
              if (cidadeExists != null)
              {
                cidadeEstadoEscola = $"{cidadeExists["no_localidade"]}/{estadoExists["no_localidade"]}";
              }

            }


          }
          #endregion

          #region RESPONSAVEL
          // RESPONSAVEL
          var cd_responsavel = matriculaExists["cd_pessoa_responsavel"];
          var pessoa_responsavel = await SQLServerService.GetFirstByFields(source, "T_PESSOA", new List<(string campo, object valor)> { new("cd_pessoa", cd_responsavel) });
          var nomeResponsavel = pessoa_responsavel["no_pessoa"]?.ToString() ?? "";
          var pessoa_responsavel_fisica = await SQLServerService.GetFirstByFields(source, "T_PESSOA_FISICA", new List<(string campo, object valor)> { new("cd_pessoa_fisica", cd_responsavel) });
          var rg_pessoa_responsavel = pessoa_responsavel_fisica != null ? pessoa_responsavel_fisica["nm_doc_identidade"]?.ToString() ?? "" : "";
          var cpfResponsavel = pessoa_responsavel_fisica != null ? pessoa_responsavel_fisica["nm_cpf"]?.ToString() ?? "" : "";
          var tituloRGResponsavel = pessoa_responsavel["nm_natureza_pessoa"]?.ToString() == "1" ? "RG" : "";
          var tituloCPFouCNPJResponsavel = pessoa_responsavel["nm_natureza_pessoa"]?.ToString() == "1" ? "CPF" : "CNPJ";
          var telefoneResponsavel = "";
          var telefone_responsavel = await SQLServerService.GetFirstByFields(source, "T_TELEFONE", new List<(string campo, object valor)> { new("cd_pessoa", cd_responsavel), new("cd_tipo_telefone", 1) });
          if (telefone_responsavel != null)
          {
            telefoneResponsavel = telefone_responsavel["dc_fone_mail"]?.ToString() ?? "";
          }
          var email_responsavel = await SQLServerService.GetFirstByFields(source, "T_TELEFONE", new List<(string campo, object valor)> { new("cd_pessoa", cd_responsavel), new("cd_tipo_telefone", 4) });
          var emailResponsavel = email_responsavel == null ? "" : email_responsavel["dc_fone_mail"]?.ToString() ?? "";

          var celular_responsavel = await SQLServerService.GetFirstByFields(source, "T_TELEFONE", new List<(string campo, object valor)> { new("cd_pessoa", cd_responsavel), new("cd_tipo_telefone", 3) });
          var celularResponsavel = celular_responsavel == null ? "" : celular_responsavel["dc_fone_mail"]?.ToString() ?? "";

          var dataNascResponsavel = "";
          if (pessoa_responsavel_fisica != null && pessoa_responsavel_fisica["dt_nascimento"] != null)
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
            var filtroLogradouro = new List<(string campo, object valor)> { new("cd_localidade", endereco_responsavel["cd_loc_logradouro"].ToString()) };
            var logradouroExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtroLogradouro);
            if (logradouroExists != null)
            {
              enderecoResponsavel = $"{logradouroExists["no_localidade"]} ";
            }
            if (!String.IsNullOrEmpty(endereco_responsavel["dc_num_endereco"].ToString()))
              enderecoResponsavel += " Nº " + endereco_responsavel["dc_num_endereco"];
            if (!String.IsNullOrEmpty(endereco_responsavel["dc_compl_endereco"]?.ToString() ?? ""))
              enderecoResponsavel += " / " + endereco_responsavel["dc_compl_endereco"];
            if (!String.IsNullOrEmpty(endereco_responsavel["dc_num_cep"].ToString()))
              enderecoResponsavel += ", CEP: " + endereco_responsavel["dc_num_cep"];
            if (!String.IsNullOrEmpty(endereco_responsavel["cd_loc_bairro"].ToString()))
            {

              var filtroBairro = new List<(string campo, object valor)> { new("cd_localidade", endereco_responsavel["cd_loc_bairro"].ToString()) };
              var bairroExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtroBairro);
              if (bairroExists != null)
              {
                enderecoResponsavel += ", Bairro: " + bairroExists["no_localidade"];
              }
            }
            if (!String.IsNullOrEmpty(endereco_responsavel["cd_loc_cidade"].ToString()))
            {

              var filtroCidade = new List<(string campo, object valor)> { new("cd_localidade", endereco_responsavel["cd_loc_cidade"].ToString()) };
              var cidadeExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtroCidade);
              if (cidadeExists != null)
              {
                enderecoResponsavel += ", Cidade: " + cidadeExists["no_localidade"];
              }
            }
            if (!String.IsNullOrEmpty(endereco_responsavel["cd_loc_estado"].ToString()))
            {

              var filtroEstado = new List<(string campo, object valor)> { new("cd_localidade", endereco_responsavel["cd_loc_estado"].ToString()) };
              var estadoExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtroEstado);
              if (estadoExists != null)
              {
                enderecoResponsavel += " - " + estadoExists["no_localidade"];
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
          var rg_pessoa_aluno = pessoa_aluno_fisica != null ? pessoa_aluno_fisica["nm_doc_identidade"]?.ToString() ?? "" : "";
          var cpfAluno = pessoa_aluno_fisica != null ? pessoa_aluno_fisica["nm_cpf"]?.ToString() ?? "" : "";

          var estadoCivilAluno = "";
          var estado_civil_aluno = await SQLServerService.GetFirstByFields(source, "T_ESTADO_CIVIL", new List<(string campo, object valor)> { new("cd_estado_civil", pessoa_aluno_fisica != null ? pessoa_aluno_fisica["cd_estado_civil"]?.ToString() ?? "" : "") });
          if (estado_civil_aluno != null)
          {
            estadoCivilAluno = estado_civil_aluno["dc_estado_civil_masc"]?.ToString() ?? "";
          }
          var sexoAluno = pessoa_aluno_fisica["nm_sexo"];
          var sexoFAluno = sexoAluno != null && sexoAluno.ToString() == "1" ? "X" : "";
          var sexoMAluno = sexoAluno != null && sexoAluno.ToString() == "2" ? "X" : "";

          var dataNascimentoAluno = "";
          if (pessoa_aluno_fisica != null && pessoa_aluno_fisica["dt_nascimento"] != null)
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
            var filtroLogradouro = new List<(string campo, object valor)> { new("cd_localidade", endereco_aluno["cd_loc_logradouro"].ToString()) };
            var logradouroExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtroLogradouro);
            if (logradouroExists != null)
            {
              enderecoAluno = $"{logradouroExists["no_localidade"]} ";
            }
            if (!String.IsNullOrEmpty(endereco_aluno["dc_num_endereco"].ToString()))
              enderecoAluno += " Nº " + endereco_aluno["dc_num_endereco"];
            if (!String.IsNullOrEmpty(endereco_aluno["dc_compl_endereco"]?.ToString() ?? ""))
              enderecoAluno += " / " + endereco_aluno["dc_compl_endereco"];
            if (!String.IsNullOrEmpty(endereco_aluno["dc_num_cep"].ToString()))
              enderecoAluno += ", CEP: " + endereco_aluno["dc_num_cep"];
            if (!String.IsNullOrEmpty(endereco_aluno["cd_loc_bairro"].ToString()))
            {

              var filtroBairro = new List<(string campo, object valor)> { new("cd_localidade", endereco_aluno["cd_loc_bairro"].ToString()) };
              var bairroExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtroBairro);
              if (bairroExists != null)
              {
                enderecoAluno += ", Bairro: " + bairroExists["no_localidade"];
              }
            }
            if (!String.IsNullOrEmpty(endereco_aluno["cd_loc_cidade"].ToString()))
            {

              var filtroCidade = new List<(string campo, object valor)> { new("cd_localidade", endereco_aluno["cd_loc_cidade"].ToString()) };
              var cidadeExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtroCidade);
              if (cidadeExists != null)
              {
                enderecoAluno += ", Cidade: " + cidadeExists["no_localidade"];
              }
            }
            if (!String.IsNullOrEmpty(endereco_aluno["cd_loc_estado"].ToString()))
            {

              var filtroEstado = new List<(string campo, object valor)> { new("cd_localidade", endereco_aluno["cd_loc_estado"].ToString()) };
              var estadoExists = await SQLServerService.GetFirstByFields(source, "T_LOCALIDADE", filtroEstado);
              if (estadoExists != null)
              {
                enderecoAluno += " - " + estadoExists["no_localidade"];
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

          // Buscar cd_turma através de T_ALUNO_TURMA usando cd_aluno
          List<Dictionary<string, object>> horariosData = new List<Dictionary<string, object>>();
          var filtroAlunoTurma = new List<(string campo, object valor)> { new("cd_aluno", cd_aluno) };
          var alunoTurma = await SQLServerService.GetFirstByFields(source, "T_ALUNO_TURMA", filtroAlunoTurma);

          if (alunoTurma != null && alunoTurma["cd_turma"] != null)
          {
            // Buscar horários da turma usando vi_horario_turma
            var horariosResult = await SQLServerService.GetList("vi_horario_turma", null, "[cd_turma]", $"[{alunoTurma["cd_turma"]}]", source, SearchModeEnum.Equals);

            if (horariosResult.success && horariosResult.data != null && horariosResult.data.Any())
            {
              horariosData = horariosResult.data;

              var diasList = horariosData
                  .Select(h => diasSemana.TryGetValue(Convert.ToInt32(h["id_dia_semana"]), out var dia) ? dia : "")
                  .Where(d => !string.IsNullOrEmpty(d))
                  .Distinct();

              diasMontado = string.Join(", ", diasList);

              var horariosList = horariosData
                  .Select(h => $"{h["dt_hora_ini"]?.ToString() ?? ""} às {h["dt_hora_fim"]?.ToString() ?? ""}")
                  .Where(t => !string.IsNullOrEmpty(t))
                  .Distinct();
              horarioMontado = string.Join(", ", horariosList);
            }
          }

          var dataInicioAula = "";
          if (matriculaExists["dt_inicial_contrato"] != null)
          {
            if (DateTime.TryParse(matriculaExists["dt_inicial_contrato"].ToString(), out DateTime dt_inicio))
            {
              dataInicioAula = dt_inicio.ToString("dd/MM/yyyy");
            }
          }
          var dataFimAula = "";
          if (matriculaExists["dt_final_contrato"] != null)
          {
            if (DateTime.TryParse(matriculaExists["dt_final_contrato"].ToString(), out DateTime dt_fim))
            {
              dataFimAula = dt_fim.ToString("dd/MM/yyyy");
            }
          }

          var matriculaRematricula = "";
          if (matriculaExists["vl_matricula_contrato"] != null)
          {
            decimal vlMatriculaContrato = Convert.ToDecimal(matriculaExists["vl_matricula_contrato"]);
            matriculaRematricula = string.Format("{0:#,0.00}", vlMatriculaContrato);
          }

          // Calcular ValorSemDesconto conforme regra de negócio
          decimal vlMaterialMatricula = 0;
          decimal vlSemDesconto = Convert.ToDecimal(matriculaExists["vl_curso_contrato"]) / Convert.ToDecimal(matriculaExists["nm_parcelas_mensalidade"]);
          byte nm_parcelas_material = 0;

          if (Convert.ToInt32(matriculaExists["nm_parcelas_material"] ?? 0) > 0)
          {
            nm_parcelas_material = (byte)Convert.ToInt32(matriculaExists["nm_parcelas_material"]);
            vlMaterialMatricula = Convert.ToDecimal(matriculaExists["vl_material_contrato"] ?? 0);
            if (nm_parcelas_material > 0) //Evitar divisão por zero
              vlSemDesconto = vlSemDesconto + vlMaterialMatricula / nm_parcelas_material;
          }

          #region Valor Com Desconto
          nm_parcelas_material = 0;
          string valor_com_desconto = "";
          vlMaterialMatricula = 0;

          // Buscar títulos abertos do contrato
          var titulosAbertos = await SQLServerService.GetList("T_TITULO", null, "[cd_origem_titulo],[id_status_titulo]", $"[{cdContrato}],[1]", source, SearchModeEnum.Equals);
          var statusCnabTitulo = new List<int> { 0, 1 }; // Status CNAB válidos

          // Buscar aditamentos
          var aditamentos_result = await SQLServerService.GetList("T_ADITAMENTO", null, "[cd_contrato]", $"[{cdContrato}]", source, SearchModeEnum.Equals);
          var aditamentos = aditamentos_result.success ? aditamentos_result.data : new List<Dictionary<string, object>>();
          if (Convert.ToDecimal(matriculaExists["vl_parcela_contrato"] ?? 0) > 0)
          {

            decimal valorbaixaDesc = 0;

            if (!titulosAbertos.success || titulosAbertos.data.Count == 0)
            {
              // Caso quando não há títulos abertos - simular baixa do contrato
              var parametrosEscola = await BuscarParametrosEscola(Convert.ToInt32(matriculaExists["cd_pessoa_escola"]), source);
              if (parametrosEscola != null)
              {
                // Criar um título simulado para cálculo
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
              // Filtrar títulos conforme lógica de aditamento
              var titulosAbertosLista = titulosAbertos.data;
              var aditamento = aditamentos.OrderByDescending(a => Convert.ToDateTime(a["dt_aditamento"])).FirstOrDefault();

              Dictionary<string, object> tituloParaCalculo = null;

              if (aditamento == null || aditamentos.Count <= 0)
              {
                // Sem aditamento - pegar primeiro título aberto
                tituloParaCalculo = titulosAbertosLista.Where(x =>
                    Convert.ToInt32(x["id_status_titulo"]) == 1 &&
                    statusCnabTitulo.Contains(Convert.ToInt32(x["id_status_cnab"] ?? 0)) &&
                    x["dc_tipo_titulo"]?.ToString() == "ME"
                ).OrderBy(x => Convert.ToInt32(x["nm_parcela_titulo"] ?? 0)).FirstOrDefault();
              }
              else
              {
                var tipoAditamento = Convert.ToByte(aditamento["id_tipo_aditamento"] ?? 0);

                if (tipoAditamento != 3) // Não é "Adicionar Parcelas"
                {
                  tituloParaCalculo = titulosAbertosLista.Where(x =>
                      Convert.ToInt32(x["id_status_titulo"]) == 1 &&
                      statusCnabTitulo.Contains(Convert.ToInt32(x["id_status_cnab"] ?? 0)) &&
                      x["dc_tipo_titulo"]?.ToString() == "ME"
                  ).OrderBy(x => Convert.ToInt32(x["nm_parcela_titulo"] ?? 0)).FirstOrDefault();
                }
                else // É "Adicionar Parcelas"
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
                // Simular baixa do título encontrado
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

          // Calcular NroVencimentoComDesconto
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
          //
          List<string> listaProdutos = new List<string>();
          listaProdutos.Add("Inglês");
          listaProdutos.Add("Espanhol");
          var tituloCurso = listaProdutos.Contains(nomeProduto) ? "Estágio" : "Módulo";

          string complemento = "";
          if (nomeCurso.Contains("R60") || nomeCurso.Contains("R60"))
            complemento += "TURMA DE 60 MINUTOS";
          //qtd_minutos_turma não encontrato em T_CONTRATO
          //else if (contrato.qtd_minutos_turma > 0)
          //    complemento += "TURMA DE " + contrato.qtd_minutos_turma + " MINUTOS";

          //tipoFinanceiro
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

          // Gerar VencimentosTitulosComDesc - Exatamente como no RelatorioController
          string vencimentosTitulosComDesc = "";

          // Esse valor só irá aparecer caso haja algum desconto no contrato.
          decimal vlDescontoContrato = Convert.ToDecimal(matriculaExists["vl_desconto_contrato"] ?? 0);
          if (vlDescontoContrato > 0)
          {
            // Buscar último aditamento
            var ultimoAditamento = aditamentos?.OrderBy(a => a["dt_aditamento"]).LastOrDefault();

            // Caso o contrato não seja de aditamento concatenar o vencimento de todos os títulos da matrícula
            if (ultimoAditamento == null || !aditamentos.Any())
            {
              // Concatenar vencimentos de todos os títulos abertos
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

              // Se for de aditamento com o tipo diferente de "Adicionar parcelas" (tipo 4)
              int? tipoAditamento = ultimoAditamento?["id_tipo_aditamento"] as int?;
              if (tipoAditamento.HasValue && tipoAditamento.Value != 4) // ADICIONAR_PARCELAS = 4
              {
                // Filtrar títulos excluindo tipos específicos de aditamento
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
              else // Caso seja aditamento com tipo "Adicionar Parcelas"
              {
                // Concatenar apenas títulos do tipo aditamento
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

          // Gerar VencimentosTitulosSemDesc - Exatamente como no RelatorioController
          string vencimentosTitulosSemDesc = "";

          // Esse valor só irá aparecer caso NÃO haja algum desconto no contrato.
          decimal vlDescontoContratoSemDesc = Convert.ToDecimal(matriculaExists["vl_desconto_contrato"] ?? 0);
          if (vlDescontoContratoSemDesc <= 0)
          {
            // Buscar último aditamento
            var ultimoAditamentoSemDesc = aditamentos?.OrderBy(a => a["dt_aditamento"]).LastOrDefault();

            // Caso o contrato não seja de aditamento concatenar o vencimento de todos os títulos da matrícula
            if (ultimoAditamentoSemDesc == null || !aditamentos.Any())
            {
              // Concatenar vencimentos de todos os títulos abertos
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

              // Se for de aditamento com o tipo diferente de "Adicionar parcelas" (tipo 4)
              int? tipoAditamentoSemDesc = ultimoAditamentoSemDesc?["id_tipo_aditamento"] as int?;
              if (tipoAditamentoSemDesc.HasValue && tipoAditamentoSemDesc.Value != 4) // ADICIONAR_PARCELAS = 4
              {
                // Filtrar títulos excluindo tipos específicos de aditamento
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
              else // Caso seja aditamento com tipo "Adicionar Parcelas"
              {
                // Concatenar apenas títulos do tipo aditamento
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

          // Gerar DiasHorariosCurso - Exatamente como no RelatorioController
          string diasHorariosCurso = "";

          // Usar os horários já buscados anteriormente
          if (horariosData != null && horariosData.Any())
          {
            diasHorariosCurso = GerarDescricaoCompletaHorarios(horariosData);
          }

          // Formatar datas de contrato
          var dataFimContrato = FormatarData(matriculaExists["dt_final_contrato"]);
          var dataMatriculaContrato = FormatarData(matriculaExists["dt_matricula_contrato"]);

          var replacements = new Dictionary<string, string>
        {
            // ESCOLA
            { "«NomeEscola»", nomeEscola },
            { "«RazaoSocial»", razaoSocialEscola },
            { "«CNPJEscola»", cnpjEscola },
            { "«EnderecoEscola»", enderecoEscolaMontado },
            { "«CidadeEstadoEscola»", cidadeEstadoEscola },

            // RESPONSÁVEL
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

            // ALUNO
            { "«NomeAluno»", nomeAluno },
            { "«TelelfoneAluno»", telefoneAluno },
            { "«TelefoneAluno»", telefoneAluno }, // Adicionar ambas as variações
            { "«RGAluno»", rg_pessoa_aluno },
            { "«CPFAluno»", cpfAluno },
            { "«EstadoCivilAluno»", estadoCivilAluno },
            { "«DataNascimentoAluno»", dataNascimentoAluno },
            { "«EnderecoAluno»", enderecoAluno },
            { "«EmailAluno»", emailAluno },
            { "«CelularAluno»", celularAluno },
            { "«SexoF»", sexoFAluno },
            { "«SexoM»", sexoMAluno },

            // CURSO/PRODUTO
            { "«TituloCurso»", tituloCurso },
            { "«Curso»", nomeCurso },
            { "«Produto»", nomeProduto },
            { "«NomeProduto»", nomeProduto },
            { "«ComplementoCursoComMinutosTurma»", complemento },

            // HORÁRIOS
            { "«DiasHorariosCurso»", diasHorariosCurso },
            { "«HorariosCurso»", diasHorariosCurso },
            { "«Dias»", diasMontado },
            { "«Horarios»", horarioMontado },
            { "«DuracaoAula»", duracaoAula },

            // DATAS
            { "«DataInicioAulas»", dataInicioAula },
            { "«DataFimTurma»", dataFimAula },
            { "«DataFimContrato»", dataFimContrato },
            { "«DataMatriculaContrato»", dataMatriculaContrato },
            { "«DataInicioAdt»", desc_data_aditamento },
            { "«DataInicioAdtExtenso»", dataInicioAdtExtenso },
            { "«AnoCorrente»", DateTime.Now.Year.ToString() },
            { "«DataCorrenteExtenso»", datacorrenteextenso },

            // VALORES FINANCEIROS
            { "«MatriculaRematricula»", $"R$ {matriculaRematricula}" },
            { "«ValorSemDesconto»", $"R$ {decimal.Round(vlSemDesconto, 2).ToString("N2")}" },
            { "«ValorComDesconto»", $"R$ {valor_com_desconto}" },
            { "«ValorCurso»", $"R$ {decimal.Parse(matriculaExists["vl_curso_contrato"]?.ToString() ?? "0").ToString("N2")}" },
            { "«ValorMaterial»", $"R$ {decimal.Parse(matriculaExists["vl_material_contrato"]?.ToString() ?? "0").ToString("N2")}" },
            { "«ValorComDescontoMaterial»", $"R$ {decimal.Parse(matriculaExists["vl_parcela_liq_material"]?.ToString() ?? "0").ToString("N2")}" },
            { "«ParcelaLiquida»", $"R$ {decimal.Parse(matriculaExists["vl_parcela_liquida"]?.ToString() ?? "0").ToString("N2")}" },

            // PARCELAS
            { "«NroParcelas»", matriculaExists["nm_parcelas_mensalidade"]?.ToString() ?? "" },
            { "«NroParcelasTotal»", nroParcelas },
            { "«NroParcelasCurso»", matriculaExists["nm_parcelas_mensalidade"]?.ToString() ?? "" },
            { "«NroParcelasMaterial»", matriculaExists["nm_parcelas_material"]?.ToString() ?? "" },

            // VENCIMENTOS
            { "«NroVencimento»", matriculaExists["nm_dia_vcto"]?.ToString() ?? "" },
            { "«NroVencimentoComDesconto»", nroVencimentoComDesconto },
            { "«VencimentosTitulosComDesc»", vencimentosTitulosComDesc },
            { "«VencimentosTitulosSemDesc»", vencimentosTitulosSemDesc },

            // ADITAMENTO
            { "«TipoAdiantamento»", aditamento_nmPrevisaoInicial?["id_tipo_aditamento"]?.ToString() ?? "" },
            { "«NroPrevisaoDias»", aditamento_nmPrevisaoInicial?["nm_previsao_inicial"]?.ToString() ?? "" },
            { "«Observacao»", aditamento_nmPrevisaoInicial?["tx_obs_aditamento"]?.ToString() ?? "" },

            // OUTROS
            { "«NumeroContrato»", matriculaExists["nm_contrato"]?.ToString() ?? "" },
            { "«OpcoesPagamento»", tipoFinanceiro?["dc_tipo_financeiro"]?.ToString() ?? "" },
            { "«TipoFinanceiroTaxa»", tipoFinanceiro?["dc_tipo_financeiro"]?.ToString() ?? "" },
            { "«TipoMatricula»", matriculaExists["id_tipo_matricula"]?.ToString() ?? "" },
            { "«Modalidade»", regime?["no_regime"]?.ToString() ?? "" },
            { "«BolsaMaterial»", decimal.Parse(matriculaExists["vl_material_contrato"]?.ToString() ?? "0").ToString("N2") },

            // GRADES (deixadas vazias conforme solicitado)
            { "«GradeCursos»", "" },
            { "«GradeValoresParcelas»", "" },
            { "«GradeDescontosAntecip»", "" },
            { "«GradeDescontosContrato»", "" },
            { "«GradeValoresLiquidos»", "" }
        };

          Console.WriteLine("Replacements para o contrato:", replacements);
          var (success, arquivo, erro) = GerarContrato(nomeContrato, replacements);

          if (!success)
          {
            throw new Exception(erro);
          }


          return (arquivo, nomeContrato);


        }

      }
      catch (Exception ex)
      {
        Console.WriteLine("[GerarContratoMatriculaError]: " + ex);
        throw;
      }

    }


    private (bool success, MemoryStream? arquivo, string? erro) GerarContrato(string nomeContrato, Dictionary<string, string> replacements)
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

        string caminhoPasta = Path.Combine(webRootPath, "Contratos");

        // Verifica se a pasta existe, se não existir, cria
        if (!Directory.Exists(caminhoPasta))
        {
          try
          {
            Directory.CreateDirectory(caminhoPasta);
            Console.WriteLine($"[INFO] Pasta de contratos criada: {caminhoPasta}");
          }
          catch (Exception ex)
          {
            return (false, null, $"Erro ao criar pasta de contratos: {ex.Message}");
          }
        }

        var path = Path.Combine(caminhoPasta, nomeContrato);

        // Adiciona extensão se não tiver
        if (!path.EndsWith(".docx", StringComparison.OrdinalIgnoreCase) &&
            !path.EndsWith(".dotx", StringComparison.OrdinalIgnoreCase))
        {
          // Tenta primeiro com .docx
          if (System.IO.File.Exists(path + ".docx"))
          {
            path += ".docx";
          }
          else if (System.IO.File.Exists(path + ".dotx"))
          {
            path += ".dotx";
          }
        }

        if (!System.IO.File.Exists(path))
        {
          return (false, null, $"Arquivo de contrato não encontrado: {path}");
        }

        Console.WriteLine($"Carregando template: {path}");

        // Carrega o template (DOTX ou DOCX)
        using (var doc = DocX.Load(path))
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

          Console.WriteLine($"Contrato gerado com sucesso. Tamanho: {memoryStream.Length} bytes");

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

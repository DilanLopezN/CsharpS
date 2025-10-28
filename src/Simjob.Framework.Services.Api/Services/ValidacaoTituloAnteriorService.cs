using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using Simjob.Framework.Services.Api.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simjob.Framework.Services.Api.Services
{
    public static class ValidacaoTituloAnteriorService
    {
        /// <summary>
        /// Valida se existem títulos anteriores em aberto antes de liquidar um título
        /// </summary>
        /// <param name="titulo">Dados do título que está sendo liquidado</param>
        /// <param name="cd_pessoa_empresa">Código da empresa</param>
        /// <param name="source">Fonte de dados</param>
        /// <param name="userId">ID do usuário logado no MongoDB (opcional - para verificar se é admin)</param>
        /// <param name="userService">Serviço de usuários do MongoDB (opcional - para verificar grupo admin)</param>
        /// <param name="groupService">Serviço de grupos do MongoDB (opcional - para verificar grupo admin)</param>
        /// <returns>Tupla com (sucesso, mensagemErro)</returns>
        public static async Task<(bool sucesso, string mensagemErro)> ValidarTituloAnteriorAberto(
            Dictionary<string, object> titulo,
            int cd_pessoa_empresa,
            Source source,
            string userId = null,
            IUserService userService = null,
            IGroupService groupService = null)
        {
            try
            {
                // Verificar se o usuário é admin da escola - admin pode burlar a validação
                if (!string.IsNullOrEmpty(userId) && userService != null && groupService != null)
                {
                    Console.WriteLine($"[ValidacaoTituloAnterior] Verificando admin para userId: {userId}");

                    // Buscar usuário no MongoDB pelo ID
                    var usuarioMongo = userService.GetUserById(userId);

                    if (usuarioMongo != null)
                    {
                        Console.WriteLine($"[ValidacaoTituloAnterior] Usuário encontrado. Id: {usuarioMongo.Id}, UserName: {usuarioMongo.UserName}, GroupId: {usuarioMongo.GroupId}");

                        if (!string.IsNullOrEmpty(usuarioMongo.GroupId))
                        {
                            // Buscar grupo do usuário
                            var grupo = groupService.GetGroupById(usuarioMongo.GroupId);

                            if (grupo != null)
                            {
                                Console.WriteLine($"[ValidacaoTituloAnterior] Grupo encontrado: {grupo.GroupName}, Cd_empresa: {grupo.Cd_empresa}");

                                // Verificar se é grupo "Administrador" ou "Administradores" e se pertence à mesma empresa
                                string groupNameUpper = grupo.GroupName?.ToUpper() ?? "";
                                bool isAdminGroup = groupNameUpper == "ADMINISTRADORES" || groupNameUpper == "ADMINISTRADOR";
                                bool isSameCompany = grupo.Cd_empresa?.ToString() == cd_pessoa_empresa.ToString();

                                Console.WriteLine($"[ValidacaoTituloAnterior] isAdminGroup: {isAdminGroup}, isSameCompany: {isSameCompany}");

                                if (isAdminGroup && isSameCompany)
                                {
                                    Console.WriteLine($"[ValidacaoTituloAnterior] BYPASS ATIVADO - Usuário é admin da escola");
                                    // Usuário é admin da escola - pode liquidar títulos sem restrição de ordem
                                    return (true, null);
                                }
                            }
                            else
                            {
                                Console.WriteLine($"[ValidacaoTituloAnterior] Grupo não encontrado para GroupId: {usuarioMongo.GroupId}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"[ValidacaoTituloAnterior] Usuário sem grupo associado");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"[ValidacaoTituloAnterior] Usuário não encontrado com userId: {userId}");
                    }
                }
                else
                {
                    Console.WriteLine($"[ValidacaoTituloAnterior] Parâmetros de admin não fornecidos. userId: {userId}, userService: {userService != null}, groupService: {groupService != null}");
                }

                // Buscar parâmetros da escola
                var parametros = await BuscarParametrosEscola(cd_pessoa_empresa, source);

                if (parametros == null || !parametros.ContainsKey("id_liquidacao_tit_ant_aberto"))
                {
                    // Se não encontrou o parâmetro, permite a operação
                    Console.WriteLine($"[ValidacaoTituloAnterior] Parâmetro não encontrado para empresa {cd_pessoa_empresa}");
                    return (true, null);
                }

                // Obter flag de bloqueio
                int bloqueioAtivo = Convert.ToInt32(parametros["id_liquidacao_tit_ant_aberto"]);
                Console.WriteLine($"[ValidacaoTituloAnterior] Empresa {cd_pessoa_empresa} - Bloqueio ativo: {bloqueioAtivo}");

                // Se for 0, o bloqueio está desabilitado
                if (bloqueioAtivo == 0)
                {
                    Console.WriteLine($"[ValidacaoTituloAnterior] Bloqueio desabilitado para empresa {cd_pessoa_empresa}");
                    return (true, null);
                }

                // Verificar se o título tem número de parcela
                if (!titulo.ContainsKey("nm_parcela_titulo") || titulo["nm_parcela_titulo"] == null)
                {
                    // Se não tem número de parcela, não aplica a regra
                    return (true, null);
                }

                int numeroParcela = Convert.ToInt32(titulo["nm_parcela_titulo"]);

                // Se é a primeira parcela, permite
                if (numeroParcela <= 1)
                {
                    return (true, null);
                }

                // Buscar o código do cliente/aluno (cd_pessoa_titulo)
                if (!titulo.ContainsKey("cd_pessoa_titulo") || titulo["cd_pessoa_titulo"] == null)
                {
                    // Se não tem pessoa do título, não consegue validar
                    return (true, null);
                }

                int cd_pessoa_titulo = Convert.ToInt32(titulo["cd_pessoa_titulo"]);

                // Obter o tipo de título para validar apenas parcelas do mesmo tipo
                string dc_tipo_titulo = titulo.ContainsKey("dc_tipo_titulo") && titulo["dc_tipo_titulo"] != null
                    ? titulo["dc_tipo_titulo"].ToString()
                    : null;

                // Verificar se existem parcelas anteriores em aberto para a mesma pessoa e mesmo tipo
                var parcelasAnteriores = await BuscarParcelasAnterioresEmAberto(
                    cd_pessoa_titulo,
                    cd_pessoa_empresa,
                    numeroParcela,
                    dc_tipo_titulo,
                    source);

                if (parcelasAnteriores != null && parcelasAnteriores.Any())
                {
                    var numeros = string.Join(", ", parcelasAnteriores.Select(p =>
                        p.ContainsKey("nm_parcela_titulo") ? p["nm_parcela_titulo"].ToString() : "?"));

                    string tipoMensagem = !string.IsNullOrEmpty(dc_tipo_titulo)
                        ? $" do tipo {dc_tipo_titulo}"
                        : "";

                    string mensagem = $"Não é possível liquidar a parcela {numeroParcela}{tipoMensagem} pois existem parcelas anteriores em aberto: {numeros}. " +
                                    $"Por favor, liquide as parcelas anteriores primeiro.";
                    return (false, mensagem);
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                // Em caso de erro na validação, registra mas permite a operação
                // para não bloquear o sistema por falha na validação
                Console.WriteLine($"Erro ao validar título anterior em aberto: {ex.Message}");
                return (true, null);
            }
        }

        private static async Task<Dictionary<string, object>> BuscarParametrosEscola(int cd_pessoa_empresa, Source source)
        {
            var filtroParametro = new List<(string campo, object valor)> { ("cd_pessoa_escola", cd_pessoa_empresa) };
            return await SQLServerService.GetFirstByFields(source, "T_PARAMETRO", filtroParametro);
        }

        private static async Task<List<Dictionary<string, object>>> BuscarParcelasAnterioresEmAberto(
            int cd_pessoa_titulo,
            int cd_pessoa_empresa,
            int numeroParcela,
            string dc_tipo_titulo,
            Source source)
        {
            try
            {
                // Buscar todos os títulos da pessoa na empresa que estão em aberto
                // Incluir filtro por tipo de título se informado
                string searchFields;
                string searchValues;

                if (!string.IsNullOrEmpty(dc_tipo_titulo))
                {
                    searchFields = "[cd_pessoa_titulo],[cd_pessoa_empresa],[id_status_titulo],[dc_tipo_titulo]";
                    searchValues = $"[{cd_pessoa_titulo}],[{cd_pessoa_empresa}],[1],[{dc_tipo_titulo}]"; // Status 1 = Em aberto
                    Console.WriteLine($"[ValidacaoTituloAnterior] Buscando parcelas anteriores - Pessoa: {cd_pessoa_titulo}, Empresa: {cd_pessoa_empresa}, Tipo: {dc_tipo_titulo}, Parcela atual: {numeroParcela}");
                }
                else
                {
                    searchFields = "[cd_pessoa_titulo],[cd_pessoa_empresa],[id_status_titulo]";
                    searchValues = $"[{cd_pessoa_titulo}],[{cd_pessoa_empresa}],[1]"; // Status 1 = Em aberto
                    Console.WriteLine($"[ValidacaoTituloAnterior] Buscando parcelas anteriores - Pessoa: {cd_pessoa_titulo}, Empresa: {cd_pessoa_empresa}, Parcela atual: {numeroParcela} (sem filtro de tipo)");
                }

                var result = await SQLServerService.GetList(
                    "T_TITULO",
                    1,
                    1000, // Limite máximo
                    "nm_parcela_titulo",
                    false, // ASC para pegar as parcelas menores primeiro
                    "",
                    searchFields,
                    searchValues,
                    source,
                    SearchModeEnum.Equals,
                    null,
                    null
                );

                if (result.success && result.data != null && result.data.Count > 0)
                {
                    Console.WriteLine($"[ValidacaoTituloAnterior] Encontrados {result.data.Count} títulos em aberto");

                    // Filtrar apenas as parcelas anteriores
                    var parcelasAnteriores = result.data
                        .Where(t => t.ContainsKey("nm_parcela_titulo")
                                 && t["nm_parcela_titulo"] != null
                                 && Convert.ToInt32(t["nm_parcela_titulo"]) < numeroParcela)
                        .ToList();

                    if (parcelasAnteriores.Count > 0)
                    {
                        var numerosEncontrados = string.Join(", ", parcelasAnteriores.Select(p => p["nm_parcela_titulo"]));
                        Console.WriteLine($"[ValidacaoTituloAnterior] Encontradas {parcelasAnteriores.Count} parcelas anteriores em aberto: {numerosEncontrados}");
                    }
                    else
                    {
                        Console.WriteLine($"[ValidacaoTituloAnterior] Nenhuma parcela anterior em aberto encontrada");
                    }

                    return parcelasAnteriores.Count > 0 ? parcelasAnteriores : null;
                }

                Console.WriteLine($"[ValidacaoTituloAnterior] Nenhum título encontrado na busca");

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar parcelas anteriores: {ex.Message}");
                return null;
            }
        }
    }
}

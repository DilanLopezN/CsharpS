using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simjob.Framework.Services.Api.Services
{
    public static class ValidacaoDataRetroativaService
    {
        /// <summary>
        /// Valida se uma data de operação está dentro do período permitido de dias retroativos
        /// </summary>
        /// <param name="dataOperacao">Data da operação financeira</param>
        /// <param name="cd_pessoa_empresa">Código da empresa</param>
        /// <param name="source">Fonte de dados</param>
        /// <param name="userId">ID do usuário logado no MongoDB (opcional - para verificar se é admin)</param>
        /// <param name="userService">Serviço de usuários do MongoDB (opcional - para verificar grupo admin)</param>
        /// <param name="groupService">Serviço de grupos do MongoDB (opcional - para verificar grupo admin)</param>
        /// <returns>Tupla com (sucesso, mensagemErro)</returns>
        public static async Task<(bool sucesso, string mensagemErro)> ValidarDataRetroativa(
            DateTime dataOperacao,
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
                    Console.WriteLine($"[ValidacaoDataRetroativa] Verificando admin para userId: {userId}");

                    // Buscar usuário no MongoDB pelo ID
                    var usuarioMongo = userService.GetUserById(userId);

                    if (usuarioMongo != null)
                    {
                        Console.WriteLine($"[ValidacaoDataRetroativa] Usuário encontrado. Id: {usuarioMongo.Id}, UserName: {usuarioMongo.UserName}, GroupId: {usuarioMongo.GroupId}");

                        if (!string.IsNullOrEmpty(usuarioMongo.GroupId))
                        {
                            // Buscar grupo do usuário
                            var grupo = groupService.GetGroupById(usuarioMongo.GroupId);

                            if (grupo != null)
                            {
                                Console.WriteLine($"[ValidacaoDataRetroativa] Grupo encontrado: {grupo.GroupName}, Cd_empresa: {grupo.Cd_empresa}");

                                // Verificar se é grupo "Administrador" ou "Administradores" e se pertence à mesma empresa
                                string groupNameUpper = grupo.GroupName?.ToUpper() ?? "";
                                bool isAdminGroup = groupNameUpper == "ADMINISTRADORES" || groupNameUpper == "ADMINISTRADOR";
                                bool isSameCompany = grupo.Cd_empresa?.ToString() == cd_pessoa_empresa.ToString();

                                Console.WriteLine($"[ValidacaoDataRetroativa] isAdminGroup: {isAdminGroup}, isSameCompany: {isSameCompany}");

                                if (isAdminGroup && isSameCompany)
                                {
                                    Console.WriteLine($"[ValidacaoDataRetroativa] BYPASS ATIVADO - Usuário é admin da escola");
                                    // Usuário é admin da escola - pode fazer operações retroativas sem restrição
                                    return (true, null);
                                }
                            }
                            else
                            {
                                Console.WriteLine($"[ValidacaoDataRetroativa] Grupo não encontrado para GroupId: {usuarioMongo.GroupId}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"[ValidacaoDataRetroativa] Usuário sem grupo associado");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"[ValidacaoDataRetroativa] Usuário não encontrado com userId: {userId}");
                    }
                }
                else
                {
                    Console.WriteLine($"[ValidacaoDataRetroativa] Parâmetros de admin não fornecidos. userId: {userId}, userService: {userService != null}, groupService: {groupService != null}");
                }

                // Buscar parâmetros da escola
                var parametros = await BuscarParametrosEscola(cd_pessoa_empresa, source);

                if (parametros == null || !parametros.ContainsKey("id_retroativo_caixa"))
                {
                    // Se não encontrou o parâmetro, permite a operação
                    return (true, null);
                }

                // Obter número de dias retroativos permitidos
                int diasBloqueio = Convert.ToInt32(parametros["id_retroativo_caixa"]);

                // Se for 0, o bloqueio está desabilitado
                if (diasBloqueio == 0)
                {
                    return (true, null);
                }

                // Calcular data limite (hoje - dias retroativos)
                DateTime dataLimite = DateTime.Today.AddDays(-diasBloqueio);

                // Validar se a data da operação está dentro do período permitido
                if (dataOperacao.Date < dataLimite)
                {
                    string mensagem = $"Operação bloqueada: data retroativa não permitida. " +
                                    $"A data informada ({dataOperacao:dd/MM/yyyy}) excede o limite de {diasBloqueio} dia(s) retroativo(s). " +
                                    $"Data mínima permitida: {dataLimite:dd/MM/yyyy}.";
                    return (false, mensagem);
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                // Em caso de erro na validação, registra mas permite a operação
                // para não bloquear o sistema por falha na validação
                Console.WriteLine($"Erro ao validar data retroativa: {ex.Message}");
                return (true, null);
            }
        }

        private static async Task<Dictionary<string, object>> BuscarParametrosEscola(int cd_pessoa_empresa, Source source)
        {
            var filtroParametro = new List<(string campo, object valor)> { ("cd_pessoa_escola", cd_pessoa_empresa) };
            return await SQLServerService.GetFirstByFields(source, "T_PARAMETRO", filtroParametro);
        }
    }
}

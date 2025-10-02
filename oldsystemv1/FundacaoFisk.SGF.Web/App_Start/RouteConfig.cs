using System.Web.Mvc;
using System.Web.Http;
using System.Web.Routing;

namespace FundacaoFisk.SGF.Web
{
    public class RouteConfig
    {
        public static void Registerroutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            //routes.MapHttpRoute("DefaultApiWithAction", "Api/{controller}/{action}");
            //routes.MapHttpRoute(
            //    name: "DefaultApiWithID",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: null
            //);

            routes.MapHttpRoute(
                name: "DefaultApiWithAction",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional });

            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            routes.MapRoute(
                "DefaultMVCAction",
                "{controller}/{action}/{id}",
                new { id = UrlParameter.Optional }
            );
            routes.MapRoute(
                "Default",
                "{controller}/{id}",
                new { controller = "Auth", action = "Index", id = UrlParameter.Optional }
            );

            ////Biblioteca
            //routes.MapHttpRoute(
            //   name: "BibliotecaApi",
            //   routeTemplate: "api/biblioteca/{action}",
            //   defaults: new { controller = "biblioteca" });

            ////EmailMarketing
            //routes.MapHttpRoute(
            //   name: "EmailMarketingApi",
            //   routeTemplate: "api/emailMarketing/{action}",
            //   defaults: new { controller = "emailMarketing" });

            ////Auth
            //routes.MapHttpRoute(
            //   name: "AuthApi",
            //   routeTemplate: "api/auth/{action}",
            //   defaults: new { controller = "auth" }
            //   );

            ////CNAB
            //routes.MapHttpRoute(
            //   name: "CNABApi",
            //   routeTemplate: "api/cnab/{action}",
            //   defaults: new { controller = "cnab" });

            ////Coordenacao
            //routes.MapHttpRoute(
            //   name: "CoordenacaoApi",
            //   routeTemplate: "api/coordenacao/{action}",
            //   defaults: new { controller = "coordenacao" });
            //routes.MapHttpRoute(
            //   name: "CursoApi",
            //   routeTemplate: "api/curso/{action}",
            //   defaults: new { controller = "curso" }
            //   );
            //routes.MapHttpRoute(
            //   name: "TurmaApi",
            //   routeTemplate: "api/turma/{action}",
            //   defaults: new { controller = "turma" });
            //routes.MapHttpRoute(
            //   name: "ProfessorApi",
            //   routeTemplate: "api/professor/{action}",
            //   defaults: new { controller = "professor" });

            ////Empresa
            //routes.MapHttpRoute(
            //   name: "EmpresaApi",
            //   routeTemplate: "api/empresa/{action}",
            //   defaults: new { controller = "empresa" });
            //routes.MapHttpRoute(
            // name: "FuncionarioApi",
            // routeTemplate: "api/funcionario/{action}",
            // defaults: new { controller = "funcionario" });

            ////Financeiro
            //routes.MapHttpRoute(
            //   name: "FinanceiroApi",
            //   routeTemplate: "api/financeiro/{action}",
            //   defaults: new { controller = "financeiro" }
            //   );
            //routes.MapHttpRoute(
            //   name: "FiscalApi",
            //   routeTemplate: "api/fiscal/{action}",
            //   defaults: new { controller = "fiscal" }
            //   );

            ////Instituicao Ensino
            //routes.MapHttpRoute(
            //   name: "EscolaApi",
            //   routeTemplate: "api/escola/{action}/{id}",
            //   defaults: new { controller = "escola", id = RouteParameter.Optional });

            ////Log
            //routes.MapHttpRoute(
            //   name: "LogGeralApi",
            //   routeTemplate: "api/logGeral/{action}",
            //   defaults: new { controller = "logGeral" });

            ////Pessoa
            //routes.MapHttpRoute(
            //   name: "PessoaApi",
            //   routeTemplate: "api/pessoa/{action}",
            //   defaults: new { controller = "pessoa" }
            //   );
            //routes.MapHttpRoute(
            //   name: "LocalidadeApi",
            //   routeTemplate: "api/localidade/{action}",
            //   defaults: new { controller = "localidade" }
            //   );

            ////Secretária
            //routes.MapHttpRoute(
            //   name: "SecretariaApi",
            //   routeTemplate: "api/secretaria/{action}",
            //   defaults: new { controller = "secretaria" }
            //   );

            //routes.MapHttpRoute(
            //   name: "AlunoApi",
            //   routeTemplate: "api/aluno/{action}",
            //   defaults: new { controller = "aluno" }
            //   );

            ////usuário
            //routes.MapHttpRoute(
            //   name: "UsuarioApi",
            //   routeTemplate: "api/usuario/{action}",
            //   defaults: new { controller = "usuario" }
            //   );
            //routes.MapHttpRoute(
            //   name: "UsuarioSenhaApi",
            //   routeTemplate: "api/usuariosenha/{action}",
            //   defaults: new { controller = "usuariosenha" }
            //   );
            //routes.MapHttpRoute(
            //   name: "PermissaoApi",
            //   routeTemplate: "api/permissao/{action}",
            //   defaults: new { controller = "permissao" }
            //   );
        }
    }
}
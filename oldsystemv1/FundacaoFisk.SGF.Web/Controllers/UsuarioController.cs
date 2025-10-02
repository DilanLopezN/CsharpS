using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Componentes.GenericBusiness.Comum;
using Componentes.GenericController;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Pessoa.Business;
using FundacaoFisk.SGF.Web.Services.Usuario.Business;
using FundacaoFisk.SGF.Web.Services.Usuario.Comum;
using FundacaoFisk.SGF.Web.Services.Usuario.Model;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Services.InstituicaoEnsino.Model;
using FundacaoFisk.SGF.Utils.Messages;
using FundacaoFisk.SGF.Web.Services.InstituicaoEnsino.Comum.IBusiness;
using log4net;


namespace FundacaoFisk.SGF.Web.Controllers
{
    public class UsuarioController : ComponentesMVCController
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(PessoaController));

        private ReturnResult retorno { get; set; }

        public UsuarioController()
        {
        }
        
        public ActionResult Index()
        {
            return View();
        }
        // GET: /UsuarioSenha/

        public ActionResult UsuarioSenha()
        {
            return View();
        }

        public ActionResult Grupo()
        {
            return View();
        }

        // POST api/Usuario
        //[MvcComponentesAuthorize(Roles = "pes.i")]
        [MvcComponentesAuthorize(Roles = "usu.i")]
        public ActionResult PostInsertUsuario(UsuarioUI usuarioPermissao)
        {
            ReturnResult retorno = new ReturnResult();
            int cdEsc = int.Parse(Session["CodEscolaSelecionada"] + "");
            bool master = bool.Parse(Session["IdMaster"] + "");
            bool masterGeral = bool.Parse(Session["MasterGeral"] + "");
            try
            {
                if (!string.IsNullOrEmpty(usuarioPermissao.email) && !Utils.Utils.validarEmail(usuarioPermissao.email))
                    throw new UsuarioBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroEmailInvalido, null,
                       UsuarioBusinessException.TipoErro.ERRO_EMAIL_INVALIDO, false);

                IEscolaBusiness BusinessEscola = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                IUsuarioBusiness BusinessUsuario = (IUsuarioBusiness)base.instanciarBusiness<IUsuarioBusiness>();
                IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                IApiAreaRestritaBusiness BusinessAreRestrita = (IApiAreaRestritaBusiness)base.instanciarBusiness<IApiAreaRestritaBusiness>();

                configuraBusiness(new List<IGenericBusiness>() { BusinessEscola, BusinessUsuario, BusinessEmpresa });
                List<SysDireitoUsuario> listaUsuarios = new List<SysDireitoUsuario>();
                PessoaFisicaSGF pessoaFisica = new PessoaFisicaSGF();
                UsuarioWebSGF usuario = usuarioPermissao.usuarioWeb;
                usuario.isMasterUserLogado = master;
                if (usuarioPermissao.permissoes != null)
                {
                    List<Permissao> permissoes = usuarioPermissao.permissoes.ToList();
                    if (permissoes != null && !usuario.id_master)
                        usuario.Direitos = MontaDireitosRecursivo(listaUsuarios, permissoes);
                }
               
                //Tranforma o objeto de view em objeto de controller:
                //Cadastra a pessoa do usuario e e-mail caso não seja o sysAdmin.
                if (!usuario.id_admin)
                {
                    if (usuario.cd_pessoa > 0)
                    {
                        pessoaFisica.cd_pessoa = (int)usuario.cd_pessoa;
                        usuario.PessoaFisica = pessoaFisica;
                    }
                    else
                    {
                        //Componentes.GenericModel.PessoaFisica pessoaFisica = new Componentes.GenericModel.PessoaFisica();
                        pessoaFisica.no_pessoa = usuarioPermissao.nm_pessoa_usuario;
                        pessoaFisica.nm_cpf = usuarioPermissao.nm_cpf;
                        pessoaFisica.nm_sexo = usuarioPermissao.nm_sexo;

                        if (!String.IsNullOrEmpty(usuarioPermissao.email))
                            pessoaFisica.TelefonePessoa.Add(new TelefoneSGF
                            {
                                cd_pessoa = pessoaFisica.cd_pessoa,
                                cd_tipo_telefone = (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL,
                                cd_classe_telefone = (int)ClasseTelefoneSGF.TIPO_COMERCIAL,
                                id_telefone_principal = true,
                                dc_fone_mail = usuarioPermissao.email
                            });
                    }
                }
                else
                {
                    usuario.id_master = false;
                    usuario.cd_pessoa = null;
                    usuario.Direitos = UsuarioUI.montarDiretirosSysAdmin(usuario.cd_usuario);
                }

                if (BusinessAreRestrita.aplicaApiAreaRestrita() == true && usuarioPermissao != null &&
                    usuarioPermissao.userAreaRestrita != null && usuario.id_admin == false &&
                    !String.IsNullOrEmpty(usuarioPermissao.userAreaRestrita.name) &&
                    !String.IsNullOrEmpty(usuarioPermissao.userAreaRestrita.email) &&
                    usuarioPermissao.userAreaRestrita.menus != null && usuarioPermissao.userAreaRestrita.menus.Count > 0 && master)
                {
                    
                    usuario.nameAreaRestrita = usuarioPermissao.userAreaRestrita.name;
                    usuario.emailAreaRestrita = usuarioPermissao.userAreaRestrita.email;
                    usuario.menusAreaRestrita = usuarioPermissao.userAreaRestrita.menus;
                }

                if (usuarioPermissao.escolas != null)
                    foreach (var e in usuarioPermissao.escolas)
                        usuarioPermissao.usuarioWeb.Empresas.Add(new UsuarioEmpresaSGF { cd_pessoa_empresa = e.cd_pessoa, cd_usuario = usuarioPermissao.usuarioWeb.cd_usuario });
                if (usuarioPermissao.gruposUsuario != null)
                    foreach (var g in usuarioPermissao.gruposUsuario)
                        usuarioPermissao.usuarioWeb.Grupos.Add(new SysGrupoUsuario { cd_grupo = g.cd_grupo, cd_usuario = usuarioPermissao.usuarioWeb.cd_usuario });
                bool sysAdmin = BusinessUsuario.verificarSysAdmin((string)Session["UserName"]);
                if (!sysAdmin && (!master && usuario.id_master) || (!master && usuario.id_admin))
                    throw new UsuarioBusinessException(Componentes.Utils.Messages.Messages.msgAcessoNegado, null, UsuarioBusinessException.TipoErro.ERROR_USUARIO_SEM_PERMISSAO, false);
                if (master && !masterGeral && usuario.id_admin)
                    throw new UsuarioBusinessException(Componentes.Utils.Messages.Messages.msgAcessoNegado, null, UsuarioBusinessException.TipoErro.ERROR_USUARIO_SEM_PERMISSAO, false);
                var insertUsuario = BusinessEscola.PostInsertUsuario(usuario, pessoaFisica, cdEsc, usuarioPermissao.horarios);
                retorno.retorno = insertUsuario;

                if ((insertUsuario.cd_usuario <= 0))
                {

                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                }
                else
                {
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                }
                return new RenderJsonActionResult { Result = retorno };

            }
            catch (PessoaBusinessException exe)
            {
                if (exe.tipoErro == PessoaBusinessException.TipoErro.ERRO_CPFJAEXISTENTE)
                {
                    retorno.AddMensagem(exe.Message, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                    return new RenderJsonActionResult { Result = new { erro = retorno } };
                    //return gerarLogException(ex.Message, retorno, logger, ex);
                }
                else
                {
                    logger.Error("UsuarioController PostInsertUsuario - Erro: " + exe.Message + exe.StackTrace + exe.InnerException);
                    retorno.AddMensagem(Messages.msgNotUpReg, exe.Message + exe.StackTrace + exe.InnerException, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                    return new RenderJsonActionResult { Result = new { erro = retorno } };
                }
            }
            catch (UsuarioBusinessException exe)
            {
                if (exe.tipoErro == UsuarioBusinessException.TipoErro.ERRO_SENHA_INVALIDA || 
                    exe.tipoErro == UsuarioBusinessException.TipoErro.ERROR_USUARIO_SEM_PERMISSAO ||
                    exe.tipoErro == UsuarioBusinessException.TipoErro.ERRO_JA_EXISTE_SYSADMIN_ESCOLAS ||
                    exe.tipoErro == UsuarioBusinessException.TipoErro.ERRO_EMAIL_INVALIDO)
                {
                    retorno.AddMensagem(exe.Message, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                    return new RenderJsonActionResult { Result = new { erro = retorno } };
                    //return gerarLogException(ex.Message, retorno, logger, ex);
                }
                else
                {
                    logger.Error("UsuarioController PostInsertUsuario - Erro: " + exe.Message + exe.StackTrace + exe.InnerException);
                    retorno.AddMensagem(Messages.msgNotUpReg, exe.Message + exe.StackTrace + exe.InnerException, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                    return new RenderJsonActionResult { Result = new { erro = retorno } };
                }
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
        }

        // POST api/Usuario
        [MvcComponentesAuthorize(Roles = "usu.a")]
        public ActionResult PostEditUsuario(UsuarioUI usuarioPermissao)
        {
            ReturnResult retorno = new ReturnResult();
            int cdEsc = int.Parse(Session["CodEscolaSelecionada"] + "");
            bool master = bool.Parse(Session["IdMaster"] + "");
            bool masterGeral = bool.Parse(Session["MasterGeral"] + "");
            List<Horario> horariosUser = null;
            try
            {
                if (!string.IsNullOrEmpty(usuarioPermissao.email) && !Utils.Utils.validarEmail(usuarioPermissao.email))
                    throw new UsuarioBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroEmailInvalido, null,
                       UsuarioBusinessException.TipoErro.ERRO_EMAIL_INVALIDO, false);

                IEscolaBusiness BusinessEscola = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                IUsuarioBusiness BusinessUsuario = (IUsuarioBusiness)base.instanciarBusiness<IUsuarioBusiness>();
                IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                IApiAreaRestritaBusiness BusinessAreRestrita = (IApiAreaRestritaBusiness)base.instanciarBusiness<IApiAreaRestritaBusiness>();
                bool sysAdmin = BusinessUsuario.verificarSysAdmin((string)Session["UserName"]);
                configuraBusiness(new List<IGenericBusiness>() { BusinessEscola, BusinessUsuario, BusinessEmpresa });
                List<SysDireitoUsuario> listaUsuarios = new List<SysDireitoUsuario>();
                PessoaFisicaSGF pessoaFisica = new PessoaFisicaSGF();
                UsuarioWebSGF usuario = usuarioPermissao.usuarioWeb;
                usuario.isMasterUserLogado = master;
                if (usuarioPermissao.atualizouPermissoes && usuarioPermissao.permissoes != null)
                {
                    List<Permissao> permissoes = usuarioPermissao.permissoes.ToList();
                    if (permissoes != null && !usuario.id_master)
                        usuario.Direitos = MontaDireitosRecursivo(listaUsuarios, permissoes);
                }
                else
                    usuario.Direitos = null;
                if (usuarioPermissao.atualizouEscolasOrGrupos)
                {
                    if (usuarioPermissao.escolas != null)
                        foreach (var e in usuarioPermissao.escolas)
                            usuario.Empresas.Add(new UsuarioEmpresaSGF { cd_pessoa_empresa = e.cd_pessoa, cd_usuario = usuarioPermissao.usuarioWeb.cd_usuario });
                    if (usuarioPermissao.gruposUsuario != null)
                        foreach (var g in usuarioPermissao.gruposUsuario)
                            usuario.Grupos.Add(new SysGrupoUsuario { cd_grupo = g.cd_grupo, cd_usuario = usuarioPermissao.usuarioWeb.cd_usuario });
                }
                else
                {
                    usuarioPermissao.usuarioWeb.Empresas = null;
                    usuarioPermissao.usuarioWeb.Grupos = null;
                }
                if (!master && !sysAdmin)
                {
                    if (usuario.Empresas != null)
                        usuario.Empresas = null;
                }
                if (usuarioPermissao.atualizouHorarios)
                    horariosUser = usuarioPermissao.horarios;
                if (!usuario.id_admin)
                {
                    pessoaFisica.cd_pessoa = (int)usuario.cd_pessoa;
                    pessoaFisica.no_pessoa = usuarioPermissao.nm_pessoa_usuario;
                    pessoaFisica.nm_cpf = usuarioPermissao.nm_cpf;
                    pessoaFisica.nm_sexo = usuarioPermissao.nm_sexo;

                    if (!String.IsNullOrEmpty(usuarioPermissao.email))
                        pessoaFisica.TelefonePessoa.Add(new TelefoneSGF
                        {
                            cd_pessoa = pessoaFisica.cd_pessoa,
                            cd_tipo_telefone = (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL,
                            cd_classe_telefone = (int)ClasseTelefoneSGF.TIPO_COMERCIAL,
                            id_telefone_principal = true,
                            dc_fone_mail = usuarioPermissao.email
                        });

                    usuario.PessoaFisica = pessoaFisica;
                }
                else
                {
                    usuario.id_master = false;
                    usuario.cd_pessoa = null;
                    usuario.Direitos = UsuarioUI.montarDiretirosSysAdmin(usuario.cd_usuario);
                }
                //Proteções
                if (!sysAdmin && (!master && usuario.id_master) || (!master && usuario.id_admin))
                    throw new UsuarioBusinessException(Componentes.Utils.Messages.Messages.msgAcessoNegado, null, UsuarioBusinessException.TipoErro.ERROR_USUARIO_SEM_PERMISSAO, false);
                if (master && !masterGeral && usuario.id_admin)
                    throw new UsuarioBusinessException(Componentes.Utils.Messages.Messages.msgAcessoNegado, null, UsuarioBusinessException.TipoErro.ERROR_USUARIO_SEM_PERMISSAO, false);


                UsuarioWebSGF usuarioBd = BusinessUsuario.findUsuarioById(usuario.cd_usuario);
                //Se o usuario existe no sgf e não está cadastrado na area restrita(todos os campos devem estar preenchidos para fazer o cadastro) 
                if (BusinessAreRestrita.aplicaApiAreaRestrita() == true &&
                    usuarioPermissao.userAreaRestrita != null && usuarioBd.id_admin == false && 
                    !String.IsNullOrEmpty(usuarioPermissao.userAreaRestrita.name) &&
                    !String.IsNullOrEmpty(usuarioPermissao.userAreaRestrita.email) &&
                    usuarioPermissao.userAreaRestrita.menus != null && usuarioPermissao.userAreaRestrita.menus.Count > 0 && master)
                {
                    
                    usuario.nameAreaRestrita = usuarioPermissao.userAreaRestrita.name;
                    usuario.emailAreaRestrita = usuarioPermissao.userAreaRestrita.email;
                    usuario.menusAreaRestrita = usuarioPermissao.userAreaRestrita.menus;
                }
               

                var insertUsuario = BusinessEscola.PostEditUsuario(usuario, cdEsc, horariosUser);
                retorno.retorno = insertUsuario;

                if ((insertUsuario.cd_usuario <= 0))
                {

                    retorno.AddMensagem(Messages.msgNotUpReg, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                }
                else
                {
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                }
                   return new RenderJsonActionResult { Result = retorno };

            }
            catch (PessoaBusinessException exe)
            {
                if (exe.tipoErro == PessoaBusinessException.TipoErro.ERRO_CPFJAEXISTENTE)
                {
                    retorno.AddMensagem(exe.Message, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                    return new RenderJsonActionResult { Result = new { erro = retorno } };
                    //return gerarLogException(ex.Message, retorno, logger, ex);
                }
                else
                {
                    logger.Error("UsuarioController PostEditUsuario - Erro: " + exe.Message + exe.StackTrace + exe.InnerException);
                    retorno.AddMensagem(Messages.msgNotUpReg, exe.Message + exe.StackTrace + exe.InnerException, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                    return new RenderJsonActionResult { Result = new { erro = retorno } };
                }
            }
            catch (UsuarioBusinessException exe)
            {
                if (exe.tipoErro == UsuarioBusinessException.TipoErro.ERRO_SENHA_INVALIDA || 
                    exe.tipoErro == UsuarioBusinessException.TipoErro.ERROR_USUARIO_SEM_PERMISSAO ||
                    exe.tipoErro == UsuarioBusinessException.TipoErro.ERRO_JA_EXISTE_SYSADMIN_ESCOLAS || 
                    exe.tipoErro == UsuarioBusinessException.TipoErro.ERRO_USUARIO_COMUM_SYSADMIN ||
                    exe.tipoErro == UsuarioBusinessException.TipoErro.ERRO_SYSADMIN_USUARIO ||
                    exe.tipoErro == UsuarioBusinessException.TipoErro.ERRO_EMAIL_INVALIDO)
                {
                    retorno.AddMensagem(exe.Message, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                    return new RenderJsonActionResult { Result = new { erro = retorno } };
                    //return gerarLogException(ex.Message, retorno, logger, ex);
                }
                else
                {
                    logger.Error("UsuarioController PostInsertUsuario - Erro: " + exe.Message + exe.StackTrace + exe.InnerException);
                    retorno.AddMensagem(Messages.msgNotUpReg, exe.Message + exe.StackTrace + exe.InnerException, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                    return new RenderJsonActionResult { Result = new { erro = retorno } };
                }
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
        }

        private List<SysDireitoUsuario> MontaDireitosRecursivo(List<SysDireitoUsuario> listaUsuarios, ICollection<Permissao> permissoes1)
        {
            // Transforma os dados de permissões para entidades de negócio:
            var permissoes = permissoes1.ToList();
            for (int i = 0; i < permissoes.Count; i++)
            {
                if (permissoes[i].visualizar)
                {
                    SysDireitoUsuario sysUsuario = new SysDireitoUsuario();

                    sysUsuario.cd_menu = permissoes[i].id;
                    sysUsuario.id_alterar = permissoes[i].alterar;
                    sysUsuario.id_inserir = permissoes[i].incluir;
                    sysUsuario.id_deletar = permissoes[i].excluir;

                    listaUsuarios.Add(sysUsuario);
                }
                if (permissoes[i].children.Count > 0)
                    MontaDireitosRecursivo(listaUsuarios, permissoes[i].children);
            }

            return listaUsuarios;
        }

        [HttpGet]
        [MvcComponentesAuthorize(Roles = "usu")]
        public ActionResult verifyExistLoginOK(string login, string nomePessoa)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IUsuarioBusiness BusinessUsuario = (IUsuarioBusiness)base.instanciarBusiness<IUsuarioBusiness>();
                BusinessUsuario.verifyExistLoginOK(login, nomePessoa);
                return null;
            }
            catch (UsuarioBusinessException exe)
            {
                if (exe.tipoErro == UsuarioBusinessException.TipoErro.ERRO_COMBINACOES || exe.tipoErro == UsuarioBusinessException.TipoErro.ERRO_USUARIOEXISTENTE)
                 {
                     retorno.AddMensagem(string.Format(exe.Message, Messages.msgRedeFisk), null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                    return new RenderJsonActionResult { Result = new { erro = retorno } };
                }
                else
                {
                    logger.Error(exe);
                    retorno.AddMensagem(Messages.msgRegBuscError, exe.Message + exe.StackTrace + exe.InnerException, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                    return new RenderJsonActionResult { Result = new { erro = retorno } };
                }
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

    }
}

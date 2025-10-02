using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IBusiness;
using FundacaoFisk.SGF.Utils;
using log4net;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;
using Componentes.GenericController;
using FundacaoFisk.SGF.Web.Services.InstituicaoEnsino.Comum.IBusiness;
using FundacaoFisk.SGF.Services.Financeiro.Model;
using FundacaoFisk.SGF.Utils.Messages;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;
using Componentes.Web.Services.Pessoa.Model;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.Web.Controllers
{
    public class LinkController : ComponentesMVCController
    {
        public LinkController()
        {
        }
        //
        // GET: /Financeiro/

        public ActionResult Index()
        {
            LinkAlunoCurso link = new LinkAlunoCurso();

            link.TipoRetorno = TipoRetornoLink.EDICAO;
            link.Selecionados = new List<Curso>();
            
            PessoaFisicaUI p = new PessoaFisicaUI();

            p.celular = Guid.NewGuid().ToString();
            p.descFoto = Guid.NewGuid().ToString();
            p.email = Guid.NewGuid().ToString();
            p.site = Guid.NewGuid().ToString();
            p.telefone = Guid.NewGuid().ToString();
            p.pessoaFisica = new Componentes.GenericModel.PessoaFisica();
            p.pessoaFisica.cd_atividade_principal = 1;
            p.pessoaFisica.cd_endereco_principal = 1;
            p.pessoaFisica.cd_estado_civil = 1;
            p.pessoaFisica.cd_estado_expedidor = 1;
            p.pessoaFisica.cd_integracao_folha = 1;
            p.pessoaFisica.cd_loc_nacionalidade = 1;
            p.pessoaFisica.cd_loc_nascimento = 1;
            p.pessoaFisica.cd_orgao_expedidor= 1;
            p.pessoaFisica.cd_papel_principal = 1;
            p.pessoaFisica.cd_pessoa =1;
            p.pessoaFisica.cd_pessoa_cpf = 1;
            p.pessoaFisica.cd_telefone_principal = 1;
            p.pessoaFisica.cd_tratamento = 1;
            p.pessoaFisica.dc_carteira_motorista = Guid.NewGuid().ToString();;
            p.pessoaFisica.dc_carteira_trabalho = Guid.NewGuid().ToString();;
            p.pessoaFisica.dc_num_certidao_nascimento = Guid.NewGuid().ToString();;
            p.pessoaFisica.dc_num_crc = Guid.NewGuid().ToString();;
            p.pessoaFisica.dc_num_insc_inss = Guid.NewGuid().ToString();;
            p.pessoaFisica.dc_num_pessoa = Guid.NewGuid().ToString();;
            p.pessoaFisica.dc_num_titulo_eleitor = Guid.NewGuid().ToString();;
            p.pessoaFisica.dc_reduzido_pessoa = Guid.NewGuid().ToString();;
            p.pessoaFisica.dt_cadastramento = new DateTime();
            p.pessoaFisica.dt_casamento = new DateTime();
            p.pessoaFisica.dt_emis_expedidor = new DateTime();
            p.pessoaFisica.dt_nascimento = new DateTime();
            p.pessoaFisica.dt_venc_habilitacao = new DateTime();

            // Coloca 40 caracteres na obs:
            p.pessoaFisica.txt_obs_pessoa = "1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890";
            link.DadosRetorno = new AlunoUI();
            link.DadosRetorno.pessoaFisicaUI = p;
            Session[Guid.NewGuid().ToString()] = link;

            return View();
        }
    }
}

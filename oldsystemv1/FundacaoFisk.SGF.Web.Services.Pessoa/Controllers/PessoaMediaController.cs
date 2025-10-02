using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Componentes.GenericController;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.Business;
using log4net;
using Componentes.Utils;

using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.Controllers
{
    public class PessoaMediaController : ComponentesApiController
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(PessoaController));

        public PessoaMediaController()
        {
        }

        [AllowAnonymous]
        public HttpResponseMessage Get(int id)
        {
            IPessoaBusiness Business = (IPessoaBusiness)base.instanciarBusiness<IPessoaBusiness>();
            PessoaSGF pessoa = Business.getPessoaImage(id);
            var mediaType = ManipuladorArquivo.dealImageExtensionById(id, pessoa.ext_img_pessoa);
            var image = pessoa.img_pessoa;
            if (!string.IsNullOrEmpty(mediaType))
                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK, image, mediaType);
                }
                catch (Exception)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Foto não encontrada");
                    throw;
                }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "não existe foto para esse arquivo");
            }
        }

    }
}
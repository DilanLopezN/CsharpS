using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Simjob.Framework.Application.Controllers;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Core.Utils;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;

namespace Simjob.Framework.Services.Api.Controllers
{
    [Authorize]
    public class ProfileController : BaseController
    {
        private readonly IProfileService _profileService;
        public static IWebHostEnvironment _webHostEnvironment;
        private readonly IRepository<ProfileContext, Profile> _ProfileRepository;

        public ProfileController(IMediatorHandler bus, INotificationHandler<DomainNotification> notifications, IProfileService profileService, IWebHostEnvironment webHostEnvironment, IRepository<ProfileContext, Profile> ProfileRepository) : base(bus, notifications)
        {
            _profileService = profileService;
            _webHostEnvironment = webHostEnvironment;
            _ProfileRepository = ProfileRepository;
        }

        /// <summary>
        /// Insert preferences to Tenanty
        /// </summary>
        [HttpPost("insert")]
        [ExcludeFromCodeCoverage]
        public ActionResult Post([FromForm] string nomeEmpresa, [FromForm] string color, [FromForm] string banner, [FromForm] string[] dominio)
        {
            try
            {
                var accessToken = Request.Headers[HeaderNames.Authorization];
                var tenanty = "";
                var tokenInfo = Util.GetUserInfoFromToken(accessToken);

                if (tokenInfo.Count > 0) tenanty = tokenInfo["tenanty"];

                //if (accessToken != "")
                //{
                //    var token = accessToken.ToString().Split(" ");
                //    var onlyToken = "";
                //    foreach (var itens in token) { if (itens.Length > 10) onlyToken = itens; }
                //    var handler = new JwtSecurityTokenHandler();
                //    var jwtSecurityToken = handler.ReadJwtToken(onlyToken);
                //    var claims = jwtSecurityToken.Claims.ToList();

                //    foreach (var claim in claims)
                //    {
                //        if (claim.Type == "tenanty")
                //        {
                //            tenanty = claim.Value;
                //            break;
                //        }
                //    }
                //}

                var newProfile = new Profile(nomeEmpresa, color, banner, tenanty, dominio);

                bool existe = _ProfileRepository.Exists(u => u.Tenanty == newProfile.Tenanty);
                if (existe)
                {
                    var profile = _profileService.GetByTenanty(newProfile.Tenanty);
                    _profileService.UpdateProfile(newProfile, profile.Id);
                    return Ok("Tenanty Updated successfully");
                }

                _profileService.Register(newProfile);
                return Ok("tenanty registered successfully");
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Insert a new Logo Image
        /// </summary>
        /// <returns>Return OK</returns>
        /// <response code="200">Return OK</response>
        [HttpPost("insertLogo")]
        [ExcludeFromCodeCoverage]
        public ActionResult Post(IFormFile logo)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];
            var tenanty = "";
            if (accessToken != "")
            {
                var token = accessToken.ToString().Split(" ");
                var onlyToken = "";
                foreach (var itens in token) { if (itens.Length > 10) onlyToken = itens; }
                var handler = new JwtSecurityTokenHandler();
                var jwtSecurityToken = handler.ReadJwtToken(onlyToken);
                var claims = jwtSecurityToken.Claims.ToList();

                foreach (var claim in claims)
                {
                    if (claim.Type == "tenanty")
                    {
                        tenanty = claim.Value;
                        break;
                    }
                }
            }

            string ILogo = "";
            string folder = "images/" + tenanty;

            string webRootPath = _webHostEnvironment.WebRootPath;
            string newPath = Path.Combine(webRootPath, folder);
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }

            if (logo != null)
            {
                var fileName = Path.GetFileName(logo.FileName);
                var caminho = "logo" + Path.GetExtension(fileName);
                var fileExt = Convert.ToString(folder + "/logo" + Path.GetExtension(fileName));
                string fullPath = Path.Combine(newPath, caminho);
                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    logo.CopyTo(fileStream);
                }
                ILogo = fileExt;
            }
            var newProfile = new Profile(ILogo, tenanty);

            bool existe = _ProfileRepository.Exists(u => u.Tenanty == newProfile.Tenanty);
            if (existe)
            {
                var profile = _profileService.GetByTenanty(newProfile.Tenanty);
                _profileService.UpdateProfileLogo(newProfile, profile.Id);
                return Ok("Tenanty Updated successfully");
            }

            _profileService.Register(newProfile);
            return Ok("tenanty registered successfully");
        }

        /// <summary>
        /// Delete a logo image
        /// </summary>
        /// <returns>Return OK</returns>
        /// <response code="200">Return OK</response>
        [HttpPost("RemoveLogo")]
        [ExcludeFromCodeCoverage]
        public ActionResult Post()
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];
            var tenanty = "";
            if (accessToken != "")
            {
                var token = accessToken.ToString().Split(" ");
                var onlyToken = "";
                foreach (var itens in token) { if (itens.Length > 10) onlyToken = itens; }
                var handler = new JwtSecurityTokenHandler();
                var jwtSecurityToken = handler.ReadJwtToken(onlyToken);
                var claims = jwtSecurityToken.Claims.ToList();

                foreach (var claim in claims)
                {
                    if (claim.Type == "tenanty")
                    {
                        tenanty = claim.Value;
                        break;
                    }
                }
            }
            var newProfile = new Profile("", tenanty);

            bool existe = _ProfileRepository.Exists(u => u.Tenanty == newProfile.Tenanty);
            if (existe)
            {
                var profile = _profileService.GetByTenanty(newProfile.Tenanty);
                _profileService.UpdateProfileLogo(newProfile, profile.Id);
                return Ok("Logo removed successfully");
            }

            return Ok("tenanty not found");
        }

        /// <summary>
        /// Get profile by Tenanty
        /// </summary>
        /// <returns>Return OK</returns>
        /// <response code="200">Return profile object</response>
        [HttpGet("getByTenanty")]
        [ExcludeFromCodeCoverage]
        public IActionResult GetProfile()
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];

            var tenanty = "";
            if (accessToken != "")
            {
                var token = accessToken.ToString().Split(" ");
                var onlyToken = "";
                foreach (var itens in token) { if (itens.Length > 10) onlyToken = itens; }
                var handler = new JwtSecurityTokenHandler();
                var jwtSecurityToken = handler.ReadJwtToken(onlyToken);
                var claims = jwtSecurityToken.Claims.ToList();

                foreach (var claim in claims)
                {
                    if (claim.Type == "tenanty")
                    {
                        tenanty = claim.Value;
                        break;
                    }
                }
            }
            var profile = _profileService.GetByTenanty(tenanty);

            if (profile != null)
            {
                return ResponseDefault(profile);
            }
            else
            {
                return Ok("profile not found");
            }
        }

        /// <summary>
        /// Get profile by Tenanty
        /// </summary>
        /// <returns>Return OK</returns>
        /// <response code="200">Return profile object</response>
        [AllowAnonymous]
        [HttpGet("getByTenanty/{tenanty}")]
        public IActionResult GetProfileByTenanty(string tenanty)
        {
            var profile = _profileService.GetByTenanty(tenanty);
            if (profile != null)
            {
                return ResponseDefault(profile);
            }
            else
            {
                return ResponseDefault();
            }
        }

        #region unusedRoutes

        //[HttpGet("getById{id}")]
        //public IActionResult GetProfile(string id)
        //{
        //    return ResponseDefault(_profileService.GetProfileById(id));
        //}

        //[HttpGet("getAll")]
        //public IActionResult GetProfiles()
        //{
        //    return ResponseDefault(_profileService.GetProfiles());
        //}

        //[HttpPut("update{id}")]
        //public async Task<IActionResult> UpdateProfile([FromForm] string nomeEmpresa, string color, string id, IFormFile logo, IFormFile backGround, IFormFile backGroundLogon)
        //{
        //    var acessToken = Request.Headers[HeaderNames.Authorization];

        //    var tenanty = "";
        //    if (acessToken != "")
        //    {
        //        var token = acessToken.ToString().Split("Bearer ");
        //        var handler = new JwtSecurityTokenHandler();
        //        var jwtSecurityToken = handler.ReadJwtToken(token[1]);
        //        var claims = jwtSecurityToken.Claims.ToList();

        //        foreach (var claim in claims)
        //        {
        //            if (claim.Type == "tenanty")
        //            {
        //                tenanty = claim.Value;
        //                break;
        //            }
        //        }
        //    }

        //    var profileId = _profileService.GetProfileById(id);
        //    string ILogo = "";
        //    string IBackGround = "";
        //    string IBackGroundLogon = "";
        //    string folder = "images/" + tenanty;
        //    string webRootPath = _webHostEnvironment.WebRootPath;
        //    string newPath = Path.Combine(webRootPath, folder);
        //    if (!Directory.Exists(newPath))
        //    {
        //        Directory.CreateDirectory(newPath);
        //    }
        //    if (logo != null)
        //    {
        //        var fileName = Path.GetFileName(logo.FileName);
        //        var caminho = "logo" + Path.GetExtension(fileName);
        //        var fileExt = Convert.ToString(folder + "/logo" + Path.GetExtension(fileName));
        //        string fullPath = Path.Combine(newPath, caminho);
        //        using (var fileStream = new FileStream(fullPath, FileMode.Create))
        //        {
        //            logo.CopyTo(fileStream);
        //        }

        //        ILogo = fileExt;

        //    }
        //    else {
        //        ILogo = profileId.Logo;
        //    }
        //    if (backGround != null)
        //    {
        //        var fileName = Path.GetFileName(backGround.FileName);
        //        var caminho = "background" + Path.GetExtension(fileName);
        //        var fileExt = Convert.ToString(folder + "/background" + Path.GetExtension(fileName));
        //        string fullPath = Path.Combine(newPath, caminho);
        //        using (var fileStream = new FileStream(fullPath, FileMode.Create))
        //        {
        //            backGround.CopyTo(fileStream);
        //        }

        //        IBackGround = fileExt;

        //    }
        //    else
        //    {
        //        IBackGround = profileId.BackGround;
        //    }
        //    if (backGroundLogon != null)
        //    {
        //        var fileName = Path.GetFileName(backGroundLogon.FileName);
        //        var caminho = "backgroundlogon" + Path.GetExtension(fileName);
        //        var fileExt = Convert.ToString(folder + "/backgroundlogon" + Path.GetExtension(fileName));
        //        string fullPath = Path.Combine(newPath, caminho);
        //        using (var fileStream = new FileStream(fullPath, FileMode.Create))
        //        {
        //            backGroundLogon.CopyTo(fileStream);
        //        }

        //        IBackGroundLogon = fileExt;

        //    }
        //    else
        //    {
        //        IBackGroundLogon = profileId.BackGroundLogon;
        //    }
        //    var newProfile = new Profile(nomeEmpresa, color, tenanty, ILogo, IBackGroundLogon, IBackGround);

        //    var profile = _profileService.GetProfileById(id);
        //    _profileService.UpdateProfile(newProfile, id);
        //    return ResponseDefault(newProfile);
        //}

        //[HttpDelete("deleteById{id}")]
        //public async Task<IActionResult> DeleteProfile(string id)
        //{
        //    var profile = _profileService.GetProfileById(id);
        //    try
        //    {
        //        _profileService.DeleteProfile(id);
        //        return ResponseDefault(profile);
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        #endregion unusedRoutes
    }
}
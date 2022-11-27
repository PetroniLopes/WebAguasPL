using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Vereyon.Web;
using WebAguasPL.Helpers;
using WebAguasPL.Models;

namespace WebAguasPL.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IFlashMessage _flashMessage;
        private readonly IMailHelper _mailHelper;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IFlashMessage flashMessage, IMailHelper mailHelper, IConfiguration configuration)
        {
            _logger = logger;
            _flashMessage = flashMessage;
            _mailHelper = mailHelper;
            _configuration = configuration;
        }

        

        [Route("error/404")]
        public IActionResult Error404()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult ContactUs()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ContactUs(ContactUsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var mailTo = _configuration["Mail:From"];

                if(mailTo == null)
                {
                    _flashMessage.Danger("Invalid Submission");
                    return View(model);
                }

                Response response = _mailHelper.SendEmail(
                    mailTo,
                    model.Subject,

                    $"<h1>Name: {model.Name}    Email: {model.Email}</h1>" + model.Message);

                if (response.IsSuccess)
                {
                    _flashMessage.Confirmation("Submission sent");
                    return View(model);

                    //ViewBag.Message = "The instructions to allow your access has been sent to email";
                    //return View();

                }

                _flashMessage.Danger("Invalid Submission");
                return View(model);
            }
            _flashMessage.Danger("Invalid Submission");
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

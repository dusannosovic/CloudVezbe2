using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Client.Models;
using System.ServiceModel;
using Validation1;

namespace Client.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [Route("/HomeController/PostData")]
        public async Task<IActionResult> PostData(string ime, string prezime,string book,int iden)
        {
            var myBinding = new NetTcpBinding(SecurityMode.None);
            var myEndpoint = new EndpointAddress("net.tcp://localhost:53850/WebCommunication");

            using (var myChannelFactory = new ChannelFactory<IValidation>(myBinding, myEndpoint))
            {
                IValidation client = null;
                ViewData["Title"] = null;
                try
                {
                    client = myChannelFactory.CreateChannel();
                    
                    if (await client.ValidateAsync(ime, prezime, book,iden))
                    {
                        ViewData["Title"] = "Knjiga je uspesno kupljena";
                    }
                    else
                    {
                        ViewData["Title"] = "Polja nisu ispravno popunjena";
                    }
                    ((ICommunicationObject)client).Close();
                    myChannelFactory.Close();
                }
                catch
                {
                    (client as ICommunicationObject)?.Abort();
                }
            }
            return View("Index");
        }
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HtmlToPdf.Models;
using wkhtmltopdfCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.NodeServices;

namespace CorePOC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ViewRender view;
        private readonly INodeServices nodeServices;

        public HomeController(ViewRender view, INodeServices nodeServices)
        {
            this.view = view;
            this.nodeServices = nodeServices;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            var model = new InvoiceModel { InvoiceNo = "PWS-0001-2017", Address = "Jaipur rajasthan", CompanyName = "Planet Web Solution", DueDate = DateTime.Now, InvoiceDate = DateTime.Now, OfficeAddress = "Sita pura", PhoneNo = "9876543210" };
            ViewBag.Message = "Your application description page.";
            return new ViewAsPdf(model);
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public async Task<IActionResult> Node()
        {
            string htmlContent = this.view.Render("Home/Node");
            var result = await this.nodeServices.InvokeAsync<byte[]>("./pdf", htmlContent);
            ////HttpContext.Response.ContentType = "application/pdf";
            ////HttpContext.Response.Headers.Add("x-filename", "report.pdf");
            ////HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "x-filename");
            ////HttpContext.Response.Body.Write(result, 0, result.Length);
            string fileName = string.Format("{0}.pdf", DateTime.Now.ToString("ddMMyyyyhhmmss"));
            return this.File(result, "application/pdf");
        }

        public async Task<IActionResult> PdfCrowd()
        {
            string htmlContent = this.view.Render("Home/PdfCrowd");
            HttpResponse Response = HttpContext.Response;
            pdfcrowd.Client client = new pdfcrowd.Client("surendrakandira", "06277a91ab88bd95efab907bcdbd9bf5");
            MemoryStream Stream = new MemoryStream();
            await client.convertHtml(htmlContent, Stream);
            string fileName = string.Format("{0}.pdf", DateTime.Now.ToString("ddMMyyyyhhmmss"));
            // set HTTP response headers
            byte[] byteInfo = Stream.ToArray();
            ////Stream.Write(byteInfo, 0, byteInfo.Length);
            ////Stream.Position = 0;
            ////return this.File(Stream, "application/pdf", fileName);
            //// HttpContext.Response.ContentType = "application/pdf";
            ///HttpContext.Response.Body.Write(byteInfo, 0, byteInfo.Length);
            return this.File(byteInfo, "application/pdf");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using PostIFormFileToBlobStorage.Services;
using Microsoft.Net.Http.Headers;

namespace PostIFormFileToBlobStorage.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task PostImage(IList<IFormFile> files)
        {
            foreach (var file in files)
            {
                var fileName = ContentDispositionHeaderValue
                .Parse(file.ContentDisposition)
                .FileName
                .Trim('"');// FileName returns "fileName.ext"(with double quotes) in beta 3
                var imageFile = file;
                if (fileName.EndsWith(".jpg") || fileName.EndsWith(".png"))// Important for security if saving in webroot
                {
                    await UpdateAd(imageFile, fileName);
                }
            }
        }
        public async Task UpdateAd(IFormFile imageFile, string fileName)
        {
            var _blobStorage = new BlobStorage();
            await _blobStorage.UploadAndSaveBlobAsync(imageFile, fileName);
        }
        
    }
}

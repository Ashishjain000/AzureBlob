using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzureBlobStorage.Controllers
{
    public class UploadController : Controller
    {
        IConfiguration _configuration;
        public UploadController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Content("file not selected");

            // To parse the connection string to CloudStorage services
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
               _configuration["StorageConnectionString"].ToString()
                );

            // Create CloudClient object 
            var CloudClient = storageAccount.CreateCloudBlobClient();

            // Check for Container

            var container = CloudClient.GetContainerReference("filetest");
            // Check for container exists or not
            container.CreateIfNotExistsAsync();

            

            var blockBlob = container.GetBlockBlobReference("ashish12.jpg");
            var path = Path.Combine(
                 Directory.GetCurrentDirectory(), "wwwroot",
                 file.FileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            using (var filestream = System.IO.File.OpenRead(path))
            {
                await blockBlob.UploadFromStreamAsync(filestream);
            }
            return Content("File Uploded has been successfull");
        }
    }
}
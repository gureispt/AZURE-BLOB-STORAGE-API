using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;

namespace AZURE_BLOB_STORAGE_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArquivoController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly string _containerName;

        public ArquivoController(IConfiguration configuration)
        {
            _connectionString = configuration["BlobConnectionString"];
            _containerName = configuration["BlobContainerName"];
        }

        public IActionResult Upload(IFormFile arquivo)
        {
            BlobContainerClient containerClient = new(_connectionString, _containerName);

            BlobClient blob = containerClient.GetBlobClient(arquivo.FileName);

            using var data = arquivo.OpenReadStream();
            blob.Upload(data, new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders {ContentType = arquivo.ContentType }
            });
            return Ok(blob.Uri.ToString());
        }
    }
}
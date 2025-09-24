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

        [HttpPost("upload")]
        public IActionResult Upload(IFormFile arquivo)
        {
            BlobContainerClient containerClient = new(_connectionString, _containerName);

            BlobClient blob = containerClient.GetBlobClient(arquivo.FileName);

            using var data = arquivo.OpenReadStream();
            blob.Upload(data, new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = arquivo.ContentType }
            });
            return Ok(blob.Uri.ToString());
        }

        [HttpGet("download/{nomeArquivo}")]
        public IActionResult Download(string nomeArquivo)
        {
            BlobContainerClient containerClient = new(_connectionString, _containerName);
            BlobClient blob = containerClient.GetBlobClient(nomeArquivo);

            if (blob.Exists())
            {
                var retorno = blob.DownloadContent();
                return File(retorno.Value.Content.ToArray(), retorno.Value.Details.ContentType, blob.Name);
            }
            else
            {
                return NotFound("Arquivo não encontrado.");
            }
        }

        [HttpDelete("delete/{nomeArquivo}")]
        public IActionResult Delete(string nomeArquivo)
        {
            BlobContainerClient containerClient = new(_connectionString, _containerName);
            BlobClient blob = containerClient.GetBlobClient(nomeArquivo);

            blob.DeleteIfExists();
            return NoContent();
        }

        [HttpGet("listar")]
        public IActionResult Listar()
        {
            List<BlobDto> blobs = [];
            BlobContainerClient containerClient = new(_connectionString, _containerName);

            foreach (var blob in containerClient.GetBlobs())
            {
                blobs.Add(new BlobDto
                {
                    Nome = blob.Name,
                    Tipo = blob.Properties.ContentType,
                    Url = containerClient.Uri.AbsoluteUri + "/" + blob.Name
                });
            }

            return Ok(blobs);
        }
    }
}
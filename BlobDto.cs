using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AZURE_BLOB_STORAGE_API
{
    public class BlobDto // Data Transfer Object
    {
        public string Nome { get; set; }
        public string Tipo { get; set; }
        public string Url { get; set; }

    }
}
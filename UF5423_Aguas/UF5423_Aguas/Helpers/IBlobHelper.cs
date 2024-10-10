using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace UF5423_Aguas.Helpers
{
    public interface IBlobHelper
    {
        Task<Guid> UploadBlobAsync(IFormFile file, string containerName); // Upload browser form file.

        Task<Guid> UploadBlobAsync(byte[] file, string containerName); // Upload mobile app image.

        Task<Guid> UploadBlobAsync(string path, string containerName); // Upload URL path image.
    }
}

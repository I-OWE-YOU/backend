using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage.Blob;

namespace IOU.Common.Azure
{
    public class BlobStorageRepository : BaseStorageRepository
    {
        #region Fields

        private CloudBlobClient _blobClient;

        #endregion

        #region Constants

        private const string FILES_CONTAINER = "files";

        #endregion

        #region Constructors

        public BlobStorageRepository(string connectionString) : base(connectionString)
        {
            _blobClient = _storageAccount.CreateCloudBlobClient();
        }

        #endregion

        public async Task<string> GetFileContentAsync(string fileName)
        {
            var container = _blobClient.GetContainerReference(FILES_CONTAINER);
            var blob = container.GetBlockBlobReference(fileName);

            return await blob.DownloadTextAsync();
        }
    }
}
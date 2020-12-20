using Azure.Storage.Blobs;

namespace LetsMeatAPI {
  public class BlobClientFactory {
    public BlobClientFactory(string connectionString) {
      _connectionString = connectionString;
    }
    public BlobClient GetImageClient(string filename) {
      return new BlobClient(
        _connectionString,
        "images",
        filename
      );
    }
    private readonly string _connectionString;
  }
}

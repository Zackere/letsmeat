using Azure.Storage.Blobs;
using System;

namespace LetsMeatAPI {
  public class BlobClientFactory {
    public BlobClientFactory() : this("") { }
    public BlobClientFactory(string connectionString) {
      _connectionString = connectionString;
    }
    public BlobClient GetImageClient(string filename) {
      return new(
        _connectionString,
        "images",
        filename
      );
    }
    public BlobClient GetClientFromUri(Uri uri) {
      var container = uri.Segments[^2];
      container = container.Remove(container.Length - 1);
      return new(
        _connectionString,
        container,
        uri.Segments[^1]
      );
    }
    private readonly string _connectionString;
  }
}

using BudgetTracker.Storage.Models;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.Control.V2;
using Google.Cloud.Storage.V1;

namespace BudgetTracker.Storage.Services;

public class GoogleCloudStorageService
{
    /// <summary>
    /// The Google credential used for authentication with Google Cloud services.
    /// This credential grants the necessary permissions to interact with Google Cloud Storage.
    /// </summary>
    protected readonly GoogleCredential _credential;

    /// <summary>
    /// The client used for performing common operations on Google Cloud Storage,
    /// such as uploading, downloading, and managing objects.
    /// </summary>
    protected readonly StorageClient _storageClient;

    /// <summary>
    /// The client used for advanced, programmatic control over Google Cloud Storage buckets,
    /// including creating, updating, and deleting bucket-level settings.
    /// </summary>
    protected readonly StorageControlClient _storageControlClient;

    /// <summary>
    /// The name of the Google Cloud Storage bucket used by this class.
    /// </summary>
    protected readonly string _bucketName;

    // TODO: Update with ENV vars or with Secret configuration 
    public GoogleCloudStorageService()
    {
        _credential = GoogleCredential.FromFile(Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS"));
        _storageClient = StorageClient.Create(_credential);
        _bucketName = "receipt_dummy_storage"; // TODO: Need to change name and fetch from appsettings or env secret
        _storageControlClient = StorageControlClient.Create();
    }

    // gcloud storage buckets notifications create gs://[YOUR_BUCKET_NAME] --topic=[YOUR_TOPIC_NAME] --event-types=OBJECT_FINALIZE
    // to setup notification link between pub/sub and gcp cloud storage
    public string GenerateUploadSignedUrl(string fileName, UploadFileDto file)
    {
        TimeSpan expireIn = TimeSpan.FromMinutes(10);
        UrlSigner signer = UrlSigner.FromCredential(GoogleCredential.GetApplicationDefault());
        UrlSigner.RequestTemplate template = UrlSigner.RequestTemplate
            .FromBucket("receipt_dummy_storage")
            .WithHttpMethod(HttpMethod.Put)
            .WithContentHeaders(new Dictionary<string, IEnumerable<string>>
            {
                { "Content-Type", new string[] { file.ContentType } }
            })
            .WithQueryParameters(new Dictionary<string, IEnumerable<string>>
            {
                { "x-transaction-id", new string[] { file.TransactionId.ToString() } },
            })
            .WithRequestHeaders(new Dictionary<string, IEnumerable<string>>
            {
                { "x-goog-meta-transaction-id", new string[] { file.TransactionId.ToString() } }
            })
            .WithObjectName(fileName);
        
        string url = signer.Sign(requestTemplate: template, options: UrlSigner.Options.FromDuration(expireIn));

        return url;
    }
}
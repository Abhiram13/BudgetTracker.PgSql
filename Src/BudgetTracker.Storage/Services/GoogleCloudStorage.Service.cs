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
    public string GenerateUploadSignedUrl(string fileName)
    {
        UrlSigner signer = UrlSigner.FromCredential(GoogleCredential.GetApplicationDefault());
        
        string url = signer.Sign(
            bucket: "receipt_dummy_storage",
            objectName: fileName,
            duration: TimeSpan.FromMinutes(10),
            httpMethod: HttpMethod.Put,
            signingVersion: SigningVersion.V4
        );

        return url;
    }
}
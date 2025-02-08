namespace Infrastructure.FileStorage.Settings;

/// <summary>
/// Amazon S3 ayarları
/// </summary>
public class AmazonS3Settings
{
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string BucketName { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
}
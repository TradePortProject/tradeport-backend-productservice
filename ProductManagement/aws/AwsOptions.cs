namespace ProductManagement.aws
{
    public class AwsOptions
    {
        public string Profile { get; set; } = "";
        public string Region { get; set; } = "";
        public S3Options S3 { get; set; } = new();
        public string CdnBaseUrl { get; set; } = "";   // e.g., https://dxxxx.cloudfront.net
    }
    public class S3Options
    {
        public string BucketName { get; set; } = "";
        public int UploadExpiryMinutes { get; set; }
        public int ViewExpiryMinutes { get; set; }
    }

}

namespace ProductManagement.Utilities
{
    public class Util
    {
        public static string GetContentType(string key)
        {
            var ext = Path.GetExtension(key).ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".webp" => "image/webp",
                ".gif" => "image/gif",
                ".svg" => "image/svg+xml",
                _ => "application/octet-stream"
            };
        }
    }
}

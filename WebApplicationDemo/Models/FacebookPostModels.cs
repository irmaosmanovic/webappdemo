namespace WebApplicationDemo.Models
{
    public class FacebookPostModels
    {
        public post[] data;
    }
    public class post
    {
        public string created_time { get; set; }
        public string id { get; set; }

        public postAttachments attachments { get; set; }
    }

    public class postAttachments
    {
        public attachment[] data { get; set; }
    }
    public class attachment
    {
        public string description { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public string url { get; set; }

        public media media { get; set; }
        public target target { get; set; }
    }

    public class media
    {
        public image image { get; set; }
    }
    public class image
    {
        public int height { get; set; }
        public int width { get; set; }
        public string src { get; set; }
    }
    public class target
    {
        public string id { get; set; }
        public string url { get; set; }
    }
}
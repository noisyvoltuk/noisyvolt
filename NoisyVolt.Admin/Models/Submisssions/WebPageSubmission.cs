using System.ComponentModel.DataAnnotations;

namespace NoisyVolt.Admin.Models.Submissions
{
    public class WebPageSubmission
    {
        [Required]
        public Uri PageUrl { get; set; } = new Uri("https://www.noisyvolt.com");

        public string? Subjects { get;  set; }

        public Boolean CrawlFromHere { get; set; }
    }
}

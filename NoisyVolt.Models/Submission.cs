using System.Runtime.InteropServices;

namespace NoisyVolt.Models
{
    public class Submission
    {

        public int Id { get; set; }
        public string PageUrl { get; set; }
        
        public string? SubmissionType { get; set; }
        public string? Subjects { get; set; }

        public string? Status { get; set; }

        public Boolean CrawlFromHere { get; set; }

    }
}
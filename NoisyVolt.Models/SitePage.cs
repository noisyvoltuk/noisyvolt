using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoisyVolt.Models
{
    public class SitePage
    {
        public int Id { get; set; }

        public Guid SystemGuid { get; set; }
        public int SiteId { get; set; }
        public int SubmissionId { get; set; }
        public string LastHttpStatus { get; set; }
        public string ResolvedUrl { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string Keywords { get; set; }

    }
}

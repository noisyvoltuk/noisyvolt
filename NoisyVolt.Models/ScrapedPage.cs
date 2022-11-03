using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoisyVolt.Models
{
    public class ScrapedPage
    {
        public string UsableText { get; set; }
        public List<string> InternalLinks { get; set; }
        public List<string> ExternalLinks { get; set; }
    }
}

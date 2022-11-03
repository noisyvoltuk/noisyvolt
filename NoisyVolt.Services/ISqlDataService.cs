using NoisyVolt.Models;

namespace NoisyVolt.Services
{
    public interface ISqlDataService
    {
        public int AddSubmission(Submission submission);

        public List<Submission> GetSubmissions();

        public List<Submission> GetNewSubmissions();

        public int GetPageByUrl(string PageUrl);

        public int SavePage(SitePage sitePage);
    }
}

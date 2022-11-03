using NoisyVolt.Models;

namespace NoisyVolt.Services
{
    public interface IPageService
    {

        public ScrapedPage Scrape(string PageToScrape);
        public string BuildEmbedCard(SitePage sitePage);
        public void GetText();
        public void GetExternalLinks();

    }
}

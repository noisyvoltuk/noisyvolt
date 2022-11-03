using NoisyVolt.Models;
using HtmlAgilityPack;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;

namespace NoisyVolt.Services
{
    public class PageService : IPageService
    {
        public string BuildEmbedCard(SitePage sitePage)
        {
            
            /*
             
             <blockquote class="embed-card-std">
               <h4>
                 <a href="https://www.bbc.co.uk/news/uk-politics-63486665">Not enough asylum claims are processed admits Rishi Sunak</a>
               </h4>
               <p>Rishi Sunak promises to fix the system, but Keir Starmer says the government has "lost control".</p>
             </blockquote>
            
            */
            StringBuilder sbCard = new StringBuilder();

            sbCard.Append("<blockquote class=\"embed-card-std\">");
            sbCard.Append($"<a href='{sitePage.ResolvedUrl}'>");
            sbCard.Append($"<h4>{sitePage.Title}</h4>");
            sbCard.Append($"<img src='{sitePage.ImageUrl}' />");
            sbCard.Append("</a>");
            sbCard.Append($"<p>{sitePage.Description}</p>");
           
            sbCard.Append("read more on site...");

            sbCard.Append("</blockquote>");
            return sbCard.ToString();
            
        }

        void IPageService.GetExternalLinks()
        {
            throw new NotImplementedException();
        }

        void IPageService.GetText()
        {
            throw new NotImplementedException();
        }


        ScrapedPage IPageService.Scrape(string PageToScrape)
        {
            ScrapedPage sp = new ScrapedPage();

            HtmlWeb web = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = web.Load(PageToScrape);

            sp.UsableText = GetPageText(doc);


            return sp;
        }

        string GetPageText(HtmlDocument doc)
        {

            StringBuilder sb = new StringBuilder();
            IEnumerable<HtmlNode> nodes = doc.DocumentNode.Descendants().Where(n =>
                n.NodeType == HtmlNodeType.Text &&
                n.ParentNode.Name != "script" &&
                n.ParentNode.Name != "style");
            foreach (HtmlNode node in nodes)
            {
                string text = node.InnerText;
                if (!string.IsNullOrEmpty(text))
                    sb.AppendLine(text.Trim());
            }
            return Regex.Replace(sb.ToString(), @"\r\n?|\n", " "); ;
        }
    }
}

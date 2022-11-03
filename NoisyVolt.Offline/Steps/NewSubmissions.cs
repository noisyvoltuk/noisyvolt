using NoisyVolt.Services;
using NoisyVolt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using static System.Net.Mime.MediaTypeNames;
using Azure;
using System.Security.Policy;

namespace NoisyVolt.Offline.Steps
{
    public class NewSubmissions
    {
        private ISqlDataService _sqlDataService;
        private IPageService _pageService;
        private INoSqlDataService _noSqlDataService;
        private struct HttpHeadResp
        {
            public string OrigUrl;
            public Boolean IsSuccess;
            public String HttpStatus;
            public string ResolvedUrl;

        }

        public NewSubmissions(ISqlDataService sqlDataService, IPageService pageService, INoSqlDataService noSqlDataService)
        {
            _sqlDataService = sqlDataService;
            _pageService = pageService;
            _noSqlDataService = noSqlDataService;
        }
        public async void Process()
        {
           var newSubs =  _sqlDataService.GetNewSubmissions();

            foreach (var sub in newSubs)
            {
                //validate Url - parse it
                HttpHeadResp checkUrl =  ValidateUrl(sub.PageUrl);

               if (checkUrl.IsSuccess)
               {
                    //check already seen
                    if (_sqlDataService.GetPageByUrl(checkUrl.ResolvedUrl) <= 0)
                    {
                        var scrapedPage = _pageService.Scrape(checkUrl.ResolvedUrl);

                        SitePage sitePage = BuildNewPage(sub, checkUrl, scrapedPage).Result;

                        _sqlDataService.SavePage(sitePage);

                        if (sub.CrawlFromHere)
                        {
                         //   SaveInternalLinks(scrapedPage.InternalLinks);
                         //   SaveExternalLinks(scrapedPage.ExternalLinks);
                        }

                        //build the embed card from SQL and save to Cosmos
                        EmbedCard embedCard = new EmbedCard();
                        embedCard.id = sitePage.SystemGuid.ToString();
                        embedCard.EmbedHtml = _pageService.BuildEmbedCard(sitePage);

                        var cdbResp =   _noSqlDataService.SaveEmbedCard(embedCard).Result;

                        var c = cdbResp;
                    }                              
               }

          


            }//end subs loop
        }

        async Task<SitePage> BuildNewPage(Submission sub, HttpHeadResp headResp, ScrapedPage scrapedPage)
        {

            SitePage sp = new SitePage();

            sp.SubmissionId = sub.Id;
            sp.LastHttpStatus = headResp.HttpStatus;

            sp.SystemGuid = Guid.NewGuid();

            sp.LastHttpStatus = headResp.HttpStatus;
            sp.ResolvedUrl = headResp.ResolvedUrl;

            //build meta
            HttpClient httpClient = new HttpClient();
            string html = await httpClient.GetStringAsync(sp.ResolvedUrl);

            // Load the HTML Document
            // AngleSharp.Html.Dom.IHtmlDocument
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);

            string Title = "";
            var titleElement = document.DocumentNode.SelectSingleNode("//meta[@property='og:title']");
            //            .FirstOrDefault();
            if (titleElement != null)
                Title = titleElement.GetAttributeValue("content", "");
            else
            {
                // Try and get from the <TITLE> element
                titleElement = document.DocumentNode.SelectSingleNode("//title");
                                      
                if (titleElement != null)
                    Title = titleElement.InnerText;
            }
            sp.Title = Title;

            // Try and get the description from OpenGraph data
            string Description = "";
            var descriptionElement = document.DocumentNode.SelectSingleNode("//meta[@property='og:description']");
                                   
            if (descriptionElement != null)
                Description = descriptionElement.GetAttributeValue("content", "");
            else
            {
                descriptionElement = document.DocumentNode.SelectSingleNode("//meta[@name='Description']");
                                     //   .FirstOrDefault();
                if (descriptionElement != null)
                    Description = descriptionElement.GetAttributeValue("content", "");
            }
            sp.Description = Description;

            // Try and get the images from OpenGraph data
            List<Uri> Images = new List<Uri>();
            var imageElements = document.DocumentNode.SelectNodes("//meta[@property='og:image']");
            foreach (var imageElement in imageElements)
            {
                Uri imageUri = null;
                string imageUrl = imageElement.GetAttributeValue("content", "");
                if (imageUrl != null && Uri.TryCreate(imageUrl, UriKind.Absolute, out imageUri))
                    Images.Add(imageUri);
            }
            if (Images.Count > 0)
            {
                sp.ImageUrl = Images[0].AbsoluteUri;
            }

            return sp;
        }

       
        HttpHeadResp ValidateUrl(string url)
        {
            HttpHeadResp resp = new HttpHeadResp();
            resp.OrigUrl = url;
            if (!url.StartsWith("http"))
            {
                url = "https://" + url;
            }

            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Timeout = 3000;
                //request.AllowAutoRedirect = false; // find out if this site is up and don't follow a redirector
                request.Method = "HEAD";

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    resp.IsSuccess= true;
                    //  resp.title = response.
                    resp.ResolvedUrl = response.ResponseUri.ToString();
                    var h = response.Headers;
                    int sc = (int)response.StatusCode;
                    resp.HttpStatus = sc.ToString();
                    
                }
            }
            catch (WebException ex) when ((ex.Response as HttpWebResponse)?.StatusCode == HttpStatusCode.NotFound)
            {
                if (url.StartsWith("http://"))
                {
                    resp = ValidateUrl(url.Replace("http://", "https://"));
                }
            }
            catch (Exception ex)
            {
               // var tt = ex.Message;
                resp.IsSuccess = false;
            }

            return resp;
        }
    }
}

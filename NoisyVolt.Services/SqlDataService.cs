using NoisyVolt.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using System.Data;


namespace NoisyVolt.Services
{
    public class SqlDataService : ISqlDataService
    {
        private IConfiguration _configuration;

        private SqlConnection _conn;
        public SqlDataService(IConfiguration configuration)
        {
            _configuration = configuration;
            _conn = new SqlConnection(_configuration.GetConnectionString("AzureSQL"));
            _conn.Open();
        }
        public List<Submission> GetSubmissions()
        {
            var subs = new List<Submission>();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = _conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "up_SELECT_Submissions";

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var s = new Submission();
                        s.PageUrl = reader.GetString("PageUrl");
                        s.SubmissionType = reader.GetString("SubmissionType");
                        s.Subjects = reader.GetString("Subjects");
                        subs.Add(s);
                    }
                }
            }
            return subs;
        }

        public List<Submission> GetNewSubmissions()
        {
            var subs = new List<Submission>();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = _conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "up_SELECT_NewSubmissions";

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var s = new Submission();
                        s.Id = reader.GetInt32("Id");
                        s.PageUrl = reader.GetString("PageUrl");
                        s.SubmissionType = reader.GetString("SubmissionType");
                        s.Subjects = reader.GetString("Subjects");
                        subs.Add(s);
                    }
                }
            }
            return subs;
        }

        public int AddSubmission(Submission submission)
        {
            int newId = -1;
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = _conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "up_INSERT_Submission";
                cmd.Parameters.Add(new SqlParameter("@PageUrl", submission.PageUrl));
                cmd.Parameters.Add(new SqlParameter("@SubmissionType", (submission.SubmissionType ?? "").ToString()));
                cmd.Parameters.Add(new SqlParameter("@Subjects", (submission.Subjects ?? "").ToString()));
                cmd.Parameters.Add(new SqlParameter("@CrawlFromHere", submission.CrawlFromHere));

                newId = Convert.ToInt32(cmd.ExecuteScalar());
                 
            }
            return newId;
        }

        public int GetPageByUrl(string PageUrl)
        {
            int pageId = -1;
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = _conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "up_SELECT_PageByUrl";
                cmd.Parameters.Add(new SqlParameter("@PageUrl", PageUrl));
             
                pageId = Convert.ToInt32(cmd.ExecuteScalar());

            }
            return pageId;
        }

        public int SavePage(SitePage sitePage)
        {
            int newId = -1;
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = _conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "up_INSERT_SitePage";
                cmd.Parameters.Add(new SqlParameter("@SiteId", -1));
                cmd.Parameters.Add(new SqlParameter("@SubmissionId", sitePage.SubmissionId));
                cmd.Parameters.Add(new SqlParameter("@SystemGuid", sitePage.SystemGuid));
                cmd.Parameters.Add(new SqlParameter("@LastHttpStatus", (sitePage.LastHttpStatus ?? "").ToString()));
                cmd.Parameters.Add(new SqlParameter("@ResolvedUrl", sitePage.ResolvedUrl));
                cmd.Parameters.Add(new SqlParameter("@Title", (sitePage.Title ?? "").ToString()));
                cmd.Parameters.Add(new SqlParameter("@Description", (sitePage.Description ?? "").ToString()));
                cmd.Parameters.Add(new SqlParameter("@ImageUrl", (sitePage.ImageUrl ?? "").ToString()));
                cmd.Parameters.Add(new SqlParameter("@Keywords", (sitePage.Keywords ?? "").ToString()));

                newId = Convert.ToInt32(cmd.ExecuteScalar());

            }
            return newId;
        }

    }
}

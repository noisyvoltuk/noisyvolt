using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoisyVolt.Models;
using NoisyVolt.Services;

namespace NoisyVolt.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SubmissionController : ControllerBase
    {

        private ISqlDataService _sqlDataService;

        public SubmissionController(ISqlDataService sqlDataService)
        {
            _sqlDataService = sqlDataService;
        }


        

        // GET: SubmissionController/Create
       

        // POST: SubmissionController/Create
        [HttpPost]
   
        public string Create(Submission submission)
        {
            _sqlDataService.AddSubmission(submission);

            return "ok";
        }

       
    }
}

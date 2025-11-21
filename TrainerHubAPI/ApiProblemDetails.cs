using Microsoft.AspNetCore.Mvc;

namespace TrainerHubAPI
{
    public class ApiProblemDetails : ProblemDetails
    {
        public string? TraceId { get; set; }
    }
}

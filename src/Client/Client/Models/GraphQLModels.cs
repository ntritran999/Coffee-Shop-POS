using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace Client.Models
{
    public class GraphQLRequest
    {
        public string query { get; set; }
        public object variables { get; set; }
    }

    public class GraphQLResponse<T>
    {
        public T data { get; set; }
        public List<GraphQLError> errors { get; set; }
    }

    public class GraphQLError
    {
        public string message { get; set; }
    }
    public class LoginData
    {
        public string token { get; set; }
        public Account account { get; set; }
    }
}
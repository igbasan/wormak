using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PostService.Models.Implementations
{
    public class PostResponse
    {
        [JsonPropertyName("isSuccessful")]
        public bool IsSuccessful { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }

    public class PostObjectResponse<T> : PostResponse
    {
        [JsonPropertyName("result")]
        public T Result { get; set; }
    }

    public class PostListResponse<T> : PostResponse
    {
        [JsonPropertyName("result")]
        public List<T> Result { get; set; }
        public long TotalCount { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace InterUserService.Models.Implemetations
{
    public class InterUserResponse
    {
        [JsonPropertyName("isSuccessful")]
        public bool IsSuccessful { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
    public class InterUserBoolResponse : InterUserResponse
    {
        public bool Result { get; set; }
    }

    public class InterUserObjectResponse<T> : InterUserResponse
    {
        public T Result { get; set; }
    }

    public class InterUserListResponse<T> : InterUserResponse
    {
        [JsonPropertyName("result")]
        public List<T> Result { get; set; }
        public long TotalCount { get; set; }
    }

    public class InterUserListWithRatingResponse<T> : InterUserListResponse<T>
    {
        public double AverageRating { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProfileService.Models.Implementations
{
    public class ProfileResponse
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
    }

    public class ProfileObjectResponse<T> : ProfileResponse 
    {
        public T Result { get; set; }
    }

    public class ProfileListResponse<T> : ProfileResponse 
    {
        public List<T> Result { get; set; }
    }
}

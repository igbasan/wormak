using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace InterUserService.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProfileType
    {
        [EnumMember(Value = "GeneralUser")]
        GeneralUser = 0,
        [EnumMember(Value = "Company")]
        Company,
        [EnumMember(Value = "Professional")]
        Professional
    }
}

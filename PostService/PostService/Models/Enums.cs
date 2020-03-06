using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PostService.Models
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

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Interest
    {
        [EnumMember(Value = "Art")]
        Art = 0,
        [EnumMember(Value = "Business")]
        Business,
        [EnumMember(Value = "Pleasure")]
        Pleasure,
        [EnumMember(Value = "Science")]
        Science
    }
}

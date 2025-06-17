using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WhiteListing_Backend.Models
{
    public class Whitelisting_MemberDTO
    {
        //public Guid Id { get; set; }


        [JsonPropertyName("member_no")]
        public required string Member_No { get; set; }

        [JsonPropertyName("member_name")]
        public required string Member_Name { get; set; }

        [JsonPropertyName("id_no")]
        public required string Id_No { get; set; }

        [JsonPropertyName("phone_no")]
        public required string Phone_No { get; set; }

        [JsonPropertyName("client_id")]
        public required string Client_id { get; set; }

        [JsonPropertyName("status")]
        public required StatusOption Status { get; set; }



    }

    public enum StatusOption
    {
        [Display(Name = "White-Listed")]
        WhiteListed,

        [Display(Name = "Pending")]
        Pending,

        [Display(Name = "Rejected")]
        Rejected
    }

}

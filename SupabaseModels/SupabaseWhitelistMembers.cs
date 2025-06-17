using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.ComponentModel.DataAnnotations;
using ColumnAttribute = Supabase.Postgrest.Attributes.ColumnAttribute;
using TableAttribute = Supabase.Postgrest.Attributes.TableAttribute;

namespace WhiteListing_Backend.SupabaseModels
{
    [Table("Whitelisted Members")]
    public class SupabaseWhitelistMembers : BaseModel
    {
        [PrimaryKey("unique_id", false)]
        public virtual Guid ID { get; set; } = Guid.NewGuid();


        [Column("id_no")]
        [StringLength(10, MinimumLength = 8, ErrorMessage = "Must be at least 8 characters long.")]
        public string Id_No { get; set; }


        [Column("member_no")]
        [Required(ErrorMessage = "Member no is required.")]
        public string Member_No { get; set; }

        [Column("member_name")]
        [Required(ErrorMessage = "Member name is required.")]
        public string Member_Name { get; set; }

        [Column("phone_no")]
        [Required(ErrorMessage = "Member phone no is required.")]
        public string Phone_No { get; set; }

        [Column("client_id")]
        [Required(ErrorMessage = "Client ID is required.")]
        public string Client_id { get; set; }

        [Column("status")]
        [Required(ErrorMessage = "Status is required.")]
        public StatusOption Status { get; set; }
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

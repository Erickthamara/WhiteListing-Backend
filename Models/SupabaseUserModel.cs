using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using ColumnAttribute = Supabase.Postgrest.Attributes.ColumnAttribute;
using TableAttribute = Supabase.Postgrest.Attributes.TableAttribute;

namespace WhiteListing_Backend.Models
{
    [Table("userstable")]
    public class SupabaseUserModel : BaseModel
    {
        [PrimaryKey("Id", false)]
        public virtual Guid Id { get; set; } = Guid.NewGuid();

        [Column("username")]
        public virtual string UserName { get; set; }
        [Column("email")]
        public virtual string Email { get; set; }
        [Column("emailconfirmed")]
        public virtual bool EmailConfirmed { get; set; }
        [Column("passwordhash")]
        public virtual String PasswordHash { get; set; }
        [Column("normalizedusername")]
        public string NormalizedUserName { get; internal set; }
        [Column("authenticationtype")]
        public string AuthenticationType { get; set; }
        [Column("isauthenticated")]
        public bool IsAuthenticated { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("Id_No")]
        public string IdNo { get; set; }
    }
}

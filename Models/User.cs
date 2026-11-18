using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeamTaskTracking.Web.Models;

[Table("Users")]
public class User
{
    [Key]
    [Column("UserID")]
    public int UserID { get; set; }

    [Required]
    [StringLength(150)]
    [Column("FullName")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    [Column("Email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    [Column("PasswordHash")]
    public string PasswordHash { get; set; } = string.Empty;

    [Column("CreatedAt")]
    public DateTime? CreatedAt { get; set; }
}
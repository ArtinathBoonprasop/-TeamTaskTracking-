using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeamTaskTracking.Web.Models;

[Table("Projects")]
public class Project
{
    [Key]
    [Column("ProjectID")]
    public int ProjectID { get; set; }

    [Required]
    [StringLength(200)]
    [Column("ProjectName")]
    public string ProjectName { get; set; } = string.Empty;

    [Column("Description")]
    public string? Description { get; set; }

    [Column("StartDate")]
    public DateTime? StartDate { get; set; }

    [Column("EndDate")]
    public DateTime? EndDate { get; set; }

    [Column("Status")]
    [StringLength(30)]
    public string? Status { get; set; } = "Active";

    [Column("CreatedAt")]
    public DateTime? CreatedAt { get; set; }
}
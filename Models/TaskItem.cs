using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeamTaskTracking.Web.Models;

[Table("Tasks")]
public class TaskItem
{
    [Key]
    [Column("TaskID")]
    public int TaskID { get; set; }

    [Required]
    [Column("ProjectID")]
    public int ProjectID { get; set; }

    [Required]
    [StringLength(200)]
    [Column("TaskName")]
    public string TaskName { get; set; } = string.Empty;

    [Column("Description")]
    public string? Description { get; set; }

    [Column("DueDate")]
    public DateTime? DueDate { get; set; }

    [Column("Status")]
    [StringLength(30)]
    public string? Status { get; set; } = "Pending";

    [Column("CreatedAt")]
    public DateTime? CreatedAt { get; set; }
}
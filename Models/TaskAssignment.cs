using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeamTaskTracking.Web.Models;

[Table("TaskAssignments")]
public class TaskAssignment
{
    [Key]
    [Column("AssignmentID")]
    public int AssignmentID { get; set; }

    [Required]
    [Column("TaskID")]
    public int TaskID { get; set; }

    [Required]
    [Column("UserID")]
    public int UserID { get; set; }
}
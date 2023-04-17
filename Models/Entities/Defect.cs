using System.ComponentModel.DataAnnotations;

namespace Digichlist.Bot.Models.Entities
{
    /// <summary>
    /// The entity that represents the defect.
    /// </summary>
    public class Defect
    {
       /// <summary>
       /// The unique identifier of the defect.
       /// </summary>
       public int Id { get; set; }
       
       /// <summary>
       /// The number of the room where the defect is spotted.
       /// </summary>
       public int RoomNumber { get; set; }
       
       /// <summary>
       /// The description of the defect.
       /// </summary>
       public string? Description { get; set; }
       
       /// <summary>
       /// The date the defect was created.
       /// </summary>
       public DateTime CreatedAt { get; set; }
       
       /// <summary>
       /// The name of the creator.
       /// </summary>
       public string CreatedBy { get; set; }
       
       /// <summary>
       /// The date and time the defect was closed.
       /// </summary>
       public DateTime? ClosedAt { get; set; }
       
       /// <summary>
       /// The date and time the defect status was changed.
       /// </summary>
       public DateTime? StatusChangedAt { get; set; }
       
       /// <summary>
       /// The status of the defect.
       /// Null implies this defect was not assigned.
       /// </summary>
       public int Status { get; set; }
       
       /// <summary>
       /// The assigned worker identifier.
       /// </summary>
       public int? UserId { get; set; }
       
       /// <summary>
       /// More information about the assigned worker.
       /// </summary>
       public User? AssignedWorker { get; set; }
       
       /// <summary>
       /// The related images.
       /// </summary>
       public List<DefectImage> DefectImages { get; set; } = new();
    }
}

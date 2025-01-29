public class Appointment
{
    public int Id { get; set; }
    public int DoctorId { get; set; }
    public int UserId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public bool IsCompleted { get; set; }
    public string AreaOfInterest {get;set;}
    public string City {get;set;}
    public string Town {get;set;}
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

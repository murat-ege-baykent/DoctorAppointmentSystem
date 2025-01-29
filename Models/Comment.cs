public class Comment
{
    public int Id { get; set; }
    public int AppointmentId { get; set; }
    public string UserName { get; set; }
    public int Rating { get; set; } // Example: 1-5
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
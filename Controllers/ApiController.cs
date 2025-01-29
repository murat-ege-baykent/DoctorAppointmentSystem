using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;
using MongoDB.Driver;

[ApiController]
[Route("api/[controller]")]
[ApiVersion("1.0")]
public class AppointmentsController : ControllerBase
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IMongoClient _mongoClient;

    public AppointmentsController(IConnectionMultiplexer redis, IMongoClient mongoClient)
    {
        _redis = redis;
        _mongoClient = mongoClient;
    }

    [HttpGet]
    public IActionResult GetAppointments([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        // Fetch appointments from Redis
        var db = _redis.GetDatabase();
        var appointments = db.ListRange("appointments", 0, -1); 
        var paginatedResult = appointments
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToList();

    var totalItems = appointments.Count();
    return Ok(new 
    { 
        Data = paginatedResult, 
        Page = page, 
        PageSize = pageSize, 
        TotalItems = totalItems, 
        TotalPages = (int)Math.Ceiling((double)totalItems / pageSize) 
    });
    }

    [HttpPost]
    public IActionResult CreateAppointment([FromBody] Appointment appointment)
    {
        // Store appointment in Redis
        var db = _redis.GetDatabase();
        db.ListRightPush("appointments", appointment.ToString());  
        return Ok("Appointment created");
    }
}

[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly IMongoClient _mongoClient;

    public CommentsController(IMongoClient mongoClient)
    {
        _mongoClient = mongoClient;
    }

    [HttpPost]
    public IActionResult PostComment([FromBody] Comment comment)
    {
        var database = _mongoClient.GetDatabase("DoctorAppointments");
        var collection = database.GetCollection<Comment>("comments");
        collection.InsertOne(comment);
        return Ok("Comment posted");
    }

    [HttpGet("{appointmentId}")]
    public IActionResult GetComments(int appointmentId)
    {
        var database = _mongoClient.GetDatabase("DoctorAppointments");
        var collection = database.GetCollection<Comment>("comments");
        var comments = collection.Find(c => c.AppointmentId == appointmentId).ToList();
        return Ok(comments);
    }
}

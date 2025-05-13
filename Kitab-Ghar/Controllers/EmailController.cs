using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class EmailController : ControllerBase
{
    private readonly IEmailSender _emailSender;

    public EmailController(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendEmail([FromBody] EmailRequest request)
    {
        if (string.IsNullOrEmpty(request.To) || string.IsNullOrEmpty(request.Subject) || string.IsNullOrEmpty(request.Body))
        {
            return BadRequest("All fields are required.");
        }

        await _emailSender.SendEmailAsync(request.To, request.Subject, request.Body);
        return Ok("Email sent successfully.");
    }
}

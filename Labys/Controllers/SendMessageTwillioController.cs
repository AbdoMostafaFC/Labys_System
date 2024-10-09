using Labys.models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Labys.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendMessageTwilioController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly SmsService _smsService;
        public SendMessageTwilioController(IConfiguration configuration, SmsService smsService)
        {
            this.configuration = configuration;
            _smsService = smsService;
        }
        
        private const string fromWhatsAppNumber = "whatsapp:+15203532166"; 

       

        [HttpPost("send-message")]
        public IActionResult SendMessage([FromBody] WhatsAppMessageRequest request)
        {
            try
            {
                // Get Twilio credentials from configuration
                string accountSid = configuration["Twilio:AccountSid"];
                string authToken = configuration["Twilio:AuthToken"];
                string fromNumber = configuration["Twilio:WhatsAppFromNumber"];

                // Initialize Twilio client
                TwilioClient.Init(accountSid, authToken);

                // Create and send the message
                var message = MessageResource.Create(
                    from: new PhoneNumber($"whatsapp:{fromNumber}"),
                    to: new PhoneNumber($"whatsapp:{request.ToNumber}"),
                    body: request.Message
                );

                return Ok(new { message.Sid, Status = "Sent" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }


        // POST api/sms/send
        [HttpPost("send")]
        public IActionResult SendSms([FromBody] SmsRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.PhoneNumber) || string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest("Phone number and message cannot be empty.");
            }

            _smsService.SendSms(request.PhoneNumber, request.Message);
            return Ok("SMS sent successfully!");
        }
    }

    
    public class WhatsAppMessageRequest
    {
        public string ToNumber { get; set; }
        public string Message { get; set; }
    }

    public class SmsRequest
    {
        public string PhoneNumber { get; set; }
        public string Message { get; set; }
    }
}
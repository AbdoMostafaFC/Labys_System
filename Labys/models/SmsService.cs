using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Labys.models
{
    public class SmsService
    {
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _fromPhoneNumber;

        public SmsService(IConfiguration config)
        {
            _accountSid = config["Twilio:AccountSid"];
            _authToken = config["Twilio:AuthToken"];
            _fromPhoneNumber = config["Twilio:WhatsAppFromNumber"];
        }

        public void SendSms(string toPhoneNumber, string message)
        {
            TwilioClient.Init(_accountSid, _authToken);

            var messageResult = MessageResource.Create(
                body: message,
                from: new Twilio.Types.PhoneNumber(_fromPhoneNumber),
                to: new Twilio.Types.PhoneNumber(toPhoneNumber)
            );

            Console.WriteLine($"Message sent: {messageResult.Sid}");
        }
    }
}

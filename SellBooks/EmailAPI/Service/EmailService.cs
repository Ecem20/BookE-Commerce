using RabbitMQ.Client;
using System.Text;
using EmailAPI.Data;
using EmailAPI.Models;

namespace EmailAPI.Service
{
    public class EmailService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ApplicationDbContext _dbContext;


        public EmailService(string hostName, string queueName, ApplicationDbContext dbContext)
        {
            var factory = new ConnectionFactory() { HostName = hostName };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: queueName,
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);
            _dbContext = dbContext;
        }

        public void SendEmail(string receiverEmail, string subject, string body)
        {
            var email = new Email
            {
                ReceiverEmail = receiverEmail,
                Subject = subject,
                Body = body
            };
            _dbContext.emails.Add(email);
            _dbContext.SaveChanges();

            var message = Newtonsoft.Json.JsonConvert.SerializeObject(email);
            var bodyBytes = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "",
                                  routingKey: "email_queue1",
                                  basicProperties: null,
                                  body: bodyBytes);
            Console.WriteLine(" [x] Sent {0}", message);
        }
    }
}

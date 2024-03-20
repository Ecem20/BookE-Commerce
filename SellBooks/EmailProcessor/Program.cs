using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace EmailProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "email_queue1",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(message);
                    var emailDto = Newtonsoft.Json.JsonConvert.DeserializeObject<EmailAPI.Models.Dto.EmailDto>(message);
                    SendEmail(emailDto);
                };
                channel.BasicConsume(queue: "email_queue1",
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine("Press [enter] to exit.");
                Console.ReadLine();
            }
        }

        static void SendEmail(EmailAPI.Models.Dto.EmailDto emailDto)
        {
            try
            {
                var mail = new MailMessage();
                mail.From = new MailAddress("cansinhakko@gmail.com");
                mail.To.Add(emailDto.ReceiverEmail);
                mail.Subject = emailDto.Subject;
                mail.Body = emailDto.Body;

                var client = new SmtpClient("smtp.gmail.com", 587);
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("cansinhakko@gmail.com", "irbo yrpq cumv vpmk");
                client.EnableSsl = true;
                client.Send(mail);
                Console.WriteLine("Mail sent successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to send email: " + ex.Message);
            }
        }
    }
}
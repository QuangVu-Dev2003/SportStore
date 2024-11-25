﻿using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace SportStore.BusinessLogicLayer.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");

            SmtpClient client = new SmtpClient
            {
                Port = emailSettings.GetValue<int>("Port"),
                Host = emailSettings["SmtpServer"],
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(emailSettings["Username"], emailSettings["Password"])
            };

            var mailMessage = new MailMessage(emailSettings["SenderEmail"], email, subject, htmlMessage)
            {
                IsBodyHtml = true
            };

            return client.SendMailAsync(mailMessage);
        }
    }
}
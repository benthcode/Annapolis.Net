using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Abstract.Work;
using Annapolis.Entity;
using System.Net.Mail;
using System.Net;
using System.Xml;
using System.Web;
using Annapolis.Shared.Model;

namespace Annapolis.Work
{
    internal class EmailContent
    {
        public string Subject { get; set; }
        public string Body { get; set; }
    }

    public class EmailWork : IEmailWork
    {

        private static Dictionary<EmailNotificationReason, EmailContent> EmailContentDict;
        private readonly ILoggingWork _loggingWork;
        private readonly ISettingWork _settingsWork;
        public EmailWork(ILoggingWork loggingWork, ISettingWork settingsWork)
        {
            _loggingWork = loggingWork;
            _settingsWork = settingsWork;
        }

        static EmailWork()
        {
            EmailContentDict = new Dictionary<EmailNotificationReason, EmailContent>();
            foreach (EmailNotificationReason emailType in Enum.GetValues(typeof(EmailNotificationReason)))
            {
                string fileLocation = null;
                switch (emailType)
                {
                    case EmailNotificationReason.UserCreateNotification:
                        fileLocation = @"~/Content/emails/UserCreateNotification.xml"; break;
                    case EmailNotificationReason.UserCreateVerification:
                        fileLocation = @"~/Content/emails/UserCreateVerification.xml"; break;
                    default:
                        break;
                }

                if (string.IsNullOrEmpty(fileLocation)) continue;

                XmlDocument doc = new XmlDocument();
                doc.Load(HttpContext.Current.Server.MapPath(fileLocation));

                EmailContentDict.Add(emailType, new EmailContent() { Subject = doc.SelectSingleNode(@"/email/subject").Value,
                                                                     Body = doc.SelectSingleNode(@"/email/body").Value });
           
            }
        }

        private string ReplaceTemplate(string text, string receiverName = null)
        {
            var defaultSetting = _settingsWork.GetDefaultSetting();
            text = text.Replace("#SiteName#", defaultSetting.ForumName).Replace("#SiteUrl#", defaultSetting.ForumUrl);
            if(receiverName != null)
            {
                text = text.Replace("#UserName#", receiverName);
            }
            return text;
        }

        /// <summary>
        /// Send a single emails
        /// </summary>
        /// <param name="emails"></param>
        public void SendMail(Email email)
        {
            SendMail(new List<Email> { email });
        }

        /// <summary>
        /// Send multiple emails
        /// </summary>
        /// <param name="emails"></param>
        public void SendMail(List<Email> emails)
        {
            try
            {
                if (emails != null && emails.Count > 0)
                {
                    var defaultSetting = _settingsWork.GetDefaultSetting();

                    var mySmtpClient = new SmtpClient(defaultSetting.SMTPHost);
                    if (!string.IsNullOrWhiteSpace(defaultSetting.SMTPUsername) && !string.IsNullOrWhiteSpace(defaultSetting.SMTPPassword))
                    {
                        mySmtpClient.Credentials = new NetworkCredential(defaultSetting.SMTPUsername, defaultSetting.SMTPPassword);
                    }

                    if (defaultSetting.SMTPEnableSSL.HasValue) { mySmtpClient.EnableSsl = defaultSetting.SMTPEnableSSL.Value; }
                    if (defaultSetting.SMTPPort.HasValue) { mySmtpClient.Port = defaultSetting.SMTPPort.Value; }
                    
                    foreach (var email in emails)
                    {
                        try
                        {
                            var content = EmailContentDict[email.NotificationReadon];
                            var msg = new MailMessage
                            {
                                IsBodyHtml = true,
                                Subject = ReplaceTemplate(content.Subject),
                                Body = ReplaceTemplate(content.Body),
                                From = new MailAddress(email.EmailFrom)
                            };
                            msg.To.Add(email.EmailTo);
                            mySmtpClient.Send(msg);
                        }
                        catch (Exception ex)
                        {
                            _loggingWork.Error(string.Format("EmailService => SendMail => EmailSending : {0} ", ex.Message));
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                _loggingWork.Error(string.Format("EmailService => SendMail => SMTP : {0} ", ex.Message));
            }
        }

    }
}

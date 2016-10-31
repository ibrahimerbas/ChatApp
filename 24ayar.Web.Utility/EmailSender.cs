using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace _24ayar.Web.Utility
{
    public class EmailSender
    {
        public bool SendMail(string FromEmail, string toEmail, string ToName, string FromName, string Subject, string MailBody)
        {
            return SendMail(FromEmail, toEmail, ToName, FromName, Subject, MailBody, null);
        }
        public bool SendMail(string FromEmail, string ToEmail, string ToName, string FromName, string Subject, string MailBody, string ReplyTo)
        {
            System.Net.Mail.MailAddressCollection mToMails = new MailAddressCollection();
            mToMails.Add(new MailAddress(ToEmail, ToName));
            return SendMail(FromEmail, mToMails, FromName, Subject, MailBody, ReplyTo, null);
        }
        public bool SendMail(string FromEmail, System.Net.Mail.MailAddressCollection toEmails, string adSoyad, string Subject, string MailBody, string ReplyTo)
        {
            return SendMail(FromEmail, toEmails, adSoyad, Subject, MailBody, ReplyTo, null);
        }
        public bool SendMail(string FromEmail, System.Net.Mail.MailAddressCollection ToEmails, string FromName, string Subject, AlternateView aView, string ReplyTo, Collection<Attachment> attachments)
        {
            System.Net.Mail.SmtpClient mSmtp = SmtpClient == null ? GetSmtpClient("smtp.yandex.com.tr", 587, "info@24ayar.net", "24ayarnurgul") : SmtpClient;
            System.Net.Mail.MailMessage mMessage = new System.Net.Mail.MailMessage();
            mMessage.HeadersEncoding = Encoding.Unicode;
            //mSmtp.UseDefaultCredentials = true;
            //MailBody = MailBody.Replace("#EbultenRemoveText", "");

            mMessage.From = new System.Net.Mail.MailAddress(FromEmail, FromName, Encoding.UTF8);
            foreach (System.Net.Mail.MailAddress item in ToEmails)
            {
                mMessage.To.Add(item);
            }
            if (ReplyTo != null)
            {
                mMessage.ReplyToList.Add(new System.Net.Mail.MailAddress(ReplyTo, FromName));
            }
            mMessage.BodyEncoding = Encoding.UTF8;
            mMessage.AlternateViews.Add(aView);
            mMessage.Subject = Subject;
            mMessage.SubjectEncoding = Encoding.UTF8;
            mMessage.HeadersEncoding = Encoding.UTF8;
            mMessage.IsBodyHtml = true;
            IList mAttachs = mMessage.Attachments;
            if (attachments != null)
            {
                foreach (Attachment item in attachments)
                {
                    item.ContentStream.Position = 0;
                    item.NameEncoding = Encoding.UTF8;
                    mAttachs.Add(item);
                }
            }
            try
            {
                mSmtp.Send(mMessage);
                return true;
            }
            catch (Exception er)
            {
                //DBAdresDataContext dataContext = new DBAdresDataContext();
                //senderror mError = new senderror { email = null, error = getErrorMessages(er) };
                //dataContext.senderrors.InsertOnSubmit(mError);
                //dataContext.SubmitChanges();
                return false;
            }
        }
        public bool SendMail(string FromEmail, System.Net.Mail.MailAddressCollection ToEmails, string FromName, string Subject, string MailBody, string ReplyTo, Collection<Attachment> attachments)
        {
            System.Net.Mail.SmtpClient mSmtp = SmtpClient == null ? GetSmtpClient("smtp.yandex.com.tr", 587, "info@24ayar.net", "24ayarnurgul") : SmtpClient; 
            System.Net.Mail.MailMessage mMessage = new System.Net.Mail.MailMessage();
            mMessage.HeadersEncoding = Encoding.Unicode;
            //mSmtp.UseDefaultCredentials = true;
            //MailBody = MailBody.Replace("#EbultenRemoveText", "");
            
            mMessage.From = new System.Net.Mail.MailAddress(FromEmail, FromName, Encoding.UTF8);
            foreach (System.Net.Mail.MailAddress item in ToEmails)
            {
                mMessage.To.Add(item);
            }
            if (ReplyTo != null)
            {
                mMessage.ReplyToList.Add(new System.Net.Mail.MailAddress(ReplyTo, FromName));
            }
            mMessage.BodyEncoding = Encoding.UTF8;
            mMessage.Body = MailBody;
            mMessage.Subject = Subject;
            mMessage.SubjectEncoding = Encoding.UTF8;
            mMessage.HeadersEncoding = Encoding.UTF8;
            mMessage.IsBodyHtml = true;
            IList mAttachs = mMessage.Attachments;
            if (attachments != null)
            {
                foreach (Attachment item in attachments)
                {
                    item.ContentStream.Position = 0;
                    item.NameEncoding = Encoding.UTF8;
                    mAttachs.Add(item);
                }
            }
            try
            {
                mSmtp.Send(mMessage);
                return true;
            }
            catch (Exception er)
            {
                //DBAdresDataContext dataContext = new DBAdresDataContext();
                //senderror mError = new senderror { email = null, error = getErrorMessages(er) };
                //dataContext.senderrors.InsertOnSubmit(mError);
                //dataContext.SubmitChanges();
                return false;
            }

        }
        public SmtpClient SmtpClient { get; set; }

        private SmtpClient GetSmtpClient(string smtpServer, int port, string userName, string password)
        {
            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = false;
            client.Host = smtpServer;
            client.Port = port;
            //client.TargetName = "SMTPSVC/" + smtpServer;
            //client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            NetworkCredential credential = new NetworkCredential();
            credential.UserName = userName;
            credential.Password = password;
            client.Credentials = credential;
            return client;
        }

  
    }
}

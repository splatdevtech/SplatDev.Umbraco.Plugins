//public class ProcessQueuedEmails : ITaskAction
//{
//    private readonly ApplicationDbContext context = new ApplicationDbContext();

//    public void Perform(ScheduleEventArgs args)
//    {
//        var emails = context.QueuedEmails.OrderByDescending(x => x.PriorityId).ToList();

//        EmailService emailService = new EmailService();
//        MailMessage msg = new MailMessage();
//        foreach (var email in emails)
//        {
//            if (email.DontSendBeforeDateUtc < DateTime.UtcNow)
//                continue;

//            try
//            {

//                switch (email.Subject)
//                {
//                    case Constants.EmailSubject.WELCOME:
//                        var user = context.Users.SingleOrDefault(x => x.Email == email.To);
//                        emailService.SendWelcome(user, email.Url);
//                        break;
//                    case Constants.EmailSubject.RESET_PASSWORD:
//                        emailService.SendResetPassword(email.To);
//                        break;
//                    case Constants.EmailSubject.STUDENT_WELCOME:
//                        var student = context.Students.SingleOrDefault(x => x.Email == email.To);
//                        emailService.SendStudentWelcome(student);
//                        break;
//                    default:
//                        msg = new MailMessage
//                        {
//                            Subject = email.Subject,
//                            Body = email.Body,
//                            IsBodyHtml = true
//                        };

//                        msg.To.Add(new MailAddress(email.To, email.ToName));

//                        if (!string.IsNullOrEmpty(email.ReplyTo))
//                            msg.ReplyToList.Add(new MailAddress(email.ReplyTo, email.ReplyToName));

//                        if (email.Bcc != null)
//                        {
//                            var bccAddresses = email.Bcc.Split(',');
//                            foreach (var address in bccAddresses.Where(bccValue => !string.IsNullOrWhiteSpace(bccValue)))
//                            {
//                                msg.Bcc.Add(address.Trim());
//                            }
//                        }

//                        if (email.CC != null)
//                        {
//                            var ccAddresses = email.CC.Split(',');
//                            foreach (var address in ccAddresses.Where(ccValue => !string.IsNullOrWhiteSpace(ccValue)))
//                            {
//                                msg.CC.Add(address.Trim());
//                            }
//                        }

//                        if (!string.IsNullOrEmpty(email.AttachmentFilePath) &&
//                        FileHelpers.FileExists(email.AttachmentFilePath))
//                        {
//                            var attachment = new Attachment(email.AttachmentFilePath);
//                            attachment.ContentDisposition.CreationDate = FileHelpers.GetCreationTime(email.AttachmentFilePath);
//                            attachment.ContentDisposition.ModificationDate = FileHelpers.GetLastWriteTime(email.AttachmentFilePath);
//                            attachment.ContentDisposition.ReadDate = FileHelpers.GetLastAccessTime(email.AttachmentFilePath);
//                            if (!string.IsNullOrEmpty(email.AttachmentFileName))
//                            {
//                                attachment.Name = email.AttachmentFileName;
//                            }

//                            msg.Attachments.Add(attachment);
//                        }

//                        if (!string.IsNullOrEmpty(msg.Body))
//                        {
//                            emailService.SendAsync(msg).GetAwaiter();
//                        }
//                        break;
//                }

//                context.QueuedEmails.Remove(email);

//            }
//            catch
//            {
//                email.SentTries++;
//            }
//            finally
//            {
//                context.SaveChanges();
//            }
//        }
//    }
//}
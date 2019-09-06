using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Text;
using Newtonsoft.Json;

namespace WTS.Events
{
    /// <summary>
    /// Summary description for EmailEvent
    /// </summary>
    public class EmailEvent : EventBase
    {
        public Dictionary<string, string> ToAddresses { get; set; }
        public Dictionary<string, string> CcAddresses { get; set; }
        public Dictionary<string, string> BccAddresses { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string FromAddress { get; set; }
        public string FromDisplayName { get; set; }
        public bool FormatAsHtml { get; set; }
        public MailPriority Priority { get; set; }
        public Dictionary<string, byte[]> Attachments { get; set; }
        public bool SendCcToSystemEmail { get; set; }

        public const string ATTACHMENTBOUNDARY = "--------------------------------------------------ATTACHMENTBYTEBOUNDARY--------------------------------------------------";

        public EmailEvent() : base()
        {
            EventTypeID = (int)EventTypeEnum.Email;
            ToAddresses = new Dictionary<string, string>();
            CcAddresses = new Dictionary<string, string>();
            BccAddresses = new Dictionary<string, string>();
            Attachments = new Dictionary<string, byte[]>();
        }

        public override string SerializePayload()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            // email formats are EMAIL1:NAME1|EMAIL2:NAME2|EMAIL3:NAME3
            dict.Add("ToAddresses", ToAddresses != null ? string.Join("|", ToAddresses.Select((kv) => EscapeString(kv.Key) + ":" + EscapeString(kv.Value))) : "");
            dict.Add("CcAddresses", CcAddresses != null ? string.Join("|", CcAddresses.Select((kv) => EscapeString(kv.Key) + ":" + EscapeString(kv.Value))) : "");
            dict.Add("BccAddresses", BccAddresses != null ? string.Join("|", BccAddresses.Select((kv) => EscapeString(kv.Key) + ":" + EscapeString(kv.Value))) : "");
            dict.Add("Subject", Subject ?? "");
            dict.Add("Body", Body ?? "");
            dict.Add("FromAddress", FromAddress ?? "");
            dict.Add("FromDisplayName", FromDisplayName ?? "");
            dict.Add("FormatAsHtml", FormatAsHtml.ToString());
            dict.Add("MailPriority", Priority.ToString("d"));

            // attachment format will be FILENAME|FILENAME|FILENAME[ATTACHMENTBOUNDARY]FILE2BYTESASSTRING[ATTACHMENTBOUNDARY]FILE2BYTESASSTRING[ATTACHMENTBOUNDARY]FILE3BYTESASSTRING
            if (Attachments != null && Attachments.Count > 0)
            {
                StringBuilder str = new StringBuilder();

                foreach (string fn in Attachments.Keys)
                {
                    byte[] fd = Attachments[fn];

                    if (str.Length > 0) str.Append("|");
                    str.Append(EscapeString(fn));
                }                

                foreach (string fn in Attachments.Keys)
                {
                    byte[] fd = Attachments[fn];

                    str.Append(ATTACHMENTBOUNDARY);
                    str.Append(Convert.ToBase64String(fd));
                }

                dict.Add("Attachments", str.ToString());
            }
            else
            {
                dict.Add("Attachments", "");
            }

            dict.Add("SendCcToSystemEmail", SendCcToSystemEmail.ToString());

            return JsonConvert.SerializeObject(dict, Newtonsoft.Json.Formatting.None);
        }

        public override void ParsePayload(string payload)
        {
            if (payload == null)
            {
                return;
            }

            Dictionary<string, string> dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(payload);

            foreach (var kv in dict)
            {
                if (string.IsNullOrWhiteSpace(kv.Value)) continue;

                string key = kv.Key;
                string value = kv.Value;

                switch (key) {
                    case "ToAddresses":
                        AddEmailsToList(value, ToAddresses);
                        break;
                    case "CcAddresses":
                        AddEmailsToList(value, CcAddresses);
                        break;
                    case "BccAddresses":
                        AddEmailsToList(value, BccAddresses);
                        break;
                    case "Subject":
                        Subject = value;
                        break;
                    case "Body":
                        Body = value;
                        break;
                    case "FromAddress":
                        FromAddress = value;
                        break;
                    case "FromDisplayName":
                        FromDisplayName = value;
                        break;
                    case "FormatAsHtml":
                        FormatAsHtml = value == "True";
                        break;
                    case "MailPriority":
                        Priority = (MailPriority)Convert.ToInt32(value);
                        break;
                    case "Attachments":
                        string[] attArr = value.Split(new string[] { ATTACHMENTBOUNDARY }, StringSplitOptions.RemoveEmptyEntries);
                        if (attArr != null && attArr.Length > 1)
                        {
                            string[] fnArr = attArr[0].Split('|');

                            for (int i = 0; i < fnArr.Length; i++)
                            {
                                string fn = UnescapeString(fnArr[i]);
                                string byteStr = attArr[i + 1];

                                if (!string.IsNullOrWhiteSpace(byteStr))
                                {
                                    Attachments.Add(fn, Convert.FromBase64String(byteStr));
                                }
                            }
                        }
                        break;
                    case "SendCcToSystemEmail":
                        SendCcToSystemEmail = value == "True";
                        break;
                }
            }
        }

        private string EscapeString(string s)
        {
            return s != null ? s.Replace("|", "[pipe]").Replace(":", "[colon]") : null;
        }

        private string UnescapeString(string s)
        {
            return s != null ? s.Replace("[pipe]", "|").Replace("[colon]", ":") : null;
        }

        private void AddEmailsToList(string str, Dictionary<string, string> dict)
        {
            string[] emailList = str.Split('|');

            foreach (string email in emailList)
            {
                if (email.IndexOf(":") > 0)
                {
                    dict.Add(UnescapeString(email.Split(':')[0]), UnescapeString(email.Split(':')[1]));
                }
            }
        }
        
        public override int Execute()
        {
            List<System.Net.Mail.Attachment> mailAttachments = null;

            if (Attachments != null && Attachments.Count > 0)
            {
                mailAttachments = new List<Attachment>();

                foreach (var att in Attachments)
                {
                    mailAttachments.Add(new Attachment(new System.IO.MemoryStream(att.Value), att.Key));
                }
            }
            
            if (WTSUtility.Send_Email(ToAddresses, CcAddresses, BccAddresses, Subject, Body, FromAddress, FromDisplayName, FormatAsHtml, Priority, mailAttachments, SendCcToSystemEmail))
            {
                return (int)EventStatusEnum.Complete;
            }
            else
            {
                // TODO: RESCHEDULE THE EMAIL IF FALSE IS RETURNED AND EMAIL LOOKS LEGIT (HAS NO CONFIG ISSUES LIKE MISSING DESTINATION)
                if (
                    ((ToAddresses != null && ToAddresses.Count > 0) || (CcAddresses != null && CcAddresses.Count > 0) || (BccAddresses != null && BccAddresses.Count > 0))
                    && (!string.IsNullOrWhiteSpace(Subject) || !string.IsNullOrWhiteSpace(Body) || (mailAttachments != null && mailAttachments.Count > 0))
                    && !string.IsNullOrWhiteSpace(FromAddress)
                )
                {
                    EventQueue.Instance.RescheduleEvent(this, DateTime.Now.AddMinutes(60));
                }

                return (int)EventStatusEnum.Error;
            }
        }
    }
}
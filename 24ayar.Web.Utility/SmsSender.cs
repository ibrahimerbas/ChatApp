using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _24ayar.Web.Utility
{
    public abstract class SmsSenderBase:ISmsSender
    {
        protected string userName = null;
        protected string password = null;
        protected string smsOperator = null;
        protected string smsSenderName = null;
        public SmsSenderBase()
        {
            userName = ConfigurationManager.AppSettings["SmsUserName"];
            password = ConfigurationManager.AppSettings["SmsPassword"];
            smsOperator = ConfigurationManager.AppSettings["SmsOperator"];
            smsSenderName = ConfigurationManager.AppSettings["SmsSenderName"];
            if (String.IsNullOrWhiteSpace(userName) || String.IsNullOrWhiteSpace(password) || String.IsNullOrWhiteSpace(smsOperator))
            {
                throw new Exception("SmsUserName veya SmsPassword veya SmsOperator");
            }
        }
        public abstract object SendSMS(string gsmNumber, string Message);
        public abstract object SendSMS(string[] gsmNumbers, string Message);
        public abstract object SendSMS(string[] gsmNumbers, string[] Message);
    }

    public class SmsSender : SmsSenderBase
    {
        public override object SendSMS(string gsmNumber, string Message)
        {
            string[] array = new string[] { gsmNumber };
            return SendSMS(array, Message);
            
        }

        public override object SendSMS(string[] gsmNumbers, string Message)
        {
            string[] array = new string[] { Message };
            return SendSMS(gsmNumbers, array);
        }

        public override object SendSMS(string[] gsmNumbers, string[] Message)
        {
            object result = null;
            if (smsOperator.ToLower() == "postaguvercini")
            {
                using (PostaGuverciniSmsService.smsserviceSoapClient mClient = new PostaGuverciniSmsService.smsserviceSoapClient())
                {
                    result = mClient.SmsInsert_N_N(userName, password, null, null, gsmNumbers, Message);
                 
                }
            }
            else if (smsOperator.ToLower() == "netgsm")
            {
                using (NetGSMSmsService.smsnnClient client = new NetGSMSmsService.smsnnClient())
                {
                    
                    result =  client.sms_gonder_nn(userName, password, "", smsSenderName, Message, gsmNumbers, "EN", "", "");
                }
            }
            return result;
        }
        public object GetReport(string bulkid)
        {
            object result = null;
            using (NetGSMSmsService.smsnnClient client = new NetGSMSmsService.smsnnClient())
            {

                result =  client.rapor_v2(userName, password, bulkid, 100, 1, null, 1);
            }
            return result;
        }
        public string KalanKontur()
        {

            string result = null;
            if (smsOperator.ToLower() == "postaguvercini")
            {
                using (PostaGuverciniSmsService.smsserviceSoapClient mClient = new PostaGuverciniSmsService.smsserviceSoapClient())
                {
                    result = mClient.CreditBalance(userName, password).ToString();

                }
            }
            else if (smsOperator.ToLower() == "netgsm")
            {
                using (NetGSMSmsService.smsnnClient client = new NetGSMSmsService.smsnnClient())
                {

                    result = client.kredi(userName, password);
                }
            }
            return result;
            

        }

        public string SenderID()
        {
            string result = null;
            if (smsOperator.ToLower() == "postaguvercini")
            {
                using (PostaGuverciniSmsService.smsserviceSoapClient mClient = new PostaGuverciniSmsService.smsserviceSoapClient())
                {
                    result = mClient.IdCheck(userName, password).ToString();

                }
            }
            else if (smsOperator.ToLower() == "netgsm")
            {
                using (NetGSMSmsService.smsnnClient client = new NetGSMSmsService.smsnnClient())
                {

                    result = client.gondericiadlari(userName, password);
                }
            }
            return result;
        }
    }


    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class mainbody
    {

        private mainbodyTelno[] telnoField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("telno")]
        public mainbodyTelno[] telno
        {
            get
            {
                return this.telnoField;
            }
            set
            {
                this.telnoField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class mainbodyTelno
    {

        private byte durumField;

        private byte operatorField;

        private byte msg_boyuField;

        private string iletim_tarihiField;

        private byte failreasonField;

        private ulong valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte durum
        {
            get
            {
                return this.durumField;
            }
            set
            {
                this.durumField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte @operator
        {
            get
            {
                return this.operatorField;
            }
            set
            {
                this.operatorField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte msg_boyu
        {
            get
            {
                return this.msg_boyuField;
            }
            set
            {
                this.msg_boyuField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string iletim_tarihi
        {
            get
            {
                return this.iletim_tarihiField;
            }
            set
            {
                this.iletim_tarihiField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte failreason
        {
            get
            {
                return this.failreasonField;
            }
            set
            {
                this.failreasonField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public ulong Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }
}

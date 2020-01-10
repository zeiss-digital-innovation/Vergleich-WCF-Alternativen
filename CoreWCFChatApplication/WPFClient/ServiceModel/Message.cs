using System;
using System.Runtime.Serialization;

// Namespace in Server und Client muss gleich sein, sonst klappt das Deserialisieren bei TCP nicht (binary!!)
namespace CoreWCFChatApplication.Shared.Contract
{
    [DataContract]
    public class Message
    {
        string user = "Anonymous";
        string text = "";
        DateTime time = DateTime.MinValue;

        public Message() { }

        public Message(string user, string text)
        {
            this.user = user;
            this.text = text;
        }

        public Message(string user, string text, DateTime time)
        {
            this.user = user;
            this.text = text;
            this.time = time;
        }

        [DataMember]
        public string User
        {
            get { return user; }
            set { user = value; }
        }

        [DataMember]
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        [DataMember]
        public DateTime Time
        {
            get { return time; }
            set { time = value; }
        }
    }
}
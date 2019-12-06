using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WCFChat
{
    // HINWEIS: Mit dem Befehl "Umbenennen" im Menü "Umgestalten" können Sie den Schnittstellennamen "IChat" sowohl im Code als auch in der Konfigurationsdatei ändern.
    public interface IChatClient
    {
        void ReceiveChat(ChatInfos chatInfos);
        void RefreshUserList(List<string> userList);
    }
    
    public interface IChat
    {
        void SendChat(string name, string text);

        void Join(string name);

        void Refresh();

        void LogOut();
        List<ChatInfos> GetChats();
    }


    // Verwenden Sie einen Datenvertrag, wie im folgenden Beispiel dargestellt, um Dienstvorgängen zusammengesetzte Typen hinzuzufügen.
    [DataContract]
    public class ChatInfos
    {
        string user = "Anonymous";
        string text = "";
        DateTime time = DateTime.MinValue;

        public ChatInfos() { }

        public ChatInfos(string name, string text)
        {
            this.user = name;
            this.text = text;
        }

        public ChatInfos(string name, string text, DateTime time)
        {
            this.user = name;
            this.text = text;
            this.time = time;
        }

        [DataMember]
        public string Name
        {
            get { return user; }
            set { user = value; }
        }

        [DataMember]
        public string Chat
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

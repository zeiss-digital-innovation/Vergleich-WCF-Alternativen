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
        [OperationContract(IsOneWay = true)]
        void ReceiveChat(ChatInfos chatInfos);
        [OperationContract(IsOneWay = true)]
        void RefreshUserList(List<string> userList);
    }
    
    [ServiceContract(CallbackContract = typeof(IChatClient), SessionMode = SessionMode.Required)]
    public interface IChat
    {
        [OperationContract(IsOneWay = true)]
        void SendChat(string name, string text);

        [OperationContract(IsOneWay = true, IsInitiating = true)]
        void Join(string name);

        [OperationContract(IsOneWay = true)]
        void Refresh();

        [OperationContract(IsOneWay = true)]
        void LogOut();
        [OperationContract]
        List<ChatInfos> GetChats();
    }


    // Verwenden Sie einen Datenvertrag, wie im folgenden Beispiel dargestellt, um Dienstvorgängen zusammengesetzte Typen hinzuzufügen.
    [DataContract]
    public class ChatInfos
    {
        string name = "Anonymous";
        string chat = "";
        DateTime time = DateTime.MinValue;

        public ChatInfos() { }

        public ChatInfos(string name, string chat)
        {
            this.name = name;
            this.chat = chat;
        }

        public ChatInfos(string name, string chat, DateTime time)
        {
            this.name = name;
            this.chat = chat;
            this.time = time;
        }

        [DataMember]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [DataMember]
        public string Chat
        {
            get { return chat; }
            set { chat = value; }
        }

        [DataMember]
        public DateTime Time
        {
            get { return time; }
            set { time = value; }
        }
    }
}

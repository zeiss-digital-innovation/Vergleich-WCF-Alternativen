using System.Collections.Generic;
using CoreWCF;

// Namespace in Server und Client muss gleich sein, sonst klappt das Deserialsieren bei TCP nicht (binary!!)
namespace CoreWCFChatApplication.Shared.Contract
{
    [ServiceContract(CallbackContract = typeof(IChatCallback), SessionMode = SessionMode.Required)]
    public interface IChatService
    {
        [OperationContract(IsOneWay = true)]
        void SendChat(string name, string text);

        [OperationContract(IsOneWay = true)]
        void Join(string name);

        [OperationContract(IsOneWay = true)]
        void Refresh();

        [OperationContract(IsOneWay = true)]
        void LogOut();

        [OperationContract]
        List<Message> GetChats();
    }
}

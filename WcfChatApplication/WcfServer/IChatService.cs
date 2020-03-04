using System.Collections.Generic;
using System.ServiceModel;

namespace WcfServer
{
    [ServiceContract(CallbackContract = typeof(IChatCallback), SessionMode = SessionMode.Required)]
    public interface IChatService
    {
        [OperationContract(IsOneWay = true)]
        void SendChat(string name, string text);

        [OperationContract(IsOneWay = true, IsInitiating = true)]
        void Join(string name);

        [OperationContract(IsOneWay = true)]
        void Refresh();

        [OperationContract(IsOneWay = true, IsTerminating = true)]
        void LogOut();

        [OperationContract]
        List<Message> GetChats();
    }
}

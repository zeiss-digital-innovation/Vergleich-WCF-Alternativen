using System.Collections.Generic;
using System.ServiceModel;

namespace WcfServer
{
    interface IChatCallback
    {
        [OperationContract(IsOneWay = true)]
        void ReceiveChat(Message message);

        [OperationContract(IsOneWay = true)]
        void RefreshUserList(List<string> userList);
    }
}

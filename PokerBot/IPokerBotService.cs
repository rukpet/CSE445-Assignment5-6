using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace PokerBot
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IPokerBotService" in both code and config file together.
    [ServiceContract]
    public interface IPokerBotService
    {
        [OperationContract]
        void DoWork();
    }
}

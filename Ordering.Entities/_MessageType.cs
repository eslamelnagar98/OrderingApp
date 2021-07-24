using System;

namespace Ordering.Server.Contract
{
    #region oEnvelop
    public enum _MessageType : short
    {
        NullType = 0,
        ClientLogIn = 202,
        UserInfo = 204,
        DataFeed = 205,
    }
    public enum _MessageDestination : short
    {
        NotSet = 0,
        OMSServer = 1,
        OMSDistrubutionManager = 2,
        OMSClient = 3,
    }
    #endregion
}

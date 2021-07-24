using NLog;
using Ordering.Entities;
using Ordering.Server.Contract;
using System;
using System.Net.Sockets;
using System.Threading;


namespace Ordering.Server
{
    internal class ClientSessionHandler
    {
        private Socket o_Socket;
        public NetworkStream o_NetworkStream { get; private set; }
        private Thread o_ThreadClientListener;
        private static readonly Logger m_Logger = LogManager.GetCurrentClassLogger();
        tdsBroadCasting.OrderDetailDataTable _ordertable;

        public bool IsWorking = true;

        public ClientSessionHandler(Socket o_Socket, tdsBroadCasting.OrderDetailDataTable orderTable)
        {
            this._ordertable = orderTable;
            this.o_Socket = o_Socket;
            o_NetworkStream = new NetworkStream(o_Socket);
            o_ThreadClientListener = new Thread(this.ClientListener);
            o_ThreadClientListener.IsBackground = true;
            o_ThreadClientListener.Start();
        }


        public void ClientListener()
        {
            try
            {
                while (IsWorking)
                {
                    while (!o_NetworkStream.DataAvailable && IsWorking)
                        Thread.Sleep(100);
                    if (!IsWorking)
                        break;

                    oEnvelop o_oEnvelop = null;

                    try
                    {
                        o_oEnvelop = (oEnvelop)new oEnvelop().Deserialize(o_NetworkStream);
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }

                    ProcessRequest(o_oEnvelop);

                    m_Logger.Debug("Received message: {0}", o_oEnvelop.MessageType);
                }
            }
            catch (ThreadAbortException taXp)
            {
                m_Logger.WarnException("An error occurred while aborting DistributionManager listener thread", taXp);
            }
            catch (Exception gXp)
            {
                m_Logger.FatalException("An unknown error occurred while aborting DistributionManager listener thread", gXp);
            }
        }

        private void ProcessRequest(oEnvelop request)
        {
            try
            {
                switch (request.MessageType)
                {
                    case _MessageType.NullType:
                        break;
                    case _MessageType.ClientLogIn://DM wants to Authenticate Client 
                        AuthenticateUser(request);
                        RestoreOldMessages();
                        break;
                    case _MessageType.UserInfo:
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void RestoreOldMessages()
        {
            foreach (var item in _ordertable)
            {
                try
                {
                    var o_oEnvelopMessagetoClient = new oEnvelop(_MessageType.DataFeed);
                    o_oEnvelopMessagetoClient.IsClientMessage = true;
                    o_orderData o_OrderData = new o_orderData();
                    o_OrderData.Account = item.Account;
                    o_OrderData.Price = item.Price;
                    o_OrderData.Quantity = item.Quantity;
                    o_oEnvelopMessagetoClient.oMessages.Add(o_OrderData);
                    o_oEnvelopMessagetoClient.Serialize(o_NetworkStream);
                }
                catch (Exception gXp)
                {
                    m_Logger.ErrorException("An error occurred while authenticating client", gXp);
                }
            }
           
        }

        private void AuthenticateUser(oEnvelop o_oEnvelop)
        {
            try
            {
                var o_oEnvelopMessagetoClient = new oEnvelop(_MessageType.UserInfo);
                o_oEnvelopMessagetoClient.IsClientMessage = true;
                o_oEnvelopMessagetoClient.SessionID = o_oEnvelop.SessionID;
                oClientLogIn o_oClientLogIn = (oClientLogIn)o_oEnvelop.oMessages[0];

                oUserInfo o_oUserInfo;
                var userhandler = new UserHandler();
                o_oUserInfo = userhandler.AuthenticateUser(o_oClientLogIn);
                o_oUserInfo.ClientIP = o_oClientLogIn.ClientIP;
                o_oUserInfo.UserSessionID = o_oEnvelop.SessionID;
                o_oUserInfo.DMSessionID = "SessionID: " + o_oClientLogIn.UserName;

                o_oEnvelopMessagetoClient.oMessages.Add(o_oUserInfo);
                o_oEnvelopMessagetoClient.Serialize(o_NetworkStream);
                m_Logger.Info("OMSClient {0} authenticated successfully", o_oUserInfo.UserNameA);

            }
            catch (Exception gXp)
            {
                m_Logger.ErrorException("An error occurred while authenticating client", gXp);
            }
        }
    }
}
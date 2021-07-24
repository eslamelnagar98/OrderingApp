using System;
using System.Collections;
using System.IO;
using System.Text;

namespace Ordering.Server.Contract
{
    #region oEnvelop
    [Serializable()]
    public class oEnvelop
    {
        public _MessageType MessageType;
        public _MessageDestination MessageDestination;

        /// <summary>
        /// Identify this message as client message not DM while handling in OMS Server
        /// </summary>
        public bool IsClientMessage = false;
        /// <summary>
        /// DM to Client SessionID 
        /// This field is unique for every session , and is used in message routing
        /// </summary>
        public string SessionID = string.Empty;

        public ArrayList oMessages;
        public UInt32 Sequence;
        public oEnvelop(_MessageType messageType = _MessageType.NullType)
        {
            oMessages = new ArrayList();
            MessageType = messageType;
        }
        public oEnvelop()
        {
            oMessages = new ArrayList();
            MessageType = _MessageType.NullType;
        }
        #region Custom Serialization

        public void Serialize(System.IO.Stream stream)
        {
            SerializationWriter wr = SerializationWriter.GetWriter(stream);
            wr.Write((short)MessageType);
            wr.Write((short)0);
            wr.Write(IsClientMessage);
            wr.Write(SessionID);
            wr.Write(Sequence);
            wr.WriteInnerMessages(oMessages.ToArray(), this.MessageType);
        }

        /// <summary>
        /// Try De-serialize current stream to message
        /// this check is done to ensure that receive bytes represents a message
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        /// <Author>Mohamed Atef</Author>
        public bool TryDeserialize(Stream stream)
        {
            //Backup current position befog reading stream
            long streamPosition = stream.Position;

            try
            {
                //try De-serialize
                Deserialize(stream);
            }
            //catch (EndOfStreamException)
            catch (Exception)
            {
                stream.Position = streamPosition; //reset stream position to its original position
                return false;
            }
            return true; //succeeded to de-serialize from stream
        }

        /// <summary>
        /// De-serialize stream to message using my custom De-serialize
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        /// <Author>Mohamed Atef</Author>
        public object Deserialize(System.IO.Stream stream)
        {
            SerializationReader rd = SerializationReader.GetReader(stream);
            this.MessageType = (_MessageType)rd.ReadInt16();
            var MessageDestination = rd.ReadInt16();
            this.IsClientMessage = rd.ReadBoolean();
            this.SessionID = rd.ReadString();
            this.Sequence = rd.ReadUInt32();
            this.oMessages = rd.ReadInnerMessages(this.MessageType);
            return this;
        }

        #endregion

        public string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(string.Format("MessageType: {0}", this.MessageType));
            sb.Append(string.Format(" , IsClientMessage: {0}", this.IsClientMessage));
            sb.Append(string.Format(" , SessionID: {0}", this.SessionID));

            if (this.oMessages != null)
                sb.Append(string.Format(" , Messages Count: {0}", this.oMessages.Count));

            return sb.ToString();
        }
    }

    #endregion
}

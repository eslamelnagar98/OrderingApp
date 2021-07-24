using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Ordering.Server.Contract
{
    #region oEnvelop


    /// <summary> SerializationReader.  Extends BinaryReader to add additional data types,
    /// handle null strings and simplify use with ISerializable. </summary>
    public class SerializationReader : BinaryReader
    {

        private SerializationReader(Stream s) : base(s) { }

        /// <summary> Static method to take a SerializationInfo object (an input to an ISerializable constructor)
        /// and produce a SerializationReader from which serialized objects can be read </summary>.
        public static SerializationReader GetReader(SerializationInfo info)
        {
            byte[] byteArray = (byte[])info.GetValue("X", typeof(byte[]));
            MemoryStream ms = new MemoryStream(byteArray);
            return new SerializationReader(ms);
        }

        public static SerializationReader GetReader(Stream s)
        {
            return new SerializationReader(s);
        }

        /// <summary> Reads a string from the buffer.  Overrides the base implementation so it can cope with nulls. </summary>
        public override string ReadString()
        {
            var t = ReadByte();
            if (t == 11) return base.ReadString();
            return null;
        }

        /// <summary> Reads a byte array from the buffer, handling nulls and the array length. </summary>
        public byte[] ReadByteArray()
        {
            int len = ReadInt32();
            if (len > 0) return ReadBytes(len);
            if (len < 0) return null;
            return new byte[0];
        }

        /// <summary> Reads a char array from the buffer, handling nulls and the array length. </summary>
        public char[] ReadCharArray()
        {
            int len = ReadInt32();
            if (len > 0) return ReadChars(len);
            if (len < 0) return null;
            return new char[0];
        }

        /// <summary> Reads a DateTime from the buffer. </summary>
        public DateTime ReadDateTime() { return new DateTime(ReadInt64()); }


        public ArrayList ReadList()
        {
            int count = ReadInt32();
            if (count < 0) return null;
            ArrayList d = new ArrayList();
            for (int i = 0; i < count; i++) d.Add(ReadObject());
            return d;
        }


        /// <summary> Reads an object which was added to the buffer by WriteObject. </summary>
        public object ReadObject()
        {
            var t = ReadByte();
            switch (t)
            {
                case 1: return ReadBoolean();
                case 2: return ReadByte();
                case 3: return ReadUInt16();
                case 4: return ReadUInt32();
                case 5: return ReadUInt64();
                case 6: return ReadSByte();
                case 7: return ReadInt16();
                case 8: return ReadInt32();
                case 9: return ReadInt64();
                case 10: return ReadChar();
                case 11: return base.ReadString();
                case 12: return ReadSingle();
                case 13: return ReadDouble();
                case 14: return ReadDecimal();
                case 15: return ReadDateTime();
                case 16: return ReadByteArray();
                case 17: return ReadCharArray();
                case 18: return new BinaryFormatter().Deserialize(BaseStream);
                default: return null;
            }
        }


        public ArrayList ReadInnerMessages(_MessageType MessageType)
        {
            int count = ReadInt32();
            if (count < 0) return null;
            ArrayList d = new ArrayList();
            for (int i = 0; i < count; i++)
                d.Add(ReadInnerMessage(MessageType));
            return d;
        }

        public object ReadInnerMessage(_MessageType MessageType)
        {
            switch (MessageType)
            {
                case _MessageType.ClientLogIn:
                    return (new oClientLogIn().Deserialize(BaseStream));
                case _MessageType.UserInfo:
                    return (new oUserInfo().Deserialize(BaseStream));
                case _MessageType.DataFeed:
                    return (new o_orderData().Deserialize(BaseStream));
             
                default:
                    return new BinaryFormatter().Deserialize(BaseStream);
            }
        }
    } // SerializationReader

    #endregion
}

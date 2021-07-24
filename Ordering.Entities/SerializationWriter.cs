using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Ordering.Server.Contract
{
    #region oEnvelop

    /// <summary> SerializationWriter.  Extends BinaryWriter to add additional data types,
    /// handle null strings and simplify use with ISerializable. 
    /// </summary>
    public class SerializationWriter : BinaryWriter
    {
        private SerializationWriter(Stream s) : base(s) { }

        /// <summary> 
        /// Static method to initialise the writer with a suitable MemoryStream. 
        /// </summary>
        public static SerializationWriter GetWriter()
        {
            MemoryStream ms = new MemoryStream(1024);
            return new SerializationWriter(ms);
        }

        public static SerializationWriter GetWriter(Stream s)
        {
            return new SerializationWriter(s);
        }

        /// <summary> Writes a string to the buffer.  Overrides the base implementation so it can cope with nulls </summary>

        public override void Write(string str)
        {
            if (str == null)
            {
                Write((byte)0);
            }
            else
            {
                Write((byte)11);
                base.Write(str);
            }
        }

        /// <summary> Writes a byte array to the buffer.  Overrides the base implementation to
        /// send the length of the array which is needed when it is retrieved </summary>
        public override void Write(byte[] b)
        {
            if (b == null)
            {
                Write(-1);
            }
            else
            {
                int len = b.Length;
                Write(len);
                if (len > 0) base.Write(b);
            }
        }

        /// <summary> Writes a char array to the buffer.  Overrides the base implementation to
        /// sends the length of the array which is needed when it is read. </summary>
        public override void Write(char[] c)
        {
            if (c == null)
            {
                Write(-1);
            }
            else
            {
                int len = c.Length;
                Write(len);
                if (len > 0) base.Write(c);
            }
        }

        /// <summary> Writes a DateTime to the buffer. <summary>
        public void Write(DateTime dt) { Write(dt.Ticks); }

        /// <summary> Writes a generic ICollection (such as an IList<T>) to the buffer. </summary>
        public void Write<T>(ICollection<T> c)
        {
            if (c == null)
            {
                Write(-1);
            }
            else
            {
                Write(c.Count);
                foreach (T item in c) WriteObject(item);
            }
        }

        public void Write(ArrayList c)
        {
            if (c == null)
            {
                Write(-1);
            }
            else
            {
                Write(c.Count);
                foreach (object item in c) WriteObject(item);
            }
        }

        public void Write(BitArray c)
        {
            if (c == null)
            {
                Write(-1);
            }
            else
            {
                Write(c.Count);
                foreach (bool item in c) this.Write(item);
            }
        }

        public void Write(Object[] objs)
        {
            if (objs == null)
            {
                Write(-1);
            }
            else
            {
                Write(objs.Length);
                foreach (object item in objs) WriteObject(item);
            }
        }

        public void WriteByteArray(byte[] array)
        {
            if (array == null)
            {
                Write(-1);
            }
            else
            {
                int len = array.Length;
                Write(len);
                if (len > 0) base.Write(array);
            }
        }

        /// <summary> Writes a generic IDictionary to the buffer. </summary>
        public void Write<T, U>(Dictionary<T, U> d)
        {
            if (d == null)
            {
                Write(-1);
            }
            else
            {
                Write(d.Count);
                foreach (KeyValuePair<T, U> kvp in d)
                {
                    WriteObject(kvp.Key);
                    WriteObject(kvp.Value);
                }
            }
        }

        /// <summary> Writes an arbitrary object to the buffer.  Useful where we have something of type "object"
        /// and don't know how to treat it.  This works out the best method to use to write to the buffer. </summary>
        public void WriteObject(object obj)
        {
            if (obj == null)
            {
                Write((byte)0);
            }
            else
            {
                new BinaryFormatter().Serialize(BaseStream, obj);

            } // if obj==null

        }  // WriteObject

        public void WriteInnerMessages(object[] objs, _MessageType MessageType)
        {
            if (objs == null)
            {
                Write(-1);
            }
            else
            {
                Write(objs.Length);
                foreach (object item in objs)
                    WriteInnerMessage(item, MessageType);
            }
        }

        public void WriteInnerMessage(object obj, _MessageType MessageType)
        {
            if (obj == null)
            {
                Write((byte)_MessageType.NullType);
            }
            else
            {
                switch (MessageType)
                {
                    case _MessageType.ClientLogIn:
                        ((oClientLogIn)obj).Serialize(BaseStream);
                        break;
                    case _MessageType.UserInfo:
                        ((oUserInfo)obj).Serialize(BaseStream);
                        break;
                    case _MessageType.DataFeed:
                        ((o_orderData)obj).Serialize(BaseStream);
                        break;

                    default:
                        new BinaryFormatter().Serialize(BaseStream, obj);
                        break;
                }
            }
        }


    } // SerializationWriter

    #endregion
}

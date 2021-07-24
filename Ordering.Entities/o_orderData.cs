using Ordering.Server;
using Ordering.Server.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Server
{
    public class o_orderData
    {
        public int Quantity { get; set; }

        public string Account { get; set; }
        public Decimal Price { get; set; }

        #region Custom Serialization

        public void Serialize(System.IO.Stream stream)
        {
            SerializationWriter wr = SerializationWriter.GetWriter(stream);
            wr.Write(this.Quantity);
            wr.Write(this.Account);
            wr.Write(this.Price);
        }

        public object Deserialize(System.IO.Stream stream)
        {
            SerializationReader rd = SerializationReader.GetReader(stream);
            this.Quantity = rd.ReadInt32();
            this.Account = rd.ReadString();
            this.Price = rd.ReadDecimal();

            return this;
        }
        #endregion

        public override string ToString()
        {
            return $"Quantity: {Quantity}, Price:{Price}, Account:{Account}";
        }
    }

}

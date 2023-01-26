using MertexerV2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2SQL.Exceptions
{
    public class AddressInsertException : Exception
    {
        private DeliveryAddress DeliveryAddress;

        public AddressInsertException(DeliveryAddress objAddress) : base()
        {
            DeliveryAddress = objAddress;
        }

        public AddressInsertException(Exception ex, DeliveryAddress objAddress) : base(ex.ToString())
        {
            DeliveryAddress = objAddress;
        }

        public override string ToString()
        {
            return $"Failed DeliveryAddress INSERT statement." +
                $"" +
                $"{DeliveryAddress}" +
                $"" +
                $"{base.ToString()}";
        }
    }
}

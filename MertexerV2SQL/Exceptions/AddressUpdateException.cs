using MertexerV2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2SQL.Exceptions
{
    public class AddressUpdateException : Exception
    {
        public DeliveryAddress DeliveryAddress { get; set; }

        public AddressUpdateException(DeliveryAddress address) : base()
        {
            DeliveryAddress = address;
        }

        public AddressUpdateException(Exception ex, DeliveryAddress address) : base (ex.ToString())
        {
            DeliveryAddress = address;
        }
    }
}

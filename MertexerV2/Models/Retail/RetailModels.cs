using MertexerV2.AccountFunctions;
using MertexerV2.Utilities;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models
{

    public class MertexRetailTill
    {
        public string TillNumber { get; set; }
        public string Name { get; set; }

        public override bool Equals(object obj)
        {
            return ((MertexRetailTill)obj).TillNumber == TillNumber;
        }

        public override int GetHashCode()
        {
            return TillNumber.GetHashCode();
        }
    }

    public class MertexRetailTillSession
    {
        public string SessionNumber { get; set; }
        public DateTime OpenedOn { get; set; }
        public TimeSpan OpenedAt { get; set; }
        public string OpenedBy { get; set; }
        public string Label
        {
            get
            {
                return $"Session {SessionNumber} Opened: {OpenedOn.ToString("dd/MM/yyyy HH:mm")} by: {OpenedBy}";
            }
        }
    }

    public class PassRetailTillNumber
    {
        public string retailTillNumber { get; set; }
    }
}
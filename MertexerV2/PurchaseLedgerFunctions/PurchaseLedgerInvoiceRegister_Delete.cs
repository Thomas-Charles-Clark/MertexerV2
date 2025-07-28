using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IBMU2.UODOTNET;

using MertexerV2.GeneralFunctions;
using MertexerV2.Models.Pricing;
using MertexerV2.Models.PurchaseLedger;
using MertexerV2.PricingFunctions;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;

using Newtonsoft.Json;

namespace MertexerV2.PurchaseLedgerFunctions
{
    public static partial class PurchaseLedgerInvoiceRegisterFunctions
    {
        public static bool Delete(
            ref UniSession session,
            string invoiceRegisterCode)
        {
            if (!String.IsNullOrEmpty(invoiceRegisterCode))
            {
                var f = session.CreateUniFile("PLINVREG");

                f.DeleteRecord(invoiceRegisterCode);
            }
            else
            {
                return true;
            }

            return false;
        }
    }
}

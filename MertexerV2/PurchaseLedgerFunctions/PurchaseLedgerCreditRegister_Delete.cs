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
    public static partial class PurchaseLedgerCreditNoteRegisterFunctions
    {
        internal static bool DeleteViaTransaction(
            ref UniSession session,
            string invoiceRegisterJSON)
        {
            var creditNoteRegisterEntry = JsonConvert.DeserializeObject<PurchaseLedgerCreditNoteRegister>(invoiceRegisterJSON);

            if (!String.IsNullOrEmpty(creditNoteRegisterEntry.Key))
            {
                var f = session.CreateUniFile("PLCRNREG");

                f.DeleteRecord(creditNoteRegisterEntry.Key);
            }
            else
            {
                return true;
            }

            return false;
        }
    }
}

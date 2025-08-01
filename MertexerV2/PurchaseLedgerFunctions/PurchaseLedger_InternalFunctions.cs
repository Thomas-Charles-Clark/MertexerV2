using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IBMU2.UODOTNET;

namespace MertexerV2.PurchaseLedgerFunctions
{
    public static partial class PurchaseLedgerFunctions
    {
        /// <summary>
        /// Mimics the PLPROGS program named PLAMOUNT
        /// </summary>
        /// <param name="session">
        ///     Just in case we want to call from within transaction, we can pass a session by reference...
        /// </param>
        /// <param name="type">
        ///     String; null: standard, N: next, Y: year
        /// </param>
        /// <param name="incomingValue">
        ///     The value we're consuming into the ledger
        /// </param>
        /// <param name="sumMultiValues">
        ///     Should we be summing multivalues from the record at hand...
        /// </param>
        /// <param name="countMultiValues">
        ///     Should we just be counting multivalue entries
        /// </param>
        /// <returns></returns>
        internal static decimal CalculatePurchaseLedgerAmount(ref UniSession session, string period, decimal incomingValue, bool sumMultiValues = false, bool countMultiValues = false)
        {
            var type = sumMultiValues ? "A" : countMultiValues ? "X" : (string)null;

            var multi = sumMultiValues || countMultiValues;

            var result = 0.0m;

            switch (period)
            {
                case "N":

                    break;
                case "Y":

                    break;
                default:

                    break;
            }

            return result;
        }
    }
}

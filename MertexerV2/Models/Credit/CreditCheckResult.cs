using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.Credit
{
    public class CreditCheckResult
    {
        public string CreditStop { get; set; }
        public string CreditStatus { get; set; }
        public decimal? CreditLimit { get; set; }
        public string RetailTillCust { get; set; }
        public decimal? Balance { get; set; }
        public decimal? BalanceCurrency { get; set; }
        public decimal? OutstandingBalance { get; set; }
        public decimal? OutstandingBalanceCurrency { get; set; }
        public decimal? OutstandingReserves { get; set; }
        public decimal? OutstandingReservesCurrency { get; set; }

        public CreditCheckResult(
            string creditStop,
            string creditStatus,
            string retailTillCust,
            string creditLimit)
        {
            CreditStop = creditStop;

            CreditStatus = creditStatus;

            RetailTillCust = retailTillCust;

            if (Decimal.TryParse(creditLimit, out decimal cred))
            {
                CreditLimit = cred;
            }
        }

        public CreditCheckResult (
            string balance,
            string balanceCurr,
            string outstandingBalance,
            string outstandingBalanceCurrency,
            string outstandingReserves,
            string outstandingReservesCurrency)
        {
            if (Decimal.TryParse(balance, out decimal bal))
            {
                Balance = bal;
            }

            if (Decimal.TryParse(balanceCurr, out decimal balCur))
            {
                BalanceCurrency = balCur;
            }

            if (Decimal.TryParse(outstandingBalance, out decimal outBal)) 
            {
                OutstandingBalance = outBal;
            }

            if (Decimal.TryParse(outstandingBalanceCurrency, out decimal outBalCur))
            {
                OutstandingBalanceCurrency = outBalCur;
            }

            if (Decimal.TryParse(outstandingReserves, out decimal outRes))
            {
                OutstandingReserves = outRes;
            }

            if (Decimal.TryParse(outstandingReservesCurrency, out decimal outResCur))
            {
                OutstandingReservesCurrency = outResCur;
            }
        }
    }
}

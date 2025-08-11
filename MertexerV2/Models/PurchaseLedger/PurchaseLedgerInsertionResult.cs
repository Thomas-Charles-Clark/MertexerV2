using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.PurchaseLedger
{
    public class PurchaseLedgerInsertionResult
    {
        public string PurchaseLedgerInvoiceRegistryKey { get; set; }

        public string FileKey { get; set; }

        public string FileName { get; set; }

        public IEnumerable<string> OperationsPerformed { get; set; }

        public string JSONBefore { get; set; }

        public string JSONAfterwards { get; set; }
    
        public PurchaseLedgerInsertionResult(
            string purchaseLedgerInvoiceRegistryKey,
            string fileKey, 
            string fileName, 
            IEnumerable<string> operationsPerformed, 
            string jsonBefore, 
            string jsonAfterwards)
        {
            PurchaseLedgerInvoiceRegistryKey = purchaseLedgerInvoiceRegistryKey;
            FileKey = fileKey;
            FileName = fileName;
            OperationsPerformed = operationsPerformed;
            JSONBefore = jsonBefore;
            JSONAfterwards = jsonAfterwards;
        }
    }
}

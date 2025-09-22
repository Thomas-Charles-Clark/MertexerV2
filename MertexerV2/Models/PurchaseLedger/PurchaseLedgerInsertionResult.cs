using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.PurchaseLedger
{
    public class PurchaseLedgerInsertionResult
    {
        public string PurchaseLedgerRegistryKey { get; set; }

        public string FileKey { get; set; }

        public string FileName { get; set; }

        public IEnumerable<string> OperationsPerformed { get; set; }

        public string JSONBefore { get; set; }

        public string JSONAfterwards { get; set; }
    
        public PurchaseLedgerInsertionResult(
            string purchaseLedgerRegistryKey,
            string fileKey, 
            string fileName, 
            IEnumerable<string> operationsPerformed, 
            string jsonBefore, 
            string jsonAfterwards)
        {
            PurchaseLedgerRegistryKey = purchaseLedgerRegistryKey;
            FileKey = fileKey;
            FileName = fileName;
            OperationsPerformed = operationsPerformed;
            JSONBefore = jsonBefore;
            JSONAfterwards = jsonAfterwards;
        }
    }
}

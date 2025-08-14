using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.PurchaseLedger
{
    public class PurchaseLedgerTransactionRollbackResult
    {
        public string PurchaseLedgerInvoiceRegistryKey { get; set; }
        public string FileName { get; set; }
        public string FileKey { get; set; }
        public IEnumerable<string> OperationsPerformed { get; set; }
        public bool Success { get; set; }
        public string UniCommandResponse { get; set; }
        public int? UniCommandStatus { get; set; }

        public PurchaseLedgerTransactionRollbackResult(
            string purchaseLedgerInvoiceRegistryKey,
            string fileKey,
            string fileName, 
            IEnumerable<string> operationsPerformed,
            bool success,
            string uniVerseResponse,
            int? uniVerseCommandStatus)
        {
            PurchaseLedgerInvoiceRegistryKey = purchaseLedgerInvoiceRegistryKey;
            FileName = fileName;
            FileKey = fileKey;
            OperationsPerformed = operationsPerformed;
            Success = success; 
            UniCommandResponse = uniVerseResponse;
            UniCommandStatus = UniCommandStatus;
        }
    }
}

using MertexerV2.GeneralFunctions;
using MertexerV2.Models.EDAReplacement;
using MertexerV2.Models.Product;
using MertexerV2.Models.PurchaseLedger;
using MertexerV2.Models.SalesAnalysis;
using MertexerV2.Models.Supplier;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;

using Newtonsoft.Json;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Supplier = MertexerV2.Models.Supplier.Supplier;

namespace MertexerV2.SupplierFunctions
{
    public class SupplierCode
    {
        public string Code { get; set; }
    }

    public class SupplierDocument
    {
        public string SupplierReference { get; set; }
        public string SupplierName { get; set; }
        public string SupplierCurrency { get; set; }
        public string DocumentReference { get; set; }
        public string MertexSourceFileName { get; set; }

        public SupplierDocument(string supplierReference, string supplierName, string supplierCurrency, string documentReference, string mertexSourceFileName)
        {
            SupplierReference = supplierReference;
            SupplierName = supplierName;
            SupplierCurrency = supplierCurrency;
            DocumentReference = documentReference;
            MertexSourceFileName = mertexSourceFileName;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Include,
                Formatting = Formatting.Indented
            });
        }
    }

    public class SupplierAccountDetail : SupplierDocument
    {
        public DateTime? DocumentDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string DocumentType { get; set; }
        public string Sel1 { get; set; }
        public string Sel1D { get; set; }
        public string Sel2 { get; set; }
        public string Sel2D { get; set; }
        public string Sel3 { get; set; }
        public string Sel3D { get; set; }
        public string ItemReference { get; set; }
        public decimal OriginalValue { get; set; }
        public decimal Balance { get; set; }
        public bool Disputed { get; set; }
        public string Division { get; set; }
        public decimal Unallocated { get; set; }
        public bool Approved { get; set; }
        public string PassedTo { get; set; }

        public SupplierAccountDetail(
            string supRef,
            string supName,
            string supCur,
            string sel1,
            string sel1d,
            string sel2,
            string sel2d,
            string sel3,
            string sel3d,
            string sourceFileName,
            PurchaseLedgerTransaction plt) : base(supRef, supName, supCur, plt.Key, sourceFileName)
        {

            int f = 0;

            Sel1 = sel1;
            Sel1D = sel1d;
            Sel2 = sel2;
            Sel2D = sel2d;
            Sel3 = sel3;
            Sel3D = sel3d;
            DocumentType = plt.TransactionType;
            DocumentDate = plt.DocumentDT;
            DueDate = plt.DueDT;
            ItemReference = plt.ItemReference;
            OriginalValue = plt.OriginalValueABS;
            Balance = plt.Balance;
            Disputed = plt.Dispute == "D";
            Division = plt.Division;
            Unallocated = plt.Unallocated == 0 ? 0 : plt.Unallocated * -1;
            Approved = true;
            PassedTo = null;
        }

        public SupplierAccountDetail(
            string supRef,
            string supName,
            string supCur,
            string sel1,
            string sel1d,
            string sel2,
            string sel2d,
            string sel3,
            string sel3d,
            string sourceFileName,
            PurchaseLedgerInvoiceRegister plt,
            bool sterling) : base(supRef, supName, supCur, plt.Key, sourceFileName)
        {
            Sel1 = sel1;
            Sel1D = sel1d;
            Sel2 = sel2;
            Sel2D = sel2d;
            Sel3 = sel3;
            Sel3D = sel3d;
            DocumentType = "INV";
            DocumentDate = plt.DocumentDT;
            DueDate = plt.DueDT;
            ItemReference = plt.ItemReference;
            OriginalValue = 0.0m;
            Balance = sterling ? plt.AmountSterling : plt.AmountCurrency;
            Disputed = false;
            Division = null;
            Unallocated = 0;
            Approved = false;
            PassedTo = plt.PassedTo;
        }

        public SupplierAccountDetail(
            string supRef,
            string supName,
            string supCur,
            string sel1,
            string sel1d,
            string sel2,
            string sel2d,
            string sel3,
            string sel3d,
            string sourceFileName,
            PurchaseLedgerCreditRegister plt,
            bool sterling) : base(supRef, supName, supCur, plt.Key, sourceFileName)
        {
            Sel1 = sel1;
            Sel1D = sel1d;
            Sel2 = sel2;
            Sel2D = sel2d;
            Sel3 = sel3;
            Sel3D = sel3d;
            DocumentType = "CRN";
            DocumentDate = plt.DocumentDT;
            DueDate = plt.DueDT;
            ItemReference = plt.ItemReference;
            OriginalValue = 0.0m;
            Balance = sterling ? plt.AmountSterling : plt.AmountCurrency;
            Disputed = false;
            Division = null;
            Unallocated = 0;
            Approved = false;
            PassedTo = plt.PassedTo;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Include,
                Formatting = Formatting.Indented
            });
        }
    }

    public static partial class SupplierFunctions    
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "SupplierSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string SupplierCodes)
        {
            if (String.IsNullOrEmpty(SupplierCodes))
            {
                foreach (var q in Selector<Supplier>.SelectAllNonParallel(
                   true,
                   false,
                   50,
                   UniVerseAccount))
                {
                    yield return q;
                }
            }
            else
            {
                var results = Selector<Supplier>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<SupplierCode>>(SupplierCodes).Select(p => p.Code),
                    true,
                    false,
                    50,
                    UniVerseAccount)
                    .ToList();

                foreach (var r in results)
                {
                    yield return r;
                }
            }
        }

        public static void SupplierSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Name,
            out SqlString AddressLine1,
            out SqlString AddressLine2,
            out SqlString Town,
            out SqlString County,
            out SqlString Postcode,
            out SqlString SortName,
            out SqlString CountryCode,
            out SqlString Currency,
            out SqlDecimal Balance,
            out SqlDecimal ThisPeriodBalance,
            out SqlDecimal NextPeriodBalance,
            out SqlString PhoneNumber,
            out SqlString EmailAddress,
            out SqlDecimal DutyPercentage,
            out SqlDecimal InlandChargesPercentage,
            out SqlDecimal MiscellaneousPercentage,
            out SqlDecimal ImportChargePerUnit,
            out SqlDecimal MiscellaneousCharge,
            out SqlString BudgetHolderCode,
            out SqlString PaymentTermsCode,
            out SqlString SupplierTypeCode,
            out SqlString VATCode,
            out SqlBoolean AccountClosed
            )
        {
            var p = (Supplier)obj;

            // ADD ADDRESS DETAILS AND REMOVE EXTANT EDAREPLACEMENT VERSION

            Code = p.Key?.TrimReduceTruncate(50);
            Name = p.Name?.TrimReduceTruncate(100);
            AddressLine1 = p.AddressLine1?.TrimReduceTruncate(60);
            AddressLine2 = p.AddressLine2?.TrimReduceTruncate(60);
            Town = p.Town?.TrimReduceTruncate(50);
            County = p.County?.TrimReduceTruncate(40);
            Postcode = p.Postcode?.TrimReduceTruncate(20);
            SortName = p.SortName?.TrimReduceTruncate(100);
            CountryCode = p.CountryCode?.TrimReduceTruncate(3);
            Currency = p.Currency?.TrimReduceTruncate(10);
            Balance = p.Balance;
            ThisPeriodBalance = p.ThisPeriodBalance;
            NextPeriodBalance = p.NextPeriodBalance;
            DutyPercentage = p.DutyPercentage ?? SqlDecimal.Null;
            InlandChargesPercentage = p.InlandChargesPercentage ?? SqlDecimal.Null;
            MiscellaneousPercentage = p.MiscellaneousPercentage ?? SqlDecimal.Null;
            ImportChargePerUnit = p.ImportChargePerUnit ?? SqlDecimal.Null;
            MiscellaneousCharge = p.MiscellaneousCharge ?? SqlDecimal.Null;
            PhoneNumber = p.PhoneNumber?.TrimReduceTruncate(20);
            EmailAddress = p.Email?.TrimReduceTruncate(254);
            BudgetHolderCode = p.BudgetHolderCode?.TrimReduceTruncate(50);
            PaymentTermsCode = p.PaymentTermsCode?.TrimReduceTruncate(10);
            SupplierTypeCode = p.SEL1?.TrimReduceTruncate(10);
            VATCode = p.VATCode?.TrimReduceTruncate(10);
            AccountClosed = p.AccountClosed;
        }
        




        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "AccountDetailsSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable AccountDetailsSelect(string UniVerseAccount, string SupplierCode)
        {
            var invReg = Selector<PurchaseLedgerInvoiceRegister>.SelectAllNonParallel(
                    applyConversions: true,
                    selectChildEntities: false,
                    batchSize: 1000,
                    UniVerseAccount
                ).ToList();

            var crnReg = Selector<PurchaseLedgerCreditRegister>.SelectAllNonParallel(
                    applyConversions: true,
                    selectChildEntities: false,
                    batchSize: 1000,
                    UniVerseAccount
                ).ToList();

            if (String.IsNullOrEmpty(SupplierCode))
            {
                var tpltrans = Selector<PurchaseLedgerTransaction>.SelectAllNonParallel(
                        applyConversions: true,
                        selectChildEntities: false,
                        batchSize: 1000,
                        UniVerseAccount
                    ).ToList();

                var sel1 = Selector<SupplierType>.SelectAllNonParallel(
                        applyConversions: true,
                        selectChildEntities: false,
                        batchSize: 1000,
                        UniVerseAccount
                    ).ToList();

                var sel2 = Selector<PLSEL2>.SelectAllNonParallel(
                        applyConversions: true,
                        selectChildEntities: false,
                        batchSize: 1000,
                        UniVerseAccount
                    ).ToList();

                var sel3 = Selector<PLSEL3>.SelectAllNonParallel(
                        applyConversions: true,
                        selectChildEntities: false,
                        batchSize: 1000,
                        UniVerseAccount
                    ).ToList();

                foreach (var r in Selector<Supplier>.SelectAllNonParallel(
                    true,
                    false,
                    50,
                    UniVerseAccount))
                {
                    r.PurchaseLedgerTransferRecords = tpltrans.Where(p => p.SupplierReference == r.Key).ToList();

                    r.PLSEL1 = sel1.FirstOrDefault(p => p.Key == r.SEL1);

                    r.PLSEL2 = sel2.FirstOrDefault(p => p.Key == r.SEL2);

                    r.PLSEL3 = sel3.FirstOrDefault(p => p.Key == r.BudgetHolderCode);

                    r.PurchaseLedgerRegisterEntries_Invoice = invReg.Where(p => p.SupplierCode == r.Key).ToList();

                    r.PurchaseLedgerRegisterEntries_Credit = crnReg.Where(p => p.SupplierReference == r.Key).ToList();

                    foreach (var dr in r.PurchaseLedgerTransferRecords?
                        .Select(d => new SupplierAccountDetail(r.Key, r.Name, r.Currency, r.SEL1, r.DSEL1, r.SEL2, r.DSEL2, r.BudgetHolderCode, r.DSEL3, "TPLTRANS", d)))
                    {
                        yield return dr;
                    }

                    foreach (var inv in r.PurchaseLedgerRegisterEntries_Invoice?.Select(d => new SupplierAccountDetail(r.Key, r.Name, r.Currency,
                        r.SEL1, r.DSEL1, r.SEL2, r.DSEL2, r.BudgetHolderCode, r.DSEL3, "PLINVREG", d, false
                        )))
                    {
                        yield return inv;
                    }

                    foreach (var crn in r.PurchaseLedgerRegisterEntries_Credit?.Select(d => new SupplierAccountDetail(r.Key, r.Name, r.Currency,
                        r.SEL1, r.DSEL1, r.SEL2, r.DSEL2, r.BudgetHolderCode, r.DSEL3, "PLCRNREG", d, false
                        )))
                    {
                        yield return crn;
                    }
                }
            }
            else
            {
                var result = Selector<Supplier>.Select(
                    SupplierCode,
                    true,
                    true,
                    50,
                    UniVerseAccount);

                result.PurchaseLedgerRegisterEntries_Invoice = invReg.Where(p => p.SupplierCode == result.Key).ToList();

                result.PurchaseLedgerRegisterEntries_Credit = crnReg.Where(p => p.SupplierReference == result.Key).ToList();

                foreach (var dr in result.PurchaseLedgerTransferRecords?.Select(d => new SupplierAccountDetail(result.Key, result.Name, result.Currency, result.SEL1, result.DSEL1, result.SEL2, result.DSEL2, result.BudgetHolderCode, result.DSEL3, "TPLTRANS", d)))
                {
                    yield return dr;
                }

                foreach (var inv in result.PurchaseLedgerRegisterEntries_Invoice?.Select(d => new SupplierAccountDetail(result.Key, result.Name, result.Currency,
                    result.SEL1, result.DSEL1, result.SEL2, result.DSEL2, result.BudgetHolderCode, result.DSEL3, "PLINVREG", d, false
                    )))
                {
                    yield return inv;
                }

                foreach (var crn in result.PurchaseLedgerRegisterEntries_Credit?.Select(d => new SupplierAccountDetail(result.Key, result.Name, result.Currency,
                    result.SEL1, result.DSEL1, result.SEL2, result.DSEL2, result.BudgetHolderCode, result.DSEL3, "PLCRNREG", d, false
                    )))
                {
                    yield return crn;
                }
            }
        }

        public static void AccountDetailsSelect_FillRow(
            object obj,
            out SqlString SupplierReference,
            out SqlString SupplierName,
            out SqlString SupplierCurrency,
            out SqlString RecordCurrency,
            out SqlString DocumentReference,
            out SqlString DocumentType,
            out SqlString Sel1,
            out SqlString Sel1Desc,
            out SqlString Sel2,
            out SqlString Sel2Desc,
            out SqlString Sel3,
            out SqlString Sel3Desc,
            out SqlDateTime DocumentDT,
            out SqlDateTime DueDT,
            out SqlString ItemReference,
            out SqlDecimal OriginalValue,
            out SqlDecimal Balance,
            out SqlBoolean Disputed,
            out SqlString Division,
            out SqlDecimal Unallocated,
            out SqlBoolean Approved,
            out SqlString PassedTo,
            out SqlString MertexSourceFileName)
        {
            var result = (SupplierAccountDetail)obj;

            SupplierReference = result.SupplierReference?.TrimReduceTruncate(50);
            SupplierName = result.SupplierName?.TrimReduceTruncate(200);
            SupplierCurrency = result.SupplierCurrency?.TrimReduceTruncate(5);
            RecordCurrency = "STG";
            DocumentReference = result.DocumentReference?.TrimReduceTruncate(50);
            DocumentType = result.DocumentType?.TrimReduceTruncate(20);
            Sel1 = result.Sel1?.TrimReduceTruncate(5);
            Sel1Desc = result.Sel1D?.TrimReduceTruncate(30);
            Sel2 = result.Sel2?.TrimReduceTruncate(5);
            Sel2Desc = result.Sel2D?.TrimReduceTruncate(30);
            Sel3 = result.Sel3?.TrimReduceTruncate(5);
            Sel3Desc = result.Sel3D?.TrimReduceTruncate(30);
            DocumentDT = result.DocumentDate.HasValue ? result.DocumentDate.Value : SqlDateTime.Null; 
            DueDT = result.DueDate.HasValue ? result.DueDate.Value : SqlDateTime.Null;
            ItemReference = result.ItemReference?.TrimReduceTruncate(50);
            OriginalValue = result.OriginalValue;
            Balance = result.Balance;
            Disputed = result.Disputed;
            Division = result.Division?.TrimReduceTruncate(10);
            Unallocated = result.Unallocated;
            Approved = result.Approved;
            PassedTo = result.PassedTo?.TrimReduceTruncate(50);
            MertexSourceFileName = result.MertexSourceFileName?.TrimReduceTruncate(50);
        }



        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "AccountDetailsSelect_Currency_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable AccountDetails_Currency_Select(string UniVerseAccount, string SupplierCode)
        {
            var invReg = Selector<PurchaseLedgerInvoiceRegister>.SelectAllNonParallel(
                    applyConversions: true,
                    selectChildEntities: false,
                    batchSize: 1000,
                    UniVerseAccount
                ).ToList();

            var crnReg = Selector<PurchaseLedgerCreditRegister>.SelectAllNonParallel(
                    applyConversions: true,
                    selectChildEntities: false,
                    batchSize: 1000,
                    UniVerseAccount
                ).ToList();

            var sel1 = Selector<SupplierType>.SelectAllNonParallel(
                    applyConversions: true,
                    selectChildEntities: false,
                    batchSize: 1000,
                    UniVerseAccount
                ).ToList();

            var sel2 = Selector<PLSEL2>.SelectAllNonParallel(
                    applyConversions: true,
                    selectChildEntities: false,
                    batchSize: 1000,
                    UniVerseAccount
                ).ToList();

            var sel3 = Selector<PLSEL3>.SelectAllNonParallel(
                    applyConversions: true,
                    selectChildEntities: false,
                    batchSize: 1000,
                    UniVerseAccount
                ).ToList();

            if (String.IsNullOrEmpty(SupplierCode))
            {
                var tpltrans = Selector<PurchaseLedgerTransferCurrency>.SelectAllNonParallel(
                        applyConversions: true,
                        selectChildEntities: false,
                        batchSize: 1000,
                        UniVerseAccount
                    ).ToList();

                foreach (var r in Selector<Supplier>.SelectAllNonParallel(
                    true,
                    false,
                    50,
                    UniVerseAccount))
                {
                    r.PurchaseLedgerTransferRecordsCurrency = tpltrans.Where(p => p.SupplierReference == r.Key).ToList();

                    r.PLSEL1 = sel1.FirstOrDefault(p => p.Key == r.SEL1);

                    r.PLSEL2 = sel2.FirstOrDefault(p => p.Key == r.SEL2);

                    r.PLSEL3 = sel3.FirstOrDefault(p => p.Key == r.BudgetHolderCode);

                    r.PurchaseLedgerRegisterEntries_Invoice = invReg.Where(p => p.SupplierCode == r.Key).ToList();

                    r.PurchaseLedgerRegisterEntries_Credit = crnReg.Where(p => p.SupplierReference == r.Key).ToList();

                    foreach (var dr in r.PurchaseLedgerTransferRecordsCurrency?
                        .Select(d => new SupplierAccountDetail(r.Key, r.Name, r.Currency, r.SEL1, r.DSEL1, r.SEL2, r.DSEL2, r.BudgetHolderCode, r.DSEL3, "TPLTRANS.CUR", d)))
                    {
                        yield return dr;
                    }

                    foreach (var inv in r.PurchaseLedgerRegisterEntries_Invoice?.Select(d => new SupplierAccountDetail(r.Key, r.Name, r.Currency,
                        r.SEL1, r.DSEL1, r.SEL2, r.DSEL2, r.BudgetHolderCode, r.DSEL3,"PLINVREG", d, true
                        )))
                    {
                        yield return inv;
                    }

                    foreach (var crn in r.PurchaseLedgerRegisterEntries_Credit?.Select(d => new SupplierAccountDetail(r.Key, r.Name, r.Currency,
                        r.SEL1, r.DSEL1, r.SEL2, r.DSEL2, r.BudgetHolderCode, r.DSEL3, "PLCRNREG", d, true
                        )))
                    {
                        yield return crn;
                    }
                }
            }
            else
            {
                var result = Selector<Supplier>.Select(
                    SupplierCode,
                    true,
                    true,
                    1000,
                    UniVerseAccount);

                result.PurchaseLedgerRegisterEntries_Invoice = invReg.Where(p => p.SupplierCode == result.Key).ToList();

                result.PurchaseLedgerRegisterEntries_Credit = crnReg.Where(p => p.SupplierReference == result.Key).ToList();

                foreach (var dr in result.PurchaseLedgerTransferRecordsCurrency?.Select(d => new SupplierAccountDetail(result.Key, result.Name, result.Currency,
                    result.SEL1, result.DSEL1, result.SEL2, result.DSEL2, result.BudgetHolderCode, result.DSEL3, "TPLTRANS.CUR", d)))
                {
                    yield return dr;
                }

                foreach (var inv in result.PurchaseLedgerRegisterEntries_Invoice?.Select(d => new SupplierAccountDetail(result.Key, result.Name, result.Currency,
                   result.SEL1, result.DSEL1, result.SEL2, result.DSEL2, result.BudgetHolderCode, result.DSEL3, "PLINVREG", d, true
                    )))
                {
                    yield return inv;
                }

                foreach (var crn in result.PurchaseLedgerRegisterEntries_Credit?.Select(d => new SupplierAccountDetail(result.Key, result.Name, result.Currency,
                   result.SEL1, result.DSEL1, result.SEL2, result.DSEL2, result.BudgetHolderCode, result.DSEL3, "PLCRNREG", d, true
                    )))
                {
                    yield return crn;
                }
            }
        }

        public static void AccountDetailsSelect_Currency_FillRow(
            object obj,
            out SqlString SupplierReference,
            out SqlString SupplierName,
            out SqlString SupplierCurrency,
            out SqlString RecordCurrency,
            out SqlString DocumentReference,
            out SqlString DocumentType,
            out SqlString Sel1,
            out SqlString Sel1Desc,
            out SqlString Sel2,
            out SqlString Sel2Desc,
            out SqlString Sel3,
            out SqlString Sel3Desc,
            out SqlDateTime DocumentDT,
            out SqlDateTime DueDT,
            out SqlString ItemReference,
            out SqlDecimal OriginalValue,
            out SqlDecimal Balance,
            out SqlBoolean Disputed,
            out SqlString Division,
            out SqlDecimal Unallocated,
            out SqlBoolean Approved,
            out SqlString PassedTo,
            out SqlString MertexSourceFileName)
        {
            var result = (SupplierAccountDetail)obj;

            SupplierReference = result.SupplierReference?.TrimReduceTruncate(50);
            SupplierName = result.SupplierName?.TrimReduceTruncate(200);
            SupplierCurrency = result.SupplierCurrency?.TrimReduceTruncate(5);
            RecordCurrency = result.SupplierCurrency?.TrimReduceTruncate(5);
            DocumentReference = result.DocumentReference?.TrimReduceTruncate(50);
            DocumentType = result.DocumentType?.TrimReduceTruncate(20);
            Sel1 = result.Sel1?.TrimReduceTruncate(5);
            Sel1Desc = result.Sel1D?.TrimReduceTruncate(50);
            Sel2 = result.Sel2?.TrimReduceTruncate(5);
            Sel2Desc = result.Sel2D?.TrimReduceTruncate(50);
            Sel3 = result.Sel3?.TrimReduceTruncate(5);
            Sel3Desc = result.Sel3D?.TrimReduceTruncate(50);
            DocumentDT = result.DocumentDate.HasValue ? result.DocumentDate.Value : SqlDateTime.Null;
            DueDT = result.DueDate.HasValue ? result.DueDate.Value : SqlDateTime.Null;
            ItemReference = result.ItemReference?.TrimReduceTruncate(50);
            OriginalValue = result.OriginalValue;
            Balance = result.Balance;
            Disputed = result.Disputed;
            Division = result.Division?.TrimReduceTruncate(10);
            Unallocated = result.Unallocated;
            Approved = result.Approved;
            PassedTo = result.PassedTo?.TrimReduceTruncate(50);
            MertexSourceFileName = result.MertexSourceFileName?.TrimReduceTruncate(50);
        }
    }
}

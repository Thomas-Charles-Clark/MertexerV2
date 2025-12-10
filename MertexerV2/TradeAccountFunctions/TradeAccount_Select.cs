using MertexerV2.GeneralFunctions;
using MertexerV2.Models;
using MertexerV2.Models.DataSource;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;
using Newtonsoft.Json;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace MertexerV2.TradeAccountFunctions
{
    public static partial class TradeAccountFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "TradeAccountSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string TradeAccountCodes)
        {
            if (String.IsNullOrEmpty(TradeAccountCodes))
            {
                foreach (var r in Selector<TradeAccount>.SelectAllNonParallel(
                    true,
                    false,
                    50,
                    UniVerseAccount))
                {
                    yield return r;
                }
            }
            else
            {
                var results = Selector<TradeAccount>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<TradeAccountCode>>(TradeAccountCodes).Select(p => p.Code),
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

        public static void TradeAccountSelect_FillRow(
            object obj,
            out SqlString TradeAccountCode,
            out SqlString Status,
            out SqlDateTime CreatedDT,
            out SqlDateTime ModifiedDT,
            out SqlString Name,
            out SqlString ContactName,
            out SqlString SortName,
            out SqlString PhoneNumber,
            out SqlString Email,
            out SqlString MarketingEmail,
            out SqlString Website,
            out SqlString CompanyRegistrationNumber,
            out SqlString CustomersReference,
            out SqlString EORINumber,
            out SqlString TaxID,
            out SqlString Currency,
            out SqlString TradeAccountGroup,
            out SqlString TradeAccountType,
            out SqlString TradeAccountClass,
            out SqlString OrderCategory,
            out SqlString Market,
            out SqlString PriceList,
            out SqlString SalesDivision,
            out SqlString InternalOrExternal,
            out SqlBoolean MiscellaneousAccount,
            out SqlString PriceCode,
            out SqlString ContractOrDisplay,
            out SqlString RetailOrContract,
            out SqlBoolean Advertiser,
            out SqlBoolean Stockist,
            out SqlBoolean NoEmail,
            out SqlString SalesRepresentativeCode,
            out SqlString SalesAgentCode,
            out SqlString SalesArea,
            out SqlString InvoiceAddressLine1,
            out SqlString InvoiceAddressLine2,
            out SqlString InvoiceAddressTown,
            out SqlString InvoiceAddressCounty,
            out SqlString InvoiceAddressPostcode,
            out SqlString InvoiceAddressCountryCode,
            out SqlString InvoiceAccounts,
            out SqlString DefaultDeliveryReference,
            out SqlBoolean SendInvoiceToDeliveryAddress,
            out SqlString PaymentTerms,
            out SqlInt32 CreditLimit,
            out SqlBoolean CreditStopped,
            out SqlString BankCode,
            out SqlString CreditController,
            out SqlDateTime CreditLimitExpiryDT,
            out SqlBoolean HighValueOrderCheckExempt,
            out SqlInt32 InsuredCreditLimit,
            out SqlByte CreditLimitCheckAgainst,
            out SqlDateTime InsuredCreditLimitExpiryDT,
            out SqlString Bankrupt,
            out SqlBoolean ExcludeFromInvoicePrint,
            out SqlBoolean SendOverdueLetters,
            out SqlByte OverdueInvoiceGracePeriod,
            out SqlByte OverdueLetter1Delay,
            out SqlByte OverdueLetter2Delay,
            out SqlByte OverdueLetter3Delay,
            out SqlByte OverdueLetter4Delay,
            out SqlByte OverdueLetter5Delay,
            out SqlBoolean EDIInvoice,
            out SqlBoolean CommericalInvoice,
            out SqlBoolean ProformaTrading,
            out SqlDecimal ProformaSurcharge,
            out SqlString InvoiceDetailPrint,
            out SqlByte CreditNoteCopiesCount,
            out SqlBoolean ABCPricing,
            out SqlByte InvoiceCopiesCount,
            out SqlBoolean ContractPricing,
            out SqlBoolean PrintInvoiceDetails,
            out SqlBoolean EndCustomerRequired,
            out SqlBoolean EndCustomerInvoicing,
            out SqlString CreditCardDetails,
            out SqlBoolean StatementPrint,
            out SqlBoolean RetailTillCustomer,
            out SqlBoolean PermitTillCreditNotes,
            out SqlString StatementTradeAccountCode,
            out SqlString StatementAccountName,
            out SqlString StatementAddressLine1,
            out SqlString StatementAddressLine2,
            out SqlString StatementAddressTown,
            out SqlString StatementAddressCounty,
            out SqlString StatementAddressPostcode,
            out SqlString StatementContactName,
            out SqlString StatementPhoneNumber,
            out SqlBoolean PurchaseOrderRequired,
            out SqlString AnalysisCodeA,
            out SqlString AnalysisCodeD,
            out SqlString AnalysisCodeS,
            out SqlString Memo,
            out SqlString Contacts,
            out SqlString RFScannerMemos,
            out SqlString ConfirmationMemos,
            out SqlString ExtendedMemos,
            out SqlString FreeOfChargePromotionCode,
            out SqlString SellingCompany,
            out SqlString DistributorReference,
            out SqlBoolean TransferAccount,
            out SqlBoolean StockOnlyAccount,
            out SqlString VATNormalRateCode,
            out SqlString VATNumber,
            out SqlString VATCountry,
            out SqlDateTime VATLastCheckedDT,
            out SqlString VATNumberValid,
            out SqlString VATNumberInvalid,
            out SqlString VATInvoiceDisplay,
            out SqlString HaulierCode,
            out SqlString CarriageCode,
            out SqlString UPSInvoiceAccountNumber,
            out SqlString UPSTaxesDutyAccountNumber,
            out SqlString DHLAccountNumber,
            out SqlString TNTAccountNumber,
            out SqlString DeliveryTerms,
            out SqlString GroupageType,
            out SqlBoolean DirectDelivery,
            out SqlByte PackingListCopiesCount,
            out SqlBoolean ApplyMinimumCarriageValueSurcharge,
            out SqlBoolean DeliveryChargeReconfirmation,
            out SqlByte MinimumOrderQuantity,
            out SqlString ReceivingLocation,
            out SqlByte CutVerify,
            out SqlBoolean OutOfStockLetter,
            out SqlString DirectToCustomer,
            out SqlBoolean FullOrderOnly,
            out SqlBoolean UPSReturnsLabel,
            out SqlDecimal UPSAddOnPercentage,
            out SqlByte DefaultOrderPriority,
            out SqlString PartnerCompany,
            out SqlBoolean AcceptSundries,
            out SqlString SampleStockLocation,
            out SqlString SampleHaulierCode,
            out SqlString SampleCarriageCode,
            out SqlString SampleCarriageExempt,
            out SqlString DefaultStockLocation,
            out SqlBoolean Export,
            out SqlBoolean ExportWithNoPackingNote,
            out SqlBoolean Consolidate,
            out SqlString CarriageSurchargeExempt,
            out SqlString OrderConfirmationPrint,
            out SqlBoolean SendCancellationLetters,
            out SqlBoolean InterimStatementActions,
            out SqlString InformOnPromiseDateChanged,
            out SqlBoolean ProductScreen,
            out SqlBoolean CheckForDuplicates,
            out SqlBoolean MeasurePiece,
            out SqlBoolean AllocateStockForProformaOrders,
            out SqlBoolean PrintInvoiceAtConfirmation,
            out SqlBoolean PrintUnitPriceOnDeliveryNote,
            out SqlBoolean PrintJLPEanLabels,
            out SqlBoolean NewPickingListForGoodsInProblemItems,
            out SqlBoolean MTMCarriageCharged,
            out SqlDecimal StandardRollLength,
            out SqlString FileProtocol,
            out SqlString UserID,
            out SqlString SampleLetterTitleText,
            out SqlString ProductExclusionListCode,
            out SqlString USSalesTaxExemptionCertificate,
            out SqlString DeliveryDateCalculation,
            out SqlDecimal MinimumOrderValue,
            out SqlString TillIDCode,
            out SqlString UsualOrderUnit,
            out SqlString StoreName,
            out SqlString TransferAddress,
            out SqlString TransferPassword,
            out SqlString HaulierForConsolidatedInvoice,
            out SqlInt32 CutOffValue,
            out SqlByte DaysForwardForReservations,
            out SqlString DebtCollector,
            out SqlString SSField2,
            out SqlString CreditControlRegion,
            out SqlString SSField5,
            out SqlString StockistCategory,
            out SqlString ExitCodeEmail)
        {
            var result = (TradeAccount)obj;

            if (result == null)
            {
                result = new TradeAccount(null);
            }

            TradeAccountCode = result.Key?.TrimReduceTruncate(50);
            Name = result.Name?.TrimReduceTruncate(100);
            Status = result.Status?.TrimReduceTruncate(1);
            SortName = result.SortName?.TrimReduceTruncate(100);
            CreatedDT = result.CreatedDT ?? SqlDateTime.Null;
            ModifiedDT = result.ModifiedDT ?? SqlDateTime.Null;
            TradeAccountGroup = result.TradeAccountGroup?.TrimReduceTruncate(3);
            ContactName = result.ContactName?.TrimReduceTruncate(100);
            InvoiceAddressLine1 = result.InvoiceAddressLine1?.TrimReduceTruncate(150);
            InvoiceAddressLine2 = result.InvoiceAddressLine2?.TrimReduceTruncate(100);
            InvoiceAddressTown = result.InvoiceAddressTown?.TrimReduceTruncate(100);
            InvoiceAddressCounty = result.InvoiceAddressCounty?.TrimReduceTruncate(100);
            InvoiceAddressPostcode = result.InvoiceAddressPostcode?.TrimReduceTruncate(40);
            InvoiceAddressCountryCode = result.InvoiceAddressCountryCode?.TrimReduceTruncate(3);
            InvoiceAccounts = result.InvoiceAccounts.Any() ? JsonConvert.SerializeObject(result.InvoiceAccounts) : SqlString.Null;
            StatementTradeAccountCode = result.StatementTradeAccountCode?.TrimReduceTruncate(50);
            StatementAccountName = result.StatementAccountName?.TrimReduceTruncate(100);
            StatementAddressLine1 = result.StatementAddressLine1?.TrimReduceTruncate(60);
            StatementAddressLine2 = result.StatementAddressLine2?.TrimReduceTruncate(60);
            StatementAddressTown = result.StatementAddressTown?.TrimReduceTruncate(50);
            StatementAddressCounty = result.StatementAddressCounty?.TrimReduceTruncate(40);
            StatementAddressPostcode = result.StatementAddressPostcode?.TrimReduceTruncate(20);
            StatementContactName = result.StatementContactName?.TrimReduceTruncate(100);
            StatementPhoneNumber = result.StatementPhoneNumber?.TrimReduceTruncate(50);
            PhoneNumber = result.PhoneNumber?.TrimReduceTruncate(50);
            Email = result.Email?.TrimReduceTruncate(254);
            MarketingEmail = result.MarketingEmail?.TrimReduceTruncate(254);
            Website = result.Website?.TrimReduceTruncate(200);
            Market = result.Market?.TrimReduceTruncate(1);
            TradeAccountType = result.TradeAccountType?.TrimReduceTruncate(2);
            TradeAccountClass = result.TradeAccountClass?.TrimReduceTruncate(2);
            SalesRepresentativeCode = result.SalesRepresentativeCode?.TrimReduceTruncate(3);
            SalesAgentCode = result.SalesAgentCode?.TrimReduceTruncate(3);
            SalesArea = result.SalesArea?.TrimReduceTruncate(10);
            SalesDivision = result.SalesDivision?.TrimReduceTruncate(3);
            CompanyRegistrationNumber = result.CompanyRegistrationNumber?.TrimReduceTruncate(50);
            CustomersReference = result.CustomersReference?.TrimReduceTruncate(50);
            EORINumber = result.EORINumber?.TrimReduceTruncate(50);
            TaxID = result.TaxID?.TrimReduceTruncate(50);
            VATNumber = result.VATNumber?.TrimReduceTruncate(30);
            VATCountry = result.VATCountry?.TrimReduceTruncate(3);
            VATNormalRateCode = result.VATNormalRateCode?.TrimReduceTruncate(1);
            VATLastCheckedDT = result.VATLastCheckedDT ?? SqlDateTime.Null;
            VATNumberValid = result.VATNumberValid?.TrimReduceTruncate(3);
            VATNumberInvalid = result.VATNumberInvalid?.TrimReduceTruncate(50);
            Currency = result.Currency?.TrimReduceTruncate(3);
            PriceList = result.PriceList?.TrimReduceTruncate(10);
            BankCode = result.BankCode?.TrimReduceTruncate(3);
            CreditController = result.CreditController?.TrimReduceTruncate(10);
            CreditLimit = result.CreditLimit ?? SqlInt32.Null;
            CreditLimitExpiryDT = result.CreditLimitExpiryDT ?? SqlDateTime.Null;
            HighValueOrderCheckExempt = result.HighValueOrderCheckExempt;
            InsuredCreditLimit = result.InsuredCreditLimit ?? SqlInt32.Null;
            CreditStopped = result.CreditStopped;
            CreditLimitCheckAgainst = result.CreditLimitCheckAgainst ?? SqlByte.Null;
            InsuredCreditLimitExpiryDT = result.InsuredCreditLimitExpiryDT ?? SqlDateTime.Null;
            Bankrupt = result.Bankrupt?.TrimReduceTruncate(20);
            PaymentTerms = result.PaymentTerms?.TrimReduceTruncate(120);
            PurchaseOrderRequired = result.PurchaseOrderRequired;
            HaulierCode = result.HaulierCode?.TrimReduceTruncate(10);
            CarriageCode = result.CarriageCode?.TrimReduceTruncate(10);
            UPSInvoiceAccountNumber = result.UPSInvoiceAccountNumber?.TrimReduceTruncate(20);
            UPSTaxesDutyAccountNumber = result.UPSTaxesDutyAccountNumber?.TrimReduceTruncate(20);
            DHLAccountNumber = result.DHLAccountNumber?.TrimReduceTruncate(20);
            TNTAccountNumber = result.TNTAccountNumber?.TrimReduceTruncate(20);
            DeliveryTerms = result.DeliveryTerms?.TrimReduceTruncate(3);
            ExcludeFromInvoicePrint = result.ExcludeFromInvoicePrint;
            VATInvoiceDisplay = result.VATInvoiceDisplay?.TrimReduceTruncate(50);
            OverdueInvoiceGracePeriod = result.OverdueInvoiceGracePeriod ?? SqlByte.Null;
            EDIInvoice = result.EDIInvoice;
            ExtendedMemos = result.ExtendedMemos.Any() ? JsonConvert.SerializeObject(result.ExtendedMemos) : SqlString.Null;
            PartnerCompany = result.PartnerCompany?.TrimReduceTruncate(20);
            AnalysisCodeA = result.AnalysisCodeA?.TrimReduceTruncate(12);
            AnalysisCodeD = result.AnalysisCodeD?.TrimReduceTruncate(2);
            AnalysisCodeS = result.AnalysisCodeS?.TrimReduceTruncate(2);
            Memo = result.Memo?.TrimReduceTruncate(500);

            var dummyList = new List<string>();

            for (var i = 0; i < result.Contacts?.Count(); i++)
            {
                dummyList.Add(null);
            }

            var emails = (result.ContactEmails?.Count() >= dummyList.Count ? result.ContactEmails : dummyList).ToArray();
            var phones = (result.ContactPhones?.Count() >= dummyList.Count ? result.ContactPhones : dummyList).ToArray();
            var mobiles = (result.ContactPhonesMobile?.Count() >= dummyList.Count ? result.ContactPhonesMobile : dummyList).ToArray();
            var types = (result.ContactTypes?.Count() >= dummyList.Count ? result.ContactTypes : dummyList).ToArray();

            Contacts = result.Contacts.Any() ? JsonConvert.SerializeObject(result.Contacts.Select((c, i) => {
                return new
                {
                    Name = c,
                    Email = emails[i],
                    Phone = phones[i],
                    Mobile = mobiles[i],
                    Type = types[i]
                };
            })) : SqlString.Null;

            AcceptSundries = result.AcceptSundries;
            OverdueLetter1Delay = result.OverdueLetter1Delay ?? SqlByte.Null;
            OverdueLetter2Delay = result.OverdueLetter2Delay ?? SqlByte.Null;
            OverdueLetter3Delay = result.OverdueLetter3Delay ?? SqlByte.Null;
            OverdueLetter4Delay = result.OverdueLetter4Delay ?? SqlByte.Null;
            OverdueLetter5Delay = result.OverdueLetter5Delay ?? SqlByte.Null;
            DefaultDeliveryReference = result.DefaultDeliveryReference?.TrimReduceTruncate(20);
            CommericalInvoice = result.CommercialInvoice;
            SampleStockLocation = result.SampleStockLocation?.TrimReduceTruncate(10);
            SampleHaulierCode = result.SampleHaulierCode?.TrimReduceTruncate(10);
            SampleCarriageCode = result.SampleCarriageCode?.TrimReduceTruncate(10);
            SampleCarriageExempt = result.SampleCarriageExempt?.TrimReduceTruncate(1);
            DefaultStockLocation = result.DefaultStockLocation?.TrimReduceTruncate(10);
            Export = result.Export;
            ExportWithNoPackingNote = result.ExportWithNoPackingNote;
            Consolidate = result.Consolidate;
            CarriageSurchargeExempt = result.CarriageSurchargeExempt?.TrimReduceTruncate(1);
            OrderConfirmationPrint = result.OrderConfirmationPrint?.TrimReduceTruncate(1);
            ProformaTrading = result.ProformaTrading;
            FreeOfChargePromotionCode = result.FreeOfChargePromotionCode?.TrimReduceTruncate(5);
            InvoiceDetailPrint = result.InvoiceDetailPrint?.TrimReduceTruncate(1);
            SellingCompany = result.SellingCompany?.TrimReduceTruncate(10);
            GroupageType = result.GroupageType?.TrimReduceTruncate(1);
            DirectDelivery = result.DirectDelivery;
            PackingListCopiesCount = result.PackingListCopiesCount ?? SqlByte.Null;
            CreditNoteCopiesCount = result.CreditNoteCopiesCount ?? SqlByte.Null;
            ABCPricing = result.ABCPricing;
            StandardRollLength = result.StandardRollLength ?? SqlDecimal.Null;
            ApplyMinimumCarriageValueSurcharge = result.ApplyMinimumCarriageValueSurcharge;
            DeliveryChargeReconfirmation = result.DeliveryChangeReconfirmation;
            ProformaSurcharge = result.ProformaSurcharge ?? SqlDecimal.Null;
            InvoiceCopiesCount = result.InvoiceCopiesCount ?? SqlByte.Null;
            DistributorReference = result.DistributorReference?.TrimReduceTruncate(10);
            TransferAccount = result.TransferAccount;
            StockOnlyAccount = result.StockOnlyAccount;
            ReceivingLocation = result.ReceivingLocation?.TrimReduceTruncate(10);
            ContractPricing = result.ContractPricing;
            ProductScreen = result.ProductScreen;
            MinimumOrderQuantity = result.MinimumOrderQuantity ?? SqlByte.Null;
            SendOverdueLetters = result.SendOverdueLetters;
            PrintInvoiceDetails = result.PrintInvoiceDetails;
            CutVerify = result.CutVerify ?? SqlByte.Null;
            CreditCardDetails = result.CreditCardDetails?.TrimReduceTruncate(1);
            OutOfStockLetter = result.OutOfStockLetter;
            StatementPrint = result.StatementPrint;
            InternalOrExternal = result.InternalOrExternal?.TrimReduceTruncate(1);
            DirectToCustomer = result.DirectToCustomer?.TrimReduceTruncate(20);
            FileProtocol = result.FileProtocol?.TrimReduceTruncate(1);
            UserID = result.UserID?.TrimReduceTruncate(20);
            PermitTillCreditNotes = result.PermitTillCreditNotes;
            SampleLetterTitleText = result.SampleLetterTitleText?.TrimReduceTruncate(2);
            EndCustomerInvoicing = result.EndCustomerInvoicing;
            ProductExclusionListCode = result.ProductExclusionListCode?.TrimReduceTruncate(3);
            USSalesTaxExemptionCertificate = result.USSalesTaxExemptionCertificate?.TrimReduceTruncate(255);
            MiscellaneousAccount = result.MiscellaneousAccount;
            PriceCode = result.PriceCode?.TrimReduceTruncate(3);
            DeliveryDateCalculation = result.DeliveryDateCalculation?.TrimReduceTruncate(2);
            MinimumOrderValue = result.MinimumOrderValue ?? SqlDecimal.Null;
            CheckForDuplicates = result.CheckForDuplicates;
            TillIDCode = result.TillIDCode?.TrimReduceTruncate(10);
            UsualOrderUnit = result.UsualOrderUnit?.TrimReduceTruncate(5);
            SendCancellationLetters = result.SendCancellationLetters;
            InterimStatementActions = result.InterimStatementActions;
            InformOnPromiseDateChanged = result.InformOnPromiseDateChanged?.TrimReduceTruncate(1);
            StoreName = result.StoreName?.TrimReduceTruncate(20);
            MeasurePiece = result.MeasurePiece;
            TransferAddress = result.TransferAddress?.TrimReduceTruncate(200);
            TransferPassword = result.TransferPassword?.TrimReduceTruncate(20);
            RetailTillCustomer = result.RetailTillCustomer;
            EndCustomerRequired = result.EndCustomerRequired;
            DefaultOrderPriority = result.DefaultOrderPriority ?? SqlByte.Null;
            FullOrderOnly = result.FullOrderOnly;
            SendInvoiceToDeliveryAddress = result.SendInvoiceToDeliveryAddress;
            PrintUnitPriceOnDeliveryNote = result.PrintUnitPriceOnDeliveryNote;
            OrderCategory = result.OrderCategory?.TrimReduceTruncate(2);
            RetailOrContract = result.RetailOrContract?.TrimReduceTruncate(1);
            HaulierForConsolidatedInvoice = result.HaulierCodeForConsolidatedInvoice?.TrimReduceTruncate(10);
            NewPickingListForGoodsInProblemItems = result.NewPickingListForGoodsInProblemItems;
            MTMCarriageCharged = result.MTMCarriageCharged;
            CutOffValue = result.CutOffValue ?? SqlInt32.Null;
            UPSAddOnPercentage = result.UPSAddOnPercentage ?? SqlDecimal.Null;
            AllocateStockForProformaOrders = result.AllocateStockForProformaOrders;
            PrintInvoiceAtConfirmation = result.PrintInvoiceAtConfirmation;
            DaysForwardForReservations = result.DaysForwardForReservations ?? SqlByte.Null;
            PrintJLPEanLabels = result.PrintJLPEanLabels;
            RFScannerMemos = result.RFScannerMemos.Any() ? JsonConvert.SerializeObject(result.RFScannerMemos) : SqlString.Null;
            ConfirmationMemos = result.ConfirmationMemos.Any() ? JsonConvert.SerializeObject(result.ConfirmationMemos) : SqlString.Null;
            DebtCollector = result.DebtCollector;
            SSField2 = result.DeliveryAlertEmail?.TrimReduceTruncate(50);
            CreditControlRegion = result.CreditControlRegion?.TrimReduceTruncate(50);
            SSField5 = result.SSField5?.TrimReduceTruncate(50);
            ContractOrDisplay = result.ContractOrDisplay?.TrimReduceTruncate(1);
            Advertiser = result.Advertiser;
            Stockist = result.Stockist;
            StockistCategory = result.StockistCategory?.TrimReduceTruncate(2);
            UPSReturnsLabel = result.UPSReturnsLabel;
            ExitCodeEmail = result.ExitCodeEmail?.TrimReduceTruncate(254);
            NoEmail = result.NoEmail;
        }

        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "EndCustomerSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable EndCustomerSelect(string UniVerseAccount)
        {
            int f = 0;

            var results = new List<Tuple<string, string>>();

            var accounts = new List<string>();

            using (var qe = new CommandExecutor(UniVerseAccount))
            {
                accounts = qe.Arbitrary($"LIST SLCUST @ID FMT\"60L\" WITH INV.ACCOUNTS # \"\" COUNT.SUP ID.SUPP COL.HDR.SUPP")
                    .Split(Framework.EnvironmentNewLineDelimiter, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();
            }

            using (var qe = new QueryExecutor(UniVerseAccount))
            {
                foreach (var acc in accounts)
                {
                    var xmlResult = qe.Run("LIST_END_CUSTOMERS", $"CUSREF=\"{acc}\"", "CUSREF", "");

                    var xDoc = XDocument.Parse(xmlResult);

                    var pr = new List<Tuple<string, string>>();

                    xDoc.Descendants("record").ToList().ForEach(c =>
                    {
                        var cusRef = c.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value;

                        results.Add(new Tuple<string,string>(acc, cusRef));
                    });
                }
            }

            foreach (var r in results.OrderBy(r => r.Item1).ThenBy(r => r.Item2))
            {
                yield return r;
            }
        }

        public static void EndCustomerSelect_FillRow(
            object obj,
            out SqlString ParentTradeAccountCode,
            out SqlString TradeAccountCode)
        {
            var result = (Tuple<string,string>)obj;

            ParentTradeAccountCode = result.Item1.TrimReduceTruncate(50);
            TradeAccountCode = result.Item2.TrimReduceTruncate(50);
        }

        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "EndCustomerSearch_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable EndCustomerSearch(string TradeDataSourceJson, string SearchTerm)
        {
            var preResult = new List<Tuple<string, string>>();
            
            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(TradeDataSourceJson);

            var locker = new object();

            foreach (var dataSource in dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.EndCustomers) // I have no idea what EnquiryType we'll want for this...
                                        .Single()
                                        .DataSources)
            {
                using (var qe = new QueryExecutor(dataSource.UniVerseAccount))
                {
                    var xmlResult = qe.Run("LIST_END_CUSTOMERS", $"CUSREF=\"{dataSource.CustomerReference}\"", "CUSREF CUSNAME", "");

                    var xDoc = XDocument.Parse(xmlResult);

                    var pr = new List<Tuple<string, string>>();

                    xDoc.Descendants("record").ToList().ForEach(c =>
                    {
                        var cusRef = c.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value;
                        var cusName = c.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value;

                        pr.Add(new Tuple<string, string>(cusRef, cusName));
                    });

                    //lock (locker)
                    //{
                        preResult.AddRange(pr);
                    //}
                }
            }

            var result = new List<string>();

            if (!String.IsNullOrWhiteSpace(SearchTerm))
            {
                SearchTerm = SearchTerm.ToLowerInvariant();

                result = preResult.Where(p => (p.Item1.ToLowerInvariant().StartsWith(SearchTerm)) || p.Item2.ToLowerInvariant().Contains(SearchTerm)).Select(p => p.Item1).ToList();
            }
            else
            {
                result = preResult.Select(p => p.Item1).ToList();
            }

            foreach (var r in result.OrderBy(r => r))
            {
                yield return r;
            }
        }

        public static void EndCustomerSearch_FillRow(
            object obj,
            out SqlString TradeAccountCode)
        {
            var result = (string)obj;

            TradeAccountCode = result.TrimReduceTruncate(50);
        }

        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "TradeAccountContactSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable ContactSelect(string UniVerseAccount, string TradeAccountCodes)
        {
            if (String.IsNullOrEmpty(TradeAccountCodes))
            {
                foreach (var r in Selector<TradeAccount>.SelectAllNonParallel(
                    true,
                    false,
                    50,
                    UniVerseAccount))
                {
                    foreach (var c in r.TradeAccountContacts)
                    {
                        yield return c;
                    }
                }
            }
            else
            {
                var results = Selector<TradeAccount>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<TradeAccountCode>>(TradeAccountCodes).Select(p => p.Code),
                    true,
                    false,
                    50,
                    UniVerseAccount)
                    .ToList();

                foreach (var r in results)
                {
                    foreach (var c in r.TradeAccountContacts)
                    {
                        yield return c;
                    }
                }
            }
        }

        public static void TradeAccountContactSelect_FillRow(
            object obj,
            out SqlString TradeAccountContactTypeCode,
            out SqlString TradeAccountCode,
            out SqlString Name,
            out SqlString Email,
            out SqlString Phone,
            out SqlString Mobile)
        {
            var result = (TradeAccountContact)obj;

            TradeAccountContactTypeCode = result.Type;
            TradeAccountCode = result.TradeAccountCode;
            Name = result.Name?.TrimReduceTruncate(100);
            Email = result.Email?.TrimReduceTruncate(254);
            Phone = result.Phone?.TrimReduceTruncate(20);
            Mobile = result.Mobile?.TrimReduceTruncate(20);
        }
    }
}

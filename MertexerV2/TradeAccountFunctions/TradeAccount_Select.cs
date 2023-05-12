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
            out SqlString EORINumber,
            out SqlString TaxID,
            out SqlString Currency,
            out SqlString TradeAccountGroup,
            out SqlString CustomerType,
            out SqlString CustomerClass,
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
            out SqlString SendInvoiceToDeliveryAddress,
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
            out SqlString StatementAccount,
            out SqlString StatementAccountName,
            out SqlString StatementContactName,
            out SqlString StatementAddressLine1,
            out SqlString StatementAddressLine2,
            out SqlString StatementAddressTown,
            out SqlString StatementAddressCounty,
            out SqlString StatementAddressPostcode,
            out SqlString StatementPhoneNumber,
            out SqlBoolean PurchaseOrderRequired,
            out SqlString AnalysisCodeA,
            out SqlString AnalysisCodeD,
            out SqlString AnalysisCodeS,
            out SqlString Memo,
            out SqlString RFScannerMemo,
            out SqlString ConfirmationMemo,
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
            out SqlString DefaultStockLocation,
            out SqlBoolean Export,
            out SqlBoolean ExportWithNoPackingNote,
            out SqlBoolean Consolidate,
            out SqlString CarriageSurchargeExempt,
            out SqlString OrderConfirmationPrint,
            out SqlBoolean CancellationLetters,
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
            out SqlString SSField1,
            out SqlString SSField2,
            out SqlString SSField4,
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
            InvoiceAccounts = JsonConvert.SerializeObject(result.InvoiceAccounts);
            StatementAccount = result.StatementAccount?.TrimReduceTruncate(50);
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
            CustomerType = result.CustomerType?.TrimReduceTruncate(2);
            CustomerClass = result.CustomerClass?.TrimReduceTruncate(2);
            SalesRepresentativeCode = result.SalesRepresentativeCode?.TrimReduceTruncate(3);
            SalesAgentCode = result.SalesAgentCode?.TrimReduceTruncate(3);
            SalesArea = result.SalesArea?.TrimReduceTruncate(10);
            SalesDivision = result.SalesDivision?.TrimReduceTruncate(3);
            CompanyRegistrationNumber = result.CompanyRegistrationNumber?.TrimReduceTruncate(50);
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
            ExtendedMemos = JsonConvert.SerializeObject(result.ExtendedMemos);
            PartnerCompany = result.PartnerCompany?.TrimReduceTruncate(20);
            AnalysisCodeA = result.AnalysisCodeA?.TrimReduceTruncate(12);
            AnalysisCodeD = result.AnalysisCodeD?.TrimReduceTruncate(2);
            AnalysisCodeS = result.AnalysisCodeS?.TrimReduceTruncate(2);
            AcceptSundries = result.AcceptSundries;
            OverdueLetter1Delay = result.OverdueLetter1Delay ?? SqlByte.Null;
            OverdueLetter2Delay = result.OverdueLetter2Delay ?? SqlByte.Null;
            OverdueLetter3Delay = result.OverdueLetter3Delay ?? SqlByte.Null;
            OverdueLetter4Delay = result.OverdueLetter4Delay ?? SqlByte.Null;
            OverdueLetter5Delay = result.OverdueLetter5Delay ?? SqlByte.Null;
            DefaultDeliveryReference = result.DefaultDeliveryReference?.TrimReduceTruncate(20);
            CommericalInvoice = result.CommercialInvoice;
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
            MiscellaneousAccount = result.MiscellaneousAccount;
            PriceCode = result.PriceCode?.TrimReduceTruncate(3);
            DeliveryDateCalculation = result.DeliveryDateCalculation?.TrimReduceTruncate(2);
            MinimumOrderValue = result.MinimumOrderValue ?? SqlDecimal.Null;
            CheckForDuplicates = result.CheckForDuplicates;
            TillIDCode = result.TillIDCode?.TrimReduceTruncate(10);
            UsualOrderUnit = result.UsualOrderUnit?.TrimReduceTruncate(5);
            CancellationLetters = result.CancellationLetters;
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
            RFScannerMemo = result.RFScannerMemo?.TrimReduceTruncate(200);
            ConfirmationMemo = result.ConfirmationMemo?.TrimReduceTruncate(200);
            SSField1 = result.SSField1?.TrimReduceTruncate(50);
            SSField2 = result.SSField2?.TrimReduceTruncate(50);
            SSField4 = result.SSField4?.TrimReduceTruncate(50);
            SSField5 = result.SSField5?.TrimReduceTruncate(50);
            ContractOrDisplay = result.ContractOrDisplay?.TrimReduceTruncate(1);
            Advertiser = result.Advertiser;
            Stockist = result.Stockist;
            StockistCategory = result.StockistCategory?.TrimReduceTruncate(2);
            UPSReturnsLabel = result.UPSReturnsLabel;
            ExitCodeEmail = result.ExitCodeEmail?.TrimReduceTruncate(254);
            NoEmail = result.NoEmail;
            Memo = result.Memo?.TrimReduceTruncate(500);
        }

        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "EndCustomerSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable EndCustomerSelect(string TradeDataSourceJson, string SearchTerm)
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

        public static void EndCustomerSelect_FillRow(
            object obj,
            out SqlString TradeAccountCode)
        {
            var result = (string)obj;

            TradeAccountCode = result.TrimReduceTruncate(50);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MertexerV2.Utilities;
using Newtonsoft.Json;

namespace MertexerV2.Models.PurchaseLedger
{
    [UniVerseFileName("TPLEDG")]
    public class PurchaseLedger : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public PurchaseLedger(string key) : base(key)
        {
            Initialise();
        }

        public PurchaseLedger() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(PurchaseLedger).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        // There are some 350 fields on here. 
        // Aw, heck.

        // That's nothing: you're going to map them (all) then you're going to go through the changes in source control
        // and double check every single field
        // 
        // Then test all of the data by creating the appropriate CLRs for SELECTion
        // (and adjusting the extant ones which have had additional fields)
        //
        // Then you're going to create tables in the Mertex schema to hold all of this data, so we can pull and keep it
        // in SQL
        //
        // Oh, boing.

        // Well, great. This is an empty field.
        // >LIST TPLEDG WITH CODE # ""
        // >0 records listed.
        //[UniVerseIndex(1)]
        //[UniVerseFieldName("CODE")]
        //public string Code { get; set; }

        // This one isn't even named in the dictionary, I've just...
        // read the data enough to know that it ought to be set appropriately.
        [UniVerseIndex(2)]
        [UniVerseFieldName("TRANSACTIONDEFINITION")]
        public string SourceTransactionDefinition { get; set; }

        [UniVerseIndex(4)]
        [UniVerseFieldName("DISC")]
        [UniVerseConversion("MR33")]
        public decimal? DiscountPercentage { get; set; }

        [UniVerseIndex(5)]
        [UniVerseFieldName("DAYS")]
        public byte? Days { get; set; }

        // Deprecated / unused
        [UniVerseIndex(7)]
        [UniVerseFieldName("NUMDIS")]
        public byte? DiscountNumber { get; set; }

        // Deprecated / unused
        [UniVerseIndex(8)]
        [UniVerseFieldName("DECDIS")]
        public byte? DiscountDecimals { get; set; }

        /// <summary>
        /// "This Pd Zero ?    Y/N"
        /// Default Value P("DCURR")[S][M]
        /// DCURR is a "Default" definition
        /// IF(F("PLSUP",<14>)<18,1> = 0 OR F("PLSUP",<14>)<18,1> = "","Y","N"
        /// 
        /// Essentially, it's a lookup to PLSUP to read "NEXT" (field 18.1).
        /// If that's empty or zero, then this period is indeed zero.
        /// </summary>
        [UniVerseIndex(16)]
        [UniVerseFieldName("CURR")]
        public bool? ThisPeriodZero { get; set; }

        /// <summary>
        /// Period 1 Zero?
        /// Default Value P("DPD1")[S][M]
        /// DPD1 is a "Default" definition
        /// IF(F("SLCUST",<14>)<18,2> = 0 OR F("SLCUST",<14>)<18,2> = "","Y","N"
        /// 
        /// Essentially, it's a lookup to SLCUST to read "PD1" (field 18.2).
        /// If that's empty or zero, then this period is indeed zero.
        /// </summary>
        [UniVerseIndex(17)]
        [UniVerseFieldName("PD1")]
        public bool? Period1Zero { get; set; }

        /// <summary>
        /// Period 1 Zero?
        /// Default Value P("DPD2")[S][M]
        /// DPD1 is a "Default" definition
        /// IF(F("SLCUST",<14>)<18,3> = 0 OR F("SLCUST",<14>)<18,3> = "","Y","N"
        /// 
        /// Essentially, it's a lookup to SLCUST to read "PD2" (field 18.3).
        /// If that's empty or zero, then this period is indeed zero.
        /// </summary>
        [UniVerseIndex(18)]
        [UniVerseFieldName("PD2")]
        public bool? Period2Zero { get; set; }

        /// <summary>
        /// Period 3 Zero?
        /// Default Value P("DPD3")[S][M]
        /// DPD1 is a "Default" definition
        /// IF(F("SLCUST",<14>)<18,4> = 0 OR F("SLCUST",<14>)<18,4> = "","Y","N"
        /// 
        /// Essentially, it's a lookup to SLCUST to read "PD3" (field 18.4).
        /// If that's empty or zero, then this period is indeed zero.
        /// </summary>
        [UniVerseIndex(19)]
        [UniVerseFieldName("PD3")]
        public bool? Period3Zero { get; set; }

        /// <summary>
        /// Period 4 Zero?
        /// Default Value P("DPD4")[S][M]
        /// DPD1 is a "Default" definition
        /// IF(F("SLCUST",<14>)<18,5> = 0 OR F("SLCUST",<14>)<18,5> = "","Y","N"
        /// 
        /// Essentially, it's a lookup to SLCUST to read "PD4" (field 18.5).
        /// If that's empty or zero, then this period is indeed zero.
        /// </summary>
        [UniVerseIndex(20)]
        [UniVerseFieldName("PD4")]
        public bool? Period4Zero { get; set; }

        /// <summary>
        /// No activity?
        /// Default Value P("DNOACT")[S][M]
        /// DNOACT is a "Default" definition
        /// IF(F("PLSUP",<14>)<19> = "","Y","N")
        /// 
        /// Essentially, it's a lookup to PLSUP to read "TLINK" (field 19.M).
        /// If that's empty, then there indeed has never been any activity on the supplier.
        /// </summary>
        [UniVerseIndex(21)]
        [UniVerseFieldName("NOACT")]
        public bool? NoActivity { get; set; }

        /// <summary>
        /// Balance Zero?
        /// Default Value P("DBALZ")[S][M]
        /// DBALZ is a "Default" definition
        /// IF(F("PLSUP",<14>)<15> = "0" OR F("PLSUP"<14>)<15> = "","Y","N")
        /// 
        /// Essentially, it's a lookup to PLSUP to read "BALANCE" (field 15).
        /// If that's empty, or zero, then there indeed is zero balance.
        /// </summary>
        [UniVerseIndex(22)]
        [UniVerseFieldName("BALANCE")]
        public bool? BalanceZero { get; set; }

        /// <summary>
        /// Deletion Valid?
        /// Default Value P("DVALID")[S][M]
        /// DVALID is a "Default" definition
        /// IF(
        ///     @RECORD<16> = "Y" AND 
        ///     @RECORD<17> = "Y" AND 
        ///     @RECORD<18> = "Y" AND 
        ///     @RECORD<19> = "Y" AND
        ///     @RECORD<20> = "Y" AND
        ///     @RECORD<21> = "Y" AND
        ///     @RECORD<22> = "Y" AND
        ///     @RECORD<25> = "Y" AND
        ///     @RECORD<27> = "Y",
        /// "Y","N")
        /// 
        /// Essentially, it's a check against nine fields on the current @RECORD determine whether or not
        /// a deletion of this record is appropriate / permitted.
        /// </summary>
        [UniVerseIndex(23)]
        [UniVerseFieldName("VALID")]
        public bool? DeletionValid { get; set; }

        /// <summary>
        /// Confirm Delete?
        /// Default Value P("DCONF")
        /// DCONF is a "Default" definition
        /// IF(<23> = "Y","","N"[S])
        /// 
        /// Essentially, it's a lookup to field 23 on @RECORD to read "VALID".
        /// If that's equal to "Y", then you don't need to confirm deletion. Otherwise, you do need to confirm deletion.
        /// </summary>
        [UniVerseIndex(24)]
        [UniVerseFieldName("CONFIRM")]
        public bool? ConfirmOnDelete { get; set; }

        /// <summary>
        /// No order activitiy?
        /// Default Value P("DORDACT")[S][M]
        /// DORDACT is a "Default" definition
        /// IF(F("PLSUP"<14>)<39> = "","Y","N")
        /// 
        /// Essentially, it's a lookup to field 39 on PLSUP to read "OLINK", an MV field containing order numbers.
        /// If that's equal to "", then there have been no orders from this supplier.
        /// </summary>
        [UniVerseIndex(25)]
        [UniVerseFieldName("ORDACT")]
        public bool? NoOrderActivity { get; set; }

        /// <summary>
        /// No Current transactions this year?
        /// 
        /// P("RSP,SL,PG.CURRYR.TRANS,PL")[S][M]
        /// Runs a paragraph to determine if there have been any transactions this year (financial or calendar, I don't know)
        /// </summary>
        [UniVerseIndex(27)]
        [UniVerseFieldName("CURRYR.TRANS")]
        public bool? NoCurrentTransactionsThisYear { get; set; }

        [JsonAlsoUpdates("DocumentDate")]
        public DateTime? DocumentDT { get; set; }

        [UniVerseIndex(33)]
        [UniVerseFieldName("SS.DOCDATE.INT")]
        public string DocumentDate
        {
            get
            {
                if (DocumentDT.HasValue)
                {
                    return Framework.UniVerseDateString(DocumentDT.Value);
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    if (Int32.TryParse(value, out int valInt))
                    {
                        DocumentDT = Framework.DateTimeFromUniVerseOffset(valInt);
                    }
                    else
                    {
                        DocumentDT = null;
                    }
                }
            }
        }

        [UniVerseIndex(34)]
        [UniVerseFieldName("DOCNO")]
        public string DocumentNumber { get; set; }

        [JsonAlsoUpdates("PostDate")]
        public DateTime? PostDT { get; set; }

        [UniVerseIndex(35)]
        [UniVerseFieldName("SS.POSTDATE.INT")]
        public string PostDate
        {
            get
            {
                if (PostDT.HasValue)
                {
                    return Framework.UniVerseDateString(PostDT.Value);
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    if (Int32.TryParse(value, out int valInt))
                    {
                        PostDT = Framework.DateTimeFromUniVerseOffset(valInt);
                    }
                    else
                    {
                        PostDT = null;
                    }
                }
            }
        }

        [JsonAlsoUpdates("OnFileDate")]
        public DateTime? OnFileDT { get; set; }

        [UniVerseIndex(36)]
        [UniVerseFieldName("SS.DATEONFILE.INT")]
        public string OnFileDate
        {
            get
            {
                if (OnFileDT.HasValue)
                {
                    return Framework.UniVerseDateString(OnFileDT.Value);
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    if (Int32.TryParse(value, out int valInt))
                    {
                        OnFileDT = Framework.DateTimeFromUniVerseOffset(valInt);
                    }
                    else
                    {
                        OnFileDT = null;
                    }
                }
            }
        }

        [UniVerseIndex(37)]
        [UniVerseFieldName("SUPINVNO")]
        public string SupplierInvoiceReference { get; set; }

        [JsonAlsoUpdates("InvoiceDueDate")]
        public DateTime? InvoiceDueDT { get; set; }

        [UniVerseIndex(38)]
        [UniVerseFieldName("SS.INVDUE.INT")]
        public string InvoiceDueDate
        {
            get
            {
                if (InvoiceDueDT.HasValue)
                {
                    return Framework.UniVerseDateString(InvoiceDueDT.Value);
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    if (Int32.TryParse(value, out int valInt))
                    {
                        InvoiceDueDT = Framework.DateTimeFromUniVerseOffset(valInt);
                    }
                    else
                    {
                        InvoiceDueDT = null;
                    }
                }
            }
        }

        [UniVerseIndex(39)]
        [UniVerseFieldName("PSUPREF")]
        public string SupplierCode { get; set; }

        [UniVerseIndex(40)]
        [UniVerseFieldName("BANKNO")]
        public int? BankNumber { get; set; }

        [UniVerseIndex(41)]
        [UniVerseFieldName("BANKAC")]
        public int? BankAccountNumber { get; set; }

        [UniVerseIndex(43)]
        [UniVerseFieldName("PAMOUNT")]
        [UniVerseConversion("MR22")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> Amounts { get; set; }

        [UniVerseIndex(44)]
        [UniVerseFieldName("GLCODE")]
        [UniVerseMultiValue]
        public IEnumerable<string> GeneralLedgerCodes { get; set; }

        [UniVerseIndex(47)]
        [UniVerseFieldName("VATCODE")]
        [UniVerseMultiValue]
        public IEnumerable<string> VATCodes { get; set; }
        
        /// <summary>
        /// Default value:
        /// P("PG.PLVATTAX,D.47.49..160")[M]
        /// </summary>
        [UniVerseIndex(48)]
        [UniVerseFieldName("VATTAX")]
        [UniVerseConversion("MR22")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> VATableAmounts { get; set; }

        /// <summary>
        /// Default Value: SUM<43.n> (PAMOUNT)
        /// </summary>
        [UniVerseIndex(49)]
        [UniVerseFieldName("VATVALUE")]
        [UniVerseConversion("MR22")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> VATValues { get; set; }

        [UniVerseIndex(50)]
        [UniVerseFieldName("TOTDUE")] // "CHQTOTAL/DTOTAL/RDTOTAL/STGAMT/STGCHQTOTAL/TOTDUE"
        [UniVerseConversion("MR22")]
        public decimal TotalDueInSterling { get; set; }

        [UniVerseIndex(51)]
        [UniVerseFieldName("STGORIGVAL")]
        [UniVerseConversion("MR22")]
        public decimal SterlingOriginalValue { get; set; }

        /// <summary>
        /// Default Value: <51>-<50>
        /// </summary>
        [UniVerseIndex(52)]
        [UniVerseFieldName("AEX.DIFF")]
        [UniVerseConversion("MR22")]
        public decimal ExchangeRateDifference { get; set; }

        [UniVerseIndex(53)]
        [UniVerseFieldName("PINVDOC")] // PCRNDOC,PINVDOC,PLADJDOC,POPDOC,PPAYDOC,PPETDOC,PPOTHDOC
        /// <summary>
        /// PI{RegisterKey}
        /// </summary>
        public string DocumentReference { get; set; }

        /// <summary>
        /// Must exist on PLTERMS
        /// </summary>
        [UniVerseIndex(54)]
        [UniVerseFieldName("TERMS")]
        public string SettlementTerms { get; set; }

        [JsonAlsoUpdates("DueDates")]
        public IEnumerable<DateTime?> SettlementDueDTs { get; set; }

        [UniVerseIndex(55)]
        [UniVerseFieldName("SS.OBALDUED.INT")]
        [UniVerseMultiValue]
        public IEnumerable<string> SettlementDueDates
        {
            get
            {
                var result = new List<string>();

                if (SettlementDueDTs.Any())
                {
                    foreach (var dueDt in SettlementDueDTs)
                    {
                        if (dueDt.HasValue)
                        {
                            result.Add(Framework.UniVerseDateString(dueDt.Value));
                        }
                        else
                        {
                            result.Add(null);
                        }
                    }

                    return result;
                }
                else
                {
                    return result;
                }
            }
            set
            {
                var results = new List<DateTime?>();

                if (value.Any())
                {
                    foreach (var dueDate in value)
                    {
                        if (Int32.TryParse(dueDate, out int valInt))
                        {
                            results.Add(Framework.DateTimeFromUniVerseOffset(valInt));
                        }
                        else
                        {
                            results.Add(null);
                        }
                    }
                }

                SettlementDueDTs = results;
            }
        }

        [JsonAlsoUpdates("DueDate")]
        public DateTime? DueDT { get; set; }

        [UniVerseIndex(55, 1)]
        [UniVerseFieldName("SS.DUEDATE.INT")]
        public string DueDate
        {
            get
            {
                if (DueDT.HasValue)
                {
                    return Framework.UniVerseDateString(DueDT.Value);
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    if (Int32.TryParse(value, out int valInt))
                    {
                        DueDT = Framework.DateTimeFromUniVerseOffset(valInt);
                    }
                    else
                    {
                        DueDT = null;
                    }
                }
            }
        }

        [UniVerseIndex(56)]
        [UniVerseFieldName("MEMO")]
        public string Memo { get; set; }

        /// <summary>
        /// Default value: F("TPLTRANS",<85>)<18>
        /// </summary>
        [UniVerseIndex(57)]
        [UniVerseFieldName("DISCAMD")]
        [UniVerseConversion("MR33")]
        public decimal SettlementDiscount { get; set; }

        [UniVerseIndex(58)]
        [UniVerseFieldName("KEY2")]
        public string Key2 { get; set; }

        [UniVerseIndex(59)]
        [UniVerseFieldName("KEY3")]
        public string Key3 { get; set; }

        /// <summary>
        /// Tax + Values
        /// Default Value: SUM<48> + SUM<49>
        /// </summary>
        [UniVerseIndex(60)]
        [UniVerseFieldName("VATBAL")]
        [UniVerseConversion("MR22")]
        public decimal VATBalance { get; set; }

        /// <summary>
        /// Amount + Tax
        /// Default Value: SUM<43> + SUM<48>
        /// </summary>
        [UniVerseIndex(61)]
        [UniVerseFieldName("DOCBAL")]
        [UniVerseConversion("MR22")]
        public decimal DocumentBalance { get; set; }

        /// <summary>
        /// Amount Paid!
        /// </summary>
        [UniVerseIndex(62)]
        [UniVerseFieldName("CASHPD")]
        [UniVerseConversion("MR22")]
        public decimal? AmountPaid { get; set; }

        /// <summary>
        /// Erm... These fields are a bit confused, so I'm mapping both names in the hopes
        /// that we can sort it out someday.
        /// </summary>
        [UniVerseIndex(63)]
        [UniVerseFieldName("CUNALLOC")]
        [UniVerseConversion("MR22")]
        public decimal? CurrentlyUnallocated { get; set; }

        [UniVerseIndex(63)] // Note: this is the exact same data as the field directly above. Obviously.
        [UniVerseFieldName("DISCREC")]
        [UniVerseConversion("MR22")]
        public decimal? DiscountReceived { get; set; } // But with a completely different name.

        /// <summary>
        /// Cash on Account
        /// </summary>
        [UniVerseIndex(64)]
        [UniVerseFieldName("AONACC")]
        [UniVerseConversion("MR22")]
        public decimal? CashOnAccount { get; set; }

        /// <summary>
        /// Not Allocated
        /// </summary>
        [UniVerseIndex(64)] // Note: this is the exact same data as the field directly above. Obviously.
        [UniVerseFieldName("ONACC")]
        [UniVerseConversion("MR22")]
        public decimal? NotAllocated { get; set; } // But with a completely different name.

        /// <summary>
        /// Unallocated Value
        /// </summary>
        [UniVerseIndex(65)]
        [UniVerseFieldName("AUNALLOC")] // How in the heck this might be different from Not Allocated I don't know.
        [UniVerseConversion("MR22")] 
        public decimal? UnallocatedValue { get; set; } // Honestly.

        /// <summary>
        /// Default: <66,-1> (All of field 66)... Which is... THIS FIELD. FFS.
        /// </summary>
        [UniVerseIndex(66)]
        [UniVerseFieldName("CKEY")]
        [UniVerseMultiValue]
        public IEnumerable<string> DocumentNumbers { get; set; }

        [UniVerseIndex(69)]
        [UniVerseFieldName("DOCREFS")]
        [UniVerseMultiValue]
        public IEnumerable<string> DocumentReferences { get; set; }

        [UniVerseIndex(70)]
        [UniVerseFieldName("TRANKEY")]
        [UniVerseMultiValue]
        public IEnumerable<string> TransactionKeys { get; set; }

        [UniVerseIndex(71)]
        [UniVerseFieldName("SUPREFS")]
        [UniVerseMultiValue]
        public IEnumerable<string> SupplierReferences { get; set; }

        [UniVerseIndex(73)]
        [UniVerseFieldName("IND")]
        public string Indicator { get; set; }

        [UniVerseIndex(74)]
        [UniVerseFieldName("CALLOC")]
        [UniVerseConversion("MR22")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> Allocated { get; set; }

        /// <summary>
        /// Default Value: F("PLSUP",<39>)<3>[S][M]
        /// </summary>
        [UniVerseIndex(75)]
        [UniVerseFieldName("OPENITEM")]
        public bool? OpenItem { get; set; }

        [UniVerseIndex(77)]
        [UniVerseFieldName("KEY4")]
        public string Key4 { get; set; }

        /// <summary>
        /// Default Value SUM<43> * <160> | "100000" // does PIPE mean DIVIDE in here? Maybe
        /// </summary>
        [UniVerseIndex(80)]
        [UniVerseFieldName("DISCAMT")]
        [UniVerseConversion("MR22")]
        public decimal DiscountAmount { get; set; }

        [UniVerseIndex(81)]
        [UniVerseFieldName("BALLOC")]
        [UniVerseConversion("MR22")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> Allocations { get; set; }

        [UniVerseIndex(82)]
        [UniVerseFieldName("MEMO1")]
        [UniVerseMultiValue]
        public IEnumerable<string> OtherMemos { get; set; }

        [UniVerseIndex(83)]
        [UniVerseFieldName("UNNAMED")]
        [UniVerseMultiValue]
        public IEnumerable<string> OtherPrepaymentMemos { get; set; }

        /// <summary>
        /// Must exist on TPLTRANS
        /// </summary>
        [UniVerseIndex(85)]
        [UniVerseFieldName("DISPKEY")]
        public string DisputeDocumentNumber { get; set; }

        /// <summary>
        /// Default Value: F("TPLTRANS",<85>)<12>[S][M]
        /// </summary>
        [UniVerseIndex(86)]
        [UniVerseFieldName("DISPCUS")]
        public string DisputeCustomerReference { get; set; }

        /// <summary>
        /// Default Value: F("TPLTRANS",<85>)<16>
        /// </summary>
        [UniVerseIndex(87)]
        [UniVerseFieldName("DISPIND")]
        public string DisputeIndicator { get; set; }

        /// <summary>
        /// Default Value: P("DMEMO") == If and only if 87 == D, read 88. If 87 != D, read "".
        /// </summary>
        [UniVerseIndex(88)]
        [UniVerseFieldName("DISPMEM")]
        public string DisputeMemo { get; set; }

        /// <summary>
        /// Default Value: "Y"
        /// </summary>
        [UniVerseIndex(89)]
        [UniVerseFieldName("CLEAROK")]
        public bool? OkayToClear { get; set; }

        [UniVerseIndex(90)]
        [UniVerseFieldName("RELBY")]
        public string ReleasedBy { get; set; }

        [UniVerseIndex(92)]
        [UniVerseFieldName("AMEMO")]
        public string Details { get; set; }

        [UniVerseIndex(93)]
        [UniVerseFieldName("TINVAMT")]
        [UniVerseConversion("MR22")]
        public decimal? TransferAmount { get; set; }

        [UniVerseIndex(94)]
        [UniVerseFieldName("GOODS")]
        [UniVerseConversion("MR22")]
        public decimal? GoodsValue { get; set; }

        [UniVerseIndex(95)]
        [UniVerseFieldName("VATAMT")]
        [UniVerseConversion("MR22")]
        public decimal? VATAmount { get; set; }

        /// <summary>
        /// Default Value: <94>+<95>[S][M]
        /// </summary>
        [UniVerseIndex(96)]
        [UniVerseFieldName("TOTAMT")]
        [UniVerseConversion("MR22")]
        public decimal? TotalValue { get; set; }

        /// <summary>
        /// Opening Balance
        /// Default Value: F("PLSUP",<39>)<14>[S][M]
        /// </summary>
        [UniVerseIndex(98)]
        [UniVerseFieldName("SETOPEN")]
        public bool? SetOpen { get; set; }

        [JsonAlsoUpdates("OpeningDates")]
        public IEnumerable<DateTime?> OpeningDTs { get; set; }

        [UniVerseIndex(99)]
        [UniVerseFieldName("SS.OPDATE.INT")]
        [UniVerseMultiValue]
        public IEnumerable<string> OpeningDates
        {
            get
            {
                var result = new List<string>();

                if (OpeningDTs.Any())
                {
                    foreach (var openDt in OpeningDTs)
                    {
                        if (openDt.HasValue)
                        {
                            result.Add(Framework.UniVerseDateString(openDt.Value));
                        }
                        else
                        {
                            result.Add(null);
                        }
                    }

                    return result;
                }
                else
                {
                    return result;
                }
            }
            set
            {
                var results = new List<DateTime?>();

                if (value.Any())
                {
                    foreach (var openDt in value)
                    {
                        if (Int32.TryParse(openDt, out int valInt))
                        {
                            results.Add(Framework.DateTimeFromUniVerseOffset(valInt));
                        }
                        else
                        {
                            results.Add(null);
                        }
                    }
                }

                OpeningDTs = results;
            }
        }

        [UniVerseIndex(100)]
        [UniVerseFieldName("OP.GOODS")]
        [UniVerseConversion("MR22")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> OpeningGoodsValue { get; set; }

        [UniVerseIndex(101)]
        [UniVerseFieldName("OP.VATAMT")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> OpeningVATAmounts { get; set; }

        /// <summary>
        /// Default Value: (OPGOODS + OPVATAMT)
        /// </summary>
        [UniVerseIndex(102)]
        [UniVerseFieldName("OP.BAL")]
        [UniVerseConversion("MR22")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> OpeningBalancesOutstanding { get; set; }

        [UniVerseIndex(103)]
        [UniVerseFieldName("OPITEM")]
        [UniVerseMultiValue]
        public IEnumerable<string> OpeningItemReferences { get; set; }

        /// <summary>
        /// Default Value: P("PG.DEF.OPBAL.AGE")[M]
        /// </summary>
        [UniVerseIndex(104)]
        [UniVerseFieldName("OPAGE")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> OpeningAges { get; set; }

        /// <summary>
        /// Default Value: P("DUNALLOC")[S][M]
        /// </summary>
        [UniVerseIndex(106)]
        [UniVerseFieldName("TUNALLOC")]
        public bool? IsUnallocatedValueZero { get; set; }

        /// <summary>
        /// Default Value: P("DONACC")[S][M]
        /// </summary>
        [UniVerseIndex(107)]
        [UniVerseFieldName("TONACC")]
        public bool? IsOnAccountZero { get; set; }

        /// <summary>
        /// Default Value: P("DCHCURR")[S][M]
        /// </summary>
        [UniVerseIndex(108)]
        [UniVerseFieldName("TCURR")]
        public bool? IsThisPeriodZero { get; set; }

        /// <summary>
        /// Default Value: P("DCHVAL")[S][M]
        /// </summary>
        [UniVerseIndex(109)]
        [UniVerseFieldName("CHVALID")]
        public bool? TransferValid { get; set; }

        /// <summary>
        /// Default Value: P("DCHCONF")
        /// </summary>
        [UniVerseIndex(110)]
        [UniVerseFieldName("CHCONF")]
        public bool? ConfirmChange { get; set; }

        [UniVerseIndex(112)]
        [UniVerseFieldName("ALLOCCASH")]
        public bool? AllocateCash { get; set; }

        [UniVerseIndex(113)]
        [UniVerseFieldName("TOTALLOC")]
        [UniVerseConversion("MR22")]
        public decimal? TotalAllocated { get; set; }

        [JsonAlsoUpdates("ReceivedDate")]
        public DateTime? ReceivedDT { get; set; }

        [UniVerseIndex(114)]
        [UniVerseFieldName("SS.RDATE.INT")]
        public string ReceivedDate
        {
            get
            {
                if (ReceivedDT.HasValue)
                {
                    return Framework.UniVerseDateString(ReceivedDT.Value);
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    if (Int32.TryParse(value, out int valInt))
                    {
                        ReceivedDT = Framework.DateTimeFromUniVerseOffset(valInt);
                    }
                    else
                    {
                        ReceivedDT = null;
                    }
                }
            }
        }

        [UniVerseIndex(115)]
        [UniVerseFieldName("PTO")]
        public string PassedTo { get; set; }

        [JsonAlsoUpdates("PassedToDate")]
        public DateTime? PassedToDT { get; set; }

        [UniVerseIndex(116)]
        [UniVerseFieldName("SS.PDATE.INT")]
        public string PassedToDate
        {
            get
            {
                if (PassedToDT.HasValue)
                {
                    return Framework.UniVerseDateString(PassedToDT.Value);
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    if (Int32.TryParse(value, out int valInt))
                    {
                        PassedToDT = Framework.DateTimeFromUniVerseOffset(valInt);
                    }
                    else
                    {
                        PassedToDT = null;
                    }
                }
            }
        }

        /// <summary>
        /// Default Value: P("DTEMPMESS")[S][M]
        /// </summary>
        [UniVerseIndex(120)]
        [UniVerseFieldName("TEMPMESS")]
        public string TemporaryMessage { get; set; }

        [UniVerseIndex(120)]
        [UniVerseFieldName("ZA120")]
        public string Zaplcashall120 { get; set; }

        [UniVerseIndex(121)]
        [UniVerseFieldName("ZA121")]
        public string Zaplcashall121 { get; set; }

        [UniVerseIndex(122)]
        [UniVerseFieldName("ZA122")]
        public string Zaplcashall122 { get; set; }

        [UniVerseIndex(123)]
        [UniVerseFieldName("RF134")]
        public string RFPLUFO01 { get; set; }

        [UniVerseIndex(124)]
        [UniVerseFieldName("ZA124")]
        public string Zaplcashall124 { get; set; }

        /// <summary>
        /// Default Value: B("PL.B.DEFGLCODE")
        /// </summary>
        [UniVerseIndex(132)]
        [UniVerseFieldName("GLCODE9")]
        [UniVerseMultiValue]
        public IEnumerable<string> GeneralLedgerCode9s { get; set; }

        [UniVerseIndex(132,1)]
        [UniVerseFieldName("GLCODE9")]
        public string GeneralLedgerCode9 { get; set; }

        [UniVerseIndex(133)]
        [UniVerseFieldName("GLSUB")]
        [UniVerseMultiValue]
        public IEnumerable<string> GeneralLedgerSubCodes { get; set; }

        [UniVerseIndex(133,1)]
        [UniVerseFieldName("GLSUB")]
        public string GeneralLedgerSubCode { get; set; }

        [UniVerseIndex(134)]
        [UniVerseFieldName("RREGREF")]
        [UniVerseMultiValue]
        public IEnumerable<string> RegisterReferences { get; set; }

        /// <summary>
        /// Default Value: F("PLSUP",<39>)<1,1>[S][M]
        /// </summary>
        [UniVerseIndex(135)]
        [UniVerseFieldName("PNAME")]
        [UniVerseMultiValue]
        public IEnumerable<string> RegisterEntryTypes { get; set; }

        [UniVerseIndex(148)]
        [UniVerseFieldName("TOTCSHALL")]
        [UniVerseConversion("MR22")]
        public decimal? TotalCashAllocated { get; set; }

        [UniVerseIndex(151)]
        [UniVerseFieldName("ZA151")]
        public decimal ZaplcashallRF { get; set; }

        [UniVerseIndex(152)]
        [UniVerseFieldName("ALLOCCHEQUE")]
        [UniVerseConversion("MR22")]
        public decimal? ChequeValue { get; set; }

        [UniVerseIndex(153)]
        [UniVerseFieldName("CHQCALLOC")]
        [UniVerseConversion("MR22")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> ChequeAllocations { get; set; }

        /// <summary>
        /// This is used in determining which period is set on the ledger: current period or next period?
        /// </summary>
        [UniVerseIndex(154)]
        [UniVerseFieldName("GLPOST")]
        public bool? Future { get; set; }

        /// <summary>
        /// Default Value: (<152> * "1000" / <162>)[S][M]
        /// </summary>
        [UniVerseIndex(155)]
        [UniVerseFieldName("ALLOCCHQ.STG")]
        [UniVerseConversion("MR22")]
        public decimal? SterlingChequeValue { get; set; }

        [UniVerseIndex(157)]
        [UniVerseFieldName("DISCAMD.CUR")]
        [UniVerseConversion("MR33")]
        public decimal? SettlementDiscountAmountInCurrency { get; set; }

        /// <summary>
        /// Don't ask me.
        /// </summary>
        [UniVerseIndex(160)]
        [UniVerseFieldName("DISCPERC")]
        [UniVerseConversion("MR33")]
        public decimal? DiscountPercentage_Again { get; set; }

        [UniVerseIndex(161)]
        [UniVerseFieldName("CURRENCY")]
        public string CurrencyCode { get; set; }

        [UniVerseIndex(162)]
        [UniVerseFieldName("AXRATE")]
        [UniVerseConversion("MR66")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> ExchangeRates { get; set; }

        [UniVerseIndex(164)]
        [UniVerseFieldName("EXDIFFS")]
        [UniVerseConversion("MR22")]
        public decimal? ExchangeRateDifferences { get; set; }

        [UniVerseIndex(165)]
        [UniVerseFieldName("FOREIGN")]
        public bool Foreign { get; set; }

        [UniVerseIndex(180)]
        [UniVerseFieldName("DISCADIS.CUR")]
        [UniVerseConversion("MR22")]
        public decimal? DiscountAmountInCurrency { get; set; }

        [UniVerseIndex(181)]
        [UniVerseFieldName("AUTHORIZER")]
        public string Authoriser { get; set; } // MUST EXIST ON "POAUTHORIZER" FILE

        [UniVerseIndex(182)]
        [UniVerseFieldName("DNMESSAGE")]
        [UniVerseMultiValue]
        public IEnumerable<string> DNMessages { get; set; }

        [UniVerseIndex(183)]
        [UniVerseFieldName("DNDESC")]
        [UniVerseMultiValue]
        public IEnumerable<string> DNDescriptions { get; set; }

        [UniVerseIndex(184)]
        [UniVerseFieldName("DNQTY")]
        [UniVerseConversion("MR11")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> DNQuantities { get; set; }

        [UniVerseIndex(185)]
        [UniVerseFieldName("DNPRICE")]
        [UniVerseConversion("MR4")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> DNPrices { get; set; }

        /// <summary>
        /// Default Value: DNQTY * DNPRICE / 100 | "10"
        /// </summary>
        [UniVerseIndex(186)]
        [UniVerseFieldName("DNLVALUE")]
        [UniVerseConversion("MR22")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> DNValues { get; set; }

        /// <summary>
        /// Default Value: F("PLSUP",PSUPREF)<58>
        /// </summary>
        [UniVerseIndex(187)]
        [UniVerseFieldName("DNVATCODE")]
        [UniVerseMultiValue]
        public IEnumerable<string> DNVATCodes { get; set; }

        [UniVerseIndex(188)]
        [UniVerseFieldName("DNVATVAL")]
        [UniVerseConversion("MR22")]
        public decimal? DNVATValue { get; set; }

        [UniVerseIndex(199)]
        [UniVerseFieldName("PLINVREG")]
        public string PurchaseLedgerRegisterKey { get; set; }

        [UniVerseIndex(200)]
        [UniVerseFieldName("OPGOODS.CUR")]
        [UniVerseConversion("MR22")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> OpeningGoodsValuesInCurrency { get; set; }

        [UniVerseIndex(201)]
        [UniVerseFieldName("OPVATAMT.CUR")]
        [UniVerseConversion("MR22")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> OpeningVATAmountsInCurrency { get; set; }
        
        /// <summary>
        /// Default Value: OPGOODS.CUR + OPVATAMT.CUR
        /// </summary>
        [UniVerseIndex(202)]
        [UniVerseFieldName("OPBAL.CUR")]
        [UniVerseConversion("MR22")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> OpeningBalancesOutstandingInCurrency { get; set; }

        [UniVerseIndex(234)]
        [UniVerseFieldName("EXDIFFS.CUR")]
        [UniVerseConversion("MR22")]
        public decimal ExchangeRateDifferencesInCurrency { get; set; }

        [UniVerseIndex(241)]
        [UniVerseFieldName("DISCREC.CUR")]
        [UniVerseConversion("MR22")]
        public decimal? DiscountAllowedInCurrency { get; set; }

        [UniVerseIndex(243)]
        [UniVerseFieldName("PAMOUNT.CUR")]
        [UniVerseConversion("MR22")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> AmountsPaidInCurrency { get; set; }

        [UniVerseIndex(248)]
        [UniVerseFieldName("VATTAX.CUR")]
        [UniVerseConversion("MR22")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> VATTaxesInCurrency { get; set; }

        [UniVerseIndex(250)]
        [UniVerseFieldName("CURAMT")]
        // [UniVerseConversion("MR22")] // ERGH...JUST USE MR22!!! = NO DON'T
        // There are no XRATE records where <4> (NO.DP) = "Y", so... BLEEEHHH. this is horrid!
        // (IF(F("XRATE",CURRENCY)<4> = "Y",
        // OCONV(OCONV(@VALUE,"MR02,"),"MR#":F("DICT TPLEDG",".CURAMT")<10>),
        // OCONV(OCONV(@VALUE,"MR22"),"MR#":F("DICT TPLEDG",".CURAMT")<10>))) <<< This branch wins, but... what does it result in? DUNNO!
        // Looking at the data, this is stored as an integer with no conversion.
        public decimal AmountInCurrency { get; set; }

        [UniVerseIndex(264)]
        [UniVerseFieldName("CNBAL.CUR")]
        [UniVerseConversion("MR22")]
        public decimal? UnallocatedCreditNoteAmountInCurrency { get; set; }

        [UniVerseIndex(265)]
        [UniVerseFieldName("UNALLOC.CUR")]
        [UniVerseConversion("MR22")]
        public decimal UnallocatedAmountInCurrency { get; set; }

        [UniVerseIndex(267)]
        [UniVerseFieldName("TRANTYPE")]
        [UniVerseMultiValue]
        public IEnumerable<string> TransactionTypes { get; set; }

        [UniVerseIndex(268)]
        [UniVerseFieldName("SUPINVKEY")]
        [UniVerseMultiValue]
        public IEnumerable<string> SupplierInvoiceKeys { get; set; }

        [UniVerseIndex(269)]
        [UniVerseFieldName("DISPUTE.MV")]
        [UniVerseMultiValue]
        public IEnumerable<string> Disputations { get; set; }

        [JsonAlsoUpdates("SettlementDates")]
        public IEnumerable<DateTime?> SettlementDTs { get; set; }

        /// <summary>
        /// Default Value: B("RFPLCALCDUE,S")
        /// </summary>
        [UniVerseIndex(270)]
        [UniVerseFieldName("SS.DC.DUEDATE.INT")]
        [UniVerseMultiValue]
        public IEnumerable<string> SettlementDates
        {
            get
            {
                var result = new List<string>();

                if (SettlementDTs.Any())
                {
                    foreach (var settlementDt in SettlementDTs)
                    {
                        if (settlementDt.HasValue)
                        {
                            result.Add(Framework.UniVerseDateString(settlementDt.Value));
                        }
                        else
                        {
                            result.Add(null);
                        }
                    }

                    return result;
                }
                else
                {
                    return result;
                }
            }
            set
            {
                var results = new List<DateTime?>();

                if (value.Any())
                {
                    foreach (var settlementDt in value)
                    {
                        if (Int32.TryParse(settlementDt, out int valInt))
                        {
                            results.Add(Framework.DateTimeFromUniVerseOffset(valInt));
                        }
                        else
                        {
                            results.Add(null);
                        }
                    }
                }

                SettlementDTs = results;
            }
        }

        /// <summary>
        /// Default Value: (F("PLTERMS",<54>)<1> * "1")
        /// </summary>
        [UniVerseIndex(271)]
        [UniVerseFieldName("DC.DISCPERC")]
        [UniVerseConversion("MR33")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> DiscountPercentages { get; set; }

        /// <summary>
        /// Default Value: F("TPLTRANS",<85>)<18>
        /// </summary>
        [UniVerseIndex(272)]
        [UniVerseFieldName("DC.DISCAMD")]
        [UniVerseConversion("MR22")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> SettlementDiscountAmounts { get; set; }

        [UniVerseIndex(273)]
        [UniVerseFieldName("DC.DISCAMD.CUR")]
        [UniVerseConversion("MR22")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> SettlementDiscountAmountsInCurrency { get; set; }

        // Do try to avoid mapping all the useless sh!te fields, won't you...

        [UniVerseIndex(370)]
        [UniVerseFieldName("PP.START.PD.DES")]
        [UniVerseMultiValue]
        public IEnumerable<byte> PrepaymentStartPeriods { get; set; }

        [UniVerseIndex(371)]
        [UniVerseFieldName("PP.NO.PDS.DES")]
        [UniVerseMultiValue]
        public IEnumerable<byte> PrepaymentNumberOfPeriods { get; set; }

        [UniVerseIndex(372)]
        [UniVerseFieldName("PP.SPLIT.EQUAL.DES")]
        [UniVerseMultiValue]
        public IEnumerable<bool> PrepaymentsSplitEqually { get; set; }

        [UniVerseIndex(373)]
        [UniVerseFieldName("PP.SPLIT.AMOUNT.DES")]
        [UniVerseConversion("MR22")]
        [UniVerseMultiValue]
        public IEnumerable<string> PrepaymentSplitAmounts { get; set; }

        [UniVerseIndex(374)]
        [UniVerseFieldName("PP.SPLIT.MEMO.DES")]
        [UniVerseMultiValue]
        public IEnumerable<string> PrepaymentSplitMemos { get; set; }

        [UniVerseIndex(375)]
        [UniVerseFieldName("PP.MEMO.DES")]
        [UniVerseMultiValue]
        public IEnumerable<string> PrepaymentMemos { get; set; }

        [UniVerseIndex(376)]
        [UniVerseFieldName("PP.EXP.GLCODE.DES")]
        [UniVerseMultiValue]
        public IEnumerable<int> PrepaymentGeneralLedgerExpenseCodes { get; set; }
    }
}

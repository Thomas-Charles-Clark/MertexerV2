using 	System;
using 	System.Collections.Generic;
using 	System.Linq;
using 	System.Text;
using 	System.Threading.Tasks;
using 	System.Web.UI.WebControls.WebParts;
using 	System.Web.UI.WebControls;
using 	IBMU2.UODOTNET;
using 	MertexerV2.Models.Product;

using 	Newtonsoft.Json.Linq;

using 	static System.Net.Mime.MediaTypeNames;
using MertexerV2.Utilities;
using MertexerV2.Models.PurchaseLedger;

namespace 	MertexerV2.PurchaseLedgerFunctions
{
    public static partial class PurchaseLedgerFunctions
    {
        /* 	
        0001 	SUBROUTINE PLAMOUNT                                                        
        0002 	     $INCLUDE RFPROGS COMMON                                               
        0003 	EQUATE AM TO CHAR(254), VM TO CHAR(253)
        0004 	* IF RECORD<154> = '' THEN CURRENT PERIOD POSTING                          
        0005 	* IF NXT PERIOD AND GL IN LAST PERIOD DO NOT UPDATE YTD FIGURES
        0006 	* U:PLAMOUNT,N62     U:PLAMOUNT,Y62     U:PLAMOUNT,43.A     U:PLAMOUNT, N43.A
        0007 	* U:PLAMOUNT,48.X
        0008 	* N= NXT  Y= YTD FIGURE   62=RECORD NO  43.A = ADD SUM OF ALL MULTI-VALUES
        0009 	*                                       43.X = ADD ONLY CNT MULTI-VALUE
        0010 	ENTRY = PARAM                                                              
        0011 	RTN.FLAG= 0                                                                 
        0012 	VALUE= ''                                                                   
        0013 	NXT= 0                                                                      
        0014 	IF ENTRY[1, 1]= "N" THEN NXT = 1                                               
        0015 	YEAR= 0                                                                     
        0016 	IF ENTRY[1, 1]= "Y" THEN YEAR = 1                                              
        0017 	MULTI= COUNT(ENTRY, ".")                                                     
        0018 	NO= ENTRY                                                                   
        0019 	IF NXT = 1 OR YEAR = 1 THEN
        0020 	   L= LEN(ENTRY)                                                            
        0021 	   NO= NO[2, L - 1]                     
        0022 	   END
        0023 	IF MULTI = 1 THEN TYPE = FIELD(NO, ".", 2) * TYPE = A or X                        
        0024 	IF MULTI = 1 THEN NO = FIELD(NO, ".", 1) * NO = Field Number
        0025 	IF RECORD<154> = '' THEN    * IF FUTURE = '' AND NEXT = 0 THEN JUMP TO CALCULATING                        
        0026 	   IF NXT = 0 THEN GOSUB 200                                                 
        0027 	   RETURN
        0028 	   END
        0029 	IF NXT = 1 THEN GOSUB 200; RETURN   * IF NEXT THEN JUMP TO CALCULATING                             
        0030 	IF YEAR = 0 THEN RETURN                                                      
        0031 	*-                                                                         
        0032 	* Use PL period if separate PL or GL period if GL/PL combined              
        0033 	*-                                                                         
        0034 	PER=OCONV('1','TCOMPANY;X;;134')
        0035 	IF PER = "" THEN                                                           
        0036 	   PER=OCONV('1','TCOMPANY;X;;10')
        0037 	END                                                                        
        0038 	*-                                                                         
        0039 	NOPER=OCONV('1','TCOMPANY;X;;11')
        0040 	IF PER = NOPER THEN RETURN                                                   
        0041 	200*  ;* WORK OUT VALUE                                                    
        0042 	     BEGIN CASE                                                            
        0043 	      CASE MULTI = 0                                                       
        0044 	        VALUE = RECORD<NO>                                   
        0045 	      CASE MULTI = 1 AND TYPE = "A"                                        
        0046 	        MAX = DCOUNT(RECORD<NO>, VM)
        0047 	        FOR I = 1 TO MAX                                                   
        0048 	            VALUE = VALUE + RECORD<NO, I>                                   
        0049 	        NEXT I                                                             
        0050 	      CASE MULTI = 1 AND TYPE = "X"                                        
        0051 	        VALUE = VALUE + RECORD<NO, CNT>                                     
        0052 	     END CASE                                                              
        0053 	     RETURN                                                                
        0054 	END
                */
        /// <summary>
        /// Mimics PLPROGS PLAMOUNT
        /// </summary>
        /// <param name="session">
        ///     If to be used within a transaction, and we need to read any files or perform any operations in UniVerse,
        ///     this session should be used.
        /// </param>
        /// <param name="fieldNumber">
        ///     The field number we are concerned with as source for... data... which is already being passed by incomingValue, is it not?
        /// </param>
        /// <param name="incomingValue">
        ///     The value passed in for consideration
        /// </param>
        /// <param name="currentPeriod">
        ///     the current period (determined from COMPANY file <134> "PLPER" == current P/L period (if using Purchase Ledger periods separate from General Ledger periods) 
        ///     or <10> "GLPER" == current G/L period, to be used if PLPER is NULL.
        /// </param>
        /// <param name="numberOfPeriods">
        ///     the number of financial periods in use: COMPANY <11> "GLPRDS"
        /// </param>
        /// <param name="operationPeriod">
        ///     The period to consider: either Next, Year, or null.
        /// </param>
        /// <param name="future">
        ///     Whether or not the period is in the future.
        /// </param>
        /// <param name="mvOperationType">
        ///     If dealing with a multivalued field, either A or X: sum or count, respectively.
        /// </param>
        /// <returns>
        ///     Decimal value representing the updated purchase ledger field value
        /// </returns>
        internal static decimal CalculatePurchaseLedgerAmount(IEnumerable<decimal> amounts, byte currentPeriod, byte numberOfPeriods, string operationPeriod = null, bool future = false, string mvOperationType = null)
        {
            // 0012: VALUE = ''
            var result = 0.0m;

            // 0014: IF ENTRY[1, 1]= "N" THEN NXT = 1
            var next = operationPeriod == "N";

            // 0016: IF ENTRY[1, 1]= "Y" THEN YEAR = 1
            var year = operationPeriod == "Y";

            // 0017: MULTI= COUNT(ENTRY, ".")    
            var multi = !String.IsNullOrEmpty(mvOperationType);

            // 0018 through 0022 are moot; we already have to hand what they devise.
            // Essentially they perform string operations to identify the field number.

            // 0023 and 0024 are also moot: we have the type of mv operation being passed in, as well as the field number.

            // 0025 - 0028: IF RECORD<154> = '' THEN IF NXT = 0 THEN GOSUB 200
            if (future == false && operationPeriod != "N")
            {
                // GOSUB 200
                result = PerformPurchaseLedgerCalculation(amounts, multi, mvOperationType);

                return result;
            }

            if (operationPeriod == "N")
            {
                // GOSUB 200
                result = PerformPurchaseLedgerCalculation(amounts, multi, mvOperationType);
                
                return result;
            }

            if (year == false)
            {
                return result; // which will be zero at this point
            }

            // 0040: IF PER = NOPER THEN RETURN   
            if (currentPeriod == numberOfPeriods)
            {
                return result; // which will be zero at this point
            }



            return result;
        }

        // Call this one sub: 200
        private static decimal PerformPurchaseLedgerCalculation(IEnumerable<decimal> amounts, bool mv, string mvOperationType)
        {
            if (mv == false)
            {
                return amounts.First();
            }
            else
            {
                if (mvOperationType == "A")
                {
                    return amounts.Select(s => Convert.ToDecimal(s)).Sum(); // sum all MV values
                }
                else if (mvOperationType == "X")
                {
                    return amounts.First(); // Return only most the recent MV entry; this MV field is cumulative.
                }
            }

            return 0;
        }
    }
}

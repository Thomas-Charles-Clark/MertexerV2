using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;

using MertexerV2.CompanyFileFunctions;
using MertexerV2.GeneralFunctions;
using MertexerV2.SupplierFunctions;
using MertexerV2.GeneralLedgerFunctions;
using MertexerV2.Models.Company;
using MertexerV2.Models.GeneralLedger;
using MertexerV2.Models.PurchaseLedger;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.UI.WebControls;
using MertexerV2.Models.General;

/*
 * /SD TPLEDG TPINVIR.CUR.DES1  >  Requires of the user an Invoice Register key
 * 
 * Process after screen accept: TPLEDG*TPINVIR.CUR1
 * This is a Paragraph, stored in the PL subsystem. It is brief. It does this:
 *  
 *      @VALUE = PLINVREG ; CALL PINVREG,3 ; IF @RTN.FLAG THEN EXIT 1
        CALL PINVREG,3
        @RECORD<2> = @OTHER(4)
        @RECORD<3> = @OTHER(5)

PINVREG is a UniBasic subroutine stored in PLPROGS. Above, is provided the number 3 as a parameter.
It is called multiple times subsequently during this transaction process, with various parameters.
                                                                   
001     SUBROUTINE PINVREG       
*****************************************      
*
* Comments section [...]
*
*****************************************      
012      $INCLUDE RFPROGS COMMON                                                
013      ENTRY = PARAM                                                            
014      BEGIN CASE                                      


015      CASE ENTRY = 1     ;*VERIFY AND READ IN                                
016           GOSUB 30    ;* OPEN PLINVREG                                      
017           IF RTN.FLAG THEN GOTO 999                                         
018           ITEM.READ = 0                                                     
019           OPEN "","PLBATCHDATA" TO F.PLBATCHDATA ELSE                       
020                CALL SB.DISP(3,'PLBATCHDATA [E1]')
021                RTN.FLAG = 1; GOTO 999                                       
022           END                                                               
023           IF KEY NE '' THEN                                                 
024              READ RECORD FROM F.PLBATCHDATA, KEY THEN                       
025                   PARMS(2) = ACTION                                         
026                   ITEM.READ = 1                                             
027              END                                                            
028           END                                                               
029           IF NOT(ITEM.READ) THEN                                            
030              READ RECORD FROM PLINV, VALUE ELSE                             
031                   CALL SB.DISP(3,VALUE:' Invoice Register not found')       
032                   RTN.FLAG = 1                                              
033                   GOTO 999                                                  
034              END                                                            
035           END                                                               
036         * V1.2+                                                             
037         * IF RECORD<1> = "Y" THEN GOSUB 20 ; GOTO 999                         
038           IF RECORD<1> = "Y" AND ACTION = '1' THEN GOSUB 20 ; GOTO 999        
039           IF ACTION = '1' THEN                                              
040              RECORD<34> ='';RECORD<53> =''                                    
041           END                                               
042         * V1.2-                                                             
043         * V1.1+                                                             
044           ACTION = 1                                                        
045         * V1.1-                                                             
046           IF RECORD<199> = "" THEN PARMS(2) = ACTION;* Save Genuine Action..                                                               
047           ACTION = 2                                                        
048         * RECORD<34> ='';RECORD<53> ='' ;* V1.2           


049      CASE ENTRY = 2                                                           
050           GOSUB 30                                         * Loads PLINVREG, so that it can be read using TPLEDG.PurchaseLedgerRegisterKey by the next line...
051           READU REC FROM PLINV,RECORD<199> ELSE GOSUB 20;RELEASE PLINV;RETURN                                                                           
052         * V1.2+                                                             
053         * IF REC<1> NE '' THEN GOSUB 20; RELEASE PLINV;RETURN               
054         * Only Validate if in INSERT Mode (as stored in PARMS(2))...        
055                                                                     
056           IF PARMS(2) = '1' THEN                                            
057              IF REC<1> NE '' THEN GOSUB 20; RELEASE PLINV;RETURN            
058           END                                                               
059         * V1.2-                                                             
060           VALUE = RECORD<199>                                                 
061           WRITEV "Y" ON PLINV,RECORD<199>,1                 * If we reach here, we're updating PLINVREG [TPLEDG<199>]<1> to "Y" ✔
062           NEWITEM = 1                                                   
063        *; CHECK THAT INV REGISTER STILL OK TO PROCEED OR IF DELETE BATCH TRANSACTION  // Eh?


064      CASE ENTRY = 3                                          * <199> on TPLEDG is "PurchaseLedgerRegisterKey", the key from the incoming PINVREG record.   
065           IF ACTION = 1 OR ACTION = '' THEN                * This is interesting: OCONV File Translation: https://docs.rocketsoftware.com/bundle/unidataunibasiccommands_rg_824/page/zyo1685024687160.html
066              OK = OCONV(RECORD<199>,'TPLINVREG;X;;0')        * OCONV(RecordID, $"T{DictionaryName}{FileName};c;n;o
067              IF OK NE RECORD<199> THEN GOSUB 20;  RETURN   * c = V – If the record does not exist, or if the attribute is an empty string, return an empty string and print an error message.
                                                               *     C – If the record does not exist, or if the attribute is an empty string, substitute id.expr for the value of the function.
                                                               *     X – If the record does not exist, or if the attribute is an empty string, return an empty string.
068              OK = OCONV(RECORD<199>,'TPLINVREG;X;;1')        * n = multivalue selector (if omitted and field requested is MV, all SVs are returned)    
069              IF OK = 'Y' THEN GOSUB 20  ; RETURN           * o = field to output                
070              END                                           * 
                                                               * So it's reading TPLEDG<199> and if that doesn't match RECORD<199> then all bets are off: GOSUB 20,
                                                               * which displays 'Invoice Register Number already Allocated'.
                                                               * 
                                                               * It then checks PLINVREG<1> which is "POSTED": empty if not posted, holds 'Y' if posted.
                                                               * 
                                                               * Essentially this case statement determines "Does this PurchaseLedgerRegisterKey exist on PLINVREG and is it as-yet not posted?
                                                               * 
                                                               * ✔
071      CASE ENTRY = 4                                                         
072           IF ACTION = 3 THEN     ;* DELETE FRONM BATCH                      
073              GOSUB 30                                                       
074              WRITEV "" ON PLINV,RECORD<199>,1                               
075           END                                                            
076      	  CALL PIUNIQUE                                                     
077      END CASE                                                               
078      RTN.FLAG = 0                                            * If we've reached this RETURN statement then we proceed unhindered (via some unknown mechanism) to the subsequent screen: [TPLEDG].[TPINVIR.CUR.DES2]               
079 999 *   
080      RETURN                                                * If we've jumped to this RETURN statement then we've been ousted with some non-zero return flag (error!)
081 *------------------------------------------                                                            
082 20*                                                                         
083         CALL SB.DISP(3,'Invoice Register Number already Allocated')         
084         RTN.FLAG = 1                                                        
085         RETURN                                                              
086 *-------------------------------------------                                
087 30*                                                                         
088      OPEN '',"PLINVREG" TO PLINV ELSE                                       
089        CALL SB.DISP(3,'PLINVREG FILE NOT FOUND')                            
090        RTN.FLAG = 1                                                         
091        END                                                                  
092      RETURN                                                                 
093     END                  


 * /SD TPLEDG TPINVIR.CUR.DES2  >  Requires the user to review the Invoice Register entry having provided a valid, non-posted key to Screen 1
 * 
 * Process after screen accept: TPLEDG*TPINVIR.CUR2.DES,INV
 * 
 * This is a much longer paragraph: it performs validation.
 * It checks for Prepayment GL codes, and if any exist it ensures that the "Prepayment Total matches the Prepayment Figure"
 * It checks for duplicate supplier document reference by reading PLSUPINV given <39>*<37> and if it finds a match it throws
 * words to the effect of "PLEASE NOTE - THIS ITEM REFERENCE HAS ALREADY BEEN USED FOR TRANSACTION L.DOCREF ON L.DOCDATE"
 * Allows user to change reference or accept reference...
 * 
 * If the routine gets past that, it then executes the following in order:
 * 
 * @VALUE = INVDUE ; EXEC INVDUE
 *  INVDUE is a paragraph existing in the PL subsystem, it's fairly simple:                                                 Catered for by validation step?
 *      ³ IF @VALUE < <33> THEN                                 * If INVDUE less than DOCDATE then throw                    ✔
        ³    ERROR 'INVOICE DUE DATE IS BEFORE DOCUMENT DATE'
        ³    EXIT 1
        ³ END
        ³ IF <55> < <33> THEN                                   * If OBALDUED less than DOCDATE then throw                  ✔
        ³    ERROR 'SETTLEMENT DATE IS BEFORE DOCUMENT DATE'
        ³    EXIT 1
        ³ END
 * 
 * @VALUE = PLINVREG ; CALL PLINVREG,2
 *  Ensures that <199> can be read from PLINVREG, and if so it updates PLINVREG<1> WITH @ID = <199> to be "Y".              ✔
 * 
 * 
 * @VALUE = PIDOCDATE ; CALL PLCHKDATE
 *  Yet more validation - this time around permittable posting dates / periods.
 *  
 *       PLCHKDATE                                                                  
0001     SUBROUTINE PLCHKDATE                                                   
0002 *   Generalised date validation logic for purchase ledger to handle future
0003 *   posting if purchase ledger is in advance of general ledger          
0004 *   ENTRY POINTS:-                                                         
0005 *   AMENDMENT HISTORY :-                                                   
0006 *   DATE      BY    REASON                                                 
0007 *   09/04/02  PT    JOB 10OF NEXT P/L 575 - ALLOW POSTING DATE UP TO END PERIOD                                                                       
0008 *                   EVEN WHEN P/L IS 1 PERIOD AHEAD OF G/L                 
0009 *                                                                          
0010     $INCLUDE RFPROGS COMMON                                                
0011 *                                                                          
0012     EQUATE VM TO CHAR(253)                                                 
0013     EQUATE UPDATE.REC TO PARMS(2)                                          
0014     RTN.FLAG = 1                                                             
0015     IF VALUE='' THEN RETURN                                                
0016     IF VALUE > DATE() THEN                                                 
0017       MSG = 'Date Should Be Less Than Or Equal To Current Date Of '                                ;* DOCDATE<33> should be LTE DateTime.Now.Date ✔
0018       MSG = MSG:OCONV(DATE(),'D2/')                                          
0019       CALL SB.DISP(3,MSG)                                                  
0020       RETURN           
0021	 END	
0022     OPEN '','PERIODS' TO F.PERIODS ELSE                                                            ;* Must be able to open PERIODS ✔
0023		CALL SB.DISP(3,'Cannot Open PERIODS')                                      
0024       RETURN                                                               
0025     END                                                                    
0026     READ PERIOD.REC FROM F.PERIODS,"1" ELSE
0027       CALL SB.DISP(3,'Record 1 Not Found On PERIODS')                      
0028       RETURN                                                               
0029     END                                                                                            ;* Have now mapped and exposed a CLR SELECT for the PERIODS table
0030        * Job 7721 Start (Added 30/03/01 by MSD)                               
0031        * Add 'sensibility' check to date entered. This is based on new        
0032        * field added to Company file to indicate max no of years that entry   
0033        * may be before current financial year. If the field has not been      
0034        * set up then 2 years is assumed.                                                           ;* Sanity check: you can only POST items which are sufficiently recent
0035     L.YEAR = PERIOD.REC<2,1>                ;* Get start of current GL year.                       ;* We can now mimic this
0036     L.YEAR = OCONV(L.YEAR,"DY")             ;* Convert to year only ie 2001                        ;* and pull the year component easily
0037     L.INPYR = OCONV(VALUE,"DY")             ;* Convert input to year ie 1984                       ;* and pull the year component from the incoming register entry easily
0038     L.SENSEYR = OCONV("1",'TCOMPANY;X;;165');* Get no of yrs allowed ie 3                          ;* "recent" being dictated by COMPANY<165> @ID = "1"
0039     IF L.SENSEYR = "" THEN
0040         L.SENSEYR = 2                       ;* Limit to 2                                          ;* defaulting to 2 years hence
0041     END                                                                    
0042     L.VALYR = L.YEAR - L.SENSEYR            ;* Set to start of year - accept able yrs ie 2001 - 3  ;* Start of the current GL year less Limit                                         
0043	 IF L.INPYR < L.VALYR THEN                                                                      ;* Is the incoming DOCDCATE older than the maximum accepted? 
                                                                                                        ;* if so, explodey kaboom with an error message.
0044         CALL SB.DISP(3,"Date Must Be Within ":L.SENSEYR:" Year(s) Of The Current Financial Year : ":L.YEAR)
0045         RETURN                                                                                     ;* RETURN (RTN.FLAG currently == 1)
0046     END                                                                                            ;*
                                                                                                        ;* ✔
0047     * Job 7721 end                                                         
0048     PERIODS = OCONV("1",'TCOMPANY;X;;11')     ;* Pull the number of General Ledger Periods from COMPANY<11> @ID = "1"  (we already have this mapped); we have 12 GL periods.     
0049 *-                                            ;* Companies vary; you might have quarterly ledgering and then this number would be 4 (one financial year of 12 months divided into 4 periods, each of 3 months)
0050 * Use PL PERIOD for separate PL or GL period if GL/PL combined       
0051 *-                                                                  ;* Check if we need to use a separate PL period from the GL period
0052     THIS.PD = OCONV("1",'TCOMPANY;X;;134')                          ;* by reading "PLPER" from COMPANY<134> @ID = "1" == This Period P/L
0053 * Job 2000 MH                                                              
0054     PLPER = THIS.PD                                                 ;* PLPER == PurchaseLedgerPeriod == ThisPeriod
0055 *   IF THIS.PD = "" THEN                                            ;* [this line is commented out!!!] If Purchase Ledger Period is "" (your business might not separate PL from GL periods; ours does)   
0056     THIS.PD = OCONV("1",'TCOMPANY;X;;10')                           ;* Read "GLPER" COMPANY<10> @ID = "1" == current GL Period
0057     GLPER = THIS.PD                                                 ;* ... then overwrite GLPER with THIS.PD
                                                                         ;* at this point, THIS.PD is set to THIS GENERAL LEDGER PERIOD
                                                                         ;* and PLPER is the only definitive grab we have on PurchaseLedgerPeriod
0058 *   END                                                                    
0059 * Job 2000 End                                                      ;* ✔
0060 *-                                                                         
0061     RECORD<154> = ''                                                ;* Set Future on TPLEDG entry to ''

                                                                         ;* Sanity checks incoming...
0062     IF PERIODS = '' OR THIS.PD = '' THEN                            ;* Check if CountOfGeneralLedgerPeriods = '' or CurrentGeneralLedgerPeriod = '' 
0063       CALL SB.DISP(3,'Invalid No Of Periods/Period No. On COMPANY') ;* Kaboom, we've got bigger problems.    ✔
0064       RETURN                                                        ;*
0065	 END                                                             ;* Still sane? Right, good...

0066 * Job 2000 MH                                                              
0067 * Job 2573 MH                                                              
0068 *   IF PLPER > GLPER THEN                                           ;* [this line is commented out; ignore it]
                                                                         
0069     PLCLOSE = OCONV("1",'TCOMPANY;X;;135')                          ;* read PLCLOSE from COMPANY<135> "Purchase Ledger Current Period End"
0070     IF VALUE > PLCLOSE THEN                                         ;* If the incoming Value is greater than the currently PL period close
0071 * Job 2573 End                                                      ;* This line is an annoyingly placed comment, ignore it   
0072       RECORD<154> = 'Y'                                             ;* Set TPLEDG<154> "FUTURE" to be equal to "Y".     ✔ See This document circa:700
0073       ** Job 7535 +                                                        
0074       IF RECORD<154> = 'Y' THEN                                     ;* If TPLEDG<154> "FUTURE" == "Y" (come on, guys... we literally just set this value...)
0075         SAVE.RTN.FLAG = RTN.FLAG ;*10170                            ;* SAVE.RTN.FLAG must be a temporary... I don't know why. Possibly because the user can do X to the display, changing RTN.FLAG.
0076         CALL SB.DISP(4,'Warning! This date is in advance of the current P/L period')   ;* Display a warning but allow it to be accepted                                               
0077         RTN.FLAG = SAVE.RTN.FLAG ;*10170                            ;* I do know that RTN.FLAG is restored from SAVE.RTN.FLAG here. Hurrah. Warning ignored.
0078       END                                                           ;* ✖ I can't warn from within the context of a CLR transaction execution.
0079     ** Job 7535 -                                                          
0080     END                                                                    
0081 * Job 2000 End                                                          
0082 *                                                                          
0083 * CALCULATE THIS OR NEXT                                            ;* Determine whether we're posting into this period or the next one
0084     GLFLAG = 0                                                      ;* Set a variable named GLFLAG to 0
0085 * Job 2573 MH                                                       ;* VALUE is the input to the execution of the subroutine: TPLEDG<33> (DOCDATE aka. PIDOCDATE) 
0086 *                                                                   ;* PERIOD.REC is the single record in PERIODS
0087 	 IF RECORD<154> # "Y" THEN GOTO 999                              ;*       ; PERIOD.REC field 2 is "THISYEAR" (MV),  --- Escape early, we're not dealing with a future posting
                                                                         ;*       ; THIS.PD is lifted from OCONV(TCOMPANY;X;;134) above (just read field 134 "PLPER")
0088 * JOB 2573 END                                                      ;* So: if DOCDATE > (e.g. 30/09/2025) (LIST PERIODS "1" EVAL "OCONV(OCONV("1","TPERIODS;X;;2,3"), "D4/")")
0089 * JOB 10575                                                         ;*       Then TPLEDG<154> "FUTURE" is set to "Y" (true, in our case)
                                                                         ;*     else
                                                                         ;*       GOTO 999 == RTN.FLAG = 0 RETURN! Hooray!
0090 *                                                                   ;* 
0091     IF THIS.PD = PERIODS OR (PLPER + 1) > PERIODS THEN              ;* IF ThisGeneralLedgerPeriod == GeneralLedgerPeriodCount (is this the final period?) e.g. 12 == 12
                                                                         ;*     OR if ThisPurchaseLedgerPeriod + 1 (== next period) > GeneralLedgerPeriodCount
                                                                         ;* THEN we're looking to next year...
0092       IF PERIOD.REC<3> = '' THEN                                           ;*     and... if next year doesn't exist (accounts haven't yet sorted out the GL dates for next financial year)
0093         CALL SB.DISP(3,'Next Year GL Period dates not set')                    ;*      then explodey kaboom; you're not POSTing this.
0094         RETURN                                                                     ;*  Exit, stage left. ✔
0095       END                                                                          ;*     
0096 *                                                                                  ;*  
                                                                                        ;*  PERIOD.REC<3,PD> is "NEXTYEAR", subvalue PD. 
                                                                                        ;*  if (DOCDATE > PD) 
                                                                                        ;*    GLFLAG = 1; GLDATE = First GL period of next year.
0097       IF PLPER = PERIODS THEN PD = 1 ELSE PD = 2                                   ;*  if (PurchaseLedgerPeriod = Last Period)
                                                                                        ;*     PD = 1
                                                                                        ;*  else
                                                                                        ;*     PD = 2
                                                                                        ;*     
0098       IF VALUE > PERIOD.REC<3,PD> THEN GLFLAG = 1; GLDATE = PERIOD.REC<3,PD>       ;*  Bad times. ✔
                                                                                        ;*  
0099     END ELSE                                                                       ;*
0100 *                                                                                  ;*
0101       IF VALUE > PERIOD.REC<2,PLPER+1> THEN GLFLAG = 1; GLDATE = PERIOD.REC<2,PLPER+1>    ;* This is all to determine whether or not we are attempting to ledger a document dated in the future: past the next period on the General Ledger.                                        
0102     END                                                                                   ;* It's all but (and possibly actually) illegal to post into future periods, for a number of reasons.
0103     IF GLFLAG = 1 THEN                                                                    ;*
0104       GLD = OCONV(GLDATE,"D2/")                                                           ;*                                             
0105 *                                                                                         ;*
0106       CALL SB.DISP(3,"Cannot Post Past Next Pd PL ":GLD)                                  ;*    
0107 * JOB 10575 END                                                                           ;*
0108       RETURN                                                                              ;*                                                      
0109     END                                                                                   ;*                                               
0110 *                                                                          
0111 999 RTN.FLAG = 0                                                             
0112     RETURN
 *  
 *  
 *  
 * 
 * 
 * Process after update: PG.RUN.PREPAY.APPORTION.DES,<53>,<39>  -- <53> == DocumentReference, <39> == SupplierCode
 * 
 * 
 * 
 * ÚÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄ Process Defn - Paragraph ÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄ¿ 
 * ³ Process Name    PG.RUN.PREPAY.APPORTION.DES                                 ³ 
 * ³ ÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄ ³ 
 * ³                                                              			      ³ 
 * ³ LOCAL L.TRANSNO, L.SUPREF, L.SVPARMS1, L.SVPARMS2, L.PPKEYS, L.MAX, L.POS   ³ 
 * ³ LOCAL L.SVRTNFLAG                                                           ³ 
 * ³ *                                                                           ³ 
 * ³ L.SVRTNFLAG = @RTN.FLAG ; @RTN.FLAG = 0                                     ³ 
 * ³ L.TRANSNO = @PARAM"G0,1"                                                    ³ G0,1 means "Zeroth Parameter, SV mark 1"
 * ³ L.SUPREF  = @PARAM"G1,1"                                                    ³ G1,1 means "Zeroth+1 Parameter, SV mark 1"
 * ³ *                                                                           ³ 
 * ³ L.SVPARMS1 = @PARMS(1) ; @PARMS(1) = ""                                     ³ 
 * ³ L.SVPARMS2 = @PARMS(2) ; @PARMS(2) = ""                                     ³ 
 * ³ *                                                                           ³ 
 * ³ @PARMS(1) = 'PREPAY.TRANSNO = "':L.TRANSNO:'"'                              ³ @PARMS(1) = 'PREPAY.TRANSNO = "PI######"'
 * ³ L.PPKEYS = P("PS.SEL.GLPPTRANS.DES")                                        ³     SELECT from GLPPTRANS.DES mode MVRZ = return all MV keys in requested format, suppressing messages, no window
 * ³ IF NOT(@RTN.FLAG) THEN                                                      ³ By this point therefore the transaction will have created GLPPTRANS.DES records as appropriate:
 * ³    L.MAX = DCOUNT(L.PPKEYS,@VM)                                             ³ without asking the user, these are apportioned.
 * ³    L.POS = 1                                                                ³ 
 * ³    IF L.MAX > "0" THEN                                                      ³ 
 * ³       EXEC "RSP,GL,PG.POST.PREPAY.DES,":L.PPKEYS                            ³ Oh, fun, more paragraphs to digest...
 * ³    END                                                                      ³ 
 * ³ END                                                                         ³ 
 * ³ 999                                                                         ³ 
 * ³ @PARMS(1) = L.SVPARMS1                                                      ³ 
 * ³ @PARMS(2) = L.SVPARMS2                                                      ³ 
 * ³ @RTN.FLAG = L.SVRTNFLAG                                                     ³ 
 * ³                                                                             ³ 
 * ÀÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÙ 
 * 
 * 
 * [General Ledger]
 * PG.POST.PREPAY.DES
 ³ *************************************************************************** ... So, for each of the GLPPTRANS.DES keys you've just gen
³ * Job No.  - DES1002                                                      * ³ 
³ * Dev By   - MRG                                                          * ³ 
³ * Dev Date - 06/01/10                                                     * ³ 
³ * Purpose  - This routine will automatically post the Prepayments to the  * ³ 
³ *            to the Ledgers for a given list of GLPPTRANS.DES records.    * ³ 
³ *************************************************************************** ³ 
³ LOCAL L.SV.RECORD, L.SV.WORK, L.SV.KEY, L.SV.CNT, L.SV.PARMS2, L.PPKEYS     ³ 
³ LOCAL L.PPREC, L.POS, L.MAX, L.AUTO, L.MANKEYS, I, L.COREC, L.ENDDATE       ³ 
³ LOCAL L.AUTOKEYS                                                            ³ 
³ *                                                                           ³ 
³ L.PPKEYS = @PARAM                                                           ³ 
³ *                                                                           ³ 
³ L.SV.RECORD = @RECORD   ;                                                   ³ 
³ L.SV.PARMS2 = @PARMS(2) ; @PARMS(2) = ""                                    ³ 
³ L.SV.WORK   = @WORK                                                         ³ 
³ L.SV.KEY    = @KEY                                                          ³ 
³ L.SV.CNT    = @CNT                                                          ³ 
³ *                                                                           ³ 
³ L.PPREC = "" ; L.MANKEYS = "" ; L.AUTOKEYS = "" ; L.COREC = ""              ³ 
³ *                                                                           ³ 
³ L.COREC = F("COMPANY","1")                                                  ³ 
³ *                                                                           ³ 
³ L.POS = 1                                                                   ³ 
³ L.MAX = DCOUNT(L.PPKEYS,@VM)                                                ³ 
³ WHILE L.POS <= L.MAX DO                                                     ³ 
³    READ L.PPREC FROM "GLPPTRANS.DES",L.PPKEYS<1,L.POS>                      ³ 
³    IF NOT(@RTN.FLAG) THEN                                                   ³ 
³       ***                                                                   ³ 
³       * Check that all required info is present.                            ³ 
³       ***                                                                   ³ 
³       IF L.COREC<11> = "" THEN GOTO 100 ;* No. Financial Periods Null       ³ 
³       IF L.COREC<10> = "" THEN GOTO 100 ;* This Period Null                 ³ 
³       IF L.PPREC<1>  = "" THEN GOTO 100 ;* Prepayment Supplier              ³ 
³       IF L.PPREC<2>  = "" THEN GOTO 100 ;* Prepayment G/L Code              ³ 
³       IF L.PPREC<3>  = "" THEN GOTO 100 ;* Prepayment Amount                ³ 
³       IF L.PPREC<21> = "" THEN GOTO 100 ;* Start Period                     ³ 
³       IF L.PPREC<22> = "" THEN GOTO 100 ;* No. Periods                      ³ 
³       IF L.PPREC<23> = "" THEN GOTO 100 ;* Split Equally                    ³ 
³       IF L.PPREC<24> = "" THEN GOTO 100 ;* Split Amounts                    ³ 
³       IF L.PPREC<27> = "" THEN GOTO 100 ;* Expense G/L Code                 ³ 
³       IF S(L.PPREC<24>) # L.PPREC<3> THEN GOTO 100 ;* Sum of Split Amounts  ³ ;* You want to check that they're actually auto-postable
³       ***                                                                   ³ 
³       * Automatic Posting Allowed                                           ³ 
³       ***                                                                   ³ 
³       *                                                                     ³ 
³       @WORK<1>      = L.PPKEYS<1,L.POS> ;* Audit Key                        ³ 
³       @WORK<2>      = L.PPREC<1>        ;* Supplier                         ³ 
³       @RECORD<57>   = L.PPREC<5>:" ":F("PLSUP",@WORK<2>)<1.1> ;* Memo       ³ 
³       @RECORD<54.1> = L.PPREC<27>       ;* Expense G/L Code                 ³ 
³       @RECORD<54.2> = L.PPREC<2>        ;* Prepayment G/L Code              ³ 
³       @RECORD<60>   = L.PPREC<3>        ;* Prepayment Amount                ³ 
³       @RECORD<55>   = L.PPREC<21>       ;* Start Period                     ³ 
³       @RECORD<62>   = L.PPREC<22>       ;* No. Periods                      ³ 
³       @RECORD<56>   = L.PPREC<23>       ;* Split Equally                    ³ 
³       @RECORD<58>   = L.PPREC<24>       ;* Split Amounts                    ³ 
³       IF L.PPREC<25> # "" THEN          ;* Split Memos                      ³ 
³          @RECORD<67> = L.PPREC<25>                                          ³ 
³       ELSE                                                                  ³ 
³          @RECORD<67> = DUP(@RECORD<57>,@RECORD<58>)                         ³ 
³       END                                                                   ³ 
³       @RECORD<61>   = S(L.PPREC<24>)    ;* Balance                          ³ 
³       *                                                                     ³ 
³       L.ENDDATE = F("PERIODS","1")<2,L.COREC<10>>                           ³ 
³       IF @DATE > L.ENDDATE THEN GOTO 100                                    ³ 
³       @RECORD<50>   = @DATE             ;* Document Date                    ³ 
³       *                                                                     ³ 
³       @VALUE = @RECORD<62>                                                  ³ 
³       CALL GLPREPAY.DES,2                                                   ³   /// this performs the call to GLPREPAY.DES!
³       IF @RTN.FLAG THEN GOTO 100                                            ³ 
³       *                                                                     ³ 
³       @RECORD<51>   = B("RFGENKEY,.1.9") ;* Document Reference              ³ 
³       @RECORD<2>    = @OTHER(4)                                             ³ 
³       @RECORD<3>    = @OTHER(5)                                             ³ 
³       FOR I = 1 TO DCOUNT(@RECORD<58>,@VM)                                  ³ 
³          @RECORD<104,I> = @RECORD<51>:".PRE":I                              ³  /// this stores the prepayment records generated to @RECORD<104>
³       NEXT I                                                                ³  
³       EXEC "PU.PREPAY.DES"                                                  ³  

        /// what and where is PU.PREPAY.DES
        /// it's a File Update definition in the GL subsystem.
        /// Here we go:
    
        ÚÄÄÄÄÄÄÄÄÄÄÄÄÄProcess Defn - File UpdateÄÄÄÄÄÄÄÄÄÄÄÄÄÄ¿           
        ³ Process Name    PU.PREPAY.DES                       ³           
        ³ Description     Prepayment Apportionment            ³           
        ³ Source Dict File          Files To Update           ³           
        ³ TGLEDG                    GLBATCH                   ³           
        ³³                          GLBATCHDATA1              ³           
        ³                           GLPPTRANS.DES             ³           
        ³                                                     ³           
        ³                                                     ³          
        ³                                                     ³   
        ÚÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄ Secondary File Update ÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄ¿   
        ³ File Update (Dict) : GLBATCH                                              ³   
        ³                                                                           ³   
        ³ Update/Delete (U/D)  U                                                    ³   
        ³ Update Condition                                                          ³   
        ³ Data File (if diff)                                                       ³   
        ³ Key Source           <51>                                                 ³   
        ³                                                                           ³   
        ³ Fld To Update  MV To Update    Oper   Value To Use                        ³   
        ³ TYPE                           R      "RFPOST"                            ³   
        ³ DATAKEYS                       I      <104>                               ³   
        ³ NORECS                         R      <62>                                ³   
        ³                                                                           ³   
        ÀÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÙ          
        ³                                                     ³          
        ³                                                     ³   
        ÚÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄ Secondary File Update ÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄ¿   
        ³ File Update (Dict) : GLBATCHDATA1                                         ³   
        ³                                                                           ³   
        ³ Update/Delete (U/D)  U                                                    ³   
        ³ Update Condition                                                          ³   
        ³ Data File (if diff)                                                       ³   
        ³ Key Source           <104,-1>                                             ³   
        ³                                                                           ³   
        ³ Fld To Update  MV To Update    Oper   Value To Use                        ³   
        ³ TYPE                           R      "RFPOST"                            ³   
        ³ DOCREF                         R      <104,-1>                            ³   
        ³ PREFIX                         R      "PRE"                               ³   
        ³ DOCDATE                        R      <49,-1>                             ³   
        ³ POSTDATE                       R      @DATE                               ³   
        ³ SUPREF                         R      W.PREPAY.SUPREF.DES                 ³   
        ³ CUSNAME                        R      W.PREPAY.SUPNAME.DES                ³   
        ³ AMOUNT                         R      <58,-1>                             ³   
        ³ AMOUNT                         I      0-<58,-1>                           ³   
        ³ GLCODE                         R      <54>                                ³   
        ³ TRANSTYPE                      R      "PREPAYMENT"                        ³   
        ³ MEMO                           R      <57>                                ³   
        ³ MEMO1                          R      <67,-1>                             ³   
        ³ MEMO1                          I      <67,-1>                             ³   
        ³ USERID                         R      @USER.ID                            ³   
        ³ POSTTIME                       R      @TIME                               ³   
        ³                                                                           ³   
        ÀÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÙ          
        ³                                                     ³          
        ³                                                     ³   
        ÚÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄ Secondary File Update ÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄ¿   
        ³ File Update (Dict) : GLPPTRANS.DES                                        ³   
        ³                                                                           ³   
        ³ Update/Delete (U/D)  U                                                    ³   
        ³ Update Condition     F("GLPPTRANS.DES",W.PREPAY.AUDITKEY)<0> # ""         ³   
        ³ Data File (if diff)                                                       ³   
        ³ Key Source           W.PREPAY.AUDITKEY                                    ³   
        ³                                                                           ³   
        ³ Fld To Update  MV To Update    Oper   Value To Use                        ³   
        ³ APPORTIONED                    R      "Y"                                 ³   
        ³ START.PD                       R      <55>                                ³   
        ³ NO.PDS                         R      <62>                                ³   
        ³ SPLIT.EQUAL                    R      <56>                                ³   
        ³ SPLIT.AMOUNT                   R      <58>                                ³   
        ³ SPLIT.MEMO                     R      <67>                                ³   
        ³ SPLIT.DOCDAT                   R      <49>                                ³   
        ³                                                                           ³   
        ÀÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÙ  


/// aaand back to the paragraph in question ([GL]PG.POST.PREPAY.DES)

³       L.AUTOKEYS = INS(L.AUTOKEYS,1,-1,L.PPKEYS<1,L.POS>)                   ³  /// can only imagine INS in INSERT into position
³       GOTO 200                                                              ³ 
³ 100                                                                         ³ 
³ ****                                                                  	  ³ /// All of this MANKEY process stuff has been commented out!
³ 	 * Manual Posting Required                                            	  ³ 
³ 	 *                                                                   	  ³ 
³ 	 * L.MANKEYS = INS(L.MANKEYS,1,-1,L.PPKEYS<1,L.POS>)                      ³ 
³ END                                                                      	  ³ 
³ 200                                                                         ³ 
³    @WORK = "" ; @KEY = "" ; @RECORD = "" ; @CNT = 0                         ³ 
³    L.POS = L.POS + 1                                                        ³ 
³ REPEAT                                                                      ³
³ ***                                                                         ³ 
³ * If Manual Posting Required then run routine.                              ³ 
³ ***                                                                         ³ 
³ *IF L.MANKEYS # "" THEN                                                     ³ 
³ *   IF L.AUTOKEYS = "" THEN                                                 ³ 
³ *      DISP 4,"All Prepayments need to be posted Manually."                 ³ 
³ *   ELSE                                                                    ³ 
³ *      DISP 4,"Some Prepayments need to be posted Manually."                ³ 
³ *   END                                                                     ³ 
³ *   @PARMS(2) = L.MANKEYS                                                   ³ 
³ *   EXEC "GL*PREPAY.DES"                                                    ³ 
³ *END																		  ³ 
³ *                                                                           ³ 
³ @RECORD   = L.SV.RECORD                                                     ³ 
³ @WORK     = L.SV.WORK                                                       ³ 
³ @KEY      = L.SV.KEY                                                        ³ 
³ @CNT      = L.SV.CNT                                                        ³ 
³ @PARMS(2) = L.SV.PARMS2                                                     ³ 
³                                                                             ³ 
ÀÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÙ 
 * 
 */

namespace MertexerV2.PurchaseLedgerFunctions
{
    public static partial class PurchaseLedgerFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "PurchaseLedger_InsertFromInvoiceRegistry_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable InsertFromInvoiceRegistry(string UniVerseAccount, string PurchaseLedgerInvoiceRegistryJSON)
        {
            var results = new List<PurchaseLedgerInsertionResult>();

            var now = DateTime.Now;

            var registerEntryJson = PurchaseLedgerInvoiceRegistryJSON;

            var registerEntryJObj = JObject.Parse(registerEntryJson);

            // IncomingEmail.InvoiceRegisterEntry_SubmitToPurchaseLedger_Select 
            var registerEntry = registerEntryJObj["invoiceRegisterEntry"].ToObject<PurchaseLedgerInvoiceRegister>();

            var vatableAmountsInCurrency = registerEntryJObj["vatEntries"].Children()["vatableAmountCurrency"].Values<decimal>().ToArray();

            var vatTaxesInCurrency = registerEntryJObj["vatEntries"].Children()["vatAmountCurrency"].Values<decimal>().ToArray();

            using (var qe = new QueryExecutor(UniVerseAccount))
            {
                var session = qe.Session;

                var transaction = session.CreateUniTransaction();

                transaction.Begin();

                #region Properties & Initialisation

                #region Keys

                // Read the most recently held TPLEDG key from PLCONTROL<9> WITH @ID = "GENNO"

                // !! Important: are these field positions the same between MERTEX and PLAY?
                // In this instance, yes.

                var purchaseLedgerControlFileFieldPosition = 9;

                var purchaseLedgerControlFile = session.CreateUniFile("PLCONTROL");

                var numberGeneratorRecords = purchaseLedgerControlFile.ReadRecords(new[] { "GENNO" });

                var numberGeneratorRecord = numberGeneratorRecords.GetRecord(0);

                var numberField = numberGeneratorRecord.Record.Extract(purchaseLedgerControlFileFieldPosition); // Field 9 on the record "GENNO"

                var currentKey = Convert.ToInt32(numberField.StringValue);

                var newKey = currentKey += 1;

                numberGeneratorRecord.Record.Replace(purchaseLedgerControlFileFieldPosition, $"{newKey}");

                purchaseLedgerControlFile.WriteRecords(numberGeneratorRecords);

                var purchaseLedgerKey = $"PI{newKey.ToString().PadLeft(6, '0')}";

                #endregion

                #region Other Files and Errata

                var companyEntry = Selector<Company>.Select("1", true, false, 1, UniVerseAccount);

                var generalLedgerPrePaymentCodes = companyEntry.PrepaymentGeneralLedgerCodes.ToList();

                var companyStatsEntry = Selector<CompanyStats>.Select("1", true, false, 1, UniVerseAccount);

                var supplierEntry = Selector<Models.Supplier.Supplier>.Select(registerEntry.SupplierCode, true, false, 1, UniVerseAccount);

                var supplierCurrencyEntry = Selector<Models.Supplier.SupplierCurrency>.Select(registerEntry.SupplierCode, true, false, 1, UniVerseAccount);

                var companyVATEntries = Selector<CompanyVAT>.Select(registerEntry.VATCodes, true, false, 1, UniVerseAccount);

                var purchaseLedgerInvoiceRegisterEntry = Selector<PurchaseLedgerInvoiceRegister>.Select(registerEntry.Key, true, false, 1, UniVerseAccount);

                var periods = Selector<Periods>.Select("1", true, false, 1, UniVerseAccount);

                var currentPurchaseLedgerPeriod = companyEntry.ThisPurchaseLedgerPeriod;

                var currentGeneralLedgerPeriod = companyEntry.ThisGeneralLedgerPeriod;

                var generalLedgerPeriodsCount = companyEntry.NumberOfFinancialPeriods;

                var purchaseLedgerTransactionPeriod = currentPurchaseLedgerPeriod;

                var prePaymentPurchaseLedgerCodes = companyEntry.PrepaymentGeneralLedgerCodes.ToList();

                var purchaseLedgerEntryIsInTheFuture = false;

                #endregion

                #endregion

                #region Validation (Preflight Checks)

                #region Step 01. Sanity checks

                if (purchaseLedgerInvoiceRegisterEntry == null)
                {
                    throw new Exception($"PLINVREG entry \"{registerEntry.Key}\" was not found");
                }

                if (purchaseLedgerInvoiceRegisterEntry.Posted)
                {
                    throw new Exception($"PLINVREG entry \"{registerEntry.Key}\" has already had POSTED = Y");
                }
                else
                {
                    // Set POSTED
                    purchaseLedgerInvoiceRegisterEntry.Posted = true;

                    PurchaseLedgerInvoiceRegisterFunctions.UpsertViaTransaction(ref session, JsonConvert.SerializeObject(purchaseLedgerInvoiceRegisterEntry));
                }

                if (registerEntry.DueDT < registerEntry.DocumentDT)
                {
                    throw new Exception("RegisterEntry.DueDT precedes RegisterEntry.DocumentDT");
                }

                if (registerEntry.SettlementDT < registerEntry.DocumentDT)
                {
                    throw new Exception("RegisterEntry.SettlementDT precedes RegisterEntry.DocumentDT");
                }

                if (registerEntry.DocumentDT > DateTime.Now)
                {
                    throw new Exception("RegisterEntry.DocumentDT must precede DateTime.Now");
                }

                #endregion


                #region Step 02. Mimic the behaviour of PLCHKDATE (see huge comments section above)

                var currentGeneralLedgerYear = periods.GeneralLedgerPeriodDatesThisYearDTs.First().Year; // This will throw if First() is NULL; good.

                var registerEntryDocumentDateYear = registerEntry.DocumentDT.Value.Year;

                var permittedPostAgeYears = companyEntry.PostDateCheckYears;

                if (registerEntryDocumentDateYear < currentGeneralLedgerYear - permittedPostAgeYears)
                {
                    throw new Exception($"You may not POST entries which are more than than {permittedPostAgeYears} years old");
                }

                if (currentGeneralLedgerPeriod == 0)
                {
                    throw new Exception("currentPeriod may not be 0");
                }

                if (generalLedgerPeriodsCount == 0)
                {
                    throw new Exception("generalLedgerPeriods may not be 0");
                }

                if (registerEntry.DocumentDT > companyEntry.ThisPurchaseLedgerPeriodEndDT)
                {
                    purchaseLedgerEntryIsInTheFuture = true;
                }

                if (purchaseLedgerEntryIsInTheFuture)
                {
                    // Whole bunch of other stuff has to happen: see line 267+ above

                    // This is some interesting BS: THIS.PD = PERIODS OR (PLPER + 1) > PERIODS

                    if ((currentGeneralLedgerPeriod == generalLedgerPeriodsCount) || ((currentPurchaseLedgerPeriod + 1) > generalLedgerPeriodsCount)) // see PLCHKDATE:0091 
                    {
                        if (!periods.GeneralLedgerPeriodDatesNextYearDTs.Any())
                        {
                            throw new Exception("Next Year's General Ledger Dates have not been set!"); // see PLCHKDATE:0093
                        }
                    }

                    var periodAdjustment = currentPurchaseLedgerPeriod == generalLedgerPeriodsCount ? 1 : 2; // see PLCHKDATE:0097

                    if (purchaseLedgerInvoiceRegisterEntry.DocumentDT > periods.GeneralLedgerPeriodDatesNextYearDTs.ElementAt(periodAdjustment))
                    {
                        throw new Exception($"RegisterEntry.DocumentDT > GeneralLedgerPeriodDatesNextYearDTs[{periodAdjustment}] ({periods.GeneralLedgerPeriodDatesNextYearDTs.ElementAt(periodAdjustment)})");
                    }

                    // Essentially here we must ensure that we forbid posting past the NEXT period. UGH.
                    // Correction: we must ensure that we are DOING WHAT MERTEX DOES.
                }

                #endregion


                #endregion



                #region Step 01. Create a Purchase Ledger entry (INSERT to TPLEDG)

                var purchaseLedgerEntry = new PurchaseLedger(purchaseLedgerKey)
                {
                    DocumentNumber = $"{newKey}",                    // Field 34 DOCNO is set to the ledger key without the PI prefix
                    DocumentReference = purchaseLedgerKey,           // Field 53 PINVDOC is set to the @ID of the TPLEDG record
                    PurchaseLedgerRegisterKey = registerEntry.Key,   // Field 199 PLINVREG is set to the registry entry's key
                    SourceTransactionDefinition = "PINVIR.CUR.DES",  // Not described by the dictionary, but reading the data tells us it should be set accordingly.
                    DocumentDT = registerEntry.DocumentDT,           // Field 33 DOCDATE is set to the document date as declared on the register entry.
                    OnFileDT = registerEntry.OnFileDT,               // Field 36 DATEONFILE is set to the date... that the... registry entry was created? Deosn't sound right.
                                                                     // Should that not be "DateTime.Now"? NO: Determined by cross referencing a TPLEDG record with a PLREGISTER.HIST record
                                                                     // the PLREGISTER.HIST record retains the date the register entry was created; this matches the DATEONFILE for the given TPLEDG rec.
                                                                     // POSTDATE is the one we want to set to DateTime.Now.
                    SupplierInvoiceReference = registerEntry.ItemReference, // Field 37 SUPINVNO is carried in registerEntry.ItemReference.
                    InvoiceDueDT = registerEntry.DueDT,              // Field 38 INVDUE is carried in registerEntry.DueDT
                    SupplierCode = registerEntry.SupplierCode,       // Field 39 PSUPREF is carried on registerEntry.SupplierCode
                    Amounts = registerEntry.AmountsExVAT,            // 
                    GeneralLedgerCodes = registerEntry.GeneralLedgerCodes,
                    VATCodes = registerEntry.VATCodes,
                    VATableValues = registerEntry.VATableValues,
                    TotalDueInSterling = registerEntry.AmountSterling,
                    SettlementTerms = registerEntry.PaymentTermsCode,
                    PostDT = now,                                    // See comments above
                    SettlementDueDTs = new List<DateTime?>() { registerEntry.SettlementDT }, // Fields 55 and 55,1 are both being set here, which is a bit redundant.
                    DueDT = registerEntry.SettlementDT,                                      // I've decided to do this simply because if we use purchaseLedgerEntry.DueDT
                                                                                             // later it's a cleaner way of pulling it, instead of asking for SettlementDTs.First()
                    Memo = registerEntry.Memo,                                               
                    VATBalance = registerEntry.VATBalance,
                    VATTaxes = registerEntry.VATTaxes,
                    VATTaxesInCurrency = vatTaxesInCurrency,
                    DocumentBalance = registerEntry.DocumentBalance,
                    DiscountAmount = 0.0m, // we don't do discounts here: LIST PLTERMS DISC
                    DiscountAmountInCurrency = 0.0M, // ""
                    OtherMemos = registerEntry.OtherMemos,
                    DisputeIndicator = registerEntry.Disputed,
                    DisputeMemo = registerEntry.DisputeMemo,
                    ReceivedDT = registerEntry.ReceivedDT,
                    PassedTo = registerEntry.PassedTo,
                    PassedToDT = registerEntry.PassedToDT,
                    DiscountPercentage = registerEntry.DiscountPercentage ?? 0.0m,
                    CurrencyCode = registerEntry.MertexCurrencyCode,
                    ExchangeRates = new List<decimal>() { registerEntry.ExchangeRate },
                    AmountInCurrency = registerEntry.AmountCurrency,
                    UnallocatedAmountInCurrency = registerEntry.AmountCurrency,
                    UnallocatedCreditNoteAmountInCurrency = 0.0m,
                    // Prognostication: is this for some future period to worry about, or is it in the current period?
                    // All I can do is compare the Invoice Date (DocumentDT) with the current Month.
                    Future = purchaseLedgerEntryIsInTheFuture
                };

                var purchaseLedgerEntryJson = JsonConvert.SerializeObject(purchaseLedgerEntry);

                PurchaseLedgerFunctions.InsertViaTransaction(ref session, purchaseLedgerEntryJson);

                results.Add(new PurchaseLedgerInsertionResult(
                    fileKey: purchaseLedgerEntry.Key,
                    fileName: "TPLEDG",
                    operationsPerformed: new[] { "INSERT" },
                    jsonBefore: null,
                    jsonAfterwards: purchaseLedgerEntryJson,
                    purchaseLedgerRegistryKey: registerEntry.Key
                ));

                if (purchaseLedgerEntry.Future == true)
                {
                    purchaseLedgerTransactionPeriod += 1;

                    if (currentGeneralLedgerPeriod > companyEntry.NumberOfFinancialPeriods)
                    {
                        purchaseLedgerTransactionPeriod = 1;
                    }
                }

                #endregion

                #region Step 02. Create a Purchase Ledger Register History entry (INSERT to PLREGISTER.HIST)

                // I'm going to be following the "Key Source" definition for each secondary file update: rather than store it in some variable and have it be
                // confusingly referred to in multiple places. I may, on reflection, refactor this. For now it avoids any potential for incorrectness or confusion.
                //
                // PLREGISTER.HIST Key Source == $"INV*{[TPLEDG].[PurchaseLedgerRegisterKey]<199>}"
                // 
                var purchaseLedgerRegisterHistoryEntry = new PurchaseLedgerRegisterHistory($"INV*{purchaseLedgerEntry.PurchaseLedgerRegisterKey}")
                {
                    RegisterNumber = purchaseLedgerEntry.PurchaseLedgerRegisterKey,
                    OnFileDT = purchaseLedgerEntry.OnFileDT,
                    SupplierCode = purchaseLedgerEntry.SupplierCode,
                    ConvertedDT = now,
                    DocumentReference = purchaseLedgerKey,
                    UserID = "BOT", // Not got anything better to set this to at this stage, I'm afraid...
                    TransactionType = "INV",
                    DeletedDT = null,
                    DocumentTotal = purchaseLedgerEntry.DocumentBalance
                };

                var purchaseLedgerRegisterHistoryEntryJson = JsonConvert.SerializeObject(purchaseLedgerRegisterHistoryEntry);

                PurchaseLedgerRegisterHistoryFunctions.InsertViaTransaction(ref session, purchaseLedgerRegisterHistoryEntryJson);

                results.Add(new PurchaseLedgerInsertionResult(
                    fileKey: purchaseLedgerRegisterHistoryEntry.Key,
                    fileName: "PLREGISTER.HIST",
                    operationsPerformed: new[] { "INSERT" },
                    jsonBefore:null,
                    jsonAfterwards: purchaseLedgerRegisterHistoryEntryJson,
                    purchaseLedgerRegistryKey: registerEntry.Key
                ));

                #endregion

                #region Step 03. Delete the given Purchase Ledger Invoice Register entry (DELETE from PLINVREG)

                var purchaseRegisterEntryJson = JsonConvert.SerializeObject(registerEntry);

                PurchaseLedgerInvoiceRegisterFunctions.DeleteViaTransaction(ref session, JsonConvert.SerializeObject(registerEntry));

                results.Add(new PurchaseLedgerInsertionResult(
                    fileKey: registerEntry.Key,
                    fileName: "PLINVREG",
                    operationsPerformed: new[] { "DELETE" },
                    jsonBefore: purchaseRegisterEntryJson,
                    jsonAfterwards: null,
                    purchaseLedgerRegistryKey: registerEntry.Key
                ));

                #endregion

                #region Step 04. Create a Purchase Ledger Transaction entry (INSERT into TPLTRANS)

                // Key == TPLEDG<53> "Document Reference"
                var purchaseLedgerTransactionEntry = new PurchaseLedgerTransaction(purchaseLedgerEntry.DocumentReference)
                {
                    DocumentDT = purchaseLedgerEntry.DocumentDT,
                    DueDT = purchaseLedgerEntry.InvoiceDueDT,
                    DiscountPercentage = purchaseLedgerEntry.DiscountPercentage_Again,
                    TransactionType = "INV",
                    ItemReference = purchaseLedgerEntry.SupplierInvoiceReference,
                    DocumentNumber = purchaseLedgerEntry.DocumentNumber,
                    VATTaxesTotal = purchaseLedgerEntry.VATTaxes.Sum(),
                    Credit = purchaseLedgerEntry.Amounts.Sum(),
                    Balance = purchaseLedgerEntry.TotalDueInSterling,
                    OriginalValueABS = purchaseLedgerEntry.TotalDueInSterling,
                    Period = purchaseLedgerTransactionPeriod,
                    SupplierReference = purchaseLedgerEntry.SupplierCode,
                    Age = purchaseLedgerEntry.Future == true ? 0 : 1,
                    Unallocated = 0.0m,
                    DiscountAmount = purchaseLedgerEntry.DiscountAmount,
                    PostDT = purchaseLedgerEntry.PostDT,
                    Memo = purchaseLedgerEntry.Memo,
                    DiscountCutOffDT = purchaseLedgerEntry.DueDT,
                    Dispute = purchaseLedgerEntry.DisputeIndicator,
                    DisputeMemo = purchaseLedgerEntry.DisputeMemo,
                    PurchaseLedgerRegisterNumber = $"INV*{purchaseLedgerEntry.PurchaseLedgerRegisterKey}"
                };

                var purchaseLedgerTransactionEntryJson = JsonConvert.SerializeObject(purchaseLedgerTransactionEntry);

                PurchaseLedgerTransactionFunctions.UpsertViaTransaction(ref session, purchaseLedgerTransactionEntryJson);

                results.Add(new PurchaseLedgerInsertionResult(
                    fileKey: purchaseLedgerTransactionEntry.Key,
                    fileName:  "TPLTRANS",
                    operationsPerformed: new [] { "INSERT" },
                    jsonBefore: null,
                    jsonAfterwards: purchaseLedgerTransactionEntryJson,
                    purchaseLedgerRegistryKey: registerEntry.Key
                ));

                #endregion

                #region Step 05. Create a Purchase Ledger Transaction Currency entry (INSERT into TPLTRANS.CUR)

                // Key == TPLEDG<53> "Document Reference"
                var purchaseLedgerTransactionCurrencyEntry = new PurchaseLedgerTransactionCurrency(purchaseLedgerEntry.DocumentReference)
                {
                    DocumentDT = purchaseLedgerEntry.DocumentDT,
                    DueDT = purchaseLedgerEntry.InvoiceDueDT,
                    DiscountPercentage = purchaseLedgerEntry.DiscountPercentage_Again,
                    TransactionType = "INV",
                    ItemReference = purchaseLedgerEntry.SupplierInvoiceReference,
                    DocumentNumber = purchaseLedgerEntry.DocumentNumber,
                    VATTaxesTotal = purchaseLedgerEntry.VATTaxesInCurrency.Sum(),
                    Credit = purchaseLedgerEntry.AmountInCurrency - purchaseLedgerEntry.VATTaxesInCurrency.Sum(),
                    Balance = purchaseLedgerEntry.AmountInCurrency,
                    OriginalValueABS = purchaseLedgerEntry.AmountInCurrency,
                    Period = purchaseLedgerTransactionPeriod,
                    SupplierReference = purchaseLedgerEntry.SupplierCode,
                    Age = purchaseLedgerEntry.Future == true ? 0 : 1,
                    Unallocated = 0.0m,
                    DiscountAmount = purchaseLedgerEntry.DiscountAmountInCurrency,
                    PostDT = purchaseLedgerEntry.PostDT,
                    Memo = purchaseLedgerEntry.Memo,
                    DiscountCutOffDT = purchaseLedgerEntry.DueDT,
                    Dispute = purchaseLedgerEntry.DisputeIndicator,
                    DisputeMemo = purchaseLedgerEntry.DisputeMemo,
                    CurrencyCode = purchaseLedgerEntry.CurrencyCode,
                    ExchangeRates = purchaseLedgerEntry.ExchangeRates,
                    PurchaseLedgerRegisterNumber = $"INV*{purchaseLedgerEntry.PurchaseLedgerRegisterKey}"
                };

                var purchaseLedgerTransactionCurrencyEntryJson = JsonConvert.SerializeObject(purchaseLedgerTransactionCurrencyEntry);

                PurchaseLedgerTransactionCurrencyFunctions.UpsertViaTransaction(ref session, purchaseLedgerTransactionCurrencyEntryJson);

                results.Add(new PurchaseLedgerInsertionResult(
                    fileKey: purchaseLedgerTransactionCurrencyEntry.Key,
                    fileName: "TPLTRANS.CUR",
                    operationsPerformed: new[] { "INSERT" },
                    jsonBefore: null,
                    jsonAfterwards: purchaseLedgerTransactionCurrencyEntryJson,
                    purchaseLedgerRegistryKey: registerEntry.Key
                ));

                #endregion

                #region Step 06. Update the relevant Supplier record (UPDATE to PLSUP)

                var supplierEntryBeforeModificationJSON = JsonConvert.SerializeObject(supplierEntry);

                supplierEntry.Balance += purchaseLedgerEntry.DocumentBalance;

                var supplierEntryNextPeriodBalance = CalculatePurchaseLedgerAmount(
                    amounts: new[] { purchaseLedgerEntry.TotalDueInSterling }, // <50> "CHQTOTAL/DTOTAL/RDTOTAL/STGAMT/STGCHQTOTAL/TOTDUE"
                                                                               // the subroutine itself explicitly returns early on line 40 if you're in the final period: IF PER=NOPER THEN RETURN        
                    currentPeriod: currentGeneralLedgerPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: "N",
                    future: purchaseLedgerEntry.Future == true,
                    mvOperationType: null
                );

                supplierEntry.NextPeriodBalance += supplierEntryNextPeriodBalance; // PLAMOUNT,N50

                var supplierEntryThisPeriodBalance = CalculatePurchaseLedgerAmount(
                    amounts: new[] { purchaseLedgerEntry.TotalDueInSterling },
                    currentPeriod: currentGeneralLedgerPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: null,
                    future: purchaseLedgerEntry.Future == true,
                    mvOperationType: null
                );

                supplierEntry.ThisPeriodBalance += supplierEntryThisPeriodBalance; // PLAMOUNT,50

                supplierEntry.DocumentReferences = supplierEntry.DocumentReferences.Prepend(purchaseLedgerEntry.Key);

                var supplierEntryPurchasesNextPeriod = CalculatePurchaseLedgerAmount(
                    amounts: purchaseLedgerEntry.Amounts, // <43> "PAMOUNT"
                    currentPeriod: currentGeneralLedgerPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: "N",
                    future: purchaseLedgerEntry.Future == true,
                    mvOperationType: "A"
                );

                supplierEntry.PurchasesNextPeriod += supplierEntryPurchasesNextPeriod; // PLAMOUNT,N43.A

                var supplierEntryPurchasesThisPeriod = CalculatePurchaseLedgerAmount(
                    amounts: purchaseLedgerEntry.Amounts,
                    currentPeriod: currentGeneralLedgerPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: null,
                    future: purchaseLedgerEntry.Future == true,
                    mvOperationType: "A"
                );

                supplierEntry.PurchasesThisPeriod += supplierEntryPurchasesThisPeriod; // PLAMOUNT,43.A

                var supplierEntryPurchasesYearToDate = CalculatePurchaseLedgerAmount(
                    amounts: purchaseLedgerEntry.Amounts,
                    currentPeriod: currentGeneralLedgerPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: "Y",
                    future: purchaseLedgerEntry.Future == true,
                    mvOperationType: "A"
                );

                supplierEntry.PurchasesYearToDate += supplierEntryPurchasesYearToDate; // PLAMOUNT,Y43.A

                // P("INACT")
                supplierEntry.PeriodsInactive = purchaseLedgerEntry.Future == true ? null : (sbyte?)-1;

                var supplierEntryJson = JsonConvert.SerializeObject(supplierEntry);

                SupplierFunctions.SupplierFunctions.UpsertViaTransaction(ref session, supplierEntryJson);

                results.Add(new PurchaseLedgerInsertionResult(
                    fileKey: supplierEntry.Key,
                    fileName: "PLSUP",
                    operationsPerformed: new[] {
                        $"<15> \"BALANCE\" (Balance) += {purchaseLedgerEntry.DocumentBalance}",
                        $"<18,1> \"NEXT\" (NextPeriodBalance) += {supplierEntryNextPeriodBalance}",
                        $"<18,2> \"CURR\" (ThisPeriodBalance) += {supplierEntryThisPeriodBalance}",
                        $"<19> \"TLINK\" += \"{purchaseLedgerEntry.Key}\"",
                        $"<21,1> \"PNEXT\" (PurchasesNextPeriod) += {supplierEntryPurchasesNextPeriod}",
                        $"<21,2> \"PCURR\" (PurchasesThisPeriod) += {supplierEntryPurchasesThisPeriod}",
                        $"<21,3> \"PYTD\" (PurchasesYearToDate) += {supplierEntryPurchasesYearToDate}",
                        $"<23> \"INACT\" (PeriodsInactive) = \"{supplierEntry.PeriodsInactive}\""
                    },
                    jsonBefore: supplierEntryBeforeModificationJSON,
                    jsonAfterwards: supplierEntryJson,
                    purchaseLedgerRegistryKey: registerEntry.Key
                ));

                #endregion

                #region Step 07. Update the relevant Supplier Currency record (UPDATE to PLSUP.CUR)

                var supplierCurrencyEntryBeforeModificationJSON = JsonConvert.SerializeObject(supplierCurrencyEntry);

                /* Update the Supplier Currency record */

                var supplierCurrencyEntryBalanceAddition = purchaseLedgerEntry.AmountInCurrency;

                supplierCurrencyEntry.Balance += supplierCurrencyEntryBalanceAddition; // <250> "CURAMT"

                var supplierCurrencyEntryNextPeriodBalanceAddition = CalculatePurchaseLedgerAmount(
                    amounts: new[] { purchaseLedgerEntry.AmountInCurrency }, // <250> "CURAMT"
                                                                             // the subroutine itself explicitly returns early on line 40 if you're in the final period: IF PER=NOPER THEN RETURN        
                    currentPeriod: currentGeneralLedgerPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: "N",
                    future: purchaseLedgerEntry.Future == true,
                    mvOperationType: null
                );

                supplierCurrencyEntry.NextPeriodBalance += supplierCurrencyEntryNextPeriodBalanceAddition; // PLAMOUNT,N250

                var supplierCurrencyEntryThisPeriodBalanceAddition = CalculatePurchaseLedgerAmount(
                    amounts: new[] { purchaseLedgerEntry.AmountInCurrency }, // <250> "CURAMT"
                    currentPeriod: currentGeneralLedgerPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: null,
                    future: purchaseLedgerEntry.Future == true,
                    mvOperationType: null
                );

                supplierCurrencyEntry.ThisPeriodBalance += supplierCurrencyEntryThisPeriodBalanceAddition; // PLAMOUNT,250

                supplierCurrencyEntry.DocumentReferences = supplierCurrencyEntry.DocumentReferences.Prepend(purchaseLedgerEntry.Key);

                var supplierCurrencyEntryPurchasesNextPeriodAddition = CalculatePurchaseLedgerAmount(
                    amounts: new[] { purchaseLedgerEntry.AmountInCurrency }, // <250> "CURAMT"
                    currentPeriod: currentGeneralLedgerPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: "N",
                    future: purchaseLedgerEntry.Future == true,
                    mvOperationType: null
                );

                supplierCurrencyEntry.PurchasesNextPeriod += supplierCurrencyEntryPurchasesNextPeriodAddition; // PLAMOUNT,N250

                var supplierCurrencyEntryPurchasesNextPeriodReduction = CalculatePurchaseLedgerAmount(
                    amounts: purchaseLedgerEntry.VATTaxesInCurrency, // <248> "VATTAX.CUR"
                    currentPeriod: currentGeneralLedgerPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: "N",
                    future: purchaseLedgerEntry.Future == true,
                    mvOperationType: "A"
                );

                supplierCurrencyEntry.PurchasesNextPeriod -= supplierCurrencyEntryPurchasesNextPeriodReduction; // PLAMOUNT,N248.A

                var supplierCurrencyEntryPurchasesThisPeriodAddition = CalculatePurchaseLedgerAmount(
                    amounts: new[] { purchaseLedgerEntry.AmountInCurrency }, // <250> "CURAMT"
                    currentPeriod: currentGeneralLedgerPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: null,
                    future: purchaseLedgerEntry.Future == true,
                    mvOperationType: null
                );

                supplierCurrencyEntry.PurchasesThisPeriod += supplierCurrencyEntryPurchasesThisPeriodAddition; // PLAMOUNT,250

                var supplierCurrencyEntryPurchasesThisPeriodReduction = CalculatePurchaseLedgerAmount(
                    amounts: purchaseLedgerEntry.VATTaxesInCurrency, // <248> "VATTAX.CUR"
                    currentPeriod: currentGeneralLedgerPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: null,
                    future: purchaseLedgerEntry.Future == true,
                    mvOperationType: "A"
                );

                supplierCurrencyEntry.PurchasesThisPeriod -= supplierCurrencyEntryPurchasesThisPeriodReduction; // PLAMOUNT,248.A

                var supplierCurrencyEntryPurchasesYearToDateAddition = CalculatePurchaseLedgerAmount(
                    amounts: new[] { purchaseLedgerEntry.AmountInCurrency }, // <250> "CURAMT"
                    currentPeriod: currentGeneralLedgerPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: "Y",
                    future: purchaseLedgerEntry.Future == true,
                    mvOperationType: null
                );

                supplierCurrencyEntry.PurchasesYearToDate += supplierCurrencyEntryPurchasesYearToDateAddition; // PLAMOUNT,Y250

                var supplierCurrencyEntryPurchasesYearToDateReduction = CalculatePurchaseLedgerAmount(
                    amounts: purchaseLedgerEntry.VATTaxesInCurrency, // <248> "VATTAX.CUR"
                    currentPeriod: currentGeneralLedgerPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: "Y",
                    future: purchaseLedgerEntry.Future == true,
                    mvOperationType: "A"
                );

                supplierCurrencyEntry.PurchasesYearToDate -= supplierCurrencyEntryPurchasesYearToDateReduction; // PLAMOUNT,Y248.A

                supplierCurrencyEntry.PeriodsInactive = purchaseLedgerEntry.Future == true ? null : (sbyte?)-1;



                // 07. UPDATE PLSUP

                var supplierCurrencyEntryJson = JsonConvert.SerializeObject(supplierCurrencyEntry);

                SupplierCurrencyFunctions.UpsertViaTransaction(ref session, supplierCurrencyEntryJson);

                results.Add(new PurchaseLedgerInsertionResult(
                    fileKey: supplierEntry.Key,
                    fileName: "PLSUP.CUR",
                    operationsPerformed: new[] {
                        $"<15> \"BALANCE\" (Balance) += {supplierCurrencyEntryBalanceAddition}",
                        $"<18,1> \"NEXT\" (NextPeriodBalance) += {supplierCurrencyEntryNextPeriodBalanceAddition}",
                        $"<18,2> \"CURR\" (ThisPeriodBalance) += {supplierCurrencyEntryThisPeriodBalanceAddition}",
                        $"<19> \"TLINK\" += \"{purchaseLedgerEntry.Key}\"",
                        $"<21,1> \"PNEXT\" (PurchasesNextPeriod) += {supplierCurrencyEntryPurchasesNextPeriodAddition}",
                        $"<21,1> \"PNEXT\" (PurchasesNextPeriod) -= {supplierCurrencyEntryPurchasesNextPeriodReduction}",
                        $"<21,2> \"PCURR\" (PurchasesThisPeriod) += {supplierCurrencyEntryPurchasesThisPeriodAddition}",
                        $"<21,2> \"PCURR\" (PurchasesThisPeriod) -= {supplierCurrencyEntryPurchasesThisPeriodReduction}",
                        $"<21,3> \"PYTD\" (PurchasesYearToDate) += {supplierCurrencyEntryPurchasesYearToDateAddition}",
                        $"<21,3> \"PYTD\" (PurchasesYearToDate) -= {supplierCurrencyEntryPurchasesYearToDateReduction}",
                        $"<23> \"INACT\" (PeriodsInactive) = \"{supplierCurrencyEntry.PeriodsInactive}\""
                    },
                    jsonBefore: supplierCurrencyEntryBeforeModificationJSON,
                    jsonAfterwards: supplierCurrencyEntryJson,
                    purchaseLedgerRegistryKey: registerEntry.Key
                ));

                #endregion

                #region Step 08. Update the relevant CFVAT record(s), for each of the VAT codes declared on the given invoice registry entry (UPDATE(s) to CFVAT)

                foreach (var vatEntry in companyVATEntries)
                {
                    var vatEntryBeforeModificationJSON = JsonConvert.SerializeObject(companyVATEntries);

                    var purchasesInvoicesTaxAddition = CalculatePurchaseLedgerAmount(
                        amounts: purchaseLedgerEntry.VATTaxes, // <48> "VATTAX"
                        currentPeriod: currentGeneralLedgerPeriod,
                        numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                        operationPeriod: null,
                        future: purchaseLedgerEntry.Future == true,
                        mvOperationType: "X"
                    );

                    vatEntry.PurchasesInvoicesTax += purchasesInvoicesTaxAddition; // PLAMOUNT,48X

                    var purchasesInvoicesValueAddition = CalculatePurchaseLedgerAmount(
                        amounts: purchaseLedgerEntry.VATableValues, // <49> "VATVALUE"
                        currentPeriod: currentGeneralLedgerPeriod,
                        numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                        operationPeriod: null,
                        future: purchaseLedgerEntry.Future == true,
                        mvOperationType: "X"
                    );

                    vatEntry.PurchasesInvoicesValue += purchasesInvoicesValueAddition; // PLAMOUNT,48X

                    var purchasesQuarterTaxTotalAddition = CalculatePurchaseLedgerAmount(
                        amounts: purchaseLedgerEntry.VATTaxes, // <48> "VATTAX"
                        currentPeriod: currentGeneralLedgerPeriod,
                        numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                        operationPeriod: null,
                        future: purchaseLedgerEntry.Future == true,
                        mvOperationType: "X"
                    );

                    vatEntry.PurchasesQuarterTaxTotal += purchasesQuarterTaxTotalAddition; // PLAMOUNT,48X

                    var purchasesQuarterValueTotalAddition = CalculatePurchaseLedgerAmount(
                        amounts: purchaseLedgerEntry.VATableValues, // <49> "VATVALUE"
                        currentPeriod: currentGeneralLedgerPeriod,
                        numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                        operationPeriod: null,
                        future: purchaseLedgerEntry.Future == true,
                        mvOperationType: "X"
                    );

                    vatEntry.PurchasesQuarterValueTotal += purchasesQuarterValueTotalAddition; // PLAMOUNT,48X

                    var purchasesNextPeriodInvoicesTaxAddition = CalculatePurchaseLedgerAmount(
                        amounts: purchaseLedgerEntry.VATTaxes, // <48> "VATTAX"
                        currentPeriod: currentGeneralLedgerPeriod,
                        numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                        operationPeriod: "N",
                        future: purchaseLedgerEntry.Future == true,
                        mvOperationType: "X"
                    );

                    vatEntry.PurchasesNextPeriodInvoicesTax += purchasesNextPeriodInvoicesTaxAddition; // PLAMOUNT,N48X

                    var purchasesNextPeriodInvoicesValueAddition = CalculatePurchaseLedgerAmount(
                        amounts: purchaseLedgerEntry.VATableValues, // <49> "VATVALUE"
                        currentPeriod: currentGeneralLedgerPeriod,
                        numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                        operationPeriod: "N",
                        future: purchaseLedgerEntry.Future == true,
                        mvOperationType: "X"
                    );

                    vatEntry.PurchasesNextPeriodInvoicesValue += purchasesNextPeriodInvoicesValueAddition; // PLAMOUNT,N48X

                    var vatEntryJson = JsonConvert.SerializeObject(vatEntry);

                    CompanyVATFunctions.UpsertViaTransaction(ref session, vatEntryJson);

                    results.Add(new PurchaseLedgerInsertionResult(
                        fileKey: vatEntry.Key,
                        fileName: "CFVAT",
                        operationsPerformed: new[] {
                            $"<30> \"PINVT\" (PurchasesInvoicesTax) += {purchasesInvoicesTaxAddition}",
                            $"<34> \"PINVV\" (PurchasesInvoicesValue) += {purchasesInvoicesValueAddition}",
                            $"<38> \"PQTTOT\" (PurchasesQuarterTaxTotal) += {purchasesQuarterTaxTotalAddition}",
                            $"<39> \"PQVTOT\" (PurchasesQuarterValueTotal) += {purchasesQuarterValueTotalAddition}",
                            $"<40> \"PNINVT\" (PurchasesNextPeriodInvoicesTax) += {purchasesNextPeriodInvoicesTaxAddition}",
                            $"<44> \"PNINVV\" (PurchasesNextPeriodInvoicesValue) += {purchasesNextPeriodInvoicesValueAddition}", },
                        jsonBefore: vatEntryBeforeModificationJSON,
                        jsonAfterwards: vatEntryJson,
                        purchaseLedgerRegistryKey: registerEntry.Key
                    ));
                }

                #endregion

                #region Step 09. Create a Purchase Ledger VAT Audit entry (INSERT to PLVATAUD)

                /* Insert the PurchaseLedgerVATAudit record */

                // Key == TPLEDG<53> "Document Reference"
                var purchaseLedgerVATAuditEntry = new PurchaseLedgerVATAudit(purchaseLedgerEntry.DocumentReference)
                {
                    DocumentDT = purchaseLedgerEntry.DocumentDT,
                    TransactionType = "INV",
                    SupplierCode = purchaseLedgerEntry.SupplierCode,
                    PurchaseLedgerTotalDueInSterling = purchaseLedgerEntry.TotalDueInSterling,
                    VATCodes = purchaseLedgerEntry.VATCodes,
                    VATableValuesInSterling = purchaseLedgerEntry.VATableValues,
                    VATTaxesInSterling = purchaseLedgerEntry.VATTaxes,
                    NextPeriod = purchaseLedgerEntry.Future == true
                };

                var purchaseLedgerVATAuditEntryJson = JsonConvert.SerializeObject(purchaseLedgerVATAuditEntry);

                PurchaseLedgerVATAuditFunctions.UpsertViaTransaction(ref session, purchaseLedgerVATAuditEntryJson);

                results.Add(new PurchaseLedgerInsertionResult(
                    fileKey: purchaseLedgerVATAuditEntry.Key,
                    fileName: "PLVATAUD",
                    operationsPerformed: new[] { "INSERT" },
                    jsonBefore: null,
                    jsonAfterwards: purchaseLedgerVATAuditEntryJson,
                        purchaseLedgerRegistryKey: registerEntry.Key
                ));

                #endregion

                #region Step 10. Update the Company Statistics record (UPDATE to CFSTATS)

                var companyStatsEntryBeforeModificationJSON = JsonConvert.SerializeObject(companyStatsEntry);

                var companyStatsEntryPurchaseLedgerInvoicesAddition = CalculatePurchaseLedgerAmount(
                    amounts: new[] { purchaseLedgerEntry.TotalDueInSterling },
                    currentPeriod: currentGeneralLedgerPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: null,
                    future: purchaseLedgerEntry.Future == true,
                    mvOperationType: null
                );

                companyStatsEntry.PurchaseLedgerInvoices += companyStatsEntryPurchaseLedgerInvoicesAddition;

                var companyStatsEntryNextPurchaseLedgerInvoicesAddition = CalculatePurchaseLedgerAmount(
                    amounts: new[] { purchaseLedgerEntry.TotalDueInSterling },
                    currentPeriod: currentGeneralLedgerPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: "N",
                    future: purchaseLedgerEntry.Future == true,
                    mvOperationType: null
                );

                companyStatsEntry.NextPurchaseLedgerInvoices += companyStatsEntryNextPurchaseLedgerInvoicesAddition;

                var companyStatsEntryJson = JsonConvert.SerializeObject(companyStatsEntry);

                CompanyStatsFunctions.UpsertViaTransaction(ref session, companyStatsEntryJson);

                results.Add(new PurchaseLedgerInsertionResult(
                    fileKey: companyStatsEntry.Key,
                    fileName: "CFSTATS",
                    operationsPerformed: new string[] {
                        $"<11> \"PLINV\" (PurchaseLedgerInvoices) += {companyStatsEntryPurchaseLedgerInvoicesAddition}",
                        $"<21> \"NPLINV\" (NextPurchaseLedgerInvoices) += {companyStatsEntryNextPurchaseLedgerInvoicesAddition}"
                    },
                    jsonBefore: companyStatsEntryBeforeModificationJSON,
                    jsonAfterwards: companyStatsEntryJson,
                    purchaseLedgerRegistryKey: registerEntry.Key
                ));

                #endregion

                #region Step 11. Create a General Ledger Batch entry (INSERT to GLBATCH)

                // Key == TPLEDG<53> "Document Reference"
                var generalLedgerBatchEntry = new GeneralLedgerBatch(purchaseLedgerEntry.DocumentReference)
                {
                    TransactionType = "RFPOST",
                    DataKeys = purchaseLedgerEntry.DocumentReferences,
                    NumberOfRecords = 1
                };

                var generalLedgerBatchEntryJson = JsonConvert.SerializeObject(generalLedgerBatchEntry);

                GeneralLedgerBatchFunctions.UpsertViaTransaction(ref session, generalLedgerBatchEntryJson);

                results.Add(new PurchaseLedgerInsertionResult(
                    fileKey: generalLedgerBatchEntry.Key,
                    fileName: "GLBATCH",
                    operationsPerformed: new[] { "INSERT" },
                    jsonBefore: null,
                    jsonAfterwards: generalLedgerBatchEntryJson,
                        purchaseLedgerRegistryKey: registerEntry.Key
                ));

                #endregion

                #region Step 12. Create a General Ledger Batch Data entry (INSERT to GLBATCHDATA)

                // Key == TPLEDG<53> "Document Reference"
                var generalLedgerBatchDataEntry = new GeneralLedgerBatchData(purchaseLedgerEntry.DocumentReference)
                {
                    Type = "RFPOST",
                    DocumentNumber = purchaseLedgerEntry.DocumentReference,
                    Prefix = "P",
                    DocumentDT = purchaseLedgerEntry.DocumentDT,
                    PostDT = purchaseLedgerEntry.PostDT,
                    SupplierCode = purchaseLedgerEntry.SupplierCode,
                    GeneralLedgerCodes = purchaseLedgerEntry.GeneralLedgerCodes,
                    VATTax = purchaseLedgerEntry.VATTaxes.Sum(),
                    DocumentTotal = purchaseLedgerEntry.TotalDueInSterling,
                    SupplierName = supplierEntry.Name,
                    TransactionType = "PINV",
                    GeneralLedgerSubCodes = purchaseLedgerEntry.GeneralLedgerSubCodes,
                    Memo = purchaseLedgerEntry.Memo,
                    OtherMemos = purchaseLedgerEntry.OtherMemos,
                    ItemReference = purchaseLedgerEntry.SupplierInvoiceReference
                };

                // 12. INSERT to GLBATCHDATA

                var generalLedgerBatchDataEntryJson = JsonConvert.SerializeObject(generalLedgerBatchDataEntry);

                GeneralLedgerBatchDataFunctions.UpsertViaTransaction(ref session, generalLedgerBatchDataEntryJson);

                results.Add(new PurchaseLedgerInsertionResult(
                    fileKey: generalLedgerBatchDataEntry.Key,
                    fileName: "GLBATCHDATA",
                    operationsPerformed: new[] { "INSERT" },
                    jsonBefore: null,
                    jsonAfterwards: generalLedgerBatchDataEntryJson,
                        purchaseLedgerRegistryKey: registerEntry.Key
                ));

                #endregion

                #region Step 13. Create a Purchase Ledger Adjustment Audit entry (INSERT to PLADJAUDIT)

                // Key == $"*{TPLEDG<53>}";
                var purchaseLedgerAdjustmentAuditEntry = new PurchaseLedgerAdjustmentAudit($"*{purchaseLedgerEntry.DocumentReference}")
                {
                    DocumentTotal = purchaseLedgerEntry.TotalDueInSterling,
                    TransactionType = "INV",
                    DocumentDT = purchaseLedgerEntry.DocumentDT,
                    PostDT = now,
                    VATTaxesTotalInSterling = purchaseLedgerTransactionEntry.VATTaxesTotal,
                    SupplierCode = purchaseLedgerEntry.SupplierCode,
                    Balances = new[] { purchaseLedgerTransactionEntry.Balance },
                    GeneralLedgerCodes = purchaseLedgerEntry.GeneralLedgerCodes,
                    Discounts = new[] { purchaseLedgerTransactionEntry.DiscountAmount },
                    GeneralLedgerAmounts = purchaseLedgerEntry.Amounts,
                    Ports = new[] { session.HostPort },
                    Time = now.ToString("HH:mm"),
                    Dispute = purchaseLedgerEntry.DisputeIndicator,
                    DueDT = purchaseLedgerEntry.InvoiceDueDT,
                    CurrencyCode = purchaseLedgerEntry.CurrencyCode,
                    VATTaxesTotalInCurrency = purchaseLedgerTransactionCurrencyEntry.VATTaxesTotal,
                    DocumentTotalInCurrency = purchaseLedgerEntry.AmountInCurrency,
                    ExchangeRates = purchaseLedgerEntry.ExchangeRates
                };

                var purchaseLedgerAdjustmentAuditEntryJson = JsonConvert.SerializeObject(purchaseLedgerAdjustmentAuditEntry);

                PurchaseLedgerAdjustmentAuditFunctions.UpsertViaTransaction(ref session, purchaseLedgerAdjustmentAuditEntryJson);

                results.Add(new PurchaseLedgerInsertionResult(
                    fileKey: purchaseLedgerAdjustmentAuditEntry.Key,
                    fileName: "PLADJAUDIT",
                    operationsPerformed: new[] { "INSERT" },
                    jsonBefore: null,
                    jsonAfterwards: purchaseLedgerAdjustmentAuditEntryJson,
                        purchaseLedgerRegistryKey: registerEntry.Key
                ));

                #endregion

                #region Step 14. Create a Purchase Ledger Supplier Invoice entry (INSERT into PLSUPINV)

                // Key == TPLEDG<39>*TPLEDG<37>
                var purchaseLedgerSupplierInvoiceEntry = new PurchaseLedgerSupplierInvoice($"{purchaseLedgerEntry.SupplierCode}*{purchaseLedgerEntry.SupplierInvoiceReference}")
                {
                    SupplierCode = purchaseLedgerEntry.SupplierCode,
                    SupplierInvoiceReference = purchaseLedgerEntry.SupplierInvoiceReference,
                    TransactionType = "INV",
                    DocumentDT = purchaseLedgerEntry.DocumentDT,
                    DocumentNumber = purchaseLedgerEntry.DocumentReference,
                    PostDT = purchaseLedgerEntry.PostDT
                };

                var purchaseLedgerSupplierInvoiceEntryJson = JsonConvert.SerializeObject(purchaseLedgerSupplierInvoiceEntry);

                results.Add(new PurchaseLedgerInsertionResult(
                    fileKey: purchaseLedgerSupplierInvoiceEntry.Key,
                    fileName: "PLSUPINV",
                    operationsPerformed: new[] { "INSERT" },
                    jsonBefore: null,
                    jsonAfterwards: purchaseLedgerSupplierInvoiceEntryJson,
                        purchaseLedgerRegistryKey: registerEntry.Key
                ));

                PurchaseLedgerSupplierInvoiceFunctions.UpsertViaTransaction(ref session, purchaseLedgerSupplierInvoiceEntryJson);

                #endregion

                #region Step 15. Create the necessary GLPPTRANS.DES entries (if any of the provided GL codes are PrePayment codes)

                // This is a bit messy: the data in GLPPTRANS.DES (even very recent data) contravenes the dictionary entries for the fields
                // all I can hope to do is faithfully follow what the transcation definition is attempting to do, so I shall.

                var generalLedgerPrepaymentTransactionEntries = new List<GeneralLedgerPrePaymentTransaction>();

                var generalLedgerControlFile = session.CreateUniFile("GLCONTROL");
                
                var generalLedgerControlRecordPrePaymentFieldPosition = 6;

                for (var i = 0; i < purchaseLedgerEntry.GeneralLedgerCodes.Count(); i++)
                {
                    var glc = purchaseLedgerEntry.GeneralLedgerCodes.ElementAt(i);

                    if (prePaymentPurchaseLedgerCodes.Contains(glc))
                    {
                        throw new Exception($"This process does not yet handle Prepayment General Ledger Codes. Saw GL Code: \"{glc}\"");

                        // Create a GLPPTRANS.DES record for this GLC.
                        // Must have a new, unique key - the largest key in GLPPTRANS.CUR file, incremented by one.

                        // Nope, bollocks == this executes PG.BLD.GLPPTRANS.KEY,PIR to build key(s) for this stage of the transaction
                        // That no doubt ends up calling RFGENKEY and reading PLCONTROL "GENNO" - but which field position?

                        // Yup, that's at line 34 of the PL paragraph "PG.BLD.GLPPTRANS.KEY"

                        var generalLedgerControlNumberGeneratorRecords = generalLedgerControlFile.ReadRecords(new[] { "GENNO" });

                        var generalLedgerControlNumberGeneratorRecord = generalLedgerControlNumberGeneratorRecords.GetRecord(0);

                        var generalLedgerPrepaymentNumberField = numberGeneratorRecord.Record.Extract(generalLedgerControlRecordPrePaymentFieldPosition); // Field 9 on the record "GENNO"

                        var generalLedgerPrepaymentCurrentKey = Convert.ToInt32(generalLedgerPrepaymentNumberField.StringValue);

                        var generalLedgerPrepaymentNewKey = generalLedgerPrepaymentCurrentKey += 1;

                        generalLedgerControlNumberGeneratorRecord.Record.Replace(generalLedgerControlRecordPrePaymentFieldPosition, $"{generalLedgerPrepaymentNewKey}");

                        generalLedgerControlFile.WriteRecords(generalLedgerControlNumberGeneratorRecords);

                        var nextGeneralLedgerPrepaymentKey = $"{generalLedgerPrepaymentNewKey}".PadLeft(8, '0');

                        var prePaymentAmountSplits = purchaseLedgerEntry.PrepaymentSplitAmounts.ElementAtOrDefault(i);

                        var prePaymentMemoSplits = purchaseLedgerEntry.PrepaymentSplitMemos.ElementAtOrDefault(i);

                        var prePaymentMemo = purchaseLedgerEntry.PrepaymentMemos.ElementAtOrDefault(i);

                        var otherPrePaymentMemo = purchaseLedgerEntry.OtherPrepaymentMemos.ElementAtOrDefault(i);

                        var generalLedgerPrepaymentTransactionEntry = new GeneralLedgerPrePaymentTransaction(nextGeneralLedgerPrepaymentKey)
                        {
                            SupplierCode = purchaseLedgerEntry.SupplierCode,
                            GeneralLedgerCodes = new string[] { glc },
                            AmountTotal = purchaseLedgerEntry.Amounts.ElementAt(i),
                            DocumentNumber = purchaseLedgerEntry.DocumentReference,
                            TransactionType = "INV",
                            DocumentDT = purchaseLedgerEntry.DocumentDT,
                            DueDT = purchaseLedgerEntry.InvoiceDueDT,
                            StartPeriod = purchaseLedgerEntry.PrepaymentStartPeriods.ElementAt(i),
                            NumberOfPeriods = purchaseLedgerEntry.PrepaymentNumberOfPeriods.ElementAt(i),
                            SplitEqually = purchaseLedgerEntry.PrepaymentsSplitEqually.ElementAt(i),
                            SplitAmounts = prePaymentAmountSplits?.Split(Framework.SubValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries).Select(a => Convert.ToDecimal(a)),
                            SplitMemos = prePaymentMemoSplits?.Split(Framework.SubValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries),
                            Memo = prePaymentMemo ?? otherPrePaymentMemo,
                        };

                        generalLedgerPrepaymentTransactionEntries.Add(generalLedgerPrepaymentTransactionEntry); 
                    }
                }

                foreach(var glppt in generalLedgerPrepaymentTransactionEntries)
                {
                    var glpptJson = JsonConvert.SerializeObject(glppt);

                    // 15. INSERT into GLPPTRANS.DES

                    results.Add(new PurchaseLedgerInsertionResult(
                        fileKey: glppt.Key,
                        fileName: "GLPPTRANS.DES",
                        operationsPerformed: new[] { "INSERT" },
                        jsonBefore: null,
                        jsonAfterwards: glpptJson,
                        purchaseLedgerRegistryKey: registerEntry.Key
                    ));

                    GeneralLedgerPrePaymentTransactionFunctions.UpsertViaTransaction(ref session, glpptJson);
                }

                #endregion

                transaction.Commit();
            }

            return new ArrayList(results);
        }

        public static void PurchaseLedger_InsertFromInvoiceRegistry_FillRow(
            object obj,
            out SqlString PurchaseLedgerInvoiceRegistryKey,
            out SqlString FileKey,
            out SqlString FileName,
            out SqlString OperationsPerformed,
            out SqlString JSONBefore,
            out SqlString JSONAfterwards
            )
        {
            var p = (PurchaseLedgerInsertionResult)obj;

            FileKey = p.FileKey;

            FileName = p.FileName;

            OperationsPerformed = p.OperationsPerformed?.Any() == true ? JsonConvert.SerializeObject(p.OperationsPerformed) : SqlString.Null;

            JSONBefore = p.JSONBefore ?? SqlString.Null;

            JSONAfterwards = p.JSONAfterwards ?? SqlString.Null;

            PurchaseLedgerInvoiceRegistryKey = p.PurchaseLedgerRegistryKey;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.Tracking_Consignments
{
    // This is rather annoying:
    // OEDNHHIST is used to house consignments as well as invoiced despatch heads
    // e.g. LIST OEDNHHIST OECONHEADKEY 1167340501-1
    // will give you ConsignmentID 628241
    // Then you want to take the OECONHEADKEY and read OEDNHHIST again, and you can ask for TOELINEP and OECONLINEKEY
    // LIST OEDNHHIST TOELINEP OECONLINEKEY 628241
    // will give you
    // 1167340501*1     628241*1
    // 1167340501*2     628241*2
    // 1167340601*1     628241*3
    // 1167340601*2     628241*4
    // You then want to take the OECONLINEKEY values and ask OEDNLHIST for ORIG.PCL.TDW
    // LIST OEDNLHIST ORIG.PCL.TDW 628241*1 628241*2 628241*3 628241*4
    // will give you
    // 628241*1     0002114261
    // 628241*2     0002114261
    // 628241*3     0002114260
    // 628241*4     0002114260
    // Then you can take those parcel numbers to... somewhere else?


    /*
     
    Oooookay 

    LIST OEDNLHIST PARCEL ORIG.PARCEL.TDW 1167340501-1*1

    The original picking list line refers to both the final big parcel and the original parcel 

    The final big parcel actually exists by that reference on OEFINDPARCEL (but you have to look by PARCEL.NO "Internal Parcel Number"!)

    The original parcel (ORIG.PARCEL.TDW) you need to prefix with INT/ to find the record 

    SoooooooooOOOOOOoooooooooo

    We... should be reading... ORIG.PARCEL.TDW and if it exists we should be... well no because... 
    
     
     */

    public class Consignment
    {
        public string ConsignmentID { get; set; }

        public IEnumerable<ConsignmentLine> ConsignmentLines { get; set; }
    }
}

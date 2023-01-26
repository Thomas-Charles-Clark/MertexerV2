using MertexerV2.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MertexerV2.Utilities;

namespace MertexerV2.Controllers
{
    public class AllocationController : QueryExecutor
    {
        /// <summary>
        /// In Debug mode, connects to PLAY. In Release, connects to MERTEX.
        /// </summary>
        /// <param name="database">Manual UV Account specification.</param>
#if DEBUG
        public AllocationController(string database = "PLAY") : base(database)
#else
        public AllocationController(string database = "MERTEX") : base(database)
#endif
        {

        }

        ~AllocationController()
        {
            base.Dispose(false);
        }

        /// <summary>
        /// This routine is used to create a new Pre Allocation for a given Product and Location. Optionally you may specify a specific Batch and / or Piece to allocate.
        /// </summary>
        /// <param name="preallocationId">A unique ID to be used as the record key for this pre allocation</param>
        /// <param name="productCode">The Mertex Product Code</param>
        /// <param name="locationCode">The Mertex Location Code</param>
        /// <param name="quantity">The quantity to be pre allocated</param>
        /// <param name="unit">The unit of stock</param>
        /// <param name="batch">A Mertex Batch Reference (optional)</param>
        /// <param name="piece">A Mertex Piece Number (optional)</param>
        /// <param name="expiryDate">Expiry Date in DD/MM/YYYY format</param>
        /// <param name="expiryTime">Expiry Time in HH:MM:SS format</param>
        /// <param name="reserveIndicator">Reserve Indicator: either R, C, null or empty string.</param>
        /// <returns>
        /// Expected results:
        /// If the routine is successful the ReturnFlag property will be set to zero.
        /// The Result property will return the Batch and Piece used to perform the Pre Allocation as structured XML:
        /// <PRE_ALLOC_DETAILS>
        /// <BATCH>NA</BATCH>
        /// <PIECE>B044709</PIECE>
        /// </PRE_ALLOC_DETAILS>
        /// The Batch and Piece are required if you want to update the Pre Allocation using the UPDATE_PREALLOC routine.
        /// A new record will be created on WTASPPREALLOCS keyed on the Pre Allocation ID provided.You can view this using the Pre Allocation Enquiry screen at:
        /// /WIMAN > Pre Allocation Enquiry
        /// A new entry for the Pre Allocation will be added to the allocation table on the Batch or Piece.The demands on the Product, Location, Batch and Piece(if applicable) will be updated as appropriate.
        /// You can view the pre allocation using the standard Mertex enquiries, for example for Piece level product:
        /// /PB > F5 View Batches > F8 Batch Items > F5 Full Item Details > F9 Toggle > F6 Allocation
        /// Designers Guild Web Integration Guide v1.1 Page: 25
        /// If the routine is not successful then the ReturnFlag property will not be zero, please see the Return Flags table that follows for an explanation of each possible ReturnFlag value.The Result property will contain this value.
        ///
        /// Return flags:
        /// Flag            Description
        /// 0               Success
        /// 100             No Pre Allocation ID provided
        /// 101             Pre Allocation ID already in use
        /// 102             Different Number of Batches and Pieces sent
        /// 103             Could not create Pre Allocation
        /// 111             Invalid Date sent, format must be DD/MM/YYYY
        /// 112             Invalid Time sent, format must be HH:MM:SS
        /// 113             Product is mandatory
        /// 114             Location is mandatory
        /// 115             Quantity must be numeric
        /// 119             Expiry Date and Time must not be in the past
        /// 199             Unrecognised option passed to Pre Allocation Routine
        /// 
        /// </returns>
        public string CreatePreAllocation(
            string preallocationId,
            string productCode,
            string locationCode,
            string quantity,
            string unit,
            string batch,
            string piece,
            string expiryDate,
            string expiryTime,
            string reserveIndicator,
            string softAllocated)
        {
            int foundCount = 0;
            int counter = 0;

            return this.Run(uvFileName: "CREATE_PREALLOC",
                uvSelectStatement: "",
                counter: ref counter,
                foundCount: ref foundCount,
                id: preallocationId,
                fieldsToReturn: "",
                webData: $@"{preallocationId}
{productCode}
{locationCode}
{quantity}
{unit}
{batch}
{piece}
{expiryDate}
{expiryTime}
{reserveIndicator}
{softAllocated}",
                returnType: "XML");
        }

        /// <summary>
        /// Use this routine to update an existing Pre Allocation.
        /// 
        /// Update scenarios:
        /// When updating a pre allocation the following scenarios apply:
        /// Change of Product and / or Location -
        /// A new pre allocation is attempted for the new Product and / or Location.If this succeeds then then the original pre allocation is removed.
        /// Change of Quantity and / or Unit -
        /// The pre allocation is changed as long as the available stock on the batch or piece can accommodate the new quantity and / or unit.
        /// Change of Batch and / or Piece -
        /// A new pre allocation is attempted for the new Batch and / or Piece.If this succeeds then the original pre allocation is removed.
        /// Change of Expiry Date and / or Time -
        /// No change is made to the allocated stock.
        /// The Expiry Date and / or Time are updated.
        /// The new Date and Time must not be in the past.
        /// 
        /// </summary>
        /// <param name="preallocationId">The ID of the Pre Allocation record to update</param>
        /// <param name="productCode">The Mertex Product Code</param>
        /// <param name="locationCode">The Mertex Location Code</param>
        /// <param name="quantity">The quantity to be pre allocated</param>
        /// <param name="unit">The unit of stock</param>
        /// <param name="batch">A Mertex Batch Reference (optional)</param>
        /// <param name="piece">A Mertex Piece Number (optional)</param>
        /// <param name="expiryDate">Expiry Date in DD/MM/YYYY format</param>
        /// <param name="expiryTime">Expiry Time in HH:MM:SS format</param>
        /// <returns>
        /// Expected results:
        /// If the routine is successful the ReturnFlag property will be set to zero.
        /// The Result property will return the Batch and Piece used to perform the Pre Allocation as structured XML:
        /// <PRE_ALLOC_DETAILS>
        /// <BATCH>NA</BATCH>
        /// <PIECE>B044709</PIECE>
        /// </PRE_ALLOC_DETAILS>
        /// The Batch and Piece are required if you want to update the Pre Allocation using the UPDATE_PREALLOC routine.
        /// The appropriate record on WTASPPREALLOCS will be updated.You can view this using the Pre Allocation Enquiry screen at:
        /// Designers Guild Web Integration Guide v1.1 Page: 28
        /// /WIMAN > Pre Allocation Enquiry
        /// The entry for the Pre Allocation will be updated in the allocation table on the Batch or Piece.The demands on the Product, Location, Batch and Piece (if applicable) will be updated as appropriate.
        /// You can view the pre allocation using the standard Mertex enquiries, for example for Piece level product:
        /// /PB > F5 View Batches > F8 Batch Items > F5 Full Item Details > F9 Toggle > F6 Allocation
        /// If the routine is not successful then the ReturnFlag property will not be zero, please see the Return Flags table that follows for an explanation of each possible ReturnFlag value.The Result property will contain this value.
        /// Return flags:
        /// Flag        Description
        /// 0           Success
        /// 100         No Pre Allocation ID provided
        /// 102         Different Number of Batches and Pieces sent
        /// 104         Pre Allocation record not found
        /// 105         No details to update
        /// 106         Could not update Pre Allocation
        /// 111         Invalid Date sent, format must be DD/MM/YYYY
        /// 112         Invalid Time sent, format must be HH:MM:SS
        /// 113         Product is mandatory
        /// 114         Location is mandatory
        /// 115         Quantity must be numeric
        /// 119         Expiry Date and Time must not be in the past
        /// 199         Unrecognised option passed to Pre Allocation Routine
        /// </returns>
        public string UpdatePreAllocation(
            string preallocationId,
            string productCode,
            string locationCode,
            string quantity,
            string unit,
            string batch,
            string piece,
            string expiryDate,
            string expiryTime,
            string reserveIndicator,
            string softAllocated)
        {
            int foundCount = 0;
            int counter = 0;

            return this.Run(uvFileName: "UPDATE_PREALLOC",
                uvSelectStatement: "",
                counter: ref counter,
                foundCount: ref foundCount,
                id: preallocationId,
                fieldsToReturn: "",
                webData: $@"{preallocationId}
{productCode}
{locationCode}
{quantity}
{unit}
{batch}
{piece}
{expiryDate}
{expiryTime}
{reserveIndicator}
{softAllocated}",
                returnType: "XML");
        }

        /// <summary>
        /// Use this routine to split an existing Pre Allocation into two or more Pre Allocations.
        /// This routine will only perform a split within the previously pre allocated Batch and/or Piece, and the overall quantity pre allocated before the split must be the same after the split.
        /// </summary>
        /// <param name="preallocationIds">A pipe delimited list of Pre Allocation IDs. The first ID is the original pre allocation to split; subsequent IDs are the IDs to use for the new Pre Allocations to be created because of the split.</param>
        /// <param name="quantities">The quantity to be set on each pre allocation after the split, pipe delimited.</param>
        /// <param name="expiryDates">Expiry Date to be used for each pre allocation after the split in DD/MM/YYYY format, pipe delimited.</param>
        /// <param name="expiryTimes">Expiry Time to be used for each pre allocation after the split in HH:MM:SS format, pipe delimited.</param>
        /// <returns>
        /// Expected results:
        /// If the routine is successful the ReturnFlag property will be set to zero.At completion of this routine a record will exist on WTASPPREALLOCS for each Pre Allocation ID provided when the routine was called. The record with the first Pre Allocation ID will be used as the template for the other records.Each record will show the appropriate Quantity and Expiry Date and Time.
        /// You can view the records using the Pre Allocation Enquiry screen at:
        /// /WIMAN > Pre Allocation Enquiry
        /// The entry for the first Pre Allocation will be updated in the allocation table on the Batch or Piece, and additional entries for each Pre Allocation ID will be added with the appropriate quantity.The overall demands on the Product, Location, Batch and Piece(if applicable) will not be affected.
        /// You can view the pre allocation using the standard Mertex enquiries, for example for Piece level product:
        /// /PB > F5 View Batches > F8 Batch Items > F5 Full Item Details > F9 Toggle > F6 Allocation
        /// If the routine is not successful then the ReturnFlag property will not be zero, please see the Return Flags table that follows for an explanation of each possible ReturnFlag value.The Result property will contain this value.
        /// Designers Guild Web Integration Guide v1.1 Page: 31
        /// Return flags:
        /// Flag        Description
        /// 0           Success
        /// 100         No Pre Allocation ID provided
        /// 101         Pre Allocation ID to be used after split already in use
        /// 104         Pre Allocation record not found
        /// 107         Different number of Pre Allocation IDs and Quantities sent
        /// 108         Different number of Pre Allocation IDs and Expiry Dates sent
        /// 109         Different number of Expiry Dates and Expiry Times sent
        /// 110         Total of Quantities to be split does to match Original Quantity
        /// 111         Invalid Date sent, format must be DD/MM/YYYY
        /// 112         Invalid Time sent, format must be HH:MM:SS
        /// 115         Quantity must be numeric
        /// 116         Could not update Pre Allocation record after split/merge
        /// 117         Unable to complete split/merge
        /// 119         Expiry Date and Time must not be in the past
        /// 199         Unrecognised option passed to Pre Allocation Routine
        /// </returns>
        public string SplitPreAllocation (
            string preallocationIds,
            string quantities,
            string expiries)
        {
            var arr = expiries.Split(new char[] { '|' }).ToList();

            var expiryDatesList = new List<string>();
            var expiryTimesList = new List<string>();

            for (var i = 0; i < arr.Count; i++)
            {
                var dt = DateTime.ParseExact(arr[i], "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                expiryDatesList.Add(dt.ToString("dd/MM/yyyy"));
                expiryTimesList.Add(dt.ToString("HH:mm:ss"));
            }

            var expiryDates = String.Join("|", expiryDatesList);
            var expiryTimes = String.Join("|", expiryTimesList);

            return this.Run(uvFileName: "SPLIT_PREALLOC",
                uvSelectStatement: "",
                fieldsToReturn: "",
                webData: $@"{preallocationIds}
{quantities}
{expiryDates}
{expiryTimes}",
                returnType: "XML");
        }

        /// <summary>
        /// Use this routine to merge two existing Pre Allocations into a single Pre Allocation.
        /// This routine will only merge within the previously pre allocated Batch and/or Piece, and the overall quantity pre allocated before the merge must be the same after the merge.
        /// </summary>
        /// <param name="preallocationIds">A pipe delimited list of Pre Allocation IDs to merge.</param>
        /// <param name="quantities">The quantity to be set on each pre allocation after the merge, pipe delimited. Set to zero to fully merge a pre allocation and remove it.</param>
        /// <param name="expiryDates">Expiry Date to be used for each pre allocation after the merge in DD/MM/YYYY format, pipe delimited. An Expiry Date is not required if the quantity is zero.</param>
        /// <param name="expiryTimes">Expiry Time to be used for each pre allocation after the merge in HH:MM:SS format, pipe delimited. An Expiry Date is not required if the quantity is zero.</param>
        /// <returns>
        /// Expected results:
        /// If the routine is successful the ReturnFlag property will be set to zero.At completion of this routine a record will exist on WTASPPREALLOCS for each Pre Allocation ID with a quantity > 0 provided when the routine was called. The record with the first Pre Allocation ID will be used as the template for the other records.Each record will show the appropriate Quantity and Expiry Date and Time. Any Pre Allocation records where the quantity is zero will be removed.
        /// You can view the records using the Pre Allocation Enquiry screen at:
        /// /WIMAN > Pre Allocation Enquiry
        /// The entry for the first Pre Allocation will be updated in the allocation table on the Batch or Piece, and additional entries for each Pre Allocation ID will either be updated with the appropriate quantity or removed as appropriate.The overall demands on the Product, Location, Batch and Piece(if applicable) will not be affected.
        /// You can view the pre allocation using the standard Mertex enquiries, for example for Piece level product:
        /// /PB > F5 View Batches > F8 Batch Items > F5 Full Item Details > F9 Toggle > F6 Allocation
        /// If the routine is not successful then the ReturnFlag property will not be zero, please see the Return Flags table that follows for an explanation of each possible ReturnFlag value.The Result property will contain this value.
        /// Designers Guild Web Integration Guide v1.1 Page: 34
        /// Return flags:
        /// Flag        Description
        /// 0           Success
        /// 100         No Pre Allocation ID provided
        /// 104         Pre Allocation record not found
        /// 107         Different number of Pre Allocation IDs and Quantities sent
        /// 108         Different number of Pre Allocation IDs and Expiry Dates sent
        /// 109         Different number of Expiry Dates and Expiry Times sent
        /// 110         Total of Quantities to be split does to match Original Quantity
        /// 111         Invalid Date sent, format must be DD/MM/YYYY
        /// 112         Invalid Time sent, format must be HH:MM:SS
        /// 115         Quantity must be numeric
        /// 116         Could not update Pre Allocation record after split/merge
        /// 117         Unable to complete split/merge
        /// 118         Could not remove Pre Allocation record after merge
        /// 119         Expiry Date and Time must not be in the past
        /// 199         Unrecognised option passed to Pre Allocation Routine
        /// </returns>
        public string MergePreAllocation (
            string preallocationIds,
            string quantities,
            string expiries)
        {
            var arr = expiries.Split(new char[] { '|' }).ToList();

            var expiryDatesList = new List<string>();
            var expiryTimesList = new List<string>();

            for (var i = 0; i < arr.Count; i++)
            {
                var dt = DateTime.ParseExact(arr[i], "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                expiryDatesList.Add(dt.ToString("dd/MM/yyyy"));
                expiryTimesList.Add(dt.ToString("HH:mm:ss"));
            }

            var expiryDates = String.Join("|", expiryDatesList);
            var expiryTimes = String.Join("|", expiryTimesList);

            return this.Run(uvFileName: "MERGE_PREALLOC",
                uvSelectStatement: "",
                fieldsToReturn: "",
                webData: $@"{preallocationIds}
{quantities}
{expiryDates}
{expiryTimes}",
                returnType: "XML");
        }

        /// <summary>
        /// This routine is used to remove an existing Pre Allocation.
        /// </summary>
        /// <param name="preallocationId"></param>
        /// <returns>
        /// Expected results:
        /// If the routine is successful the ReturnFlag property will be set to zero.At completion of this routine the record with the key provided will be removed from WTASPPREALLOCS.
        /// The entry for the Pre Allocation will be removed from the allocation table on the Batch or Piece.The demands on the Product, Location, Batch and Piece (if applicable) will also be reduced by the appropriate amount.
        /// If the routine is not successful then the ReturnFlag property will not be zero, please see the Return Flags table that follows for an explanation of each possible ReturnFlag value. The Result property will contain this value.
        /// Designers Guild Web Integration Guide v1.1 Page: 36
        /// Return flags:
        /// Flag        Description
        /// 0           Success
        /// 100         No Pre Allocation ID provided
        /// 103         Pre Allocation record not found
        /// 199         Unrecognised option passed to Pre Allocation Routine
        /// </returns>
        public string RemovePreAllocation (
            string preallocationId)
        {
            int counter = 0;
            int foundCount = 0;

            return this.Run(uvFileName: "REMOVE_PREALLOC",
                uvSelectStatement: "",
                counter: ref counter,
                foundCount: ref foundCount,
                id: preallocationId,
                fieldsToReturn: "",
                webData: $@"{preallocationId}",
                returnType: "XML");
        }

        /// <summary>
        /// This routine is used to enquire on the status of an existing pre allocation. This variant goes by PreallocationID(s)
        /// </summary>
        /// <param name="preallocationIds">One or more pipe separated preallocationIds</param>
        /// <returns>
        /// Expected results:
        /// <?xml version="1.0"?>
        /// <results>
        ///     <Header>
        ///         <fd UV_fieldname="PRE.ALLOC.ID" field_id="0.0" num_chars="20" type="text" name="Pre_Alloc_Id" id="C1"/>
        ///         <fd UV_fieldname="PRODCODE" field_id="1.0" num_chars="15" type="text" name="Product_Code" id="C2"/>
        ///         <fd UV_fieldname="LOCCODE" field_id="2.0" num_chars="10" type="text" name="Location" id="C3"/>
        ///         <fd UV_fieldname="QTYORD.ORD" field_id="3.0" num_chars="7" type="float" name="Qty_Ordered" id="C4"/>
        ///         <fd UV_fieldname="UNITSTOCK" field_id="4.0" num_chars="3" type="text" name="Unit" id="C5"/>
        ///         <fd UV_fieldname="BATCH.NO" field_id="5.0" num_chars="10" type="text" name="Batch_No" id="C6"/>
        ///         <fd UV_fieldname="PIECE.NO" field_id="6.0" num_chars="10" type="text" name="Piece_No" id="C7"/>
        ///         <fd UV_fieldname="EXPIRY.DATE" field_id="7.0" num_chars="8" type="date" name="Expiry_Date" id="C8"/>
        ///         <fd UV_fieldname="EXPIRY.TIME" field_id="8.0" num_chars="8" type="text" name="Expiry_Time" id="C9"/>
        ///         <fd UV_fieldname="QTYORD.STK" field_id="13.0" num_chars="7" type="float" name="Qty_Ordered_-_Stock" id="C10"/>
        ///         <fd UV_fieldname = "BATCH.QTYS" field_id="9.0" num_chars="7" type="float" name="Batch_Qtys" id="C11"/>
        ///         <fd UV_fieldname = "RESERVE.IND" field_id="18.0" num_chars="1" type="text" name="Reserve_Ind" id="C12"/>
        ///         <fd UV_fieldname = "REMEASURE.REQUIRED" field_id="19.0" num_chars="1" type="text" name="Remeasure_Required" id="C13"/>
        ///         <fd UV_fieldname = "REM.BUFFER" field_id="20.0" num_chars="7" type="float" name="Rem_Buffer" id="C14"/>
        ///     </Header>
        ///     <Detail>
        ///         <record>
        ///             <fd id="C1" value="1234"/>
        ///             <fd id="C2" value="F562/30"/>
        ///             <fd id="C3" value="WH"/>
        ///             <fd id="C4" value="6.00"/>
        ///             <fd id="C5" value="M"/>
        ///             <fd id="C6" value="NA"/>
        ///             <fd id="C7" value="G044702"/>
        ///             <fd id="C8" value="15/07/2019"/>
        ///             <fd id="C9" value="23:59:59"/>
        ///             <fd id="C10" value="6.00"/>
        ///             <fd id = "C11" value="9.00"/>
        ///             <fd id = "C12" value="R"/>
        ///             <fd id = "C13" value="0"/>
        ///             <fd id = "C14" value="3.00"/>
        ///         </record>
        ///     </Detail>
        ///     <Total>
        ///         <fd id="C1" value=""/>
        ///         <fd id="C2" value=""/>
        ///         <fd id="C3" value=""/>
        ///         <fd id="C4" value="6.00"/>
        ///         <fd id="C5" value=""/>
        ///         <fd id="C6" value=""/>
        ///         <fd id="C7" value=""/>
        ///         <fd id="C8" value=""/>
        ///         <fd id="C9" value=""/>
        ///         <fd id="C10" value="6.00"/>
        ///         <fd id = "C11" value="9.00"/>
        ///         <fd id = "C12" value=""/>
        ///         <fd id = "C13" value="0"/>
        ///         <fd id = "C14" value="3.00"/>
        ///     </Total>
        /// </results>
        /// </returns>
        public string PreAllocationStatusByIDs(
            string preallocationIds)
        {
            var preallocationIdArray = preallocationIds.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

            var uvSelectStatement = $"PRE.ALLOC.ID = \"{String.Join("\" OR PRE.ALLOC.ID = \"", preallocationIdArray)}\"";

            return this.Run(uvFileName: "PREALLOC_STATUS",
                uvSelectStatement: uvSelectStatement,
                fieldsToReturn: "",
                webData: "",
                returnType: "XML");
        }

        public string PreAllocationStatusByProductCodes(
            string productCodes)
        {
            var productCodeArray = productCodes.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

            var uvSelectStatement = $"PRODCODE = \"{String.Join("\" OR PRODCODE = \"", productCodeArray)}\"";

            return this.Run(uvFileName: "PREALLOC_STATUS",
                uvSelectStatement: uvSelectStatement,
                fieldsToReturn: "",
                webData: "",
                returnType: "XML");
        }

        public string PreAllocationStatusByDateRange(
            string fromDate, 
            string toDate)
        {
            return this.Run(uvFileName: "PREALLOC_STATUS",
                uvSelectStatement: $"EXPIRY.DATE >= '{fromDate}' AND EXPIRY.DATE <= '{toDate}'",
                fieldsToReturn: "",
                webData: "",
                returnType: "XML"); 
        }
    }
}

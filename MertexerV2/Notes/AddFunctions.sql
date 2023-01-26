
-- ================================================
-- Template generated from Template Explorer using:
-- Create Scalar Function (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the function.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Thomas Clark
-- Create date: 24/07/2019
-- Description:	Creates PreAllocations
-- ===========================================
CREATE FUNCTION clr.fnc_Mertex_PreAllocation_Create
(
	@PreAllocationID NVARCHAR(36),
	@ProductCode NVARCHAR(20),
	@LocationCode NVARCHAR(20),
	@Quantity DECIMAL(7,2),
	@Unit NVARCHAR(5),
	@Batch NVARCHAR(20) = NULL,
	@Piece NVARCHAR(20) = NULL,
	@Expiry DATETIME2(2),
	@ReserveIndicator NVARCHAR(1) = NULL,
	@SoftAllocated NVARCHAR(1) = NULL
)
RETURNS TABLE(
	[PreAllocationID] [nvarchar](36),
	[Batch] [nvarchar](50),
	[Piece] [nvarchar](50),
	[QuantityAllocated] [decimal] (7, 2),
	[RemeasureTriggered] [bit],
	[ReserveBuffer] [decimal] (7, 2)
) WITH EXECUTE AS CALLER
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.SQLFunctions.PreAllocation].[Create]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 24/07/2019
-- Description:	Updates PreAllocations
-- =============================================
CREATE FUNCTION clr.fnc_Mertex_PreAllocation_Update
(
	@PreAllocationID NVARCHAR(36),
	@ProductCode NVARCHAR(20),
	@LocationCode NVARCHAR(20),
	@Quantity DECIMAL(7,2),
	@Unit NVARCHAR(5),
	@Batch NVARCHAR(20) = NULL,
	@Piece NVARCHAR(20) = NULL,
	@Expiry DATETIME2(2),
	@ReserveIndicator NVARCHAR(1) = NULL,
	@SoftAllocated NVARCHAR(1) = NULL
)
RETURNS TABLE(
	[PreAllocationID] [nvarchar](36),
	[Batch] [nvarchar](20),
	[Piece] [nvarchar](20),
	[QuantityAllocated] [decimal] (7, 2),
	[RemeasureTriggered] [bit],
	[ReserveBuffer] [decimal] (7, 2)
) WITH EXECUTE AS CALLER
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.SQLFunctions.PreAllocation].[Update]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 24/07/2019
-- Description:	Splits PreAllocations
-- =============================================

CREATE FUNCTION clr.fnc_Mertex_PreAllocation_Split
(
	@PreAllocationIDs NVARCHAR(MAX),
	@Quantities NVARCHAR(MAX),
	@Expiries NVARCHAR(MAX)
)
RETURNS INT 
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.SQLFunctions.PreAllocation].[Split]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 24/07/2019
-- Description:	Merges PreAllocations
-- =============================================

CREATE FUNCTION clr.fnc_Mertex_PreAllocation_Merge
(
	@PreAllocationIDs NVARCHAR(MAX),
	@Quantities NVARCHAR(MAX),
	@Expiries NVARCHAR(MAX)
)
RETURNS INT 
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.SQLFunctions.PreAllocation].[Merge]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 24/07/2019
-- Description:	Deletes PreAllocations
-- =============================================

CREATE FUNCTION clr.fnc_Mertex_PreAllocation_Delete
(
	@PreAllocationID NVARCHAR(36)
)
RETURNS INT 
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.SQLFunctions.PreAllocation].[Delete]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 24/07/2019
-- Description:	Checks status of PreAllocations
-- =============================================

CREATE FUNCTION clr.fnc_Mertex_PreAllocation_Status
(
	@PreAllocationIDs NVARCHAR(MAX),
	@ProductCodes NVARCHAR(MAX),
	@FromDT DATETIME2(2) = NULL,
	@ToDT DATETIME2(2) = NULL
)
RETURNS TABLE (
	[PreAllocationID] nvarchar(50),
	[ProductCode] nvarchar(20),
	[LocationCode] nvarchar(20),
	[Quantity] decimal(7,2),
	[Batch] nvarchar(20),
	[Piece] nvarchar(20),
	[ExpiryDT] datetime,
	[QuantityOrderedLessStock] decimal(7,2),
	[BatchQuantities] decimal(7,2),
	[ReserveIndicator] nvarchar(1),
	[RemeasureRequired] bit,
	[ReserveBuffer] decimal(7,2)
)
WITH EXECUTE AS CALLER
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.SQLFunctions.PreAllocation].[Status]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 24/07/2019
-- Description:	Checks status of PreAllocations
-- =============================================

CREATE FUNCTION clr.fnc_Mertex_Stock_BatchPiece
(
	@ProductCodes NVARCHAR(MAX),
	@StockLocations NVARCHAR(MAX),
	@EnquiryType NVARCHAR(1)
)
RETURNS TABLE (
	[Type] nvarchar(1),
	[ProductCode] nvarchar(50),
	[LocationCode] nvarchar(20),
	[Batch] nvarchar(20),
	[Piece] nvarchar(20),
	[Pickable] bit,
	[Rack] nvarchar(20),
	[RackType] nvarchar(20),
	[Quantity] decimal(7,2),
	[OnReserve] decimal(7,2),
	[PODate] datetime,
	[POMessage] nvarchar(100),
	[POStatus] bit NULL
)
WITH EXECUTE AS CALLER
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.SQLFunctions.Stock].[BatchPiece]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 24/07/2019
-- Description:	Checks status of PreAllocations
-- =============================================

CREATE FUNCTION clr.fnc_Mertex_Stock_B2CEnquiry
(
)
RETURNS TABLE (
	[StockLocationCode] nvarchar(20),
	[ProductCode] nvarchar(50),
	[TotalFreeStock] decimal(7,2),
	[PODate] datetime,
	[POMessage] nvarchar(50),
	[NextPOQuantity] decimal(7,2),
	[TotalPOQuantity] decimal(7,2),
	[POStatus] bit NULL
)
WITH EXECUTE AS CALLER
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.SQLFunctions.Stock].[B2CEnquiry]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 19/09/2019
-- Description:	Checks credit about a customer
-- =============================================

CREATE FUNCTION clr.fnc_Mertex_Credit_Check
(
	@TradeAccountCode NVARCHAR(20)
)
RETURNS TABLE (
	[CreditStop] [nvarchar](20) NULL,
	[CreditStatusMessage] [nvarchar](20) NULL,
	[RetailTillCustomer] [nvarchar](20) NULL,
	[CreditLimit] [decimal](20, 2) NULL,
	[Balance] [decimal](20, 2) NULL,
	[BalanceCurrency] [decimal](20, 2) NULL,
	[OutstandingBalance] [decimal](20, 2) NULL,
	[OutstandingBalanceCurrency] [decimal](20, 2) NULL,
	[OutstandingReserves] [decimal](20, 2) NULL,
	[OutstandingReservesCurrency] [decimal](20, 2) NULL
)
WITH EXECUTE AS CALLER
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.SQLFunctions.Credit].[Check]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 07/04/2021
-- Description:	Fetches status update for a given line
-- =============================================

CREATE FUNCTION clr.fnc_Mertex_LineStatus
(
	@OrderHeadID INT,
	@LineNumber SMALLINT,
	@OrderLineID BIGINT
)
RETURNS TABLE (
	[ToelineID] [nvarchar](20) NULL,
	[OrderHeadID] [int] NULL,
	[OrderLineID] [bigint] NULL,
	[LineNumber] [smallint] NULL,
	[ProductCode] [nvarchar](20) NULL,
	[OrderStatusID] [tinyint] NULL,
	[DetailedStatus] [nvarchar](500) NULL,
	[StatusChangedDT] [datetime] NULL,
	[Quantity] [decimal](18,2) NULL,
	[CancelledQuantity] [decimal](18,2)NULL
)
WITH EXECUTE AS CALLER
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.SQLFunctions.LineStatus].[Check]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 07/04/2021
-- Description:	Same as above, but in batches of 1000 at a time. Calls portal.OrderLine_StatusUpdate_SelectList to get a list of OrderLine keys.
-- =============================================

CREATE FUNCTION clr.fnc_Mertex_LineStatus_Batch
(
	@TransactionSourceID TINYINT = NULL,
	@FromDT DATETIME2(2) = NULL
)
RETURNS TABLE (
	[ToelineID] [nvarchar](20) NULL,
	[OrderHeadID] [int] NULL,
	[OrderLineID] [bigint] NULL,
	[LineNumber] [smallint] NULL,
	[ProductCode] [nvarchar](20) NULL,
	[OrderStatusID] [tinyint] NULL,
	[DetailedStatus] [nvarchar](500) NULL,
	[StatusChangedDT] [datetime] NULL,
	[Quantity] [decimal](18,2) NULL,
	[CancelledQuantity] [decimal](18,2)NULL
)
WITH EXECUTE AS CALLER
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.SQLFunctions.LineStatus].[BatchCheck]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 07/04/2021
-- Description:	Fetches status update for a given line
-- =============================================

CREATE FUNCTION clr.fnc_Mertex_LineTracking
(
	@OrderHeadID INT,
	@LineNumber SMALLINT,
	@OrderLineID BIGINT
)
RETURNS TABLE (
	[ToelineID] [nvarchar](20) NULL,
	[OrderHeadID] [int] NULL,
	[OrderLineID] [bigint] NULL,
	[LineNumber] [smallint] NULL,
	[EstimatedDeliveryDate] [datetime] NULL,
	[DespatchedDT] [datetime] NULL,
	[ParcelTrackingURL] [nvarchar](500) NULL
)
WITH EXECUTE AS CALLER
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.SQLFunctions.LineTracking].[Check]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 07/04/2021
-- Description:	Same as above, but in batches of 1000 at a time. Calls portal.OrderLine_StatusUpdate_SelectList to get a list of OrderLine keys.
-- =============================================

CREATE FUNCTION clr.fnc_Mertex_LineTracking_Batch
(
	@TransactionSourceID TINYINT = NULL,
	@FromDT DATETIME2(2) = NULL
)
RETURNS TABLE (
	[ToelineID] [nvarchar](20) NULL,
	[OrderHeadID] [int] NULL,
	[OrderLineID] [bigint] NULL,
	[LineNumber] [smallint] NULL,
	[EstimatedDeliveryDate] [datetime] NULL,
	[DespatchedDT] [datetime] NULL,
	[ParcelTrackingURL] [nvarchar](500) NULL
)
WITH EXECUTE AS CALLER
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.SQLFunctions.LineTracking].[BatchCheck]
GO

/*    ==Scripting Parameters==

    Source Server Version : SQL Server 2016 (13.0.4604)
    Source Database Engine Edition : Microsoft SQL Server Enterprise Edition
    Source Database Engine Type : Standalone SQL Server

    Target Server Version : SQL Server 2016
    Target Database Engine Edition : Microsoft SQL Server Enterprise Edition
    Target Database Engine Type : Standalone SQL Server
*/

/****** Object:  StoredProcedure [clr].[sp_Mertex_Order_Submit]    Script Date: 03/03/2020 17:11:46 ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [clr].[sp_Mertex_Order_Submit]
	@OrderHeadID [INT],
	@SelectProcedure [NVARCHAR] (50),
	@ResultantOrderHeadID [INT] OUTPUT
WITH EXECUTE AS CALLER
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.SQLFunctions.Order].[Submit]
GO

/*    ==Scripting Parameters==

    Source Server Version : SQL Server 2016 (13.0.4604)
    Source Database Engine Edition : Microsoft SQL Server Enterprise Edition
    Source Database Engine Type : Standalone SQL Server

    Target Server Version : SQL Server 2016
    Target Database Engine Edition : Microsoft SQL Server Enterprise Edition
    Target Database Engine Type : Standalone SQL Server
*/

/****** Object:  UserDefinedFunction [clr].[fnc_Mertex_Rack_Check]    Script Date: 03/03/2020 17:13:12 ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER OFF
GO

CREATE FUNCTION [clr].[fnc_Mertex_Rack_Check](@Piece [nvarchar](20))
RETURNS  TABLE (
	[Rack] [nvarchar](20) NULL
) WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [MertexerV2].[MertexerV2.SQLFunctions.Rack].[Check]
GO

/*    ==Scripting Parameters==

    Source Server Version : SQL Server 2016 (13.0.4604)
    Source Database Engine Edition : Microsoft SQL Server Enterprise Edition
    Source Database Engine Type : Standalone SQL Server

    Target Server Version : SQL Server 2016
    Target Database Engine Edition : Microsoft SQL Server Enterprise Edition
    Target Database Engine Type : Standalone SQL Server
*/

/****** Object:  UserDefinedFunction [clr].[fnc_Mertex_Dedupe_Order]    Script Date: 03/03/2020 17:13:47 ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER OFF
GO

CREATE FUNCTION [clr].[fnc_Mertex_Dedupe_Order](@TradeAccountCode [nvarchar](20), @OrderReference [nvarchar](24))
RETURNS  TABLE (
	[Record] [nvarchar](50) NULL
) WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [MertexerV2].[MertexerV2.SQLFunctions.Dedupe].[CheckOrder]
GO

/*    ==Scripting Parameters==

    Source Server Version : SQL Server 2016 (13.0.4604)
    Source Database Engine Edition : Microsoft SQL Server Enterprise Edition
    Source Database Engine Type : Standalone SQL Server

    Target Server Version : SQL Server 2016
    Target Database Engine Edition : Microsoft SQL Server Enterprise Edition
    Target Database Engine Type : Standalone SQL Server
*/

/****** Object:  UserDefinedFunction [clr].[fnc_Mertex_Dedupe_OrderLine]    Script Date: 03/03/2020 17:14:01 ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER OFF
GO

CREATE FUNCTION [clr].[fnc_Mertex_Dedupe_OrderLine](@TradeAccountCode [nvarchar](20), @ProductCode [nvarchar](20), @Quantity [decimal](7, 2), @LineReference [nvarchar](24), @MonthsBack [tinyint] = 6)
RETURNS  TABLE (
	[Record] [nvarchar](50) NULL
) WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [MertexerV2].[MertexerV2.SQLFunctions.Dedupe].[CheckLine]
GO


/*    ==Scripting Parameters==

    Source Server Version : SQL Server 2016 (13.0.4604)
    Source Database Engine Edition : Microsoft SQL Server Enterprise Edition
    Source Database Engine Type : Standalone SQL Server

    Target Server Version : SQL Server 2016
    Target Database Engine Edition : Microsoft SQL Server Enterprise Edition
    Target Database Engine Type : Standalone SQL Server
*/

/****** Object:  UserDefinedFunction [clr].[fnc_Mertex_Stock_B2CEnquiry_Singular]    Script Date: 03/03/2020 17:14:33 ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER OFF
GO

CREATE FUNCTION [clr].[fnc_Mertex_Stock_B2CEnquiry_Singular](@ProductCode [nvarchar](20), @StockLocation [nvarchar](10))
RETURNS  TABLE (
	[StockLocationCode] [nvarchar](20) NULL,
	[ProductCode] [nvarchar](50) NULL,
	[TotalFreeStock] [decimal](7, 2) NULL,
	[PODate] [datetime] NULL,
	[POMessage] [nvarchar](50) NULL,
	[NextPOQuantity] [decimal](7, 2) NULL,
	[TotalPOQuantity] [decimal](7, 2) NULL,
	[POStatus] bit NULL
) WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [MertexerV2].[MertexerV2.SQLFunctions.Stock].[B2CEnquiry_Singular]
GO

-- ================================================
-- Template generated from Template Explorer using:
-- Create Scalar Function (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the function.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Thomas Clark
-- Create date: 24/07/2019
-- Description:	Creates PreAllocations
-- ===========================================
CREATE FUNCTION clr.fnc_Mertex_VATNumber_Lookup
(
	@TradeAccountCode NVARCHAR(50)
)
RETURNS TABLE(
	[VATCountryCode] [NVARCHAR] (2),
	[VATNumber] [NVARCHAR](20)
) WITH EXECUTE AS CALLER
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.SQLFunctions.VAT].[Lookup]
GO


-- ================================================
-- Template generated from Template Explorer using:
-- Create Scalar Function (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the function.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Thomas Clark
-- Create date: 24/07/2019
-- Description:	Creates PreAllocations
-- ===========================================
CREATE FUNCTION clr.fnc_Mertex_EORINumber_Lookup
(
	@TradeAccountCode NVARCHAR(50)
)
RETURNS TABLE(
	[EORINumber] [nvarchar](20)
) WITH EXECUTE AS CALLER
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.SQLFunctions.EORI].[Lookup]
GO


/*    ==Scripting Parameters==

    Source Server Version : SQL Server 2016 (13.0.4604)
    Source Database Engine Edition : Microsoft SQL Server Enterprise Edition
    Source Database Engine Type : Standalone SQL Server

    Target Server Version : SQL Server 2016
    Target Database Engine Edition : Microsoft SQL Server Enterprise Edition
    Target Database Engine Type : Standalone SQL Server
*/

/****** Object:  StoredProcedure [clr].[sp_Mertex_Order_ValidateImport]    Script Date: 01/06/2020 17:11:00 ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [clr].[sp_Mertex_Order_ValidateImport]
	@TradeOrderImportFileID [BIGINT]
WITH EXECUTE AS CALLER
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.SQLFunctions.Order].[ProcessImport]
GO


-- =============================================
-- Author:		Thomas Clark
-- Create date: 13/04/2021
-- Description:	Fetches Delivery References given parameters. TradeAccountCode and CountryCode are mandatory.
-- =============================================

CREATE FUNCTION clr.fnc_Mertex_DeliveryReference_Search
(
	@TradeAccountCode NVARCHAR(20),
	@AddressLine1 NVARCHAR(60),
	@PostalCode NVARCHAR(20),
	@CountryCode NVARCHAR(2)
)
RETURNS TABLE (
	[DeliveryReference] [nvarchar](20) NULL,
	[CompanyName] [NVARCHAR](35) NULL,
	[ContactName] [NVARCHAR](50) NULL,
	[AddressLine1] [NVARCHAR](60) NULL,
	[AddressLine2] [NVARCHAR](60) NULL,
	[Town] [NVARCHAR](50) NULL,
	[County] [NVARCHAR](40) NULL,
	[Postcode] [NVARCHAR](20) NULL,
	[CountryCode] [NVARCHAR](2) NULL,
	[PhoneNumber] [NVARCHAR](20) NULL,
	[Email] [NVARCHAR](MAX) NULL,
	[LastConfirmedDT] DATETIME NULL
)
WITH EXECUTE AS CALLER
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.SQLFunctions.DeliveryReference].[Search]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 23/04/2021
-- Description:	Fetches Delivery References given parameters. TradeAccountCode and CountryCode are mandatory.
-- =============================================

CREATE FUNCTION clr.fnc_Mertex_DeliveryReference_Check
(
	@TradeAccountCode NVARCHAR(20),
	@DeliveryReference NVARCHAR(20)
)
RETURNS BIT
WITH EXECUTE AS CALLER
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.SQLFunctions.DeliveryReference].[Check]
GO

/****** Object:  StoredProcedure [clr].[sp_Mertex_Order_ValidateImport]    Script Date: 01/06/2020 17:11:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [clr].[sp_Mertex_Order_ImportFromOldTradeSite]
WITH EXECUTE AS CALLER
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.SQLFunctions.B2BOrderImport].[SiphonOldTradeSiteOrders]
GO


/****** Object:  StoredProcedure [clr].[sp_Mertex_Order_ValidateImport]    Script Date: 01/06/2020 17:11:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [portal].[B2BOrderImport_Process]
WITH EXECUTE AS CALLER
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.SQLFunctions.B2BOrderImport].[ProcessOldTradeSiteOrders]
GO


-- =============================================
-- Author:		Thomas Clark
-- Create date: 20/05/2021
-- Description:	Creates Delivery Addresses.
-- =============================================

CREATE FUNCTION clr.fnc_Mertex_DeliveryAddress_Create
(
	@TradeAccountCode NVARCHAR(20),
	@CompanyName NVARCHAR (20) = NULL,
	@ContactName NVARCHAR (20) = NULL,
	@AddressLine1 NVARCHAR (20) = NULL,
	@AddressLine2 NVARCHAR (20) = NULL,
	@Town NVARCHAR(20) = NULL,
	@County NVARCHAR(20) = NULL,
	@Postcode NVARCHAR(20) = NULL,
	@CountryCode NVARCHAR(20) = NULL,
	@PhoneNumber NVARCHAR(20) = NULL,
	@Email NVARCHAR(2000) = NULL
	--,
 --   @DomesticOrTrade NCHAR(1) = NULL,
 --   @HaulierCode NVARCHAR(20) = NULL,
 --   @CarriageCode NVARCHAR(20) = NULL,
 --   @UsStateCode NVARCHAR(20) = NULL,
 --   @UsCountyCode NVARCHAR(20) = NULL,
 --   @UsTaxExemptionCertificateReference NVARCHAR(50) = null
)
RETURNS NVARCHAR(20)
WITH EXECUTE AS CALLER
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.AddressBookFunctions].[Create]
GO

/****** Object:  StoredProcedure [clr].[sp_Mertex_Order_ValidateImport]    Script Date: 01/06/2020 17:11:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Thomas Clark
-- Create date: 19/09/2019
-- Description:	Checks credit about a customer
-- =============================================

CREATE FUNCTION clr.fnc_Mertex_FetchInvoiceAddress
(
	@TradeAccountCode NVARCHAR(20)
)
RETURNS TABLE (
	[AddressLine1] [nvarchar] (60) NULL,
	[AddressLine2] [nvarchar] (60) NULL,
	[Town] [nvarchar] (50) NULL,
	[County] [nvarchar] (40) NULL,
	[Postcode] [nvarchar] (20) NULL,
	[CountryCode] [nvarchar](2) NULL
)
WITH EXECUTE AS CALLER
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.AddressBookFunctions].[FetchInvoiceAddress]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 21/07/2021
-- Description:	If ParcelNumber is not null / empty, writes to: <40> CUSTOM.TRACK.URL.DES on OEFINDPARCEL 
--											   else, writes to <535> CUSTOM.TRACK.URL.DES on OEDNHEAD 
--													[or the same field mark on OEDNHHIST if the PickingList record was not found on OEDNHEAD]
-- =============================================

CREATE FUNCTION clr.fnc_Mertex_Update_CustomTrackingURL
(
	@PicklistNumber NVARCHAR(50),
	@ParcelNumber NVARCHAR(50),
	@TrackingURL NVARCHAR(500)
)
RETURNS INT 
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.SQLFunctions.TrackingUpdate].[Update]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 21/07/2021
-- Description:	If ParcelNumber is not null / empty, writes to: <40> CUSTOM.TRACK.URL.DES on OEFINDPARCEL 
--											   else, writes to <535> CUSTOM.TRACK.URL.DES on OEDNHEAD 
--													[or the same field mark on OEDNHHIST if the PickingList record was not found on OEDNHEAD]
-- =============================================

CREATE FUNCTION clr.fnc_TradeAccount_MertexUpsert
(
	@TradeDataSourceJSON NVARCHAR(1000),
	@TradeAccountJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(50)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.SQLFunctions.AccountFunctions].[Upsert]
GO


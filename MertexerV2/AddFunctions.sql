USE FlockDev
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
CREATE FUNCTION [clr].[fnc_Mertex_PreAllocation_Create]
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
	[Rack] [nvarchar](50),
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
CREATE FUNCTION [clr].[fnc_Mertex_PreAllocation_Update]
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
	[Rack] [nvarchar](50),
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

CREATE FUNCTION [clr].[fnc_Mertex_PreAllocation_Split]
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

CREATE FUNCTION [clr].[fnc_Mertex_PreAllocation_Merge]
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

CREATE FUNCTION [clr].[fnc_Mertex_PreAllocation_Delete]
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

CREATE FUNCTION [clr].[fnc_Mertex_PreAllocation_Status]
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

CREATE FUNCTION [clr].[fnc_Mertex_Stock_BatchPiece]
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

CREATE FUNCTION [clr].[fnc_Mertex_Stock_B2CEnquiry]
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

CREATE FUNCTION [clr].[fnc_Mertex_Credit_Check]
(
	@TradeDataSourceJSON NVARCHAR(MAX)
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

CREATE FUNCTION [clr].[fnc_Mertex_LineStatus]
(
	@UniVerseAccount NVARCHAR(50),
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

CREATE FUNCTION [clr].[fnc_Mertex_LineStatus_Batch]
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

CREATE FUNCTION [clr].[fnc_Mertex_LineTracking]
(
	@UniVerseAccount NVARCHAR(50),
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

CREATE FUNCTION [clr].[fnc_Mertex_LineTracking_Batch]
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

CREATE FUNCTION [clr].[fnc_Mertex_Rack_Check]
(
	@Piece [nvarchar](20)
)
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

CREATE FUNCTION [clr].[fnc_Mertex_Dedupe_Order]
(
	@TradeDataSourceJson [nvarchar](MAX), 
	@OrderReference [nvarchar](24)
)
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

CREATE FUNCTION [clr].[fnc_Mertex_Dedupe_OrderLine]
(
	@TradeDataSourceJSON [nvarchar](MAX), 
	@ProductCode [nvarchar](20), 
	@Quantity [decimal](7, 2), 
	@LineReference [nvarchar](24), 
	@MonthsBack [tinyint] = 6
)
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

CREATE FUNCTION [clr].[fnc_Mertex_Stock_B2CEnquiry_Singular]
(
	@ProductCode [nvarchar](20), 
	@StockLocation [nvarchar](10)
)
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
CREATE FUNCTION [clr].[fnc_Mertex_VATNumber_Lookup]
(
	@TradeDataSourceJSON NVARCHAR(MAX)
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
CREATE FUNCTION [clr].[fnc_Mertex_EORINumber_Lookup]
(
	@TradeDataSourceJSON NVARCHAR(MAX)
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

CREATE FUNCTION [clr].[fnc_Mertex_DeliveryReference_Search]
(
	@TradeDataSourceJSON NVARCHAR(MAX),
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

CREATE FUNCTION [clr].[fnc_Mertex_DeliveryReference_Check]
(
	@TradeDataSourceJSON NVARCHAR(MAX),
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
(
	@UniVerseAccount NVARCHAR(50)
)
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

ALTER FUNCTION [clr].[fnc_Mertex_DeliveryAddress_Create]
(
	@TradeDataSourceJSON NVARCHAR(MAX),
	@CompanyName NVARCHAR (35),
	@ContactName NVARCHAR (50) = NULL,
	@AddressLine1 NVARCHAR (60) = NULL,
	@AddressLine2 NVARCHAR (60) = NULL,
	@Town NVARCHAR(50) = NULL,
	@County NVARCHAR(40) = NULL,
	@Postcode NVARCHAR(20) = NULL,
	@CountryCode NVARCHAR(2) = NULL,
	@PhoneNumber NVARCHAR(20) = NULL,
	@Email NVARCHAR(2000)
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

CREATE FUNCTION [clr].[fnc_Mertex_FetchInvoiceAddress]
(
	@TradeDataSourceJSON NVARCHAR(MAX)
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


/****** Object:  StoredProcedure [clr].[sp_Mertex_Order_ValidateImport]    Script Date: 01/06/2020 17:11:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [clr].[fnc_Mertex_Update_CustomTrackingURL]
(
	@UniVerseAccount NVARCHAR(50),
	@PicklistNumber NVARCHAR(50),
	@ParcelNumber NVARCHAR(50),
	@TrackingURL NVARCHAR(500),
	@DeliveryPostcode NVARCHAR(20)
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


/****** Object:  StoredProcedure [clr].[sp_Mertex_Order_ValidateImport]    Script Date: 01/06/2020 17:11:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [clr].[fnc_Mertex_TradeAccount_Upsert]
(
	@TradeDataSourceJSON NVARCHAR(MAX),
	@TradeAccountJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(50)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.SQLFunctions.AccountFunctions].[Upsert]
GO



-- =============================================
-- Author:		Thomas Clark
-- Create date: 21/07/2021
-- Description:	If ParcelNumber is not null / empty, writes to: <40> CUSTOM.TRACK.URL.DES on OEFINDPARCEL 
--											   else, writes to <535> CUSTOM.TRACK.URL.DES on OEDNHEAD 
--													[or the same field mark on OEDNHHIST if the PickingList record was not found on OEDNHEAD]
-- =============================================


/****** Object:  StoredProcedure [clr].[sp_Mertex_Order_ValidateImport]    Script Date: 01/06/2020 17:11:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [clr].[fnc_Mertex_TradeAccount_Select]
(
	@UniVerseAccount NVARCHAR(50),
	@TradeAccountList NVARCHAR(MAX)
)
RETURNS TABLE
(
	[CUSREF] NVARCHAR (500) NULL,
	[CUSNAME] NVARCHAR (500) NULL,
	[OPEN] NVARCHAR (500) NULL,
	[SORTNAME_EXP] NVARCHAR (500) NULL,
	[ONFILE_DT] DATETIME NULL,
	[AMENDED_DT] DATETIME NULL,
	[CODE_GROUP] NVARCHAR (500) NULL,
	[SS_CONTACT_NAME] NVARCHAR (500) NULL,
	[CUSADD1X] NVARCHAR (500) NULL,
	[CUSADD2X] NVARCHAR (500) NULL,
	[CUSADD3X] NVARCHAR (500) NULL,
	[CUSADD4X] NVARCHAR (500) NULL,
	[CUSPCODE] NVARCHAR (500) NULL,
	[COUNTRY] NVARCHAR (500) NULL,
	[TOWN] NVARCHAR (500) NULL,
	[COUNTY] NVARCHAR (500) NULL,
	[STATEMENT_AC] NVARCHAR (500) NULL,
	[STNAME] NVARCHAR (500) NULL,
	[STADD1] NVARCHAR (500) NULL,
	[STADD2] NVARCHAR (500) NULL,
	[STADD3] NVARCHAR (500) NULL,
	[STADD4] NVARCHAR (500) NULL,
	[STPCODE] NVARCHAR (500) NULL,
	[STCONTACT] NVARCHAR (500) NULL,
	[STPHONE] NVARCHAR (500) NULL,
	[STD_TELEPHONE] NVARCHAR (500) NULL,
	[EMAIL] NVARCHAR (500) NULL,
	[SS_MARKET_EMAIL] NVARCHAR (500) NULL,
	[WEB_SITE] NVARCHAR (500) NULL,
	[SSLPOSTJSON] NVARCHAR (MAX) NULL,
	[MARKTYPE_DES] NVARCHAR (500) NULL,
	[CUSTYPE] NVARCHAR (500) NULL,
	[CLASS_CODE] NVARCHAR (500) NULL,
	[SEL1] NVARCHAR (500) NULL,
	[SEL2] NVARCHAR (500) NULL,
	[SEL3] NVARCHAR (500) NULL,
	[DIVISION_CODE] NVARCHAR (500) NULL,
	[COMPANY_REGNO] NVARCHAR (500) NULL,
	[EORI_NUMBER] NVARCHAR (500) NULL,
	[TAXID] NVARCHAR (500) NULL,
	[VATREG_DES] NVARCHAR (500) NULL,
	[VATCOUNTRY] NVARCHAR (500) NULL,
	[VATNORM] NVARCHAR (500) NULL,
	[VATDATLASTCHK] NVARCHAR (500) NULL,
	[VALIDVATNO] NVARCHAR (500) NULL,
	[INVALIDVATNO] NVARCHAR (500) NULL,
	[CURRENCY] NVARCHAR (500) NULL,
	[SELLPRICE] NVARCHAR (500) NULL,
	[BANKNO] NVARCHAR (500) NULL,
	[CREDCONT_DES] NVARCHAR (500) NULL,
	[CRLIMIT] NVARCHAR (500) NULL,
	[CRLIMIT_EXPIRY_DT] [DATETIME] NULL,
	[HIGH_VALUE_EXEMPT_DES] NVARCHAR (500) NULL,
	[INS_CRLIM_SL_DES] NVARCHAR (500) NULL,
	[CRSTOP] NVARCHAR (500) NULL,
	[CL_CHECK_AGAINST_DES] NVARCHAR (500) NULL,
	[INS_EXPDT_SL_DES_DT] [DATETIME] NULL,
	[BANKRUPT] NVARCHAR (500) NULL,
	[STTERMS] NVARCHAR (500) NULL,
	[POREQ] NVARCHAR (500) NULL,
	[HAULIER] NVARCHAR (500) NULL,
	[SERVICE] NVARCHAR (500) NULL,
	[UPS_ACCOUNT_NO_DES] NVARCHAR (500) NULL,
	[DUTY_ACC_NO] NVARCHAR (500) NULL,
	[DHL_ACCOUNT_NO_DES] NVARCHAR (500) NULL,
	[TNT_BILLING_DES] NVARCHAR (500) NULL,
	[DELTERMS] NVARCHAR (500) NULL,
	[EXCLUDE_INVOICE] NVARCHAR (500) NULL,
	[INVVATDISPLAY] NVARCHAR (500) NULL,
	[INV_ODUE_POG] NVARCHAR (500) NULL,
	[EDI_INVOICE] NVARCHAR (500) NULL,
	[EXTENDEDMEMO_NV] NVARCHAR (500) NULL,
	[PARTNER_CO] NVARCHAR (500) NULL,
	[CUSACC1] NVARCHAR (500) NULL,
	[CUSALYS] NVARCHAR (500) NULL,
	[CUSSUB] NVARCHAR (500) NULL,
	[SUNDRY_ACCPT] NVARCHAR (500) NULL,
	[OD_LETTER1] NVARCHAR (500) NULL,
	[OD_LETTER2] NVARCHAR (500) NULL,
	[OD_LETTER3] NVARCHAR (500) NULL,
	[OD_LETTER4] NVARCHAR (500) NULL,
	[OD_LETTER5] NVARCHAR (500) NULL,
	[DEF_DELREF] NVARCHAR (500) NULL,
	[COMM_INV_DES] NVARCHAR (500) NULL,
	[ORDER_LOC] NVARCHAR (500) NULL,
	[EXPORT_DES] NVARCHAR (500) NULL,
	[EXP_NO_PN_DES] NVARCHAR (500) NULL,
	[DEF_CONSOLIDATE] NVARCHAR (500) NULL,
	[CARR_EXEM_SL] NVARCHAR (500) NULL,
	[CONF_REQ_SL] NVARCHAR (500) NULL,
	[PF_TRADING] NVARCHAR (500) NULL,
	[FOC_CODE_DES] NVARCHAR (500) NULL,
	[INV_DETAILS] NVARCHAR (500) NULL,
	[SELLCO] NVARCHAR (500) NULL,
	[GROUPAGE_TYPE] NVARCHAR (500) NULL,
	[DIR_DEL] NVARCHAR (500) NULL,
	[PACKLIST_SL] NVARCHAR (500) NULL,
	[CRD_COPY] NVARCHAR (500) NULL,
	[ABC_PRICING_DES] NVARCHAR (500) NULL,
	[STD_ROLLEN] NVARCHAR (500) NULL,
	[MIN_VALUE_SUR] NVARCHAR (500) NULL,
	[DELCHANGE] NVARCHAR (500) NULL,
	[PF_SURCHARGE] NVARCHAR (500) NULL,
	[INV_COPIES] NVARCHAR (500) NULL,
	[DISTREF] NVARCHAR (500) NULL,
	[TRANS_ACCNT_DES] NVARCHAR (500) NULL,
	[STOCK_ONLY_DES] NVARCHAR (500) NULL,
	[RECEIVE_LOC_DES] NVARCHAR (500) NULL,
	[CONT_PRICING] NVARCHAR (500) NULL,
	[PROD_SCREEN] NVARCHAR (500) NULL,
	[MINORDQTY_SL] NVARCHAR (500) NULL,
	[OVERDUE_LETTERS] NVARCHAR (500) NULL,
	[OVERDUE_TOTALS] NVARCHAR (500) NULL,
	[CUT_VERIFY] NVARCHAR (500) NULL,
	[CCARD_DETAILS] NVARCHAR (500) NULL,
	[OOS_LETTER] NVARCHAR (500) NULL,
	[STAT_PRINT] NVARCHAR (500) NULL,
	[INT_EXT] NVARCHAR (500) NULL,
	[EDI_DIRECT_CUST] NVARCHAR (500) NULL,
	[FILE_PROTOCOL] NVARCHAR (500) NULL,
	[TRAN_USER] NVARCHAR (500) NULL,
	[TILL_CREDITS] NVARCHAR (500) NULL,
	[TITLE_TRANS_DES] NVARCHAR (500) NULL,
	[END_CUST_INV_DES] NVARCHAR (500) NULL,
	[WEBTEX_EXCLUDE_LIST] NVARCHAR (500) NULL,
	[MISC_ACCNT_DES] NVARCHAR (500) NULL,
	[PRICECODE] NVARCHAR (500) NULL,
	[DELDATE_SL] NVARCHAR (500) NULL,
	[MINORDVAL_SL] NVARCHAR (500) NULL,
	[CHKDUPLICATE] NVARCHAR (500) NULL,
	[TILLID_CODE] NVARCHAR (500) NULL,
	[USUAL_ORD_UNIT] NVARCHAR (500) NULL,
	[CANX_LETTS_DES] NVARCHAR (500) NULL,
	[STMNT_CREDACT] NVARCHAR (500) NULL,
	[INFORM_DATE_DES] NVARCHAR (500) NULL,
	[STORE_NAME] NVARCHAR (500) NULL,
	[MEASURE_PIECE] NVARCHAR (500) NULL,
	[TRAN_ADDR] NVARCHAR (500) NULL,
	[TRAN_PWD] NVARCHAR (500) NULL,
	[RETAIL_TILL_CUST] NVARCHAR (500) NULL,
	[END_CUST_REQ_DES] NVARCHAR (500) NULL,
	[DEF_ORDER_PRIORITY] NVARCHAR (500) NULL,
	[SO_ALLOC_FULL] NVARCHAR (500) NULL,
	[INV_TO_DELIVERY] NVARCHAR (500) NULL,
	[DELNOTE_PRICES] NVARCHAR (500) NULL,
	[ORDER_CATAGORY_DES] NVARCHAR (500) NULL,
	[RET_CONT_DES] NVARCHAR (500) NULL,
	[HAULIER_CONSOLIDATE_DES] NVARCHAR (500) NULL,
	[NEWPL_FOR_GIPROB_DES] NVARCHAR (500) NULL,
	[MTM_CARR_CHARGED_DES] NVARCHAR (500) NULL,
	[CUT_OFF_VALUE_DES] NVARCHAR (500) NULL,
	[UPS_ADD_ON_DES] NVARCHAR (500) NULL,
	[ALLOC_PROFORMA_DES] NVARCHAR (500) NULL,
	[PRINT_INV_AT_CONF_DES] NVARCHAR (500) NULL,
	[RESERVE_DAYS_DES] NVARCHAR (500) NULL,
	[JLP_LABELS_DES] NVARCHAR (500) NULL,
	[RF_SCANNER_MEMO_DES] NVARCHAR (MAX) NULL,
	[CONFIRMATION_MEMO_DES] NVARCHAR (MAX) NULL,
	[SS_FIELD1] NVARCHAR (500) NULL,
	[SS_FIELD2] NVARCHAR (500) NULL,
	[SS_FIELD3] NVARCHAR (500) NULL,
	[SS_FIELD4] NVARCHAR (500) NULL,
	[SS_FIELD5] NVARCHAR (500) NULL,
	[CONTRACT_ORDER] NVARCHAR (500) NULL,
	[ADVERT_DES] NVARCHAR (500) NULL,
	[REP_STOCKIST] NVARCHAR (500) NULL,
	[SKIST_CAT] NVARCHAR (500) NULL,
	[UPS_RET_LAB] NVARCHAR (500) NULL,
	[EXIT_CODE_EMAIL] NVARCHAR (500) NULL,
	[CALLED] NVARCHAR (500) NULL,
	[MEMO] NVARCHAR (500) NULL
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.TradeAccountFunctions].[Select]
GO



-- =============================================
-- Author:		Thomas Clark
-- Create date: 21/07/2021
-- Description:	If ParcelNumber is not null / empty, writes to: <40> CUSTOM.TRACK.URL.DES on OEFINDPARCEL 
--											   else, writes to <535> CUSTOM.TRACK.URL.DES on OEDNHEAD 
--													[or the same field mark on OEDNHHIST if the PickingList record was not found on OEDNHEAD]
-- =============================================


/****** Object:  StoredProcedure [clr].[sp_Mertex_Order_ValidateImport]    Script Date: 01/06/2020 17:11:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [clr].[fnc_Mertex_TradeAccount_List]
(
	@UniVerseAccount NVARCHAR(50),
	@AccountOpen BIT = NULL,
	@CreatedSinceDT DATETIME2(2) = NULL,
	@CreatedBeforeDT DATETIME2(2) = NULL
)
RETURNS TABLE
(
	[TradeAccountCode] [NVARCHAR] (50) NULL,
	[AccountOpen] [BIT]
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.EDAReplacement.EDAReplacement].[ListTradeAccounts]
GO





-- =============================================
-- Author:		Thomas Clark
-- Create date: 28/09/2021
-- Description: Searches for Order Head records
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [clr].[fnc_Mertex_OrderHead_Select]
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@OrderHeadIDs NVARCHAR(MAX) NULL, -- Optional, list of known-extant OrderHeadIDs, JSON: [{ OrderHeadID: 12345601 }, ...]
	@FromDT DATETIME2(2) NULL, -- FromDT - only in effect if @OrderHeadIDs is NULL
	@ToDT DATETIME2(2) NULL, -- ToDT - only in effect if @OrderHeadIDs is NULL
	@TradeAccountCode NVARCHAR(50) NULL -- TradeAccountCode - only in effect if @OrderHeadIDs is NULL
)
RETURNS TABLE
(
	[TradeAccountCode] NVARCHAR(50),
	[TradeAccountName] NVARCHAR(50),
	[OrderID] NVARCHAR(25),
	[CustomerOrderReference] NVARCHAR(500),
	[OrderDT] DATETIME NULL,
	[OrderTaker] NVARCHAR(50),
	[OrderPriority] TINYINT NULL,
	[AllocationPriority] TINYINT NULL,
	[HoldOrder] BIT,
	[HoldDelivery] BIT,
	[DeliveryName] NVARCHAR(50),
	[DeliveryAddressLine1] NVARCHAR(60),
	[DeliveryAddressLine2] NVARCHAR(60),
	[DeliveryTown] NVARCHAR(50),
	[DeliveryCounty] NVARCHAR(40),
	[DeliveryPostcode] NVARCHAR(20),
	[DeliveryCountryCode] NVARCHAR(3),
	[HaulierCode] NVARCHAR(10),
	[CarriageCode] NVARCHAR(10),
	[TotalPaintWeight] DECIMAL(18, 3) NULL,
	[PaintMessage] NVARCHAR(500),
	[DeliveryAlertLandlinePhoneNumber] NVARCHAR(20),
	[DeliveryAlertMobilePhoneNumber] NVARCHAR(20),
	[DeliveryAlertEmail] NVARCHAR(254),
	[ChargeOn] NVARCHAR(1),
	[DeliveryChargeSterling] DECIMAL(16,4),
	[DeliveryChargeCurrency] DECIMAL(16,4),
	[HaulierAccountNumberForShippingCharge] NVARCHAR(20),
	[HaulierAccountNumberForDuty] NVARCHAR(20),
	[MertexCurrencyCode] NVARCHAR(3),
	[SterlingExchangeRate] DECIMAL(16,4),
	[DeliveryGroup] NVARCHAR(100),
	[EORI] NVARCHAR(20),
	[DeliveryPhone] NVARCHAR(20),
	[InvoiceAddressName] NVARCHAR(50),
	[InvoiceAddressLine1] NVARCHAR(60),
	[InvoiceAddressLine2] NVARCHAR(60),
	[InvoiceAddressTown] NVARCHAR(50),
	[InvoiceAddressCounty] NVARCHAR(40),
	[InvoiceAddressPostcode] NVARCHAR(20),
	[OrderType] NVARCHAR(20)
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.EDAReplacement.EDAReplacement].[FetchOrderHeads]
GO



-- =============================================
-- Author:		Thomas Clark
-- Create date: 28/09/2021
-- Description: Searches for Order Line records
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [clr].[fnc_Mertex_OrderLine_Select]
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@OrderHeadIDs NVARCHAR(MAX) -- OrderHeadIDs, JSON: [{ OrderHeadID: 12345601 }, ...]
)
RETURNS TABLE
(
	[OrderLineID] NVARCHAR(30),
	[OrderID] NVARCHAR(25),
	[LineNumber] SMALLINT,
	[CustomerLineReference] NVARCHAR(500),
	[ProductCode] NVARCHAR(50),
	[ProductDescription] NVARCHAR(255),
	[QuantityOrdered] DECIMAL(18,2),
	[QuantityCancelled] DECIMAL(18,2) NULL,
	[BackOrderIndicator] BIT,
	[LineStatusID] TINYINT NULL,
	[DetailedLineStatus] NVARCHAR(255) NULL,
	[LineStatusDT] DATETIME NULL,
	[DespatchHeadID] NVARCHAR(25),
	[DespatchLineID] NVARCHAR(30),
	[DiscountPercentage] DECIMAL(18,4) NULL,
	[UnitValueSterling] DECIMAL(18,4) NULL,
	[LineValueSterling] DECIMAL(18,4) NULL,
	[LineDiscountValueSterling] DECIMAL(18,4) NULL,
	[LineVATValueSterling] DECIMAL(18,4) NULL,
	[UnitValueCurrency] DECIMAL(18,4) NULL,
	[LineValueCurrency] DECIMAL(18,4) NULL,
	[LineDiscountValueCurrency] DECIMAL(18,4) NULL,
	[LineVATValueCurrency] DECIMAL(18,4) NULL,
	[VATCode] NVARCHAR(2) NULL,
	[VATRate] DECIMAL(18,4) NULL,
	[EstimatedDespatchDT] DATETIME NULL,
	[StockLocationCode] NVARCHAR(10) NULL,
	[PurchaseOrderLineID] NVARCHAR(30) NULL,
	[PartnerProductCode] NVARCHAR(30) NULL,
	[ReserveFlag] NVARCHAR(1) NULL
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.EDAReplacement.EDAReplacement].[FetchOrderLinesForOrderHeads]
GO





-- =============================================
-- Author:		Thomas Clark
-- Create date: 28/09/2021
-- Description: Searches for Products' Free Stock in Locations
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [clr].[fnc_ProductLocations_Search]
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@ProductCodes NVARCHAR(MAX) NULL,		-- ProductCodes, Optional	}	If these are both NULL, an exception will be raised. Be more specific with your criteria.
	@LocationCodes NVARCHAR(MAX) NULL		-- LocationCodes, Optional	}
)
RETURNS TABLE
(
	[ProductCode] NVARCHAR(50),
	[LocationCode] NVARCHAR(10),
	[FreeStock] DECIMAL(18,2)
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.EDAReplacement.EDAReplacement].[FetchProductLocations]
GO




-- =============================================
-- Author:		Thomas Clark
-- Create date: 28/09/2021
-- Description: Searches for Products' Free Stock in Locations
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [clr].[fnc_Mertex_PurchaseOrderHead_Select]
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@PurchaseOrderHeads NVARCHAR(MAX) NULL,	-- Purchase Order Numbers, Optional	 [{ "PurchaseOrderHeadID": 12345 }]
	@FromDT DATETIME2(2) NULL, -- Optional
	@ToDT DATETIME2(2) NULL -- Optional
)
RETURNS TABLE
(
	[PurchaseOrderHeadID] NVARCHAR(25),
	[OrderID] NVARCHAR(25) NULL,
	[PurchaseOrderStatusID] TINYINT NULL,
	[StockLocationCode] NVARCHAR(10),
	[PurchaseOrderRaisedDT] DATETIME,
	[SupplierCode] NVARCHAR(50),
	[ProcessCode] NVARCHAR(10),
	[AuthoriserCode] NVARCHAR(10),
	[CallOffIndicator] BIT,
	[DeliveryName] NVARCHAR(50),
	[DeliveryAddressLine1] NVARCHAR(60),
	[DeliveryAddressLine2] NVARCHAR(60),
	[DeliveryTown] NVARCHAR(50),
	[DeliveryCounty] NVARCHAR(40),
	[DeliveryPostcode] NVARCHAR(20),
	[DeliveryPhone] NVARCHAR(20)
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.EDAReplacement.EDAReplacement].[FetchPurchaseOrders]
GO




-- =============================================
-- Author:		Thomas Clark
-- Create date: 28/09/2021
-- Description: Searches for Products' Free Stock in Locations
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [clr].[fnc_Mertex_PurchaseOrderLine_Select]
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@PurchaseOrderHeads NVARCHAR(MAX) NULL	-- -- Purchase Order Numbers, Optional	 [{ "PurchaseOrderHeadID": 12345 }]
)
RETURNS TABLE
(
	[PurchaseOrderLineID] NVARCHAR(30),
	[PurchaseOrderHeadID] NVARCHAR(25),
	[OrderID] NVARCHAR(25) NULL,
	[ProductCode] NVARCHAR(50),
	[QuantityOrdered] DECIMAL(18,2),
	[QuantityReceived] DECIMAL(18, 2),
	[PromisedDT] DATETIME NULL,
	[ExpectedDT] DATETIME NULL,
	[DateConfirmed] BIT,
	[Deleted] BIT,
	[LineComplete] BIT,
	[PurchaseOrderLineNumber] SMALLINT NULL,
	[LineUnitPriceSTG] DECIMAL(16,4) NULL,
	[LineUnitPriceCURR] DECIMAL(16,4) NULL,
	[StockUnitCode] NVARCHAR(20) NULL,
	[SupplierProductCode] NVARCHAR(50) NULL,
	[SampleRequired] BIT,
	[DetailedPurchaseOrderLineStatus] NVARCHAR(255)
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.EDAReplacement.EDAReplacement].[FetchPurchaseOrderDetails]
GO





-- =============================================
-- Author:		Thomas Clark
-- Create date: 28/09/2021
-- Description: Searches for Products' Free Stock in Locations
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [clr].[fnc_Mertex_Product_Select]
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@ProductCodes NVARCHAR(MAX) NULL	-- Purchase Order Numbers, Optional	
)
RETURNS TABLE
(
	[Code] NVARCHAR(50),
	[Name] NVARCHAR(100),
	[Status] NCHAR(1),
	[GroupCode] NVARCHAR(10),
	[ItemType] NVARCHAR(10),
	[JLStockReference] NVARCHAR(50),
	[NewJLStockReference] NVARCHAR(50),
	[ManufacturersID] NVARCHAR(50),
	[EUExportCommodityCodeDescription] NVARCHAR(100),
	[EUExportCommodityCode] BIGINT,
	[USCommodityCode] BIGINT,
	[OriginCountry] NVARCHAR(3),
	[LatestCostPrice] DECIMAL(18,2),
	[DefaultSupplierCode] NVARCHAR(50),
	[CollectionCode] NVARCHAR(50),
	[StockCode] NVARCHAR(50),
	[RugSampleProductCode] NVARCHAR(50),
	UpholsteryGrade NVARCHAR(1) NULL,
	DeliveryBand NVARCHAR(10) NULL,
	MaxFoldLength TINYINT NULL,
	Width DECIMAL(18,2) NULL,
	[Length] DECIMAL(18,2) NULL,
	Height DECIMAL(18,2) NULL,
	Diameter DECIMAL(18,2) NULL,
	Size NVARCHAR(50) NULL,
	QualityCode NVARCHAR(50) NULL,
	FlamCode NVARCHAR(1) NULL,
	Martindale INT NULL,
	HorizontalPatternRepeat DECIMAL(18,2) NULL,
	VerticalPatternRepeat DECIMAL(18,2) NULL,
	AdditionalInformation NVARCHAR(300) NULL,
	[PileHeightMM] SMALLINT,
	[BrandCode] NVARCHAR(5) NULL,
	[ColourName] NVARCHAR(50) NULL,
	[DesignName] NVARCHAR(50) NULL,
	[EAN] NVARCHAR(20) NULL,
	[LaunchDate] DATETIME NULL,
	[CarryForwardDate] DATETIME NULL,
	[OrderFormGroup] NVARCHAR(50) NULL,
	[PriceListDescription] NVARCHAR(100) NULL,
	[AdditionalProducts] NVARCHAR(MAX),
	[ImportCommodityCode] NVARCHAR(20),
	[PackagingDescription] NVARCHAR(200)
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.EDAReplacement.EDAReplacement].[FetchProductRecords]
GO



-- =============================================
-- Author:		Thomas Clark
-- Create date: 28/09/2021
-- Description: Searches for Products' Free Stock in Locations
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_ProductCode_Select
(
	@UniVerseAccount NVARCHAR(50) -- Generally "MERTEX"
)
RETURNS TABLE
(
	[Code] NVARCHAR(50)
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.EDAReplacement.EDAReplacement].[FetchProductCodes]
GO



-- =============================================
-- Author:		Thomas Clark
-- Create date: 28/09/2021
-- Description: Selects all OECARRIAGE records
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [clr].[fnc_Mertex_Carriage_Select]
(
	@UniVerseAccount NVARCHAR(50)
)
RETURNS TABLE
(
	[Key] NVARCHAR(50),
	[CarriageCode] NVARCHAR(20),
	[HaulierCode] NVARCHAR(20),
	[Description] NVARCHAR(100),
	[DespatchPathway] NVARCHAR(100),
	[PriorityOrder] BIT,
	[GBIndicator] BIT
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.EDAReplacement.EDAReplacement].[FetchCarriageRecords]
GO





-- =============================================
-- Author:		Thomas Clark
-- Create date: 28/09/2021
-- Description: Selects all OEHAULIER records
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [clr].[fnc_Mertex_Haulier_Select]
(
	@UniVerseAccount NVARCHAR(50)
)
RETURNS TABLE
(
	[HaulierCode] NVARCHAR(10),
	[Description] NVARCHAR(100),
	[CutOffTime] DATETIME NULL,
	[ExpectParcelData] BIT
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.EDAReplacement.EDAReplacement].[FetchHauliers]
GO




-- =============================================
-- Author:		Thomas Clark
-- Create date: 28/09/2021
-- Description: Selects all Sales Market records
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [clr].[fnc_SalesMarkets_Select]
(
	@UniVerseAccount NVARCHAR(50)
)
RETURNS TABLE
(
	[MarketCode] NVARCHAR(50),
	[Name] NVARCHAR(100)
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.EDAReplacement.EDAReplacement].[FetchSalesMarkets]
GO




-- =============================================
-- Author:		Thomas Clark
-- Create date: 15/10/2021
-- Description: Selects all Composition records
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [clr].[fnc_Mertex_Composition_Select]
(
	@UniVerseAccount NVARCHAR(50)
)
RETURNS TABLE
(
	[Code] NVARCHAR(50),
	[Description1] NVARCHAR(50),
	[Description2] NVARCHAR(50),
	[Description3] NVARCHAR(50),
	[Description4] NVARCHAR(50),
	[Description5] NVARCHAR(50),
	[Description] NVARCHAR(250)
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.EDAReplacement.EDAReplacement].[FetchCompositions]
GO



-- =============================================
-- Author:		Thomas Clark
-- Create date: 15/10/2021
-- Description: Selects all Composition records
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [clr].[fnc_Mertex_Product_Aftercare_Select]
(
	@UniVerseAccount NVARCHAR(50)
)
RETURNS TABLE
(
	[ProductCode] NVARCHAR(50),
	[AftercareCode] NVARCHAR(5),
	[SortOrder] TINYINT
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.EDAReplacement.EDAReplacement].[FetchAftercares]
GO


-- =============================================
-- Author:		Thomas Clark
-- Create date: 15/10/2021
-- Description: Selects all Composition records
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [clr].[fnc_Mertex_FlamCode_Select]
(
	@UniVerseAccount NVARCHAR(50)
)
RETURNS TABLE
(
	[Code] NVARCHAR(15),
	[Value] NVARCHAR(300)
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.EDAReplacement.EDAReplacement].[FetchFlamCodes]
GO


-- =============================================
-- Author:		Thomas Clark
-- Create date: 15/10/2021
-- Description: Selects all Purchase Order Price List records
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [clr].[fnc_Mertex_PurchasePrice_Select]
(
	@UniVerseAccount NVARCHAR(50),
	@SupplierProductCodes NVARCHAR(MAX)  -- Optional JSON: [{ "Key": "A0680*CCDG0015" }]
)
RETURNS TABLE
(
	[SupplierCode] NVARCHAR(50),
	[ProductCode] NVARCHAR(50),
	[PurchasePrice] DECIMAL(16,4),
	[QuantityBreak] DECIMAL(18,2),
	[MertexCurrencyCode] NVARCHAR(5)
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.EDAReplacement.EDAReplacement].[FetchPurchasePrices]
GO





-- =============================================
-- Author:		Thomas Clark
-- Create date: 28/09/2021
-- Description: Searches for Order Head records
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [clr].[fnc_Mertex_DespatchHead_Select]
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@DespatchHeadIDs NVARCHAR(MAX) -- JSON: [{ "DespatchHeadID": "2134562-1" }, ...]
)
RETURNS TABLE
(
	[DespatchHeadID] NVARCHAR(25),
	[OrderID] NVARCHAR(25),
	[Source] NVARCHAR(20),
	[DespatchedDT] DATETIME NULL,
	[DespatchedBy] NVARCHAR(30) NULL,
	[DeliveryName] NVARCHAR(50),
	[DeliveryAddressLine1] NVARCHAR(60),
	[DeliveryAddressLine2] NVARCHAR(60),
	[DeliveryTown] NVARCHAR(50),
	[DeliveryCounty] NVARCHAR(40),
	[DeliveryPostcode] NVARCHAR(20),
	[DeliveryCountryCode] NVARCHAR(3),
	[HaulierCode] NVARCHAR(10),
	[CarriageCode] NVARCHAR(10),
	[DeliveryChargeSterling] DECIMAL(16,4),
	[DeliveryChargeCurrency] DECIMAL(16,4),
	[ParcelJSON] NVARCHAR(MAX)
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.EDAReplacement.EDAReplacement].[FetchDespatchNoteHeads]
GO



-- =============================================
-- Author:		Thomas Clark
-- Create date: 28/09/2021
-- Description: Searches for Order Head records
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [clr].[fnc_Mertex_Tuffnells_DespatchHead_Select]
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@DespatchHeadIDs NVARCHAR(MAX), -- JSON: [{ "DespatchHeadID": "2134562-1" }, ...]
	@FromDT DATETIME2(2)
)
RETURNS TABLE
(
	[DespatchHeadID] NVARCHAR(25),
	[OrderID] NVARCHAR(25),
	[Source] NVARCHAR(20),
	[DespatchedDT] DATETIME NULL,
	[DespatchedBy] NVARCHAR(30) NULL,
	[DeliveryName] NVARCHAR(50),
	[DeliveryAddressLine1] NVARCHAR(60),
	[DeliveryAddressLine2] NVARCHAR(60),
	[DeliveryTown] NVARCHAR(50),
	[DeliveryCounty] NVARCHAR(40),
	[DeliveryPostcode] NVARCHAR(20),
	[DeliveryCountryCode] NVARCHAR(3),
	[HaulierCode] NVARCHAR(10),
	[CarriageCode] NVARCHAR(10),
	[DeliveryChargeSterling] DECIMAL(16,4),
	[DeliveryChargeCurrency] DECIMAL(16,4),
	[ParcelJSON] NVARCHAR(MAX)
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.EDAReplacement.EDAReplacement].[FetchTuffnellsDespatchNoteHeads]
GO



-- =============================================
-- Author:		Thomas Clark
-- Create date: 28/09/2021
-- Description: Searches for Order Head records
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [clr].[fnc_Mertex_DespatchLine_Select]
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@DespatchLineIDs NVARCHAR(MAX) -- JSON: [{ "DespatchID": "2134562-1" }, ...]
)
RETURNS TABLE
(
	[DespatchHeadID] NVARCHAR(25),
	[DespatchLineID] NVARCHAR(30),
	[Source] NVARCHAR(20),
	[ProductCode] NVARCHAR(20),
	[ParcelJSON] NVARCHAR(MAX)
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.EDAReplacement.EDAReplacement].[FetchDespatchNoteLines]
GO









-- =============================================
-- Author:		Thomas Clark
-- Create date: 28/09/2021
-- Description: Searches for Order Head records
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [clr].[fnc_Mertex_ExchangeRate_Select]
(
	@UniVerseAccount NVARCHAR(50)
)
RETURNS TABLE
(
	[MertexCurrencyCode] NVARCHAR(5),
	[SterlingExchangeRate] DECIMAL(16,4)
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.EDAReplacement.EDAReplacement].[FetchExchangeRates]
GO





-- =============================================
-- Author:		Thomas Clark
-- Create date: 28/09/2021
-- Description: Searches for Claim Head records
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [clr].[fnc_Mertex_ClaimHead_Select]
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@ClaimIDs NVARCHAR(MAX), -- Optional - JSON: [{ "ClaimID": "2134562-1" }, ...]
	@MarketCodes NVARCHAR(MAX), -- Optional - JSON: [{ "SalesMarketCode": "U" }]
	@FromDT DATETIME2(2), -- Optional
	@ToDT DATETIME2(2) -- Optional
)
RETURNS TABLE
(
	[ClaimID] NVARCHAR(25),
	[OrderID] NVARCHAR(25),
	[DocumentNumber] NVARCHAR(50),
	[InvoiceNumber] NVARCHAR(50),
	[TradeAccountCode] NVARCHAR(50),
	[ClaimCreatedDT] DATETIME,
	[ClaimTaker] NVARCHAR(10),
	[CurrentPerson] NVARCHAR(10),
	[CurrentDepartmentCode] NVARCHAR(10),
	[Completed] BIT,
	[Result] NVARCHAR(10),
	[ContactName] NVARCHAR(50),
	[CollectionName] NVARCHAR(50),
	[CollectionAddressLine1] NVARCHAR(60),
	[CollectionAddressLine2] NVARCHAR(60),
	[CollectionTown] NVARCHAR(50),
	[CollectionCounty] NVARCHAR(40),
	[CollectionPostcode] NVARCHAR(20),
	[CollectionCountryCode] NVARCHAR(3),
	[DeliveryName] NVARCHAR(50),
	[DeliveryAddressLine1] NVARCHAR(60),
	[DeliveryAddressLine2] NVARCHAR(60),
	[DeliveryTown] NVARCHAR(50),
	[DeliveryCounty] NVARCHAR(40),
	[DeliveryPostcode] NVARCHAR(20),
	[DeliveryCountryCode] NVARCHAR(3),
	[CarrierReferencesJSON] NVARCHAR(MAX)
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.EDAReplacement.EDAReplacement].[FetchCCRHeaders]
GO


-- =============================================
-- Author:		Thomas Clark
-- Create date: 28/09/2021
-- Description: Searches for Claim Line records
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [clr].[fnc_Mertex_ClaimLine_Select]
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@ClaimIDs NVARCHAR(MAX) -- Optional - JSON: [{ "ClaimID": "2134562-1" }, ...]
)
RETURNS TABLE
(
	[ClaimID] NVARCHAR(25),
	[ClaimLineID] NVARCHAR(30),
	[OrderLineID] NVARCHAR(30),
	[ProductCode] NVARCHAR(50),
	[FaultCode] NVARCHAR(10),
	[InvoiceNumber] NVARCHAR(50),
	[InvoiceDT] DATETIME,
	[ClaimQuantity] DECIMAL(18,2),
	[AcceptedQuantity] DECIMAL(18,2),
	[ReturnedQuantity] DECIMAL(18,2),
	[CreditAmount] DECIMAL(16,4),
	[ReturnToStock] BIT,
	[OrderPriceSterlingEx] DECIMAL(16,4),
	[DiscountPercentage] DECIMAL(5,4),
	[Notes] NVARCHAR(500) 
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.EDAReplacement.EDAReplacement].[FetchCCRLines]
GO



-- =============================================
-- Author:		Thomas Clark
-- Create date: 28/09/2021
-- Description: Searches for Claim Action records
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [clr].[fnc_Mertex_ClaimAction_Select]
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@ClaimIDs NVARCHAR(MAX) -- Optional - JSON: [{ "ClaimID": "2134562-1" }, ...]
)
RETURNS TABLE
(
	[ClaimID] NVARCHAR(25),
	[ClaimActionID] NVARCHAR(30),
	[ActionedBy] NVARCHAR(10),
	[ActionedByName] NVARCHAR(50),
	[ActionDT] DATETIME,
	[AddedText] NVARCHAR(500),
	[CurrentPerson] NVARCHAR(10),
	[CurrentDepartmentCode] NVARCHAR(10),
	[ContactName] NVARCHAR(50)
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.EDAReplacement.EDAReplacement].[FetchCCRActions]
GO


-- =============================================
-- Author:		Thomas Clark
-- Create date: 28/09/2021
-- Description: Fetches Sundry Codes
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [clr].[fnc_Mertex_Sundry_Select]
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@SundryCodes NVARCHAR(MAX) -- Optional - JSON: [{ "SundryCode": "2134562-1" }, ...]
)
RETURNS TABLE
(
	[SundryCode] NVARCHAR(50),
	[Description] NVARCHAR(255),
	[ProductGroup] NVARCHAR(10),
	[ItemType] NVARCHAR(10),
	[CollectionCode] NVARCHAR(20),
	[Amount] DECIMAL(18,2),
	[PriceEx] DECIMAL(16,4), -- Is it really EX?
	[VATCode] NVARCHAR(2)
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.EDAReplacement.EDAReplacement].[FetchSundries]
GO


-- =============================================
-- Author:		Thomas Clark
-- Create date: 28/09/2021
-- Description: Fetches Sundry Codes
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [clr].[fnc_Mertex_JohnLewisProductReference_Select]
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@JohnLewisOrderReference NVARCHAR(50),
	@LineNumber SMALLINT
)
RETURNS [NVARCHAR](50)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.SQLFunctions.JLProductReference].[Fetch]
GO



-- =============================================
-- Author:		Thomas Clark
-- Create date: 28/09/2021
-- Description: Selects all OECARRIAGE records
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [clr].[fnc_Mertex_PriceCross_Select]
(
	@UniVerseAccount NVARCHAR(50)
)
RETURNS TABLE
(
	[TradePriceListCode] NVARCHAR(10),
	[RRPPriceListCode] NVARCHAR(10)
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.EDAReplacement.EDAReplacement].[FetchPriceCrosses]
GO





-- =============================================
-- Author:		Thomas Clark
-- Create date: 22/12/2022
-- Description: Fetches Quantity Break Cost Pricing 
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [clr].[fnc_Mertex_QuantityBreak_CostPrice_Select]
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@ProductCodeJSON NVARCHAR(MAX),	-- -- Product Codes, Optional	 [{ "Code": "FDG2345/01" }]
	@ProductGroup NVARCHAR(2),
	@Brand NVARCHAR(3)
)
RETURNS TABLE
(
	[ProductCode] NVARCHAR(50),
	[Brand] NVARCHAR(5),
	[SupplierCode] NVARCHAR(50),
	[SupplierName] NVARCHAR(200),
	[ExchangeRate] DECIMAL(8, 2),
	[QuantityBreak] DECIMAL(8, 2),
	[PriceCurrency] DECIMAL(8, 2),
	[PriceSterling] DECIMAL(8, 2),
	[ImportCostsCurrency] DECIMAL(8, 2),
	[ImportCostsSterling] DECIMAL(8, 2),
	[CostPriceCurrency] DECIMAL(8, 2),
	[CostPriceSterling] DECIMAL(8, 2),
	[LastAmendedDT] DATETIME NULL
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.SQLFunctions.QuantityBreakCostPrice].[Select]
GO



-- =============================================
-- Author:		Thomas Clark
-- Create date: 22/12/2022
-- Description: Fetches Quantity Break Cost Pricing 
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [clr].[fnc_Mertex_ProductDetail_Select]
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@ProductCodeJSON NVARCHAR(MAX) -- [{ "Code": "FDG2345/01" }, ...]
)
RETURNS TABLE
(
	[Code] NVARCHAR(50) NULL,
	[Description] NVARCHAR(200) NULL,
	[ProductGroup] NVARCHAR(2) NULL,
	[ItemType] NVARCHAR(3) NULL,
	[MertexSearchDescription] NVARCHAR(200) NULL,
	[UnitOfPurchase] NVARCHAR(10) NULL,
	[UnitOfStock] NVARCHAR(10) NULL,
	[VATCode] NVARCHAR(1) NULL,
	[DesignName] NVARCHAR(100) NULL,
	[PieceLevelStock]BIT,
	[Rack] NVARCHAR(20) NULL,
	[SalesAnalysisA] NVARCHAR(20) NULL,
	[MinimumOrderQuantity] DECIMAL(6,2) NULL,
	[MultipleBatches]BIT,
	[LastCostPrices] NVARCHAR(MAX) NULL,
	[LatestCostPrice] DECIMAL(12,4) NULL,
	[Quality] NVARCHAR(50) NULL,
	[Composition] NVARCHAR(20) NULL,
	[FinishedWidth] DECIMAL(6,1) NULL,
	[SupplierCodes] NVARCHAR(MAX) NULL,
	[SupplierCodePrimary] NVARCHAR(20) NULL,
	[SupplierPartNumber] NVARCHAR(100) NULL,
	[LeadTime] TINYINT NULL,
	[CustomerServiceNotes] NVARCHAR(MAX) NULL,
	[VerticalPatternRepeat] DECIMAL(10,2) NULL,
	[NetWeight] DECIMAL(8,3) NULL,
	[HorizontalPatternRepeat] DECIMAL(10,2) NULL,
	[ExportComomdityCode] NVARCHAR(20) NULL,
	[CountryOfOrigin] NVARCHAR(2) NULL,
	[SupplierUnits] NVARCHAR(10) NULL,
	[ConversionFactor] NVARCHAR(10) NULL,
	[AftercareCodes] NVARCHAR(MAX) NULL,
	[ProductTypeTextFormula] NVARCHAR(5) NULL,
	[SampleLabelText] NVARCHAR(300) NULL,
	[SalesAnalysisD] NVARCHAR(20) NULL,
	[StockAnalysisD] NVARCHAR(20) NULL,
	[FullDescriptions] NVARCHAR(MAX) NULL,
	[AllocateAtBatchLevel]BIT,
	[ValuationCostToUse] NVARCHAR(1) NULL,
	[PatternMatchCode] NVARCHAR(50) NULL,
	[UKFlameRetardancy] NVARCHAR(5) NULL,
	[ContractFlameRetardancy] NVARCHAR(5) NULL,
	[FormCode] NVARCHAR(5) NULL,
	[PatternBook] NVARCHAR(20) NULL,
	[LaunchDT] DATETIME NULL,
	[ManufacturersID] NVARCHAR(50) NULL,
	[StockCode] NVARCHAR(20) NULL,
	[UpholsteryGrade] NVARCHAR(300) NULL,
	[UsageCodes] NVARCHAR(MAX) NULL,
	[MaximumRemnantLength] DECIMAL(5,3) NULL,
	[Status] NVARCHAR(1) NULL,
	[PieceAllocation]BIT,
	[RepetitiveBatches]BIT,
	[SampleProduct]BIT,
	[DropPatternRepeat] DECIMAL(4,1) NULL,
	[PieceBatchOnPickList] NVARCHAR(1) NULL,
	[PaintVOC] NVARCHAR(100) NULL,
	[EAN] NVARCHAR(20) NULL,
	[ImportCommodityCode] NVARCHAR(20) NULL,
	[USCommodityCode] NVARCHAR(20) NULL,
	[FlamCode] NVARCHAR(5) NULL,
	[PieceOnlyOrders]BIT,
	[LinkedDocuments] NVARCHAR(MAX) NULL,
	[LinkedDocumentDescriptions] NVARCHAR(MAX) NULL,
	[TestCodes] NVARCHAR(MAX) NULL,
	[Type] NVARCHAR(30) NULL,
	[Supplier] NVARCHAR(20) NULL,
	[MylandsProductCode] NVARCHAR(50) NULL,
	[ProductClass2] NVARCHAR(20) NULL,
	[CuttingLength] DECIMAL(2,1) NULL,
	[MinimumReplenishment] DECIMAL(6,2) NULL,
	[BulkRack] NVARCHAR(20) NULL,
	[B2BCode] NVARCHAR(1) NULL,
	[AutoAllocateMultipleTimes]BIT,
	[FlameRetardant] NVARCHAR(10) NULL,
	[ClearanceDiscount] DECIMAL(6,2) NULL,
	[Martindale] INT NULL,
	[ShippingLeadTimeWorkingDays] TINYINT NULL,
	[OrderType] NVARCHAR(5) NULL,
	[WebProduct] BIT,
	[LabelLogo] NVARCHAR(1) NULL,
	[FullOrderOnly]BIT,
	[ChargeOn] NVARCHAR(1) NULL,
	[Brand] NVARCHAR(3) NULL,
	[Collection] NVARCHAR(10) NULL,
	[PickListGroup] NVARCHAR(1) NULL,
	[CutterInstructions] NVARCHAR(MAX) NULL,
	[MylandsBaseProductCode] NVARCHAR(20) NULL,
	[ProductDeveloperCode] NVARCHAR(20) NULL,
	[DesignCode] NVARCHAR(100) NULL,
	[SupplierDesignCodes] NVARCHAR(MAX) NULL,
	[SupplierColourNames] NVARCHAR(MAX) NULL,
	[BespokeRug] BIT,
	[RugPileHeight] NVARCHAR(10) NULL,
	[RugPileWeight] NVARCHAR(10) NULL,
	[IncludeInPatternBooks]BIT,
	[AdditionalProduct] NVARCHAR(50) NULL,
	[CostWithComponents] DECIMAL(8,2) NULL,
	[ProductBuyer] NVARCHAR(3) NULL,
	[MostRecentCarryForwardDT] DATETIME NULL,
	[PriceListDescription] NVARCHAR(250) NULL,
	[PriceListPackSize] NVARCHAR(5) NULL,
	[AdditionalText] NVARCHAR(250) NULL,
	[LaunchProduct]BIT,
	[Market] NVARCHAR(3) NULL,
	[PartNumberRangeStart] NVARCHAR(10) NULL,
	[PartNumberRangeEnd] NVARCHAR(10) NULL,
	[PartNumberRangeName] NVARCHAR(10) NULL,
	[PartNumberRangeLastUsed] NVARCHAR(10) NULL,
	[CreateEANCodes]BIT,
	[PurchaseOrderEmailAddress] NVARCHAR(254) NULL,
	[BasketName] NVARCHAR(200) NULL,
	[ProductType] SMALLINT NULL,
	[ColourwayName] NVARCHAR(100) NULL,
	[CostInCurrency] DECIMAL(8,2) NULL,
	[CostPriceTarget] DECIMAL(8,2) NULL,
	[Currency] NVARCHAR(20) NULL,
	[ExchangeRate] DECIMAL(6,2) NULL,
	[FinalCostPriceSterling] DECIMAL(8,2) NULL,
	[CostPriceExcludingAdditional] DECIMAL(8,2) NULL,
	[UKRRPIncVAT] DECIMAL(6,2) NULL,
	[UKRRPExVAT] DECIMAL(6,2) NULL,
	[EuroTradeExVAT] DECIMAL(6,2) NULL,
	[UKGrossMarginPercentage] DECIMAL(8,3) NULL,
	[ProductDeveloperComments] NVARCHAR(200) NULL,
	[MerchandiserCode] INT NULL,
	[OrderFormGroup] SMALLINT NULL,
	[PackagingDescription] NVARCHAR(200) NULL,
	[ShippingTerms] NVARCHAR(20) NULL,
	[KnotsPerSquareMetre] SMALLINT NULL,
	[WeightPerSquareMetre] DECIMAL(5,3) NULL,
	[Transport] NVARCHAR(20) NULL,
	[CostPerSquareMetreCurrency] DECIMAL(6,2) NULL,
	[CostPerSquareMetreSterling] DECIMAL(6,2) NULL,
	[SwatchCost] DECIMAL(6,2) NULL,
	[SalesQuantityForecast_Wholesale] SMALLINT NULL,
	[SalesQuantityForecast_eCommerce] SMALLINT NULL,
	[SalesQuantityForecast_Retail] SMALLINT NULL,
	[SalesQuantityForecast_DGInc] SMALLINT NULL,
	[LengthCM] DECIMAL(6,2) NULL,
	[WidthCM] DECIMAL(6,2) NULL,
	[HeightCM] DECIMAL(6,2) NULL,
	[LengthIN] DECIMAL(6,2) NULL,
	[WidthIN] DECIMAL(6,2) NULL,
	[HeightIN] DECIMAL(6,2) NULL,
	[SizeDescription] NVARCHAR(50) NULL,
	[LengthFeet] DECIMAL(6,2) NULL,
	[WidthFeet] DECIMAL(6,2) NULL,
	[HeightFeet] DECIMAL(6,2) NULL,
	[AttachLithography]BIT,
	[SampleCuttingSize] NVARCHAR(20) NULL,
	[ReturnableSize] NVARCHAR(20) NULL,
	[DigitalDesign]BIT,
	[WideWidth]BIT,
	[SpecialColour]BIT,
	[DiameterCM] DECIMAL(6,2) NULL,
	[OekoTexStandard]BIT,
	[MasterApproved]BIT,
	[PhotoType] NVARCHAR(20) NULL,
	[EngineeredPanel]BIT,
	[VerticalPatternRepeatInches] DECIMAL(6,2) NULL,
	[HorizontalPatternRepeatInches] DECIMAL(6,2) NULL,
	[AdditionalProductCostSterling] DECIMAL(6,2) NULL,
	[WebPattern] NVARCHAR(1) NULL,
	[MaximumFoldSize] TINYINT NULL,
	[BespokeHanger]BIT,
	[BespokeWaterfall]BIT,
	[BespokeShowlength]BIT,
	[PasteTheWall]BIT,
	[RollLength] DECIMAL(6,2) NULL,
	[Leather]BIT,
	[ProductTypeDescription] NVARCHAR(100) NULL,
	[LinkedDesignDifferentWidth] NVARCHAR(12) NULL,
	[ShowlengthInStore]BIT,
	[RollCoverage] DECIMAL(6,2) NULL,
	[StockHanger] NVARCHAR(20) NULL,
	[ShowlengthText] NVARCHAR(100) NULL,
	[PaintCoverageSquareMetres] DECIMAL(6,2) NULL,
	[PaintQualities] NVARCHAR(200) NULL,
	[OrderCutOffTime] NVARCHAR(100) NULL,
	[PaintSheenPercentage] TINYINT NULL,
	[PaintCoverageText] NVARCHAR(100) NULL,
	[PaintSamplePotCode] NVARCHAR(100) NULL,
	[PaintMinimumCoats] TINYINT NULL,
	[JLStockReference] NVARCHAR(50) NULL,
	[Reversible]BIT,
	[WallpaperHangingInstructions] NVARCHAR(100) NULL,
	[RelaunchedDT] DATETIME NULL,
	[CheckWeight]BIT,
	[PaintBase] NVARCHAR(50) NULL,
	[PaintUndercoatColour] NVARCHAR(50) NULL,
	[PaintColourNumber] NVARCHAR(10) NULL,
	[Paint1LitreUndercoatPartNumber] NVARCHAR(20) NULL,
	[Paint2PointFiveLitreUndercoatPartNumber] NVARCHAR(20) NULL,
	[CountryOfManufacture] NVARCHAR(25) NULL,
	[CountryWhereStockHeld] NVARCHAR(25) NULL,
	[LeadTimeWorkingDays] TINYINT NULL,
	[LeadSW] TINYINT NULL,
	[FurnitureSeatType] NVARCHAR(50) NULL,
	[FurnitureCategoryCode] NVARCHAR(20) NULL,
	[ImageType] NVARCHAR(50) NULL,
	[PaintFinish] NVARCHAR(50) NULL,
	[LinkedBespokeHangerPartNumber] NVARCHAR(50) NULL,
	[BulkClearancePriceSterling] DECIMAL(5,2) NULL,
	[LinkedBespokeRugPartNumber] NVARCHAR(50) NULL,
	[LinkedBespokeRugSamplePartNumber] NVARCHAR(50) NULL,
	[LinkedA6PaintCardPartNumber] NVARCHAR(50) NULL,
	[VolumetricWeight] DECIMAL(6,2) NULL,
	[VolumetricLengthCM] DECIMAL(6,2) NULL,
	[VolumetricWidthCM] DECIMAL(6,2) NULL,
	[VolumetricHeightCM] DECIMAL(6,2) NULL,
	[PaintType] NVARCHAR(50) NULL,
	[PoleSize] NVARCHAR(1) NULL,
	[CustomsCommodityDescription] NVARCHAR(100) NULL,
	[JLStockCodeMTM] NVARCHAR(50) NULL,
	[FRLPartNumber] NVARCHAR(50) NULL,
	[FoliaPartNumber] NVARCHAR(50) NULL,
	[PriceListDeliveryBand] NVARCHAR(10) NULL,
	[SpareField11] NVARCHAR(20) NULL,
	[SpareField12] NVARCHAR(20) NULL,
	[SpareField13] NVARCHAR(20) NULL,
	[SpareField14] NVARCHAR(20) NULL
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.ProductFunctions].[Select]
GO



-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Upserts Products
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_Product_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@ProductJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(50)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.ProductFunctions].[Upsert]
GO


-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches AfterCares
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_AfterCare_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@AfterCareCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(5),
	[Descriptions] NVARCHAR(MAX) NULL,
	[QuickDescription] NVARCHAR(50) NULL,
	[ImageName] NVARCHAR(50) NULL,
	[ImageFont] TINYINT NULL,
	[ImageScalingFactor] DECIMAL(5,2) NULL,
	[OffsetX] DECIMAL(5,2) NULL,
	[OffsetY] DECIMAL(5,2) NULL,
	[ImageCharacter] NVARCHAR(1) NULL,
	[Used] BIT 
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.AfterCareFunctions].[Select]
GO


-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts AfterCares
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_AfterCare_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@AfterCareJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(50)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.AfterCareFunctions].[Upsert]
GO





-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches Buyers
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER FUNCTION clr.fnc_Mertex_Buyer_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@BuyerCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(10),
	[Name] NVARCHAR(50) NULL
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.BuyerFunctions].[Select]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts Buyers
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_Buyer_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@BuyerJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(50)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.BuyerFunctions].[Upsert]
GO



-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches Collections
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER FUNCTION clr.fnc_Mertex_Collection_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@CollectionCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(10),
	[Name] NVARCHAR(100) NULL
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.CollectionFunctions].[Select]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts Collections
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_Collection_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@CollectionJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(50)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.CollectionFunctions].[Upsert]
GO




-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches Compositions
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER FUNCTION clr.fnc_Mertex_Composition_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@CompositionCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(20),
	[Description1] NVARCHAR(100) NULL,
	[Description2] NVARCHAR(100) NULL,
	[Description3] NVARCHAR(100) NULL,
	[Description4] NVARCHAR(100) NULL,
	[Description5] NVARCHAR(100) NULL,
	[Description] NVARCHAR(500) NULL
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.CompositionFunctions].[Select]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts Compositions
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_Composition_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@CompositionJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(50)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.CompositionFunctions].[Upsert]
GO




-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches ContractFlameRetardancyImages
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_ContractFlameRetardancyImage_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@ContractFlameCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(2),
	[Description] NVARCHAR(50) NULL,
	[ImageName] NVARCHAR(50) NULL,
	[ImageFont] TINYINT NULL,
	[ImageScalingFactor] DECIMAL(5,2) NULL,
	[OffsetX] DECIMAL(5,2) NULL,
	[OffsetY] DECIMAL(5,2) NULL,
	[ImageCharacter] NVARCHAR(1) NULL
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.ContractFlameRetardancyImageFunctions].[Select]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts ContractFlameRetardancyImages
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_ContractFlameRetardancyImage_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@ContractFlameRetardancyImageJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(50)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.ContractFlameRetardancyImageFunctions].[Upsert]
GO


-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches Designs
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER FUNCTION clr.fnc_Mertex_Design_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@DesignCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(10),
	[Name] NVARCHAR(20) NULL,
	[SortOrder] INT NULL
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.DesignFunctions].[Select]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts Designs
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_Design_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@DesignJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(50)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.DesignFunctions].[Upsert]
GO




-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches FlameCodeDescriptions
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_FlameCode_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@FlameCodeCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(10),
	[Detail] NVARCHAR(100) NULL,
	[BackCoating] NVARCHAR(50) NULL,
	[BackCoatingRequired] NVARCHAR(1) NULL,
	[LabelDescription] NVARCHAR(100) NULL
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.FlameCodeFunctions].[Select]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts FlameCodes
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_FlameCode_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@FlameCodeJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(50)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.FlameCodeFunctions].[Upsert]
GO





-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches FlameRetardants
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_FlameRetardant_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@FlameRetardantCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(10),
	[Name] NVARCHAR(50) NULL
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.FlameRetardantFunctions].[Select]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts FlameRetardants
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_FlameRetardant_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@FlameRetardantJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(50)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.FlameRetardantFunctions].[Upsert]
GO



-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches ItemTypes
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER FUNCTION clr.fnc_Mertex_ItemType_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@ItemTypeCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(5),
	[Name] NVARCHAR(50) NULL
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.ItemTypeFunctions].[Select]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts ItemTypes
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_ItemType_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@ItemTypeJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(50)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.ItemTypeFunctions].[Upsert]
GO



-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches MakeDetails
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_MakeDetail_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@MakeDetailCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[ProductProcessID] NVARCHAR(50),
	[ProductCode] NVARCHAR(20) NULL,
	[ProcessCode] NVARCHAR(20) NULL,
	[ProductCodes] NVARCHAR(MAX) NULL,
	[Quantities] NVARCHAR(MAX) NULL,
	[Units] NVARCHAR(MAX) NULL,
	[Comments] NVARCHAR(MAX) NULL,
	[HiddenReadField] NVARCHAR(1) NULL,
	[AgreedPercentageLosses] NVARCHAR(MAX) NULL,
	[ExpectedPercentageLosses] NVARCHAR(MAX) NULL
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.MakeDetailFunctions].[Select]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts MakeDetails
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_MakeDetail_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@MakeDetailJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(50)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.MakeDetailFunctions].[Upsert]
GO




-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches MarketRestrictions
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER FUNCTION clr.fnc_Mertex_MarketRestriction_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@MarketRestrictionCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(5),
	[Name] NVARCHAR(50) NULL
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.MarketRestrictionFunctions].[Select]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts MarketRestrictions
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_MarketRestriction_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@MarketRestrictionJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(50)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.MarketRestrictionFunctions].[Upsert]
GO



-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches Types
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_Type_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@TypeCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(5),
	[Name] NVARCHAR(50) NULL
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.MertexTypeFunctions].[Select]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts Types
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_Type_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@TypeJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(50)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.MertexTypeFunctions].[Upsert]
GO



-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches OrderFormGroups
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_OrderFormGroup_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@OrderFormGroupCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(10),
	[Name] NVARCHAR(50) NULL,
	[SortOrder] INT NULL
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.OrderFormGroupFunctions].[Select]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts OrderFormGroups
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_OrderFormGroup_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@OrderFormGroupJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(50)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.OrderFormGroupFunctions].[Upsert]
GO






-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches OriginCountries
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_OriginCountry_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@OriginCountryCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(10),
	[Name] NVARCHAR(50) NULL
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.OriginCountryFunctions].[Select]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts OrderFormGroups
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_OriginCountry_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@OriginCountryJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(50)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.OriginCountryFunctions].[Upsert]
GO


-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches Patterns
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER FUNCTION clr.fnc_Mertex_Pattern_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@PatternCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(5),
	[Name] NVARCHAR(50) NULL
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.PatternFunctions].[Select]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts Patterns
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_Pattern_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@PatternJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(50)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.PatternFunctions].[Upsert]
GO


-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches PatternMatches
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER FUNCTION clr.fnc_Mertex_PatternMatch_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@PatternMatchCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(5),
	[Name] NVARCHAR(50) NULL,
	[Full] NVARCHAR(50) NULL,
	[Half] NVARCHAR(50) NULL,
	[Straight] NVARCHAR(50) NULL,
	[Match] NVARCHAR(50) NULL,
	[Random] NVARCHAR(50) NULL
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.PatternMatchFunctions].[Select]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts PatternMatchs
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_PatternMatch_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@PatternMatchJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(50)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.PatternMatchFunctions].[Upsert]
GO



-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches ProductDevelopers
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER FUNCTION clr.fnc_Mertex_ProductDeveloper_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@ProductDeveloperCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(5),
	[Name] NVARCHAR(50) NULL
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.ProductDeveloperFunctions].[Select]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts ProductDevelopers
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_ProductDeveloper_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@ProductDeveloperJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(50)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.ProductDeveloperFunctions].[Upsert]
GO



-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches ProductGroups
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER FUNCTION clr.fnc_Mertex_ProductGroup_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@ProductGroupCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(10),
	[Name] NVARCHAR(50) NULL
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.ProductGroupFunctions].[Select]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts ProductGroups
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_ProductGroup_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@ProductGroupJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(50)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.ProductGroupFunctions].[Upsert]
GO


-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches ProductTemplates
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER FUNCTION clr.fnc_Mertex_ProductTemplate_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@ProductTemplateCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(50),
	[Name] NVARCHAR(50) NULL,
	[FirstUserScreen] NVARCHAR(20) NULL,
	[ProductGroupCode] NVARCHAR(5) NULL,
	[ItemTypeCode] NVARCHAR(5) NULL,
	[UnitOfPurchase] NVARCHAR(10) NULL,
	[UnitOfStock] NVARCHAR(10) NULL,
	[VATCode] NVARCHAR(10) NULL,
	[PieceLevelStock] BIT,
	[SalesAnalysisA] INT NULL,
	[MultipleBatches] BIT,
	[RemeasureLength] DECIMAL(5,2) NULL,
	[ProductType] NVARCHAR(10) NULL,
	[SalesAnalysisD] INT NULL,
	[StockAnalysisD] INT NULL,
	[AllocateAtBatchLevel] BIT,
	[ValuationCostToUse] NVARCHAR(1) NULL,
	[FormCode] NVARCHAR(5) NULL,
	[MaximumRemnantLength] DECIMAL(8,2) NULL,
	[CurrentStatus] NVARCHAR(1) NULL,
	[PieceAllocation] BIT,
	[RepetitiveBatches] BIT,
	[SampleProduct] BIT,
	[PieceBatchOnPickList] BIT,
	[PieceOnlyOrders] BIT,
	[GeneralLedgerDiscountGroupCode] NVARCHAR(20) NULL,
	[CuttingLength] DECIMAL(8,2) NULL,
	[MinimumReplacement] DECIMAL(8,2) NULL,
	[BulkRack] NVARCHAR(20) NULL,
	[AutoAllocateMultipleTimes] BIT,
	[MadeToMeasureType] NVARCHAR(1) NULL,
	[Logo] NVARCHAR(1) NULL,
	[FullOrderOnly] BIT,
	[ChargeOn] NVARCHAR(1) NULL,
	[Brand] NVARCHAR(3) NULL,
	[PickingListGroupCode] NVARCHAR(5) NULL,
	[BespokeRug] BIT,
	[PartNumberRangeStart] NVARCHAR(5) NULL,
	[PartNumberRangeEnd] NVARCHAR(5) NULL,
	[PartNumberPrefix] NVARCHAR(10) NULL,
	[PartNumberRangeLastUsed] NVARCHAR(10) NULL,
	[CreateEANCodes] BIT
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.ProductTemplateFunctions].[Select]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts ProductTemplates
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_ProductTemplate_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@ProductTemplateJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(50)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.ProductTemplateFunctions].[Upsert]
GO


-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches ProductTypes
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_ProductType_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@ProductTypeCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[ProductTypeID] SMALLINT,
	[Name] NVARCHAR(50) NULL,
	[MertexDescription] NVARCHAR(MAX) NULL,
	[PriceListDescription] NVARCHAR(MAX) NULL,
	[OnlineProductDescription] NVARCHAR(MAX) NULL,
	[DisplayName] NVARCHAR(MAX) NULL,
	[DisplayNamePriceList] NVARCHAR(MAX) NULL,
	[DisplayNameBasket] NVARCHAR(MAX) NULL,
	[MertexSearchDescription] NVARCHAR(MAX) NULL,
	[DisplayNameSort] NVARCHAR(MAX) NULL,
	[FileName] NVARCHAR(50) NULL,
	[PriceListDescription2] NVARCHAR(MAX) NULL
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.ProductTypeFunctions].[Select]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts ProductTypes
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_ProductType_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@ProductTypeJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(50)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.ProductTypeFunctions].[Upsert]
GO





-- =============================================
-- Author:		Thomas Clark
-- Create date: 22/12/2022
-- Description: Fetches Quantity Break Cost Pricing 
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_Quality_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@QualityCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[QualityCode] NVARCHAR(50),
	[Name] NVARCHAR(100),
	[CompositionCode] NVARCHAR(10),
	[Width] DECIMAL(8,2),
	[WeightGramsPerSquareMetre] DECIMAL(8, 2),
	[WeightGramsPerLinearMetre] DECIMAL(8, 2),
	[FinishedWidth] DECIMAL(8, 2),
	[RemeasureLength] DECIMAL(8, 2),
	[SeparateMaterialLength] BIT,
	[AlternativeRawMaterials] BIT,
	[CreatedDT] DATETIME NULL
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.QualityFunctions].[Select]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts Qualities
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_Quality_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@QualityJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(50)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.QualityFunctions].[Upsert]
GO





-- =============================================
-- Author:		Thomas Clark
-- Create date: 22/12/2022
-- Description: Fetches Suppliers
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER FUNCTION clr.fnc_Mertex_Supplier_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@SupplierCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(50),
	[Name] NVARCHAR(100),
	[AddressLine1] NVARCHAR(60),
	[AddressLine2] NVARCHAR(60),
	[Town] NVARCHAR(50),
	[County] NVARCHAR(40),
	[Postcode] NVARCHAR(20),
	[CountryCode] NVARCHAR(3),
	[Currency] NVARCHAR(10),
	[DutyPercentage] DECIMAL(8,2),
	[InlandChargesPercentage] DECIMAL(8, 2),
	[MiscellaneousPercentage] DECIMAL(8, 2),
	[ImportChargePerUnit] DECIMAL(8, 2),
	[MiscellaneousCharge] DECIMAL(8, 2)
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.SupplierFunctions].[Select]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts Suppliers
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_Supplier_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@SupplierJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(50)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.SupplierFunctions].[Upsert]
GO


-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches Tests
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER FUNCTION clr.fnc_Mertex_Test_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@ProductGroupCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(10),
	[Name] NVARCHAR(50) NULL,
	[Market] NVARCHAR(30) NULL
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.TestFunctions].[Select]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts Tests
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_Test_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@TestJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(50)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.TestFunctions].[Upsert]
GO



-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches UKFlameRetardancyImages
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_UKFlameRetardancyImage_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@UKFlameCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(2),
	[Description] NVARCHAR(50) NULL,
	[ImageName] NVARCHAR(50) NULL,
	[ImageFont] TINYINT NULL,
	[ImageScalingFactor] DECIMAL(5,2) NULL,
	[OffsetX] DECIMAL(5,2) NULL,
	[OffsetY] DECIMAL(5,2) NULL,
	[ImageCharacter] NVARCHAR(1) NULL
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.UKFlameRetardancyImageFunctions].[Select]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts UKFlameRetardancyImages
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_UKFlameRetardancyImage_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@UKFlameRetardancyImageJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(50)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.UKFlameRetardancyImageFunctions].[Upsert]
GO



-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches Usages
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_Usage_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@UsageCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(2),
	[Descriptions] NVARCHAR(MAX) NULL,
	[QuickDescription] NVARCHAR(50) NULL,
	[ImageName] NVARCHAR(50) NULL,
	[ImageFont] TINYINT NULL,
	[ImageScalingFactor] DECIMAL(5,2) NULL,
	[OffsetX] DECIMAL(5,2) NULL,
	[OffsetY] DECIMAL(5,2) NULL,
	[ImageCharacter] NVARCHAR(1) NULL
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.UsageFunctions].[Select]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts Usages
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_Usage_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@UsageJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(50)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.UsageFunctions].[Upsert]
GO




-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches Forms
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_Form_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@FormCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(5),
	[Name] NVARCHAR(50) NULL
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.FormFunctions].[Select]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts Forms
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_Form_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@FormJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(50)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.FormFunctions].[Upsert]
GO



-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches General Ledger Discount Groups
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_GeneralLedgerDiscountGroup_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@GeneralLedgerDiscountGroupCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(20),
	[Name] NVARCHAR(50) NULL
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.GeneralLedgerDiscountGroupFunctions].[Select]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts GeneralLedgerDiscountGroups
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_GeneralLedgerDiscountGroup_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@FormJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(50)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.GeneralLedgerDiscountGroupFunctions].[Upsert]
GO




-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches General Ledger Accounts
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_GeneralLedgerAccount_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@GeneralLedgerAccountCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(20),
	[Name] NVARCHAR(50) NULL
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.GeneralLedgerAccountFunctions].[Select]
GO


-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts GeneralLedgerAccounts
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_GeneralLedgerAccount_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@GeneralLedgerAccountJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(50)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.GeneralLedgerAccountFunctions].[Upsert]
GO





-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches General Ledger Analysis Codes
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_GeneralLedgerAnalysisCode_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@GeneralLedgerAnalysisCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(5),
	[Name] NVARCHAR(50) NULL
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.GeneralLedgerAnalysisCodeFunctions].[Select]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts GeneralLedgerAnalysisCodes
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_GeneralLedgerAnalysisCode_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@GeneralLedgerAnalysisCodeJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(50)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.GeneralLedgerAnalysisCodeFunctions].[Upsert]
GO





-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches General Ledger Code Lists
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_GeneralLedgerCodeList_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@GeneralLedgerCodeListCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] INT,
	[Name] NVARCHAR(50) NULL
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.GeneralLedgerCodeListFunctions].[Select]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts GeneralLedgerCodeLists
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_GeneralLedgerCodeList_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@GeneralLedgerCodeListJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(50)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.GeneralLedgerCodeListFunctions].[Upsert]
GO






-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches Statuses
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_Status_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@StatusCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(1),
	[Name] NVARCHAR(50) NULL
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.StatusFunctions].[Select]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts Statuses
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_Status_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@StatusJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(50)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.StatusFunctions].[Upsert]
GO






-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches Brands
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_Brand_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@BrandCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(10),
	[Name] NVARCHAR(50) NULL,
	[SortOrder] TINYINT NULL
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.BrandFunctions].[Select]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts Brands
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_Brand_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@BrandJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(50)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.BrandFunctions].[Upsert]
GO






-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches VAT Codes
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_VAT_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@VATCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(10),
	[Name] NVARCHAR(50) NULL,
	[Rate] DECIMAL(8,2) NULL,
	[ReportIndicator] NVARCHAR(1) NULL,
	[SortOrder] TINYINT NULL
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.VATCodeFunctions].[Select]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts VATCodes
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_VAT_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@VATJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(50)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.VATCodeFunctions].[Upsert]
GO








-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches Racks
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

alter FUNCTION clr.fnc_Mertex_Rack_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@RackCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(20),
	[Name] NVARCHAR(50) NULL,
	[WarehouseRoute] NVARCHAR(50) NULL,
	[Pickable] BIT,
	[Type] NVARCHAR(1) NULL
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.RackFunctions].[Select]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts Racks
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_Rack_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@RackJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(50)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.RackFunctions].[Upsert]
GO










-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches Rack Groups
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_RackGroup_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@RackGroupCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(10),
	[PickedBy] NVARCHAR(10) NULL
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.RackGroupFunctions].[Select]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts RackGroups
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_RackGroup_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@RackGroupJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(50)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.RackGroupFunctions].[Upsert]
GO











-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches Radio Terminal Users
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_RadioTerminalUser_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@RadioTerminalUserCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(10),
	[Name] NVARCHAR(50) NULL
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.RadioTerminalUserFunctions].[Select]
GO


-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts RadioTerminalUsers
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_RadioTerminalUser_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@RadioTerminalUserJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(50)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.RadioTerminalUserFunctions].[Upsert]
GO








-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches Product Exclusions
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_ProductExclusion_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@ProductExclusionCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Key] NVARCHAR(50),
	[ProductCode] NVARCHAR(50)
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.ProductExclusionFunctions].[Select]
GO




-- =============================================
-- Author:		Thomas Clark
-- Create date: 28/09/2021
-- Description: Searches for Order Head records
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [clr].[fnc_Mertex_InvoiceHead_Select]
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@InvoiceIDs NVARCHAR(MAX) NULL
)
RETURNS TABLE
(
	[InvoiceID] NVARCHAR(25),
	[InvoiceDT] DATETIME NULL,
	[TradeAccountCode] NVARCHAR(50),
	[OrderID] NVARCHAR(25),
	[InvoiceOrCredit] TINYINT,
	[CarriageValue] DECIMAL(16,4),
	[CarriageValueCurrency] DECIMAL(16,4),
	[MertexCurrencyCode] NVARCHAR(10)
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.InvoiceFunctions.InvoiceHeadFunctions].[Select]
GO


-- =============================================
-- Author:		Thomas Clark
-- Create date: 28/09/2021
-- Description: Searches for Order Head records
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [clr].[fnc_Mertex_InvoiceHead_Initialise_Select]
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@FromDT DATETIME2(2) NULL,
	@ToDT DATETIME2(2) NULL
)
RETURNS TABLE
(
	[InvoiceID] NVARCHAR(25)
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.InvoiceFunctions.InvoiceHeadFunctions].[InitialiseSelect]
GO



-- =============================================
-- Author:		Thomas Clark
-- Create date: 28/09/2021
-- Description: Searches for Order Head records
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [clr].[fnc_Mertex_InvoiceLine_Select]
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@InvoiceLineIDsJSON NVARCHAR(MAX) NULL, -- [{ "DocumentNumber": "SI123843*3" }, { ... }]
	@InvoiceIDsJSON NVARCHAR(MAX) NULL  -- [{  "DocumentNumber": "SI123843" }, { ... }] 
)
RETURNS TABLE
(
	[InvoiceLineID] NVARCHAR(25),
	[InvoiceID] NVARCHAR(25),
	[OrderLineID] NVARCHAR(30),
	[ProductCode] NVARCHAR(50),
	[QuantityOrdered] DECIMAL(16,2),
	[QuantityDespatched] DECIMAL(16,2),
	[QuantityInvoiced] DECIMAL(16,2),
	[UnitPriceExVAT] DECIMAL(16,4),
	[LinePriceExVAT] DECIMAL(16,4),
	[DiscountPercentage] DECIMAL(5,2),
	[VATCode] NVARCHAR(10),
	[VATRate] DECIMAL(5,2),
	[DiscountValue] DECIMAL(16,4),
	[VATValue] DECIMAL(16,4),
	[StockUnit] NVARCHAR(10),
	[UnitCost] DECIMAL(16,4),
	[UnitCostCurrency] DECIMAL(16,4),
	[LineCost] DECIMAL(16,4),
	[LineCostCurrency] DECIMAL(16,4),
	[UnitPriceExVATCurrency] DECIMAL(16,4),
	[LineValueExVATCurrency] DECIMAL(16,4),
	[VATValueCurrency] DECIMAL(16,4),
	[GeneralLedgerCode] NVARCHAR(20)
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.InvoiceFunctions.InvoiceLineFunctions].[Select]
GO




-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches ItemTypes
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_MadeToMeasureType_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@MadeToMeasureTypeCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(1),
	[Name] NVARCHAR(50) NULL
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.MadeToMeasureTypeFunctions].[Select]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts ItemTypes
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_MadeToMeasureType_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@MadeToMeasureTypeJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(50)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.MadeToMeasureTypeFunctions].[Upsert]
GO




-- =============================================
-- Author:		Thomas Clark
-- Create date: 21/04/2023
-- Description: Fetches PickingListGroups
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_PickingListGroup_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@PickingListGroupCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(10),
	[Name] NVARCHAR(50) NULL
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.PickingListGroupFunctions].[Select]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 21/04/2023
-- Description: Upserts PickingListGroups
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_PickingListGroup_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@PickingListGroupJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(50)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.PickingListGroupFunctions].[Upsert]
GO



-- =============================================
-- Author:		Thomas Clark
-- Create date: 21/04/2023
-- Description: Fetches Delivery Addresses
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_DeliveryAddress_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@DeliveryAddressReferences NVARCHAR(MAX),
	@TradeAccountCodes NVARCHAR(MAX),
	@SampleSize INT = NULL
)
RETURNS TABLE
(
	[DeliveryReference] NVARCHAR(20),
	[TradeAccountCode] NVARCHAR(50),
	[ContactName] NVARCHAR(50) NULL,
	[CompanyName] NVARCHAR(35) NULL,
	[AddressLine1] NVARCHAR(60) NULL,
	[AddressLine2] NVARCHAR(60) NULL,
	[Town] NVARCHAR(50) NULL,
	[County] NVARCHAR(40) NULL,
	[Postcode] NVARCHAR(20) NULL,
	[CountryCode] NVARCHAR(2) NULL,
	[PhoneNumber] NVARCHAR(20) NULL,
	[Email] NVARCHAR(254) NULL,
	[DomesticOrTrade] NVARCHAR(1) NULL,
	[HaulierCode] NVARCHAR(10) NULL,
	[CarriageCode] NVARCHAR(20) NULL,
	[USTaxExemptionCertificate] NVARCHAR(100) NULL,
	[USStateCode] NVARCHAR(2) NULL,
	[USCountyCode] NVARCHAR(5) NULL
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.AccountFunctions.DeliveryAddressFunctions].[Select]
GO


-- =============================================
-- Author:		Thomas Clark
-- Create date: 21/04/2023
-- Description: Fetches Delivery Addresses
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_DeliveryAddress_Search
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@TradeAccountCode NVARCHAR(MAX),
	@SearchTerm NVARCHAR(MAX),
	@SampleSize INT = NULL
)
RETURNS TABLE
(
	[DeliveryReference] NVARCHAR(20),
	[TradeAccountCode] NVARCHAR(50),
	[ContactName] NVARCHAR(50) NULL,
	[CompanyName] NVARCHAR(35) NULL,
	[AddressLine1] NVARCHAR(60) NULL,
	[AddressLine2] NVARCHAR(60) NULL,
	[Town] NVARCHAR(50) NULL,
	[County] NVARCHAR(40) NULL,
	[Postcode] NVARCHAR(20) NULL,
	[CountryCode] NVARCHAR(2) NULL,
	[PhoneNumber] NVARCHAR(20) NULL,
	[Email] NVARCHAR(254) NULL,
	[DomesticOrTrade] NVARCHAR(1) NULL,
	[HaulierCode] NVARCHAR(10) NULL,
	[CarriageCode] NVARCHAR(20) NULL,
	[USTaxExemptionCertificate] NVARCHAR(100) NULL,
	[USStateCode] NVARCHAR(2) NULL,
	[USCountyCode] NVARCHAR(5) NULL
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.AccountFunctions.DeliveryAddressFunctions].[Search]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 21/04/2023
-- Description: Upserts Delivery Addresses
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_DeliveryAddress_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@DeliveryAddressJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(50)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.AccountFunctions.DeliveryAddressFunctions].[Upsert]
GO
 

-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches StockUnits
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_StockUnit_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@StockUnitCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(10),
	[Name] NVARCHAR(30) NULL
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.UnitFunctions].[Select]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts StockUnits
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_StockUnit_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@StockUnitJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(50)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.UnitFunctions].[Upsert]
GO


-- =============================================
-- Author:		Thomas Clark
-- Create date: 21/04/2023
-- Description: Selects Trade Accounts
-- IF YOU CHANGE THE RETURN TABLE TYPE HERE, ALSO CHANGE FOR clr.fnc_Mertex_EndCustomer_Select
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_TradeAccountDetail_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@TradeAccountCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[TradeAccountCode] NVARCHAR(50),
	[Status] NVARCHAR(1),
	[CreatedDT] DATETIME,
	[ModifiedDT] DATETIME,
	[Name] NVARCHAR(100),
	[ContactName] NVARCHAR(100),
	[SortName] NVARCHAR(100),
	[PhoneNumber] NVARCHAR(50),
	[Email] NVARCHAR(254),
	[MarketingEmail] NVARCHAR(254),
	[Website] NVARCHAR(200),
	[CompanyRegistrationNumber] NVARCHAR(50),
	[EORINumber] NVARCHAR(50),
	[TaxID] NVARCHAR(50),
	[Currency] NVARCHAR(3),
	[TradeAccountGroup] NVARCHAR(3),
	[CustomerType] NVARCHAR(2),
	[CustomerClass] NVARCHAR(2),
	[OrderCategory] NVARCHAR(2),
	[Market] NVARCHAR(1),
	[PriceList] NVARCHAR(10),
	[SalesDivision] NVARCHAR(3),
	[InternalOrExternal] NVARCHAR(1),
	[MiscellaneousAccount] BIT,
	[PriceCode] NVARCHAR(3),
	[ContractOrDisplay] NVARCHAR(1),
	[RetailOrContract] NVARCHAR(1),
	[Advertiser] BIT,
	[Stockist] BIT,
	[NoEmail] BIT,
	[SalesRepresentativeCode] NVARCHAR(3),
	[SalesAgentCode] NVARCHAR(3),
	[SalesArea] NVARCHAR(10),
	[InvoiceAddressLine1] NVARCHAR(150),
	[InvoiceAddressLine2] NVARCHAR(100),
	[InvoiceAddressTown] NVARCHAR(100),
	[InvoiceAddressCounty] NVARCHAR(100),
	[InvoiceAddressPostcode] NVARCHAR(20),
	[InvoiceAddressCountryCode] NVARCHAR(3),
	[InvoiceAccounts] NVARCHAR(MAX),
	[DefaultDeliveryReference] NVARCHAR(20),
	[SendInvoiceToDeliveryAddress] NVARCHAR(1),
	[PaymentTerms] NVARCHAR(10),
	[CreditLimit] INT,
	[CreditStopped] BIT,
	[BankCode] NVARCHAR(3),
	[CreditController] NVARCHAR(10),
	[CreditLimitExpiryDT] DATETIME,
	[HighValueOrderCheckExempt] BIT,
	[InsuredCreditLimit] INT,
	[CreditLimitCheckAgainst] TINYINT,
	[InsuredCreditLimitExpiryDT] DATETIME,
	[Bankrupt] NVARCHAR(20),
	[ExcludeFromInvoicePrint] BIT,
	[SendOverdueLetters] BIT,
	[OverdueInvoiceGracePeriod] TINYINT,
	[OverdueLetter1Delay] TINYINT,
	[OverdueLetter2Delay] TINYINT,
	[OverdueLetter3Delay] TINYINT,
	[OverdueLetter4Delay] TINYINT,
	[OverdueLetter5Delay] TINYINT,
	[EDIInvoice] BIT,
	[CommercialInvoice] BIT,
	[ProformaTrading] BIT,
	[ProformaSurcharge] DECIMAL(8,2),
	[InvoiceDetailPrint] NVARCHAR(1),
	[CreditNoteCopiesCount] TINYINT,
	[ABCPricing] BIT,
	[InvoiceCopiesCount] TINYINT,
	[ContractPricing] BIT,
	[PrintInvoiceDetails] BIT,
	[EndCustomerRequired] BIT,
	[EndCustomerInvoicing] BIT,
	[CreditCardDetails] NVARCHAR(1),
	[StatementPrint] BIT,
	[RetailTillCustomer] BIT,
	[PermitTillCreditNotes] BIT,
	[StatementAccount] NVARCHAR(50),
	[StatementAccountName] NVARCHAR(100),
	[StatementAddressCounty] NVARCHAR(40),
	[StatementAddressLine1] NVARCHAR(60),
	[StatementAddressLine2] NVARCHAR(60),
	[StatementAddressTown] NVARCHAR(50),
	[StatementAddressPostcode] NVARCHAR(20),
	[StatementContactName] NVARCHAR(100),
	[StatementPhoneNumber] NVARCHAR(50),
	[PurchaseOrderRequired] BIT,
	[AnalysisCodeA] NVARCHAR(12),
	[AnalysisCodeD] NVARCHAR(2),
	[AnalysisCodeS] NVARCHAR(2),
	[Memo] NVARCHAR(500),
	[RFScannerMemo] NVARCHAR(200),
	[ConfirmationMemo] NVARCHAR(200),
	[ExtendedMemos] NVARCHAR(MAX),
	[FreeOfChargePromotionCode] NVARCHAR(5),
	[SellingCompany] NVARCHAR(10),
	[DistributorReference] NVARCHAR(10),
	[TransferAccount] BIT,
	[StockOnlyAccount] BIT,
	[VATNormalRateCode] NVARCHAR(1),
	[VATNumber] NVARCHAR(30),
	[VATCountry] NVARCHAR(3),
	[VATLastCheckedDT] DATETIME,
	[VATNumberValid] NVARCHAR(3),
	[VATNumberInvalid] NVARCHAR(50),
	[VATInvoiceDisplay] NVARCHAR(50),
	[HaulierCode] NVARCHAR(10),
	[CarriageCode] NVARCHAR(10),
	[UPSInvoiceAccountNumber] NVARCHAR(20),
	[UPSTaxesDutyAccountNumber] NVARCHAR(20),
	[DHLAccountNumber] NVARCHAR(20),
	[TNTAccountNumber] NVARCHAR(20),
	[DeliveryTerms] NVARCHAR(3),
	[GroupageType] NVARCHAR(1),
	[DirectDelivery] BIT,
	[PackingListCopiesCount] TINYINT,
	[ApplyMinimumCarriageValueSurcharge] BIT,
	[DeliveryChangeReconfirmation] BIT,
	[MinimumOrderQuantity] TINYINT,
	[ReceivingLocation] NVARCHAR(10),
	[CutVerify] TINYINT,
	[OutOfStockLetter] BIT,
	[DirectToCustomer] NVARCHAR(20),
	[FullOrderOnly] BIT,
	[UPSReturnsLabel] BIT,
	[UPSAddOnPercentage] DECIMAL(4,1),
	[DefaultOrderPriority] TINYINT,
	[PartnerCompany] NVARCHAR(20),
	[AcceptSundries] BIT,
	[SampleStockLocation] NVARCHAR(10),
	[SampleHaulierCode] NVARCHAR(10),
	[SampleCarriageCode] NVARCHAR(10),
	[DefaultStockLocation] NVARCHAR(10),
	[Export] BIT,
	[ExportWithNoPackingNote] BIT,
	[Consolidate] BIT,
	[CarriageSurchargeExempt] NVARCHAR(1),
	[OrderConfirmationPrint] NVARCHAR(1),
	[SendCancellationLetters] BIT,
	[InterimStatementActions] BIT,
	[InformOnPromiseDateChanged] NVARCHAR(1),
	[ProductScreen] BIT,
	[CheckForDuplicates] BIT,
	[MeasurePiece] BIT,
	[AllocateStockForProformaOrders] BIT,
	[PrintInvoiceAtConfirmation] BIT,
	[PrintUnitPriceOnDeliveryNote] BIT,
	[PrintJLPEanLabels] BIT,
	[NewPickingListForGoodsInProblemItems] BIT,
	[MTMCarriageCharged] BIT,
	[StandardRollLength] DECIMAL(4,1),
	[FileProtocol] NVARCHAR(1),
	[UserID] NVARCHAR(20),
	[SampleLetterTitleText] NVARCHAR(2),
	[ProductExclusionListCode] NVARCHAR(3),
	[DeliveryDateCalculation] NVARCHAR(2),
	[MinimumOrderValue] DECIMAL(8,2),
	[TillIDCode] NVARCHAR(10),
	[UsualOrderUnit] NVARCHAR(5),
	[StoreName] NVARCHAR(20),
	[TransferAddress] NVARCHAR(200),
	[TransferPassword] NVARCHAR(20),
	[HaulierCodeForConsolidatedInvoice] NVARCHAR(3),
	[CutOffValue] INT,
	[DaysForwardForReservations] TINYINT,
	[SSField1] NVARCHAR(50),
	[SSField2] NVARCHAR(50),
	[SSField4] NVARCHAR(50),
	[SSField5] NVARCHAR(50),
	[StockistCategory] NVARCHAR(2),
	[ExitCodeEmail] NVARCHAR(254)
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.TradeAccountFunctions.TradeAccountFunctions].[Select]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 21/04/2023
-- Description: Upserts Trade Accounts
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER FUNCTION clr.fnc_Mertex_TradeAccount_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@TradeAccountJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(50)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.TradeAccountFunctions.TradeAccountFunctions].[Upsert]
GO


-- =============================================
-- Author:		Thomas Clark
-- Create date: 21/04/2023
-- Description: Selects End Customer Trade Accounts
-- IF YOU CHANGE THE RETURN TABLE TYPE HERE, ALSO CHANGE FOR clr.fnc_Mertex_TradeAccountDetail_Select
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_EndCustomer_Select
(
	@UniVersAccount NVARCHAR(50)
)
RETURNS TABLE
(
	[ParentTradeAccountCode] NVARCHAR(50),
	[TradeAccountCode] NVARCHAR(50)
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.TradeAccountFunctions.TradeAccountFunctions].[EndCustomerSelect]
GO



-- =============================================
-- Author:		Thomas Clark
-- Create date: 21/04/2023
-- Description: Selects End Customer Trade Accounts
-- IF YOU CHANGE THE RETURN TABLE TYPE HERE, ALSO CHANGE FOR clr.fnc_Mertex_TradeAccountDetail_Select
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_EndCustomer_Search
(
	@TradeAccountDataSourceJSON NVARCHAR(MAX), -- Generally "MERTEX"
	@SearchTerm NVARCHAR(200)
)
RETURNS TABLE
(
	[TradeAccountCode] NVARCHAR(50)
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.TradeAccountFunctions.TradeAccountFunctions].[EndCustomerSearch]
GO



-- =============================================
-- Author:		Thomas Clark
-- Create date: 21/04/2023
-- Description: Fetches Delivery Addresses
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

alter FUNCTION clr.fnc_Mertex_DeliveryAddress_Exists
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@TradeAccountCode NVARCHAR(50),
	@CompanyName NVARCHAR(35) NULL,
	@ContactName NVARCHAR(50) NULL,
	@AddressLine1 NVARCHAR(60) NULL,
	@AddressLine2 NVARCHAR(60) NULL,
	@Town NVARCHAR(50) NULL,
	@County NVARCHAR(40) NULL,
	@Postcode NVARCHAR(20) NULL,
	@CountryCode NVARCHAR(2) NULL
)
RETURNS TABLE
(
	[DeliveryReference] NVARCHAR(20)
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.AccountFunctions.DeliveryAddressFunctions].[Exists]
GO


-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches Reasons
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER FUNCTION clr.fnc_Mertex_Reason_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@ReasonCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(20),
	[Name] NVARCHAR(200) NULL
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.GeneralFunctions.ReasonFunctions].[Select]
GO


-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches Reasons
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER FUNCTION clr.fnc_Mertex_Reason_Search
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@SearchTerm NVARCHAR(MAX),
	@SampleSize INT
)
RETURNS TABLE
(
	[Code] NVARCHAR(10),
	[Name] NVARCHAR(500) NULL
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.GeneralFunctions.ReasonFunctions].[Search]
GO


-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts Reasons
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_Reason_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@ReasonJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(50)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.GeneralFunctions.ReasonFunctions].[Upsert]
GO




-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches ExchangeRates
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER FUNCTION clr.fnc_Mertex_ExchangeRate_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@ExchangeRateCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(10),
	[Name] NVARCHAR(50),
	[Rate] DECIMAL(8,6)
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.AccountFunctions.ExchangeRateFunctions].[Select]
GO


-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts ExchangeRates
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_ExchangeRate_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@ExchangeRateJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(50)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.AccountFunctions.ExchangeRateFunctions].[Upsert]
GO



-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches ShippingTerms
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_ShippingTerm_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@ShippingTermCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(5),
	[Name] NVARCHAR(50)
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.ShippingTermsFunctions].[Select]
GO


-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts ShippingTerms
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_ShippingTerm_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@ShippingTermJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(5)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.ShippingTermsFunctions].[Upsert]
GO



-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches Transports
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_Transport_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@TransportCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(5),
	[Name] NVARCHAR(50)
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.TransportFunctions].[Select]
GO


-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts Transports
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_Transport_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@TransportJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(5)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.ProductFunctions.TransportFunctions].[Upsert]
GO




-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Checks for Extant Order References
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER FUNCTION clr.fnc_Mertex_OrderReference_DuplicateCheck
(
	@TradeDataSourceJSON NVARCHAR(MAX), -- Generally "MERTEX"
	@OrderReference NVARCHAR(50),
	@MonthsBack TINYINT
)
RETURNS TABLE
(
	[OrderID] NVARCHAR(50)
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.AccountFunctions.DuplicateChecks].[OrderReference]
GO




-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Checks for Extant Order References
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_RetailTill_Select
(
	@UniVerseAccount NVARCHAR(50)
)
RETURNS TABLE
(
	[TillNumber] NVARCHAR(5),
	[Name] NVARCHAR(50)
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.RetailFunctions.TillFunctions].[FetchOpenTills]
GO



-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Checks for Extant Order References
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER FUNCTION clr.fnc_Mertex_RetailTillSession_Select
(
	@UniVerseAccount NVARCHAR(50)
)
RETURNS TABLE
(
	[SessionNumber] NVARCHAR(10),
	[Open] BIT,
	[OpenedBy] NVARCHAR(50),
	[OpenedDT] DATETIME,
	[TillNumber] NVARCHAR(5),
	[Label] NVARCHAR(50)
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.RetailFunctions.TillFunctions].[FetchRetailTillSessions]
GO





-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches Transports
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_TradeAccountClass_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@TradeAccountClassCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(10),
	[Name] NVARCHAR(50)
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.TradeAccountFunctions.TradeAccountClassFunctions].[Select]
GO


-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts Transports
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_TradeAccountClass_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@TradeAccountClassJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(10)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.TradeAccountFunctions.TradeAccountClassFunctions].[Upsert]
GO


-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches Transports
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_TradeAccountType_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@TradeAccountTypeCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(10),
	[Name] NVARCHAR(50)
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.TradeAccountFunctions.TradeAccountTypeFunctions].[Select]
GO


-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts Transports
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_CustomerType_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@CustomerTypeJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(10)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.TradeAccountFunctions.TradeAccountTypeFunctions].[Upsert]
GO


-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches Transports
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_InvoiceTerms_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@InvoiceTermCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(10),
	[Name] NVARCHAR(50)
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.TradeAccountFunctions.InvoiceTermsFunctions].[Select]
GO


-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts Transports
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_InvoiceTerm_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@InvoiceTermJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(10)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.TradeAccountFunctions.InvoiceTermsFunctions].[Upsert]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches Transports
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_DeliveryTerms_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@DeliveryTermCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(10),
	[Name] NVARCHAR(50)
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.GeneralFunctions.DeliveryTermsFunctions].[Select]
GO


-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts Transports
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_DeliveryTerm_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@InvoiceTermJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(10)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.GeneralFunctions.DeliveryTermsFunctions].[Upsert]
GO


-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches Transports
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_TradeAccountGroup_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@DeliveryTermCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(10),
	[Name] NVARCHAR(50)
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.TradeAccountFunctions.TradeAccountGroupFunctions].[Select]
GO


-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts Transports
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_TradeAccountGroup_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@TradeAccountGroupJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(10)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.TradeAccountFunctions.TradeAccountGroupFunctions].[Upsert]
GO







-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches Transports
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_OrderCategory_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@OrderCategoryCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(10),
	[Name] NVARCHAR(50)
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.GeneralFunctions.OrderCategoryFunctions].[Select]
GO


-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts Transports
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_OrderCategory_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@OrderCategoryJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(10)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.GeneralFunctions.OrderCategoryFunctions].[Upsert]
GO




-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches Transports
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_StockLocation_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@StockLocationCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(10),
	[Name] NVARCHAR(50)
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.StockFunctions.StockLocationFunctions].[Select]
GO


-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts Transports
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_StockLocation_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@StockLocationJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(10)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.StockFunctions.StockLocationFunctions].[Upsert]
GO

-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches Transports
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_FreeOfChargeAccountingCode_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@FreeOfChargeAccountCodeCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(10),
	[Name] NVARCHAR(50)
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.PricingFunctions.FreeOfChargeAccountingCodeFunctions].[Select]
GO


-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts Transports
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_FreeOfChargeAccountingCode_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@FreeOfChargeAccountCodeJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(10)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.PricingFunctions.FreeOfChargeAccountingCodeFunctions].[Upsert]
GO




-- =============================================
-- Author:		Thomas Clark
-- Create date: 27/01/2023
-- Description: Fetches Transports
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_SalesDivision_Select
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@Mertex_SalesDivisionCodes NVARCHAR(MAX)
)
RETURNS TABLE
(
	[Code] NVARCHAR(10),
	[Name] NVARCHAR(50)
)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.PricingFunctions.SalesDivisionFunctions].[Select]
GO


-- =============================================
-- Author:		Thomas Clark
-- Create date: 03/02/2023
-- Description: Upserts Transports
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION clr.fnc_Mertex_SalesDivision_Upsert
(
	@UniVerseAccount NVARCHAR(50), -- Generally "MERTEX"
	@Mertex_SalesDivisionJSON NVARCHAR(MAX)
)
RETURNS NVARCHAR(10)
AS
EXTERNAL NAME [MertexerV2].[MertexerV2.PricingFunctions.SalesDivisionFunctions].[Upsert]
GO

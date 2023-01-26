// Decompiled with JetBrains decompiler
// Type: IBMU2.UODOTNET.UniTokens
// Assembly: UODOTNET, Version=2.1.0.7149, Culture=neutral, PublicKeyToken=335f3fbd4be82339
// MVID: E7861CAA-9C07-4D40-B738-BD78A3391394
// Assembly location: C:\Users\tclark\source\repos\MertexerV2\MertexerV2\ExternalDependencies\UODOTNET.dll

namespace IBMU2.UODOTNET
{
  internal class UniTokens
  {
    public const string UVCS_SERVICE = "uvcs";
    public const string UDCS_SERVICE = "udcs";
    public const string UNI_SERVICE = "defcs";
    public const string UNIVERSE = "UNIVERSE";
    public const string UNIDATA = "UNIDATA";
    public const int COMMS_VERSION = 4;
    public const int CS_VERSION = 6;
    public const int OLD_COMMS_VERSION = 3;
    public const int OLD_CS_VERSION = 4;
    public const int INTERNAL_FLAG = 0;
    public const int NO_COMPRESSION = 0;
    public const int INTERNAL_COMPRESSION = 1;
    public const int DEFAULT_SOCKET_TIMEOUT = 30000;
    public const int NO_ENCRYPTION = 0;
    public const int UVT_NONE = 0;
    public const int UVT_UNIX = 1;
    public const int UVT_NT = 2;
    public const int OLD_SERVER = 1;
    public const int NEW_SERVER = 2;
    public const int IM = 0;
    public const int FM = 1;
    public const int VM = 2;
    public const int SVM = 3;
    public const int TM = 4;
    public const int SQLNULL = 5;
    public const int NUM_MARK_CHARACTERS = 6;
    public const int UVE_NOERROR = 0;
    public const int UVE_ENOENT = 14002;
    public const int UVE_EIO = 14005;
    public const int UVE_EACCESS = 14013;
    public const int UVE_EINVAL = 14022;
    public const int UVE_ENFILE = 14023;
    public const int UVE_EMFILE = 14024;
    public const int UVE_ENOSPC = 14028;
    public const int UVE_NETUNREACH = 14551;
    public const int UVE_BFN = 22001;
    public const int UVE_BTS = 22002;
    public const int UVE_IID = 22003;
    public const int UVE_LRR = 22004;
    public const int UVE_NFI = 22005;
    public const int UVE_RNF = 30001;
    public const int UVE_LCK = 30002;
    public const int UVE_USC = 30096;
    public const int UVE_SELFAIL = 30097;
    public const int UVE_LOCKINVALID = 30098;
    public const int UVE_SEEKFAILED = 30101;
    public const int UVE_INVALIDATKEY = 30103;
    public const int UVE_UNABLETOLOADSUB = 30105;
    public const int UVE_BADNUMARGS = 30106;
    public const int UVE_SUBERROR = 30107;
    public const int UVE_ITYPEFTC = 30108;
    public const int UVE_ITYPEFAILEDTOLOAD = 30109;
    public const int UVE_ITYPENOTCOMPILED = 30110;
    public const int UVE_BADITYPE = 30111;
    public const int UVE_INVALIDFILENAME = 30112;
    public const int UVE_WEOFFAILED = 30113;
    public const int UVE_EXECUTEISACTIVE = 30114;
    public const int UVE_EXECUTENOTACTIVE = 30115;
    public const int UVE_TX_ACTIVE = 30124;
    public const int UVE_CANT_ACCESS_PF = 30125;
    public const int UVE_FAIL_TO_CANCEL = 30126;
    public const int UVE_INVALID_INFO_KEY = 30127;
    public const int UVE_CREATE_FAILED = 30128;
    public const int UVE_DUPHANDLE_FAILED = 30129;
    public const int UVE_NVR = 31000;
    public const int UVE_NPN = 31001;
    public const int UVE_NODATA = 39101;
    public const int UVE_AT_INPUT = 39119;
    public const int UVE_SESSION_NOT_OPEN = 39120;
    public const int UVE_UVEXPIRED = 39121;
    public const int UVE_CSVERSION = 39122;
    public const int UVE_COMMSVERSION = 39123;
    public const int UVE_BADDIR = 39125;
    public const int UVE_BAD_UVHOME = 39127;
    public const int UVE_INVALIDPATH = 39128;
    public const int UVE_INVALIDACCOUNT = 39129;
    public const int UVE_BAD_UVACCOUNT_FILE = 39130;
    public const int UVE_FTA_NEW_ACCOUNT = 39131;
    public const int UVE_ULR = 39134;
    public const int UVE_NO_NLS = 39135;
    public const int UVE_MAP_NOT_FOUND = 39136;
    public const int UVE_NO_LOCALE = 39137;
    public const int UVE_LOCALE_NOT_FOUND = 39138;
    public const int UVE_CATEGORY_NOT_FOUND = 39139;
    public const int UVE_SR_SLAVE_READ_FAIL = 39207;
    public const int UVE_INVALIDFIELD = 40001;
    public const int UVE_SESSIONEXISTS = 40002;
    public const int UVE_BADPARAM = 40003;
    public const int UVE_NOMORE = 40005;
    public const int UVE_NOTATINPUT = 40006;
    public const int UVE_INVALID_DATAFIELD = 40007;
    public const int UVE_BAD_DICTIONARY_ENTRY = 40008;
    public const int UVE_BAD_CONVERSION_DATA = 40009;
    public const int UVE_FILE_NOT_OPEN = 45000;
    public const int UVE_OPENSESSION_ERR = 45001;
    public const int UVE_NONNULL_RECORDID = 45002;
    public const int UVE_BAD_LOGINNAME = 80011;
    public const int UVE_BAD_PASSWORD = 80019;
    public const int UVE_ACCOUNT_EXPIRED = 80144;
    public const int UVE_RUN_REMOTE_FAILED = 80147;
    public const int UVE_UPDATE_USER_FAILED = 80148;
    public const int UNISESSION_NO_MORE_CONNECTIONS = 81007;
    public const int UNISESSION_CANT_FIND_SERVICE = 81014;
    public const int UNISESSION_TIMEOUT = 81015;
    public const int UNISESSION_REFUSED = 81016;
    public const int UVE_MLTPLEX_SECURE_SESSION = 84444;
    public const int UVE_NOT_A_PROXY_SESSION = 84445;
    public const int UVE_BAD_SSL_MODE = 84446;
    public const int UVE_SUCCESS_WITH_INFO = 88888;
    public const int UVE_MUST_USE_SESSION = 88889;
    public const int EIC_ALPHA = 1;
    public const int EIC_CLEARDATA = 2;
    public const int EIC_CLEARFILE = 3;
    public const int EIC_CLEARSELECT = 4;
    public const int EIC_CLOSE = 5;
    public const int EIC_CLOSESEQ = 6;
    public const int EIC_COL1 = 7;
    public const int EIC_COL2 = 8;
    public const int EIC_CONVERT = 9;
    public const int EIC_COUNT = 10;
    public const int EIC_DATA = 11;
    public const int EIC_DATE = 12;
    public const int EIC_DCOUND = 13;
    public const int EIC_DELETE = 14;
    public const int EIC_EXECUTE = 15;
    public const int EIC_FIELD = 17;
    public const int EIC_FIELDSTORE = 18;
    public const int EIC_FILEINFO = 19;
    public const int EIC_FILELOCK = 20;
    public const int EIC_FILEUNLOCK = 21;
    public const int EIC_FMT = 22;
    public const int EIC_FORMLIST = 23;
    public const int EIC_GETLIST = 24;
    public const int EIC_GETVALUE = 25;
    public const int EIC_ICONV = 26;
    public const int EIC_INDICES = 27;
    public const int EIC_INSERT = 28;
    public const int EIC_ITYPE = 29;
    public const int EIC_LOCATE = 30;
    public const int EIC_LOCK = 31;
    public const int EIC_LOWER = 32;
    public const int EIC_OCONV = 33;
    public const int EIC_OPEN = 34;
    public const int EIC_OPENSEQ = 35;
    public const int EIC_RAISE = 36;
    public const int EIC_READ = 37;
    public const int EIC_READBLK = 38;
    public const int EIC_READLIST = 39;
    public const int EIC_READNEXT = 40;
    public const int EIC_READSEQ = 41;
    public const int EIC_READV = 42;
    public const int EIC_RECORDLOCK = 43;
    public const int EIC_RECORDLOCKED = 44;
    public const int EIC_RELEASE = 45;
    public const int EIC_REPLACE = 46;
    public const int EIC_SEEK = 47;
    public const int EIC_SELECT = 48;
    public const int EIC_SELECTINDEX = 49;
    public const int EIC_SETVALUE = 50;
    public const int EIC_STRDEL = 51;
    public const int EIC_SUBCALL = 52;
    public const int EIC_TIME = 53;
    public const int EIC_TIMEDATE = 54;
    public const int EIC_TIMEOUT = 55;
    public const int EIC_TRANS = 56;
    public const int EIC_TRANSACTION = 57;
    public const int EIC_TRIM = 58;
    public const int EIC_UNLOCK = 59;
    public const int EIC_WEOFSEQ = 60;
    public const int EIC_WRITE = 61;
    public const int EIC_WRITEBLK = 62;
    public const int EIC_WRITECONTINUE = 63;
    public const int EIC_WRITESEQ = 64;
    public const int EIC_WRITEV = 65;
    public const int EIC_EXECUTECONTINUE = 66;
    public const int EIC_INPUTREPLY = 67;
    public const int EIC_CANCEL = 68;
    public const int EIC_SESSIONINFO = 69;
    public const int EIC_SETMAP = 70;
    public const int EIC_GETMAP = 71;
    public const int EIC_SETLOCALE = 72;
    public const int EIC_GETLOCALE = 73;
    public const int EIC_GETVERSION = 74;
    public const int EIC_GETSERVERINFO = 75;
    public const int EIC_READSET = 76;
    public const int EIC_READFIELDSET = 77;
    public const int EIC_READNAMEDFIELDSET = 78;
    public const int EIC_DELETESET = 79;
    public const int EIC_WRITESET = 80;
    public const int EIC_WRITEFIELDSET = 81;
    public const int EIC_WRITENAMEDFIELDSET = 82;
    public const int EIC_LOCKSET = 83;
    public const int EIC_UNLOCKSET = 84;
    public const int EIC_LASTMESSAGE = 85;
    public const int EIC_POOLOFF = 86;
    public const int EIC_INVALID_OP_POOLON = 87;
    public const int EIC_NONPOOLED_SESSION_ACTIVE = 88;
    public const int EIC_CANNOT_CHANGE_FROM_POOLED_TO_NONPOOLED = 89;
    public const int EIC_POOL_EXISTS = 90;
    public const int EIC_VERIFY_U2VERSION = 91;
    public const int EIC_CPOFF_VERIFY_U2VERSION = 92;
    public const int EIC_MINPOOL_MORETHAN_LICENSE = 93;
    public static readonly byte IM_CHAR = byte.MaxValue;
    public static readonly byte FM_CHAR = 254;
    public static readonly byte VM_CHAR = 253;
    public static readonly byte SVM_CHAR = 252;
    public static readonly byte TM_CHAR = 251;
    public static readonly byte SQLNULL_CHAR = 128;
    public static readonly byte NULL_CHAR = 0;
    public static readonly string AT_IM;
    public static readonly string AT_FM;
    public static readonly string AT_VM;
    public static readonly string AT_SVM;
    public static readonly string AT_TM;
    public static readonly string AT_SQLNULL;
    public static readonly string AT_NULL;
    public static readonly byte[] defaultMarkArray = new byte[6]
    {
      UniTokens.IM_CHAR,
      UniTokens.FM_CHAR,
      UniTokens.VM_CHAR,
      UniTokens.SVM_CHAR,
      UniTokens.TM_CHAR,
      UniTokens.SQLNULL_CHAR
    };

    static UniTokens()
    {
      UniTokens.AT_IM = UniTokens.IM_CHAR.ToString();
      UniTokens.AT_FM = UniTokens.FM_CHAR.ToString();
      UniTokens.AT_VM = UniTokens.VM_CHAR.ToString();
      UniTokens.AT_SVM = UniTokens.SVM_CHAR.ToString();
      UniTokens.AT_TM = UniTokens.TM_CHAR.ToString();
      UniTokens.AT_SQLNULL = UniTokens.SQLNULL_CHAR.ToString();
      UniTokens.AT_NULL = UniTokens.NULL_CHAR.ToString();
    }
  }
}

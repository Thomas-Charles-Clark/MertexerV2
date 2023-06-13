using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using MertexerV2.Models.Accounts;
using MertexerV2.Models.EDAReplacement;
using MertexerV2.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Tester.Utilities
{
    static class TesterFramework
    {
        public static readonly char ValueMark = (char)253;
        public static readonly char SubValueMark = (char)252;

        public static readonly char[] SpaceDelimiter = new char[] { ' ' };
        public static readonly char[] AsteriskDelimiter = new char[] { '*' };
        public static readonly char[] ValueMarkDelimiter = new char[] { (char)253 };
        public static readonly char[] SubValueMarkDelimiter = new char[] { (char)252 };

        public static readonly string WavyLineBreak = $"\r\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\r\n";

        private static ConsoleColor StoredColour;

        public static void SetConsoleColourGreen()
        {
            StoredColour = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Green;
        }

        public static void SetConsoleColourTesting()
        {
            StoredColour = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Yellow;
        }

        public static void SetConsoleColourError()
        {
            StoredColour = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Red;
        }

        public static void RestoreConsoleColour()
        {
            Console.ForegroundColor = StoredColour;
        }


        public static string GetDaySuffix(int day)
        {
            switch (day)
            {
                case 1:
                case 21:
                case 31:
                    return "st";
                case 2:
                case 22:
                    return "nd";
                case 3:
                case 23:
                    return "rd";
                default:
                    return "th";
            }
        }

        public static string FetchAccountDataSourceJson(string tradeAccountNumber)
        {
#if DEBUG
            using (var conn = new SqlConnection("Data Source=vm-sql1;Initial Catalog=Flock;Integrated Security=True"))
#else
            using (var conn = new SqlConnection("context connection=true"))
#endif
            {
                try
                {
                    return conn.Query<string>("SELECT UniVerseDataSources FROM dbo.TradeAccount WHERE Code = @TradeAccountNumber", new { tradeAccountNumber }).Single();
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public static string FetchTradeAccountListJson(int count, string tradeAccountCode)
        {
#if DEBUG
            using (var conn = new SqlConnection("Data Source=vm-sql1;Initial Catalog=Flock;Integrated Security=True"))
#else
            using (var conn = new SqlConnection("context connection=true"))
#endif
            {
                try
                {
                    return conn.Query<string>($"SELECT (SELECT TOP {count} Code FROM dbo.TradeAccount WHERE (@TradeAccountCode IS NULL OR @TradeAccountCode IS NOT NULL AND Code = @TradeAccountCode) AND (Status = 'O') ORDER BY TradeAccountID FOR JSON AUTO)", new { tradeAccountCode }).Single();
                }
                finally
                {
                    conn.Close(); 
                }
            }
        }

        public static string FetchTradeAccountUpsertJSON(string tradeAccountCode)
        {
            return @"{
                ""Code"": ""K0098"", 
                ""TradeAccountContacts"": [{
                    ""Name"":""Jess"",
                    ""Email"":""bobdole@northpole.org"",
                    ""Phone"":""0207 893 7740"",
                    ""Mobile"":""07880 241 650"",
                    ""Type"":""07""
                },{
                    ""Name"":""Name"",
                    ""Email"":""Email"",
                    ""Phone"":""Phone"",
                    ""Mobile"":""Mobile"",
                    ""Type"":""20""
                },{
                    ""Name"":""Ms Sarah Waller"",
                    ""Email"":""pfearn@designersguild.com"",
                    ""Phone"":""0207 893 7740"",
                    ""Mobile"":""2078937739"",
                    ""Type"":""06""
                }]
            }";

#if DEBUG
            using (var conn = new SqlConnection("Data Source=vm-sql1;Initial Catalog=Flock;Integrated Security=True"))
#else
            using (var conn = new SqlConnection("context connection=true"))
#endif
            {
                try
                {
                    var cmd = conn.CreateCommand();

                    cmd.CommandText = "Mertex.TradeAccount_JSON_Select";

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@TradeAccountCode", tradeAccountCode);

                    if (conn.State != System.Data.ConnectionState.Open)
                        conn.Open();

                    var reader = cmd.ExecuteReader();

                    var sb = new StringBuilder();

                    while (reader.Read())
                    {
                        sb.Append(reader.GetString(0));
                    }

#if DEBUG
                    var obj = JToken.Parse(sb.ToString());

                    Console.WriteLine(JsonConvert.SerializeObject(obj, Framework.JsonSettingsIndentedIncludeNullsAlphabetical));
#endif

                    return sb.ToString();
                }
                finally
                {
                    conn.Close();
                }
            }

        }

        public static string ModifyReserveLine(string tradeDataSourceJson, PassReserveLine objLine)
        {
            return JsonConvert.SerializeObject(MertexerV2.Accounts.AccountFunctions.ModifyReserveLine(tradeDataSourceJson, objLine), Formatting.Indented);
        }

        public static string FetchProductCodes(int count)
        {
#if DEBUG
            using (var conn = new SqlConnection("Data Source=vm-sql1;Initial Catalog=Flock;Integrated Security=True"))
#else
            using (var conn = new SqlConnection("context connection=true"))
#endif
            {
                try
                {
                    return conn.Query<string>($"SELECT (SELECT TOP {count} Code FROM dbo.Product WHERE (Status = 'C' AND Deleted = 0 AND ProductTypeID = 1) ORDER BY Code FOR JSON AUTO)").Single();
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public static string FetchOrderHeadIDs(int count)
        {
#if DEBUG
            using (var conn = new SqlConnection("Data Source=vm-sql1;Initial Catalog=Flock;Integrated Security=True"))
#else
            using (var conn = new SqlConnection("context connection=true"))
#endif
            {
                try
                {
                    return conn.Query<string>($"SELECT (SELECT TOP {count} OrderHeadID AS OrderID FROM mbr.OrderHead WHERE (AcceptedByMertexDT IS NOT NULL AND Deleted = 0 AND OrderHeadID <= 1006726601) ORDER BY OrderHeadID DESC FOR JSON AUTO)").Single();
                    //return conn.Query<string>(
                    //    $@"SELECT (	
	                   //     SELECT	OH.OrderID AS OrderID 
	                   //     FROM	Mertex.OrderHead AS OH 
			                 //       LEFT OUTER JOIN Mertex.OrderLine AS OL ON OL.OrderID = OH.OrderID 
	                   //     GROUP BY 
		                  //      OH.OrderID 
	                   //     HAVING 
		                  //      COUNT(OL.OrderLineID) = 0
	                   //     ORDER BY 
		                  //      OH.OrderID DESC
                    //    FOR JSON AUTO)").Single();
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public static string FetchOrderHeadIDsStaticList()
        {
#if DEBUG
            using (var conn = new SqlConnection("Data Source=vm-sql1;Initial Catalog=Flock;Integrated Security=True"))
#else
            using (var conn = new SqlConnection("context connection=true"))
#endif
            {
                try
                {
                    return conn.Query<string>($@"SELECT (SELECT OrderID FROM Mertex.OrderHead WHERE (OrderID IN (
'991054601',
'991078801',
'991094601',
'991099501',
'991107902',
'991131401',
'991155501',
'991160301',
'991166301',
'991168401',
'991171701',
'991186301',
'991196901',
'991208301',
'991223701',
'991248101',
'991435901',
'991438301',
'991556102',
'991601401',
'991612001',
'991622701',
'991654201',
'991695401',
'991702901',
'991715201',
'991766202',
'991769801',
'991792801',
'991846601',
'991891201',
'991943603',
'991965601',
'992020101',
'992085301',
'992085302',
'992101401',
'992110001',
'992129001',
'992163301',
'992173201',
'992174101',
'992221701',
'992291601',
'992292001',
'992330201',
'992402201',
'992435501',
'992436401',
'992464101',
'992499001',
'992509701',
'992532601',
'992604102',
'992646001',
'992710402',
'992712701',
'992731501',
'992784501',
'992841501',
'992864201',
'992911702',
'992944901',
'992949202',
'992959501',
'992973101',
'992973102',
'992982001',
'992982002',
'993005601',
'993009001',
'993050501',
'993053901',
'993054001',
'993056001',
'993061101',
'993073702',
'993138901',
'993147201',
'993154701',
'993160001',
'993164701',
'993170901',
'993210801',
'993213601',
'993223402',
'993290401',
'993295701',
'993308801',
'993313901',
'993357301',
'993360001',
'993393201',
'993404701',
'993417401',
'993417802',
'993424801',
'993428801',
'993431905',
'993440001',
'993460501',
'993469901',
'993539701',
'993575701',
'993585501',
'993643801',
'993649601',
'993685701',
'993751201',
'993760201',
'993801301',
'993805401',
'993819801',
'993886701',
'993898001',
'993901901',
'993936601',
'994039701',
'994047701',
'994051001',
'994052001',
'994070901',
'994086801',
'994099401',
'994180401',
'994191901',
'994199701',
'994201201',
'994209001',
'994209701',
'994221601',
'994227301',
'994227901',
'994243201',
'994243801',
'994268801',
'994271301',
'994299502',
'994306501',
'994312101',
'994362001',
'994368202',
'994429801',
'994430201',
'994431801',
'994432401',
'994463101',
'994464701',
'994472201',
'994508501',
'994526501',
'994539201',
'994539301',
'994570601',
'994594701',
'994594801',
'994599901',
'994612801',
'994670601',
'994672901',
'994694901',
'994697901',
'994708701',
'994709001',
'994709301',
'994720501',
'994760201',
'994760301',
'994769901',
'994772701',
'994784001',
'994805801',
'994807501',
'994827101',
'994835101',
'994837701',
'994837901',
'994837902',
'994849901',
'994857201',
'994871501',
'994917501',
'994925501',
'994933001',
'994934101',
'994934302',
'994936301',
'994964701',
'995005901',
'995027701',
'995033201',
'995033701',
'995034503',
'995045801',
'995085701',
'995094801',
'995147601',
'995151901',
'995159502',
'995162601',
'995165901',
'995175201',
'995175202',
'995183001',
'995220901',
'995220903',
'995223419',
'995223423',
'995229601',
'995244701',
'995248202',
'995248301',
'995329401',
'995334501',
'995373101',
'995400802',
'995401101',
'995408001',
'995428501',
'995446901',
'995454601',
'995466902',
'995472001',
'995487501',
'995499901',
'995501001',
'995508501',
'995526301',
'995526801',
'995543301',
'995548001',
'995557801',
'995608501',
'995639401',
'995671101',
'995672101',
'995691301',
'995698002',
'995698701',
'995702501',
'995712501',
'995746101',
'995746102',
'995758001',
'995773301',
'995778701',
'995784801',
'995796901',
'995827101',
'995831401',
'995842501',
'995843501',
'995847101',
'995856101',
'995864003',
'995872201',
'995872301',
'995874101',
'995875701',
'995880501',
'995884001',
'995885901',
'995889701',
'995911301',
'995920101',
'995920501',
'995939201',
'995955901',
'995963201',
'995964601',
'995975301',
'995977401',
'995990701',
'995992901',
'996002901',
'996009001',
'996021201',
'996022601',
'996023301',
'996023801',
'996025001',
'996043001',
'996043401',
'996060101',
'996061003',
'996067501',
'996073201',
'996079701',
'996104701',
'996109301',
'996123201',
'996128401',
'996129101',
'996132901',
'996137901',
'996140601',
'996145401',
'996160402',
'996166101',
'996169802',
'996175801',
'996190501',
'996195201',
'996201501',
'996241001',
'996242701',
'996244201',
'996245401',
'996252601',
'996295401',
'996301001',
'996302001',
'996307001',
'996314401',
'996323801',
'996326601',
'996333701',
'996341601',
'996349801',
'996350901',
'996354501',
'996356202',
'996364701',
'996366501',
'996368601',
'996372901',
'996373101',
'996384701',
'996394101',
'996394102',
'996421202',
'996427601',
'996456201',
'996457402',
'996457403',
'996464302',
'996482101',
'996506601',
'996511201',
'996516301',
'996517501',
'996523901',
'996526401',
'996531302',
'996535901',
'996540801',
'996544701',
'996546702',
'996547701',
'996555901',
'996560101',
'996568601',
'996586001',
'996599001',
'996612901',
'996615001',
'996616002',
'996621001',
'996636901',
'996645601',
'996646501',
'996653501',
'996660701',
'996669801',
'996672302',
'996674901',
'996675401',
'996679401',
'996688202',
'996690201',
'996691901',
'996692302',
'996715901',
'996717301',
'996724001',
'996725201',
'996726401',
'996733301',
'996747901',
'996751101',
'996752801',
'996756302',
'996757602',
'996772001',
'996772901',
'996774201',
'996796301',
'996806301',
'996811901',
'996819102',
'996829501',
'996830801',
'996830802',
'996839701',
'996843001',
'996859501',
'996863803',
'996863805',
'996864801',
'996871201',
'996874501',
'996880101',
'996881302',
'996884501',
'996885901',
'996886301',
'996887001',
'996887003',
'996903801',
'996904801',
'996906701',
'996906901',
'996910601',
'996910901',
'996920301',
'996921701',
'996921801',
'996923001',
'996931701',
'996950201',
'996957401',
'996960601',
'996976901',
'996979701',
'996980101',
'996980301',
'996981401',
'996990701',
'996994501',
'996998601',
'997001001',
'997003801',
'997006101',
'997011401',
'997011701',
'997014701',
'997018701',
'997027001',
'997028201',
'997039101',
'997040901',
'997045301',
'997054102',
'997061001',
'997061901',
'997065101',
'997066801',
'997067301',
'997067701',
'997069901',
'997095401',
'997097301',
'997099701',
'997111501',
'997115601',
'997123001',
'997124501',
'997134901',
'997142701',
'997172901',
'997181701',
'997185501',
'997196401',
'997201001',
'997205301',
'997212501',
'997252101',
'997261001',
'997271901',
'997297201',
'997297202',
'997316401',
'997329201',
'997338901',
'997356301',
'997359701',
'997362201',
'997364201',
'997368101',
'997370501',
'997376801',
'997377901',
'997390201',
'997391801',
'997396001',
'997406001',
'997414201',
'997422802',
'997426701',
'997439301',
'997441201',
'997455701',
'997459501',
'997462501',
'997469701',
'997470501',
'997477301',
'997481101',
'997493101',
'997501301',
'997505401',
'997510601',
'997511101',
'997512301',
'997514101',
'997516001',
'997527301',
'997537101',
'997539501',
'997547001',
'997550601',
'997551901',
'997561701',
'997566001',
'997566701',
'997570301',
'997571801',
'997572202',
'997574701',
'997592601',
'997598301',
'997602601',
'997603301',
'997609301',
'997616701',
'997639201',
'997649401',
'997655901',
'997656202',
'997669101',
'997671301',
'997671401',
'997702301',
'997711201',
'997711301',
'997722501',
'997734001',
'997735101',
'997746501',
'997762003',
'997774701',
'997780001',
'997782801',
'997782802',
'997787101',
'997806101',
'997810201',
'997822301',
'997823301',
'997824301',
'997827801',
'997828701',
'997835101',
'997835201',
'997835501',
'997838101',
'997843501',
'997846701',
'997846801',
'997848301',
'997857901',
'997860201',
'997865501',
'997890801',
'997899101',
'997901401',
'997902301',
'997906601',
'997908601',
'997908901',
'997913801',
'997915801',
'997920501',
'997921101',
'997926001',
'997943201',
'997947701',
'997955001',
'997956301',
'997956901',
'997960701',
'997974301',
'997975801',
'997979801',
'997986801',
'997991201',
'997996301',
'998005404',
'998005901',
'998009304',
'998023301',
'998027101',
'998041901',
'998043801',
'998051501',
'998057401',
'998069501',
'998082301',
'998090201',
'998097701',
'998105401',
'998106201',
'998113301',
'998114001',
'998114401',
'998118701',
'998126801',
'998146301',
'998148501',
'998149302',
'998151501',
'998156201',
'998169201',
'998174801',
'998175001',
'998175701',
'998203401',
'998206201',
'998213901',
'998215401',
'998217301',
'998218201',
'998219701',
'998220501',
'998228101',
'998235101',
'998237301',
'998248201',
'998255001',
'998255101',
'998269401',
'998272703',
'998283201',
'998285601',
'998302201',
'998318301',
'998334801',
'998343601',
'998348701',
'998351401',
'998363701',
'998366501',
'998366601',
'998375301',
'998375602',
'998375801',
'998388201',
'998390001',
'998396501',
'998398001',
'998424901',
'998425801',
'998435201',
'998436901',
'998437601',
'998438101',
'998450101',
'998450101H',
'998450102',
'998462401',
'998465301',
'998474201',
'998476601',
'998481801',
'998482802',
'998484601',
'998489501',
'998492701',
'998492801',
'998498201',
'998503101',
'998509201',
'998510902',
'998516001',
'998522501',
'998525001',
'998529401',
'998532701',
'998533901',
'998538101',
'998538401',
'998540201',
'998540501',
'998543801',
'998549901',
'998558901',
'998561302',
'998563101',
'998566702',
'998587201',
'998600901',
'998602701',
'998604901',
'998610601',
'998618502',
'998626501',
'998626701',
'998626702',
'998632501',
'998644601',
'998646501',
'998656801',
'998658401',
'998660201',
'998666001',
'998669601',
'998674101',
'998693701',
'998700101',
'998718401',
'998738801',
'998740401',
'998740402',
'998740701',
'998742801',
'998743501',
'998754801',
'998756101',
'998757101',
'998757301',
'998758701',
'998761301',
'998768001',
'998773401',
'998786902',
'998791101',
'998796101',
'998796301',
'998813801',
'998815001',
'998818101',
'998821001',
'998847902',
'998853201',
'998855902',
'998865801',
'998867901',
'998888402',
'998889801',
'998889802',
'998900701',
'998900703',
'998901101',
'998914501',
'998915801',
'998918301',
'998927601',
'998934201',
'998947802',
'998948701',
'998948702',
'998951201',
'998954501',
'998956901',
'998957601',
'998962401',
'998970701',
'998976101',
'998979201',
'998980701',
'998985903',
'998991201',
'998999501',
'999001601',
'999007101',
'999010001',
'999016701',
'999016801',
'999028001',
'999044301',
'999046101',
'999046601',
'999060501',
'999065601',
'999068201',
'999068301',
'999071901',
'999084401',
'999091201',
'999093101',
'999097901',
'999099701',
'999100401',
'999107001',
'999127501',
'999127601',
'999139101',
'999155001',
'999164001',
'999178201',
'999180001',
'999185801',
'999190901',
'999191002',
'999196901',
'999197101',
'999197401',
'999200701',
'999202101',
'999204201',
'999207501',
'999208801',
'999209802',
'999223201',
'999226801',
'999233601',
'999237701',
'999238001',
'999248601',
'999254101',
'999262402',
'999266101',
'999272601',
'999273101',
'999278201',
'999285501',
'999288301',
'999290201',
'999291001',
'999294201',
'999295501',
'999295901',
'999337201',
'999340201',
'999342701',
'999347101',
'999353701',
'999355801',
'999355901',
'999357101',
'999358601',
'999358801',
'999367401',
'999382401',
'999384601',
'999397001',
'999397401',
'999405501',
'999406101',
'999412201',
'999413501',
'999417003',
'999417301',
'999418901',
'999422101',
'999422601',
'999423501',
'999427001',
'999427901',
'999434601',
'999436801',
'999441001',
'999441201',
'999444101',
'999448801',
'999460801',
'999461901',
'999472901',
'999478002',
'999486801',
'999491801',
'999493901',
'999501301',
'999503701',
'999505301',
'999508201',
'999510301',
'999511001',
'999516901',
'999535501',
'999538101',
'999545701',
'999557301',
'999562701',
'999563301',
'999566101',
'999570001',
'999575401',
'999576501',
'999593701',
'999594301',
'999595301',
'999596901',
'999597501',
'999597801',
'999598401',
'999600001',
'999600101',
'999609101',
'999609401',
'999611901',
'999612401',
'999614901',
'999615401',
'999619301',
'999633001',
'999633401',
'999636901',
'999647501',
'999647701',
'999656001',
'999656701',
'999661201',
'999665301',
'999673001',
'999679101',
'999683501',
'999683901',
'999684001',
'999684101',
'999686402',
'999686405',
'999687701',
'999696001',
'999697901',
'999698801',
'999701201',
'999701701',
'999703101',
'999703302',
'999715601',
'999716401',
'999717101',
'999729501',
'999731601',
'999737502',
'999745901',
'999757001',
'999770901',
'999771201',
'999774201',
'999777801',
'999779801',
'999780501',
'999780502',
'999786101',
'999789401',
'999812101',
'999816501',
'999816601',
'999817301',
'999818302',
'999819201',
'999827101',
'999835001',
'999840501',
'999841301',
'999855901',
'999862301',
'999864701',
'999867501',
'999867601',
'999868001',
'999877201',
'999878201',
'999880601',
'999885401',
'999890501',
'999898101',
'999899201',
'999909501',
'999912601',
'999919601',
'999922201',
'999925101',
'999925701',
'999931504',
'999932801',
'999938701',
'999943101',
'999950201',
'999956501',
'999956502',
'999956701',
'999957201',
'999963602',
'999969801',
'999971301',
'999973801',
'999978501',
'999978601',
'999979601',
'999980202',
'999980701',
'999980801',
'999981301',
'999982801',
'999987701',
'999992201',
'999998301',
'999999101',
'999999901'
)) ORDER BY OrderID DESC FOR JSON AUTO)").Single();
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public static string FetchOrderNumbers(int count)
        {
#if DEBUG
            using (var conn = new SqlConnection("Data Source=vm-sql1;Initial Catalog=Flock;Integrated Security=True"))
#else
            using (var conn = new SqlConnection("context connection=true"))
#endif
            {
                try
                {
                    return conn.Query<string>($"SELECT (SELECT TOP {count} OrderHeadID FROM mbr.OrderHead WHERE (AcceptedByMertexDT IS NOT NULL AND Deleted = 0) ORDER BY OrderHeadID DESC FOR JSON AUTO)").Single();
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public static string FetchLocationCodes(int count)
        {
#if DEBUG
            using (var conn = new SqlConnection("Data Source=vm-sql1;Initial Catalog=Flock;Integrated Security=True"))
#else
            using (var conn = new SqlConnection("context connection=true"))
#endif
            {
                try
                {
                    return conn.Query<string>($"SELECT (SELECT TOP {count} Code FROM portal.StockLocation WHERE (CheckStock = 1 AND Deleted = 0) ORDER BY StockLocationID DESC FOR JSON AUTO)").Single();
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public static string FetchInterestingProductCodes()
        {
            return JsonConvert.SerializeObject(new List<ProductCode>() {
                new ProductCode { Code = "QL107" },
                new ProductCode { Code = "FRL5196/H" }
            });
        }

        public static string FetchInterestingOrderHeadIDs()
        {

//#if DEBUG
//            using (var conn = new SqlConnection("Data Source=vm-sql1;Initial Catalog=Flock;Integrated Security=True"))
//#else
//            using (var conn = new SqlConnection("context connection=true"))
//#endif
//            {
//                try
//                {
//                    return conn.Query<string>($"SELECT (SELECT OrderID FROM Mertex.OrderHead WHERE OrderDT BETWEEN '2019-01-01' AND '2020-01-01' AND Completed = 0 FOR JSON AUTO)").Single();
//                }
//                finally
//                {
//                    conn.Close();
//                }
//            }

            //return JsonConvert.SerializeObject(new List<OrderHead>() 
            //{
            //    new OrderHead { OrderID = "1008810001" },
            //    new OrderHead { OrderID = "1008810101" },
            //    new OrderHead { OrderID = "1008810201" },
            //    new OrderHead { OrderID = "1008810301" },
            //    new OrderHead { OrderID = "1008810401" },
            //    new OrderHead { OrderID = "1008810501" },
            //    new OrderHead { OrderID = "1008810601" },
            //    new OrderHead { OrderID = "1008810701" },
            //    new OrderHead { OrderID = "1008810801" },
            //    new OrderHead { OrderID = "1008810901" },
            //    new OrderHead { OrderID = "4448707" },
            //    new OrderHead { OrderID = "4448708" },
            //    new OrderHead { OrderID = "4469362" },
            //    new OrderHead { OrderID = "4469366" },
            //    new OrderHead { OrderID = "4469369" },
            //    new OrderHead { OrderID = "4469374" },
            //    new OrderHead { OrderID = "4469375" },
            //    new OrderHead { OrderID = "4469376" },
            //    new OrderHead { OrderID = "4469377" },
            //    new OrderHead { OrderID = "4469378" },
            //    new OrderHead { OrderID = "4469379" },
            //    new OrderHead { OrderID = "4469380" },
            //    new OrderHead { OrderID = "4469381" },
            //    new OrderHead { OrderID = "4469382" },
            //    new OrderHead { OrderID = "4469383" },
            //    new OrderHead { OrderID = "4469385" },
            //    new OrderHead { OrderID = "4469387" },
            //    new OrderHead { OrderID = "4469390" },
            //    new OrderHead { OrderID = "4469391" },
            //    new OrderHead { OrderID = "4469392" },
            //    new OrderHead { OrderID = "4469393" },
            //    new OrderHead { OrderID = "4469394" },
            //    new OrderHead { OrderID = "4469395" },
            //    new OrderHead { OrderID = "4469396" },
            //    new OrderHead { OrderID = "4469397" },
            //    new OrderHead { OrderID = "4515416" },
            //    new OrderHead { OrderID = "4515417" },
            //    new OrderHead { OrderID = "4515418" },
            //    new OrderHead { OrderID = "4515419" },
            //    new OrderHead { OrderID = "4515420" },
            //    new OrderHead { OrderID = "4515421" },
            //    new OrderHead { OrderID = "4515422" },
            //    new OrderHead { OrderID = "4515423" },
            //    new OrderHead { OrderID = "4515424" },
            //    new OrderHead { OrderID = "4515425" },
            //    new OrderHead { OrderID = "4515426" },
            //    new OrderHead { OrderID = "4515427" },
            //    new OrderHead { OrderID = "4527745" },
            //    new OrderHead { OrderID = "4527746" },
            //    new OrderHead { OrderID = "4527747" },
            //    new OrderHead { OrderID = "4527748" },
            //    new OrderHead { OrderID = "4527749" },
            //    new OrderHead { OrderID = "4527750" },
            //    new OrderHead { OrderID = "4527754" },
            //    new OrderHead { OrderID = "4527755" },
            //    new OrderHead { OrderID = "4527756" },
            //    new OrderHead { OrderID = "4527757" },
            //    new OrderHead { OrderID = "4529234" },
            //    new OrderHead { OrderID = "4530020" },
            //    new OrderHead { OrderID = "4530021" },
            //    new OrderHead { OrderID = "4533472" },
            //    new OrderHead { OrderID = "4533473" },
            //    new OrderHead { OrderID = "4533474" },
            //    new OrderHead { OrderID = "4533475" },
            //    new OrderHead { OrderID = "4533476" },
            //    new OrderHead { OrderID = "4533477" },
            //    new OrderHead { OrderID = "4533478" },
            //    new OrderHead { OrderID = "4533479" },
            //    new OrderHead { OrderID = "4541868" },
            //    new OrderHead { OrderID = "4541869" },
            //    new OrderHead { OrderID = "4549567" },
            //    new OrderHead { OrderID = "4549569" },
            //    new OrderHead { OrderID = "4549570" },
            //    new OrderHead { OrderID = "4549571" },
            //    new OrderHead { OrderID = "4549572" },
            //    new OrderHead { OrderID = "4549573" },
            //    new OrderHead { OrderID = "4549574" },
            //    new OrderHead { OrderID = "4549575" },
            //    new OrderHead { OrderID = "4549576" },
            //    new OrderHead { OrderID = "4549577" },
            //    new OrderHead { OrderID = "4549578" },
            //    new OrderHead { OrderID = "4549579" },
            //    new OrderHead { OrderID = "4549580" },
            //    new OrderHead { OrderID = "4549581" },
            //    new OrderHead { OrderID = "4549582" },
            //    new OrderHead { OrderID = "4549583" },
            //    new OrderHead { OrderID = "4549584" },
            //    new OrderHead { OrderID = "4549585" },
            //    new OrderHead { OrderID = "4549586" },
            //    new OrderHead { OrderID = "4549587" },
            //    new OrderHead { OrderID = "4558750" },
            //    new OrderHead { OrderID = "4558752" },
            //    new OrderHead { OrderID = "4558753" },
            //    new OrderHead { OrderID = "4558754" },
            //    new OrderHead { OrderID = "4558755" },
            //    new OrderHead { OrderID = "4558756" },
            //    new OrderHead { OrderID = "4558758" },
            //    new OrderHead { OrderID = "4558759" },
            //    new OrderHead { OrderID = "4558760" },
            //    new OrderHead { OrderID = "4558761" },
            //    new OrderHead { OrderID = "4558763" },
            //    new OrderHead { OrderID = "4558764" },
            //    new OrderHead { OrderID = "4558765" },
            //    new OrderHead { OrderID = "4558766" },
            //    new OrderHead { OrderID = "4558767" },
            //    new OrderHead { OrderID = "4558769" },
            //    new OrderHead { OrderID = "4558770" },
            //    new OrderHead { OrderID = "4558771" },
            //    new OrderHead { OrderID = "4558772" },
            //    new OrderHead { OrderID = "4558779" },
            //    new OrderHead { OrderID = "4560687" },
            //    new OrderHead { OrderID = "4560688" },
            //    new OrderHead { OrderID = "4560689" },
            //    new OrderHead { OrderID = "4560690" },
            //    new OrderHead { OrderID = "4560691" },
            //    new OrderHead { OrderID = "4560692" },
            //    new OrderHead { OrderID = "4560693" },
            //    new OrderHead { OrderID = "4560694" },
            //    new OrderHead { OrderID = "4560695" },
            //    new OrderHead { OrderID = "4560696" },
            //    new OrderHead { OrderID = "4560697" },
            //    new OrderHead { OrderID = "4560698" },
            //    new OrderHead { OrderID = "4576587" },
            //    new OrderHead { OrderID = "4576588" },
            //    new OrderHead { OrderID = "4576589" },
            //    new OrderHead { OrderID = "4576590" },
            //    new OrderHead { OrderID = "4576591" },
            //    new OrderHead { OrderID = "4576592" },
            //    new OrderHead { OrderID = "4579344" },
            //    new OrderHead { OrderID = "4579345" },
            //    new OrderHead { OrderID = "4579346" },
            //    new OrderHead { OrderID = "4579347" },
            //    new OrderHead { OrderID = "4579348" },
            //    new OrderHead { OrderID = "4579349" },
            //    new OrderHead { OrderID = "4579350" },
            //    new OrderHead { OrderID = "4579351" },
            //    new OrderHead { OrderID = "4579352" },
            //    new OrderHead { OrderID = "4579353" },
            //    new OrderHead { OrderID = "4579354" },
            //    new OrderHead { OrderID = "4579468" },
            //    new OrderHead { OrderID = "4579469" },
            //    new OrderHead { OrderID = "4579470" },
            //    new OrderHead { OrderID = "4579471" },
            //    new OrderHead { OrderID = "4579472" },
            //    new OrderHead { OrderID = "4579473" },
            //    new OrderHead { OrderID = "4579474" },
            //    new OrderHead { OrderID = "4579475" },
            //    new OrderHead { OrderID = "4579476" },
            //    new OrderHead { OrderID = "4579477" },
            //    new OrderHead { OrderID = "4579478" },
            //    new OrderHead { OrderID = "4617691" },
            //    new OrderHead { OrderID = "4618870" },
            //    new OrderHead { OrderID = "4618871" },
            //    new OrderHead { OrderID = "4618984" },
            //    new OrderHead { OrderID = "4620941" },
            //    new OrderHead { OrderID = "4620964" },
            //    new OrderHead { OrderID = "4620971" },
            //    new OrderHead { OrderID = "4620991" }
            //});

            return @"[{ ""OrderID"": ""4632745"" }]";
        }

        public static string FetchTuffnellsDespatchHeadIDs(DateTime? FromDT)
        {
#if DEBUG
            using (var conn = new SqlConnection("Data Source=vm-sql1;Initial Catalog=Flock;Integrated Security=True"))
#else
            using (var conn = new SqlConnection("context connection=true"))
#endif
            {
                try
                {

                    if (FromDT.HasValue)
                    {
                        return conn.Query<string>($@"SELECT (
                                                        SELECT 
                                                            DH.DespatchHeadID FROM 
                                                        Mertex.DespatchHead AS DH 
                                                            WHERE DH.HaulierCode = 'TUF' AND
                                                                  DH.DespatchedDT >= '{FromDT?.ToString("yyyy/MM/dd")}'
                                                        ORDER BY 
                                                            DH.DespatchedDT
                                                        FOR JSON AUTO)").Single();
                    }
                    else
                    {
                        return conn.Query<string>($@"SELECT (
                                                        SELECT 
                                                            DH.DespatchHeadID FROM 
                                                        Mertex.DespatchHead AS DH 
                                                            WHERE DH.HaulierCode = 'TUF' 
                                                        ORDER BY 
                                                            DH.DespatchedDT
                                                        FOR JSON AUTO)").Single();
                    }
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public static string FetchInterestingDespatchIDs(int count)
        {
#if DEBUG
            using (var conn = new SqlConnection("Data Source=vm-sql1;Initial Catalog=Flock;Integrated Security=True"))
#else
            using (var conn = new SqlConnection("context connection=true"))
#endif
            {
                try
                {
                    // Cushions, Fabric, Wallpaper, Pattern Books.
                    return conn.Query<string>($@"SELECT (
                                                        SELECT TOP {count} MOL.DespatchID 
                                                        FROM Mertex.OrderLine AS MOL
                                                        INNER JOIN mbr.OrderHead AS OH ON CONVERT(NVARCHAR(50), OH.OrderHeadID) = MOL.OrderID
                                                        INNER JOIN mbr.OrderLine AS OL ON OL.OrderHeadID = OH.OrderHeadID
                                                        INNER JOIN dbo.Product AS P ON P.ProductID = OL.ProductID
                                                        WHERE LEN(MOL.OrderID) >= 10 AND
	                                                            MOL.OrderID LIKE '100%' AND
	                                                            P.ProductTypeID IN (4, 15, 18, 76) AND
	                                                            OH.AcceptedByMertexDT IS NOT NULL AND
	                                                            OL.OrderStatusID > 4
                                                        GROUP BY MOL.DespatchID
                                                        HAVING COUNT (OL.OrderLineID) > 5
                                                        ORDER BY MOL.DespatchID DESC FOR JSON AUTO)").Single();
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public static string FetchPurchaseOrderHeads(int count)
        {
#if DEBUG
            using (var conn = new SqlConnection("Data Source=vm-sql1;Initial Catalog=MertexReportData;Integrated Security=True"))
#else
            using (var conn = new SqlConnection("context connection=true"))
#endif
            {
                try
                {
                    return conn.Query<string>($"SELECT (SELECT TOP {count} PONO AS Code FROM dbo.XIPOHEAD AS PH ORDER BY PONO DESC FOR JSON AUTO)").Single();
                }
                finally
                {
                    conn.Close();
                }
            }
        }
    }
}

using System.IO;
using Dbosoft.YaNco;
using LanguageExt;

namespace Dbosoft.SAPDms.Functions
{
    public static partial class DmsRfcFunctionExtensions
    {
        public static EitherAsync<RfcErrorInfo, string> StartRegServer(
            this IRfcContext context, string progname)
        {
            return context.CallFunction("EASYDMS_START_REGSERVER",
                f => f
                    .SetField("PROGNAME", progname)
                    .SetField("STARTMODE", "")
                    .SetField("EXCLUSIV", "Y")
                    .SetField("WAITTIME", 500)
                    .SetField("STARTPARA", " ")
                    .SetField("STARTCOMP", "C"),

                    f => f.GetField<string>("DESTINATION"));
        }

    }
}

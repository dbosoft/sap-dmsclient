using System.Threading.Tasks;
using Dbosoft.YaNco;
using LanguageExt;

namespace Dbosoft.SAPDms.Functions
{
    public static partial class DmsRfcFunctionExtensions
    {
        public static EitherAsync<RfcErrorInfo, Unit> DocumentCheckoutFile(
            this IRfcContext context, DocumentFileInfo fileInfo, string checkoutPath)
        {
            return context.CallFunction("BAPI_DOCUMENT_CHECKOUTVIEW2",
                Input: func => func
                    .SetField("DOCUMENTTYPE", fileInfo.DocumentId.Type)
                    .SetField("DOCUMENTNUMBER", fileInfo.DocumentId.Number)
                    .SetField("DOCUMENTPART", fileInfo.DocumentId.Part)
                    .SetField("DOCUMENTVERSION", fileInfo.DocumentId.Version)
                    .SetField("ORIGINALPATH", checkoutPath)
                    .SetStructure("DOCUMENTFILE", s => s
                        .SetField("ORIGINALTYPE", fileInfo.OriginalType)
                        .SetField("APPLICATION_ID", fileInfo.ApplicationId)
                        .SetField("FILE_ID", fileInfo.FileId)),
                Output: func => func
                    .HandleReturn()).Map(_ => Unit.Default);
        }

    }
}

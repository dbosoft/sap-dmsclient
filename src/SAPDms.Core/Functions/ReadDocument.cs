using System.Linq;
using System.Threading.Tasks;
using Dbosoft.YaNco;
using LanguageExt;

namespace Dbosoft.SAPDms.Functions
{
    public static partial class DmsRfcFunctionExtensions
    {
        public static Task<Either<RfcErrorInfo, DocumentData>> DocumentGetDetail(
            this IRfcContext context, DocumentId documentId)
        {
            return context.CallFunction("BAPI_DOCUMENT_GETDETAIL2",
                Input: func => func
                    .BindDocumentId(documentId)
                    .SetField("GETDOCDESCRIPTIONS", "X"),
                Output: func => func
                    .HandleReturn()
                    .MapStructure("DOCUMENTDATA", docData =>
                        from status in docData.GetField<string>("STATUSEXTERN")
                        from description in docData.GetField<string>("DESCRIPTION")
                        select new DocumentData(documentId, description, status)
                    )
                    );
        }

    }
}

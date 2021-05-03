using Dbosoft.YaNco;
using LanguageExt;

namespace Dbosoft.SAPDms.Functions
{
    public static partial class DmsRfcFunctionExtensions
    {
        public static EitherAsync<RfcErrorInfo, DocumentData> DocumentGetDetail(
            this IRfcContext context, DocumentId documentId)
        {
            return context.CallFunction("BAPI_DOCUMENT_GETDETAIL2",
                Input: func => func
                    .SetField("DOCUMENTTYPE", documentId.Type)
                    .SetField("DOCUMENTNUMBER", documentId.Number)
                    .SetField("DOCUMENTPART", documentId.Part)
                    .SetField("DOCUMENTVERSION", documentId.Version)
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

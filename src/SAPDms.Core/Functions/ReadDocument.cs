using System.IO;
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
                    .SetField("GETDOCDESCRIPTIONS", "X")
                    .SetField("GETDOCFILES", "X"),
                Output: func =>
                        from _ in func.HandleReturn()
                        from docData in func.MapStructure("DOCUMENTDATA", docData =>
                            from status in docData.GetField<string>("STATUSEXTERN")
                            from description in docData.GetField<string>("DESCRIPTION")
                            select new {status, description })

                        from fileData in func.MapTable("DOCUMENTFILES", s =>
                                from originalType in s.GetField<string>("ORIGINALTYPE")
                                from filePath in s.GetField<string>("DOCFILE")
                                from applicationId in s.GetField<string>("APPLICATION_ID")
                                from fileId in s.GetField<string>("FILE_ID")
                                select new DocumentFileInfo(documentId,
                                    applicationId, originalType, fileId , Path.GetFileName(filePath))
                            )
                        select new DocumentData(documentId, docData.description, docData.status, fileData.ToArr())
                    );
        }

    }
}

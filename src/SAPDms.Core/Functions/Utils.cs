using System.Threading.Tasks;
using Dbosoft.YaNco;
using LanguageExt;

namespace Dbosoft.SAPDms.Functions
{

    public static partial class DmsRfcFunctionExtensions
    {

        internal static Task<Either<RfcErrorInfo, IFunction>> BindDocumentId(
            this Task<Either<RfcErrorInfo, IFunction>> eitherFunc, DocumentId documentId
        ) => eitherFunc
                .SetField("DOCUMENTTYPE", documentId.Type)
                .SetField("DOCUMENTNUMBER", documentId.Number)
                .SetField("DOCUMENTPART", documentId.Part)
                .SetField("DOCUMENTVERSION", documentId.Version);



    }
}

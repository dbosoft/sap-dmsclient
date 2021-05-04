using LanguageExt;

namespace Dbosoft.SAPDms
{
    public class DocumentFileInfo : Record<DocumentFileInfo>
    {
        public readonly DocumentId DocumentId;
        public readonly string ApplicationId;
        public readonly string OriginalType;
        public readonly string FileId;

        public readonly string FileName;

        public static DocumentFileInfo Empty = new DocumentFileInfo(default, 
            default, default, default, default);

        public DocumentFileInfo(DocumentId documentId, 
            string applicationId, 
            string originalType, 
            string fileId, 
            string fileName)
        {
            DocumentId = documentId;
            ApplicationId = applicationId;
            OriginalType = originalType;
            FileId = fileId;
            FileName = fileName;

        }

    }
}
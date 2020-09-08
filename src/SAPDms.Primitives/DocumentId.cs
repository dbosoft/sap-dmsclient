using LanguageExt;

namespace Dbosoft.SAPDms
{
    public class DocumentId : Record<DocumentId>
    {
        public readonly string Type;
        public readonly string Number;
        public readonly string Part;
        public readonly string Version;

        public DocumentId(string documentType, string documentNumber, string documentPart,
            string documentVersion)
        {
            Type = documentType;
            Number = documentNumber;
            Part = documentPart;
            Version = documentVersion;
        }
    }
}
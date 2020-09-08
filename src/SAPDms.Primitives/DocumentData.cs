using LanguageExt;

namespace Dbosoft.SAPDms
{
    public class DocumentData : Record<DocumentData>
    {
        public readonly DocumentId Id;
        public readonly string Description;
        public readonly string Status;


        public DocumentData(DocumentId id, string description, string status)
        {
            Id = id;
            Description = description;
            Status = status;
        }
    }
}
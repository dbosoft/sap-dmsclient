using LanguageExt;

namespace Dbosoft.SAPDms
{
    public class DocumentData : Record<DocumentData>
    {
        public readonly DocumentId Id;
        public readonly string Description;
        public readonly string Status;

        public readonly Arr<DocumentFileInfo> Files;


        public DocumentData(DocumentId id, string description, string status, Arr<DocumentFileInfo> files)
        {
            Id = id;
            Description = description;
            Status = status;
            Files = files;
        }
    }
}
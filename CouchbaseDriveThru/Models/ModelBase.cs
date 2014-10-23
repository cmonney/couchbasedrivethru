namespace CouchbaseDriveThru.Models
{
    public abstract class ModelBase
    {
        public virtual string Id { get; set; }
        public abstract  string Type { get; }
    }
}
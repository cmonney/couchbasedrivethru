namespace CouchbaseDriveThru.Models
{
    using CouchbaseModelViews.Framework.Attributes;

    [CouchbaseDesignDoc("customers")]
    [CouchbaseAllView]
    public class Customer : ModelBase
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        public string Postcode { get; set; }

        public string Phone { get; set; }

        public override string Type
        {
            get { return  "customer"; }
        }
    }
}
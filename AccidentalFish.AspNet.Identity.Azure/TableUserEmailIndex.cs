using Microsoft.WindowsAzure.Storage.Table;

namespace AccidentalFish.AspNet.Identity.Azure
{
    public class TableUserEmailIndex : TableEntity
    {
        public TableUserEmailIndex()
        {
            
        }

        public TableUserEmailIndex(string base64EncodedEmail, string userId)
        {
            PartitionKey = base64EncodedEmail;
            RowKey = "";
            UserId = userId;
        }

        public string UserId { get; set; }
    }
}

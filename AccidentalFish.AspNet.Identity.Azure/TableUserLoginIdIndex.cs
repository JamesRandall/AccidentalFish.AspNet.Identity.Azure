using Microsoft.WindowsAzure.Storage.Table;

namespace AccidentalFish.AspNet.Identity.Azure
{
    public class TableUserIdIndex : TableEntity
    {
        public TableUserIdIndex()
        {
            
        }

        public TableUserIdIndex(string base64UserName, string userId)
        {
            PartitionKey = base64UserName;
            RowKey = base64UserName;
            UserId = userId;
        }

        public string UserId { get; set; }
    }
}

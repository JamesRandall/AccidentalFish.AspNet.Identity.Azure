using Microsoft.WindowsAzure.Storage.Table;

namespace AccidentalFish.AspNet.Identity.Azure
{
    public class TableUserIdIndex : TableEntity
    {
        public TableUserIdIndex()
        {
            
        }

        public TableUserIdIndex(string userName, string userId)
        {
            PartitionKey = userId;
            RowKey = userName;
        }
    }
}

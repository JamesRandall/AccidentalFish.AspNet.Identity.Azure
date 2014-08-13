using Microsoft.WindowsAzure.Storage.Table;
using AccidentalFish.AspNet.Identity.Azure.Extensions;

namespace AccidentalFish.AspNet.Identity.Azure
{
    public class TableUserClaim : TableEntity
    {
        public TableUserClaim()
        {
            
        }

        public TableUserClaim(string userId, string claimType, string claimValue)
        {
            UserId = userId;
            ClaimType = claimType;
            ClaimValue = claimValue;

            SetPartitionAndRowKey();
        }

        public void SetPartitionAndRowKey()
        {
            PartitionKey = UserId;
            RowKey = ClaimType.Base64Encode();
        }

        public string UserId { get; set; }

        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }
    }
}

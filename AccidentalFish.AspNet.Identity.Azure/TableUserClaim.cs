using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

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
            RowKey = ClaimType;
        }

        public string UserId { get; set; }

        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }
    }
}

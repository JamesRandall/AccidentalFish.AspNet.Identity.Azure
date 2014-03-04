using System;
using AccidentalFish.AspNet.Identity.Azure.Extensions;
using Microsoft.WindowsAzure.Storage.Table;

namespace AccidentalFish.AspNet.Identity.Azure
{
    class TableUserLoginProviderKeyIndex : TableEntity
    {
        public TableUserLoginProviderKeyIndex(string userId, string loginProviderKey, string loginProvider)
        {
            PartitionKey = String.Format("{0}_{1}", loginProvider, loginProviderKey.Base64Encode());
            RowKey = "";
            UserId = userId;
        }

        public string GetLoginProvider()
        {
            return PartitionKey.Substring(0, PartitionKey.IndexOf('_')-1);
        }

        public string GetLoginProviderKey()
        {
            return PartitionKey.Substring(PartitionKey.IndexOf('_')).Base64Decode();
        }

        public string UserId { get; set; }
    }
}

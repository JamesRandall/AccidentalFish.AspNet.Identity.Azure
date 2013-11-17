using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace AccidentalFish.AspNet.Identity.Azure
{
    public class TableUserLogin : TableEntity
    {
        public TableUserLogin()
        {
            
        }

        public TableUserLogin(string userId, string loginProvider, string providerKey)
        {
            UserId = userId;
            LoginProvider = loginProvider;
            ProviderKey = providerKey;

            SetPartitionAndRowKey();
        }

        public void SetPartitionAndRowKey()
        {
            PartitionKey = UserId;
            RowKey = ProviderKey;
        }

        public string LoginProvider { get; set; }

        public string ProviderKey { get; set; }

        public string UserId { get; set; }
    }
}

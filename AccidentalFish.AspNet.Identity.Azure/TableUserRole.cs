using System;
using Microsoft.AspNet.Identity;
using Microsoft.WindowsAzure.Storage.Table;
using AccidentalFish.AspNet.Identity.Azure.Extensions;

namespace AccidentalFish.AspNet.Identity.Azure
{
    public class TableUserRole : TableEntity, IRole
    {
        public TableUserRole()
        {

        }

        public TableUserRole(string userId, string name)
        {
            Id = Guid.NewGuid().ToString();
            UserId = userId;
            Name = name;
            SetPartitionAndRowKey();
        }

        public void SetPartitionAndRowKey()
        {
            PartitionKey = UserId;
            RowKey = Name.Base64Encode();
        }

        public string UserId { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
    }
}

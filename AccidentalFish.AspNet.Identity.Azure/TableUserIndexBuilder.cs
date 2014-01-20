using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace AccidentalFish.AspNet.Identity.Azure
{
    /// <summary>
    /// This class will build indexes for an existing table user store. It's really just there to patch a pre v0.3.0.0 release.
    /// </summary>
    public class TableUserIndexBuilder
    {
        private readonly CloudTable _userTable;
        private readonly CloudTable _userIndexTable;

        public TableUserIndexBuilder(CloudStorageAccount storageAccount) : this(storageAccount, "users", "userIndexItems")
        {
            
        }

        public TableUserIndexBuilder(CloudStorageAccount storageAccount, string userTableName, string userIndexTableName)
        {
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            _userTable = tableClient.GetTableReference(userTableName);
            _userIndexTable = tableClient.GetTableReference(userIndexTableName);

            _userIndexTable.CreateIfNotExists();
        }

        public async Task BuildIndexes()
        {
            TableQuery<TableUser> query = new TableQuery<TableUser>();
            TableQuerySegment<TableUser> querySegment = null;
            List<Task> insertOperation = new List<Task>();

            while (querySegment == null || querySegment.ContinuationToken != null)
            {
                querySegment = await _userTable.ExecuteQuerySegmentedAsync(query, querySegment != null ? querySegment.ContinuationToken : null);
                foreach (TableUser tableUser in querySegment.Results)
                {
                    TableUserIdIndex indexItem = new TableUserIdIndex(tableUser.UserName, tableUser.Id);
                    insertOperation.Add(_userIndexTable.ExecuteAsync(TableOperation.InsertOrReplace(indexItem)));
                    if (insertOperation.Count > 100)
                    {
                        await Task.WhenAll(insertOperation);
                        insertOperation.Clear();
                    }
                }
                if (insertOperation.Count > 0)
                {
                    await Task.WhenAll(insertOperation);
                    insertOperation.Clear();
                }
            }
        }
    }
}

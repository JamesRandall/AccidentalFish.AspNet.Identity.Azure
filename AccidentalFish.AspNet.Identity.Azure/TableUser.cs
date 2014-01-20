using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.WindowsAzure.Storage.Table;

namespace AccidentalFish.AspNet.Identity.Azure
{
    public class TableUser : TableEntity, IUser
    {
        public void SetPartitionAndRowKey()
        {
            PartitionKey = Id;
            RowKey = Id;
        }

        public string Id { get; set; }

        public string UserName { get; set; }

        public string PasswordHash { get; set; }

        public string SecurityStamp { get; set; }

        [NonSerializedTableStore]
        internal Func<IEnumerable<TableUserRole>> LazyRolesEvaluator { get; set; }

        private List<TableUserRole> _roles;
        [NonSerializedTableStore]
        public ICollection<TableUserRole> Roles
        {
            get
            {
                if (_roles == null && LazyRolesEvaluator != null)
                {
                    _roles = new List<TableUserRole>(LazyRolesEvaluator());
                }
                return _roles;
            }
        }

        [NonSerializedTableStore]
        internal Func<IEnumerable<TableUserClaim>> LazyClaimsEvaluator { get; set; }

        private List<TableUserClaim> _claims;
        [NonSerializedTableStore]
        public ICollection<TableUserClaim> Claims
        {
            get
            {
                if (_claims == null && LazyClaimsEvaluator != null)
                {
                    _claims = new List<TableUserClaim>(LazyClaimsEvaluator());
                }
                return _claims;
            }
        }

        [NonSerializedTableStore]
        internal Func<IEnumerable<TableUserLogin>> LazyLoginEvaluator { get; set; }

        private List<TableUserLogin> _logins;
        [NonSerializedTableStore]
        public ICollection<TableUserLogin> Logins
        {
            get
            {
                if (_logins == null && LazyLoginEvaluator != null)
                {
                    _logins = new List<TableUserLogin>(LazyLoginEvaluator());
                }
                return _logins;
            }
        }

        public TableUser()
        {
            this.Id = Guid.NewGuid().ToString();
            
            SetPartitionAndRowKey();
        }

        public TableUser(string userName)
            : this()
        {
            this.UserName = userName;
            SetPartitionAndRowKey();
        }

        public override IDictionary<string, EntityProperty> WriteEntity(Microsoft.WindowsAzure.Storage.OperationContext operationContext)
        {
            var entityProperties = base.WriteEntity(operationContext);
            var objectProperties = GetType().GetProperties();

            foreach (var property in from property in objectProperties
                                     let nonSerializedAttributes = property.GetCustomAttributes(typeof(NonSerializedTableStoreAttribute), false)
                                     where nonSerializedAttributes.Length > 0
                                     select property)
            {
                entityProperties.Remove(property.Name);
            }

            return entityProperties;
        }
    }
}

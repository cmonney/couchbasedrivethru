namespace CouchbaseDriveThru.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Couchbase;
    using Couchbase.Operations;
    using Enyim.Caching.Memcached;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public abstract class RepositoryBase<T> where T : ModelBase
    {
        protected static CouchbaseClient Client { get; set; }
        private readonly string _designDoc;

        static RepositoryBase()
        {
            Client = new CouchbaseClient();
        }

        protected RepositoryBase()
        {
            _designDoc = typeof (T).Name.ToLower().InflectTo().Pluralized;
        }

        public virtual IEnumerable<T> GetAll()
        {
            return Client.GetView<T>(_designDoc, "all", true);
        }

        public virtual IEnumerable<T> GetAll(int limit)
        {
            var view = Client.GetView<T>(_designDoc, "all", true);
            if (limit > 0)
            {
                view.Limit(limit);
            }

            return view;
        }

        public virtual int Create(T value, PersistTo persistTo = PersistTo.Zero )
        {
            var result = Client.ExecuteStore(StoreMode.Add, BuildKey(value), SerializeAndIgnoreId(value), persistTo);
            if (result.Exception != null)
            {
                throw result.Exception;
            }

            return result.StatusCode != null ? result.StatusCode.Value : 0;
        }

        public virtual int Update(T value, PersistTo persistTo = PersistTo.Zero)
        {
            var result = Client.ExecuteStore(StoreMode.Replace, value.Id, SerializeAndIgnoreId(value), persistTo);
            if (result.Exception != null)
            {
                throw result.Exception;
            }

            return result.StatusCode != null ? result.StatusCode.Value : 0;
        }

        public virtual int Save(T value, PersistTo persistTo = PersistTo.Zero)
        {
            var key = string.IsNullOrEmpty(value.Id) ? BuildKey(value) : value.Id;
            var result = Client.ExecuteStore(StoreMode.Set, key, SerializeAndIgnoreId(value), persistTo);
            if (result.Exception != null)
            {
                throw result.Exception;
            }

            return result.StatusCode != null ? result.StatusCode.Value : 0;
        }

        public virtual T Get(string key)
        {
            var result = Client.ExecuteGet<string>(key);

            if (result.Exception != null)
            {
                throw result.Exception;
            }

            if (result.Value == null)
            {
                return null;
            }

            var model = JsonConvert.DeserializeObject<T>(result.Value);
            model.Id = key;

            return model;
        }

        public virtual int Delete(string key, PersistTo persistTo = PersistTo.Zero)
        {
            var result = Client.ExecuteRemove(key, persistTo);
            if (result.Exception != null)
            {
                throw result.Exception;
            }

            return result.StatusCode.HasValue ? result.StatusCode.Value : 0;
        }

        protected virtual string BuildKey(T model)
        {
            return string.IsNullOrEmpty(model.Id) ? Guid.NewGuid().ToString() : model.Id.InflectTo().Underscored;
        }

        private string SerializeAndIgnoreId(T obj)
        {
            var json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings()
            {
                ContractResolver = new DocumentIdContractResolver(),
            });

            return json;
        }

        private class DocumentIdContractResolver : CamelCasePropertyNamesContractResolver
        {
            protected override List<MemberInfo> GetSerializableMembers(Type objectType)
            {
                return base.GetSerializableMembers(objectType).Where(o => o.Name != "Id").ToList();
            }
        }
    }
}
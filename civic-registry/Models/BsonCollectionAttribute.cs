using System;

namespace CivicRegistry.API.Models
{
    /// <summary>
    /// Attribute để chỉ định tên collection trong MongoDB
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class BsonCollectionAttribute : Attribute
    {
        public string CollectionName { get; }

        public BsonCollectionAttribute(string collectionName)
        {
            CollectionName = collectionName;
        }
    }
}


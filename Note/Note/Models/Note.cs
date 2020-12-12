using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Note.Models
{
    public class Note
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public string FileId { get; set; }

        public bool HasFile() => !string.IsNullOrEmpty(FileId);
    }
}
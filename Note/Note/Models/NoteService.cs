using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Note.Models
{
    public class NoteService
    {
        IGridFSBucket gridFS;
        IMongoCollection<Note> Notes;
        public NoteService()
        {
            string connectionString = "mongodb://localhost:27017/notes";
            var connection = new MongoUrlBuilder(connectionString);
            MongoClient client = new MongoClient(connectionString);
            IMongoDatabase database = client.GetDatabase(connection.DatabaseName);
            gridFS = new GridFSBucket(database);
            Notes = database.GetCollection<Note>("Notes");
        }

        public async Task<List<Note>> GetAllNotes()
        {
            return await Notes.Find(_ => true).ToListAsync();
        }

        public async Task<Note> GetById(string id)
        {
            return await Notes.Find(new BsonDocument("_id", new ObjectId(id))).FirstOrDefaultAsync();
        }

        public async Task<long> GetCountNotes()
        {
            return await Notes.CountAsync(_ => true);
        }

        public async Task CreateNote(Note note)
        {
            await Notes.InsertOneAsync(note);
        }

        public async Task UpdateNote(Note note)
        {
            await Notes.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(note.Id)), note);
        }

        public async Task Remove(string id)
        {
            await Notes.DeleteOneAsync(new BsonDocument("_id", new ObjectId(id)));
        }

        public async Task<byte[]> GetImage(string id)
        {
            return await gridFS.DownloadAsBytesAsync(new ObjectId(id));
        }

        public async Task StoreImage(string id, Stream imageStream, string imageName)
        {
            Note note = await GetById(id);
            if (note.HasFile())
            {
                await gridFS.DeleteAsync(new ObjectId(note.FileId));
            }
            ObjectId imageId = await gridFS.UploadFromStreamAsync(imageName, imageStream);
            note.FileId = imageId.ToString();
            var filter = Builders<Note>.Filter.Eq("_id", new ObjectId(note.Id));
            var update = Builders<Note>.Update.Set("ImageId", note.FileId);
            await Notes.UpdateOneAsync(filter, update);
        }
    }
}

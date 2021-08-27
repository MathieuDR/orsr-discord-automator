using System;
using System.Collections.Generic;
using System.Linq;
using DiscordBot.Common.Models.Data;
using DiscordBot.Data.Interfaces;
using FluentResults;
using LiteDB;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Data.Repository {
    public abstract class BaseLiteDbRepository<T> : IRepository<T> where T : BaseModel, new() {
        public BaseLiteDbRepository(ILogger logger, LiteDatabase database) {
            Logger = logger;
            LiteDatabase = database;
        }

        public ILogger Logger { get; }
        public LiteDatabase LiteDatabase { get; }
        public abstract string CollectionName { get; }

        protected ILiteCollection<T> GetCollection() {
            return LiteDatabase.GetCollection<T>(CollectionName);
        }

        public virtual Result<IEnumerable<T>> GetAll() {
            return Result.Ok(GetCollection().FindAll());
        }

        public virtual Result<T> Get(ObjectId id) {
            return Result.Ok(GetCollection().Query().Where(x => x._id == id).FirstOrDefault());
        }

        public virtual Result Insert(T toInsert) {
            var collection = GetCollection();
            collection.Insert(toInsert);
            return Result.Ok();
        }

        public virtual Result Update(T toUpdate) {
            var collection = GetCollection();
            collection.Update(toUpdate);
            return Result.Ok();
        }

        public virtual Result UpdateOrInsert(T entity) {
            if (entity._id is not null) {
                return Update(entity);
            }

            return Insert(entity);
        }

        public virtual Result Delete(T toDelete) {
            var collection = GetCollection();
            return collection.Delete(toDelete._id) ? Result.Ok() : Result.Fail("Delete failed");
        }
    }
    
    public abstract class BaseRecordLiteDbRepository<T> : IRecordRepository<T> where T : BaseRecord, new() {
        public BaseRecordLiteDbRepository(ILogger logger, LiteDatabase database) {
            Logger = logger;
            LiteDatabase = database;
        }

        public ILogger Logger { get; }
        public LiteDatabase LiteDatabase { get; }
        public abstract string CollectionName { get; }

        protected ILiteCollection<T> GetCollection() {
            return LiteDatabase.GetCollection<T>(CollectionName);
        }

        public virtual Result<IEnumerable<T>> GetAll() {
            return Result.Ok(GetCollection().FindAll());
        }

        public virtual Result<T> Get(ObjectId id) {
            return Result.Ok(GetCollection().Query().Where(x => x.Id == id).FirstOrDefault());
        }

        public virtual Result Insert(T toInsert) {
            var collection = GetCollection();
            collection.Insert(toInsert);
            return Result.Ok();
        }

        public virtual Result Update(T toUpdate) {
            var collection = GetCollection();
            collection.Update(toUpdate);
            return Result.Ok();
        }

        public virtual Result UpdateOrInsert(T entity) {
            if (entity.Id is not null) {
                return Update(entity);
            }

            return Insert(entity);
        }

        public virtual Result Delete(T toDelete) {
            var collection = GetCollection();
            return collection.Delete(toDelete.Id) ? Result.Ok() : Result.Fail("Delete failed");
        }
    }
}
﻿using Factory.DB;
using Newtonsoft.Json;
using Serilog;
using System.Diagnostics;
using System.Reflection.Metadata;
using Couchbase.Lite;
using System.Reflection;


namespace Factory.CouchbaseLiteFactory
{
    public partial class CouchBaseFactory :CouchbaseDocument, IDisposable
    {
        private bool disposedValue;

       
        public CouchBaseFactory(string databaseName)
        {
            try
            {
                _database = GetDatabase(databaseName);
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);

                Log.Error("{funcName}: Connection to SQLITE Failed: {error}", funcName, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public CouchBaseFactory(string databaseName, string scopeName, string collectionName)
        {
            try
            {
                _database = GetDatabase(databaseName);
                _collection = _database.GetCollection(collectionName, scopeName);
                if (_collection == null)
                    _collection = _database.CreateCollection(collectionName, scopeName);
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);

                Log.Error("{funcName}: Connection to SQLITE Failed: {error}", funcName, ex.Message);
                throw new Exception(ex.Message);
            }
        }

      

        

       
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)

                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~CouchBaseFactory()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

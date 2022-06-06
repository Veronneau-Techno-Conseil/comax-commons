using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Comax.Commons.StorageProvider
{
    internal class Common
    {
        private static object _lockObj = new object();
        private static Dictionary<string, LiteDatabase> _dbs = new Dictionary<string, LiteDatabase>();

        internal static LiteDatabase GetOrAdd(string name)
        {
            lock (_lockObj)
            {
                if (!_dbs.ContainsKey(name))
                {
                    var db = new LiteDatabase(name);
                    _dbs.Add(name, db);
                }
            }

            return _dbs[name];
        }
    }
}

using CommunAxiom.Commons.Client.IO.Datasource;
using Deedle;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IOTests
{
    [TestClass]
    public class TestImportJson
    {
        [TestMethod]
        public void TestMethod1()
        {
            var rnd = new Random();
            var objects = Enumerable.Range(0, 10).Select(i =>
              new { Key = "ID_" + i.ToString(), Number = rnd.Next() });

            //Frame.ReadReader
            // Create data frame with properties as column names
            var dfObjects = Frame.FromRecords(objects);
            dfObjects.Print();
            
        }

        [TestMethod]
        public void TestImportCollection()
        {
            List<JObject> collection = new List<JObject>();

            using (var fs = new FileStream("./Files/JsonCollection.json", FileMode.Open, FileAccess.Read))
            using (var sr = new StreamReader(fs))
            using (JsonTextReader jsonTextReader = new JsonTextReader(sr))
            {
                while (jsonTextReader.Read())
                {
                    if (jsonTextReader.Depth == 0 && (jsonTextReader.TokenType == JsonToken.StartArray
                            || jsonTextReader.TokenType == JsonToken.EndArray))
                        continue;

                    var obj = JObject.Load(jsonTextReader);
                    collection.Add(obj);
                }
            }

            Assert.AreEqual(collection.Count, 2);
        }

        public void TestDataSource()
        {
            var ds = new JsonTextDataSource();
            
        }
    }
}
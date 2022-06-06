using LiteDB;
using LiteDbTests.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiteDbTests
{
    [TestClass]
    public class TestWrite
    {
        static LiteDatabase _db;
        public TestWrite()
        {
            if(_db == null )
                _db = new LiteDatabase("LiteDbTests.db");
        }

        [TestMethod]
        public void TestFlat()
        {
            var entity = CreateFlat();
            var _grains = _db.GetCollection<Flat>("grains");
            
            _grains.Upsert(entity);
            var read = _grains.FindById(entity.ObjectId);
            
            Assert.IsNotNull(read);
            Assert.AreEqual(entity.BoolValue, read.BoolValue);
            Assert.AreEqual(entity.DateTimeOffset, read.DateTimeOffset);
            Assert.AreEqual(entity.LongValue, read.LongValue);
            Assert.AreEqual(entity.Name, read.Name);
            Assert.AreEqual(entity.ObjectId, read.ObjectId);
            if(entity.Content != null && read.Content != null)
                Assert.IsTrue(entity.Content.SequenceEqual(read.Content));
        }

        [TestMethod]
        public void TestComplex()
        {
            var entity = CreateComplex();
            var _grains = _db.GetCollection<ComplexCollection>("grains");
            
            _grains.Upsert(entity);
            var read = _grains.FindById(entity.ObjectId);

            Assert.IsNotNull(read);
            Assert.AreEqual(entity.BoolValue, read.BoolValue);
            Assert.AreEqual(entity.DateTimeOffset, read.DateTimeOffset);
            Assert.AreEqual(entity.LongValue, read.LongValue);
            Assert.AreEqual(entity.Name, read.Name);
            Assert.AreEqual(entity.ObjectId, read.ObjectId);
            if (entity.Content != null && read.Content != null)
                Assert.IsTrue(entity.Content.SequenceEqual(read.Content));
        }

        [TestMethod]
        public void TestChild()
        {
            var entity = CreateChild();
            var _grains = _db.GetCollection<ComplexChild>("grains");
            
            _grains.Upsert(entity);
            var read = _grains.FindById(entity.ObjectId);

            Assert.IsNotNull(read);
            Assert.AreEqual(entity.BoolValue, read.BoolValue);
            Assert.AreEqual(entity.DateTimeOffset, read.DateTimeOffset);
            Assert.AreEqual(entity.LongValue, read.LongValue);
            Assert.AreEqual(entity.Name, read.Name);
            Assert.AreEqual(entity.ObjectId, read.ObjectId);
            if (entity.Content != null && read.Content != null)
                Assert.IsTrue(entity.Content.SequenceEqual(read.Content));
        }

        private ComplexChild CreateChild()
        {
            return new ComplexChild
            {
                ObjectId = ObjectId.NewObjectId(),
                BoolValue = true,
                DateTimeOffset = System.DateTimeOffset.MaxValue,
                LongValue = long.MaxValue,
                Name = "Blah ble bli",
                Content = System.Text.Encoding.UTF8.GetBytes("Blah ble bli"),
                Flats = CreateFlat()
            };
        }

        private ComplexCollection CreateComplex()
        {
            return new ComplexCollection
            {
                ObjectId = ObjectId.NewObjectId(),
                BoolValue = true,
                DateTimeOffset = System.DateTimeOffset.MaxValue,
                LongValue = long.MaxValue,
                Name = "Blah ble bli",
                Content = System.Text.Encoding.UTF8.GetBytes("Blah ble bli"),
                Flats = new List<Flat>()
                {
                    CreateFlat(),
                    CreateFlat()
                }
            };
        }

        private Flat CreateFlat()
        {
            return new Flat
            {
                ObjectId = ObjectId.NewObjectId(),
                BoolValue = true,
                DateTimeOffset = System.DateTimeOffset.MaxValue,
                LongValue = long.MaxValue,
                Name = "Blah ble bli",
                Content = System.Text.Encoding.UTF8.GetBytes("Blah ble bli")
            };
        }
    }
}
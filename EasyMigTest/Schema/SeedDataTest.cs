using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyMigLib.Schema;

namespace EasyMigTest.Schema
{
    [TestClass]
    public class SeedDataTest
    {
        // check

        [TestMethod]
        public void TestSeed_WithSameName_Fail()
        {
            bool failed = false;

            try
            {
                SeedData.New.Set("id", 1).Set("id", 2);
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        [TestMethod]
        public void TestSeed_WithNotSameName_Success()
        {
            bool failed = false;

            try
            {
                SeedData.New.Set("id", 1).Set("name", "user1");
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsFalse(failed);
        }

        [TestMethod]
        public void TestSeed_AddValues()
        {
            var result = SeedData.New.Set("id", 1).Set("name", "user1");

            Assert.IsTrue(result.Has("id"));
            Assert.AreEqual(1, result.Get("id"));

            Assert.IsTrue(result.Has("name"));
            Assert.AreEqual("user1", result.Get("name"));
        }
    }
}

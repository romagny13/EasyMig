using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyMigLib.Query.MySqlClient;
using System.Collections.Generic;
using EasyMigLib.Db.MySqlClient;

namespace EasyMigTest.Query.MySqlClient
{
    [TestClass]
    public class MySqlQuerySplitterTest
    {

        [TestMethod]
        public void TestConcat()
        {
            var service = new MySqlQuerySplitter();
            var list = new List<string> {"a" };
            var toAdd = new string[] { "b", "c" };

            service.Concat(list, toAdd);

            Assert.AreEqual(3, list.Count);
            Assert.AreEqual("a", list[0]);
            Assert.AreEqual("b", list[1]);
            Assert.AreEqual("c", list[2]);
        }


        [TestMethod]
        public void TestSplitOnDELIMITERKeyword_WithNoClosures_ReturnsOneResult()
        {
            var service = new MySqlQuerySplitter();
            var result = service.SplitOnDELIMITERKeyword(@"instruction1;
                                                           instruction2;
                                                           instruction3;
                                                           instruction4;");


            Assert.AreEqual(1, result.Length);
        }

        [TestMethod]
        public void TestSplitOnDELIMITERKeyword_WithClosuresToStart()
        {
            var service = new MySqlQuerySplitter();
            var result = service.SplitOnDELIMITERKeyword(@"DELIMITER $$
                                                           closure1;
                                                           endclosure1$$
                                                           closure2;
                                                           endclosure2$$
                                                           DELIMITER ;
                                                           instruction1;
                                                           instruction2;");


            Assert.AreEqual(4, result.Length);
            Assert.AreEqual("$$", result[0]);
            Assert.AreEqual(";", result[2]);
        }

        [TestMethod]
        public void TestSplit_WithClosures()
        {
            var service = new MySqlQuerySplitter();
            var result = service.SplitOnDELIMITERKeyword(@"instruction1;
                                                           instruction2;
                                                           DELIMITER $$
                                                           closure1;
                                                           endclosure1$$
                                                           closure2;
                                                           endclosure2$$
                                                           DELIMITER ;
                                                           instruction3;
                                                           instruction4;");


            Assert.AreEqual(5, result.Length);
            Assert.AreEqual("$$", result[1]);
            Assert.AreEqual(";", result[3]);
        }

        [TestMethod]
        public void TestSplitOnDelimiter()
        {
            var service = new MySqlQuerySplitter();
            var result = service.SplitOnDelimiter(@"instruction1;instruction2;instruction3;instruction4;");

            Assert.AreEqual(4, result.Length);
            Assert.AreEqual("instruction1", result[0]);
            Assert.AreEqual("instruction2", result[1]);
            Assert.AreEqual("instruction3", result[2]);
            Assert.AreEqual("instruction4", result[3]);
        }

        [TestMethod]
        public void TestSplitOnDelimiter_WithClosures()
        {
            var service = new MySqlQuerySplitter();
            var result = service.SplitOnDelimiter(@"closure1;endclosure1$$closure2;endclosure2$$", "\\s*\\$\\$\\s*");


            Assert.AreEqual(2, result.Length);
            Assert.AreEqual("closure1;endclosure1", result[0]);
            Assert.AreEqual("closure2;endclosure2", result[1]);
        }

        [TestMethod]
        public void TestSplitQuery_WithNoClosures()
        {
            var service = new MySqlQuerySplitter();
            var result = service.SplitQuery(@"instruction1;instruction2;instruction3;instruction4;");

            Assert.AreEqual(4, result.Length);
            Assert.AreEqual("instruction1", result[0]);
            Assert.AreEqual("instruction2", result[1]);
            Assert.AreEqual("instruction3", result[2]);
            Assert.AreEqual("instruction4", result[3]);
        }

        [TestMethod]
        public void TestSplitQuery_WithClosureToStart()
        {
            var service = new MySqlQuerySplitter();
            var result = service.SplitQuery(@"DELIMITER $$
                                                closure1;
                                                endclosure1$$
                                                closure2;
                                                endclosure2$$
                                                DELIMITER ;
                                                instruction1;
                                                instruction2;");

            Assert.AreEqual(4, result.Length);
            Assert.AreEqual("closure1;\r\n                                                endclosure1", result[0]);
            Assert.AreEqual("closure2;\r\n                                                endclosure2", result[1]);
            Assert.AreEqual("instruction1", result[2]);
            Assert.AreEqual("instruction2", result[3]);
        }

        [TestMethod]
        public void TestSplitQuery_WithClosures()
        {
            var service = new MySqlQuerySplitter();
            var result = service.SplitQuery(@"instruction1;
                                                instruction2;
                                                DELIMITER $$
                                                closure1;
                                                endclosure1$$
                                                closure2;
                                                endclosure2$$
                                                DELIMITER ;
                                                instruction3;
                                                instruction4;");

            Assert.AreEqual(6, result.Length);
            Assert.AreEqual("instruction1", result[0]);
            Assert.AreEqual("instruction2", result[1]);
            Assert.AreEqual("closure1;\r\n                                                endclosure1", result[2]);
            Assert.AreEqual("closure2;\r\n                                                endclosure2", result[3]);
            Assert.AreEqual("instruction3", result[4]);
            Assert.AreEqual("instruction4", result[5]);
        }

    }
}

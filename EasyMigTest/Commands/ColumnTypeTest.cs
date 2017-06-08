using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyMigLib.Commands;

namespace EasyMigTest.Commands
{
    [TestClass]
    public class ColumnTypeTest
    {

        // tiny

        [TestMethod]
        public void TestTinyInt()
        {
            var result = ColumnType.TinyInt();
            Assert.AreEqual(false,result.Unsigned);
            Assert.AreEqual(typeof(TinyIntColumnType), result.GetType());

            var result2 = ColumnType.TinyInt(true);
            Assert.AreEqual(true, result2.Unsigned);
            Assert.AreEqual(typeof(TinyIntColumnType), result2.GetType());
        }

        // small

        [TestMethod]
        public void TestSmallInt()
        {
            var result = ColumnType.SmallInt();
            Assert.AreEqual(false, result.Unsigned);
            Assert.AreEqual(typeof(SmallIntColumnType), result.GetType());

            var result2 = ColumnType.SmallInt(true);
            Assert.AreEqual(true, result2.Unsigned);
            Assert.AreEqual(typeof(SmallIntColumnType), result2.GetType());
        }

        // int

        [TestMethod]
        public void TestInt()
        {
            var result = ColumnType.Int();
            Assert.AreEqual(false, result.Unsigned);
            Assert.AreEqual(typeof(IntColumnType), result.GetType());

            var result2 = ColumnType.Int(true);
            Assert.AreEqual(true, result2.Unsigned);
            Assert.AreEqual(typeof(IntColumnType), result2.GetType());
        }

        // big

        [TestMethod]
        public void TestBigInt()
        {
            var result = ColumnType.BigInt();
            Assert.AreEqual(false, result.Unsigned);
            Assert.AreEqual(typeof(BigIntColumnType), result.GetType());

            var result2 = ColumnType.BigInt(true);
            Assert.AreEqual(true, result2.Unsigned);
            Assert.AreEqual(typeof(BigIntColumnType), result2.GetType());
        }

        // char

        [TestMethod]
        public void TestChar()
        {
            var result = ColumnType.Char();
            Assert.AreEqual(10, result.Length);
            Assert.AreEqual(typeof(CharColumnType), result.GetType());

            var result2 = ColumnType.Char(100);
            Assert.AreEqual(100, result2.Length);
            Assert.AreEqual(typeof(CharColumnType), result2.GetType());
        }

        // bit

        [TestMethod]
        public void TestBit()
        {
            var result = ColumnType.Bit();
            Assert.AreEqual(typeof(BitColumnType), result.GetType());
        }

        // float

        [TestMethod]
        public void TestFloat()
        {
            var result = ColumnType.Float();
            Assert.AreEqual(false, result.Digits.HasValue);
            Assert.AreEqual(typeof(FloatColumnType), result.GetType());

            var result2 = ColumnType.Float(2);
            Assert.AreEqual(true, result2.Digits.HasValue);
            Assert.AreEqual(2, result2.Digits.Value);
            Assert.AreEqual(typeof(FloatColumnType), result2.GetType());
        }

        // varchar

        [TestMethod]
        public void TestVarChar()
        {
            var result = ColumnType.VarChar();
            Assert.AreEqual(255, result.Length);
            Assert.AreEqual(typeof(VarCharColumnType), result.GetType());

            var result2 = ColumnType.VarChar(100);
            Assert.AreEqual(100, result2.Length);
            Assert.AreEqual(typeof(VarCharColumnType), result2.GetType());
        }

        // text

        [TestMethod]
        public void TestText()
        {
            var result = ColumnType.Text();
            Assert.AreEqual(typeof(TextColumnType), result.GetType());
        }

        // longtext

        [TestMethod]
        public void TestLongText()
        {
            var result = ColumnType.LongText();
            Assert.AreEqual(typeof(LongTextColumnType), result.GetType());
        }

        // datetime

        [TestMethod]
        public void TestDateTime()
        {
            var result = ColumnType.DateTime();
            Assert.AreEqual(typeof(DateTimeColumnType), result.GetType());
        }

        // date

        [TestMethod]
        public void TestDate()
        {
            var result = ColumnType.Date();
            Assert.AreEqual(typeof(DateColumnType), result.GetType());
        }

        // time

        [TestMethod]
        public void TestTime()
        {
            var result = ColumnType.Time();
            Assert.AreEqual(typeof(TimeColumnType), result.GetType());
        }

        // timestamp

        [TestMethod]
        public void TestTimestamp()
        {
            var result = ColumnType.Timestamp();
            Assert.AreEqual(typeof(TimestampColumnType), result.GetType());
        }

        // blob

        [TestMethod]
        public void TestBlob()
        {
            var result = ColumnType.Blob();
            Assert.AreEqual(typeof(BlobColumnType), result.GetType());
        }
    }
}

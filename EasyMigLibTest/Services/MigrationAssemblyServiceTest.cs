using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyMigLib.Services;
using EasyMigLib;
using EasyMigLib.Commands;

namespace EasyMigLibTest.Services
{
    [TestClass]
    public class MigrationAssemblyServiceTest
    {
        [TestMethod]
        public void TestFindTypes_Filter()
        {
            var service = new MigrationAssemblyService();
            var types = new Type[]
            {
                typeof(SeederA),
                typeof(A),
                typeof(NotMigration)
            };

            var result = service.FindTypes<Migration>(types);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("A", result[0].FullName);
            Assert.AreEqual("A", result[0].Name);
            Assert.AreEqual(typeof(A), result[0].Type);
            Assert.AreEqual(null, result[0].Version);
        }

        [TestMethod]
        public void TestFindTypes_FilterWithSeeder()
        {
            var service = new MigrationAssemblyService();
            var types = new Type[]
            {
                typeof(SeederA),
                typeof(A),
                typeof(NotMigration)
            };

            var result = service.FindTypes<Seeder>(types);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("SeederA", result[0].FullName);
            Assert.AreEqual("SeederA", result[0].Name);
            Assert.AreEqual(typeof(SeederA), result[0].Type);
            Assert.AreEqual(null, result[0].Version);
        }

        [TestMethod]
        public void TestFindTypes_Sort()
        {
            var service = new MigrationAssemblyService();
            var types = new Type[]
            {
                typeof(A),
                typeof(_2_A),
                typeof(_1_B),
                typeof(B),
                typeof(_1_A)
            };

            var result = service.FindTypes<Migration>(types);
            Assert.AreEqual("_1_A", result[0].FullName);
            Assert.AreEqual("A", result[0].Name);
            Assert.AreEqual(typeof(_1_A), result[0].Type);
            Assert.AreEqual("_1_", result[0].Version);

            Assert.AreEqual("_2_A", result[1].FullName);
            Assert.AreEqual("A", result[1].Name);
            Assert.AreEqual(typeof(_2_A), result[1].Type);
            Assert.AreEqual("_2_", result[1].Version);

            Assert.AreEqual("_1_B", result[2].FullName);
            Assert.AreEqual("B", result[2].Name);
            Assert.AreEqual(typeof(_1_B), result[2].Type);
            Assert.AreEqual("_1_", result[2].Version);

            Assert.AreEqual("A", result[3].FullName);
            Assert.AreEqual("A", result[3].Name);
            Assert.AreEqual(typeof(A), result[3].Type);
            Assert.AreEqual(null, result[3].Version);

            Assert.AreEqual("B", result[4].FullName);
            Assert.AreEqual("B", result[4].Name);
            Assert.AreEqual(typeof(B), result[4].Type);
            Assert.AreEqual(null, result[4].Version);
        }

        [TestMethod]
        public void TestGroup()
        {
            var service = new MigrationAssemblyService();
            var types = new Type[]
            {
                typeof(A),
                typeof(_2_A),
                typeof(_1_B),
                typeof(B),
                typeof(_1_A)
            };

            var result = service.FindTypes<Migration>(types);

            var groups = service.Group(result);

            Assert.AreEqual(2,groups.Count);

            Assert.AreEqual("A", groups["A"][0].Name);
            Assert.AreEqual("A", groups["A"][0].FullName);
            Assert.AreEqual(null, groups["A"][0].Version);

            Assert.AreEqual("A", groups["A"][1].Name);
            Assert.AreEqual("_1_A", groups["A"][1].FullName);
            Assert.AreEqual("_1_", groups["A"][1].Version);

            Assert.AreEqual("A", groups["A"][2].Name);
            Assert.AreEqual("_2_A", groups["A"][2].FullName);
            Assert.AreEqual("_2_", groups["A"][2].Version);

            Assert.AreEqual("B", groups["B"][0].Name);
            Assert.AreEqual("B", groups["B"][0].FullName);
            Assert.AreEqual(null, groups["B"][0].Version);

            Assert.AreEqual("B", groups["B"][1].Name);
            Assert.AreEqual("_1_B", groups["B"][1].FullName);
            Assert.AreEqual("_1_", groups["B"][1].Version);
        }

        [TestMethod]
        public void TestRunSeeder()
        {
            var service = new MigrationAssemblyService();
            var recognizedFile = new RecognizedMigrationFile(typeof(SeederC), "SeederC", "SeederC");

            Assert.IsFalse(SeederC.IsCalled);

            service.RunSeeder(recognizedFile);

            Assert.IsTrue(SeederC.IsCalled);
        }

        [TestMethod]
        public void TestRunMigration_Up()
        {
            C.DownIsCalled = false;
            C.DownIsCalled = false;

            var service = new MigrationAssemblyService();
            var recognizedFile = new RecognizedMigrationFile(typeof(C), "C", "C");

            Assert.IsFalse(C.UpIsCalled);
            Assert.IsFalse(C.DownIsCalled);

            service.RunMigration(recognizedFile,MigrationDirection.Up);

            Assert.IsTrue(C.UpIsCalled);
            Assert.IsFalse(C.DownIsCalled);
        }

        [TestMethod]
        public void TestRunMigration_Down()
        {
            A.DownIsCalled = false;
            A.DownIsCalled = false;

            var service = new MigrationAssemblyService();
            var recognizedFile = new RecognizedMigrationFile(typeof(A), "A", "A");

            Assert.IsFalse(A.UpIsCalled);
            Assert.IsFalse(A.DownIsCalled);

            service.RunMigration(recognizedFile, MigrationDirection.Down);

            Assert.IsTrue(A.DownIsCalled);
            Assert.IsFalse(A.UpIsCalled);
        }
    }

    public class NotMigration { }

    public class SeederA : Seeder
    {
        public static bool IsCalled = false;

        public override void Run()
        {
            IsCalled = true;
        }
    }

    public class SeederB : Seeder
    {
        public static bool IsCalled = false;

        public override void Run()
        {
            IsCalled = true;
        }
    }

    public class SeederC : Seeder
    {
        public static bool IsCalled = false;

        public override void Run()
        {
            IsCalled = true;
        }
    }

    public class A : Migration
    {
        public static bool UpIsCalled = false;
        public static bool DownIsCalled = false;

        public override void Up()
        {
            UpIsCalled = true;
        }

        public override void Down()
        {
            DownIsCalled = true;
        }
    }

    public class B : Migration
    {
        public static bool UpIsCalled = false;
        public static bool DownIsCalled = false;

        public override void Up()
        {
            UpIsCalled = true;
        }

        public override void Down()
        {
            DownIsCalled = true;
        }

    }

    public class C : Migration
    {
        public static bool UpIsCalled = false;
        public static bool DownIsCalled = false;

        public override void Up()
        {
            UpIsCalled = true;
        }

        public override void Down()
        {
            DownIsCalled = true;
        }
    }

    public class _1_A : Migration { }

    public class _2_A : Migration { }

    public class _1_B : Migration { }

    public class _2_Migration : Migration
    {
        public override void Up()
        {

        }

        public override void Down()
        {

        }
    }

    public class _1_Migration : Migration
    {
        public override void Up()
        {

        }

        public override void Down()
        {

        }
    }


    public class FirstMigration : Migration
    {
        public override void Up()
        {
          
        }

        public override void Down()
        {
            
        }
    }

    public class SecondMigration : Migration
    {
        public override void Up()
        {

        }

        public override void Down()
        {

        }
    }
}

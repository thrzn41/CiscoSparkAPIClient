using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading.Tasks;
using Thrzn41.CiscoSpark;
using Thrzn41.CiscoSpark.Version1;
using Thrzn41.Util;

namespace UnitTest.DotNetCore.Thrzn41.CiscoSpark
{
    [TestClass]
    public class UnitTestBasic
    {
        private const string UNIT_TEST_SPACE_TAG = "#ciscosparkapiclientunittestspace";

        private const string UNIT_TEST_CREATED_PREFIX = "#createdciscosparkapiclientunittest";

        SparkAPIClient spark;

        Space unitTestSpace;

        [TestInitialize]
        public async Task Init()
        {
            string userDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            var dirInfo = new DirectoryInfo(String.Format("{0}{1}.thrzn41{1}unittest{1}spark", userDir, Path.DirectorySeparatorChar));

            byte[] encryptedToken;
            byte[] entropy;

            using (var stream = new FileStream(String.Format("{0}{1}sparktoken.dat", dirInfo.FullName, Path.DirectorySeparatorChar), FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var memory = new MemoryStream())
            {
                stream.CopyTo(memory);

                encryptedToken = memory.ToArray();
            }

            using (var stream = new FileStream(String.Format("{0}{1}tokenentropy.dat", dirInfo.FullName, Path.DirectorySeparatorChar), FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var memory = new MemoryStream())
            {
                stream.CopyTo(memory);

                entropy = memory.ToArray();
            }


            spark = SparkAPI.CreateVersion1Client(ProtectedString.FromEncryptedData(encryptedToken, entropy));

            var r = await spark.ListSpacesAsync(sortBy:SpaceSortBy.Created, max:50);

            while (true)
            {
                if (r.IsSuccessStatus && r.Data.HasItems)
                {
                    foreach (var item in r.Data.Items)
                    {
                        if (item.Title.Contains(UNIT_TEST_SPACE_TAG))
                        {
                            unitTestSpace = item;
                            break;
                        }
                    }
                }

                if (unitTestSpace == null && r.HasNext)
                {
                    r = await r.ListNextAsync();
                }
                else
                {
                    break;
                }
            }
        }


        [TestCleanup]
        public async Task Clean()
        {
            var r = await spark.ListSpacesAsync(sortBy: SpaceSortBy.Created, max: 50);

            while (true)
            {
                if (r.IsSuccessStatus && r.Data.HasItems)
                {
                    foreach (var item in r.Data.Items)
                    {
                        if (item.Title.Contains(UNIT_TEST_CREATED_PREFIX))
                        {
                            await spark.DeleteSpaceAsync(item);
                        }
                    }
                }

                if (unitTestSpace == null && r.HasNext)
                {
                    r = await r.ListNextAsync();
                }
                else
                {
                    break;
                }
            }
        }

        [TestMethod]
        public async Task TestCreateMessage()
        {
            var r = await this.spark.CreateMessageAsync(unitTestSpace.Id, "Hello, Cisco Spark!!");

            Assert.IsTrue(r.IsSuccessStatus);
            Assert.IsTrue(r.Data.HasValues);

            Assert.IsNotNull(r.Data.Id);
            Assert.IsNotNull(r.TrackingId);

            Assert.AreEqual("Hello, Cisco Spark!!", r.Data.Text);

        }

        [TestMethod]
        public async Task TestCreateAndDeleteMessage()
        {
            var r = await this.spark.CreateMessageAsync(unitTestSpace.Id, "This message will be deleted.");

            Assert.IsTrue(r.IsSuccessStatus);
            Assert.IsTrue(r.Data.HasValues);

            Assert.IsNotNull(r.Data.Id);

            var rdm = await this.spark.DeleteMessageAsync(r.Data);

            Assert.IsTrue(rdm.IsSuccessStatus);
            Assert.IsFalse(rdm.Data.HasValues);

        }

        [TestMethod]
        public async Task TestPagination()
        {
            for (int i = 0; i < 5; i++)
            {
                var r = await this.spark.CreateSpaceAsync(String.Format("Test Space {0}{1}", UNIT_TEST_CREATED_PREFIX, i));

                Assert.IsTrue(r.IsSuccessStatus);
            }

            var rls = await this.spark.ListSpacesAsync(max:2);

            Assert.IsTrue(rls.IsSuccessStatus);
            Assert.IsTrue(rls.HasNext);
            Assert.AreEqual(2, rls.Data.ItemCount);

            rls = await rls.ListNextAsync();

            Assert.IsTrue(rls.IsSuccessStatus);
            Assert.IsTrue(rls.HasNext);
            Assert.AreEqual(2, rls.Data.ItemCount);

        }

    }
}

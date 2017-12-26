using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Reflection;
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

            spark = SparkAPI.CreateVersion1Client(LocalProtectedString.FromEncryptedData(encryptedToken, entropy));

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


            checkSparkAPIClient();
            checkUnitTestSpace();
        }


        [TestCleanup]
        public async Task Clean()
        {
            if(this.spark == null)
            {
                return;
            }

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

        private void checkSparkAPIClient()
        {
            if (this.spark == null)
            {
                Assert.Fail("You need to configure Cisco Spark Token by using UnitTestTool.EncryptCiscoSparkTokenForm.");
            }
        }

        private void checkUnitTestSpace()
        {
            if (this.unitTestSpace == null)
            {
                Assert.Fail("You need to create manually a Space for unit test that contains name '#ciscosparkapiclientunittestspace' in Title, and add bot or integration user that is used in the test.");
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
        public async Task TestCreateMessageWithMarkdown()
        {
            var r = await this.spark.CreateMessageAsync(unitTestSpace.Id, "Hello, **markdown**!!");

            Assert.IsTrue(r.IsSuccessStatus);
            Assert.IsTrue(r.Data.HasValues);

            Assert.IsNotNull(r.Data.Id);
            Assert.IsNotNull(r.TrackingId);

            Assert.AreEqual("Hello, markdown!!", r.Data.Text);

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
        public async Task TestCreateMessageWithAttachment()
        {
            
            var exePath = new FileInfo(Assembly.GetExecutingAssembly().Location);
            
            string path = String.Format("{0}{1}TestData{1}thrzn41.png", exePath.DirectoryName, Path.DirectorySeparatorChar);

            Uri fileUri = null;
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var data = new SparkFileData(fs, "mypng.png", SparkMediaType.ImagePNG))
            {
                var r = await this.spark.CreateMessageAsync(unitTestSpace.Id, "**Post with attachment!!**", data);
                Assert.IsTrue(r.IsSuccessStatus);
                Assert.IsTrue(r.Data.HasValues);
                Assert.IsTrue(r.Data.HasFiles);

                fileUri = r.Data.FileUris[0];
            }

            var r1 = await this.spark.GetFileInfoAsync(fileUri);
            Assert.IsTrue(r1.IsSuccessStatus);
            Assert.IsTrue(r1.Data.HasValues);

            Assert.AreEqual("mypng.png", r1.Data.FileName);
            Assert.AreEqual(SparkMediaType.ImagePNG, r1.Data.MediaType);
            Assert.AreEqual(34991, r1.Data.Size);


            var r2 = await this.spark.GetFileDataAsync(fileUri);
            Assert.IsTrue(r2.IsSuccessStatus);
            Assert.IsTrue(r2.Data.HasValues);

            Assert.AreEqual("mypng.png", r2.Data.FileName);
            Assert.AreEqual(SparkMediaType.ImagePNG, r2.Data.MediaType);
            Assert.AreEqual(34991, r2.Data.Size);

            using (var data = r2.Data.Stream)
            {
                Assert.AreEqual(34991, data.Length);
            }

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


        [TestMethod]
        public void TestPartialErrorResponse()
        {
            string str = "{\"items\":[{\"id\":\"id01\",\"title\":\"title01\",\"type\":\"group\",\"isLocked\":true,\"teamId\":\"teamid01\",\"lastActivity\":\"2016-04-21T19:12:48.920Z\",\"created\":\"2016-04-21T19:01:55.966Z\"},{\"id\":\"id02\",\"title\":\"xyz...\",\"errors\":{\"title\":{\"code\":\"kms_failure\",\"reason\":\"Key management server failed to respond appropriately. For more information: https://developer.ciscospark.com/errors.html\"}}}]}";

            var spaces = SparkObject.FromJsonString<SpaceList>(str);

            var space = spaces.Items[1];

            var errors = space.GetPartialErrors();

            Assert.IsTrue(errors.ContainsKey("title"));
            Assert.AreEqual(PartialErrorCode.KMSFailure, errors["title"].Code);
            Assert.AreEqual("Key management server failed to respond appropriately. For more information: https://developer.ciscospark.com/errors.html", errors["title"].Reason);

        }

        [TestMethod]
        public void TestErrorResponse()
        {
            string str = "{\"errorCode\":1,\"message\":\"The requested resource could not be found.\",\"errors\":[{\"errorCode\":1,\"description\":\"The requested resource could not be found.\"}],\"trackingId\":\"xyz\"}";

            var spaces = SparkObject.FromJsonString<SpaceList>(str);

            var errors = spaces.GetErrors();

            Assert.AreEqual(1, errors[0].ErrorCode);
            Assert.AreEqual("The requested resource could not be found.", errors[0].Description);

        }


    }
}

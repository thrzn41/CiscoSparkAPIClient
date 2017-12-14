using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Specialized;
using System.Linq;
using Thrzn41.Util;

namespace UnitTest.DotNetCore.Thrzn41.Util
{
    [TestClass]
    public class UnitTestBasic
    {

        [TestMethod]
        public void TestProtectedString()
        {
            var ps1 = ProtectedString.FromString("Hello");
            var ps2 = ProtectedString.FromString("Hello");

            Assert.AreEqual(128, ps1.Entropy.Length);
            Assert.AreEqual(128, ps2.Entropy.Length);

            Assert.AreEqual(false, ps1.EncryptedData.SequenceEqual(ps2.EncryptedData));

            Assert.AreEqual("Hello", ps1.DecryptToString());
            Assert.AreEqual("Hello", ps2.DecryptToString());

        }


        [TestMethod]
        public void TestClearChars()
        {
            char[] chars = { 'a', 'b', 'c', 'd' };

            ProtectedString.ClearChars(chars);

            foreach (var item in chars)
            {
                Assert.AreEqual('\0', item);
            }
        }


        [TestMethod]
        public void TestCryptoRandom()
        {
            CryptoRandom rand = new CryptoRandom();

            var rb1 = rand.NextBytes(4);
            Assert.AreEqual(4, rb1.Length);

            var rb2 = rand.NextBytes(10);
            Assert.AreEqual(10, rb2.Length);

            Assert.AreEqual(0, rand.NextInt(0));
            Assert.AreEqual(0, rand.NextInt(1));

            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => 
                {
                    rand.NextInt(-10);
                }
                );


        }

        
        [TestMethod]
        public void TestBuildQueryParam()
        {
            var nvc1 = new NameValueCollection();

            nvc1.Add("a", "b");
            nvc1.Add("c", "b");

            Assert.AreEqual("a=b&c=b", HttpUtils.BuildQueryParameters(nvc1));


            var nvc2 = new NameValueCollection();

            nvc2.Add("a", "b");
            nvc2.Add("a", "c");

            Assert.AreEqual("a=b&a=c", HttpUtils.BuildQueryParameters(nvc2));


            var nvc3 = new NameValueCollection();

            nvc3.Add("a", "b");
            nvc3.Add("a", null);
            nvc3.Add("a", "c");

            Assert.AreEqual("a=b&a=c", HttpUtils.BuildQueryParameters(nvc3));


            var nvc4 = new NameValueCollection();

            nvc4.Add("a b", "b&c");
            nvc4.Add("d%e", "f$g");

            Assert.AreEqual("a%20b=b%26c&d%25e=f%24g", HttpUtils.BuildQueryParameters(nvc4));


            var nvc5 = new NameValueCollection();

            nvc5.Add("a", "b");
            nvc5.Add("c", null);
            nvc5.Add("d", "e");

            Assert.AreEqual("a=b&d=e", HttpUtils.BuildQueryParameters(nvc5));

        }


        [TestMethod]
        public void TestBuildUri()
        {
            var uri = new Uri("https://api.example.com/path1/path2");
            var nvc = new NameValueCollection();

            nvc.Add("a", "b");
            nvc.Add("c", "b");

            Assert.AreEqual("https://api.example.com/path1/path2?a=b&c=b", HttpUtils.BuildUri(uri, nvc).AbsoluteUri);


            uri = new Uri("https://api.example.com/path1/path2");
            nvc = new NameValueCollection();

            nvc.Add("a b", "b&c");
            nvc.Add("d%e", "f$g");

            Assert.AreEqual("https://api.example.com/path1/path2?a%20b=b%26c&d%25e=f%24g", HttpUtils.BuildUri(uri, nvc).AbsoluteUri);


            uri = new Uri("https://api.example.com/path1/path2/");
            nvc = new NameValueCollection();

            nvc.Add("a b", "b&c");
            nvc.Add("d%e", "f$g");

            Assert.AreEqual("https://api.example.com/path1/path2/?a%20b=b%26c&d%25e=f%24g", HttpUtils.BuildUri(uri, nvc).AbsoluteUri);



            uri = new Uri("https://api.example.com/path1/path2?a=b&c=d");
            nvc = new NameValueCollection();

            nvc.Add("e", "f");
            nvc.Add("g", "h");

            Assert.AreEqual("https://api.example.com/path1/path2?a=b&c=d&e=f&g=h", HttpUtils.BuildUri(uri, nvc).AbsoluteUri);

            uri = new Uri("http://api.example.com/path1/path2?a=b&c=d");
            nvc = new NameValueCollection();

            nvc.Add("e", "f");
            nvc.Add("g", "h");

            Assert.AreEqual("http://api.example.com/path1/path2?a=b&c=d&e=f&g=h", HttpUtils.BuildUri(uri, nvc).AbsoluteUri);


            uri = new Uri("ftp://api.example.com/path1/path2?a=b&c=d");
            nvc = new NameValueCollection();

            nvc.Add("e", "f");
            nvc.Add("g", "h");

            Assert.ThrowsException<ArgumentException>(
                () =>
                {
                    HttpUtils.BuildUri(uri, nvc);
                }
                );



        }

    }
}

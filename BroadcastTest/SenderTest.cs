using BroadcastLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BroadcastTest
{
    
    
    /// <summary>
    ///This is a test class for SenderTest and is intended
    ///to contain all SenderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SenderTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Send
        ///</summary>
        [TestMethod()]
        public void SendTest()
        {
            Sender target = new Sender();
            target.Init(15000, null);
            string message = "Hello world";
            int expected = 11; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.Send(message);
            target.Close();

            Assert.AreEqual(expected, actual);
         }
    }
}

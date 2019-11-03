using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RidersApp;

namespace RideRefTests
{
    [TestClass]
    public class LoggerTests
    {
        [TestMethod]
        public void LoggingEntryWithNullastheOnlyparameter()
        {
            StringBuilder sb = new StringBuilder();
            for (int a = 0; a <= 80; a++) { sb.Append('*'); }
            string str = Logger.BuildLine(null);

            bool result = str == sb.ToString() ? true : false;
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void LoggingEntryWithNullandDasharameter()
        {
            StringBuilder sb = new StringBuilder();
            for (int a = 0; a <= 80; a++) { sb.Append('-'); }
            string str = Logger.BuildLine(null, '-');

            bool result = str == sb.ToString() ? true : false;
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void LoggingEntryWithTitle()
        { 
            string title = "Test Title";
            StringBuilder sb = new StringBuilder();
            for (int a = 0; a <= 80-title.Length; a++) {
                if (a==((80 - title.Length) / 2)) { sb.Append(title); }
                sb.Append('*');
            }
            string str = Logger.BuildLine(title );

            bool result = str == sb.ToString() ? true : false;
            Assert.IsTrue(result);
        }
    }
}

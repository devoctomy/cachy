using System;
using devoctomy.cachy.Build.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace devoctomy.cachy.Tests.Build
{

    [TestClass]
    public class ChangeLogTests
    {

        #region public methods

        [TestMethod]
        public void GenerateReleaseSummary()
        {
            string summary = ChangeLog.CreateLatestReleaseSummary();
            Console.WriteLine(summary);
        }

        #endregion

    }

}

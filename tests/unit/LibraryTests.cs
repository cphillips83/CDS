using System;
using CDS;
using CDS.FileSystem;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace unit
{
    class ManyConsoleCommandTest : ManyConsole.ConsoleCommand
    {
        public override int Run(string[] remainingArguments)
        {
            return 0;
        }
    }

    [TestClass]
    public class LibraryTests
    {
        static LibraryTests()
        {
            EmbeddedAssembly.InitializeResolver();
        }

        [TestMethod]
        public void Library_Load_Common()
        {
            Assert.IsNotNull(EmbeddedAssembly.LoadCommon());
            Assert.IsNotNull(new FileEntry(string.Empty, string.Empty));
        }

        [TestMethod]
        public void Library_Load_Server()
        {
            Assert.IsNotNull(EmbeddedAssembly.LoadServer());
        }

        [TestMethod]
        public void Library_Load_Client()
        {
            Assert.IsNotNull(EmbeddedAssembly.LoadClient());
        }

        [TestMethod]
        public void Library_Load_ManyConsole()
        {
            Assert.IsNotNull(EmbeddedAssembly.LoadManyConsole());
            Assert.IsNotNull(new ManyConsoleCommandTest());
        }

        [TestMethod]
        public void Library_Load_NDeskOptions()
        {
            Assert.IsNotNull(EmbeddedAssembly.LoadNDeskOptions());
            Assert.IsNotNull(new NDesk.Options.OptionSet());
        }
    }
}

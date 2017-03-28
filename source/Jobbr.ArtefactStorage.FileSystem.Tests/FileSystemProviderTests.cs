using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jobbr.ArtefactStorage.FileSystem.Tests
{
    [TestClass]
    public class FileSystemProviderTests
    {
        private static FileSystemArtefactsStorageProvider GivenArtefactStorageProviderInCurrentPath()
        {
            return new FileSystemArtefactsStorageProvider(new FileSystemArtefactStorageConfiguration() { DataDirectory = Directory.GetCurrentDirectory() });
        }

        private static void GivenAContainerWithFourArbitraryFiles(string container)
        {
            Directory.CreateDirectory(container);
            File.WriteAllText(container + "\\file1.txt", "Hello1");
            File.WriteAllText(container + "\\file2.doc", "some bites");
            File.WriteAllText(container + "\\file3.log", "Debug[] blablalb");
            File.WriteAllText(container + "\\file4.pdf", "some bites");
        }

        [TestCleanup]
        public void CleanupAfterEachTest()
        {
            if (Directory.Exists("container"))
            {
            Directory.Delete("container", true);
        }
        }

        [TestMethod]
        public void SaveFile_WithValidName_ContainerIsCreated()
        {
            var provider = GivenArtefactStorageProviderInCurrentPath();

            provider.Save("container", "test123.txt", new MemoryStream(500));

            Assert.IsTrue(Directory.Exists("container"));
        }

        [TestMethod]
        public void SaveFile_WithValidName_FileIsCreated()
        {
            var provider = GivenArtefactStorageProviderInCurrentPath();

            provider.Save("container", "test123.txt", new MemoryStream(500));

            Assert.IsTrue(File.Exists("container\\test123.txt"));
        }

        [TestMethod]
        public void SaveFile_WithValidName_ContentIsSaved()
        {
            var provider = GivenArtefactStorageProviderInCurrentPath();
            
            provider.Save("container", "test123.txt", new MemoryStream(Encoding.UTF8.GetBytes("HelloWorld")));

            var fileContent = File.ReadAllText("container\\test123.txt", Encoding.UTF8);
            Assert.AreEqual("HelloWorld", fileContent);
        }

        [TestMethod]
        public void SaveFile_WithExistingFile_FileIsOverwritten()
        {
            var provider = GivenArtefactStorageProviderInCurrentPath();

            provider.Save("container", "test123.txt", new MemoryStream(Encoding.UTF8.GetBytes("HelloWorld")));
            provider.Save("container", "test123.txt", new MemoryStream(Encoding.UTF8.GetBytes("NewValue")));

            var fileContent = File.ReadAllText("container\\test123.txt", Encoding.UTF8);
            Assert.AreEqual("NewValue", fileContent);
        }

        [TestMethod]
        public void ExistingContainerWithFiles_GetArtefacts_ReturnsList()
        {
            GivenAContainerWithFourArbitraryFiles("container");
            var provider = GivenArtefactStorageProviderInCurrentPath();

            var artefacts = provider.GetArtefacts("container");

            Assert.IsNotNull(artefacts);
            Assert.AreEqual(4, artefacts.Count);
        }

        [TestMethod]
        public void ExistingContainerWithFiles_GetArtefacts_AllHaveFileName()
        {
            GivenAContainerWithFourArbitraryFiles("container");
            var provider = GivenArtefactStorageProviderInCurrentPath();

            var artefacts = provider.GetArtefacts("container");

            var itemsWithoutFileNames = artefacts.Where(a => string.IsNullOrWhiteSpace(a.FileName)).ToList();

            Assert.IsNotNull(artefacts);
            Assert.AreEqual(0, itemsWithoutFileNames.Count);
        }

        [TestMethod]
        public void ExistingContainerWithFiles_GetArtefacts_AllHaveFileSize()
        {
            GivenAContainerWithFourArbitraryFiles("container");
            var provider = GivenArtefactStorageProviderInCurrentPath();

            var artefacts = provider.GetArtefacts("container");

            var itemsWithoutFileSizes = artefacts.Where(a => a.Size == 0).ToList();

            Assert.IsNotNull(artefacts);
            Assert.AreEqual(0, itemsWithoutFileSizes.Count, "The following files did not supply their Size: " + string.Join(", ", itemsWithoutFileSizes.Select(a => a.FileName)));
        }

    }
}

using System;
using System.IO;
using System.Linq;
using Jobbr.ComponentModel.ArtefactStorage;
using Jobbr.ComponentModel.JobStorage;
using Jobbr.ComponentModel.Registration;
using Jobbr.Server;
using Jobbr.Server.Builder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jobbr.ArtefactStorage.FileSystem.Tests
{
    [TestClass]
    public class ProviderRegistrationTests
    {
        [TestMethod]
        public void Jobbr_WithRegisteredFileStorage_CanBeStarted()
        {
            var builder = new JobbrBuilder();

            builder.AddFileSystemArtefactStorage(config =>
            {
                config.DataDirectory = Directory.GetCurrentDirectory();
            });

            using (var server = builder.Create())
            {
                server.Start();

                Assert.AreEqual(JobbrState.Running, server.State);
            }
        }

        [TestMethod]
        public void Jobbr_WithRegisteredFileStorage_StorageProviderHasCorrectType()
        {
            var builder = new JobbrBuilder();

            builder.AddFileSystemArtefactStorage(config =>
            {
                config.DataDirectory = Directory.GetCurrentDirectory();
            });

            builder.Register<IJobbrComponent>(typeof(DirectServiceAccessComponent));

            using (var server = builder.Create())
            {
                server.Start();

                Assert.IsNotNull(DirectServiceAccessComponent.Instance.artefactStorageProvider);
                Assert.AreEqual(typeof(FileSystemArtefactsStorageProvider), DirectServiceAccessComponent.Instance.artefactStorageProvider.GetType());
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Configuration_WithEmptyDataDirectoy_ThrowsExceptionOnJobStart()
        {
            var builder = new JobbrBuilder();

            builder.AddFileSystemArtefactStorage(config =>
            {
                config.DataDirectory = "";
            });

            using (var server = builder.Create())
            {
                server.Start();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Configuration_WithInvalidDrive_ThrowsExceptionOnJobStart()
        {
            var builder = new JobbrBuilder();

            var possibleDriveLetters = Enumerable.Range(65, 91).Select(i => (char)i);
            var usedDriveLetters = DriveInfo.GetDrives().Select(d => d.Name[0]);
            var invalidDrives = possibleDriveLetters.Except(usedDriveLetters);

            builder.AddFileSystemArtefactStorage(config =>
            {
                config.DataDirectory = invalidDrives.First().ToString() + ":\\test";
            });

            using (var server = builder.Create())
            {
                server.Start();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Configuration_WithInvalidPath_ThrowsExceptionOnJobStart()
        {
            var builder = new JobbrBuilder();

            builder.AddFileSystemArtefactStorage(config =>
            {
                config.DataDirectory = ":\\\\$^09//";
            });

            using (var server = builder.Create())
            {
                server.Start();
            }
        }

        [TestMethod]
        public void Configuration_WithPossibleUncPath_ThrowsExceptionOnJobStart()
        {
            var builder = new JobbrBuilder();

            builder.AddFileSystemArtefactStorage(config =>
            {
                config.DataDirectory = @"\\localhost\folder";
            });

            using (var server = builder.Create())
            {
                server.Start();
                Assert.AreEqual(JobbrState.Running, server.State);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Configuration_WithInaccessablePath_ThrowsExceptionOnJobStart()
        {
            var builder = new JobbrBuilder();

            var possibleDriveLetters = Enumerable.Range(65, 91).Select(i => (char)i);
            var usedDriveLetters = DriveInfo.GetDrives().Select(d => d.Name[0]);
            var invalidDrives = possibleDriveLetters.Except(usedDriveLetters);

            builder.AddFileSystemArtefactStorage(config =>
            {
                config.DataDirectory = invalidDrives.First().ToString() + ":\\test";
            });

            using (var server = builder.Create())
            {
                server.Start();
            }
        }
    }

    public class DirectServiceAccessComponent : IJobbrComponent
    {
        public static DirectServiceAccessComponent Instance;
        public readonly IArtefactsStorageProvider artefactStorageProvider;

        public DirectServiceAccessComponent(IArtefactsStorageProvider artefactsStorageProvider)
        {
            Instance = this;
            this.artefactStorageProvider = artefactsStorageProvider;
        }

        public void Dispose() { }

        public void Start() { }

        public void Stop() { }
    }
}

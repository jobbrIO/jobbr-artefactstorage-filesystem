using System;
using System.IO;
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

        public void Dispose()
        {
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }
    }
}

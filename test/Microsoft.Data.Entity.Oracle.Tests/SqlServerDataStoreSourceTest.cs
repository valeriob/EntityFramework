// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Data.Entity.Infrastructure;
using Moq;
using Xunit;

namespace Microsoft.Data.Entity.Oracle.Tests
{
    public class SqlServerDataStoreSourceTest
    {
        [Fact]
        public void Returns_appropriate_name()
        {
            Assert.Equal(typeof(OracleDataStore).Name, new SqlOracleDataStoreSourceName);
        }

        [Fact]
        public void Is_configured_when_configuration_contains_associated_extension()
        {
            var configuration = new DbContextOptions()
                .AddBuildAction(c => c.AddOrUpdateExtension<SqlServerConfigurationExtension>(e => { }))
                .BuildConfiguration();

            var configurationMock = new Mock<DbContextConfiguration>();
            configurationMock.Setup(m => m.ContextOptions).Returns(configuration);

            Assert.True(new OracleDataStoreSource().IsConfigured(configurationMock.Object));
        }

        [Fact]
        public void Is_not_configured_when_configuration_does_not_contain_associated_extension()
        {
            var configuration = new DbContextOptions().BuildConfiguration();

            var configurationMock = new Mock<DbContextConfiguration>();
            configurationMock.Setup(m => m.ContextOptions).Returns(configuration);

            Assert.False(new OracleDataStoreSource().IsConfigured(configurationMock.Object));
        }

        [Fact]
        public void Is_available_when_configured()
        {
            var configuration = new DbContextOptions()
                .AddBuildAction(c => c.AddOrUpdateExtension<SqlServerConfigurationExtension>(e => { }))
                .BuildConfiguration();

            var configurationMock = new Mock<DbContextConfiguration>();
            configurationMock.Setup(m => m.ContextOptions).Returns(configuration);

            Assert.True(new OracleDataStoreSource().IsAvailable(configurationMock.Object));
        }

        [Fact]
        public void Is_not_available_when_not_configured()
        {
            var configuration = new DbContextOptions().BuildConfiguration();

            var configurationMock = new Mock<DbContextConfiguration>();
            configurationMock.Setup(m => m.ContextOptions).Returns(configuration);

            Assert.False(new OracleDataStoreSource().IsAvailable(configurationMock.Object));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicConfig.Client;
using DynamicConfig.Client.Models;
using DynamicConfig.Client.Providers;
using Xunit;

namespace DynamicConfig.Client.Tests
{
    public class ConfigurationReaderTests
    {
        [Fact]
        public void GetValue_Should_Return_String_When_Config_Exists()
        {
            // Arrange
            var fakeProvider = new FakeConfigProvider();
            var reader = new ConfigurationReader("TEST", fakeProvider, 10000);

            // Act
            var value = reader.GetValue<string>("SiteName");

            // Assert
            Assert.Equal("soty.io", value);
        }

        [Fact]
        public void GetValue_Should_Return_Int_When_Config_Is_Int()
        {
            // Arrange
            var fakeProvider = new FakeConfigProvider();
            var reader = new ConfigurationReader("TEST", fakeProvider, 10000);

            // Act
            var value = reader.GetValue<int>("MaxItemCount");

            // Assert
            Assert.Equal(50, value);
        }

        [Fact]
        public void GetValue_Should_Throw_Exception_When_Key_Not_Found()
        {
            // Arrange
            var fakeProvider = new FakeConfigProvider();
            var reader = new ConfigurationReader("TEST", fakeProvider, 10000);

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() =>
                reader.GetValue<string>("NonExistingKey"));
        }


    }
}

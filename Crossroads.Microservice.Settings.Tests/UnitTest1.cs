using System;
using Xunit;
using Crossroads.Microservice.Services;
namespace Crossroads.Microservice.Settings.Tests
{
    public class UnitTest1
    {

        [Fact]
        public void GetSecret_NullKey()
        {
            var service = new SettingsService();

            //TODO: 
            Assert.ThrowsAny<Exception>(() => service.GetSecret(null));
        }
    }
}

using Kostal.Inverter.Piko;
using Xunit;
using Xunit.Abstractions;

namespace Kostal.Piko.Tests
{
    public class PikoClientTest
    {
        private readonly ITestOutputHelper output;

        public PikoClientTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void TestDeviceInfo()
        {
            using (var client = new PikoClient("192.168.0.43"))
            {
                var value = client.GetDeviceInfo(255);

                Assert.False(string.IsNullOrEmpty(value.Name));
                output.WriteLine($"Name: {value.Name}");

                Assert.False(string.IsNullOrEmpty(value.Model));
                output.WriteLine($"Model: {value.Model}");

                Assert.False(string.IsNullOrEmpty(value.Serial));
                output.WriteLine($"Serial: {value.Serial}");
            }
        }

        [Fact]
        public void TestStatus()
        {
            using (var client = new PikoClient("192.168.0.43"))
            {
                var value = client.GetStatus(255);
                output.WriteLine($"Status: {value}");
            }
        }

        [Fact]
        public void TestDailyKwh()
        {
            using (var client = new PikoClient("192.168.0.43"))
            {
                double value = client.GetDailyKwh(255);
                Assert.True(value > 0);
                output.WriteLine($"Daily Energy: {value}");
            }
        }

        [Fact]
        public void TestTotalKwh()
        {
            using (var client = new PikoClient("192.168.0.43"))
            {
                double value = client.GetTotalKwh(255);
                Assert.True(value > 0);
                output.WriteLine($"Total Energy: {value}");
            }
        }
    }
}

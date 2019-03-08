using Xunit;

namespace CryptoDownloader.Tests
{
    public class FilePathServiceTest
    {
        [Fact]
        public void ShouldNotAlterNiceName()
        {
            FilePathService ds = new FilePathService();
            string dir = ds.GetTimeSeriesFilePath("/var/my/dummy/path", 3, "BTC_USD");
            Assert.Equal("/var/my/dummy/path/3/BTC_USD.csv", dir);
        }

        [Fact]
        public void ShouldAlterNameWithSpace()
        {
            FilePathService ds = new FilePathService();
            string dir = ds.GetTimeSeriesFilePath("/var/my/dummy/path", 128, "BTC USD");
            Assert.Equal("/var/my/dummy/path/128/BTC_USD.csv", dir);
        }
    }
}
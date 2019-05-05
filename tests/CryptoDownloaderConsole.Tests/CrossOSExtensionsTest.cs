using Xunit;

namespace CryptoDownloaderConsole.Tests
{
    public class CrossOSExtensionsTest
    {
        [Fact]
        public void ShouldSplitByNewLines()
        {
            string line = "abc";
            string manyLinesLinux = "abc\ndef";
            string manyLinesWindows = "abc\r\ndef";

            string[] lineSplit = line.SplitByNewLine();
            Assert.Single(lineSplit);
            Assert.Equal("abc", lineSplit[0]);
            
            string[] manyLinesLinuxSplit = manyLinesLinux.SplitByNewLine();
            Assert.Equal(2, manyLinesLinuxSplit.Length);
            Assert.Equal("abc", manyLinesLinuxSplit[0]);
            Assert.Equal("def", manyLinesLinuxSplit[1]);
            
            string[] manyLinesWindowsSplit = manyLinesWindows.SplitByNewLine();
            Assert.Equal(2, manyLinesWindowsSplit.Length);
            Assert.Equal("abc", manyLinesWindowsSplit[0]);
            Assert.Equal("def", manyLinesWindowsSplit[1]);
        }
    }
}
using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using XspfOpener.Classes;
using Windows.Storage;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Windows.System;

namespace XspfOpener.Test
{
    [TestClass]
    public class Comic
    {
        Xspf xspf;

        [TestInitialize]
        public async Task Load()
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///TestFiles/comic.xspf"));
            using (Stream stream = await file.OpenStreamForReadAsync())
                xspf = Xspf.Load(stream);
        }

        [TestMethod]
        public void TestMeta()
        {
            Assert.AreEqual(xspf.Title, "播放列表");
            Assert.AreEqual(xspf.Location, new Uri("http://comic.sjtu.edu.cn/"));
        }

        [TestMethod]
        public async Task TestTrackListFirst()
        {
            XspfTrack trk = xspf.TrackList.First();
            Assert.AreEqual(trk.Title, "组播:CCTV-⒈ 综合");

            string streamUriStr = Uri.EscapeDataString(trk.Location.ToString());
            await Launcher.LaunchUriAsync(
                new Uri(string.Format("vlc://openstream?from=url&url={0}", streamUriStr)));
        }

        [TestMethod]
        public async Task TestTrackListIndex()
        {
            XspfTrack trk = xspf.TrackList[1];
            Assert.AreEqual(trk.Title, "单播:CCTV-⒈ 综合");

            string streamUriStr = Uri.EscapeDataString(trk.Location.ToString());
            await Launcher.LaunchUriAsync(
                new Uri(string.Format("vlc://openstream?from=url&url={0}", streamUriStr)));
        }

        [TestMethod]
        public void AcquireURL()
        {
            foreach(XspfTrack track in xspf.TrackList)
            {
                Assert.IsNotNull(track.Location);
            }
        }
    }
}

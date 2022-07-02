using System;
using System.IO;
using WMPLib;

namespace SpeechToSignLanguage.Client.Functions.Video
{
    public class VideoFunciton
    {
        private readonly string path = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + "\\Videos\\";
        public string CreateVideoPath(string value)
        {
            string videopath = path + value + ".mp4";
            return videopath;
        }

        public bool CheckVideoInclude(string path)
        {
            if (File.Exists(path))
                return true;
            else
                return false;
        }

        public int Duration(String file)
        {
            WindowsMediaPlayer wmp = new WindowsMediaPlayerClass();
            IWMPMedia mediainfo = wmp.newMedia(file);
            return ((int)mediainfo.duration) * 1000 + 250;
        }
    }
}

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomSongPlayer
{
    internal static class FileLogHelper
    {
        internal static void Log(string text)
        {
            if (!File.Exists("randomLog.txt"))
                File.Create("randomLog.txt").Close();

            using (StreamWriter wr = new StreamWriter("randomLog.txt", true))
            {
                wr.WriteLine(text);
            }
        }
    }
}

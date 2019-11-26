using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomSongPlayer
{
    public class MapMetaData
    {
        public List<Characteristic> characteristics = null;
        public MapDifficulties difficulties = null;
        public string songName;
        public string songSubName;
        public string songAuthorName;
        public string levelAuthorName;
        public double bpm;

        public MapMetaData(List<Characteristic> characteristics, MapDifficulties difficulties, string songName, string songSubName, string songAuthorName, string levelAuthorName, double bpm)
        {
            this.characteristics = characteristics;
            this.difficulties = difficulties;
            this.songName = songName;
            this.songSubName = songSubName;
            this.songAuthorName = songAuthorName;
            this.levelAuthorName = levelAuthorName;
            this.bpm = bpm;
        }
    }
}

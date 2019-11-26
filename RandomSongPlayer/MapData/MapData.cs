using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomSongPlayer
{
    public class MapData
    {
        public MapMetaData metadata = null;
        public Stats stats = null;

        public string _id;
        public string key;

        public MapData(MapMetaData metadata, Stats stats, string _id, string key)
        {
            this.metadata = metadata;
            this.stats = stats;
            this._id = _id;
            this.key = key;
        }
    }
}

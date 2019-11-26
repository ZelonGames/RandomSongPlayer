using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomSongPlayer
{
    public class Document
    {
        public string _id;
        public string key;
        public Stats stats = null;

        public Document(string _id, string key, Stats stats)
        {
            this._id = _id;
            this.key = key;
            this.stats = stats;
        }
    }
}

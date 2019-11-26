using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomSongPlayer
{
    public class Stats
    {
        public int downloads;
        public int plays;
        public int downVotes;
        public int upVotes;
        public double rating;

        public Stats(int downloads, int plays, int downVotes, int upVotes, double rating)
        {
            this.downloads = downloads;
            this.plays = plays;
            this.downVotes = downVotes;
            this.upVotes = upVotes;
            this.rating = Math.Round(rating * 100, 2);
        }
    }
}

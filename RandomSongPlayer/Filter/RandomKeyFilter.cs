using RandomSongPlayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomSongPlayer.Filter
{
    internal class RandomKeyFilter
    {
        internal int? NPS { get; private set; }
        internal double? Rating { get; private set; }

        internal bool UsingFilters => NPS != null || Rating != null;

        internal RandomKeyFilter(int? nps, double? rating)
        {
            this.NPS = nps;
            this.Rating = rating;
        }

        internal bool MatchingFilters(Difficulty difficulty, MapData mapData)
        {
            bool npsCondition = NPS.HasValue ? MapTools.GetNotesPerSecond(mapData.metadata.bpm, difficulty.duration, difficulty.notes) >= NPS.Value : true;
            bool ratingCondition = Rating.HasValue ? mapData.stats.rating >= Rating.Value * 100 : true;

            return UsingFilters ? npsCondition && ratingCondition : true;
        }
    }
}

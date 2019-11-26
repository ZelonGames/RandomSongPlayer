using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomSongPlayer
{
    public class MapDifficulties
    {
        public bool easy = false;
        public bool normal = false;
        public bool hard = false;
        public bool expert = false;
        public bool expertPlus = false;

        public MapDifficulties(bool easy, bool normal, bool hard, bool expert, bool expertPlus)
        {
            this.easy = easy;
            this.normal = normal;
            this.hard = hard;
            this.expert = expert;
            this.expertPlus = expertPlus;
        }

        public List<string> GetDifficulties()
        {
            var difficulties = new List<string>();

            if (expertPlus)
                difficulties.Add("ExpertPlus");
            if (expert)
                difficulties.Add("Expert");
            if (hard)
                difficulties.Add("Hard");
            if (normal)
                difficulties.Add("Normal");
            if (easy)
                difficulties.Add("Easy");

            return difficulties;
        }
    }
}

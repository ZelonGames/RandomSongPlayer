using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomSongPlayer
{
    public class CharacteristicsDifficulty
    {
        public Difficulty easy = null;
        public Difficulty normal = null;
        public Difficulty hard = null;
        public Difficulty expert = null;
        public Difficulty expertPlus = null;

        public CharacteristicsDifficulty(Difficulty easy, Difficulty normal, Difficulty hard, Difficulty expert, Difficulty expertPlus)
        {
            this.easy = easy;
            this.normal = normal;
            this.hard = hard;
            this.expert = expert;
            this.expertPlus = expertPlus;
        }

        public Difficulty GetHighestDifficulty()
        {
            if (expertPlus != null)
                return expertPlus;
            else if (expert != null)
                return expert;
            else if (hard != null)
                return hard;
            else if (normal != null)
                return normal;
            else if (easy != null)
                return easy;
            else
                return null;
        }

        public Difficulty GetDifficulty(string difficulty)
        {
            difficulty = difficulty.ToLower();

            switch (difficulty)
            {
                case "easy":
                    return easy;
                case "normal":
                    return normal;
                case "hard":
                    return hard;
                case "expert":
                    return expert;
                case "expertplus":
                    return expertPlus;
                default:
                    break;
            }

            return null;
        }

        /// <summary>
        /// Gets a difficulty based on index where 0 is expert+ and 4 is easy
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Difficulty GetDifficulty(int index)
        {
            var difficulties = new Difficulty[] 
            {
                expertPlus,
                expert,
                hard,
                normal,
                easy
            };

            return difficulties[index];
        }
    }

    public class Difficulty
    {
        public double duration;
        public double length;
        public double njs;
        public int bombs;
        public int notes;
        public int obstacles;

        public Difficulty(double duration, double length, double njs, int bombs, int notes, int obstacles)
        {
            this.duration = duration;
            this.length = length;
            this.njs = njs;
            this.bombs = bombs;
            this.notes = notes;
            this.obstacles = obstacles;
        }
    }
}

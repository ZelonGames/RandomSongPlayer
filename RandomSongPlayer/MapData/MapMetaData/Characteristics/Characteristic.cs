using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomSongPlayer
{
    public class Characteristic
    {
        public string name;
        public CharacteristicsDifficulty difficulties = null;

        public Characteristic(string name, CharacteristicsDifficulty difficulties)
        {
            this.name = name;
            this.difficulties = difficulties;
        }
    }
}

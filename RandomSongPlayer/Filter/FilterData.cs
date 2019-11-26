using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomSongPlayer.Filter
{
    internal class FilterData
    {
        internal Dictionary<string, Dictionary<int, int>> filterData = null;

        internal FilterData(Dictionary<string, Dictionary<int, int>> filterData)
        {
            this.filterData = filterData;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomSongPlayer
{
    public class Latest
    {
        public List<Document> docs = null;
        public int lastPage;

        public Latest(List<Document> docs, int lastPage)
        {
            this.docs = docs;
            this.lastPage = lastPage;
        }
    }
}

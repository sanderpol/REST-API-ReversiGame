using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rest_API_ReversiGame.Entities.GameFolder
{
    public class Cordinate
    {
        public int Y { get; set; }
        public int X { get; set; }

        public Cordinate(int y, int x)
        {
            Y = y;
            X = x;
        }
    }
}

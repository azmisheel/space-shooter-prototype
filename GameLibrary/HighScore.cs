using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary
{
    public class HighScore
    {
        public string Initials { get; set; } = "---";
        public TimeSpan Time { get; set; }

        public HighScore() { } // <= add this

        public HighScore(string initials, TimeSpan time)
        {
            Initials = initials;
            Time = time;
        }
    }
}

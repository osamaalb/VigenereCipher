using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworksTP1
{
    public class RepeatedSequenceStatistics
    {
        public int occurences { get; set; }
        public int firstPosition { get; set; }
        public int secondPosition { get; set; }
        public RepeatedSequenceStatistics(int occurences, int firstPosition, int secondPosition)
        {
            this.occurences = occurences;
            this.firstPosition = firstPosition;
            this.secondPosition = secondPosition;
        }
    }
}

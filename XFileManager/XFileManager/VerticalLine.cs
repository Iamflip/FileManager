using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XFileManager
{
    class VerticalLine
    {
        public VerticalLine(int yTop, int yBot, int x, char sym)
        {
            for (int i = yTop; i < yBot; i++)
            {
                Point p = new Point(x, i, sym);
                p.Draw();
            }
        }
    }
}

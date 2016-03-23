using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Multinet.Math
{
    public class Cell
    {
        int x;
        int y;
        public Cell(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int X
        {
            get
            {
                return x;
            }
        }

        public int Y
        {
            get
            {
                return y;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj != null)
            {
                if (obj is Cell)
                {
                    return ((obj as Cell).x == x && (obj as Cell).y == y);
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hash = 7;
            hash = 97 * hash + this.x;
            hash = 97 * hash + this.y;
            return hash;
        }

    }
}

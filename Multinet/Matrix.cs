using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Multinet.Math
{
    public class Matrix
    {
        private Dictionary<Cell, double> weight;
        private int size;

        public Matrix(int s)
        {
            this.size = s;
            this.weight = new Dictionary<Cell, double>();
        }


        public int Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
            }
        }

        public double this[int x, int y]
        {
            get
            {
                Cell pos = new Cell(x, y);
                if (this.weight.ContainsKey(pos))
                {
                    return this.weight[pos];
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                this.weight.Add(new Cell(x, y), value);
            }
        }

    }
}

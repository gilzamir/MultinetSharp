using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Multinet.Net
{
    public class Synapse
    {
        private double intensity;
        private int from, to;

        public Synapse(int from = 0, int to = 0)
        {
            this.from = from;
            this.to = to;
        }

        public double Intensity
        {
            get
            {
                return intensity;
            }

            set
            {
                intensity = value;
            }
        }

        public int From
        {
            get
            {
                return from;
            }

            set
            {
                from = value;
            }
        }


        public int To
        {
            get
            {
                return to;
            }

            set
            {
                to = value;
            }
        }
    }
}

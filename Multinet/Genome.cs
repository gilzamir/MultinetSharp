using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multinet.Genetic
{
    public class Genome : Evaluable, IComparable
    {
        private Dictionary<uint, Chromossome> chromossomes;
        private double score;

        public Genome()
        {
            chromossomes = new Dictionary<uint, Chromossome>();
        }

        public double Value
        {
            get
            {
                return score;
            }
            set
            {
                score = value;
            }
        }

        public ICollection<uint> GetKeys()
        {
            return chromossomes.Keys;
        }

        public void AddChromossome(uint id, Chromossome chr)
        {
            chromossomes.Add(id, chr);
        }

        public Chromossome DelChromossome(uint id)
        {
            Chromossome chr = chromossomes[id];
            chromossomes.Remove(id);
            return chr;
        }

        public bool Contains(uint id)
        {
            return chromossomes.ContainsKey(id);
        }

        public Chromossome GetChromossome(uint id)
        {
            return chromossomes[id];
        }

        public int Size()
        {
            return chromossomes.Count();
        }

        override
        public string ToString()
        {
            string txt = "";

            foreach (KeyValuePair<uint, Chromossome> chr in this.chromossomes)
            {
                txt += string.Format("{0}:{1}\n", chr.Key, chr.Value);
            }
            return txt;
        }

        public int CompareTo(object obj)
        {
            if (obj is Genome)
            {
                Genome g = (Genome)obj;
                if (Value < g.Value)
                {
                    return -1;
                }
                else if (Value > g.Value)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }

            }
            return -1;
        }
    }
}

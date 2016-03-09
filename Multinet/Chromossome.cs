using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Multinet.Utils;

namespace Multinet.Genetic
{
    public class Chromossome
    {
        public static int GENE_SIZE = 32;

        private Dictionary<uint, BitArray> genes;

        public Chromossome()
        {
            genes = new Dictionary<uint, BitArray>();
        }

        public void AddGene(uint id, double value)
        {
            genes.Add(id, BitArrayUtils.ToBitArray(value));
        }

        public bool SetGene(uint id, BitArray gene)
        {
            genes[id] = gene;
            return true;
        }

        public int Size
        {
            get
            {
                return this.genes.Count();
            }
        }

        public void AddGene(uint id, uint value)
        {
            genes.Add(id, BitArrayUtils.ToBitArray(value));
        }

        public void AddGene(uint id, BitArray code)
        {
            genes.Add(id, code);
        }

        public BitArray GetGene(uint gid)
        {
            if (genes.ContainsKey(gid))
            {
                return (BitArray)genes[gid];
            }
            else
            {
                return null;
            }
        }

        public ICollection<uint> GetKeys()
        {
            return genes.Keys;
        }

        public BitArray DelGene(uint id)
        {
            BitArray ret = GetGene(id);
            genes.Remove(id);
            return ret;
        }

        override
        public string ToString()
        {
            string txt = "";

            foreach (KeyValuePair<uint, BitArray> gene in genes)
            {

                txt += string.Format("{0}:{1} ", gene.Key, BitArrayUtils.ToNDouble(gene.Value));
            }

            return txt;
        }

    }
}

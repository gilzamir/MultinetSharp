using System;
using System.Collections.Generic;
using System.Collections;
using Multinet.Utils;

namespace Multinet.Genetic
{
    public sealed class CrossoverImpl
    {
        public static int seed = -1;
        public static int last_seed = -1;
        public static Genome CrossoverMethod(Genome gen1, Genome gen2)
        {
            if (seed < 0)
            {
                seed = unchecked(DateTime.Now.Ticks.GetHashCode());
            }
            last_seed = seed;
            Random rnd = new Random();
            Genome child = new Genome();

            ICollection<uint> ids = gen1.GetKeys();
            foreach (uint id in ids)
            {
                Chromossome chr1 = gen1.GetChromossome(id);

                if (!gen2.Contains(id))
                {
                    throw new IncompatibleGenomeException();
                }

                Chromossome chr2 = gen2.GetChromossome(id);

                int m = System.Math.Min(chr1.Size, chr2.Size);
                int m2 = chr2.Size;

                int    point = (int)rnd.Next(m);
                
               // Console.WriteLine("POINT: {0}", point);
                Chromossome nchr = new Chromossome();

                
                for (int i = 0; i < point; i++)
                {
                    BitArray copy = BitArrayUtils.DeepClone(chr1.GetGene((uint)i));
                    nchr.AddGene((uint)i, copy);
                }

                for (int j = point; j < m2; j++)
                {
                    BitArray copy = BitArrayUtils.DeepClone(chr2.GetGene((uint)j));
                    nchr.AddGene((uint)j, copy);
                }

                child.AddChromossome(id, nchr);
            }
            seed = -1;
            return child;
        }
    }
}

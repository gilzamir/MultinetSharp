using System;
using System.Collections.Generic;
using System.Collections;
using Multinet.Utils;

namespace Multinet.Genetic
{
    public sealed class CrossoverImpl
    {
        public static Genome CrossoverMethod(Genome gen1, Genome gen2)
        {
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

                ICollection<uint> genesKeys = chr1.GetKeys();
                
                int m = System.Math.Min(chr1.Size, chr2.Size);

                int    point = (int)rnd.Next(m);
                
               // Console.WriteLine("POINT: {0}", point);
                Chromossome nchr = new Chromossome();

                for (int i = 0; i < point; i++)
                {
                    BitArray copy = BitArrayUtils.DeepClone(chr1.GetGene((uint)i));
                    nchr.AddGene((uint)i, copy);
                }

                for (int i = point; i < chr2.Size; i++)
                {
                    BitArray copy = BitArrayUtils.DeepClone(chr2.GetGene((uint)i));
                    nchr.AddGene((uint)i, copy);
                }

                child.AddChromossome(id, nchr);
            }
            return child;
        }
    }
}

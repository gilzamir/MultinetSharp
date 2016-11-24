using System.Collections.Generic;
using System.Collections;

namespace Multinet.Genetic
{
    public sealed class MutationImpl
    {
        public static void MutationMethod(Genome gen, double chance)
        {
            int c = 0;
            int cm = 0;
            ICollection<uint> ids = gen.GetKeys();
            foreach (uint id in ids)
            {
                Chromossome chr = gen.GetChromossome(id);
                ICollection<uint> genesKeys = chr.GetKeys();

                foreach (uint gid in genesKeys)
                {
                    BitArray gene = chr.GetGene(gid);
                    
                    for (int i = 0; i < Chromossome.GENE_SIZE; i++)
                    {
                        double selected = Multinet.Math.PRNG.NextDouble();
                        if (selected <= chance)
                        {
                            gene.Set(i, !gene.Get(i));
                        }
                    }
                }
            }
        }
    }
}

using System;
using System.Collections;
using Multinet.Net;
using Multinet.Math;
using Multinet.Genetic;
using Multinet.Utils;
using Multinet.Sample;

class OneNet : Net
{
    public Neuron neuron;
    public Synapse w = new Synapse();

    override
    public int Size
    {
        get
        {
            return 1;
        }
    }

    override
    public Neuron this[int id]
    {
        get
        {
            return neuron;
        }

        set
        {
            neuron = value;
        }
    }

    override
    public Synapse this[int i, int j]
    {
        get
        {
            return w;
        }
    }

    override
    public double GetDouble(string var)
    {
        throw new NotImplementedException();
    }

    override
    public void SetDouble(string var, double v)
    {
        throw new NotImplementedException();
    }
}


namespace Multinet.Test
{
    class Test
    {
        
        public static void TestGenetic()
        {
            XORProblem problem = new XORProblem();
            problem.Run();   
        }


        public static void testBitArrayUtils()
        {
            BitArray array = BitArrayUtils.ToBitArray(1);
            Console.WriteLine("LENGTH: {0}", array.Length);
            Console.WriteLine("BITS: ");
            Console.WriteLine(array.ToString());
            for (int i = array.Length-1; i >= 0; i--)
            {
                Console.Write( (array.Get(i) ? "1": "0") );
            }
            Console.WriteLine();

            array = BitArrayUtils.ToBitArray(uint.MaxValue);
            Console.WriteLine("LENGTH: {0}", array.Length);
            Console.WriteLine("BITS: ");
            Console.WriteLine(array.ToString());
            for (int i = array.Length - 1; i >= 0; i--)
            {
                Console.Write((array.Get(i) ? "1" : "0"));
            }
            Console.WriteLine();

            double code = BitArrayUtils.ToNDouble(array);
            System.Console.WriteLine("BITARRAY(uint32.MaxValue) = {0}", code);
        }

        public static void testMutation()
        {
            Genome g1 = new Genome();

            Chromossome ch1 = new Chromossome();
            ch1.AddGene(0, BitArrayUtils.ToBitArray(0));
            ch1.AddGene(1, BitArrayUtils.ToBitArray(1));
            ch1.AddGene(2, BitArrayUtils.ToBitArray(2));
            ch1.AddGene(3, BitArrayUtils.ToBitArray(3));

            g1.AddChromossome(0, ch1);

            MutationImpl.MutationMethod(g1, 1.0);

            Chromossome ch = g1.GetChromossome(0);
            for (int i = 0; i < ch.Size; i++)
            {
                BitArray g = ch.GetGene((uint)i);
                System.Console.Write("{0} ", BitArrayUtils.ToUInt32(g));
            }
            System.Console.WriteLine();

        }


        public static void testCrossover()
        {
            Genome g1 = new Genome();
            Genome g2 = new Genome();

            Chromossome ch1 = new Chromossome();
            ch1.AddGene(0, BitArrayUtils.ToBitArray(0));
            ch1.AddGene(1, BitArrayUtils.ToBitArray(1));
            ch1.AddGene(2, BitArrayUtils.ToBitArray(2));
            ch1.AddGene(3, BitArrayUtils.ToBitArray(3));

            Chromossome ch2 = new Chromossome();
            ch2.AddGene(0, BitArrayUtils.ToBitArray(4));
            ch2.AddGene(1, BitArrayUtils.ToBitArray(5));
            ch2.AddGene(2, BitArrayUtils.ToBitArray(6));
            ch2.AddGene(3, BitArrayUtils.ToBitArray(7));

            g1.AddChromossome(0, ch1);
            g2.AddChromossome(0, ch2);
            
            Genome gen = CrossoverImpl.CrossoverMethod(g1, g2);

            Chromossome ch = gen.GetChromossome(0);
            for (int i = 0; i < ch.Size; i++)
            {
                BitArray g = ch.GetGene((uint)i);
                System.Console.Write("{0} ", BitArrayUtils.ToUInt32(g));
            }
            System.Console.WriteLine();
        }

        public static void testBeer1995Neuron()
        {
            NeuralNet net = new NeuralNet();

            int n1 = net.CreateNeuron();
            int n2 = net.CreateNeuron();
            int n3 = net.CreateNeuron();
            int n4 = net.CreateNeuron();
            int n5 = net.CreateNeuron();

            Neuron ne1 = net[n1];
            Neuron ne2 = net[n2];
            Neuron ne3 = net[n3];
            Neuron ne4 = net[n4];
            Neuron ne5 = net[n5];

            ne1.Implementation = new Beer1995Neuron();
            ne2.Implementation = new Beer1995Neuron();
            ne3.Implementation = new Beer1995Neuron();
            ne4.Implementation = new Beer1995Neuron();
            ne5.Implementation = new Beer1995Neuron();
            
            net.CreateSynapse(n1, n3, 930);
            net.CreateSynapse(n2, n3, 930);
            net.CreateSynapse(n1, n4, 930);
            net.CreateSynapse(n2, n4, 930);
            net.CreateSynapse(n3, n5, 1.0);
            net.CreateSynapse(n4, n5, 1.0);

            ne1.Implementation["inputgain"] = 1000.0;
            ne1.Implementation["outputgain"] = 1.0;
            ne1.Implementation["inputweight"] = 0.0;
            ne1.Implementation["sensorweight"] = 1.0;
            ne1.TimeConst = 1.0;
            
            ne2.Implementation["inputgain"] = 1000.0;
            ne2.Implementation["outputgain"] = 1.0;
            ne2.Implementation["inputweight"] = 0.0;
            ne2.Implementation["sensorweight"] = 1.0;
            ne2.TimeConst = 1.0;
            
            ne3.Implementation["inputgain"] = 1.0;
            ne3.Implementation["outputgain"] = 1.0;
            ne3.Implementation["inputweight"] = -1.0;
            ne3.Implementation["sensorweight"] = 0.0;
            ne3.Implementation["bias"] = 100.0;
            ne3.TimeConst = 1.0;

            ne4.Implementation["inputgain"] = 1.0;
            ne4.Implementation["outputgain"] = 1.0;
            ne4.Implementation["inputweight"] = 1.0;
            ne4.Implementation["sensorweight"] = 0.0;
            ne4.Implementation["bias"] = 100.0;
            ne4.TimeConst = 1.0;

            ne5.Implementation["inputgain"] = 1.0;
            ne5.Implementation["outputgain"] = 1.0;
            ne5.Implementation["inputweight"] = 1.0;
            ne5.Implementation["sensorweight"] = 0.0;
            ne5.Implementation["bias"] = 0;
            ne5.TimeConst = 1.0;

            
            ne1.Implementation.SetInput(ne1, 0.0);
            ne2.Implementation.SetInput(ne2, 0.0);
            //Console.WriteLine(ne1.State);
            net.Proccess();
            //Console.WriteLine(ne1.State);
            double v = ne3.GetOutput();
            Console.WriteLine("OUTPUT = {0}", v);
        }

        public static void testHomeoestaticNet()
        {
            Neuron n = new Neuron(0);
            n.TimeConst = 10;
            n.Implementation["bias"] = 0;
            n.Implementation["inputgain"] = 1.0;
            n.Implementation["outputgain"] = 1.0;
            n.Implementation["inputweight"] = 1.0;
            n.Implementation["sensorweight"] = 1.0;
            n.Implementation["weightgain"] = 20;
            OneNet net = new OneNet();
            net.neuron = n;
            net[0, 0].Intensity = 0.2;
            n.Network = net;
            n.Incame.AddLast(new Cell(0, 0));
            n.Outcame.AddLast(new Cell(0, 0));
            n.ProccessInput(2.0);
            bool stopped = false;
            const double min = 0.4;
            const double max = 0.5;


            Console.WriteLine("********************************HELP**********************************");
            Console.WriteLine("* This software lets you to control a single neuron!!!\t*");
            Console.WriteLine("* Press C key to supply 2.0v to neuron input.\t*");
            Console.WriteLine("* Press ESC to exit!\t*");
            Console.WriteLine("* Press any other key to supply 0.0v to neuron input.\t*");
            Console.WriteLine("* ---------------------------------------------------\t*");
            Console.WriteLine("HUD shows: ");
            Console.WriteLine("Plast({0}): where {0} is a plasticity level of neuron.");
            Console.WriteLine("state({0}) : pot({1}) : w({2}), where: ");
            Console.WriteLine("{0} is neuron state; {1} is neuron frequency potential; and {2} is a auto connection weight of the neuron.");
            Console.WriteLine("* ---------------------------------------------------\t*");

            while (!stopped)
            {

                double state = n.Proccess();
                double pot = n.Implementation.GetPotential(n);
                double p = 0.0;
                if (pot < min)
                {
                    p = 1 - pot / min;

                }
                else if (pot > max)
                {
                    p = 1 - pot / max;
                }

                net.w.Intensity += 0.8 * p * (1 - pot);

                Console.WriteLine("plast({0})", p);
                Console.WriteLine("state({0}) : pot({1}) : w({2})", state, pot, net.w.Intensity);
                Console.WriteLine("Press ESC to exit; C to supply 2.0v to neuron; or other key to supply 0.0v to neuron...");
                ConsoleKeyInfo info = Console.ReadKey();
                if (info.Key == ConsoleKey.Escape)
                {
                    stopped = true;
                }
                else if (info.Key == ConsoleKey.C)
                {
                    n.ProccessInput(2.0);
                }
            }
        }
    }
}

/**
* This file contains basic implementation of genetic algorithm.
* For more informations, contact me: gilzamir@outlook.com
*/
using System;

namespace Multinet.Genetic
{
    public class GenomeBuilderNotDefinedException : Exception {}
    public class GenomeTranslatorNotDefinedException : Exception {}
    public class IncompatibleGenomeException: Exception {}

    public delegate void MutationMethod(Genome gen, double chance);
    public delegate Genome CrossoverMethod(Genome g1, Genome g2);
    public delegate Genome GenomeBuilderMethod();
    public delegate Evaluable GenomeTranslatorMethod(Genome gen);
}

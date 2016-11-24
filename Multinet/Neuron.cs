/**
* This file contains basic implementation of a neuralnet with homeostatic properties.
* For more informations, contact me: gilzamir@gmail.com.
*/
using System;
using System.Collections.Generic;
using Multinet.Math;


namespace Multinet.Net
{

   /// <summary>
   /// This class encapsulates the key features of the neural nodes.
   /// </summary>
   public class Neuron
    {
        private Net net;
        private int Id;
        private double state;
        private double sensorValue;
        private double timeConstant;
        private double duration;
        private INeuronImpl neuronImpl;
        private LinkedList<Cell> income;
        private LinkedList<Cell> outcame;
        
        public Neuron(int id)
        {
            this.income = new LinkedList<Cell>();
            this.outcame = new LinkedList<Cell>();
            this.state = 0.0;
            this.Id = id;
            this.sensorValue = 0.0;
            this.timeConstant = 1.0;
            this.duration = 1.0;
            this.neuronImpl = new Multinet.Net.Impl.Beer1995Neuron();
        }
        
        public double Duration
        {
            get
            {
                return duration;
            }

            set
            {
                duration = value;
            }
        }

        public LinkedList<Cell> Incame
        {
            get
            {
                return income;
            }
        }

        public LinkedList<Cell> Outcame
        {
            get
            {
                return outcame;
            }
        }
        
        public Net Network
        {
            get
            {
                return net;
            }

            set
            {
                net = value;
            }
        }

        public int ID
        {
            get
            {
                return Id;
            }

            set
            {
                Id = value;
            }
        }
        
        public double State
        {
            get
            {
                return state;
            }
            set
            {
                state = value;
            }
        }

        public double SensorValue
        {
            get
            {
                return sensorValue;
            }
            set
            {
                sensorValue = value;
            }
        }
        
        public double TimeConst
        {
            get
            {
                return timeConstant;
            }
            set
            {
                timeConstant = value;
            }
        }
        
        public double ProccessInput(double input)
        {
            return this.Implementation.SetInput(this, input);
        }

        public double GetOutput()
        {
            return this.Implementation.GetOutput(this);
        }

        public double Proccess()
        {
            if (Implementation.UseNumericalMethod)
            {
                return (state = net.NumericalMethod.nextState(duration, state, onestep));
            } else
            {
                return (state = onestep(state));
            }    
        }

        public INeuronImpl Implementation
        {
            get
            {
                return neuronImpl;
            }
            set
            {
                neuronImpl = value;
            }
        }


        private double onestep(double state)
        {
            return neuronImpl.Step(this, net, state);
        }
    }
} //namespace end

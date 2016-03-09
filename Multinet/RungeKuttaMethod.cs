﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Multinet.Math.Impl
{
    public class RungeKuttaMethod : INumericalMethod
    {
        private Dictionary<string, double> vars;

        public RungeKuttaMethod()
        {
            vars = new Dictionary<string, double>();
            vars["step"] = 0.02;
        }

        public double this[string name]
        {
            get
            {
                return vars[name];
            }
            set
            {
                vars[name] = value;
            }
        }

        public double nextState(double state, TargetFunction machine)
        {
            //4th Order Runge-Kutta ====================================================
            double k1, k2, k3, k4;
            double step = this["step"];
            double pState = state;

            k1 = machine(state);
            state = pState + k1 * step / 2.0;

            k2 = machine(state);
            state = pState + k2 * step / 2.0;

            k3 = machine(state);
            state = pState + k3 * step;

            k4 = machine(state);
            state = pState + (k1 + 2 * k2 + 2 * k3 + k4) * step / 6.0;
            if (state > 100)
            {
                state = 100;
            }
            else if (state < -100)
            {
                state = -100;
            }
            return state;
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multinet.Genetic
{
    public interface Evaluable
    {
        double Value
        {
            get;
            set;
        }
    }
}

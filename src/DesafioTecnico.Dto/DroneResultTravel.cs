// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesafioTecnico.Dto
{
    public class DroneResultTravel
    {
        public string startingPoint { get; set; } = string.Empty;
        public string objLocation { get; set; } = string.Empty;
        public string finalDestination { get; set; } = string.Empty;
        public string routeTraveled { get; set; } = string.Empty;

        public double elapsedTime { get; set; }

    }

   
}

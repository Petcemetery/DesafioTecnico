// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesafioTecnico.Domain.Entities
{
    [Table("DroneTravel")]
    public class DroneTravel : BaseEntity<long>
    {
        public string startingPosition { get; set; }
        public string objPosition { get; set; }
        public string finalPosition { get; set; }
        public double elapsedTime { get; set; }
        public string route { get; set; }
        public DateTime date { get; set; }
    }
}

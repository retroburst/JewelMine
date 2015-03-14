using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelMine.Engine.Models
{
    /// <summary>
    /// Represents a jewel movement.
    /// </summary>
    public class JewelMovement
    {
        public Jewel Jewel { get; set; }
        public Coordinates Original { get; set; }
        public Coordinates New { get; set; }
    }
}

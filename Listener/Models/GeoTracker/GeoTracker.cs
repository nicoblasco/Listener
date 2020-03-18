using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Listener.Models.GeoTracker
{
    public class GeoTracker
    {
        public int Id { get; set; }
        public string Identifier { get; set; }
        public string GpsModel { get; set; }
        public bool Enabled { get; set; }
        public DateTime CreationDate { get; set; }
    }
}

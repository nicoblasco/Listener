using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Listener.Models.Georeference
{
    public class CreateGeoReference
    {
        public string Identifier { get; set; }
        public Int64 TrackerLogId { get; set; }
        public int GeoTrackerId { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}

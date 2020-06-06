using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCSCarousel.Classes
{
    public class FilterValues
    {
        public Boolean Active { get; set; }
        public EnvironmentState environmentState { get; set; }
        public PlatformReleaseInformation platformReleaseInformation { get; set; }

        public ReleaseInformation releaseInformation { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCSCarousel.Classes
{
    public class PlatformReleaseInformation : IComparable
    {
        public string PlatformRelease { get; set; }

        public int CompareTo(object obj)
        {
            if (obj is null)
            {
                throw new Exception(Properties.Resources.CompareObjIsNull);
            }

            var platformReleaseInformation = obj as PlatformReleaseInformation;
            if (platformReleaseInformation is null)
            {
                throw new Exception(Properties.Resources.CompareObjIsNull);
            }

#pragma warning disable CA1307 // Specify StringComparison
            return PlatformRelease.CompareTo(platformReleaseInformation.PlatformRelease);
#pragma warning restore CA1307 // Specify StringComparison
        }
        public override string ToString()
        {
            return PlatformRelease;
        }
    }
}

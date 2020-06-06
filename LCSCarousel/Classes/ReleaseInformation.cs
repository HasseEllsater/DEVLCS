using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCSCarousel.Classes
{
    public class ReleaseInformation : IComparable
    {
        public string Release { get; set; }
        public int CompareTo(object obj)
        {
            if (obj is null)
            {
                throw new Exception(Properties.Resources.CompareObjIsNull);
            }

            var releaseInformation = obj as ReleaseInformation;
            if (releaseInformation is null)
            {
                throw new Exception(Properties.Resources.CompareObjIsNull);
            }

#pragma warning disable CA1307 // Specify StringComparison
            return Release.CompareTo(releaseInformation.Release);
#pragma warning restore CA1307 // Specify StringComparison
        }

        public override string ToString()
        {
            return Release;
        }
    }
}

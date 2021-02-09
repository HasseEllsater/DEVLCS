using LCSCarousel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCSCarousel.Classes
{
    public class EnvironmentState : IComparable
    {
        public DeploymentState StateNum { get; set; }
        public string StateDescription { get; set; }

        public int CompareTo(object obj)
        {
            if (obj is null)
            {
                throw new Exception(Properties.Resources.CompareObjIsNull);
            }

            var environmentState = obj as EnvironmentState;
            if (environmentState is null)
            {
                throw new Exception(Properties.Resources.CompareObjIsNull);
            }

#pragma warning disable CA1307 // Specify StringComparison
            return StateDescription.CompareTo(environmentState.StateDescription);
#pragma warning restore CA1307 // Specify StringComparison
        }

        public override string ToString()
        {
            return StateDescription;
        }
    }
}

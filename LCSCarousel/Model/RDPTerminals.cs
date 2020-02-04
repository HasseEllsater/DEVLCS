using LCSCarousel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCSCarousel.Model
{
    public class RDPTerminal : IComparable
    {
        public string InstanceId { get; set; }
        public string DeploymentStatus { get; set; }
        public string ImageSource { get; set; }
        public string ApplicationRelease { get; set; }
        public string CurrentPlatformReleaseName { get; set; }
        public string TopologyType { get; set; }
        public string DisplayName { get; set; }
        public string EnvironmentId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "<Pending>")]
        public Instance[] Instances { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "<Pending>")]
        public Credentials[] SqlAzureCredentials { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "<Pending>")]
        public Navigationlink[] NavigationLinks { get; set; }

        public int CompareTo(object obj)
        {
            if (obj is null)
            {
                throw new Exception(Properties.Resources.CompareObjIsNull);
            }

            var rdpTerminal = obj as RDPTerminal;
            if (rdpTerminal is null)
            {
                throw new Exception(Properties.Resources.ObjIsNotRDPTerminal);
            }
#pragma warning disable CA1307 // Specify StringComparison
            return InstanceId.CompareTo(rdpTerminal.InstanceId);
#pragma warning restore CA1307 // Specify StringComparison
        }
        public override string ToString()
        {
            return InstanceId;
        }
    }
}

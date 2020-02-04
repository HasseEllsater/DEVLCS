using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCSCarousel.Model
{
    public class UserCredentials : IComparable
    {
        public string UserId { get; set; }
        public string Password { get; set; }
        public int CompareTo(object obj)
        {
            if (obj is null)
            {
                throw new Exception(Properties.Resources.CompareObjIsNull);
            }

            var credentials = obj as UserCredentials;
            if (credentials is null)
            {
                throw new Exception(Properties.Resources.ObjIsNotRDPTerminal);
            }

#pragma warning disable CA1307 // Specify StringComparison
            return UserId.CompareTo(credentials.UserId);
#pragma warning restore CA1307 // Specify StringComparison
        }
        public override string ToString()
        {
            return UserId;
        }
    }
}

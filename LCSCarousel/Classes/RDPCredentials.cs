using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LCSCarousel.Classes
{
    internal class RdpCredentials : IDisposable
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public RdpCredentials(string host, string userName, string password)
        {
            Host = host;
            var cmdkey = new Process
            {
                StartInfo =
                {
                    FileName = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\system32\cmdkey.exe"),
                    Arguments = $@"/generic:TERMSRV/{host} /user:{userName} /pass:{password}",
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };
            cmdkey.Start();
        }

        private string Host { get; }

        public void Dispose()
        {
            if (Host != null)
            {
                var task = new Thread(DeleteEntry);
                task.Start();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        private void DeleteEntry()
        {
            Thread.Sleep(10000);//Give it time before deleting credentials
            var cmdkey = new Process
            {
                StartInfo =
                {
                    FileName = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\system32\cmdkey.exe"),
                    Arguments = $@"/delete:TERMSRV/{Host}",
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };
            cmdkey.Start();
        }
    }
}

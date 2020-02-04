using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LCSCarousel.Mvvm;


namespace LCSCarousel.ViewModels
{
    public class RDPConnectionDetailsViewModel : BindableBase
    {
        private System.Collections.ObjectModel.ObservableCollection<RDPConnectionDetails> _rdpConnectionDetailsList;
        private LCSCarousel.RDPConnectionDetails _rdpConnectionDetails;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>")]
        public System.Collections.ObjectModel.ObservableCollection<RDPConnectionDetails> RDPConnectionDetailsList
        {
            get { return this._rdpConnectionDetailsList; }
            set { this.SetProperty(ref this._rdpConnectionDetailsList, value); }
        }
        public RDPConnectionDetails RDPConnectionDetails
        {
            get { return this._rdpConnectionDetails; }
            set { this.SetProperty(ref this._rdpConnectionDetails, value); }
        }
        public RDPConnectionDetailsViewModel(List<RDPConnectionDetails> rdpList)
        {
            if(rdpList == null)
            {
                throw new Exception(Properties.Resources.CredentialsListIsNull);
            }

            _rdpConnectionDetailsList = new System.Collections.ObjectModel.ObservableCollection<RDPConnectionDetails>();

            foreach (var userCredential in rdpList)
            {
                _rdpConnectionDetailsList.Add(userCredential);
            }
        }
    }
}

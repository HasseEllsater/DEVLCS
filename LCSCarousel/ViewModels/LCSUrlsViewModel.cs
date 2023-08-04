using LCSCarousel.Model;
using LCSCarousel.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCSCarousel.ViewModels
{
    internal class LCSUrlsViewModel : BindableBase
    {
        public ObservableCollection<LCSUrl> LCSUrls { get; set; }   
        public LCSUrl SelectedItem { get; set; }

        public LCSUrlsViewModel()
        {
            LCSUrls = new ObservableCollection<LCSUrl>();
            LoadUrlsFromSettings();
        }
        public void LoadUrlsFromSettings()
        {
            if(string.IsNullOrEmpty(Properties.Settings.Default.SerializedLCSUrls))
            {
                ResetUrls();  
                //Select region united states as default
                SelectedItem = LCSUrls.Where(x => x.Region == "United States").FirstOrDefault();
            }
            else
            {
                LCSUrls = Newtonsoft.Json.JsonConvert.DeserializeObject<ObservableCollection<LCSUrl>>(Properties.Settings.Default.SerializedLCSUrls);
                if(string.IsNullOrEmpty(Properties.Settings.Default.SelectedRegion))
                {
                    Properties.Settings.Default.SelectedRegion = "United States";
                    Properties.Settings.Default.Save();
                }   

                SelectedItem = LCSUrls.Where(x => x.Region == Properties.Settings.Default.SelectedRegion).FirstOrDefault();
            }

        }
        public void SetSelectedRegion()
        {
            Properties.Settings.Default.SelectedRegion = SelectedItem.Region;
            Properties.Settings.Default.Save();
        }
        public bool ValidateURL(string url)
        {
            bool retVal = false;    
            //Check the LCSUrls to see if there is url equal with the url passed in
            var lcsUrl = LCSUrls.Where(x => x.Url + "/v2" == url).FirstOrDefault();
            if(lcsUrl != null)   
            {
                retVal = true;
            }
            return retVal;
        }
        public void SaveUrlsToSettings()
        {
            //serialize the LCSUrls to the Properties.Settings.Default.SerializedLCSUrls
            Properties.Settings.Default.SerializedLCSUrls = Newtonsoft.Json.JsonConvert.SerializeObject(LCSUrls);
            Properties.Settings.Default.Save();
        }
        public void ResetUrls()
        {
            LCSUrls = new ObservableCollection<LCSUrl>();

            LCSUrl lCSUrl = new LCSUrl
            {
                Region = "United States",
                Url = "https://lcs.dynamics.com",
                DiagnosticUrl = "https://diag.lcs.dynamics.com",
                UpdateUrl = "https://update.lcs.dynamics.com",
                IssueSearchUrl = "https://fix.lcs.dynamics.com",
            };
            LCSUrls.Add(lCSUrl);

            lCSUrl = new LCSUrl
            {
                Region = "Europe",
                Url = "https://eu.lcs.dynamics.com",
                DiagnosticUrl = "https://diag.eu.lcs.dynamics.com",
                UpdateUrl = "https://update.eu.lcs.dynamics.com",
                IssueSearchUrl = "https://fix.eu.lcs.dynamics.com"
            };
            LCSUrls.Add(lCSUrl);

            //Add for fr
            lCSUrl = new LCSUrl
            {
                Region = "France",
                Url = "https://fr.lcs.dynamics.com",
                DiagnosticUrl = "https://diag.fr.lcs.dynamics.com",
                UpdateUrl = "https://update.fr.lcs.dynamics.com",
                IssueSearchUrl = "https://fix.fr.lcs.dynamics.com"
            };
            LCSUrls.Add(lCSUrl);

            lCSUrl = new LCSUrl
            {
                Region = "United Arab Emirates",
                Url = "https://uae.lcs.dynamics.com",
                DiagnosticUrl = "https://diag.uae.lcs.dynamics.com",
                UpdateUrl = "https://update.uae.lcs.dynamics.com",
                IssueSearchUrl = "https://fix.uae.lcs.dynamics.com"
            };
            LCSUrls.Add(lCSUrl);

            lCSUrl = new LCSUrl
            {
                Region = "Switzerland",
                Url = "https://ch.lcs.dynamics.com",
                DiagnosticUrl = "https://diag.ch.lcs.dynamics.com",
                UpdateUrl = "https://update.ch.lcs.dynamics.com",
                IssueSearchUrl = "https://fix.ch.lcs.dynamics.com"
            };
            LCSUrls.Add(lCSUrl);

            lCSUrl = new LCSUrl
            {
                Region = "Norway",
                Url = "https://no.lcs.dynamics.com",
                DiagnosticUrl = "https://diag.no.lcs.dynamics.com",
                UpdateUrl = "https://update.no.lcs.dynamics.com",
                IssueSearchUrl = "https://fix.no.lcs.dynamics.com"
            };
            LCSUrls.Add(lCSUrl);

            SaveUrlsToSettings();

        }
    }
}

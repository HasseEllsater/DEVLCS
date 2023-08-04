using LCSCarousel.Model;
using LCSCarousel.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCSCarousel.Classes
{
    internal sealed class URLsSingleton
    {
        private static readonly Lazy<URLsSingleton> lazy = new Lazy<URLsSingleton>(() => new URLsSingleton());
        public static URLsSingleton Instance => lazy.Value;
        public LCSUrl LcsRegion { get; set; }
        private LCSUrlsViewModel _lcsUrlsViewModel;
        private URLsSingleton() 
        {
            _lcsUrlsViewModel = new LCSUrlsViewModel();
            LcsRegion = _lcsUrlsViewModel.SelectedItem;
        }
        public bool ValidateURL(string url)
        {
            return _lcsUrlsViewModel.ValidateURL(url);
        }
    }
}

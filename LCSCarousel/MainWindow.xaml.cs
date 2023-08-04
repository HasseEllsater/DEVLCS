using System;
using System.Windows;
using System.Windows.Navigation;
using MahApps.Metro.Controls;
using LCSCarousel.ViewModels;
using MenuItem = LCSCarousel.ViewModels.MenuItem;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Globalization;
using LCSCarousel.Views;
using Newtonsoft.Json;
using System.Linq;
using LCSCarousel.Classes;
using System.Diagnostics;
using LCSCarousel.Model;
using System.Windows.Input;
using System.Configuration;
using LCSCarousel.Enums;
using IKriv.Threading.Tasks;

namespace LCSCarousel
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1060:Move pinvokes to native methods class", Justification = "<Pending>")]
    public partial class MainWindow : MetroWindow, IDisposable
    {
        #region members and properties
        private static readonly string _lcsDiagUrl = URLsSingleton.Instance.LcsRegion.DiagnosticUrl;//"https://diag.lcs.dynamics.com";
        private static readonly string _lcsUpdateUrl = URLsSingleton.Instance.LcsRegion.UpdateUrl;//"https://update.lcs.dynamics.com";
        private ViewModels.MenuItem MyCloudHostedMenuItem { get; set; }
        private ViewModels.MenuItem MyMSHostedMenuItem { get; set; }
        private Uri LoggedInUri;
        private ViewModels.MenuItem MySettingsMenuItem { get; set; }
        public static readonly string _lcsUrl = URLsSingleton.Instance.LcsRegion.Url; //"https://eu.lcs.dynamics.com";
        private const int InternetCookieHttponly = 0x2000;
        private bool _disposed;
        private HttpClientHelper httpClientHelper;
        private LcsProject SelectedProject;
        private List<ProjectInstance> Instances;
        private List<LcsProject> Projects;
        private List<CloudHostedInstance> SaasInstancesList;
        private List<CloudHostedInstance> CloudHostedInstancesList;
        private List<EnvironmentState> EnvironmentStatesList;
        private List<PlatformReleaseInformation> PlatformReleaseInformationList;
        private List<ReleaseInformation> ReleaseInformationList;
        private FilterValues filterValues,microsoftHostedFilterValues;

        List<RDPConnectionDetailsCache> UserCredentialsCloud = new List<RDPConnectionDetailsCache>();
        List<RDPConnectionDetailsCache> UserCredentialsSAAS= new List<RDPConnectionDetailsCache>();

        public ProjectType MSHostedProjectType { get; private set; }
        private List<DeployablePackage> Packages = new List<DeployablePackage>();
        public string Logouttime { get; set; }
        private List<InstanceAttribute> InstanceAttributeList;
        private List<CloudHostedInstance> AllVMsList;
        private bool ClearVMStatus { get; set; }
        private bool ClearSaaSStatus { get; set; }
        private bool ClearCloudHostStatus { get; set; }
        private bool GetPackagesStatus { get; set; }
        private bool RefreshCredentialsCloudStatus { get; set; }
        private bool RefreshCredentialsMSHostedStatus { get; set; }

        public string SelectedProjectName
        {
            get
            {
                if(SelectedProject == null)
                {
                    return Properties.Resources.NoProjectSelected;
                }
                return string.Format(Properties.Resources.CurrentProjectName,SelectedProject.Name); 
            }
        }
        public List<CloudHostedInstance> CloudHosted
        {
            get
            {
                return CloudHostedInstancesList;
            }
        }
        public List<CloudHostedInstance> MSHosted
        {
            get
            {
                return SaasInstancesList;
            }
        }
        public List<CloudHostedInstance> AllWMs
        {
            get
            {
                return AllVMsList;
            }
        }

        public List<InstanceAttribute> InstanceAttributes
        {
            get
            {
                return InstanceAttributeList;
            }
        }


        public CookieContainer Cookies { get; set; }
        #endregion

        [DllImport("wininet.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool InternetGetCookieEx(string url, string cookieName, StringBuilder cookieData, ref int size, Int32 dwFlags, IntPtr lpReserved);

        #region base methods
        public MainWindow()
        {
            InitializeComponent();
            this.Icon = null;
            Navigation.Navigation.Frame = new FrameExt() { NavigationUIVisibility = NavigationUIVisibility.Hidden };
            Navigation.Navigation.Frame.Navigated += SplitViewFrame_OnNavigated;

            // Navigate to the home page.
            this.Loaded += (sender, args) => Navigation.Navigation.Navigate(new Uri("Views/MainPage.xaml", UriKind.RelativeOrAbsolute));

        }
        private void SplitViewFrame_OnNavigated(object sender, NavigationEventArgs e)
        {
            this.HamburgerMenuControl.Content = e.Content;
            this.HamburgerMenuControl.SelectedItem = e.ExtraData ?? ((ShellViewModel)this.DataContext).GetItem(e.Uri);
            this.HamburgerMenuControl.SelectedOptionsItem = e.ExtraData ?? ((ShellViewModel)this.DataContext).GetOptionsItem(e.Uri);
            GoBackButton.Visibility = Navigation.Navigation.Frame.CanGoBack ? Visibility.Visible : Visibility.Collapsed;
        }

        private void GoBack_OnClick(object sender, RoutedEventArgs e)
        {
            Navigation.Navigation.GoBack();
        }
        private void HamburgerMenuControl_OnItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs e)
        {
            if (e.InvokedItem is MenuItem menuItem && menuItem.IsNavigation)
            {
                Navigation.Navigation.Navigate(menuItem.NavigationDestination, menuItem);
            }
        }
        #endregion

        internal List<RDPConnectionDetails> GetUsers(CloudHostedInstance instance)
        {
            List<RDPConnectionDetails> rdpList = null;
            using (new WaitCursor())
            {
                rdpList = httpClientHelper.GetRdpConnectionDetails(instance);
            }
            return rdpList;
        }

        internal RDPTerminal GetMyRDP(string selectedPersonalVM)
        {
            RDPTerminal personalTerminal = null;
            foreach (var instance in AllVMsList)
            {
                if (instance.EnvironmentId == selectedPersonalVM)
                {
                    InstanceAttribute instanceAttribute = InstanceAttributeList.Find(x => x.EnvironmentId == instance.EnvironmentId);

                    string imageSource = "/Resources/DefaultVM.png";
                    if (instanceAttribute != null)
                    {
                        imageSource = instanceAttribute.ImageSource;
                    }
                    personalTerminal = new Model.RDPTerminal()
                    {
                        InstanceId = instance.InstanceId,
                        DeploymentStatus = instance.DeploymentStatus,
                        ApplicationRelease = instance.CurrentApplicationReleaseName,
                        CurrentPlatformReleaseName = instance.CurrentPlatformReleaseName,
                        TopologyType = instance.TopologyType,
                        DisplayName = instance.DisplayName,
                        EnvironmentId = instance.EnvironmentId,
                        Instances = instance.Instances,
                        SqlAzureCredentials = instance.SqlAzureCredentials,
                        NavigationLinks = instance.NavigationLinks,
                        ImageSource = imageSource
                    };
                    break;
                }
            }


            return personalTerminal;
        }
        public void LoadOrSelectProject()
        {
            Cookies = MainWindow.GetUriCookieContainer();
            if (Cookies != null)
            {
                httpClientHelper = new HttpClientHelper(Cookies)
                {
                    LcsUrl = _lcsUrl,
                    LcsUpdateUrl = _lcsUpdateUrl,
                    LcsDiagUrl = _lcsDiagUrl
                };
                
                if (SelectedProject != null)
                {
                    httpClientHelper.LcsProjectTypeId = SelectedProject.ProjectTypeId;
                    MSHostedProjectType = SelectedProject.ProjectTypeId;
                    httpClientHelper.ChangeLcsProjectId(SelectedProject.Id.ToString(CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name)));
                }
                SelectProject();
            }
            Navigation.Navigation.Navigate(new Uri("Views/MainPage.xaml", UriKind.RelativeOrAbsolute));
        }
        public void SelectProject()
        
        {
            if (httpClientHelper is null)
            {
                SharedMethods.NotLoggedIn();
                return;
            }
            LcsProject selected = ShowProjectListAndSelect();

            if(selected is null)
            {
                SharedMethods.StopDialog(Properties.Resources.NoProjectSelected, Properties.Resources.NoProjectSelectedInformation);
                return;
            }

            if (SelectedProject == null)
            {
                SelectedProject = selected;
                SwitchProjectForcedRefresh();
                return;
            }

            if (SelectedProject.Id != selected.Id)
            {
                SelectedProject = selected;
                SwitchProjectForcedRefresh();
                return;
            }

            ChoseRefreshOption();
        }
        public List<EnvironmentState> GetEnvironmentStatesList()
        {
            return EnvironmentStatesList;
        }

        public List<ReleaseInformation> GetReleaseInformation()
        {
            return ReleaseInformationList;
        }

        public List<PlatformReleaseInformation> GetPlatformReleaseInformation()
        {
            return PlatformReleaseInformationList;
        }
        public void SetFilter(bool active, EnvironmentState environmentState, ReleaseInformation releaseInformation, PlatformReleaseInformation platformReleaseInformation, CloudEnvironment cloudEnvironment)
        {
          
            if (cloudEnvironment == CloudEnvironment.CloudHosted)
            {
                filterValues = new FilterValues()
                {
                    Active = active,
                    environmentState = environmentState,
                    platformReleaseInformation = platformReleaseInformation,
                    releaseInformation = releaseInformation
                };                
                Properties.Settings.Default.FilterValues = JsonConvert.SerializeObject(filterValues, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
            }
            else
            {
                microsoftHostedFilterValues = new FilterValues()
                {
                    Active = active,
                    environmentState = environmentState,
                    platformReleaseInformation = platformReleaseInformation,
                    releaseInformation = releaseInformation
                };
                Properties.Settings.Default.MicrosoftHostedFilterValues = JsonConvert.SerializeObject(microsoftHostedFilterValues, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
            }
            Properties.Settings.Default.Save();
        }
        public FilterValues GetFilter(CloudEnvironment cloudEnvironment)
        {
            if(cloudEnvironment == CloudEnvironment.CloudHosted)
            {
                return filterValues;
            }
            if (cloudEnvironment == CloudEnvironment.MicrosoftHosted)
            {
                return microsoftHostedFilterValues;
            }
            return null;
        }
        public void AddEnvironmentState(EnvironmentState _state)
        {
            var environmentState = EnvironmentStatesList.FirstOrDefault(x => x.StateNum.Equals(_state.StateNum));
            if(environmentState == null)
            {
                EnvironmentStatesList.Add(_state);
                Properties.Settings.Default.EnvironmentStatesList = JsonConvert.SerializeObject(EnvironmentStatesList, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
            }
        }

        public void AddReleaseInformation(ReleaseInformation _releaseinformation, PlatformReleaseInformation _platformReleaseInformation)
        {
            var releaseinformation = ReleaseInformationList.FirstOrDefault(x => x.Release.Equals(_releaseinformation.Release));
            if (releaseinformation == null)
            {
                ReleaseInformationList.Add(_releaseinformation);
                Properties.Settings.Default.ReleaseInformationList = JsonConvert.SerializeObject(ReleaseInformationList, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
            }

            var platformreleaseinformation = PlatformReleaseInformationList.FirstOrDefault(x => x.PlatformRelease.Equals(_platformReleaseInformation.PlatformRelease));
            if(platformreleaseinformation == null)
            {
                PlatformReleaseInformationList.Add(_platformReleaseInformation);
                Properties.Settings.Default.PlatformReleaseInformationList = JsonConvert.SerializeObject(PlatformReleaseInformationList, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });

            }
        }

        private LcsProject ShowProjectListAndSelect()
        {
            SelectProjectDialog dlg = new SelectProjectDialog
            {
                HttpClientHelper = httpClientHelper,
                Owner = this
            };

            dlg.ShowDialog();
            LcsProject selected = dlg.LcsProject;
            return selected;
        }

        private void ChoseRefreshOption()
        {
            SelectRefreshOptions SelectOptionsDlg = new SelectRefreshOptions
            {
                Owner = this
            };

            SelectOptionsDlg.ShowDialog();

            bool refreshVM = SelectOptionsDlg.RefreshAllVms;
            bool refreshCredentials = SelectOptionsDlg.RefreshCredentials;

            if (refreshVM == false && refreshCredentials == false)
            {
                SharedMethods.StopDialog(Properties.Resources.CancellingRefresh, Properties.Resources.NoRefreshOptionsSelected);
                return;
            }
            Navigation.Navigation.Navigate(new Uri("Views/WaitPage.xaml", UriKind.RelativeOrAbsolute));
            RefreshAllInstances(refreshVM, refreshCredentials);
        }

        private void SwitchProjectForcedRefresh()
        {
            Navigation.Navigation.Navigate(new Uri("Views/WaitPage.xaml", UriKind.RelativeOrAbsolute));
            httpClientHelper.ChangeLcsProjectId(SelectedProject.Id.ToString(CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name)));
            RefreshAllInstances(true, true);
        }

        private static CookieContainer GetUriCookieContainer()
        {
            CookieContainer cookies = null;
            var datasize = 8192 * 16;
            var cookieData = new StringBuilder(datasize);
            if (!InternetGetCookieEx(_lcsUrl, null, cookieData, ref datasize, InternetCookieHttponly, IntPtr.Zero))
            {
                if (datasize < 0)
                    return null;
                cookieData = new StringBuilder(datasize);
                if (!InternetGetCookieEx(
                    _lcsUrl,
                    null, cookieData,
                    ref datasize,
                    InternetCookieHttponly,
                    IntPtr.Zero))
                    return null;
            }
            if (cookieData.Length > 0)
            {
                cookies = new CookieContainer();

                Properties.Settings.Default.cookie = cookieData.ToString().Replace(';', ',');
                Properties.Settings.Default.Save();
                cookies.SetCookies(new Uri(_lcsUrl), Properties.Settings.Default.cookie);
                cookies.SetCookies(new Uri(_lcsUpdateUrl), Properties.Settings.Default.cookie);
                cookies.SetCookies(new Uri(_lcsDiagUrl), Properties.Settings.Default.cookie);
            }
 
            return cookies;
        }

        public delegate void SessionChanged();
        public event SessionChanged SessionChangedEvent;

        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing">enable dispose</param>
        private void Dispose(bool disposing)
        {
            if (_disposed) return;
            _disposed = true;
            if (disposing)
            {
                httpClientHelper?.Dispose();
            }
        }

        public void SetLastActivity(DateTime _lastActivity)
        {
            Properties.Settings.Default.LastActivity = _lastActivity;

            DateTime sessionTimeOut = _lastActivity.AddMinutes(60.0);

            Properties.Settings.Default.SessionTimeOut = sessionTimeOut;
            Logouttime = string.Format(Properties.Resources.SessionTime, sessionTimeOut.ToString());

            Properties.Settings.Default.SessionStatus = Logouttime;
            Properties.Settings.Default.Save();

        }
        internal void CheckStartup()
        {
            DateTime sessionTimeOut = Properties.Settings.Default.LastActivity.AddMinutes(59.0);
            DateTime session24Hours = Properties.Settings.Default.LastActivity.AddHours(24.00);
            Properties.Settings.Default.SessionTimeOut = sessionTimeOut;

            DateTime DateTimeNow = DateTime.Now;
            Properties.Settings.Default.LimitFunctions = false;

            Logouttime = string.Format(Properties.Resources.SessionTime, sessionTimeOut.ToString());

            if(sessionTimeOut > DateTimeNow)
            {
                Properties.Settings.Default.LimitFunctions = false;
            }

            if(sessionTimeOut < DateTimeNow && sessionTimeOut < session24Hours)
            {
                Logouttime = Properties.Resources.LimitedSession;
                Properties.Settings.Default.LimitFunctions = true;
            }

            if(DateTimeNow > session24Hours)        
            {
                Logouttime = Properties.Resources.SessionEnded;
                Properties.Settings.Default.LimitFunctions = true;
            }

            Properties.Settings.Default.SessionStatus = Logouttime;
            Properties.Settings.Default.Save();
        }
        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Instances = JsonConvert.DeserializeObject<List<ProjectInstance>>(Properties.Settings.Default.instances) ?? new List<ProjectInstance>();
            Projects = JsonConvert.DeserializeObject<List<LcsProject>>(Properties.Settings.Default.projects) ?? new List<LcsProject>();
            AllVMsList = JsonConvert.DeserializeObject<List<CloudHostedInstance>>(Properties.Settings.Default.AllWMs) ?? new List<CloudHostedInstance>();
            UserCredentialsCloud = JsonConvert.DeserializeObject<List<RDPConnectionDetailsCache>>(Properties.Settings.Default.CloudHostedCredentials) ?? new List<RDPConnectionDetailsCache>();
            UserCredentialsSAAS  = JsonConvert.DeserializeObject<List<RDPConnectionDetailsCache>>(Properties.Settings.Default.MSHostedCredentials) ?? new List<RDPConnectionDetailsCache>();

            EnvironmentStatesList = JsonConvert.DeserializeObject<List<EnvironmentState>>(Properties.Settings.Default.EnvironmentStatesList) ?? new List<EnvironmentState>();
            PlatformReleaseInformationList = JsonConvert.DeserializeObject<List<PlatformReleaseInformation>>(Properties.Settings.Default.PlatformReleaseInformationList) ?? new List<PlatformReleaseInformation>();
            ReleaseInformationList = JsonConvert.DeserializeObject<List<ReleaseInformation>>(Properties.Settings.Default.ReleaseInformationList) ?? new List<ReleaseInformation>();
            
            //EnvironmentStatesList.Clear();
            //PlatformReleaseInformationList.Clear();
            //ReleaseInformationList.Clear();


            if (string.IsNullOrEmpty(Properties.Settings.Default.FilterValues))
            {
                filterValues = new FilterValues()
                {
                    Active = false
                };
            }
            else
            {
                filterValues = JsonConvert.DeserializeObject<FilterValues>(Properties.Settings.Default.FilterValues);
            }

            if (string.IsNullOrEmpty(Properties.Settings.Default.MicrosoftHostedFilterValues))
            {
                microsoftHostedFilterValues = new FilterValues()
                {
                    Active = false
                };
            }
            else
            {
                microsoftHostedFilterValues = JsonConvert.DeserializeObject<FilterValues>(Properties.Settings.Default.MicrosoftHostedFilterValues);
            }

            GetMenuItems();
            //CheckStartup();

            InstanceAttributeList = JsonConvert.DeserializeObject<List<InstanceAttribute>>(Properties.Settings.Default.instanceattributes) ?? new List<InstanceAttribute>();

            if(!string.IsNullOrEmpty(Properties.Settings.Default.LoggedInUri))
            {
                LoggedInUri = new Uri(Properties.Settings.Default.LoggedInUri);
            }

            if (!string.IsNullOrEmpty(Properties.Settings.Default.cookie))
            {
                Cookies = new CookieContainer();
                Cookies.SetCookies(new Uri(_lcsUrl), Properties.Settings.Default.cookie);
                Cookies.SetCookies(new Uri(_lcsUpdateUrl), Properties.Settings.Default.cookie);
                Cookies.SetCookies(new Uri(_lcsDiagUrl), Properties.Settings.Default.cookie);
                httpClientHelper = new HttpClientHelper(Cookies)
                {
                    LcsUrl = _lcsUrl,
                    LcsUpdateUrl = _lcsUpdateUrl,
                    LcsDiagUrl = _lcsDiagUrl
                };

                SelectedProject = GetLcsProjectFromCookie();

                if (SelectedProject != null)
                {
                    httpClientHelper.LcsProjectTypeId = SelectedProject.ProjectTypeId;
                    MSHostedProjectType = SelectedProject.ProjectTypeId;
                    httpClientHelper.ChangeLcsProjectId(SelectedProject.Id.ToString(CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name)));

                    var projectInstance = Instances.FirstOrDefault(x => x.LcsProjectId.Equals(SelectedProject.Id));
                    if (projectInstance != null)
                    {
                        if (projectInstance.CheInstances != null)
                        {
                            CloudHostedInstancesList = projectInstance.CheInstances;
                        }
                        if (projectInstance.SaasInstances != null)
                        {
                            SaasInstancesList = projectInstance.SaasInstances;
                        }
                    }
                }
            }
            StartTimer();
        }



        private async void StartTimer()
        {
            MainWindow mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            if(mainWindow == null)
            {
                return;
            }   
            using (var timer = new TaskTimer(15000).Start())
            {
                foreach (var tick in timer)
                {
                    await tick;
                    CheckStartup();
                    CheckIfRefreshPossible();
                    SessionChangedEvent?.Invoke();
                }
            }
        }

        private void CheckIfRefreshPossible()
        {
            if (Logouttime != Properties.Resources.SessionEnded)
            {
                DateTime timeOut = Properties.Settings.Default.SessionTimeOut;
                DateTime timeNow = DateTime.Now;

            }
        }

        private LcsProject GetLcsProjectFromCookie()
        {
            var cookies = Cookies.GetCookies(new Uri(_lcsUrl));
            foreach (Cookie cookie in cookies)
            {
                if (cookie.Name == "lcspid")
                {
                    return Projects.FirstOrDefault(x => x.Id.Equals(int.Parse(cookie.Value, CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name))));
                }
            }
            return null;
        }
        private async void RefreshCloudHosted(bool refreshAllWM,bool refreshCredentials)
        {
            bool t = await Task.Run(() => PerformReloadCloudHosted(refreshAllWM, refreshCredentials)).ConfigureAwait(true);
        }
        private bool PerformReloadCloudHosted(bool refreshAllWM, bool refreshCredentials)
        {
            try
            {
                if (refreshAllWM == true)
                {
                    CloudHostedInstancesList = httpClientHelper.GetCheInstances();
                    if (CloudHostedInstancesList != null)
                    {
                        if (Instances.Exists(x => x.LcsProjectId == SelectedProject.Id))
                        {
                            Instances.Where(x => x.LcsProjectId == SelectedProject.Id)
                                .Select(x => { x.CheInstances = CloudHostedInstancesList; return x; })
                                    .ToList();
                        }

                        Properties.Settings.Default.instances = JsonConvert.SerializeObject(Instances, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
                        AddToAllWMsList(CloudHostedInstancesList);
                    }
                }
                if (refreshCredentials == true)
                {
                    CacheCredentials(CloudHostedInstancesList, LCSEnvironments.CHE);
                    RefreshCredentialsCloudStatus = true;
                }

                Properties.Settings.Default.Save();
                ClearCloudHostStatus = true;
            }
            catch (Exception ex)
            {
                SharedMethods.StopDialog(Properties.Resources.Error, ex.Message);
            }
            return ClearCloudHostStatus;

        }

        private void CacheCredentials(List<CloudHostedInstance> cloudHostedInstancesList, LCSEnvironments environment)
        {
            List<RDPConnectionDetailsCache> rdpConnectionDetails = new List<RDPConnectionDetailsCache>();
            foreach (var (instance, connectionDetails, connectionCache) in from CloudHostedInstance instance in cloudHostedInstancesList
                                                                           let instanceConnectionDetails = httpClientHelper.GetRdpConnectionDetails(instance)
                                                                           from RDPConnectionDetails connectionDetails in instanceConnectionDetails
                                                                           let connectionCache = new RDPConnectionDetailsCache()
                                                                           select (instance, connectionDetails, connectionCache))
            {
                connectionCache.Address = connectionDetails.Address;
                connectionCache.Domain = connectionDetails.Domain;
                connectionCache.EnvironmentId = instance.EnvironmentId;
                connectionCache.Machine = connectionDetails.Machine;
                connectionCache.Password = connectionDetails.Password;
                connectionCache.Port = connectionDetails.Port;
                connectionCache.Username = connectionDetails.Username;
                rdpConnectionDetails.Add(connectionCache);
            }

            if (environment == LCSEnvironments.CHE)
            {
                Properties.Settings.Default.CloudHostedCredentials = JsonConvert.SerializeObject(rdpConnectionDetails, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
            }
            if(environment == LCSEnvironments.SAAS)
            {
                Properties.Settings.Default.MSHostedCredentials = JsonConvert.SerializeObject(rdpConnectionDetails, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
            }
        }

        private async void ClearAllWmsList(bool refreshAllWM, bool refreshCredentials)
        {
            bool t = await Task.Run(() => ClearVM(refreshAllWM, refreshCredentials)).ConfigureAwait(true);
        }
        private bool ClearVM(bool refreshAllWM, bool refreshCredentials)
        {
            if(refreshAllWM == true)
            {
                AllVMsList.Clear();
            }
            if(refreshCredentials == true)
            {
                UserCredentialsSAAS.Clear();
                UserCredentialsCloud.Clear();
            }
            ClearVMStatus = true;
            return ClearVMStatus;
        }
        private bool PerformReloadSaas(bool refreshAllWM, bool refreshCredentials)
        {
            try
            {
                if (refreshAllWM == true)
                {
                    SaasInstancesList = httpClientHelper.GetHostedInstances();

                    if (SaasInstancesList != null)
                    {
                        if (Instances.Exists(x => x.LcsProjectId == SelectedProject.Id))
                        {
                            Instances.Where(x => x.LcsProjectId == SelectedProject.Id)
                                .Select(x => { x.SaasInstances = SaasInstancesList; return x; })
                                    .ToList();
                        }
                        AddToAllWMsList(SaasInstancesList);
                        Properties.Settings.Default.instances = JsonConvert.SerializeObject(Instances, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
                    }
                }
                if (refreshCredentials == true)
                {
                    CacheCredentials(SaasInstancesList, LCSEnvironments.SAAS);
                    RefreshCredentialsMSHostedStatus = true;
                }

                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                SharedMethods.StopDialog(Properties.Resources.Error, ex.Message);
            }

     

            ClearSaaSStatus = true;
            return ClearSaaSStatus;
        }
        private async void RefreshSaas(bool refreshAllWM, bool refreshCredentials)
        {
            bool t = await Task.Run(() => PerformReloadSaas(refreshAllWM, refreshCredentials)).ConfigureAwait(true);
        }

        private void AddToAllWMsList(List<CloudHostedInstance> cloudHostedInstancesList)
        {
            foreach (CloudHostedInstance instance in cloudHostedInstancesList)
            {
                AllVMsList.Add(instance);
            }
        }

        public void AddInstance(int _projectId)
        {
            if (!Instances.Exists(x => x.LcsProjectId == _projectId))
            {
                var instance = new ProjectInstance()
                {
                    LcsProjectId = _projectId,
                };

                Instances.Add(instance);
                Properties.Settings.Default.instances = JsonConvert.SerializeObject(Instances, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
                Properties.Settings.Default.Save();
            }

        }
        public Dictionary<string, string> GetCredentials(string environmentId, string itemName)
        {
            return httpClientHelper.GetCredentials(environmentId, itemName);
        }
        internal bool CheckLogin()
        {
            if (httpClientHelper is null)
            {
                SharedMethods.NotLoggedIn();
                return false;
            }
            if(Logouttime == Properties.Resources.SessionEnded)
            {
                SharedMethods.NotLoggedIn();
                return false;
            }
            return true;
        }
        internal async void RefreshAllInstances(bool refreshAllWM, bool refreshCredentials)
        {

            try
            {
                if (CheckLogin() == true)
                {
                    var mySettings = new MetroDialogSettings()
                    {
                        NegativeButtonText = "Close now",
                        AnimateShow = false,
                        AnimateHide = false,
                        ColorScheme = this.MetroDialogOptions.ColorScheme
                    };

                    string message = string.Format(Properties.Resources.InitRefresh, Environment.NewLine, Environment.NewLine);

                    var controller = await this.ShowProgressAsync("Please wait...", message, settings: mySettings).ConfigureAwait(true);
                    controller.SetIndeterminate();

                    ClearSaaSStatus = false;
                    ClearCloudHostStatus = false;
                    ClearVMStatus = false;
                    RefreshCredentialsCloudStatus = false;
                    RefreshCredentialsMSHostedStatus = false;

                    ClearAllWmsList(refreshAllWM, refreshCredentials);
                    RefreshCloudHosted(refreshAllWM, refreshCredentials);
                    RefreshSaas(refreshAllWM, refreshCredentials);

                    string clearProgress = Properties.Resources.InProgress;
                    string clearSaaSProgress = clearProgress;
                    string clearCloudProgress = clearProgress;
                    string clearCredentialsCloudProgress = clearProgress;
                    string clearCredentialsMSProgress = clearProgress;


                    if (refreshAllWM == false)
                    {
                        clearProgress = Properties.Resources.Skipped;
                        clearSaaSProgress = clearProgress;
                        clearCloudProgress = clearProgress;
                    }
                    if (refreshCredentials == false)
                    {
                        clearCredentialsCloudProgress = Properties.Resources.Skipped;
                        clearCredentialsMSProgress = clearCredentialsCloudProgress;
                    }


                    while (true)
                    {

                        if (ClearVMStatus == true)
                        {
                            if (refreshAllWM == true && clearProgress != Properties.Resources.Skipped)
                            {
                                clearProgress = Properties.Resources.Done;
                            }
                        }
                        if (ClearSaaSStatus == true)
                        {
                            if (refreshAllWM == true && clearSaaSProgress != Properties.Resources.Skipped)
                            {
                                clearSaaSProgress = Properties.Resources.Done;
                            }
                        }
                        if (ClearCloudHostStatus == true)
                        {
                            if (refreshAllWM == true && clearCloudProgress != Properties.Resources.Skipped)

                            {
                                clearCloudProgress = Properties.Resources.Done;
                            }
                        }
                        if (RefreshCredentialsCloudStatus == true && refreshCredentials == true)
                        {
                            clearCredentialsCloudProgress = Properties.Resources.Done;
                        }
                        if (RefreshCredentialsMSHostedStatus == true && refreshCredentials == true)
                        {
                            clearCredentialsMSProgress = Properties.Resources.Done;
                        }

                        message = string.Format(Properties.Resources.RefreshProgress, clearProgress, clearCloudProgress, clearSaaSProgress, clearCredentialsCloudProgress, clearCredentialsMSProgress);
                        controller.SetMessage(message);

                        if (controller.IsCanceled)
                            break; //canceled progressdialog auto closes.

                        if (refreshAllWM == true)
                        {
                            if (ClearSaaSStatus == true && ClearVMStatus == true && ClearCloudHostStatus == true)
                            {
                                Properties.Settings.Default.AllWMs = JsonConvert.SerializeObject(AllVMsList, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
                                Properties.Settings.Default.Save();
                                break;
                            }

                        }
                        if (refreshCredentials == true)
                        {
                            if (RefreshCredentialsCloudStatus == true && RefreshCredentialsMSHostedStatus && true)
                            {
                                break;
                            }
                        }
                        if (refreshAllWM == false && refreshCredentials == false)
                        {
                            break;
                        }

                        await Task.Delay(2000).ConfigureAwait(true);
                    }

                    await controller.CloseAsync().ConfigureAwait(true);
                    EnableMenuOptions(true);
                    Navigation.Navigation.Navigate(new Uri("Views/MainPage.xaml", UriKind.RelativeOrAbsolute));
                }

            }
            catch (Exception ex)
            {
                SharedMethods.StopDialog(Properties.Resources.Error, ex.Message);
            }

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        internal void OpenRDPSession(string _environmentId)
        {
            if (httpClientHelper is null)
            {
                SharedMethods.NotLoggedIn();
                return;
            }

            CloudHostedInstance selectedInstance = null;
            foreach (var instance in from CloudHostedInstance instance in AllVMsList
                                     where instance.EnvironmentId == _environmentId
                                     select instance)
            {
                selectedInstance = instance;
                break;
            }

            if (selectedInstance != null)
            {
               
                RDPConnectionDetails rdpEntry = GetRDPEntry(selectedInstance);

                if (rdpEntry != null)
                {
                    if (Properties.Settings.Default.MimimizeOnStartRDP == true)
                    {
                        this.WindowState = WindowState.Minimized;
                    }
                    using (new RdpCredentials(rdpEntry.Address, $"{rdpEntry.Domain}\\{rdpEntry.Username}", rdpEntry.Password))
                    {
                        var rdcProcess = CreateProcess(rdpEntry);
                        rdcProcess.Start();
                    }

                }
            }
            return;
        }
        internal RDPConnectionDetails GetRDPEntry(CloudHostedInstance selectedInstance)
        {
            RDPConnectionDetails rdpEntry = null;
            bool isSelectedEnvironment = false;

            string environmentId = Properties.Settings.Default.SelectedPersonalVM;
            if(selectedInstance.EnvironmentId == environmentId)
            {
                rdpEntry = GetDefaultUser();
                isSelectedEnvironment = true;
            }

            if (rdpEntry is null)
            {
                List<RDPConnectionDetails> rdpList = null;
                if (selectedInstance != null)
                {
                    rdpList = new List<RDPConnectionDetails>();
                    using (new WaitCursor())
                    {
                        if (UserCredentialsCloud.Count > 0)
                        {

                            rdpList.AddRange(from RDPConnectionDetailsCache connectionCache in UserCredentialsCloud
                                             where connectionCache.EnvironmentId == selectedInstance.EnvironmentId
                                             let connection = new RDPConnectionDetails
                                             {
                                                 Address = connectionCache.Address,
                                                 Domain = connectionCache.Domain,
                                                 Machine = connectionCache.Machine,
                                                 Password = connectionCache.Password,
                                                 Port = connectionCache.Port,
                                                 Username = connectionCache.Username
                                             }
                                             select connection);
                        }
                        if (UserCredentialsSAAS.Count > 0)
                        {

                            rdpList.AddRange(from RDPConnectionDetailsCache connectionCache in UserCredentialsSAAS
                                             where connectionCache.EnvironmentId == selectedInstance.EnvironmentId
                                             let connection = new RDPConnectionDetails
                                             {
                                                 Address = connectionCache.Address,
                                                 Domain = connectionCache.Domain,
                                                 Machine = connectionCache.Machine,
                                                 Password = connectionCache.Password,
                                                 Port = connectionCache.Port,
                                                 Username = connectionCache.Username
                                             }
                                             select connection);
                        }
                    }

                    if(rdpList.Count == 0)
                    {
                        using (new WaitCursor())
                        {
                            rdpList = httpClientHelper.GetRdpConnectionDetails(selectedInstance);
                            SetLastActivity(DateTime.Now);
                        }
                    }

                    if (rdpList.Count > 1)
                    {
                        rdpEntry = ChooseRdpLogonUser(rdpList, isSelectedEnvironment);
                    }
                    else
                    {
                        rdpEntry = rdpList.First();
                    }
                }
            }

            return rdpEntry;
        }
        internal Process CreateProcess(RDPConnectionDetails rdpEntry)
        {
            Process process = new Process();

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\system32\mstsc.exe")
            };

            string arguments = "/v " + $"{rdpEntry.Address}:{rdpEntry.Port}";

            if(Properties.Settings.Default.Span == true)
            {
                arguments = string.Format("{0} /span", arguments);
            }

            if(Properties.Settings.Default.Span == false && Properties.Settings.Default.Multimon == false)
            {
                if (Properties.Settings.Default.Resolution == 16)
                {
                    arguments = string.Format("{0} /f", arguments);
                }
                else
                {
                    string resolution = GetResolution();
                    arguments = string.Format("{0} {1}", arguments, resolution);
                }
            }

            if(Properties.Settings.Default.Multimon == true)
            {
                arguments = string.Format("{0} /multimon", arguments);
            }


            startInfo.Arguments = arguments;
            process.StartInfo = startInfo;

            return process;

        }

        private string GetResolution()
        {
            double value = Properties.Settings.Default.Resolution;
            string resolution;
            switch (value)
            {
                case 0:
                    resolution = $"/w:640 /h:480";
                    break;

                case 1:
                    resolution = $"/w:800 /h:600";
                    break;

                case 2:
                    resolution = $"/w:1024 /h:768";
                    break;

                case 3:
                    resolution = $"/w:1280 /h:720";
                    break;

                case 4:
                    resolution = $"/w:1280 /h:768";
                    break;

                case 5:
                    resolution = $"/w:1280 /h:800";
                    break;

                case 6:
                    resolution = $"/w:1280 /h:1024";
                    break;

                case 7:
                    resolution = $"/w:1366 /h:768";
                    break;

                case 8:
                    resolution = $"/w:1440 /h:900";
                    break;

                case 9:
                    resolution = $"/w:1440 /h:1050";
                    break;

                case 10:
                    resolution = $"/w:1600 /h:1200";
                    break;

                case 11:
                    resolution = $"/w:1680 /h:1050";
                    break;

                case 12:
                    resolution = $"/w:1920 /h:1080";
                    break;

                case 13:
                    resolution = $"/w:1920 /h:1200";
                    break;

                case 14:
                    resolution = $"/w:2560 /h:1440";
                    break;

                case 15:
                    resolution = $"/w:3440 /h:1440";
                    break;

                case 16:
                    resolution = $"/f";
                    break;
                default:
                    resolution = $"/f";
                    break;
            }
            return resolution;
        }

        internal void StartStopRDPSession(string _environmentId, VMAction _action)
        {
            if (httpClientHelper is null)
            {
                SharedMethods.NotLoggedIn();
                return;
            }

            CloudHostedInstance selectedInstance = null;

            foreach (CloudHostedInstance instance in AllVMsList)
            {
                if (instance.EnvironmentId == _environmentId)
                {
                    selectedInstance = instance;
                    break;
                }
            }

            if (selectedInstance != null)
            {
                string action = string.Empty;
                string message = string.Empty;
                if (_action == VMAction.Start)
                {
                    action = "start";
                    message = string.Format(Properties.Resources.ConfirmStartInstance, selectedInstance.DisplayName);
                }
                if (_action == VMAction.Stop)
                {
                    action = "stop";
                    message = string.Format(Properties.Resources.ConfirmStopInstance, selectedInstance.DisplayName);
                }

                SetLastActivity(DateTime.Now);
                ConfirmStartStopInstance(selectedInstance, action, message);
            }
        }

        private async void ConfirmStartStopInstance(CloudHostedInstance selectedInstance, string action,string message)
        {
            var res = await InfoBox.ShowMessageAsync(Properties.Resources.ConfirmAction, message, MessageDialogStyle.AffirmativeAndNegative).ConfigureAwait(true);
            if (res == MessageDialogResult.Affirmative)
            {
                using (new WaitCursor())
                {
                    if (selectedInstance != null)
                    {
                        var tasks = new List<Task>
                        {
                            Task.Run(() => new HttpClientHelper(Cookies) { LcsUrl = _lcsUrl, LcsUpdateUrl = _lcsUpdateUrl, LcsDiagUrl = _lcsDiagUrl, LcsProjectId = SelectedProject.Id.ToString() }.StartStopDeployment(selectedInstance, action))
                        };
                        Task.WhenAll(tasks).Wait();
                    }
                }
            }
        }

        public RDPConnectionDetails ChooseRdpLogonUser(List<RDPConnectionDetails> rdpList,bool isSelectedEnvironment)
        {
            SelectRDPUserDialog dlg = new SelectRDPUserDialog();
            dlg.SetRDPList(rdpList);
            dlg.SelectedEnvironment = isSelectedEnvironment;
            dlg.Owner = this;
            dlg.ShowDialog();
            RDPConnectionDetails selectedUser = dlg.getSelectedUser();
            return selectedUser;
        }
        internal void NewFirewallRule(string environmentId)
        {
            if (httpClientHelper is null)
            {
                SharedMethods.NotLoggedIn();
                return;
            }

            CloudHostedInstance selectedInstance = null;

            foreach (CloudHostedInstance instance in AllVMsList)
            {
                if (instance.EnvironmentId == environmentId)
                {
                    selectedInstance = instance;
                    break;
                }
            }
            if (selectedInstance != null)
            {
                AddFirewallExceptionDialog dlg = new AddFirewallExceptionDialog
                {
                    selectedInstance = selectedInstance,
                    Owner = this
                };
                dlg.ShowDialog();
            }
        }
        private async void GetPackages(CloudHostedInstance _instance)
        {
            bool t = await Task.Run(() => PerformGetPackages(_instance)).ConfigureAwait(true);
        }
        private bool PerformGetPackages(CloudHostedInstance _instance)
        {
            if (_instance != null)
            {
                Packages = httpClientHelper.GetPagedDeployablePackageList(_instance);
            }
            GetPackagesStatus = true;
            return GetPackagesStatus;
        }
        internal async void DeployPackage(string environmentId)
        {
            if (httpClientHelper is null)
            {
                SharedMethods.NotLoggedIn();
                return;
            }

            Packages.Clear();
            GetPackagesStatus = false;

            CloudHostedInstance selectedInstance = null;
            foreach (CloudHostedInstance instance in AllVMsList)
            {
                if (instance.EnvironmentId == environmentId)
                {
                    selectedInstance = instance;
                    break;
                }
            }
            if (selectedInstance != null)
            {
                GetPackages(selectedInstance);
            }

            var mySettings = new MetroDialogSettings()
            {
                NegativeButtonText = "Close now",
                AnimateShow = false,
                AnimateHide = false,
                ColorScheme = this.MetroDialogOptions.ColorScheme
            };

            string message = string.Format(Properties.Resources.FetchingPackages, selectedInstance.EnvironmentId, selectedInstance.DisplayName);
            var controller = await this.ShowProgressAsync(Properties.Resources.PleaseWait, message, settings: mySettings).ConfigureAwait(true);

            controller.SetIndeterminate();

            while (GetPackagesStatus == false)
            {
                controller.SetMessage(message);
                if (controller.IsCanceled)
                    break; //canceled progressdialog auto closes.
                await Task.Delay(2000).ConfigureAwait(true);
            }

            await controller.CloseAsync().ConfigureAwait(true);

            if (Packages.Count > 0)
            {
                DeployPackageDialog dlg = new DeployPackageDialog
                {
                    Owner = this
                };
                dlg.SetPackagesList(Packages);
                dlg.ShowDialog();
                DeployablePackage selectedPackage = dlg.DeployablePackage;
                if (selectedPackage != null)
                {
                    ConfirmDeployPackage(selectedPackage, selectedInstance);
                }
            }

        }
        private async void ConfirmDeployPackage(DeployablePackage deployablePackage, CloudHostedInstance selectedInstance)
        {
            string message = string.Format(Properties.Resources.ConfirmDeployPackage, deployablePackage.Description);
            var res = await InfoBox.ShowMessageAsync(Properties.Resources.ConfirmAction, message, MessageDialogStyle.AffirmativeAndNegative).ConfigureAwait(true);
            if (res == MessageDialogResult.Affirmative)
            {
                using (new WaitCursor())
                {
                    var tasks = new List<Task<string>>();
                    if(tasks != null)
                    {
                        tasks.Add(Task.Run(() => new HttpClientHelper(Cookies) { LcsUrl = _lcsUrl, LcsUpdateUrl = _lcsUpdateUrl, LcsDiagUrl = _lcsDiagUrl, LcsProjectId = SelectedProject.Id.ToString() }.ApplyPackage(selectedInstance, deployablePackage)));
                        Task.WhenAll(tasks).Wait();
                    }
                }
            }
        }
        internal void RemoveFirewallRule(string environmentId)
        {
            if (httpClientHelper is null)
            {
                SharedMethods.NotLoggedIn();
                return;
            }

            CloudHostedInstance selectedInstance = null;

            foreach (CloudHostedInstance instance in AllVMsList)
            {
                if (instance.EnvironmentId == environmentId)
                {
                    selectedInstance = instance;
                    break;
                }
            }
            if (selectedInstance != null)
            {
                RemoveFirewallException dlg = new RemoveFirewallException
                {
                    SelectedInstance = selectedInstance,
                    Owner = this
                };
                dlg.ShowDialog();
                if (dlg.NSGRule != null)
                {
                    ConfirmRemoval(dlg.NSGRule, selectedInstance);
                }
            }

        }
        private async void ConfirmRemoval(NSGRule nSGRule, CloudHostedInstance selectedInstance)
        {
            string message = string.Format(Properties.Resources.RemoveFirewallRule, nSGRule.Name);
            var res = await InfoBox.ShowMessageAsync(Properties.Resources.ConfirmAction, message, MessageDialogStyle.AffirmativeAndNegative).ConfigureAwait(true);
            try
            {
                if (res == MessageDialogResult.Affirmative)
                {
                    SetLastActivity(DateTime.Now);

                    using (new WaitCursor())
                    {
                        List<Task<string>> tasks = new List<Task<string>>
                        {
                            Task.Run(() => httpClientHelper.DeleteNsgRule(selectedInstance, nSGRule.Name))
                        };
                        //tasks.Add(Task.Run(() => new HttpClientHelper(Cookies) { LcsUrl = _lcsUrl, LcsUpdateUrl = _lcsUpdateUrl, LcsDiagUrl = _lcsDiagUrl, LcsProjectId = SelectedProject.Id.ToString() }.DeleteNsgRule(selectedInstance, nSGRule.Name)));
                        Task.WhenAll(tasks).Wait();
                    }
                }
            }
            catch (Exception ex)
            {
                using (EventLog eventLog = new EventLog(Properties.Resources.Application))
                {
                    eventLog.Source = Properties.Resources.Application;
                    eventLog.WriteEntry(ex.ToString(), EventLogEntryType.Error, 100);
                }

            }

        }
        internal void AddFirewallRule(CloudHostedInstance selectedInstance, string name, string address)
        {
            if (selectedInstance is null) return;
            if (string.IsNullOrEmpty(name)) return;
            if (string.IsNullOrEmpty(address)) return;
            try
            {
                using (new WaitCursor())
                {
                    SetLastActivity(DateTime.Now);

                    List<Task> tasks = new List<Task>
                    {
                        Task.Run(() => httpClientHelper.AddNsgRule(selectedInstance, name, address))
                    };
                    //asks.Add(Task.Run(() => new HttpClientHelper(Cookies) { LcsUrl = _lcsUrl, LcsUpdateUrl = _lcsUpdateUrl, LcsDiagUrl = _lcsDiagUrl, LcsProjectId = SelectedProject.Id.ToString() }.AddNsgRule(selectedInstance, name, address)));
                    Task.WhenAll(tasks).Wait();
                }
            }
            catch (Exception ex)
            {
                using (EventLog eventLog = new EventLog(Properties.Resources.Application))
                {
                    eventLog.Source = Properties.Resources.Application;
                    eventLog.WriteEntry(ex.ToString(), EventLogEntryType.Error, 100);
                }

            }
        }

        internal NetworkSecurityGroup GetNetworkSecurityGroup(CloudHostedInstance selectedInstance)
        {
            if (httpClientHelper is null)
            {
                SharedMethods.NotLoggedIn();
                return null;
            }

            return httpClientHelper.GetNetworkSecurityGroup(selectedInstance);
        }
        private void GetMenuItems()
        {
            ShellViewModel viewmodel = (ShellViewModel)this.DataContext;
            MyCloudHostedMenuItem = viewmodel.GetItem(new Uri("Views/CloudHostedMachinePage.xaml", UriKind.RelativeOrAbsolute)) as ViewModels.MenuItem;
            MyMSHostedMenuItem = viewmodel.GetItem(new Uri("Views/MSHostedPage.xaml", UriKind.RelativeOrAbsolute)) as ViewModels.MenuItem;
            MySettingsMenuItem = viewmodel.GetOptionsItem(new Uri("Views/SettingsPage.xaml", UriKind.RelativeOrAbsolute)) as ViewModels.MenuItem;
        }
        internal void EnableCloudHosted(bool enable)
        {
            MyCloudHostedMenuItem.IsEnabled = enable;
        }
        internal void EnableMSHosted(bool enable)
        {
            MyMSHostedMenuItem.IsEnabled = enable;
        }
        public void EnableMenuOptions(bool enable)
        {
            MyCloudHostedMenuItem.IsEnabled = enable;
            MyMSHostedMenuItem.IsEnabled = enable;
            MySettingsMenuItem.IsEnabled = enable;
            if(CloudHosted == null || CloudHosted.Count == 0)
            {
                MyCloudHostedMenuItem.IsEnabled = false;
            }
            if(MSHosted == null || MSHosted.Count == 0)
            {
                MyMSHostedMenuItem.IsEnabled = false;
            }

        }
        public void ClearLocalList()
        {
            SelectedProject = null;
            if(Instances != null)
            {
                Instances.Clear();
            }
            if(Projects != null)
            {
                Projects.Clear();
            }
            if(SaasInstancesList != null)
            {
                SaasInstancesList.Clear();
            }
            if(CloudHostedInstancesList != null)
            {
                CloudHostedInstancesList.Clear();
            }
            if(Packages != null)
            {
                Packages.Clear();
            }
            if(AllVMsList != null)
            {
                AllVMsList.Clear();
            }
        }
        public void SetLoggedInUri(Uri loggedInUri)
        {
            LoggedInUri = loggedInUri ?? throw new Exception(Properties.Resources.NullURI);
            Properties.Settings.Default.LoggedInUri = LoggedInUri.ToString();
            Properties.Settings.Default.Save();
        }
        public Uri GetLoggedInUri()
        {
            return LoggedInUri;
        }
        internal void GoToLogoutPage()
        {
            Navigation.Navigation.Navigate(new Uri("Views/LogOutPage.xaml", UriKind.RelativeOrAbsolute));

        }
        internal void ClearAndClose(bool _close = true)
        {
            Properties.Settings.Default.projects = "";
            Properties.Settings.Default.instances = "";
            Properties.Settings.Default.cookie = "";
            Properties.Settings.Default.AllWMs = "";
            Properties.Settings.Default.SelectedPersonalVM = "";
            Properties.Settings.Default.instanceattributes = "";
            Properties.Settings.Default.Save();

            ClearLocalList();
            if(_close == true)
            {
                Application.Current.MainWindow.Close();
            }
        }
        internal void SetPersonalVM(string environmentId)
        {
            Properties.Settings.Default.SelectedPersonalVM = environmentId;
            Properties.Settings.Default.Save();
        }
        internal void SetDefaultUser(RDPConnectionDetails selectedUser)
        {
            Properties.Settings.Default.DefaultUser = JsonConvert.SerializeObject(selectedUser, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
            Properties.Settings.Default.Save();
        }
        internal RDPConnectionDetails GetDefaultUser()
        {
            RDPConnectionDetails connectionDetails;
            connectionDetails = JsonConvert.DeserializeObject<RDPConnectionDetails>(Properties.Settings.Default.DefaultUser);

            return connectionDetails;
        }

    }

    public class WaitCursor : IDisposable
    {
        private readonly Cursor _previousCursor;

        public WaitCursor()
        {
            _previousCursor = Mouse.OverrideCursor;

            Mouse.OverrideCursor = Cursors.Wait;
        }

        #region IDisposable Members

        public void Dispose()
        {
            Mouse.OverrideCursor = _previousCursor;
        }

        #endregion
    }
}

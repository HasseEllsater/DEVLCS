using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using MahApps.Metro.Controls;
using LCSCarousel.ViewModels;
using MenuItem = LCSCarousel.ViewModels.MenuItem;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using LCSCarousel;
using System.Globalization;
using LCSCarousel.Views;
using Newtonsoft.Json;
using System.Linq;
using LCSCarousel.Classes;
using System.Diagnostics;
using LCSCarousel.Model;
using System.Windows.Input;
using System.Web.ClientServices.Providers;
using System.Configuration;
using System.Reflection;

namespace LCSCarousel
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1060:Move pinvokes to native methods class", Justification = "<Pending>")]
    public partial class MainWindow : MetroWindow, IDisposable
    {
#pragma warning disable CA1802 // Use literals where appropriate
        private static readonly string _lcsDiagUrl = "https://diag.lcs.dynamics.com";
        private static readonly string _lcsUpdateUrl = "https://update.lcs.dynamics.com";
        //private ViewModels.MenuItem myRDPMenuItem { get; set; }
        private ViewModels.MenuItem myCloudHostedMenuItem { get; set; }
        private ViewModels.MenuItem myMSHostedMenuItem { get; set; }
        private Uri LoggedInUri;
        private RDPConnectionDetails selectedDefaultUser;

        private ViewModels.MenuItem mySettingsMenuItem { get; set; }
        private static readonly string _lcsUrl = "https://lcs.dynamics.com";

#pragma warning restore CA1802 // Use literals where appropriate

        private const int InternetCookieHttponly = 0x2000;
        private bool _disposed;
        private bool sessionStarted;

        private HttpClientHelper httpClientHelper;
        private LcsProject SelectedProject;
        private List<ProjectInstance> Instances;

        internal List<RDPConnectionDetails> GetUsers(CloudHostedInstance instance)
        {
            List<RDPConnectionDetails> rdpList = null;
            using (new WaitCursor())
            {
                rdpList = httpClientHelper.GetRdpConnectionDetails(instance);
            }
            return rdpList;
        }

        private List<LcsProject> Projects;
        private List<CloudHostedInstance> SaasInstancesList;
        private List<CloudHostedInstance> CloudHostedInstancesList;
        private List<DeployablePackage> Packages = new List<DeployablePackage>();

        internal RDPTerminal getMyRDP(string selectedPersonalVM)
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

        private List<InstanceAttribute> InstanceAttributeList;
        private List<CloudHostedInstance> AllVMsList;
        private bool ClearVMStatus { get; set; }
        private bool ClearSaaSStatus { get; set; }
        private bool ClearCloudHostStatus { get; set; }
        private bool GetPackagesStatus { get; set; }

        public enum VMAction
        {
            Start,
            Stop
        }
        public enum HotfixesType
        {
            Metadata = 8,
            PlatformBinary = 11,
            ApplicationBinary = 9,
            CriticalMetadata = 16
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

        [DllImport("wininet.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool InternetGetCookieEx(string url, string cookieName, StringBuilder cookieData, ref int size, Int32 dwFlags, IntPtr lpReserved);

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

        public void LoginToLCS()
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
                    httpClientHelper.ChangeLcsProjectId(SelectedProject.Id.ToString(CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name)));
                }
                Navigation.Navigation.Navigate(new Uri("Views/WaitPage.xaml", UriKind.RelativeOrAbsolute));
                SelectProject();
            }

        }
        public void SelectProject(bool showWait = false)
        {
            if (httpClientHelper is null)
            {
                SharedMethods.NotLoggedIn();
                return;
            }
            SelectProjectDialog dlg = new SelectProjectDialog();
            dlg.HttpClientHelper = httpClientHelper;
            dlg.Owner = this;
            dlg.ShowDialog();
            SelectedProject = dlg.LcsProject;
            if (SelectedProject != null)
            {
                if(showWait == true)
                {
                    Navigation.Navigation.Navigate(new Uri("Views/WaitPage.xaml", UriKind.RelativeOrAbsolute));
                }
                httpClientHelper.ChangeLcsProjectId(SelectedProject.Id.ToString(CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name)));
                RefreshAllInstances();
            }
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
        static void ToggleConfigEncryption(string exeConfigName)
        {
            // Takes the executable file name without the
            // .config extension.
            try
            {
                // Open the configuration file and retrieve 
                // the connectionStrings section.
                Configuration config = ConfigurationManager.OpenExeConfiguration(exeConfigName);

                ConnectionStringsSection section =
                    config.GetSection("LCSCarousel.Properties.Settings")
                    as ConnectionStringsSection;


                if (section.SectionInformation.IsProtected)
                {
                    // Remove encryption.
                    section.SectionInformation.UnprotectSection();
                }
                else
                {
                    // Encrypt the section.
                    section.SectionInformation.ProtectSection(
                        "DataProtectionConfigurationProvider");
                }
                // Save the current configuration.
                config.Save();

                Console.WriteLine("Protected={0}",
                    section.SectionInformation.IsProtected);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {

//            ToggleConfigEncryption("LCSSimpleUtil.exe");

            Instances = JsonConvert.DeserializeObject<List<ProjectInstance>>(Properties.Settings.Default.instances) ?? new List<ProjectInstance>();
            Projects = JsonConvert.DeserializeObject<List<LcsProject>>(Properties.Settings.Default.projects) ?? new List<LcsProject>();
            AllVMsList = JsonConvert.DeserializeObject<List<CloudHostedInstance>>(Properties.Settings.Default.AllWMs) ?? new List<CloudHostedInstance>();

            GetMenuItems();

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
        private async void RefreshCloudHosted(bool reloadFromLcs = true)
        {
            bool t = await Task.Run(() => performReloadCloudHosted(reloadFromLcs)).ConfigureAwait(true);
        }
        private bool performReloadCloudHosted(bool reloadFromLcs = true)
        {
            if (reloadFromLcs)
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
                    Properties.Settings.Default.Save();

                }
            }
            else
            {
                var projectInstance = Instances?.FirstOrDefault(x => x.LcsProjectId.Equals(SelectedProject.Id));
                if (projectInstance != null)
                    CloudHostedInstancesList = projectInstance.CheInstances;
            }
            AddToAllWMsList(CloudHostedInstancesList);
            ClearCloudHostStatus = true;
            return ClearCloudHostStatus;

        }
        private async void ClearAllWmsList()
        {
            bool t = await Task.Run(() => ClearVM()).ConfigureAwait(true);
        }
        private bool ClearVM()
        {
            AllVMsList.Clear();
            ClearVMStatus = true;
            return ClearVMStatus;
        }
        private bool performReloadSaas(bool reloadFromLcs = true)
        {
            if (reloadFromLcs)
            {
                SaasInstancesList = httpClientHelper.GetSaasInstances();

                if (SaasInstancesList != null)
                {
                    if (Instances.Exists(x => x.LcsProjectId == SelectedProject.Id))
                    {
                        Instances.Where(x => x.LcsProjectId == SelectedProject.Id)
                            .Select(x => { x.SaasInstances = SaasInstancesList; return x; })
                                .ToList();
                    }

                    Properties.Settings.Default.instances = JsonConvert.SerializeObject(Instances, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
                    Properties.Settings.Default.Save();
                }
            }
            else
            {
                var projectInstance = Instances?.FirstOrDefault(x => x.LcsProjectId.Equals(SelectedProject.Id));
                if (projectInstance != null)
                    SaasInstancesList = projectInstance.SaasInstances;
            }
            AddToAllWMsList(SaasInstancesList);
            ClearSaaSStatus = true;
            return ClearSaaSStatus;
        }
        private async void RefreshSaas(bool reloadFromLcs = true)
        {
            bool t = await Task.Run(() => performReloadSaas(reloadFromLcs)).ConfigureAwait(true);
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
            return true;
        }
        internal async void RefreshAllInstances()
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

                ClearAllWmsList();
                RefreshCloudHosted();
                RefreshSaas();

                while (true)
                {
                    string clearProgress = "In Progress";
                    string clearSaaSProgress = clearProgress;
                    string clearCloudProgress = clearProgress;

                    if (ClearVMStatus == true)
                    {
                        clearProgress = "Done";
                    }
                    if (ClearSaaSStatus == true)
                    {
                        clearSaaSProgress = "Done";
                    }
                    if (ClearCloudHostStatus == true)
                    {
                        clearCloudProgress = "Done";
                    }

                    message = string.Format(Properties.Resources.RefreshProgress, clearProgress, Environment.NewLine, clearCloudProgress, Environment.NewLine, clearSaaSProgress);
                    controller.SetMessage(message);

                    if (controller.IsCanceled)
                        break; //canceled progressdialog auto closes.


                    if (ClearSaaSStatus == true && ClearVMStatus == true && ClearCloudHostStatus == true)
                    {
                        Properties.Settings.Default.AllWMs = JsonConvert.SerializeObject(AllVMsList, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
                        Properties.Settings.Default.Save();

                        break;
                    }
                    await Task.Delay(2000).ConfigureAwait(true);
                }

                await controller.CloseAsync().ConfigureAwait(true);
                EnableMenuOptions(true);
                Navigation.Navigation.Navigate(new Uri("Views/MainPage.xaml", UriKind.RelativeOrAbsolute));
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
               
                RDPConnectionDetails rdpEntry = getRDPEntry(selectedInstance);

                if (rdpEntry != null)
                {
                    if (Properties.Settings.Default.MimimizeOnStartRDP == true)
                    {
                        this.WindowState = WindowState.Minimized;
                    }
                    using (new RdpCredentials(rdpEntry.Address, $"{rdpEntry.Domain}\\{rdpEntry.Username}", rdpEntry.Password))
                    {
                        var rdcProcess = createProcess(rdpEntry);
                        rdcProcess.Start();
                    }

                }
            }
            return;
        }
        internal RDPConnectionDetails getRDPEntry(CloudHostedInstance selectedInstance)
        {
            RDPConnectionDetails rdpEntry = null;
            bool isSelectedEnvironment = false;

            string environmentId = Properties.Settings.Default.SelectedPersonalVM;
            if(selectedInstance.EnvironmentId == environmentId)
            {
                rdpEntry = getDefaultUser();
                isSelectedEnvironment = true;
            }

            if (rdpEntry is null)
            {
                if (selectedInstance != null)
                {
                    List<RDPConnectionDetails> rdpList = null;
                    using (new WaitCursor())
                    {
                        rdpList = httpClientHelper.GetRdpConnectionDetails(selectedInstance);
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
        internal Process createProcess(RDPConnectionDetails rdpEntry)
        {
            Process process = new Process();

            ProcessStartInfo startInfo = new ProcessStartInfo();

            startInfo.FileName = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\system32\mstsc.exe");

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
                    string resolution = getResolution();
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

        private string getResolution()
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
                        var tasks = new List<Task>();
                        tasks.Add(Task.Run(() => new HttpClientHelper(Cookies) { LcsUrl = _lcsUrl, LcsUpdateUrl = _lcsUpdateUrl, LcsDiagUrl = _lcsDiagUrl, LcsProjectId = SelectedProject.Id.ToString() }.StartStopDeployment(selectedInstance, action)));
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
                AddFirewallExceptionDialog dlg = new AddFirewallExceptionDialog();
                dlg.selectedInstance = selectedInstance;
                dlg.Owner = this;
                dlg.ShowDialog();
            }
        }
        internal void FindAvailableHotfixes(string environmentId)
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
                //RemoveFirewallException dlg = new RemoveFirewallException();
                //dlg.selectedInstance = selectedInstance;
                //dlg.Owner = this;
                //dlg.ShowDialog();
                //if (dlg.NSGRule != null)
                //{
                //    ConfirmRemoval(dlg.NSGRule, selectedInstance);
                //}
            }
        }
        private async void getPackages(CloudHostedInstance _instance)
        {
            bool t = await Task.Run(() => performGetPackages(_instance)).ConfigureAwait(true);
        }
        private bool performGetPackages(CloudHostedInstance _instance)
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
                getPackages(selectedInstance);
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
                DeployPackageDialog dlg = new DeployPackageDialog();
                dlg.Owner = this;
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
                RemoveFirewallException dlg = new RemoveFirewallException();
                dlg.selectedInstance = selectedInstance;
                dlg.Owner = this;
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
                    using (new WaitCursor())
                    {
                        var tasks = new List<Task<string>>();

                        tasks.Add(Task.Run(() => httpClientHelper.DeleteNsgRule(selectedInstance, nSGRule.Name)));
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
                    var tasks = new List<Task>();
                    tasks.Add(Task.Run(() => httpClientHelper.AddNsgRule(selectedInstance, name, address)));
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
            if (httpClientHelper is null) return null;

            return httpClientHelper.GetNetworkSecurityGroup(selectedInstance);
        }
        private void GetMenuItems()
        {
            ShellViewModel viewmodel = (ShellViewModel)this.DataContext;

            //myRDPMenuItem = viewmodel.GetItem(new Uri("Views/MyRDPPage.xaml", UriKind.RelativeOrAbsolute)) as ViewModels.MenuItem;
            myCloudHostedMenuItem = viewmodel.GetItem(new Uri("Views/CloudHostedMachinePage.xaml", UriKind.RelativeOrAbsolute)) as ViewModels.MenuItem;
            myMSHostedMenuItem = viewmodel.GetItem(new Uri("Views/MSHostedPage.xaml", UriKind.RelativeOrAbsolute)) as ViewModels.MenuItem;
            mySettingsMenuItem = viewmodel.GetOptionsItem(new Uri("Views/SettingsPage.xaml", UriKind.RelativeOrAbsolute)) as ViewModels.MenuItem;
        }
        internal void EnableCloudHosted(bool enable)
        {
            myCloudHostedMenuItem.IsEnabled = enable;
        }
        internal void EnableMSHosted(bool enable)
        {
            myMSHostedMenuItem.IsEnabled = enable;
        }
        public void EnableMenuOptions(bool enable)
        {
            //myRDPMenuItem.IsEnabled = enable;
            myCloudHostedMenuItem.IsEnabled = enable;
            myMSHostedMenuItem.IsEnabled = enable;
            mySettingsMenuItem.IsEnabled = enable;
            if(CloudHosted == null || CloudHosted.Count == 0)
            {
                myCloudHostedMenuItem.IsEnabled = false;
            }
            if(MSHosted == null || MSHosted.Count == 0)
            {
                myMSHostedMenuItem.IsEnabled = false;
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
            if(loggedInUri == null)
            {
                throw new Exception(Properties.Resources.NullURI);
            }
            LoggedInUri = loggedInUri;
            Properties.Settings.Default.LoggedInUri = LoggedInUri.ToString();
            Properties.Settings.Default.Save();
        }
        public Uri GetLoggedInUri()
        {
            return LoggedInUri;
        }
        internal void goToLogoutPage()
        {
            Navigation.Navigation.Navigate(new Uri("Views/LogOutPage.xaml", UriKind.RelativeOrAbsolute));

        }
        internal void clearAndClose()
        {
            Properties.Settings.Default.projects = "";
            Properties.Settings.Default.instances = "";
            Properties.Settings.Default.cookie = "";
            Properties.Settings.Default.AllWMs = "";
            Properties.Settings.Default.SelectedPersonalVM = "";
            Properties.Settings.Default.instanceattributes = "";
            Properties.Settings.Default.Save();

            ClearLocalList();
            Application.Current.MainWindow.Close();
        }
        internal void setPersonalVM(string environmentId)
        {
            Properties.Settings.Default.SelectedPersonalVM = environmentId;
            Properties.Settings.Default.Save();
        }
        internal void setDefaultUser(RDPConnectionDetails selectedUser)
        {
            Properties.Settings.Default.DefaultUser = JsonConvert.SerializeObject(selectedUser, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
            Properties.Settings.Default.Save();
        }
        internal RDPConnectionDetails getDefaultUser()
        {
            RDPConnectionDetails connectionDetails;
            connectionDetails = JsonConvert.DeserializeObject<RDPConnectionDetails>(Properties.Settings.Default.DefaultUser);

            return connectionDetails;
        }
    }

    public class WaitCursor : IDisposable
    {
        private Cursor _previousCursor;

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

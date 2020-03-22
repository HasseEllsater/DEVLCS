using LCSCarousel.Classes;
using LCSCarousel.Model;
using LCSCarousel.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LCSCarousel.ViewModels
{
    public class CredentialsViewModel : BindableBase 
    {

        private System.Collections.ObjectModel.ObservableCollection<Model.UserCredentials> _credentials;
        private Model.UserCredentials _selectedCredential;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>")]
        public System.Collections.ObjectModel.ObservableCollection<Model.UserCredentials> UserCredentials
        {
            get { return this._credentials; }
            set { this.SetProperty(ref this._credentials, value); }
        }
        public Model.UserCredentials UserCredential
        {
            get { return this._selectedCredential; }
            set { this.SetProperty(ref this._selectedCredential, value); }
        }
        public CredentialsViewModel(CloudHostedViewModel CloudHostedViewModel)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            UserCredentials = new System.Collections.ObjectModel.ObservableCollection<Model.UserCredentials>();

            var viewModel = CloudHostedViewModel;
            if(viewModel != null)
            {
                if (viewModel.SelectedRDPTerminal != null)
                {
                    var credentials = new Dictionary<string, string>();
                    var instance = viewModel.SelectedRDPTerminal;
                    foreach (var vm in instance.Instances)
                    {
                        var creds = mainWindow.GetCredentials(instance.EnvironmentId, vm.ItemName);
                        credentials = credentials.Concat(creds).GroupBy(d => d.Key).ToDictionary(d => d.Key, d => d.First().Value);
                    }
                    foreach (var cred in instance.SqlAzureCredentials.Select(x => x.DeploymentItemName).Distinct())
                    {
                        var creds = mainWindow.GetCredentials(instance.EnvironmentId, cred);
                        credentials = credentials.Concat(creds).GroupBy(d => d.Key).ToDictionary(d => d.Key, d => d.First().Value);
                    }

                    foreach (var value in credentials)
                    {
                        UserCredentials.Add(new Model.UserCredentials()
                        {
                            EnvironmentId = instance.EnvironmentId,
                            UserId = value.Key,
                            Password = value.Value
                        });

                    }
                    UserCredentials.BubbleSort();
                }
            }
        }
        public CredentialsViewModel(MSHostedViewModel MsHostedViewModel)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            UserCredentials = new System.Collections.ObjectModel.ObservableCollection<Model.UserCredentials>();

            var viewModel = MsHostedViewModel;
            if(viewModel != null)
            {
                if (viewModel.SelectedRDPTerminal != null)
                {
                    var credentials = new Dictionary<string, string>();
                    var instance = viewModel.SelectedRDPTerminal;
                    foreach (var vm in instance.Instances)
                    {
                        var creds = mainWindow.GetCredentials(instance.EnvironmentId, vm.ItemName);
                        credentials = credentials.Concat(creds).GroupBy(d => d.Key).ToDictionary(d => d.Key, d => d.First().Value);
                    }
                    foreach (var cred in instance.SqlAzureCredentials.Select(x => x.DeploymentItemName).Distinct())
                    {
                        var creds = mainWindow.GetCredentials(instance.EnvironmentId, cred);
                        credentials = credentials.Concat(creds).GroupBy(d => d.Key).ToDictionary(d => d.Key, d => d.First().Value);
                    }

                    foreach (var value in credentials)
                    {
                        UserCredentials.Add(new Model.UserCredentials()
                        {
                            EnvironmentId = instance.EnvironmentId,
                            UserId = value.Key,
                            Password = value.Value
                        });

                    }
                    UserCredentials.BubbleSort();
                }
            }
        }
        public CredentialsViewModel(RDPTerminal terminal)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            UserCredentials = new System.Collections.ObjectModel.ObservableCollection<Model.UserCredentials>();
            try
            {
                if (terminal != null)
                {
                    var credentials = new Dictionary<string, string>();
                    var instance = terminal;
                    foreach (var vm in instance.Instances)
                    {
                        var creds = mainWindow.GetCredentials(instance.EnvironmentId, vm.ItemName);
                        credentials = credentials.Concat(creds).GroupBy(d => d.Key).ToDictionary(d => d.Key, d => d.First().Value);
                    }
                    foreach (var cred in instance.SqlAzureCredentials.Select(x => x.DeploymentItemName).Distinct())
                    {
                        var creds = mainWindow.GetCredentials(instance.EnvironmentId, cred);
                        credentials = credentials.Concat(creds).GroupBy(d => d.Key).ToDictionary(d => d.Key, d => d.First().Value);
                    }

                    foreach (var value in credentials)
                    {
                        UserCredentials.Add(new Model.UserCredentials()
                        {
                            EnvironmentId = instance.EnvironmentId,
                            UserId = value.Key,
                            Password = value.Value
                        });

                    }
                    UserCredentials.BubbleSort();
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }

}

using LCSCarousel;
using LCSCarousel.Classes;
using MahApps.Metro.Controls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LCSCarousel.Views
{
    /// <summary>
    /// Interaction logic for SelectProjectDialog.xaml
    /// </summary>
    public partial class SelectProjectDialog : MetroWindow
    {
        private List<LcsProject> Projects;
        internal HttpClientHelper HttpClientHelper { get; set; }
        const int NotSelected = 0;
        internal LcsProject LcsProject { get; private set; }

        public SelectProjectDialog()
        {
            InitializeComponent();
            this.BorderThickness = new Thickness(0);
            this.BorderBrush = null;
            this.GlowBrush = Brushes.Black;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if(ProjectsGrid.SelectedItems.Count > NotSelected)
            {
                if(ProjectsGrid.SelectedItem is LcsProject)
                {
                    LcsProject = ProjectsGrid.SelectedItem as LcsProject;
                    UpdateProjectLinks();
                }

            }
            this.Close();
        }
        private void UpdateProjectLinks()
        {
            var projectDetails = HttpClientHelper.GetProject(LcsProject.Id.ToString(CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name)));
            if (projectDetails != null)
            {
                var project = Projects.FirstOrDefault(prj => prj.Id == LcsProject.Id);
                project.SharepointSite = projectDetails.Settings.SharepointSite;
                project.TfsServerSite = projectDetails.Settings.TfsServerSite;
                project.TfsProjectName = projectDetails.Settings.TfsProjectName;
                Properties.Settings.Default.projects = JsonConvert.SerializeObject(Projects, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
                Properties.Settings.Default.Save();

                LcsProject.SharepointSite = projectDetails.Settings.SharepointSite;
                LcsProject.TfsServerSite = projectDetails.Settings.TfsServerSite;
                LcsProject.TfsProjectName = projectDetails.Settings.TfsProjectName;

                MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                mainWindow.AddInstance(project.Id);

            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Projects = JsonConvert.DeserializeObject<List<LcsProject>>(Properties.Settings.Default.projects);
            if (Projects != null)
            {
                ProjectsGrid.ItemsSource = Projects.OrderBy(f => f.Favorite).ThenBy(i => i.Id).Reverse();

            }
            else
            {
                Refresh_Click(null, null);
            }
            //TODO: Ska jag ha favoriter?
            //Projects = JsonConvert.DeserializeObject<List<LcsProject>>(Properties.Settings.Default.projects);
            //if (Projects != null)
            //{
            //    ProjectsGrid.ItemsSource = Projects.OrderBy(f => f.Favorite).ThenBy(i => i.Id).Reverse();
            //}
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshProjectsList();
        }
        private void RefreshProjectsList()
        {
            Projects = HttpClientHelper.GetAllProjects();
            ProjectsGrid.ItemsSource = Projects.OrderBy(f => f.Favorite).ThenBy(i => i.Id).Reverse();
        }
    }
}

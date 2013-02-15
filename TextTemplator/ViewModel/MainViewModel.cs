using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.Shell;
using ScottIsAFool.WindowsPhone.IsolatedStorage;
using TextTemplator.Model;
using Microsoft.Phone.Controls;

namespace TextTemplator.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly INavigationService navigationService;
        private bool templatesLoaded = false;
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(INavigationService navigation)
        {
            navigationService = navigation;
            Templates = new ObservableCollection<Template>();
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.

                Templates.Add(new Template
                {
                    Id = Guid.NewGuid(),
                    Name = "Long email",
                    Content = "Sorry I can't make it yada yada yada"
                });
                Templates.Add(new Template
                {
                    Id = Guid.NewGuid(),
                    Name = "New SMS",
                    Content = "Hi, looks like I can't make it."
                });
            }
            else
            {
                WireCommands();
            }


        }

        private void WireCommands()
        {
            MainPageLoaded = new RelayCommand(() =>
            {
                ProgressIsVisible = true;
                ProgressText = "Loading...";

                if (!templatesLoaded)
                {
                    Templates = ISettings.GetKeyValue<ObservableCollection<Template>>("TheTemplates") ?? new ObservableCollection<Template>();
                    templatesLoaded = true;
                }

                ProgressText = string.Empty;
                ProgressIsVisible = false;
            });

            AddNewTemplate = new RelayCommand(() =>
            {
                var newTemplate = new Template
                {
                    Id = Guid.NewGuid()
                };
                Templates.Add(newTemplate);
                SelectedTemplate = Templates.Last();
                navigationService.NavigateToPage("/Views/TemplateView.xaml");
            });

            ItemTapped = new RelayCommand<Template>(template =>
            {
                SelectedTemplate = template;
            });
            CopyContentCommand = new RelayCommand(() =>
            {
                if (SelectedTemplate != null)
                {
                    Clipboard.SetText(SelectedTemplate.Content);
                    App.ShowMessage("", "Copied to your clipboard");
                }
            });
            PinTemplateCommand = new RelayCommand<Template>(template =>
            {
                if (template == null)
                {
                    template = SelectedTemplate;
                }

                var tileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().EndsWith(template.Id.ToString()));
                if (tileToFind != null)
                {
                    App.ShowMessage("", "This template is already pinned");
                    return;
                }

                var newTile = new StandardTileData
                {
                    Title = template.Name,
                    BackgroundImage = new Uri("isostore:/Shared/ShellContent/Background.png", UriKind.Absolute)
                };
                ShellTile.Create(new Uri("/Views/PinnedTemplate.xaml?id=" + template.Id, UriKind.Relative), newTile);
            });

            DeleteItemCommand = new RelayCommand<Template>(template =>
            {
                bool shouldGoBack = false;
                if (template == null)
                {
                    template = SelectedTemplate;
                    shouldGoBack = true;
                }
                var messageBox = new CustomMessageBox
                {
                    Caption = "Delete item?",
                    Message = "Are you sure you wish to delete this item?",
                    LeftButtonContent = "yes",
                    RightButtonContent = "no",
                    IsFullScreen = false
                };
                messageBox.Dismissed += (sender, args) =>
                {
                    if (args.Result == CustomMessageBoxResult.LeftButton)
                    {
                        Templates.Remove(template);
                    }
                };
                messageBox.Show();
            });
            NavigateToPage = new RelayCommand<string>(navigationService.NavigateToPage);
        }

        public ObservableCollection<Template> Templates { get; set; }

        public string ProgressText { get; set; }
        public bool ProgressIsVisible { get; set; }

        public Template SelectedTemplate { get; set; }

        public RelayCommand MainPageLoaded { get; set; }
        public RelayCommand AddNewTemplate { get; set; }
        public RelayCommand<Template> ItemTapped { get; set; }
        public RelayCommand CopyContentCommand { get; set; }
        public RelayCommand<Template> PinTemplateCommand { get; set; }
        public RelayCommand<Template> DeleteItemCommand { get; set; }
        public RelayCommand<string> NavigateToPage { get; set; }
    }
}
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using ScottIsAFool.WindowsPhone.IsolatedStorage;
using TextTemplator.Model;

namespace TextTemplator.Views
{
    public partial class PinnedTemplate : PhoneApplicationPage
    {
        private Guid templateId;
        public PinnedTemplate()
        {
            InitializeComponent();
            Loaded += (sender, args) =>
                          {
                              var templates = ISettings.GetKeyValue<ObservableCollection<Template>>("TheTemplates");
                              var template = templates.SingleOrDefault(x => x.Id == templateId);
                              if (template != null)
                              {
                                  Clipboard.SetText(template.Content);
                              }
                              App.Current.Terminate();
                          };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string id = "";
            if(NavigationContext.QueryString.TryGetValue("id", out id))
            {
                templateId = Guid.Parse(id);
            }
        }
    }
}
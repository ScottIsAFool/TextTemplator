using System;
using System.Windows.Navigation;

namespace TextTemplator.Model
{
    public interface INavigationService
    {
        event NavigatingCancelEventHandler Navigating;
        void NavigateTo(Uri pageUri);
        void NavigateTo(string pageUri);
        void GoBack();
        void NavigateToPage(string link);
    }
}

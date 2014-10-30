using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;

namespace Veloose.ViewModel
{
    public class ViewModelLocator
    {
        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<MainViewModel>();
            }
            else
            {
                SimpleIoc.Default.Register<MainViewModel>();
            }
            SimpleIoc.Default.Register<INavigationService>(() =>
            {
                var nav = new NavigationService();
                nav.Configure(Constants.MainPageName, typeof(MainPage));
                nav.Configure(Constants.DetailsPageName, typeof(Details));
                return nav;
            });
            SimpleIoc.Default.Register<MainViewModel>();
        }

        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<MainViewModel>();
            
        }
    }
}
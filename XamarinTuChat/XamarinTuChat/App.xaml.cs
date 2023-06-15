using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinTuChat.Views;

namespace XamarinTuChat
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            //Application.Current.Properties.Remove("userId");
            //Application.Current.Properties.Remove("username");

            // если свойство username существует главной страницей выберается AppShell иначе главной страницей становится AuthorizationPage (страница авторизации)
            if (Current.Properties.ContainsKey("username"))
            {
                MainPage = new AppShell();
            }
            else
            {
                MainPage = new NavigationPage(new AuthorizationPage())
                {
                    BarBackgroundColor = Color.FromHex("#0e6049"),
                    BarTextColor = Color.FromHex("#02a572")
                };
            }
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}

using System;
using System.Collections.Generic;
using Xamarin.Forms;
using XamarinTuChat.Views;

namespace XamarinTuChat
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public string userId;

        public AppShell()
        {
            InitializeComponent();

            // проверка существования свойства username в глобальном словаре
            if (Application.Current.Properties.ContainsKey("username"))
            {
                // присваивание значения имени пользователя из словаря в usernameText
                usernameText.Title = Application.Current.Properties["username"] as string;
            }
        }

        //private async void OnMenuItemClicked(object sender, EventArgs e)
        //{
        //    await Shell.Current.GoToAsync("//LoginPage");
        //}

        // метод для получения данных из другого класса
        public void GetData(string id, string username)
        {
            userId = id;
            usernameText.Title = username;
        }
    }
}

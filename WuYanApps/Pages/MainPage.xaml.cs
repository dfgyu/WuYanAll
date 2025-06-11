using WuYanApps.Models;
using WuYanApps.PageModels;

namespace WuYanApps.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}
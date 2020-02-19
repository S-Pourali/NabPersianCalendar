using System.ComponentModel;
using Xamarin.Forms;

namespace NabPersianDatePicker
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new NabPersianDateViewModel();
        }

        #region PersianDateText

        public string PersianDate
        {
            get => (string)GetValue(PersianDateProperty);
            set => SetValue(PersianDateProperty, value);
        }

        public static readonly BindableProperty PersianDateProperty =
            BindableProperty.Create(
                nameof(PersianDate),
                typeof(string),
                typeof(MainPage),
                string.Empty);

        #endregion PersianDateText
    }
}
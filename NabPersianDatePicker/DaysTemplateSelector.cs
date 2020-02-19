using Xamarin.Forms;

namespace NabPersianDatePicker
{
    public class DaysTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DayNotExistTemplate { get; set; }
        public DataTemplate NonVacationDayTemplate { get; set; }
        public DataTemplate VacationDayTemplate { get; set; }

        public DataTemplate TodayDayTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var day = ((NabPersianDateViewModel.DaySource)item).DayName;

            if (string.IsNullOrWhiteSpace(day))
                return DayNotExistTemplate;

            if (((NabPersianDateViewModel.DaySource)item).IsToday ?? false)
                return TodayDayTemplate;

            return day == "جمعه" ? VacationDayTemplate : NonVacationDayTemplate;
        }
    }
}
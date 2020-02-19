using MvvmCross.Commands;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace NabPersianDatePicker
{
    /// <summary>
    /// Due to handle view items on UI,grouping days by day name and declared an item source to each them.
    /// it's can be done with only one item source to make days in month but it should be fluent desing on UI.
    /// </summary>
    public class NabPersianDateViewModel : MvvmCross.ViewModels.MvxViewModel
    {
        public NabPersianDateViewModel()
        {
            PopulateData();
        }

        #region Properties

        private PersianCalendar PersianCalendar => new PersianCalendar();
        private DateTime GregorianCalendar => new DateTime();

        private DateTime _GeneratedDate;

        public DateTime GenereatedDate
        {
            get { return _GeneratedDate; }
            set
            {
                if (_GeneratedDate != value)
                {
                    _GeneratedDate = value;
                    RaisePropertyChanged(nameof(GenerateDate));
                }
            }
        }

        private string _GeneratedDateShamsi;

        public string GeneratedDateShamsi
        {
            get { return _GeneratedDateShamsi; }
            set
            {
                if (_GeneratedDateShamsi != value)
                {
                    _GeneratedDateShamsi = value;
                    RaisePropertyChanged(nameof(GeneratedDateShamsi));
                }
            }
        }

        public string GeneratedPersianDate { get; set; }
        public DateTime GeneratedGregorianDate { get; set; }

        private int CurrentMonth
        {
            get
            {
                return PersianCalendar.GetMonth(DateTime.Now);
            }
        }

        #endregion Properties

        #region YearRegion

        private YearSource _SelectedYear;

        public YearSource SelectedYear
        {
            get { return _SelectedYear; }
            set
            {
                _SelectedYear = value;
                if (_SelectedYear != value)
                {
                    _SelectedYear = value;
                    RaisePropertyChanged(nameof(SelectedYear));
                }
            }
        }

        private ObservableCollection<YearSource> _Years;

        public ObservableCollection<YearSource> Years
        {
            get
            {
                if (_Years == null)
                {
                    _Years = new ObservableCollection<YearSource>();
                    GetYear();
                }
                return _Years;
            }
        }

        private void GetYear()
        {
            Years.Clear();
            for (var yearNo = 1380; yearNo < 1500; yearNo++)
            {
                _Years.Add(new YearSource(yearNo));
            }
            _SelectedYear = _Years.FirstOrDefault(f => f.YearNo == PersianCalendar.GetYear(DateTime.Now));
            RaisePropertyChanged(nameof(Years));
            RaisePropertyChanged(nameof(SelectedYear));
        }

        #endregion YearRegion

        #region MonthRegion

        private MonthSource _SelectedMonth;

        public MonthSource SelectedMonth
        {
            get { return _SelectedMonth; }
            set
            {
                if (_SelectedMonth != value)
                {
                    _SelectedMonth = value;
                    RaisePropertyChanged(nameof(SelectedMonth));
                }
            }
        }

        private ObservableCollection<MonthSource> _Months;

        public ObservableCollection<MonthSource> Months
        {
            get
            {
                if (_Months == null)
                {
                    _Months = new ObservableCollection<MonthSource>();
                    GetMonth();
                }
                return _Months;
            }
        }

        private void GetMonth()
        {
            Months.Clear();
            for (var monthNo = 1; monthNo < 13; monthNo++)
            {
                var monthName = GetMonthName(monthNo);

                var month = new MonthSource(false, monthNo, monthName, monthNo < 10 ? "0" + monthNo : monthNo.ToString());
                _Months.Add(month);
            }
            _SelectedMonth = _Months.FirstOrDefault(f => f.MonthNo == PersianCalendar.GetMonth(DateTime.Now));

            RaisePropertyChanged(nameof(Months));
            RaisePropertyChanged(nameof(SelectedMonth));
        }

        private string GetMonthName(int monthNo)
        {
            switch (monthNo)
            {
                case 1:
                    return "فروردین";

                case 2:
                    return "اردیبهشت";

                case 3:
                    return "خرداد";

                case 4:
                    return "تیر";

                case 5:
                    return "مرداد";

                case 6:
                    return "شهریور";

                case 7:
                    return "مهر";

                case 8:
                    return "آبان";

                case 9:
                    return "آذر";

                case 10:
                    return "دی";

                case 11:
                    return "بهمن";

                case 12:
                    return "اسفند";
            }
            return "";
        }

        #endregion MonthRegion

        #region DayRegion

        private ObservableCollection<DaySource> _Days;

        public ObservableCollection<DaySource> Days
        {
            get
            {
                if (_Days == null)
                    _Days = new ObservableCollection<DaySource>();
                return _Days;
            }
        }

        private DaySource _SelectedDay;

        public DaySource SelectedDay
        {
            get { return _SelectedDay; }
            set
            {
                if (_SelectedDay != value)
                {
                    _SelectedDay = value;
                    RaisePropertyChanged(nameof(SelectedDay));
                }
            }
        }

        private void GetDaysInMonth()
        {
            Days.Clear();
            try
            {
                var daysInMonth = PersianCalendar.GetDaysInMonth(SelectedYear.YearNo, SelectedMonth.MonthNo);
                for (var day = 1; day <= daysInMonth; day++)
                {
                    var tmpDate = PersianCalendar.ToDateTime(SelectedYear.YearNo, SelectedMonth.MonthNo, day, 0, 0, 0, 0, 0);
                    var isToday = (SelectedMonth.MonthNo == CurrentMonth && day == PersianCalendar.GetDayOfMonth(DateTime.Now));
                    var dayItem = new DaySource(isToday, day, PersianCalendar.GetDayOfWeek(tmpDate), day < 10 ? "0" + day : day.ToString());

                    _Days.Add(dayItem);
                }

                if (_SelectedDay == null)
                    _SelectedDay = Days.FirstOrDefault(f => f.IsToday == true);

                GroupingDaysByDayName();
                RaisePropertyChanged(nameof(Days));
                RaisePropertyChanged(nameof(SelectedDay));

                GenerateDate();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion DayRegion

        #region Methods

        private void GroupingDaysByDayName()
        {
            _Saturdays = new ObservableCollection<DaySource>();
            _Sundays = new ObservableCollection<DaySource>();
            _Mondays = new ObservableCollection<DaySource>();
            _Tuesdays = new ObservableCollection<DaySource>();
            _Wednsdays = new ObservableCollection<DaySource>();
            _Thursdays = new ObservableCollection<DaySource>();
            _Fridays = new ObservableCollection<DaySource>();

            foreach (var day in Days.OrderBy(o => o.DayNoInMonth))
            {
                switch (day.DayName)
                {
                    case "شنبه":
                        _Saturdays.Add(day);
                        break;

                    case "یکشنبه":
                        _Sundays.Add(day);
                        break;

                    case "دوشنبه":
                        _Mondays.Add(day);
                        break;

                    case "سه شنبه":
                        _Tuesdays.Add(day);
                        break;

                    case "چهارشنبه":
                        _Wednsdays.Add(day);
                        break;

                    case "پنج شنبه":
                        _Thursdays.Add(day);
                        break;

                    case "جمعه":
                        _Fridays.Add(day);
                        break;
                }
            }

            SetDayPositionInTheirList();

            RaisePropertyChanged(nameof(Saturdays));
            RaisePropertyChanged(nameof(Sundays));
            RaisePropertyChanged(nameof(Mondays));
            RaisePropertyChanged(nameof(Tuesdays));
            RaisePropertyChanged(nameof(Wednsdays));
            RaisePropertyChanged(nameof(Thursdays));
            RaisePropertyChanged(nameof(Fridays));
        }

        private void PopulateData()
        {
            GetYear();
            GetMonth();
            GetDaysInMonth();

            GenerateDate();
        }

        private void GenerateDate()
        {
            GeneratedGregorianDate = PersianCalendar.ToDateTime(SelectedYear.YearNo, SelectedMonth.MonthNo, SelectedDay.DayNoInMonth ?? 0, 0, 0, 0, 0);
            GeneratedDateShamsi = string.Format("{0}/{1}/{2}", SelectedYear.YearNo, SelectedMonth.DisplayMonthNo, SelectedDay.DisplayDayNo);
        }

        private void SetDayPositionInTheirList()
        {
            switch (_Days.FirstOrDefault(f => f.DayNoInMonth == 1).DayOfWeekInSource)
            {
                case DayOfWeek.Saturday:
                    break;

                case DayOfWeek.Sunday:
                    _Saturdays.Insert(0, new DaySource(null, null, null, string.Empty));
                    break;

                case DayOfWeek.Monday:
                    _Saturdays.Insert(0, new DaySource(null, null, null, string.Empty));
                    _Sundays.Insert(0, new DaySource(null, null, null, string.Empty));
                    break;

                case DayOfWeek.Tuesday:
                    _Saturdays.Insert(0, new DaySource(null, null, null, string.Empty));
                    _Sundays.Insert(0, new DaySource(null, null, null, string.Empty));
                    _Mondays.Insert(0, new DaySource(null, null, null, string.Empty));
                    break;

                case DayOfWeek.Wednesday:
                    _Saturdays.Insert(0, new DaySource(null, null, null, string.Empty));
                    _Sundays.Insert(0, new DaySource(null, null, null, string.Empty));
                    _Mondays.Insert(0, new DaySource(null, null, null, string.Empty));
                    _Tuesdays.Insert(0, new DaySource(null, null, null, string.Empty));
                    break;

                case DayOfWeek.Thursday:
                    _Saturdays.Insert(0, new DaySource(null, null, null, string.Empty));
                    _Sundays.Insert(0, new DaySource(null, null, null, string.Empty));
                    _Mondays.Insert(0, new DaySource(null, null, null, string.Empty));
                    _Tuesdays.Insert(0, new DaySource(null, null, null, string.Empty));
                    _Wednsdays.Insert(0, new DaySource(null, null, null, string.Empty));
                    break;

                case DayOfWeek.Friday:
                    _Saturdays.Insert(0, new DaySource(null, null, null, string.Empty));
                    _Sundays.Insert(0, new DaySource(null, null, null, string.Empty));
                    _Mondays.Insert(0, new DaySource(null, null, null, string.Empty));
                    _Tuesdays.Insert(0, new DaySource(null, null, null, string.Empty));
                    _Wednsdays.Insert(0, new DaySource(null, null, null, string.Empty));
                    _Thursdays.Insert(0, new DaySource(null, null, null, string.Empty));
                    break;
            }
        }

        #endregion Methods

        #region Commands

        private IMvxAsyncCommand<string> _YearChangedCommand;
        private IMvxAsyncCommand<string> _MonthChangedCommand;
        private IMvxAsyncCommand<DaySource> _DaySelectedCommand;

        public IMvxAsyncCommand<string> YearChangedCommand
        {
            get
            {
                return _YearChangedCommand = new MvxAsyncCommand<string>((parameter) =>
                {
                    try
                    {
                        var currentYear = SelectedYear;
                        switch (parameter)
                        {
                            case "NextYear":
                                if (currentYear.YearNo > 1500)
                                    return Task.CompletedTask;
                                _SelectedYear = Years.FirstOrDefault(f => f.YearNo == currentYear.YearNo + 1);
                                break;

                            case "PastYear":
                                if (currentYear.YearNo < 1380)
                                    return Task.CompletedTask;
                                _SelectedYear = Years.FirstOrDefault(f => f.YearNo == currentYear.YearNo - 1);
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        GetDaysInMonth();
                        RaisePropertyChanged(nameof(SelectedYear));
                    }
                    return Task.CompletedTask;
                });
            }
        }

        public IMvxAsyncCommand<string> MonthChangedCommand
        {
            get
            {
                return _MonthChangedCommand = new MvxAsyncCommand<string>((parameter) =>
                {
                    try
                    {
                        var currentMonth = SelectedMonth;
                        switch (parameter)
                        {
                            case "NextMonth":
                                if (currentMonth.MonthNo > 11)
                                    return Task.CompletedTask;
                                _SelectedMonth = Months.FirstOrDefault(f => f.MonthNo == currentMonth.MonthNo + 1);
                                break;

                            case "PastMonth":
                                if (currentMonth.MonthNo < 2)
                                    return Task.CompletedTask;
                                _SelectedMonth = Months.FirstOrDefault(f => f.MonthNo == currentMonth.MonthNo - 1);
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        GetDaysInMonth();
                        RaisePropertyChanged(nameof(SelectedMonth));
                    }
                    return Task.CompletedTask;
                });
            }
        }

        public IMvxAsyncCommand<DaySource> DaySelectedCommand
        {
            get
            {
                return _DaySelectedCommand = new MvxAsyncCommand<DaySource>((parameter) =>
                {
                    try
                    {
                        _SelectedDay = parameter;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        RaisePropertyChanged(nameof(SelectedDay));
                        GenerateDate();
                    }
                    return Task.CompletedTask;
                });
            }
        }

        #endregion Commands

        #region Sources

        private ObservableCollection<DaySource> _Saturdays;

        public ObservableCollection<DaySource> Saturdays
        {
            get
            {
                if (_Saturdays == null)
                    _Saturdays = new ObservableCollection<DaySource>();
                return _Saturdays;
            }
        }

        private ObservableCollection<DaySource> _Sundays;

        public ObservableCollection<DaySource> Sundays
        {
            get
            {
                if (_Sundays == null)
                    _Sundays = new ObservableCollection<DaySource>();
                return _Sundays;
            }
        }

        private ObservableCollection<DaySource> _Mondays;

        public ObservableCollection<DaySource> Mondays
        {
            get
            {
                if (_Mondays == null)
                    _Mondays = new ObservableCollection<DaySource>();
                return _Mondays;
            }
        }

        private ObservableCollection<DaySource> _Tuesdays;

        public ObservableCollection<DaySource> Tuesdays
        {
            get
            {
                if (_Tuesdays == null)
                    _Tuesdays = new ObservableCollection<DaySource>();
                return _Tuesdays;
            }
        }

        private ObservableCollection<DaySource> _Wednsdays;

        public ObservableCollection<DaySource> Wednsdays
        {
            get
            {
                if (_Wednsdays == null)
                    _Wednsdays = new ObservableCollection<DaySource>();
                return _Wednsdays;
            }
        }

        private ObservableCollection<DaySource> _Thursdays;

        public ObservableCollection<DaySource> Thursdays
        {
            get
            {
                if (_Thursdays == null)
                    _Thursdays = new ObservableCollection<DaySource>();
                return _Thursdays;
            }
        }

        private ObservableCollection<DaySource> _Fridays;

        public ObservableCollection<DaySource> Fridays
        {
            get
            {
                if (_Fridays == null)
                    _Fridays = new ObservableCollection<DaySource>();
                return _Fridays;
            }
        }

        public class DaySource
        {
            public bool? IsToday { get; set; }
            public int? DayNoInMonth { get; set; }

            public string DisplayDayNo { get; set; }

            public string DayName { get; set; }

            public DayOfWeek? DayOfWeekInSource { get; set; }

            public DaySource(bool? isToday, int? dayNoInMonth, DayOfWeek? dayNoInWeek, string displayDayNo)
            {
                IsToday = isToday;
                DayNoInMonth = dayNoInMonth;
                DayOfWeekInSource = dayNoInWeek;
                SetDayName(dayNoInWeek);
                DisplayDayNo = displayDayNo;
            }

            private void SetDayName(DayOfWeek? dayNoInWeek)
            {
                if (dayNoInWeek == null)
                    return;

                switch (dayNoInWeek)
                {
                    case DayOfWeek.Sunday:
                        DayName = "یکشنبه";
                        break;

                    case DayOfWeek.Monday:
                        DayName = "دوشنبه";
                        break;

                    case DayOfWeek.Tuesday:
                        DayName = "سه شنبه";
                        break;

                    case DayOfWeek.Wednesday:
                        DayName = "چهارشنبه";
                        break;

                    case DayOfWeek.Thursday:
                        DayName = "پنج شنبه";
                        break;

                    case DayOfWeek.Friday:
                        DayName = "جمعه";
                        break;

                    case DayOfWeek.Saturday:
                        DayName = "شنبه";
                        break;
                }
            }
        }

        public class MonthSource
        {
            public bool MonthIsSelected { get; set; }
            public int MonthNo { get; set; }
            public string DisplayMonthNo { get; set; }
            public string MonthName { get; set; }

            public MonthSource(bool monthIsSelected, int monthNo, string monthName, string displayMonthNo)
            {
                MonthIsSelected = monthIsSelected;
                MonthNo = monthNo;
                MonthName = monthName;
                DisplayMonthNo = displayMonthNo;
            }
        }

        public class YearSource
        {
            public int YearNo { get; set; }

            public YearSource(int yearNo)
            {
                YearNo = yearNo;
            }
        }

        #endregion Sources
    }
}
using System;
using System.Collections.Generic;
using System.Globalization;

namespace ZDGV
{
    class FilterStatus
    {
        public string columnName { get; set; }
        public string valueString { get; set; }
        public bool check { get; set; }
    }

    class FilterStatusComparer : IComparer<FilterStatus>
    {
        public int Compare(FilterStatus x, FilterStatus y) => String.Compare(x.valueString, y.valueString);
    }

    class FilterStatusComparerDateTime : IComparer<FilterStatus>
    {
        public int Compare(FilterStatus x, FilterStatus y)
        {
            DGVWithFilter zdgv = new DGVWithFilter();
            if (x.valueString == y.valueString) return 0;
            if (x.valueString == zdgv.SpaceText) return -1;
            if (y.valueString == zdgv.SpaceText) return 1;
            return DateTime.Compare(this.ToDateTime(x.valueString), this.ToDateTime(y.valueString));
        }

        public DateTime ToDateTime(string datestr)
        {
            if (datestr.Length == 19)
                return DateTime.ParseExact(datestr, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            else
                return DateTime.ParseExact(datestr, "dd.MM.yyyy H:mm:ss", CultureInfo.InvariantCulture);
        }

        public DateTime ToDate(string datestr) => DateTime.ParseExact(datestr.Substring(0, 10), "dd.MM.yyyy", CultureInfo.InvariantCulture);
    }

    class FilterStatusComparerInt : IComparer<FilterStatus>
    {
        public int Compare(FilterStatus x, FilterStatus y) => Int32.Parse(x.valueString).CompareTo(Int32.Parse(y.valueString));
    }
}
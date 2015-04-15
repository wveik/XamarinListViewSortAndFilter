using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace XamarinListViewSortAndFilter {
    public static class ExtMethods {
        public static bool Contains(this string source, string toCheck, StringComparison comparisonType) {
            return (source.IndexOf(toCheck, comparisonType) >= 0);
        }
    }
}
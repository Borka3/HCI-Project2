using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NetworkService.Model
{
    public class DateTimeValue
    {
        public string Time {  get; set; }
        public string Date { get; set; }
        public double Value { get; set; }

        public DateTimeValue(string date, string time, double value)
        {
            Time = time;
            Date = date;
            Value = value;
        }


    }
}

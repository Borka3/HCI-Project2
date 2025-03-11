using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Helpers
{
    public class ComboBoxItems
    {
        public static Dictionary<string, string> entityTypes = new Dictionary<string, string>()
        {
            {"Cable sensor","pack://application:,,,/NetworkService;component/Images/CableSensor.png"},
            {"Digital manometer","pack://application:,,,/NetworkService;component/Images/DigitalManometer.png"}
        };
    }
}

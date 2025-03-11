using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService
{
    public static class SharedData
    {
        public static ObservableCollection<EntitiesByType> AllEntities { get; set; }
    }
}

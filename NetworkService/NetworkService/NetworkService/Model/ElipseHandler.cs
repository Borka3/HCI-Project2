using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkService.Helpers;
using System.Windows.Media;

namespace NetworkService.Model
{
    public class ElipseHandler : BindableBase
    {

        private double cmValue;
        private string cmDate;
        private string cmTime;
        private Brush cmColor;
        private int cmWidthAndHeight;

        public ElipseHandler()
        {
            cmValue = 1;
        }

        public ElipseHandler(double cmValue, string cmDate, string cmTime)
        {
            CmValue = cmValue;
            CmDate = cmDate;
            CmTime = cmTime;
        }

        public double CmValue
        {
            get { return cmValue; }
            set
            {
                cmValue = value;
                CmWidthAndHeight = (int)Math.Round(cmValue * 2);
                if (cmValue >= 5 && cmValue <= 16)
                {
                    CmColor = Brushes.Green;
                }
                else if ((cmValue > 0 && cmValue < 5) || cmValue > 16)
                {
                    CmColor = Brushes.Red;
                }
                else
                {
                    CmColor = Brushes.CadetBlue;
                }
                OnPropertyChanged("CmValue");
            }
        }

        public string CmDate
        {
            get { return cmDate; }
            set
            {
                cmDate = value;
                OnPropertyChanged("CmDate");
            }
        }

        public int CmWidthAndHeight
        {
            get { return cmWidthAndHeight; }
            set
            {
                cmWidthAndHeight = value;
                OnPropertyChanged("CmWidthAndHeight");
            }
        }

        public string CmTime
        {
            get { return cmTime; }
            set
            {
                cmTime = value;
                OnPropertyChanged("CmTime");
            }
        }

        public Brush CmColor
        {
            get { return cmColor; }
            set
            {
                cmColor = value;
                OnPropertyChanged("CmColor");
            }
        }
    }
}

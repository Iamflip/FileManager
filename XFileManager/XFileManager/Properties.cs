using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XFileManager
{
    class Properties
    {
        private int WIDTH = 200;
        private int HEIGHT = 60;
        private int AMOUNT_IN_ONE_PAGE = 42;
        private string LAST_PATH;
        private int INFO_PLACE_START_POSITION = 46;
        private int WRITE_PLACE = 58;
        private int X_PLACE = 1;

        public int Width { get; set; }
        public int Height { get; set; }
        public int AmountInOnePage { get; set; }
        public string LastPath { get; set; }
        public int InfoPlaceStartPosition { get; set; }
        public int WritePlace { get; set; }
        public int xPlace { get; set; }
        public Properties()
        {
            Width = WIDTH;
            Height = HEIGHT;
            AmountInOnePage = AMOUNT_IN_ONE_PAGE;
            LastPath = LAST_PATH;
            InfoPlaceStartPosition = INFO_PLACE_START_POSITION;
            WritePlace = WRITE_PLACE;
            xPlace = X_PLACE;
        }
        public Properties(string LastPath)
        {
            Width = WIDTH;
            Height = HEIGHT;
            AmountInOnePage = AMOUNT_IN_ONE_PAGE;
            this.LastPath = LastPath;
            InfoPlaceStartPosition = INFO_PLACE_START_POSITION;
            WritePlace = WRITE_PLACE;
            xPlace = X_PLACE;
        }
    }
}

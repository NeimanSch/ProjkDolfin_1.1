using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapGen
{
    class Room
    {
        public int xLoc = 0;
        public int yLoc = 0;
        public int width = 0;
        public int height = 0;
        public int dataType = 0;
        public string displayCharacter = ".";

        //Building Specifics
        public int doorX = 0;
        public int doorY = 0;
        public int wallDataType = 1;
        public int floorDataType = 0;
        public string wallDisplayChar = "X";
        public string floorDisplayChar = "+";

        public int XMaxLocation()
        {
            return xLoc + width;
        }

        public int YMaxLocation()
        {
            return yLoc + height;
        }
    }
}

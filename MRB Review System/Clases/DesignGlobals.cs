using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace QualityDefectsRepair
{
    class DesignGlobals
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        public static extern IntPtr CreateRoundRectRgn
     (
       int nLeftRect,     // x-coordinate of upper-left corner
       int nTopRect,      // y-coordinate of upper-left corner
       int nRightRect,    // x-coordinate of lower-right corner
       int nBottomRect,   // y-coordinate of lower-right corner
       int nWidthEllipse, // width of ellipse
       int nHeightEllipse // height of ellipse
     );


        private static bool _MenuExtended = false;

        public static bool MenuExtended
        {
            get
            {
                return _MenuExtended;
            }
            set
            {
                _MenuExtended = value;
            }
        }

        public static Color Green
        {
            get
            {
                return ColorTranslator.FromHtml("#4CAF50");
            }
        }


        public static Color Red
        {
            get
            {
                return ColorTranslator.FromHtml("#F44336");
            }
        }

        public static Color Indigo
        {
            get
            {
                return ColorTranslator.FromHtml("#3F51B5");
            }
        }

        public static Color Indigo900
        {
            get
            {
                return ColorTranslator.FromHtml("#1A237E");
            }
        }

        public static Color Teal
        {
            get
            {
                return ColorTranslator.FromHtml("#009688");
            }
        }

        public static Color White
        {
            get
            {
                return ColorTranslator.FromHtml("#FFFFFF");
            }
        }
    }
}

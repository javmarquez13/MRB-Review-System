using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRB_Review_System
{
    class Globals
    {
        private static string _SERIAL_NUMBER;
        private static string _PROCESS;
        private static string _CRD;
        private static string _DEFECT;
        private static string _STATUS;
        private static bool _FLAG_FAIL;
  

        /// <summary>
        /// Variable to get or set Serial Number from Current Unit
        /// </summary>
        public static string SERIAL_NUMBER
        {
            get
            {
                return _SERIAL_NUMBER;
            }
            set
            {
                _SERIAL_NUMBER = value;
            }
        }

        public static string PROCESS
        {
            get
            {
                return _PROCESS;
            }
            set
            {
                _PROCESS = value;
            }
        }

        public static string CRD
        {
            get
            {
                return _CRD;
            }
            set
            {
                _CRD = value;
            }
        }

        public static string DEFECT
        {
            get
            {
                return _DEFECT;
            }
            set
            {
                _DEFECT = value;
            }
        }

        public static string STATUS
        {
            get
            {
                return _STATUS;
            }
            set
            {
                _STATUS = value;
            }
        }

        public static bool FLAG_FAIL
        {
            get
            {
                return _FLAG_FAIL;
            }
            set
            {
                _FLAG_FAIL = value;
            }
        }


        private static int _CUSTOMER_ID;
        private static int _WIP_ID;
        public static int CUSTOMER_ID
        {
            get
            {
                return _CUSTOMER_ID;
            }
            set
            {
                _CUSTOMER_ID = value;
            }
        }

        public static int WIP_ID
        {
            get
            {
                return _WIP_ID;
            }
            set
            {
                _WIP_ID = value;
            }
        }

        public static string CONFIG_FILE
        {
            get
            {
                return @"\\MXCHIM0REL02\Dexcom\TEApplications\MRB Review System\Config.INI";
            }
        }

        public static string PATH_FILE
        {
            get
            {
                string ExecuteLocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location).ToUpper();

                if (ExecuteLocation.Contains(@"MXCHIM0REL02"))
                {                                   
                    return ConfigFiles.reader("MRB Review System", "UAT_EXCEL_FILE", CONFIG_FILE);
                }
                else
                {
                    return ConfigFiles.reader("MRB Review System", "EXCEL_FILE", CONFIG_FILE);
                }           
            }
        }


        public static string PATH_FILE_ALL
        {
            get
            {
                string ExecuteLocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location).ToUpper();

                if (ExecuteLocation.Contains(@"MXCHIM0REL02"))
                {
                    return ConfigFiles.reader("MRB Review System", "UAT_EXCEL_FILE_ALL", CONFIG_FILE);
                }
                else
                {
                    return ConfigFiles.reader("MRB Review System", "EXCEL_FILE_ALL", CONFIG_FILE);
                }
            }
        }


        private static DataTable _APP_DATA_ALL;
        public static DataTable APP_DATA_ALL
        {
            get
            {
                return _APP_DATA_ALL;
            }

            set
            {
                _APP_DATA_ALL = value;
            }
        }

        private static DataTable _APP_DATA;
        public static DataTable APP_DATA
        {
            get
            {
                return _APP_DATA;
            }

            set
            {
                _APP_DATA = value;
            }
        }

        public static List<String> DATA_MATRIX
        {
            get
            {
                return ConfigFiles.GetKeys("DATA_MATRIX");
            }
        }



        private static int _COUNT_MATRIX;
        public static int COUNT_MATRIX
        {
            get
            {
                return _COUNT_MATRIX;
            }
            set
            {
                _COUNT_MATRIX = value;
            }
        }

        private static string _STEPS_MISSING;
        public static string STEPS_MISSING
        {
            get
            {
                return _STEPS_MISSING;
            }
            set
            {
                _STEPS_MISSING = value;
            }
        }


        public static int TRIGGER
        {
            get
            {
                return Convert.ToInt32(ConfigFiles.reader("MRB Review System", "TRIGGER", CONFIG_FILE));
            }

        }





        private static bool _DOCK_MENU = true;
        public static bool DOCK_MENU
        {
            get
            {
                return _DOCK_MENU;
            }
            set
            {
                _DOCK_MENU = value;
            }
        }



        private static string _PREVIOUS_SN;
        public static string PREVIOUS_SN
        {
            get
            {
                return _PREVIOUS_SN;
            }
            set
            {
                _PREVIOUS_SN = value;
            }
        }


        private static int _QTY_UNITS_SCANNED = 0;
        public static int QTY_UNITS_SCANNED
        {
            get
            {
                return _QTY_UNITS_SCANNED;
            }
            set
            {
                _QTY_UNITS_SCANNED = value;
            }
        }




    }
}

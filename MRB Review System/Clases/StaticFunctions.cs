using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using Microsoft.Win32;

namespace MRB_Review_System
{
    class StaticFunctions
    {
        public static void RegisterUnit()
        {
            DateTime fechaHoraUtcActual = DateTime.Now;

            tryReadAgainAll:
            Globals.APP_DATA_ALL = ExcelFunctions.Excel_To_DataTable(Globals.PATH_FILE_ALL, 0);
            if (Globals.APP_DATA_ALL == null) goto tryReadAgainAll;

            tryReadAgain:
            Globals.APP_DATA = ExcelFunctions.Excel_To_DataTable(Globals.PATH_FILE, 0);
            if (Globals.APP_DATA == null) goto tryReadAgain;


            Globals.APP_DATA_ALL = AddLineRecord(Globals.APP_DATA_ALL, fechaHoraUtcActual, Globals.SERIAL_NUMBER, Globals.PROCESS, Globals.CRD, Globals.DEFECT,
                                             Globals.STATUS);

            Globals.APP_DATA = AddLineRecord(Globals.APP_DATA, fechaHoraUtcActual, Globals.SERIAL_NUMBER, Globals.PROCESS, Globals.CRD, Globals.DEFECT,
                                         Globals.STATUS);
        TryInserAgainAll:
            string errorAll = ExcelFunctions.DataTable_To_Excel(Globals.APP_DATA_ALL, Globals.PATH_FILE_ALL);
            if (!string.IsNullOrEmpty(errorAll)) goto TryInserAgainAll;


            TryInserAgain:
            string error = ExcelFunctions.DataTable_To_Excel(Globals.APP_DATA, Globals.PATH_FILE);
            if (!string.IsNullOrEmpty(error)) goto TryInserAgain;

            Globals.QTY_UNITS_SCANNED++;
        }

        public static DataTable AddLineRecord(DataTable dataTable, DateTime DateTime, string SERIAL_NUMBER, string PROCESS, string CRD, string DEFECT, string STATUS)
        {
            DataTable _dt = dataTable;
            _dt.TableName = "AppData";
            _dt.Rows.Add(DateTime.ToString(), SERIAL_NUMBER, PROCESS, CRD, DEFECT, STATUS);
            return _dt;
        }

        public static void NotifyOK()
        {
            System.Media.SystemSounds.Question.Play();
        }


        public static void NotifyError()
        {
            SystemSounds.Exclamation.Play();
        }

        public static void MakeToast(string PathFile)
        {
            new QualityDefectsRepair.ToastWindow(PathFile).Show();
        }


        public static bool IsAlreadyRegister() 
        {
            bool result = false;

            TryReadAgainAll:
            Globals.APP_DATA_ALL = ExcelFunctions.Excel_To_DataTable(Globals.PATH_FILE_ALL, 0);
            if (Globals.APP_DATA_ALL == null) goto TryReadAgainAll;

            try
            {
                DataTable EventsBySerialNumber = Globals.APP_DATA_ALL.AsEnumerable()
                .Where(r => r.Field<string>("SERIAL NUMBER") == Globals.SERIAL_NUMBER)
                .CopyToDataTable();

                if (EventsBySerialNumber.Rows.Count >= 1) result = true;
            }
            catch (Exception) { result = false; }

            return result;
        }



        public static void CleanExcelFile() 
        {
            DataTable _dtCelan = new DataTable();
            _dtCelan.Columns.Add("DATE");
            _dtCelan.Columns.Add("SERIAL NUMBER");
            _dtCelan.Columns.Add("PROCESS");
            _dtCelan.Columns.Add("CRD");
            _dtCelan.Columns.Add("DEFECT");
            _dtCelan.Columns.Add("STATUS");

            _dtCelan = StaticFunctions.AddLineRecord(_dtCelan, DateTime.Now, "DUMMY", "DUMMY", "DUMMY", "DUMMY", "DUMMY");
            _dtCelan = StaticFunctions.AddLineRecord(_dtCelan, DateTime.Now, "DUMMY", "DUMMY", "DUMMY", "DUMMY", "DUMMY");

            ExcelFunctions.DataTable_To_Excel(_dtCelan, Globals.PATH_FILE);          
        }
    }
}

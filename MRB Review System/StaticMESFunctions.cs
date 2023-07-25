using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRB_Review_System
{
    class StaticMESFunctions
    {

        public static bool CheckPoint(DataSet MesHistory, string StepToCheck, string TestType, string TestStatus) 
        {
            try 
            {
                DataTable EventsByStepMatrix1 = MesHistory.Tables[0].AsEnumerable()
                                          .Where(r => r.Field<string>("TestType") == TestType &&
                                                      r.Field<string>("Test_Process") == StepToCheck &&
                                                      r.Field<string>("TestStatus") == TestStatus)
                                          .CopyToDataTable();

                return true;
            }
            catch(Exception ex) 
            {
                return false;
            }          
        }



        public static Tuple<bool, string,string,string,string> GetFailure(DataSet MesHistory, string StepToCheck, string TestType, string TestStatus)
        {           
            DataTable EventsByStepMatrix1 = new DataTable();
            string Process = string.Empty;
            string Status = string.Empty;
            string Defect = string.Empty;
            string Crd = string.Empty;
            try
            {
                EventsByStepMatrix1 = MesHistory.Tables[0].AsEnumerable()
                                          .Where(r => r.Field<string>("TestType") == TestType &&
                                                      r.Field<string>("Test_Process") == StepToCheck &&
                                                      r.Field<string>("TestStatus") == TestStatus)
                                          .CopyToDataTable();

                Process = EventsByStepMatrix1.Rows[0][9].ToString();
                Status = EventsByStepMatrix1.Rows[0][10].ToString();
                Defect = EventsByStepMatrix1.Rows[0][19].ToString();
                Crd = EventsByStepMatrix1.Rows[0][11].ToString();

                return Tuple.Create(true,Process, Status, Defect, Crd);
            }
            catch (Exception ex)
            {
                return Tuple.Create(false,Process, Status, Defect, Crd);
            }
        }
    }
}

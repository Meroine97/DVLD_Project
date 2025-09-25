using DVLD_DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_Business
{
    public class clsTest
    {
        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode = enMode.AddNew;

        public int TestID { get; set; }
        public int TestAppointmentID { get; set; }
        public clsTestAppointment testAppointmentInfo { get; set; }
        public bool TestResult { get; set; }
        public string Notes { get; set; }
        public int CreatedByUserID { get; set; }


        public clsTest()
        {
            this.TestID = -1;
            this.TestAppointmentID = -1;
            this.TestResult = false;
            this.Notes = "";
            this.CreatedByUserID = -1;

            Mode = enMode.AddNew;
        }
        private clsTest(int TestID, int TestAppointmentID, bool TestResult,
            string Notes, int CreatedByUserID)
        {
            this.TestID = TestID;
            this.TestAppointmentID = TestAppointmentID;
            this.testAppointmentInfo = clsTestAppointment.Find(this.TestAppointmentID);
            this.TestResult = TestResult;
            this.Notes = Notes;
            this.CreatedByUserID = CreatedByUserID;

            Mode = enMode.Update;
        }

        private bool _AddNewTest()
        {
            this.TestID = clsTestData.AddNewTest(this.TestAppointmentID,
                this.TestResult, this.Notes, this.CreatedByUserID);

            return (this.TestID != -1);
        }

        private bool _UpdateTest()
        {
            return clsTestData.UpdateTest(this.TestID, this.TestAppointmentID, 
                               this.TestResult, this.Notes, this.CreatedByUserID);
        }

        public static clsTest Find(int TestID)
        {
            int TestAppointmentID = -1, CreatedByUserID = -1;
            bool TestResult = false;
            string Notes = "";

            if (clsTestData.GetTestByID(TestID, ref TestAppointmentID,
                                ref TestResult, ref Notes, ref CreatedByUserID))
            {
                return new clsTest(TestID, TestAppointmentID, TestResult, 
                                           Notes, CreatedByUserID);
            }
            else
                return null;
        }

        public static clsTest FindLastTestPerPersonAndLicenseClassAndTestType(int PersonID,
            int LicenseClassID, clsTestType.enTestType testType)
        {
            int TestID = -1, TestAppointmentID = -1, CreatedByUserID = -1;
            bool TestResult = false;
            string Notes = "";

            if (clsTestData.GetLastTestByPersonAndTestTypeAndLicenseClass(PersonID,
                        (int)testType, LicenseClassID, ref TestID, ref TestAppointmentID,
                        ref TestResult, ref Notes, ref CreatedByUserID))
            {
                return new clsTest(TestID, TestAppointmentID, TestResult,
                                           Notes, CreatedByUserID);
            }
            else
                return null;
        }

        public static DataTable GetAllTests()
        {
            return clsTestData.GetAllTests();
        }

        public static bool DeleteTest(int TestID)
        {
            return clsTestData.DeleteTest(TestID);
        }

        public static bool isTestExist(int TestID)
        {
            return clsTestData.isTestExist(TestID);
        }

        public static byte GetPassedTestCount(int LDLAppID)
        {
            return clsTestData.GetPassedTestCount(LDLAppID);
        }

        public static bool PassedAllTests(int LDLAppID)
        {
            // If Total passed test is equal to 3 which means this LDLAppID 
            // is pass all the test
            return GetPassedTestCount(LDLAppID) == 3;
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewTest())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:
                    return _UpdateTest();
            }

            return false;
        }
    }
}

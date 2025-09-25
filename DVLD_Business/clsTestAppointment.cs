using DVLD_DataAccess;
using System;
using System.Data;

namespace DVLD_Business
{
    public class clsTestAppointment
    {
        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode = enMode.AddNew;

        public int TestAppointmentID { get; set; }
        public clsTestType.enTestType TestTypeID { get; set; }
        public int LDLAppID { get; set; }
        public DateTime AppointmentDate { get; set; }
        public float PaidFees { get; set; }
        public int CreatedByUserID { get; set; }
        public bool isLocked { get; set; }
        public int RetakeTestApplicationID { get; set; }
        public clsApplication RetakeTestAppInfo { get; set; }
        public int TestID
        {
            get
            {
                return _GetTestID();
            }
        }

        public clsTestAppointment()
        {
            this.TestAppointmentID = -1;
            this.TestTypeID = clsTestType.enTestType.VisionTest;
            this.LDLAppID = -1;
            this.AppointmentDate = DateTime.Now;
            this.PaidFees = 0;
            this.CreatedByUserID = -1;
            this.isLocked = false;
            this.RetakeTestApplicationID = -1;

            Mode = enMode.AddNew;
        }
        private clsTestAppointment(int TestAppointmentID, clsTestType.enTestType TestTypeID,
            int LDLAppID, DateTime AppointmentDate, float PaidFees, 
            int CreatedByUserID, bool isLocked, int RetakeTestApplicationID)
        {
            this.TestAppointmentID = TestAppointmentID;
            this.TestTypeID = TestTypeID;
            this.LDLAppID = LDLAppID;
            this.AppointmentDate = AppointmentDate;
            this.PaidFees = PaidFees;
            this.CreatedByUserID = CreatedByUserID;
            this.isLocked = isLocked;
            this.RetakeTestApplicationID = RetakeTestApplicationID;
            this.RetakeTestAppInfo = clsApplication.FindBaseApplication(RetakeTestApplicationID);

            Mode = enMode.Update;
        }

        private bool _AddNewTestAppointment()
        {
            this.TestAppointmentID = clsTestAppointmentData.AddNewTestAppointment(
                (int)this.TestTypeID, this.LDLAppID, this.AppointmentDate, this.PaidFees,
                this.CreatedByUserID, this.isLocked, this.RetakeTestApplicationID);

            return (this.TestAppointmentID != -1);
        }

        private bool _UpdateTestAppointment()
        {
            return clsTestAppointmentData.UpdateTestAppointment(this.TestAppointmentID,
                (int)this.TestTypeID, this.LDLAppID, this.AppointmentDate, this.PaidFees,
                this.CreatedByUserID, this.isLocked, this.RetakeTestApplicationID);
        }

        public static clsTestAppointment Find(int TestAppointmentID)
        {
            int TestTypeID = -1, LDLAppID = -1, CreatedByUserID = -1,
                RetakeTestApplicationID = -1;
            DateTime AppointmentDate = DateTime.Now;
            float PaidFees = 0;
            bool isLocked = false;

            if (clsTestAppointmentData.GetTestAppointmentInfoByID(TestAppointmentID,
                ref TestTypeID, ref LDLAppID, ref AppointmentDate, ref PaidFees, 
                ref CreatedByUserID, ref isLocked, ref RetakeTestApplicationID))
            {
                return new clsTestAppointment(TestAppointmentID, 
                    (clsTestType.enTestType)TestTypeID, LDLAppID, AppointmentDate, 
                    PaidFees, CreatedByUserID, isLocked, RetakeTestApplicationID);
            }
            else
                return null;
        }

        public static clsTestAppointment GetLastTestAppointment(int LDLAppID,
            clsTestType.enTestType TestTypeID)
        {
            int TestAppointmentID = -1, CreatedByUserID = -1,
                RetakeTestApplicationID = -1;
            DateTime AppointmentDate = DateTime.Now;
            float PaidFees = 0;
            bool isLocked = false;

            if (clsTestAppointmentData.GetLastTestAppointment(LDLAppID,
                (int)TestTypeID, ref TestAppointmentID, ref AppointmentDate, ref PaidFees,
                ref CreatedByUserID, ref isLocked, ref RetakeTestApplicationID))
            {
                return new clsTestAppointment(TestAppointmentID, 
                    (clsTestType.enTestType)TestTypeID, LDLAppID, AppointmentDate,
                    PaidFees, CreatedByUserID, isLocked, RetakeTestApplicationID);
            }
            else
                return null;
        }

        public static DataTable GetAllTestAppointments()
        {
            return clsTestAppointmentData.GetAllTestAppointments();
        }

        public DataTable GetApplicationTestAppointmentPerTestType(clsTestType.enTestType TestTypeID)
        {
            return clsTestAppointmentData.GetApplicationTestAppointmentPerTestType(
                this.LDLAppID, (int)TestTypeID);
        }

        public static DataTable GetApplicationTestAppointmentPerTestType(int LDLAppID,
            clsTestType.enTestType TestTypeID)
        {
            return clsTestAppointmentData.GetApplicationTestAppointmentPerTestType(
                   LDLAppID, (int)TestTypeID);
        }

        public bool Save()
        {
            switch(Mode)
            {
                case enMode.AddNew:
                    if(_AddNewTestAppointment())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:
                    return _UpdateTestAppointment();
            }

            return false;
        }

        private int _GetTestID()
        {
            return clsTestAppointmentData.GetTestID(this.TestAppointmentID);
        }
    }
}

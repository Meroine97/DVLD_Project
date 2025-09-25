using DVLD_DataAccess;
using System;
using System.Data;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DVLD_Business
{
    public class clsLocalDrivingLicenseApplication : clsApplication
    {
        private enum enMode { AddNew = 0, Update = 1 }
        private enMode _Mode = enMode.AddNew;

        public int LDLApplicationID { get; set; }
        public int LicenseClassID { get; set; }
        public clsLicenseClass LicenseClassInfo;
        public string PersonFullName
        {
            get
            {
                return clsPerson.Find(ApplicantPersonID).FullName;
            }
        }

        public clsLocalDrivingLicenseApplication()
        {
            LDLApplicationID = -1;
            LicenseClassID = -1;

            _Mode = enMode.AddNew;
        }

        private clsLocalDrivingLicenseApplication(int LDLApplicationID, int ApplicationID,
            int ApplicantPersonID, DateTime ApplicationDate, int ApplicationTypeID,
            enApplicationStatus ApplicationStatus, DateTime LastStatusDate,
            float PaidFees, int CreatedByUserID, int LicenseClassID)
        {
            this.LDLApplicationID = LDLApplicationID;
            this.ApplicationID = ApplicationID;
            this.ApplicantPersonID = ApplicantPersonID;
            this.ApplicationDate = ApplicationDate;
            this.ApplicationTypeID = ApplicationTypeID;
            this.ApplicationStatus = ApplicationStatus;
            this.LastStatusDate = LastStatusDate;
            this.PaidFees = PaidFees;
            this.CreatedByUserID = CreatedByUserID;
            this.LicenseClassID = LicenseClassID;
            this.LicenseClassInfo = clsLicenseClass.Find(LicenseClassID);
            _Mode = enMode.Update;
        }

        private bool _AddNewLDLApplication()
        {
            this.LDLApplicationID = clsLocalDrivingLicenseApplicationData.AddNewLocalDrivingLicenseApplication(
                this.LicenseClassID, this.ApplicationID);

            return (this.LDLApplicationID != -1);
        }

        private bool _UpdateLDLApplication()
        {
            return clsLocalDrivingLicenseApplicationData.UpdateLocalDrivingLicenseApplication(
                this.LDLApplicationID, this.LicenseClassID, this.ApplicationID);
        }

        public static clsLocalDrivingLicenseApplication FindByLocalDrivingLicenseAppID(int LDLApplicationID)
        {
            int LicenseClassID = -1, ApplicationID = -1;

            if (clsLocalDrivingLicenseApplicationData.GetLocalDrivingLicenseApplicationInfoByID(LDLApplicationID, ref LicenseClassID,
                ref ApplicationID))
            {
                clsApplication Application = clsApplication.FindBaseApplication(ApplicationID);

                return new clsLocalDrivingLicenseApplication(LDLApplicationID, ApplicationID, 
                    Application.ApplicantPersonID, Application.ApplicationDate, Application.ApplicationTypeID,
                    Application.ApplicationStatus, Application.LastStatusDate, Application.PaidFees,
                    Application.CreatedByUserID, LicenseClassID);
            }
            else
            {
                return null;
            }
        }

        public static clsLocalDrivingLicenseApplication FindByApplicationID(int ApplicationID)
        {
            int LicenseClassID = -1, LDLApplicationID = -1;

            if (clsLocalDrivingLicenseApplicationData.GetLocalDrivingLicenseApplicationInfoByApplicationID(
                ApplicationID, ref LDLApplicationID, ref LicenseClassID))
            {
                clsApplication Application = clsApplication.FindBaseApplication(ApplicationID);

                return new clsLocalDrivingLicenseApplication(LDLApplicationID, ApplicationID,
                    Application.ApplicantPersonID, Application.ApplicationDate, Application.ApplicationTypeID,
                    (enApplicationStatus)Application.ApplicationStatus, Application.LastStatusDate, 
                    Application.PaidFees, Application.CreatedByUserID, LicenseClassID);
            }
            else
            {
                return null;
            }
        }

        public static DataTable GetAllLDLApplication()
        {
            return clsLocalDrivingLicenseApplicationData.GetAllLocalDrivingLicenseApplications();
        }

        public bool Delete()
        {
            bool isLocalDrivingApplicationDeleted = false;
            bool isBaseApplicationDeleted = false;

            // first delete local Driving License Application 
            isLocalDrivingApplicationDeleted = clsLocalDrivingLicenseApplicationData.DeleteLocalDrivingLicenseApplication(
                this.LDLApplicationID);

            if (!isLocalDrivingApplicationDeleted)
                return false;

            // Then we delete base Application ...
            isBaseApplicationDeleted = base.Delete();
            return isBaseApplicationDeleted;
        }

        public bool Save()
        {
            /*
             * Because of inhiretance calling Save method to take care adding data to Application Table 
             * Telling the base class which mode that we are in to save base info ...
             */
 
            base.Mode = (clsApplication.enMode)_Mode;
            if (!base.Save())
                return false;

            // after saving data in base, we have to save child info too ...
            switch(_Mode)
            {
                case enMode.AddNew:
                    if(_AddNewLDLApplication())
                    {
                        _Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:
                    return _UpdateLDLApplication();
            }

            return false;
        }

        public static bool IsThereAnActiveScheduledTest(int LDLAppID, clsTestType.enTestType TestType)
        {
            return clsLocalDrivingLicenseApplicationData.IsThereAnActiveScheduleTest(LDLAppID,
                (int)TestType);
        }
        public bool IsThereAnActiveScheduledTest(clsTestType.enTestType TestType)
        {
            return clsLocalDrivingLicenseApplicationData.IsThereAnActiveScheduleTest(
                this.LDLApplicationID, (int)TestType);
        }

        public bool DoesPassTestType(clsTestType.enTestType TestType)
        {
            return clsLocalDrivingLicenseApplicationData.DoesPassedTestType(
                this.LDLApplicationID, (int)TestType);
        }
        public static bool DoesPassTestType(int LDLAppID, clsTestType.enTestType TestType)
        {
            return clsLocalDrivingLicenseApplicationData.DoesPassedTestType(LDLAppID, (int)TestType);
        }

        public bool DoesPassPreviousTest(clsTestType.enTestType CurrentTestType)
        {
            switch(CurrentTestType)
            {
                case clsTestType.enTestType.VisionTest:
                    return true;

                case clsTestType.enTestType.WrittenTest:
                    return this.DoesPassTestType(clsTestType.enTestType.VisionTest);

                case clsTestType.enTestType.StreetTest:
                    return this.DoesPassTestType(clsTestType.enTestType.WrittenTest);

                default:
                    return false;
            }

        }

        public bool DoesAttendTestType(clsTestType.enTestType TestType)
        {
            return clsLocalDrivingLicenseApplicationData.DoesAttendTestType(
                LDLApplicationID, (int)TestType);
        }
        public static bool DoesAttendTestType(int LDLAppID, clsTestType.enTestType TestType)
        {
            return clsLocalDrivingLicenseApplicationData.DoesAttendTestType(LDLAppID, (int)TestType);
        }

        public byte TotalTrailsPerTest(clsTestType.enTestType TestTypeID)
        {
            return clsLocalDrivingLicenseApplicationData.TotalTrialsPerTest(this.LDLApplicationID,
                (int)TestTypeID);
        }
        public static byte TotalTrailsPerTest(int LDLAppID, clsTestType.enTestType TestType)
        {
            return clsLocalDrivingLicenseApplicationData.TotalTrialsPerTest(LDLAppID, (int)TestType);
        }

        public static bool AttendedTest(int LDLAppID, clsTestType.enTestType TestType)
        {
            return clsLocalDrivingLicenseApplicationData.TotalTrialsPerTest(LDLAppID, (int)TestType) > 0;
        }
        public bool AttendedTest(clsTestType.enTestType TestType)
        {
            return clsLocalDrivingLicenseApplicationData.TotalTrialsPerTest(this.LDLApplicationID, (int)TestType) > 0;
        }

        // ====> The rest methode should work with table Tests first ...
        public clsTest GetLastTestPerTestType(clsTestType.enTestType testType)
        {
            return clsTest.FindLastTestPerPersonAndLicenseClassAndTestType(this.ApplicantPersonID,
                this.LicenseClassID, testType);
        }

        public byte GetPassedTestCount()
        {
            return clsTestData.GetPassedTestCount(this.LDLApplicationID);
        }
        public static byte GetPassedTestCount(int LDLAppID)
        {
            return clsTestData.GetPassedTestCount(LDLAppID);
        }

        public bool PassedAllTests()
        {
            return clsTest.PassedAllTests(this.LDLApplicationID);
        }
        public static bool PassedAllTests(int LDLAppID)
        {
            return clsTest.PassedAllTests(LDLAppID);
        }

        // ===> Part License and issue for first time

        public int IssueLicenseForFirstTime(string Notes, int CreatedByUserID)
        {
            // First check if that person is linked with driver which means person is driver
            // if yes so we should'nt add it again to driver table
            // Other wise we should add this person to driver table as a driver
            int DriverID = -1;

            clsDriver Driver = clsDriver.FindByPersonID(this.ApplicantPersonID);
            if(Driver == null)
            {
                Driver = new clsDriver();
                Driver.PersonID = this.ApplicantPersonID;
                Driver.CreatedByUserID = CreatedByUserID;
                Driver.CreatedDate = DateTime.Now;

                if(Driver.Save())
                {
                    DriverID = Driver.DriverID;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                DriverID = Driver.DriverID;
            }

            // now the driver is here so 
            // Create a new license and return the [ license id ]
            clsLicense License = new clsLicense();

            License.ApplicationID = this.ApplicationID;
            License.DriverID = DriverID;
            License.LicenseClass = this.LicenseClassInfo.LicenseClassID;
            License.IssueDate = DateTime.Now;
            License.ExpirationDate = DateTime.Now.AddYears(this.LicenseClassInfo.defaultValidityLength);
            License.Notes = Notes;
            License.PaidFees = this.LicenseClassInfo.classFees;
            License.IsActive = true; 
            License.IssueReason = clsLicense.enIssueReason.FirstTime;
            License.CreatedByUserID = CreatedByUserID;

            if (License.Save())
            {
                this.SetCompleted();
                return License.LicenseID;
            }
            else
                return -1;

        }

        public int GetActiveLicenseID()
        {
            return clsLicense.GetActiveLicenseIDByPersonID(this.ApplicantPersonID,
                this.LicenseClassID);
        }
        
        public bool IsLicenseIssued()
        {
            return (GetActiveLicenseID() != -1);
        }
    }
}

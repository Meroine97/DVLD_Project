using DVLD_DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_Business
{
    public class clsLicenseClass
    {

        public enum enMode { AddNew = 0, Update = 1 }
        private enMode _Mode = enMode.AddNew;

        public int LicenseClassID {  get; set; }
        public string className { get; set; }
        public string classDescription { get; set; }
        public byte minAllowAge { get; set; }
        public byte defaultValidityLength {  get; set; }
        public float classFees { get; set; }

        public clsLicenseClass()
        {
            this.LicenseClassID = -1;
            this.className = "";
            this.classDescription = "";
            this.minAllowAge = 18;
            this.defaultValidityLength = 10;
            this.classFees = 0;

            _Mode = enMode.AddNew;
        }

        private clsLicenseClass(int ID,  string className, string Description,
            byte minAge, byte defaultValidityLength, float classFees)
        {
            this.LicenseClassID = ID;
            this.className = className;
            this.classDescription = Description;
            this.minAllowAge = minAge;
            this.defaultValidityLength = defaultValidityLength;
            this.classFees = classFees;

            _Mode = enMode.Update;
        }

        public static clsLicenseClass Find(int LicenseClassID)
        {
            string className = "", Description = "";
            byte minAge = 18, defaultValidityLength = 10;
            float fees = 0;

            if (clsLicenseClassData.GetLicenseClassByID(LicenseClassID, ref className,
                ref Description, ref minAge, ref defaultValidityLength, ref fees))
            {
                return new clsLicenseClass(LicenseClassID, className, Description,
                    minAge, defaultValidityLength, fees);
            }
            else
                return null;
        }

        public static clsLicenseClass Find(string ClassName)
        {
            int LicenseClassID = -1;
            string Description = "";
            byte minAge = 18, defaultValidityLength = 10;
            float fees = 0;

            if (clsLicenseClassData.GetLicenseClassByClassName(ClassName, ref LicenseClassID,
                ref Description, ref minAge, ref defaultValidityLength, ref fees))
            {
                return new clsLicenseClass(LicenseClassID, ClassName, Description,
                    minAge, defaultValidityLength, fees);
            }
            else
                return null;
        }

        public bool _AddNewLicenseClass()
        {
            this.LicenseClassID = clsLicenseClassData.AddNewLicenseClass(this.className,
                this.classDescription, this.minAllowAge, this.defaultValidityLength,
                this.classFees);

            return (this.LicenseClassID != -1);
        }
        
        public bool _UpdateLicenseClass()
        {
            return clsLicenseClassData.UpdateLicenseClass(this.LicenseClassID,
                this.className, this.classDescription, this.minAllowAge,
                this.defaultValidityLength, this.classFees);
        }

        public bool Save()
        {
            switch(_Mode)
            {
                case enMode.AddNew:
                    if (_AddNewLicenseClass())
                    {
                        _Mode = enMode.Update;
                        return true;
                    }
                    else
                        return false;

                case enMode.Update:
                    return _UpdateLicenseClass();
            }

            return false;
        }

        public static DataTable GetAllLicenseClasses()
        {
            return clsLicenseClassData.GetAllLicenseClasses();
        }

        public static bool DeleteLicenseClass(int LicenseClassID)
        {
            return clsLicenseClassData.DeleteLicenseClass(LicenseClassID);
        }
    }
}

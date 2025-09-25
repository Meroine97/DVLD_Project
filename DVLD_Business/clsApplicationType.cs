using DVLD_DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_Business
{
    public class clsApplicationType
    {
        public enum enMode { AddNew = 0, Update = 1 }
        private enMode _Mode = enMode.AddNew;

        public int AppTypeID { get; set; }
        public string Title { get; set; }
        public float Fees { get; set; }

        public clsApplicationType()
        {
            this.AppTypeID = -1;
            this.Title = string.Empty;
            this.Fees = 0;

            _Mode = enMode.AddNew;
        }

        private clsApplicationType(int ID, string Title, float Fees)
        {
            this.AppTypeID = ID;
            this.Title = Title;
            this.Fees= Fees;

            _Mode = enMode.Update;
        }


        public static clsApplicationType Find(int AppTypeID)
        {
            string Title = "";
            float Fees = 0;

            if (clsApplicationTypeData.GetApplicationTypeByID(AppTypeID, ref Title,
                ref Fees))
            {
                return new clsApplicationType(AppTypeID, Title, Fees);
            }
            else
                return null;
        }

        private bool _AddNewApplicationType()
        {
            this.AppTypeID = clsApplicationTypeData.AddNewApplicationType(this.Title,
                this.Fees);

            return (this.AppTypeID != -1);
        }

        private bool _UpdateApplicationType()
        {
            return clsApplicationTypeData.UpdateApplicationType(this.AppTypeID,
                this.Title, this.Fees);
        }

        public bool Save()
        {
            switch(_Mode)
            {
                case enMode.AddNew:
                    if (_AddNewApplicationType())
                    {
                        _Mode = enMode.Update;
                        return true;
                    }
                    else
                        return false;
                    
                case enMode.Update:
                    return _UpdateApplicationType();
                    
            }

            return false;
        }

        public static DataTable GetAllApplicationTypes()
        {
            return clsApplicationTypeData.GetAllApplicationTypes();
        }
    }
}

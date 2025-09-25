using DVLD_DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_Business
{
    public class clsTestType
    {
        public enum enTestType { VisionTest = 1, WrittenTest = 2, StreetTest = 3 }
        
        public enum enMode { AddNew = 0, Update = 1 }
        private enMode _Mode = enMode.AddNew;

        public clsTestType.enTestType ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public float Fees { get; set; }

        public clsTestType()
        {
            this.ID = enTestType.VisionTest;
            this.Title = string.Empty;
            this.Description = string.Empty;
            this.Fees = 0;

            _Mode = enMode.AddNew;
        }

        private clsTestType(clsTestType.enTestType ID, string Title, string Description, float Fees)
        {
            this.ID = ID;
            this.Title = Title;
            this.Description = Description;
            this.Fees = Fees;

            _Mode = enMode.Update;
        }


        public static clsTestType Find(clsTestType.enTestType TestTypeID)
        {
            string Title = "", Description = "";
            float Fees = 0;

            if (clsTestTypesData.GetTestTypeByID((int)TestTypeID, ref Title, ref Description,
                ref Fees))
            {
                return new clsTestType(TestTypeID, Title, Description, Fees);
            }
            else
                return null;
        }

        private bool _AddNewTestType()
        {
            this.ID = (enTestType)clsTestTypesData.AddNewTestType(this.Title, this.Description,
                this.Fees);

            return (this.Title != "");
        }

        private bool _UpdateTestType()
        {
            return clsTestTypesData.UpdateTestType((int)this.ID, this.Title,
                this.Description, this.Fees);
        }

        public bool Save()
        {
            switch (_Mode)
            {
                case enMode.AddNew:
                    if (_AddNewTestType())
                    {
                        _Mode = enMode.Update;
                        return true;
                    }
                    else
                        return false;

                case enMode.Update:
                    return _UpdateTestType();

            }

            return false;
        }

        public static DataTable GetAllTestTypes()
        {
            return clsTestTypesData.GetAllTestTypes();
        }

    }
}

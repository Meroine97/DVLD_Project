using DVLD_DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_Business
{
    public class clsPerson
    {
        private enum enMode { AddNew = 0, Update = 1 }
        enMode _Mode = enMode.AddNew;

        public int PersonID { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string ThirdName { get; set; }
        public string LastName { get; set; }
        public string FullName
        {
            get
            {
                return FirstName + " " + SecondName + " " + ThirdName + " " + LastName;
            }
        }
        public string NationalNo { get; set; }
        public DateTime DateOfBirth { get; set; }
        public byte Gender { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int CountryID { get; set; }
        public clsCountry CountryInfo; // this is the composition

        private string _ImagePath;
        public string ImagePath
        {
            get { return _ImagePath; }
            set { _ImagePath = value; }
        }


        public clsPerson()
        {
            this.PersonID = -1;
            this.FirstName = string.Empty;
            this.LastName = string.Empty;
            this.ThirdName = string.Empty;
            this.LastName = string.Empty;
            this.NationalNo = string.Empty;
            this.DateOfBirth = DateTime.Now;
            this.Gender = 0;
            this.Address = string.Empty;
            this.Phone = string.Empty;
            this.Email = string.Empty;
            this.CountryID = -1;
            this.ImagePath = string.Empty;

            _Mode = enMode.AddNew;
        }

        private clsPerson(int PersonID, string FirstName, string SecondName,
            string ThirdName, string LastName, string NationalNo, DateTime DateOfBirth,
            byte Gender, string Address, string Phone, string Email, int CountryID,
            string ImagePath)
        {
            this.PersonID = PersonID;
            this.FirstName = FirstName;
            this.SecondName = SecondName;
            this.ThirdName = ThirdName;
            this.LastName = LastName;
            this.NationalNo = NationalNo;
            this.DateOfBirth = DateOfBirth;
            this.Gender = (byte)Gender;
            this.Address = Address;
            this.Phone = Phone;
            this.Email = Email;
            this.CountryID = CountryID;
            this.CountryInfo = clsCountry.Find(CountryID);
            this.ImagePath = ImagePath;

            _Mode = enMode.Update;

        }

        public static clsPerson Find(int PersonID)
        {
            string FirstName = "", SecondName = "", ThirdName = "", LastName = "",
                Address = "", Phone = "", Email = "", ImagePath = "", NationalNo = "";
            DateTime DateOfBirth = DateTime.Now;
            byte Gender = 0;
            int CountryID = 0;

            bool isFound = clsPersonData.GetPersonInfoByID(PersonID, ref FirstName, ref SecondName
                , ref ThirdName, ref LastName, ref NationalNo, ref DateOfBirth, ref Gender, ref Address, ref Phone,
                ref Email, ref CountryID, ref ImagePath);

            if (isFound)
            {
                return new clsPerson(PersonID, FirstName, SecondName, ThirdName, LastName,
                    NationalNo, DateOfBirth, Gender, Address, Phone, Email,
                    CountryID, ImagePath);
            }
            else
                return null;
        }

        public static clsPerson Find(string NationalNo)
        {
            string FirstName = "", SecondName = "", ThirdName = "", LastName = "",
                Address = "", Phone = "", Email = "", ImagePath = "";
            DateTime DateOfBirth = DateTime.Now;
            byte Gender = 0;
            int CountryID = 0, PersonID = 0;

            bool isFound = clsPersonData.GetPersonInfoByNationalNo(NationalNo, ref PersonID, ref FirstName, ref SecondName
                , ref ThirdName, ref LastName, ref DateOfBirth, ref Gender, ref Address, ref Phone,
                ref Email, ref CountryID, ref ImagePath);

            if (isFound)
            {
                return new clsPerson(PersonID, FirstName, SecondName, ThirdName, LastName,
                    NationalNo, DateOfBirth, Gender, Address, Phone, Email,
                    CountryID, ImagePath);
            }
            else
                return null;
        }

        private bool _AddNewPerson()
        {
            this.PersonID = clsPersonData.AddNewPerson(this.FirstName, this.SecondName,
                this.ThirdName, this.LastName, this.NationalNo, this.DateOfBirth, this.Gender,
                this.Address, this.Phone, this.Email, this.CountryID, this.ImagePath);

            return this.PersonID != -1;
        }

        private bool _UpdatePerson()
        {
            return clsPersonData.UpdatePerson(this.PersonID, this.FirstName, this.SecondName,
                this.ThirdName, this.LastName, this.NationalNo, this.DateOfBirth, this.Gender,
                this.Address, this.Phone, this.Email, this.CountryID, this.ImagePath);
        }

        public bool Save()
        {
            switch (_Mode)
            {
                case enMode.AddNew:
                    if (_AddNewPerson())
                    {
                        _Mode = enMode.Update;
                        return true;
                    }
                    else
                        return false;

                case enMode.Update:
                    return _UpdatePerson();
            }

            return false;
        }

        public static DataTable GetAllPeople()
        {
            return clsPersonData.GetAllPeople();
        }

        public static bool DeletePerson(int PersonID)
        {
            return clsPersonData.DeletePerson(PersonID);
        }

        public static bool isPersonExist(int PersonID)
        {
            return clsPersonData.isPersonExist(PersonID);
        }

        public static bool isPersonExist(string NationalNo)
        {
            return clsPersonData.isPersonExist(NationalNo);
        }
    }
}

using DVLD_DataAccess;
using System.Data;

namespace DVLD_Business
{
    public class clsUser
    {
        public enum enMode { AddNew = 0, Update = 1}
        private enMode _Mode = enMode.AddNew;
        public int UserID { get; set; }
        public int PersonID { get; set; }
        public clsPerson PersonInfo;
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }

        public clsUser()
        {
            this.UserID = -1;
            this.PersonID = -1;
            this.UserName = "";
            this.Password = "";
            this.IsActive = false;

            _Mode = enMode.AddNew;
        }

        private clsUser(int UserID, int PersonID, string UserName, string Password,
            bool IsActive)
        {
            this.UserID = UserID;
            this.PersonID = PersonID;
            this.PersonInfo = clsPerson.Find(this.PersonID);
            this.UserName = UserName;
            this.Password = Password;
            this.IsActive = IsActive;

            _Mode = enMode.Update;
        }

        // Also here i make an update
        private bool _AddNewUser()
        {
            this.Password = clsSHA256.ComputeHash(this.Password);

            this.UserID = clsUserData.AddNewUser(this.PersonID, this.UserName,
                          this.Password, this.IsActive);

            return (this.UserID != -1);
        }

        // I make an update here
        private bool _UpdateUser()
        {
            this.Password = clsSHA256.ComputeHash(this.Password);

            return clsUserData.UpdateUser(this.UserID, this.PersonID, this.UserName,
                   this.Password, this.IsActive);
        }

        public static clsUser Find(int UserID)
        {
            int PersonID = -1;
            string UserName = "", Password = "";
            bool isActive = false;

            if (clsUserData.GetUserInfoByID(UserID, ref PersonID, ref UserName,
                ref Password, ref isActive))
            {
                return new clsUser(UserID, PersonID, UserName, Password, isActive);
            }
            else
                return null;
        }

        public static clsUser FindByPersonID(int PersonID)
        {
            int UserID = -1;
            string UserName = "", Password = "";
            bool isActive = false;

            if (clsUserData.GetUserInfoByPersonID(PersonID, ref UserID, ref UserName,
               ref Password, ref isActive))
            {
                return new clsUser(UserID, PersonID, UserName, Password, isActive);
            }
            else
                return null;
        }

        public static clsUser FindByUsernameAndPassword(string Username, string Password)
        {
            int UserID = -1, PersonID = -1;
            bool isActive = false;

            if (clsUserData.GetUserInfoByUsrenameAndPassword(Username, Password,
                ref UserID, ref PersonID, ref isActive))
            {
                return new clsUser(UserID, PersonID, Username, Password, isActive);
            }
            else
                return null;
        }

        public bool Save()
        {
            switch(_Mode)
            {

                case enMode.AddNew:
                    if (_AddNewUser())
                    {
                        _Mode = enMode.Update;
                        return true;
                    }
                    else
                        return false;

                case enMode.Update:
                    return _UpdateUser();

            }

            return false;
        }

        public static DataTable GetAllUsers()
        {
            return clsUserData.GetAllUsers();
        }

        public static bool DeleteUser(int UserID)
        {
            return clsUserData.DeleteUser(UserID);
        }

        public static bool isUserExist(int UserID)
        {
            return clsUserData.isUserExist(UserID);
        }

        public static bool isUserExist(string UserName)
        {
            return clsUserData.isUserExist(UserName);
        }

        public static bool isUserExistForPersonID(int PersonID)
        {
            return clsUserData.isUserExistForPersonID(PersonID);
        }


    }
}

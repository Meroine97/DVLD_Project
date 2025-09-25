using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_DataAccess
{
    public class clsLocalDrivingLicenseApplicationData
    {
        public static DataTable GetAllLocalDrivingLicenseApplications()
        {
            DataTable dt = new DataTable();

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"Select * From LocalDrivingLicenseApplications_View
                                      Order by ApplicationDate Desc;";
            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if(reader.HasRows)
                {
                    dt.Load(reader);
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                clsEventLog.setEventLogError(ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return dt;
        }

        public static bool GetLocalDrivingLicenseApplicationInfoByID(int LDLApplicationID, ref int LicenseClassID, 
            ref int ApplicationID)
        {
            bool isFound = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"Select * From LocalDrivingLicenseApplications 
                   Where LocalDrivingLicenseApplicationID = @LDLApplicationID";
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@LDLApplicationID", LDLApplicationID);

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                if(reader.Read())
                {
                    isFound = true;

                    LicenseClassID = (int)reader["LicenseClassID"];
                    ApplicationID = (int)reader["ApplicationID"];
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                isFound = false;
                clsEventLog.setEventLogError(ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return isFound;
        }

        public static bool GetLocalDrivingLicenseApplicationInfoByApplicationID(int ApplicationID, ref int LDLApplicationID,
            ref int LicenseClassID)
        {
            bool isFound = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"Select * From LocalDrivingLicenseApplications 
                   Where ApplicationID = @ApplicationID";
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@ApplicationID", ApplicationID);

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    isFound = true;

                    LDLApplicationID = (int)reader["LocalDrivingLicenseApplicationID"];
                    LicenseClassID = (int)reader["LicenseClassID"];
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                isFound = false;
                clsEventLog.setEventLogError(ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return isFound;
        }

        public static int AddNewLocalDrivingLicenseApplication(int LicenseClassID, int ApplicationID)
        {
            int LDLAppID = -1;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"Insert Into LocalDrivingLicenseApplications
                                         (LicenseClassID, ApplicationID)
                                         Values(@LicenseClassID, @ApplicationID);

                             Select SCOPE_IDENTITY();";
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);
            command.Parameters.AddWithValue("@ApplicationID", ApplicationID);

            try
            {
                connection.Open();
                object result = command.ExecuteScalar();

                if(result != null && int.TryParse(result.ToString(), out int InsertedID))
                {
                    LDLAppID = InsertedID;
                }
            }
            catch (Exception ex)
            {
                clsEventLog.setEventLogError(ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return LDLAppID;
        }

        public static bool UpdateLocalDrivingLicenseApplication(int LDLAppID, int LicenseClassID,
            int ApplicationID)
        {

            int rowsAffected = 0;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"Update LocalDrivingLicenseApplications
                                         Set LicenseClassID = @LicenseClassID,
                                             ApplicationID = @ApplicationID
                                         Where LocalDrivingLicenseApplicationID = @LDLAppID";
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@LDLAppID", LDLAppID);
            command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);
            command.Parameters.AddWithValue("@ApplicationID", ApplicationID);

            try
            {
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                clsEventLog.setEventLogError(ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return (rowsAffected != 0);

        }

        public static bool DeleteLocalDrivingLicenseApplication(int LDLAppID)
        {
            int rowsAffected = 0;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"Delete LocalDrivingLicenseApplications
                                         Where LocalDrivingLicenseApplicationID = @LDLAppID";
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@LDLAppID", LDLAppID);

            try
            {
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                clsEventLog.setEventLogError(ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return (rowsAffected != 0);
        }

        // This method checked if the local Driving License Application ID passed 
        // test type dependent on it type (Vision, Written and Street)
        public static bool DoesPassedTestType(int LDLAppID, int TestTypeID)
        {
            bool isFound = false;

            SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"SELECT top 1 TestResult FROM LocalDrivingLicenseApplications 
                             INNER JOIN TestAppointments ON 
                             LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID = TestAppointments.LocalDrivingLicenseApplicationID
                             INNER JOIN Tests ON 
                             TestAppointments.TestAppointmentID = Tests.TestAppointmentID
                             WHERE 
                             (LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID = @LDLAppID) 
                             AND(TestAppointments.TestTypeID = @TestTypeID)
                             ORDER BY TestAppointments.TestAppointmentID desc";
            SqlCommand cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@LDLAppID", LDLAppID);
            cmd.Parameters.AddWithValue("@TestTypeID", TestTypeID);

            try
            {
                conn.Open();
                object result = cmd.ExecuteScalar();

                if (result != null && bool.TryParse(result.ToString(), out bool returnedResult))
                {
                    isFound = returnedResult;
                }
            }
            catch (Exception ex)
            {
                isFound = false;
                clsEventLog.setEventLogError(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return isFound;
        }

        // In this method we checked if the LDLAppID is attended to test type 
        // which means is it passed or failed the test
        public static bool DoesAttendTestType(int LDLAppID, int TestTypeID)
        {
            bool isFound = false;

            SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"Select top 1 Found = 1 From LocalDrivingLicenseApplications
            Inner join TestAppointments On LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID
                       = TestAppointments.LocalDrivingLicenseApplicationID
            Inner join Tests On TestAppointments.TestAppointmentID = Tests.TestAppointmentID
            Where LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID = @LDLAppID
            And TestAppointments.TestTypeID = @TestTypeID
            Order by TestAppointments.TestAppointmentID Desc";
            SqlCommand cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@LDLAppID", LDLAppID);
            cmd.Parameters.AddWithValue("@TestTypeID", TestTypeID);

            try
            {
                conn.Open();
                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    isFound = true;
                }
            }
            catch (Exception ex)
            {
                isFound = false;
                clsEventLog.setEventLogError(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return isFound;

        }

        // In this method we return how many time's i applied for this test type dependent on test
        public static byte TotalTrialsPerTest(int LDLAppID, int TestTypeID)
        {
            byte TotalTrialsPerTest = 0;

            SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"Select TotalTrialsPerTest = count(TestID) From LocalDrivingLicenseApplications
            INNER JOIN TestAppointments On 
            LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID = TestAppointments.LocalDrivingLicenseApplicationID
            INNER JOIN Tests On TestAppointments.TestAppointmentID = Tests.TestAppointmentID
            Where LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID = @LDLAppID
            and TestAppointments.TestTypeID = @TestTypeID";
            SqlCommand cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@LDLAppID", LDLAppID);
            cmd.Parameters.AddWithValue("@TestTypeID", TestTypeID);

            try
            {
                conn.Open();
                object result = cmd.ExecuteScalar();

                if (result != null && byte.TryParse(result.ToString(), out byte Trials))
                {
                    TotalTrialsPerTest = Trials;
                }
            }
            catch (Exception ex)
            {
                clsEventLog.setEventLogError(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return TotalTrialsPerTest;
        }

        // The responsibility of this method is to check if there is an active schedule for
        // this LocalDrivingLicenseApplicationID and not locked
        public static bool IsThereAnActiveScheduleTest(int LDLAppID, int TestTypeID)
        {
            bool isFound = false;
            SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"Select top 1 Found = 1 from LocalDrivingLicenseApplications
            inner join TestAppointments On 
            LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID = TestAppointments.LocalDrivingLicenseApplicationID
            Where (LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID = @LDLAppID) 
            and (TestAppointments.TestTypeID = @TestTypeID) and IsLocked = 0
            Order by TestAppointments.TestAppointmentID Desc";
            SqlCommand cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@LDLAppID", LDLAppID);
            cmd.Parameters.AddWithValue("@TestTypeID", TestTypeID);

            try
            {
                conn.Open();
                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    isFound = true;
                }
            }
            catch (Exception ex)
            {
                isFound = false;
                clsEventLog.setEventLogError(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return isFound;
        }


    }
}

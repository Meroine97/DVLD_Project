using System;
using System.Data;
using System.Data.SqlClient;

namespace DVLD_DataAccess
{
    public class clsTestAppointmentData
    {

        public static DataTable GetAllTestAppointments()
        {
            DataTable dtTestAppointments = new DataTable();

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString); ;
            string query = @"Select * from TestAppointments_View
                             Order by AppointmentDate Desc";
            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    dtTestAppointments.Load(reader);
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

            return dtTestAppointments;

        }

        public static DataTable GetApplicationTestAppointmentPerTestType(
            int LDLAppID, int TestTypeID)
        {
            DataTable dtTestAppointments = new DataTable();

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString); ;
            string query = @"Select TestAppointmentID, AppointmentDate, PaidFees, IsLocked
                             From TestAppointments
                             Where LocalDrivingLicenseApplicationID = @LDLAppID
                             And TestTypeID = @TestTypeID
                             Order by AppointmentDate Desc";
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@LDLAppID", LDLAppID);
            command.Parameters.AddWithValue("@TestTypeID", TestTypeID);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    dtTestAppointments.Load(reader);
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

            return dtTestAppointments;

        }

        public static bool GetTestAppointmentInfoByID(int TestAppointmentID,
            ref int TestTypeID, ref int LDLAppID, ref DateTime AppointmentDate,
            ref float PaidFees, ref int CreatedByUserID, ref bool IsLocked,
            ref int RetakeTestApplicationID)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"Select * from TestAppointments 
                                      Where TestAppointmentID = @TestAppointmentID";
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@TestAppointmentID", TestAppointmentID);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    isFound = true;

                    TestTypeID = (int)reader["TestTypeID"];
                    LDLAppID = (int)reader["LocalDrivingLicenseApplicationID"];
                    AppointmentDate = (DateTime)reader["AppointmentDate"];
                    PaidFees = Convert.ToSingle(reader["PaidFees"]);
                    CreatedByUserID = (int)reader["CreatedByUserID"];
                    IsLocked = (bool)reader["IsLocked"];

                    if (reader["RetakeTestApplicationID"] != DBNull.Value)
                    {
                        RetakeTestApplicationID = (int)reader["RetakeTestApplicationID"];
                    }
                    else
                    {
                        RetakeTestApplicationID = -1;
                    }
                }
                else
                    isFound = false;

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

        public static bool GetLastTestAppointment(int LDLAppID, int TestTypeID,
            ref int TestAppointmentID, ref DateTime AppointmentDate,
            ref float PaidFees, ref int CreatedByUserID, ref bool IsLocked,
            ref int RetakeTestApplicationID)
        {
            bool isFound = false;
            SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"Select top 1 * From TestAppointments
                             Where TestTypeID = @TestTypeID
                             And LocalDrivingLicenseApplicationID = @LDLAppID
                             Order by TestAppointmentID Desc";
            SqlCommand cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@LDLAppID", LDLAppID);
            cmd.Parameters.AddWithValue("@TestTypeID", TestTypeID);

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    isFound = true;

                    TestAppointmentID = (int)reader["TestAppointmentID"];
                    AppointmentDate = (DateTime)reader["AppointmentDate"];
                    PaidFees = Convert.ToSingle(reader["PaidFees"]);
                    CreatedByUserID = (int)reader["CreatedByUserID"];
                    IsLocked = (bool)reader["IsLocked"];
                    if (reader["RetakeTestApplicationID"] != DBNull.Value)
                        RetakeTestApplicationID = (int)reader["RetakeTestApplicationID"];
                    else
                        RetakeTestApplicationID = -1;
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
                conn.Close();
            }
            return isFound;
        }

        public static int AddNewTestAppointment(int TestTypeID, int LDLAppID,
            DateTime AppointmentDate, float PaidFees, int CreatedByUserID,
            bool IsLocked, int RetakeTestApplicationID)
        {
            int TestAppointmentID = -1;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"Insert into TestAppointments(TestTypeID, LocalDrivingLicenseApplicationID,
                                    AppointmentDate, PaidFees, CreatedByUserID,
                                    IsLocked, RetakeTestApplicationID)
                                    Values(@TestTypeID, @LDLAppID, @AppointmentDate,
                                           @PaidFees, @CreatedByUserID, @IsLocked,
                                           @RetakeTestApplicationID);

                            Select SCOPE_IDENTITY();";
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@TestTypeID", TestTypeID);
            command.Parameters.AddWithValue("@LDLAppID", LDLAppID);
            command.Parameters.AddWithValue("@AppointmentDate", AppointmentDate);
            command.Parameters.AddWithValue("@PaidFees", PaidFees);
            command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);
            command.Parameters.AddWithValue("@IsLocked", IsLocked);

            if (RetakeTestApplicationID != -1)
                command.Parameters.AddWithValue("@RetakeTestApplicationID", RetakeTestApplicationID);
            else
                command.Parameters.AddWithValue("@RetakeTestApplicationID", System.DBNull.Value);

            try
            {
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int InsertedID))
                {
                    TestAppointmentID = InsertedID;
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

            return TestAppointmentID;
        }

        public static bool UpdateTestAppointment(int TestAppointmentID,
            int TestTypeID, int LDLAppID, DateTime AppointmentDate, float PaidFees,
            int CreatedByUserID, bool IsLocked, int RetakeTestApplicationID)
        {
            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"Update TestAppointments
                                    Set TestTypeID = @TestTypeID,
                                        LocalDrivingLicenseApplicationID = @LDLAppID,
                                        AppointmentDate = @AppointmentDate,
                                        PaidFees = @PaidFees,
                                        CreatedByUserID = @CreatedByUserID,
                                        IsLocked = @IsLocked,
                                        RetakeTestApplicationID = @RetakeTestApplicationID
                                    Where TestAppointmentID = @TestAppointmentID";
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@TestAppointmentID", TestAppointmentID);
            command.Parameters.AddWithValue("@TestTypeID", TestTypeID);
            command.Parameters.AddWithValue("@LDLAppID", LDLAppID);
            command.Parameters.AddWithValue("@AppointmentDate", AppointmentDate);
            command.Parameters.AddWithValue("@PaidFees", PaidFees);
            command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);
            command.Parameters.AddWithValue("@IsLocked", IsLocked);

            if (RetakeTestApplicationID != -1)
                command.Parameters.AddWithValue("@RetakeTestApplicationID", RetakeTestApplicationID);
            else
                command.Parameters.AddWithValue("@RetakeTestApplicationID", System.DBNull.Value);

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

            return (rowsAffected > 0);
        }

        public static int GetTestID(int TestAppointmentID)
        {
            int TestID = -1;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"Select TestID From Tests 
                             Where TestAppointmentID = @TestAppointmentID";
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@TestAppointmentID", TestAppointmentID);

            try
            {
                connection.Open();
                object result = command.ExecuteScalar();

                if (result != null && int.TryParse(result.ToString(), out int returnedTestID))
                {
                    TestID = returnedTestID;
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

            return TestID;
        }

    }
}

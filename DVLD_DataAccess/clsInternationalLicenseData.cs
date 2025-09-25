using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace DVLD_DataAccess
{
    public class clsInternationalLicenseData
    {

        public static DataTable GetAllInternationalLicense()
        {
            DataTable dtInterLicense = new DataTable();

            SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"Select InternationalLicenseID, ApplicationID, DriverID,
                                    IssuedUsingLocalLicenseID, IssueDate, ExpirationDate,
                                    IsActive From InternationalLicenses 
                              Order By IsActive, ExpirationDate Desc";
            SqlCommand cmd = new SqlCommand(query, conn);

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if(reader.HasRows)
                {
                    dtInterLicense.Load(reader);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                clsEventLog.setEventLogError(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return dtInterLicense;
        }

        public static DataTable GetDriverInternationalLicenses(int DriverID)
        {
            DataTable dtInterLicense = new DataTable();

            SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"Select InternationalLicenseID, ApplicationID, DriverID,
                                    IssuedUsingLocalLicenseID, IssueDate, ExpirationDate,
                                    IsActive From InternationalLicenses 
                              Where DriverID = @DriverID
                              Order By IsActive, ExpirationDate Desc";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@DriverID", DriverID);

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    dtInterLicense.Load(reader);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                clsEventLog.setEventLogError(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return dtInterLicense;
        }

        public static bool GetInternationalLicenseByID(int InternationalLicenseID, ref int ApplicationID,
            ref int DriverID, ref int IssuedUsingLocalLicenseID, ref DateTime IssueDate, 
            ref DateTime ExpirationDate, ref bool isActive, ref int CreatedByUserID)
        {
            bool isFound = false;

            SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"Select * From InternationalLicenses 
                                      Where InternationalLicenseID = @InternationalLicenseID";
            SqlCommand cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@InternationalLicenseID", InternationalLicenseID);

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    isFound = true;

                    ApplicationID = (int)reader["ApplicationID"];
                    DriverID = (int)reader["DriverID"];
                    IssuedUsingLocalLicenseID = (int)reader["IssuedUsingLocalLicenseID"];
                    IssueDate = (DateTime)reader["IssueDate"];
                    ExpirationDate = (DateTime)reader["ExpirationDate"];
                    isActive = (bool)reader["IsActive"];
                    CreatedByUserID = (int)reader["CreatedByUserID"];

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


        public static bool GetInternationalLicenseByDriverID(int DriverID, ref int InternationalLicenseID, 
            ref int ApplicationID, ref int IssuedUsingLocalLicenseID, ref DateTime IssueDate,
            ref DateTime ExpirationDate, ref bool isActive, ref int CreatedByUserID)
        {
            bool isFound = false;

            SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"Select * From InternationalLicenses 
                                      Where DriverID = @DriverID";
            SqlCommand cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@DriverID", DriverID);

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    isFound = true;

                    InternationalLicenseID = (int)reader["InternationalLicenseID"];
                    ApplicationID = (int)reader["ApplicationID"];
                    IssuedUsingLocalLicenseID = (int)reader["IssuedUsingLocalLicenseID"];
                    IssueDate = (DateTime)reader["IssueDate"];
                    ExpirationDate = (DateTime)reader["ExpirationDate"];
                    isActive = (bool)reader["IsActive"];
                    CreatedByUserID = (int)reader["CreatedByUserID"];

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

        public static int AddNewInternationalLicense(int ApplicationID, int DriverID, int IssuedUsingLocalLicenseID, 
            DateTime IssueDate, DateTime ExpirationDate, bool IsActive, int CreatedByUserID)
        {
            int InternationalLicenseID = -1;

            SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"UPDATE InternationalLicenses 
                             Set IsActive = 0 Where DriverID = @DriverID;

                             INSERT INTO InternationalLicenses(ApplicationID, DriverID, IssuedUsingLocalLicenseID,
                                   IssueDate, ExpirationDate, IsActive, CreatedByUserID)
                             Values(@ApplicationID, @DriverID, @IssuedUsingLocalLicenseID, @IssueDate,
                                    @ExpirationDate, @IsActive, @CreatedByUserID);

                            SELECT SCOPE_IDENTITY();";
            SqlCommand cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@ApplicationID", ApplicationID);
            cmd.Parameters.AddWithValue("@DriverID", DriverID);
            cmd.Parameters.AddWithValue("@IssuedUsingLocalLicenseID", IssuedUsingLocalLicenseID);
            cmd.Parameters.AddWithValue("@IssueDate", IssueDate);
            cmd.Parameters.AddWithValue("@ExpirationDate", ExpirationDate);
            cmd.Parameters.AddWithValue("@IsActive", IsActive);
            cmd.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

            try
            {
                conn.Open();
                object result = cmd.ExecuteScalar();
                if(result != null && int.TryParse(result.ToString(), out int InsertedID))
                {
                    InternationalLicenseID = InsertedID;
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

            return InternationalLicenseID;
        }

        public static bool UpdateInternationalLicense(int InternationalLicenseID, int ApplicationID, int DriverID,
            int IssuedUsingLocalLicenseID, DateTime IssueDate, DateTime ExpirationDate, 
            bool IsActive, int CreatedByUserID)
        {
            int rowsAffected = 0;

            SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"UPDATE InternationalLicenses
                                    SET ApplicationID = @ApplicationID,
                                        DriverID = @DriverID,
                                        IssuedUsingLocalLicenseID = @IssuedUsingLocalLicenseID,
                                        IssueDate = @IssueDate,
                                        ExpirationDate = @ExpirationDate,
                                        IsActive = @IsActive,
                                        CreatedByUserID = @CreatedByUserID
                                  Where InternationalLicenseID = @InternationalLicenseID";
            SqlCommand cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@InternationalLicenseID", InternationalLicenseID);
            cmd.Parameters.AddWithValue("@ApplicationID", ApplicationID);
            cmd.Parameters.AddWithValue("@DriverID", DriverID);
            cmd.Parameters.AddWithValue("@IssuedUsingLocalLicenseID", IssuedUsingLocalLicenseID);
            cmd.Parameters.AddWithValue("@IssueDate", IssueDate);
            cmd.Parameters.AddWithValue("@ExpirationDate", ExpirationDate);
            cmd.Parameters.AddWithValue("@IsActive", IsActive);
            cmd.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

            try
            {
                conn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                clsEventLog.setEventLogError(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return (rowsAffected > 0);
        }

        public static int GetActiveInternationalLicenseIDByDriverID(int DriverID)
        {
            int InternationalLicenseID = -1;
            SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"Select InternationalLicenseID From InternationalLicenses
                             Where DriverID = @DriverID And GetDate() Between IssueDate And ExpirationDate
                             Order By ExpirationDate Desc";
            SqlCommand cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@DriverID", DriverID);

            try
            {
                conn.Open();
                object result = cmd.ExecuteScalar();
                if(result != null && int.TryParse(result.ToString(), out int ActiveLicenseID))
                {
                    InternationalLicenseID = ActiveLicenseID;
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

            return InternationalLicenseID;
        }
    }
}

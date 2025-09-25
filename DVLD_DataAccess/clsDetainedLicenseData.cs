using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_DataAccess
{
    public class clsDetainedLicenseData
    {

        public static DataTable GetAllDetianedLicenses()
        {
            DataTable dtDetainedLicense = new DataTable();

            SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string Query = @"Select * From DetainedLicenses_View Order By IsReleased, DetainID";
            SqlCommand cmd = new SqlCommand(Query, conn);

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if(reader.HasRows)
                {
                    dtDetainedLicense.Load(reader);
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

            return dtDetainedLicense;
        }

        public static bool GetDetainedLicenseByID(int DetainID, ref int LicenseID,
            ref DateTime DetainDate, ref float FineFees, ref int CreatedByUserID,
            ref bool IsReleased, ref DateTime ReleaseDate, ref int ReleasedByUserID,
            ref int ReleaseApplicationID)
        {
            bool isFound = false;

            SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string Query = @"Select * From DetainedLicenses Where DetainID = @DetainID";
            SqlCommand cmd = new SqlCommand(Query, conn);

            cmd.Parameters.AddWithValue("@DetainID", DetainID);

            try
            {
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                if(reader.Read())
                {
                    isFound = true;

                    LicenseID = (int)reader["LicenseID"];
                    DetainDate = (DateTime)reader["DetainDate"];
                    FineFees = Convert.ToSingle(reader["FineFees"]);
                    CreatedByUserID = (int)reader["CreatedByUserID"];
                    IsReleased = (bool)reader["IsReleased"];

                    if (reader["ReleaseDate"] != System.DBNull.Value)
                        ReleaseDate = (DateTime)reader["ReleaseDate"];
                    else
                        ReleaseDate = DateTime.MaxValue;

                    if (reader["ReleasedByUserID"] != System.DBNull.Value)
                        ReleasedByUserID = (int)reader["ReleasedByUserID"];
                    else
                        ReleasedByUserID = -1;

                    if (reader["ReleaseApplicationID"] != System.DBNull.Value)
                        ReleaseApplicationID = (int)reader["ReleaseApplicationID"];
                    else
                        ReleaseApplicationID = -1;

                }
                else
                {
                    isFound = false;
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

        public static bool GetDetainedLicenseInfoByLicenseID(int LicenseID, ref int DetainID,
            ref DateTime DetainDate, ref float FineFees, ref int CreatedByUserID,
            ref bool IsReleased, ref DateTime ReleaseDate, ref int ReleasedByUserID,
            ref int ReleaseApplicationID)
        {
            bool isFound = false;

            SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string Query = @"Select top 1 * From DetainedLicenses Where LicenseID = @LicenseID
                                            Order By DetainID desc";
            SqlCommand cmd = new SqlCommand(Query, conn);

            cmd.Parameters.AddWithValue("@LicenseID", LicenseID);

            try
            {
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    isFound = true;

                    DetainID = (int)reader["DetainID"];
                    DetainDate = (DateTime)reader["DetainDate"];
                    FineFees = Convert.ToSingle(reader["FineFees"]);
                    CreatedByUserID = (int)reader["CreatedByUserID"];
                    IsReleased = (bool)reader["IsReleased"];

                    if (reader["ReleaseDate"] != System.DBNull.Value)
                        ReleaseDate = (DateTime)reader["ReleaseDate"];
                    else
                        ReleaseDate = DateTime.MaxValue;

                    if (reader["ReleasedByUserID"] != System.DBNull.Value)
                        ReleasedByUserID = (int)reader["ReleasedByUserID"];
                    else
                        ReleasedByUserID = -1;

                    if (reader["ReleaseApplicationID"] != System.DBNull.Value)
                        ReleaseApplicationID = (int)reader["ReleaseApplicationID"];
                    else
                        ReleaseApplicationID = -1;

                }
                else
                {
                    isFound = false;
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

        public static int AddNewDetainedLicense(int LicenseID, DateTime DetainDate,
            float FineFees, int CreatedByUserID)
        {
            int DetainedID = -1;

            SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string Query = @"INSERT INTO DetainedLicenses(LicenseID, DetainDate, FineFees,
                                         CreatedByUserID, IsReleased)

                                  Values(@LicenseID, @DetainDate, @FineFees, @CreatedByUserID, 0);

                            Select SCOPE_IDENTITY();";
            SqlCommand cmd = new SqlCommand(Query, conn);

            cmd.Parameters.AddWithValue("@LicenseID", LicenseID);
            cmd.Parameters.AddWithValue("@DetainDate", DetainDate);
            cmd.Parameters.AddWithValue("@FineFees", FineFees);
            cmd.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

            try
            {
                conn.Open();

                object result = cmd.ExecuteScalar();
                if(result != null && int.TryParse(result.ToString(), out int InsertedID))
                {
                    DetainedID = InsertedID;
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

            return DetainedID;
        }

        public static bool UpdateDetainedLicense(int DetainID, int LicenseID, DateTime DetainDate, 
            float FineFees, int CreatedByUserID)
        {
            int RowsAffected = 0;

            SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string Query = @"UPDATE DetainedLicenses
                                        Set LicenseID = @LicenseID,
                                            DetainDate = @DetainDate,
                                            FineFees = @FineFees,
                                            CreatedByUserID = @CreatedByUserID
                                      Where DetainID = @DetianID";
            SqlCommand cmd = new SqlCommand(Query, conn);

            cmd.Parameters.AddWithValue("@DetainID", DetainID);
            cmd.Parameters.AddWithValue("@LicenseID", LicenseID);
            cmd.Parameters.AddWithValue("@DetainDate", DetainDate);
            cmd.Parameters.AddWithValue("@FineFees", FineFees);
            cmd.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

            try
            {
                conn.Open();

                RowsAffected = cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                clsEventLog.setEventLogError(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return (RowsAffected > 0);
        }

        public static bool ReleaseDetainedLicense(int DetainID, int ReleasedByUserID,
            int ReleaseApplicationID)
        {
            int RowsAffected = 0;

            SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"Update DetainedLicenses
                                        Set IsReleased = 1,
                                            ReleaseDate = @ReleaseDate,
                                            ReleasedByUserID = @ReleasedByUserID,
                                            ReleaseApplicationID = @ReleaseApplicationID
                                    Where DetainID = @DetainID";
            SqlCommand cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@DetainID", DetainID);
            cmd.Parameters.AddWithValue("@ReleaseDate", DateTime.Now);

            if(ReleasedByUserID != -1)
                cmd.Parameters.AddWithValue("@ReleasedByUserID", ReleasedByUserID);
            else
                cmd.Parameters.AddWithValue("@ReleasedByUserID", DBNull.Value);

            if(ReleaseApplicationID != -1)
                cmd.Parameters.AddWithValue("@ReleaseApplicationID", ReleaseApplicationID);
            else
                cmd.Parameters.AddWithValue("@ReleaseApplicationID", DBNull.Value);

            try
            {
                conn.Open();

                RowsAffected = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                clsEventLog.setEventLogError(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return (RowsAffected > 0);
        }

        public static bool IsLicenseDetained(int LicenseID)
        {
            bool isDetained = false;

            SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"Select isDetained = 1 From DetainedLicenses
                                    Where LicenseID = @LicenseID
                                    And IsReleased = 0;";
            SqlCommand cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@LicenseID", LicenseID);

            try
            {
                conn.Open();
                object result = cmd.ExecuteScalar();
                if(result != null)
                {
                    isDetained = Convert.ToBoolean(result);
                }
            }
            catch (Exception ex)
            {
                isDetained = false;
                clsEventLog.setEventLogError(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return isDetained;
        }

    }
}

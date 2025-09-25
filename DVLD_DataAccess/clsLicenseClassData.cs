using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_DataAccess
{
    public class clsLicenseClassData
    {

        public static DataTable GetAllLicenseClasses()
        {
            DataTable dtLicenseClass = new DataTable();

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"Select * From LicenseClasses Order by ClassName";
            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if(reader.HasRows)
                {
                    dtLicenseClass.Load(reader);
                }

                dtLicenseClass.Load(reader);
            }
            catch (Exception ex)
            {
                clsEventLog.setEventLogError(ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return dtLicenseClass;
        }

        public static bool GetLicenseClassByID(int ID, ref string className,
            ref string classDescription, ref byte minAllowAge,
            ref byte defaultValidityLength, ref float classFees)
        {
            bool isFound = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"Select * From LicenseClasses
                                      Where LicenseClassID = @ID";
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@ID", ID);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if(reader.Read()) 
                {
                    isFound = true;

                    className = (string)reader["ClassName"];
                    classDescription = (string)reader["ClassDescription"];
                    minAllowAge = (byte)reader["MinimumAllowedAge"];
                    defaultValidityLength = (byte)reader["DefaultValidityLength"];
                    classFees = Convert.ToSingle(reader["ClassFees"]);
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

        public static bool GetLicenseClassByClassName(string ClassName, ref int ID,
            ref string classDescription, ref byte minAllowAge,
            ref byte defaultValidityLength, ref float classFees)
        {
            bool isFound = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"Select * From LicenseClasses
                                      Where ClassName = @ClassName";
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@ClassName", ClassName);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    isFound = true;

                    ID = (int)reader["LicenseClassID"];
                    classDescription = (string)reader["ClassDescription"];
                    minAllowAge = (byte)reader["MinimumAllowedAge"];
                    defaultValidityLength = (byte)reader["DefaultValidityLength"];
                    classFees = Convert.ToSingle(reader["ClassFees"]);
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

        public static int AddNewLicenseClass(string className, string classDescription,
            byte minAllowAge, byte defaultValidityLength, float classFees)
        {
            int licenseClassID = -1;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"Insert into LicenseClasses(ClassName, ClassDescription,
                                    MinimumAllowedAge, DefaultValidityLength, ClassFees)
                                    Values(@className, @classDescription,
                                    @minAllowAge, @defaultValidityLength, @classFees);

                             Select SCOPE_IDENTITY();";
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@className", className);
            command.Parameters.AddWithValue("@classDescription", classDescription);
            command.Parameters.AddWithValue("@minAllowAge", minAllowAge);
            command.Parameters.AddWithValue("@defaultValidityLength", defaultValidityLength);
            command.Parameters.AddWithValue("@classFees", classFees);

            try
            {
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int InsertedID))
                {
                    licenseClassID = InsertedID;
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

            return licenseClassID;
        }

        public static bool UpdateLicenseClass(int ID, string className,
            string classDescription, byte minAllowAge, byte defaultValidityLength, 
            float classFees)
        {
            int rowsAffected = 0;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"Update LicenseClasses
                                    Set ClassName = @className,
                                        ClassDescription = @classDescription,
                                        MinimumAllowedAge = @minAllowAge,
                                        DefaultValidityLength = @defaultValidityLength,
                                        ClassFees = @classFees

                             Where LicenseClassID = @ID";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@ID", ID);
            command.Parameters.AddWithValue("@className", className);
            command.Parameters.AddWithValue("@classDescription", classDescription);
            command.Parameters.AddWithValue("@minAllowAge", minAllowAge);
            command.Parameters.AddWithValue("@defaultValidityLength", defaultValidityLength);
            command.Parameters.AddWithValue("@classFees", classFees);

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

        public static bool DeleteLicenseClass(int ID)
        {
            int rowsAffected = 0;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"Delete From LicenseClasses
                                        Where LicenseClassID = @ID";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@ID", ID);

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

    }
}

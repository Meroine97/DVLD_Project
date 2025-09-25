using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_DataAccess
{
    public class clsTestTypesData
    {

        public static DataTable GetAllTestTypes()
        {
            DataTable dtTestTypes = new DataTable();

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"Select * From TestTypes Order By TestTypeID";
            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    dtTestTypes.Load(reader);
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

            return dtTestTypes;
        }

        public static bool GetTestTypeByID(int ID, ref string Title,
            ref string Description, ref float Fees)
        {
            bool isFound = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"Select * From TestTypes 
                                Where TestTypeID = @ID";
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@ID", ID);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    isFound = true;

                    Title = (string)reader["TestTypeTitle"];
                    Description = (string)reader["TestTypeDescription"];
                    Fees = Convert.ToSingle(reader["TestTypeFees"]);
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

        public static int AddNewTestType(string Title, string Description, 
            float Fees)
        {
            int TestTypeID = -1;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"Insert Into TestTypes (TestTypeTitle, TestTypeDescription,
                                                    TestTypeFees)
                                    Values(@Title, @Description, @Fees);

                             Select SCOPE_IDENTITY();";
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Title", Title);
            command.Parameters.AddWithValue("@Description", Description);
            command.Parameters.AddWithValue("@Fees", Fees);

            try
            {
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int InsertedID))
                {
                    TestTypeID = InsertedID;
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

            return TestTypeID;
        }

        public static bool UpdateTestType(int ID, string Title, string Description,
            float Fees)
        {
            int RowsAffected = 0;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"Update TestTypes
                                    SET TestTypeTitle = @Title,
                                        TestTypeDescription = @Description,
                                        TestTypeFees = @Fees
                                    Where TestTypeID = @ID";
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@ID", ID);
            command.Parameters.AddWithValue("@Title", Title);
            command.Parameters.AddWithValue("@Description", Description);
            command.Parameters.AddWithValue("@Fees", Fees);

            try
            {
                connection.Open();
                RowsAffected = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                clsEventLog.setEventLogError(ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return (RowsAffected > 0);
        }


    }
}

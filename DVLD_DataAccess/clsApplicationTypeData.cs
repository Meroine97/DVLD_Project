using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_DataAccess
{
    public class clsApplicationTypeData
    {

        public static DataTable GetAllApplicationTypes()
        {
            DataTable dtAppTypes = new DataTable();

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"Select * From ApplicationTypes Order By ApplicationTypeID";
            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if(reader.HasRows) 
                {
                    dtAppTypes.Load(reader);
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

            return dtAppTypes;
        }

        public static bool GetApplicationTypeByID(int ID, ref string Title,
            ref float fees)
        {
            bool isFound = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"Select * From ApplicationTypes 
                                Where ApplicationTypeID = @ID";
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@ID", ID);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if(reader.Read())
                {
                    isFound = true;

                    Title = (string)reader["ApplicationTypeTitle"];
                    fees = Convert.ToSingle(reader["ApplicationFees"]);
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

        public static int AddNewApplicationType(string Title, float Fees)
        {
            int AppTypeID = -1;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"Insert Into ApplicationTypes (ApplicationTypeTitle, ApplicationFees)
                                    Values(@Title, @Fees);

                             Select SCOPE_IDENTITY();";
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Title", Title);
            command.Parameters.AddWithValue("@Fees", Fees);

            try
            {
                connection.Open();
                object result = command.ExecuteScalar();
                if(result != null && int.TryParse(result.ToString(), out int InsertedID))
                {
                    AppTypeID = InsertedID;
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

            return AppTypeID;
        }

        public static bool UpdateApplicationType(int ID, string Title, float Fees)
        {
            int RowsAffected = 0;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"Update ApplicationTypes
                                    SET ApplicationTypeTitle = @Title,
                                        ApplicationFees = @Fees
                                    Where ApplicationTypeID = @ID";
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@ID", ID);
            command.Parameters.AddWithValue("@Fees", Fees);
            command.Parameters.AddWithValue("@Title", Title);

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

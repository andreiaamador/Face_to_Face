using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Face2Face.Models
{
    public class CountryModel
    {
        #region DatabaseMethod  
        public List<T> ConvertTo<T>(DataTable datatable) where T : new()
        {
            List<T> Temp = new List<T>();
            try
            {
                List<string> columnsNames = new List<string>();
                foreach (DataColumn DataColumn in datatable.Columns)
                    columnsNames.Add(DataColumn.ColumnName);
                Temp = datatable.AsEnumerable().ToList().ConvertAll<T>(row => getObject<T>(row, columnsNames));
                return Temp;
            }
            catch
            {
                return Temp;
            }

        }
        public T getObject<T>(DataRow row, List<string> columnsName) where T : new()
        {
            T obj = new T();
            try
            {
                string columnname = "";
                string value = "";
                PropertyInfo[] Properties;
                Properties = typeof(T).GetProperties();
                foreach (PropertyInfo objProperty in Properties)
                {
                    columnname = columnsName.Find(name => name.ToLower() == objProperty.Name.ToLower());
                    if (!string.IsNullOrEmpty(columnname))
                    {
                        value = row[columnname].ToString();
                        if (!string.IsNullOrEmpty(value))
                        {
                            if (Nullable.GetUnderlyingType(objProperty.PropertyType) != null)
                            {
                                value = row[columnname].ToString().Replace("$", "").Replace(",", "");
                                objProperty.SetValue(obj, Convert.ChangeType(value, Type.GetType(Nullable.GetUnderlyingType(objProperty.PropertyType).ToString())), null);
                            }
                            else
                            {
                                value = row[columnname].ToString();
                                objProperty.SetValue(obj, Convert.ChangeType(value, Type.GetType(objProperty.PropertyType.ToString())), null);
                            }
                        }
                    }
                }
                return obj;
            }
            catch
            {
                return obj;
            }
        }
        #endregion

        SqlConnection con;
        SqlDataAdapter adap;
        DataTable dt;
        SqlCommand cmd;
        public CountryModel()
        {
            string conn = ConfigurationManager.ConnectionStrings["CountryConnectionString"].ConnectionString;
            con = new SqlConnection(conn);
        }

        public List<Country> GetCountries()
        {
            List<Country> countries = new List<Country>();
            try
            {
                con.Open();
                adap = new SqlDataAdapter();
                dt = new DataTable();
                cmd = new SqlCommand("GetCountries", con);
                cmd.CommandType = CommandType.StoredProcedure;

                adap.SelectCommand = cmd;
                adap.Fill(dt);
                countries = ConvertTo<Country>(dt);
            }
            catch (Exception x)
            {

            }
            finally
            {
                //cmd.Dispose();
                con.Close();
            }
            return countries;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;

namespace MySqlNEaT
{
    public class DBDriverMySql : IDBDriver
    {
        // empty space always at the beggining of appended command!
        // non-string && non-deployable ValueType  => param, string => copy straight, other => wrong

        private MySqlConnection conn;
        // logTable declared in Environment, defined here as string query, int time (miliseconds)
        
        
        // parsed from the query beginning so that we can throw an exception when the query type doesnt match the method used to execute it
        private enum QueryType
        {
            Unknown, Insert, Update, Delete, Select
        };

        public DBDriverMySql(string connstring)
        {
            conn = new MySqlConnection(connstring);
        }

        private QueryType getQueryType(string query)
        {
            query = query.Trim();
            string firstWord = query.Split(' ').First();
            switch (firstWord.ToUpper())
            {
                case "SELECT":
                case "SHOW":
                    return QueryType.Select;
                case "INSERT":
                    return QueryType.Insert;
                case "UPDATE":
                    return QueryType.Update;
                case "DELETE":
                    return QueryType.Delete;
                default:
                    throw new FormatException("Unrecognised type of MySql Command");
            }
        }

        
        /// <summary>
        /// Creates a command by concatenating the provided arguments, preferably IMySqlQueryDeployable, which will translate themselves into SQL. ValueTypes will be passed
        /// as parameters and string will directly copied to the query.
        /// </summary>
        /// <param name="parts">IMySQLQueryDeployable objects, strings or ValueTypes</param>
        /// <returns></returns>
        protected MySqlCommand translate(params object[] parts)
        {

            int paramCount = 0;
            MySqlCommand resultCmd = new MySqlCommand();
            StringBuilder resultQuery = new StringBuilder();

            foreach (object part in parts)
            {
                if (part is string)
                {         // strings are directly appended
                    string pString = (string)part;
                    resultQuery.Append(" " + pString + " ");

                }
                else if (part is IMySqlQueryDeployable)
                {
                    ((IMySqlQueryDeployable)part).Deoploy(resultCmd, resultQuery, ref paramCount);
                }
                else if (part is ValueType)
                {
                    resultQuery.Append(" @param" + paramCount);
                    resultCmd.Parameters.AddWithValue("@param" + paramCount++, part);
                }
                else throw new FormatException("Unrecognised query part " + part.GetType().ToString());
            }

            resultCmd.CommandText = resultQuery.ToString();
            return resultCmd;
        }


        /// <summary>
        /// Composes an SQL query from the passed parts and returns the result in a table - the query MUST be an SELECT statement.
        /// </summary>
        /// <param name="parts">IMySQLQueryDeployable objects, strings or ValueTypes</param>
        /// <returns></returns>        
        public System.Data.DataTable FetchAll(params object[] parts)
        {
            MySqlCommand cmd = translate(parts);
            cmd.Connection = conn;
            QueryType type = this.getQueryType(cmd.CommandText);
            if (type != QueryType.Select)
            {
                throw new Exception("Trying to fetch from a non-select query");
            }
            DataTable result = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            
            try
            {
                conn.Open();
                adapter.Fill(result);
            }
            finally
            {
                    conn.Close();
            }

            return result;
        }
        /// <summary>
        /// Composes an SQL query from the passed parts and returns the first row of the result in a DataRow - the query MUST be an SELECT statement.
        /// </summary>
        /// <param name="parts">IMySQLQueryDeployable objects, strings or ValueTypes</param>
        /// <returns></returns>        
        public System.Data.DataRow Fetch(params object[] parts)
        {
            DataTable table = this.FetchAll(parts);
            if (table.Rows.Count > 0)
                return table.Rows[0];
            else
                return null;
        }

        /// <summary>
        /// Composes an SQL query from the passed parts and returns the result the first cell of the result - the query MUST be an SELECT statement.
        /// </summary>
        /// <param name="parts">IMySQLQueryDeployable objects, strings or ValueTypes</param>
        /// <returns></returns>
        public object FetchSingle(params object[] parts)
        {
            DataTable table = this.FetchAll(parts);
            if (table.Rows.Count > 0)
                return table.Rows[0][0];
            else
                return null;
        }
        /// <summary>
        /// Composes an SQL query from the passed parts and executes it.
        /// </summary>
        /// <param name="parts">IMySQLQueryDeployable objects, strings or ValueTypes</param>
        /// <returns>Number of affected rows</returns>
        public int Query(params object[] parts)
        {
            MySqlCommand cmd = this.translate(parts);
            cmd.Connection = conn;
            int rowsAffected = 0;

            try
            {
                
                conn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }
            finally
            {
                    conn.Close();
            }
            return rowsAffected;
        }

        
        public int LastId()
        {
            string id = FetchSingle("SELECT LAST_INSERT_ID()").ToString();
            return Int32.Parse(id);
        }
        public int NextAIForTable(string tableName)
        {
            DataRow res = Fetch("SHOW TABLE STATUS LIKE '" + tableName + "'");
            return (int)res["Auto_increment"];
        }

    }
}

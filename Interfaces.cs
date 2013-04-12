using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace MySqlNEaT
{
    public interface IDBDriver
    {
        /// <summary>
        /// Creates a query (must be a SELECT) from the given parts, executes it and fills the result into a datatable
        /// </summary>
        /// <param name="parts">IDBDeployable objects / strings / ValueTypes</param>
        /// <returns></returns>
        DataTable FetchAll(params object[] parts);
        /// <summary>
        /// Similiar to FetchAll, but returns only the firest row
        /// </summary>
        /// <param name="parts"></param>
        /// <returns></returns>
        DataRow Fetch(params object[] parts);
        /// <summary>
        /// Like Fetch, but returns the first cell only
        /// </summary>
        /// <param name="parts"></param>
        /// <returns></returns>
        object FetchSingle(params object[] parts);
        /// <summary>
        /// Executes an arbitrary query, returns the number of affected rows
        /// </summary>
        /// <param name="parts"></param>
        /// <returns></returns>
        int Query(params object[] parts);
    }

    public interface IMySqlQueryDeployable
    {
        void Deoploy(MySqlCommand cmd, StringBuilder sb, ref int paramCount);
    }

    public interface IDbDeployableFactory
    {
        IDbInStr InStr(string s);   // use this for a string parameter; simple string objects will be copied to the query directly.

        IDbVals InsVals(Dictionary<string, object> vals); 
        IDbVals InsVals(DataRow r);

        IDbVals UpdVals(Dictionary<string, object> vals);
        IDbVals UpdVals(DataRow r);

        IDbCol Col(string column);
        IDbCol Col(string column, string alias);
        IDbCol Col(string table, string column, string alias);

        IMySqlQueryDeployable Cols(List<IDbCol> cols);
        IMySqlQueryDeployable Cols(List<string> colNames);

        IDbJoin Join(FK fk);
        IDbJoin Join(FK fk, string alias);

        IMySqlQueryDeployable Joins(List<IDbJoin> joins);
        IMySqlQueryDeployable Joins(List<FK> FKs);

        IDbInList InList(List<object> list);
        IMySqlQueryDeployable Condition(DataRow lowerBounds, DataRow upperBounds = null);

        IDbTable Table(string table, string alias = null);
    }

    public interface IDbInStr : IMySqlQueryDeployable
    {
        string s { get; set; }

    }
    public interface IDbVals : IMySqlQueryDeployable
    {
        Dictionary<string, object> vals { get; set; }

    }
    public interface IDbCol : IMySqlQueryDeployable
    {
        string table { get; set; }
        string column { get; set; }
        string alias { get; set; }
    }
    public interface IDbCols : IMySqlQueryDeployable
    {
        List<IDbCol> cols { get; set; }
    }
    public interface IDbJoin : IMySqlQueryDeployable
    {
        FK fk { get; set; }
        string alias { get; set; }
    }
    public interface IDbJoins : IMySqlQueryDeployable
    {
        List<IDbJoin> joins { get; set; }
    }
    public interface IDbInList : IMySqlQueryDeployable
    {
        List<object> list { get; set; }
    }

    public interface IDbTable : IMySqlQueryDeployable
    {
        string table { get; set; }
        string alias { get; set; }
    }
}

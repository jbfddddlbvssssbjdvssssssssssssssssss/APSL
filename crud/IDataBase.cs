using System;
using System.Collections.Generic;
using System.Data;

namespace CRUD
{
    public interface IDataBase<T>
    {
        public KeyValuePair<T, string> connect(Dictionary<string, string> settings);
        public KeyValuePair<DataTable,string> getQuery(string query);
        public string setQuery(string query);

        public static List<R> transformData<L, R>(Func<L, R> transformer, List<L> dataList)
        {
            List<R> result = new List<R>();

            foreach(L instance in dataList)
            {
                result.Add(transformer(instance));
            }

            return result;
        }
        public List<O> select<O>(string tableName,Predicate<O> predicate) where O : DB_Element<O>, new(); 
        public void insert<O>(string tableName, List<O> data) where O : DB_Element<O>, new();
        public void update<O>(string tableName, Predicate<O> predicate, Func<O, O> updateObject) where O : DB_Element<O>, new();
        public void remove<O>(string tableName, Predicate<O> predicate) where O : DB_Element<O>, new();
        public void disconnect();
    }

    public interface DB_Element<T> where T : new()
    {
        public T asObject(DataRow row);
        public string asString();
    }

}

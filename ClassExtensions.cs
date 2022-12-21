using System;
using System.Collections.Generic;
using System.Data;

namespace Extensions
{
    static class Extensions
    {
        private static UInt16[] getColumnLengths(this DataTable table)
        {
            UInt16[] result = new UInt16[table.Columns.Count];

            for(int k = 0;k < result.Length; k++)
            {
                int res = table.Columns[k].ColumnName.Length;
                for (int j = 0; j < table.Rows.Count;j++) res = Math.Max(res,table.Rows[j][k].ToString().Length);
                result[k] = Convert.ToUInt16(res); 
            }

            return result;
        }
        public static void Show(this DataTable table)
        {
            UInt16[] cLenghts = table.getColumnLengths();
            int rows_count = table.Rows.Count;
            string horizontal_border = $"+{Repeat("-", rows_count.ToString().Length)}+";
            string header = $"|{Repeat("#",rows_count.ToString().Length)}|";

            for(int k = 0;k < cLenghts.Length; k++)
            {
                horizontal_border += $"{Repeat("-", cLenghts[k])}+";
                header += String.Format("{0,"+ cLenghts[k].ToString() +"}|", table.Columns[k].ColumnName);
            }

            Console.Write($"{horizontal_border}\n{header}\n{horizontal_border}\n");

            for (int k = 0; k < rows_count; k++)
            {
                Console.Write(String.Format("|{0,"+rows_count.ToString().Length+"}|",k));
                for(int j = 0;j < cLenghts.Length; j++)
                {
                    Console.Write(String.Format("{0,"+cLenghts[j]+"}|",table.Rows[k][j].ToString()));
                }
                Console.WriteLine("\n"+horizontal_border);
            }

        }

        public static string Repeat(this string left,int times)
        {
            string result = String.Empty;
            for(uint k = 0;k < times; k++)
            {
                result += left;
            }
            return result;
        }
        public static string Trim(this string text, string erase_symbols)
        {
            foreach (char c in erase_symbols) text = text.Replace(c.ToString(), "");
            return text;
        }

        public static void ShowTabs<T>(this List<T> list)
        {
            list.Show($"{list.Count} :\n\t", "\n\t", $"\r;");
        }

        public static void ShowCommas<T>(this List<T> list)
        {
            list.Show($"{list.Count} : ",", ","");
        }

        public static void Show<T>(this List<T> list,string start,string sepparator,string end)
        {
            Console.Write(start);
            foreach (T item in list)
            {
                Console.Write($"{item}{sepparator}");
            }
            Console.Write(end);
        }
        public static void Show<T>(this T[] array)
        {
            Console.Write($"{array.Length} : ");
            foreach (T item in array)
            {
                Console.Write($"{item}, ");
            }
            Console.Write($"\b\b;\n");
        }

        public static List<F> MapOn<T,F>(this List<T> list,Func<T,F> transform)
        {
            List<F> result = new List<F>();

            foreach (T item in list)
            {
                result.Add(transform(item));
            }

            return result;
        }

        public static void MapIn<T>(this List<T> list,Func<T,T> transform)
        {
            for(int k = 0;k < list.Count; k++)
            {
                list[k] = transform(list[k]);
            }
        }

    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDI_Homework_5.Utils
{
    public class MyResult : IEnumerable<Dictionary<string, object>>
    {
        List<Dictionary<string, object>> rows;

        public MyResult()
        {
            rows = new List<Dictionary<string, object>>();
        }

        public void AddRow(string[] fields, object[] values)
        {
            var dict = new Dictionary<string, object>();
            for (int i = 0; i < fields.Length; i++)
                dict.Add(fields[i], values[i]);
            rows.Add(dict);
        }
        
        public IEnumerator<Dictionary<string, object>> GetEnumerator()
        {
            return rows.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
}

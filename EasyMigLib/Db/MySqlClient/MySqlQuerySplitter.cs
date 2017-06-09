using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace EasyMigLib.Db.MySqlClient
{
    public class QuerySplit
    {
        public string Delimiter { get;  }
        public string Content { get; set; }

        public QuerySplit(string delimiter, string content)
        {
            this.Delimiter = delimiter;
            this.Content = content;
        }
    }

    public class MySqlQuerySplitter : IQuerySplitter
    {
        public void Concat(List<string> list, IEnumerable<string> toAdd)
        {
            foreach (var item in toAdd)
            {
                list.Add(item);
            }
        }

        public string[] SplitOnDELIMITERKeyword(string query)
        {
            var result = new List<string>();
            var regex = new Regex("\\s*DELIMITER\\s(\\$\\$|;)\\s*", RegexOptions.Multiline);
            return regex.Split(query).Where((s) => !string.IsNullOrWhiteSpace(s)).ToArray();
        }

        public string[] SplitOnDelimiter(string query, string delimiter = "\\s*;\\s*")
        {
            var regex = new Regex(delimiter, RegexOptions.Multiline);
            return regex.Split(query).Where((s) => !string.IsNullOrWhiteSpace(s)).ToArray();
        }

        public string[] SplitQueries(string[] splits)
        {
            var length = splits.Length;
            var result = new List<string>();
            for (int i = 0; i < splits.Length; i++)
            {
                if (splits[i] == "$$")
                {
                    if (i + 1 < length)
                    {
                        // [0] $$
                        // [1] closure1;endclosure1$$
                        var closure = splits[i + 1];
                        var instructions = this.SplitOnDelimiter(closure, "\\s*\\$\\$\\s*");
                        this.Concat(result, instructions);
                        // move i
                        i = i + 1;
                    }
                }
                else if (splits[i] == ";")
                {
                    if (i + 1 < length)
                    {
                        // [0] ;
                        // [1] instruction1;instruction2;
                        var closure = splits[i + 1];
                        var instructions = this.SplitOnDelimiter(closure);
                        result.Concat(instructions);
                        this.Concat(result, instructions);
                        i = i + 1;
                    }
                }
                else
                {
                    // [0] instruction1;instruction2;
                    var instructions = this.SplitOnDelimiter(splits[i]);
                    this.Concat(result, instructions);
                }
            }
            return result.ToArray();
        }

        public string[] SplitQuery(string query)
        {
            // search for "DELIMITER" => extract delimiter value ($$ or ;) (; by default)
            var splits = this.SplitOnDELIMITERKeyword(query);
            // split queries with delimiter ($$ or ;)
            return this.SplitQueries(splits);
        }
    }
}

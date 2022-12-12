using System.Text;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;

namespace CommunAxiom.Commons.ClientUI.Shared.Helper
{
    public static class Converter
    {
        public static string? CsvToJson(string input, string delimiter)
        {
            using (TextFieldParser parser = new TextFieldParser(new MemoryStream(Encoding.UTF8.GetBytes(input))))
            {
                parser.Delimiters = new string[] { delimiter };
                string[]? headers = parser.ReadFields();
                if (headers == null) return null;
                string[]? row;
                string comma = "";
                var sb = new StringBuilder((int)(input.Length * 1.1));
                sb.Append("[");
                while ((row = parser.ReadFields()) != null)
                {
                    var dict = new Dictionary<string, object>();
                    for (int i = 0; row != null && i < row.Length; i++)
                        dict[headers[i]] = row[i];

                    var obj = JsonConvert.SerializeObject(dict);
                    sb.Append(comma + obj);
                    comma = ",";
                }
                return sb.Append("]").ToString();
            }
        }

    }
}
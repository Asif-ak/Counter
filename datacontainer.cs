using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Counter
{
    public class ProgressReport
    {
        public int PercentComplete { get; set; }
    }
    class datacontainer
    {
        public string drawname { get; set; }
        OleDbConnection connection;
        private String sheetName;
        DataTable dtt;
        Dictionary<int, string> distinctvalue;
        Random random = new Random();
        int randomid;
        List<KeyValuePair<int, string>> winners = new List<KeyValuePair<int, string>>();
        public BindingList<string> getsheetname(string query)
        {

            connection = new OleDbConnection(query);
            connection.Open();
            DataTable dt = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            List<string> ls = new List<string>();
            for (int a = 0; a < dt.Rows.Count; a++)
            {
                sheetName = dt.Rows[a]["TABLE_NAME"].ToString();
                sheetName = sheetName.Substring(0, sheetName.Length - 1);
                ls.Add(sheetName);
            }
            var listbind = new BindingList<string>(ls);
            //ds.Tables.Add(dt);
            return listbind;

        }
        public bool loadsheet(string query)
        {
            try
            {
                OleDbDataAdapter da = new OleDbDataAdapter(query, connection);
                dtt = new DataTable();
                da.Fill(dtt);
                //return dtt;
                if (dtt.Rows.Count > 0)
                    return true;
                else return false;


            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            { connection.Close(); }
        }


        public Dictionary<int, string> distinctvalues()
        {
            try
            {
                distinctvalue = new Dictionary<int, string>();


                for (int i = 0; i < dtt.Rows.Count; i++)
                {
                    if (distinctvalue.ContainsKey(Convert.ToInt32(dtt.Rows[i][0]))) continue;
                    distinctvalue.Add(Convert.ToInt32(dtt.Rows[i][0]), dtt.Rows[i][1].ToString());
                }
                return distinctvalue;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public int getrandomid()
        {
            try
            {

                randomid = random.Next(distinctvalue.Keys.First(), distinctvalue.Keys.Last() + 1);
                if (distinctvalue.ContainsKey(randomid) && distinctvalue.Keys.Count != 0)
                {
                    // the below four line extract the winners from members loaded in dictionary.
                    KeyValuePair<int, string> winner = distinctvalue.First(x => x.Key == randomid);
                    winners.Add(winner);
                    distinctvalue.Remove(winner.Key);
                    return randomid;
                }
                else return 0;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public KeyValuePair<int, string> individualwinner(int a)
        {
            try
            {
                return winners.Find(key => key.Key == a);

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public BindingList<KeyValuePair<int, string>> updatedistinctvalues() // for viweing data in development/debug mode
        {
            try
            {
                var updatedlist = new BindingList<KeyValuePair<int, string>>(winners);
                return updatedlist;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public bool writetofile(string query)
        {
            
            using (var textwriter = new StreamWriter(query))

            {

                try
                {
                    if (winners.Count > 0)
                    {
                        var writer = new CsvHelper.CsvWriter(textwriter, false);
                        writer.Configuration.Delimiter = ",";

                        writer.WriteComment("This draw name is " + drawname + " ." + "Draw Dated on " + DateTime.Now.ToString());
                        writer.NextRecord();
                        writer.WriteHeader<KeyValuePair<int, string>>();
                        writer.NextRecord();

                        foreach (var item in winners)
                        {


                            writer.WriteField(item.Key);
                            writer.WriteField(item.Value);
                            writer.NextRecord();

                        }

                        return true;
                    }
                    else
                        return false;
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using MySql.Data.MySqlClient;
using OxyPlot;
using Microsoft.Exchange.WebServices.Data;
using System.Net;

namespace DataAnalysisTool
{
    public partial class frmTest : Form
    {
        string cs = @"server=192.168.21.52;userid=webuser;password=Vanchip301;database=testdata";
        //string cs = @"server=45.76.104.155;userid=webuser;password=Vanchip301;database=testdata";

        public frmTest()
        {
            InitializeComponent();
        }

        private void frmTest_Load(object sender, EventArgs e)
        {
            MySqlConnection conn = null;
            try
            {
                conn = new MySqlConnection(cs);
                conn.Open();
                lblinfo.Text = conn.ServerVersion.ToString();
                
                string strcmd = "SELECT DISTINCT Product FROM sessioninfo";
                using (MySqlCommand sqlcmd = new MySqlCommand(strcmd, conn))
                {
                    //MySqlDataAdapter da = new MySqlDataAdapter(strcmd, conn);
                    //DataSet ds = new DataSet();
                    //da.Fill(ds, "Authors");
                    //DataTable dt = ds.Tables["Authors"];
                    //dt.WriteXml("authors.xml");

                    lbxProduct.Items.Clear();
                    MySqlDataReader dr = sqlcmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lbxProduct.Items.Add(dr.GetString(0));
                    }
                }
            }
            catch (MySqlException ex)
            {
                lblinfo.Text = ex.ToString();
            }
            finally
            {
                if (conn != null)  conn.Close();
            }
        }

        private void lbxProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            MySqlConnection conn = null;
            try
            {
                conn = new MySqlConnection(cs);
                conn.Open();
                lblinfo.Text = conn.ServerVersion.ToString();

                string strcmd = "SELECT DISTINCT ProgramRev FROM sessioninfo WHERE Product LIKE @Product";
                using (MySqlCommand sqlcmd = new MySqlCommand(strcmd, conn))
                {
                    lbxRev.Items.Clear();
                    sqlcmd.Parameters.Add(new MySqlParameter("@Product", lbxProduct.SelectedItem.ToString()));
                    MySqlDataReader dr = sqlcmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lbxRev.Items.Add(dr.GetString(0));
                    }
                }
            }
            catch (MySqlException ex)
            {
                lblinfo.Text = ex.ToString();
            }
            finally
            {
                if (conn != null) conn.Close();
            }
        }
        private void lbxRev_SelectedIndexChanged(object sender, EventArgs e)
        {
            MySqlConnection conn = null;
            try
            {
                conn = new MySqlConnection(cs);
                conn.Open();
                lblinfo.Text = conn.ServerVersion.ToString();
                
                string strcmd = "SELECT Parameter FROM testcpk WHERE SessionID IN "
                    +"(SELECT SessionID FROM sessioninfo WHERE Product LIKE @Product AND ProgramRev LIKE @Rev) "
                    +"GROUP BY Parameter;";                
                using (MySqlCommand sqlcmd = new MySqlCommand(strcmd, conn))
                {
                    lbxParameter.Items.Clear();
                    sqlcmd.Parameters.Add(new MySqlParameter("Product", lbxProduct.SelectedItem.ToString()));
                    sqlcmd.Parameters.Add(new MySqlParameter("Rev", lbxRev.SelectedItem.ToString()));
                    MySqlDataReader dr = sqlcmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lbxParameter.Items.Add(dr.GetString(0));
                    }
                }
            }
            catch (MySqlException ex)
            {
                lblinfo.Text = ex.ToString();
            }
            finally
            {
                if (conn != null) conn.Close();
            }
        }
        private void lbxParameter_SelectedIndexChanged(object sender, EventArgs e)
        {
            MySqlConnection conn = null;

            List<string> xValues = new List<string>();
            List<double> yValues = new List<double>();
            double dblAverage;
            double dblStdev;


            try
            {
                conn = new MySqlConnection(cs);
                conn.Open();
                lblinfo.Text = conn.ServerVersion.ToString();

                string strcmd = "SELECT SessionID, Average FROM testcpk WHERE SessionID IN "
                    +"(SELECT SessionID FROM sessioninfo WHERE Product LIKE @Product AND ProgramRev LIKE @Rev AND TestQuantity > @Quantity) "
                    +"AND Parameter LIKE @Parameter";
                     
                using (MySqlCommand sqlcmd = new MySqlCommand(strcmd, conn))
                {
                    gbc.Controls.Clear();
                    listBox1.Items.Clear();

                    sqlcmd.Parameters.Add(new MySqlParameter("@Product", lbxProduct.SelectedItem.ToString()));
                    sqlcmd.Parameters.Add(new MySqlParameter("@Rev", lbxRev.SelectedItem.ToString()));
                    sqlcmd.Parameters.Add(new MySqlParameter("@Quantity", 1000));
                    sqlcmd.Parameters.Add(new MySqlParameter("@Parameter", lbxParameter.SelectedItem.ToString()));

                    MySqlDataReader dr = sqlcmd.ExecuteReader();

                    while (dr.Read())
                    {
                        xValues.Add(dr.GetString(0));
                        yValues.Add(Math.Round(dr.GetDouble(1), 3));
                    }
                    dblAverage = yValues.Average();
                    dblStdev = calc_stdev(yValues);

                    tbxParameter.Text = dblAverage + "," + dblStdev.ToString();

                    //Chart chart = new Chart();

                    Series series_data = new Series("Default");
                    series_data.ChartType = SeriesChartType.FastPoint;
                    chart.Series.Add(series_data);

                    Series series_ll = new Series("LL");
                    series_ll.ChartType = SeriesChartType.FastLine;
                    chart.Series.Add(series_ll);

                    Series series_ul = new Series("UL");
                    series_ul.ChartType = SeriesChartType.FastLine;
                    chart.Series.Add(series_ul);


                    ChartArea dataplot = new ChartArea();
                    Axis yAxis = new Axis(dataplot, AxisName.Y);
                    Axis xAxis = new Axis(dataplot, AxisName.X);

                    dataplot.AxisY.Minimum = Math.Round(dblAverage - 4.5 * dblStdev, 2);
                    dataplot.AxisY.Maximum = Math.Round(dblAverage + 4.5 * dblStdev, 2);

                    dataplot.AxisY.RoundAxisValues();

                    //chart.Series["Default"].Points.DataBindXY(xValues, yValues);
                    for (int i = 0; i < yValues.Count; i++)
                    {
                        chart.Series["Default"].Points.AddY(yValues[i]);
                        chart.Series["LL"].Points.AddY(dblAverage - 1.5 * dblStdev);
                        chart.Series["UL"].Points.AddY(dblAverage + 1.5 * dblStdev);
                    }
                    //while (dr.Read())
                    //{
                    //    //chart.Series["Default"].Points.AddY(dr.GetDouble(1));
                    //}

                    chart.ChartAreas.Add(dataplot);
                    chart.Dock = DockStyle.Fill;
                    gbc.Controls.Add(chart);                    
                }
            }
            catch (MySqlException ex)
            {
                lblinfo.Text = ex.ToString();
            }
            finally
            {
                if (conn != null) conn.Close();
            }

            //var myplot = new OxyPlot.PlotModel
            //{
            //    Title = "My Title",
            //    Subtitle = "My Subtitle",
            //    PlotType = PlotType.XY,
            //    Background = OxyColors.White
            //};

        }

        private double calc_stdev( List<double> values)
        {
            double stdev = 0;
            double vsum = 0;
            List<double> variance = new List<double>();

            for (int i = 0; i < values.Count; i++)
            {
                //variance.Add(Math.Pow((values.Average() - values[i]), 2));
                vsum += Math.Pow((values.Average() - values[i]), 2);
            }
            stdev = Math.Sqrt(vsum / values.Count);
            return stdev;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<string> strTo = new List<string>();
            List<string> strCc = new List<string>();
            string strSubject;
            string strBody;

            strTo.Add("info@acelzg.net");
            strCc.Add("ace_li@vanchiptech.com");
            strCc.Add("f.a.lucifer@gmail.com");
            strSubject = "HelloWorld";
            strBody = "This is the first email I've sent by using the EWS Managed API";

            sendEmail(strTo, strCc, strSubject, strBody);
        }

        public static void sendEmail(List<string> ToRecipients, List<string> CcRecipients, string Subject, string Body)
        {
            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2013_SP1);

            service.Credentials = new WebCredentials("ace_li", "t3103e7.1", "vanchip");

            service.TraceEnabled = true;
            service.TraceFlags = TraceFlags.All;

            //To over-ride this behavior, we need to use the following line in the code, which validate the x509 certificate:
            //https://msdn.microsoft.com/en-us/library/bb408523.aspx?f=255&MSPPError=-2147217396
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

            // Set the URL.
            //service.Url = new Uri(@"https://mail.vanchiptech.com/ews/exchange.asmx");
            service.AutodiscoverUrl("ace_li@vanchiptech.com", RedirectionUrlValidationCallback);

            EmailMessage email = new EmailMessage(service);

            foreach (string strTo in ToRecipients)
            {
                email.ToRecipients.Add(strTo);
            }
            foreach (string strCc in CcRecipients)
            {
                email.CcRecipients.Add(strCc);
            }

            email.Subject = Subject;
            email.Body = new MessageBody(Body);

            email.Send();
        }

        private static bool RedirectionUrlValidationCallback(string redirectionUrl)
        {
            // The default for the validation callback is to reject the URL.
            bool result = false;

            Uri redirectionUri = new Uri(redirectionUrl);

            // Validate the contents of the redirection URL. In this simple validation
            // callback, the redirection URL is considered valid if it is using HTTPS
            // to encrypt the authentication credentials. 
            if (redirectionUri.Scheme == "https")
            {
                result = true;
            }
            return result;
        }
    }
}

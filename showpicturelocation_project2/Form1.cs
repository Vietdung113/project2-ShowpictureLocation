using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace showpicturelocation_project2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ofd.Title = "Input Picture";
            ofd.Filter = "JPG|*.jgp|JPEG|*.jpeg|PNG|*.png";
            DialogResult dr = ofd.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(ofd.FileName);
            }
            string url = location(ofd.FileName);
            webBrowser1.Navigate(url);
            
        }


        public string location(string path_to_file)
        {
            try
            {
                if (String.IsNullOrEmpty(path_to_file))
                {
                    MessageBox.Show("No file passed in");
                    return null;
                }
                GeoPoint gp = null;
                try
                {
                    gp = GeoPoint.GetFromImageFile(path_to_file);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error getting coordinates from image file. ");

                    return null;
                }
                if (gp != null && gp.IsValid)
                {
                    string url = AppSetting("MAP_URL_MASK", @"https://maps.google.com/maps?q=##LAT_COORD##,##LONG_COORD##");
                    string coord_mask = AppSetting("COORD_FORMAT_MASK", Coord.DEFAULT_COORD_MASK);
                    bool encode_coord = true;
                    try
                    {
                        encode_coord = Boolean.Parse(
                            AppSetting("ENCODE_COORD", encode_coord.ToString())
                            );
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error getting url encodong details.  Ensure app config is present and has proper settings.");
                    }
                    Func<string, string> CoordEncoder = new Func<string, string>(s => s);
                   /*
                    if (encode_coord)
                    {
                        CoordEncoder = new Func<string, string>(s => System.Web.HttpUtility.UrlEncode(s));
                    }
                    */
                    latText.Text = CoordEncoder(gp.Latitude.ToGeoCode().ToString());
                    LongText.Text = CoordEncoder(gp.Longitude.ToGeoCode().ToString());                
                    url = url
                        .Replace("##LAT_COORD##", CoordEncoder(gp.Latitude.ToString(coord_mask)))
                        .Replace("##LONG_COORD##", CoordEncoder(gp.Longitude.ToString(coord_mask)))
                        .Replace("##LAT_GEO##", latText.Text)
                        .Replace("##LONG_GEO##", LongText.Text);
                    //string url = AppSetting
                    return url;
                }
                else
                {
                    MessageBox.Show("Unable to resolve coordinates");
                    return null;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Unknown failure, see additional details.");
                return null;
            }
            
        }


        private static string AppSetting(string setting, string error_default)
        {
            try
            {
                string value = System.Configuration.ConfigurationSettings.AppSettings[setting];
                if (!String.IsNullOrEmpty(value))
                {
                    return value;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }

            return error_default;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void LongText_TextChanged(object sender, EventArgs e)
        {
            string url = AppSetting("MAP_URL_MASK", @"https://maps.google.com/maps?q=##LAT_COORD##,##LONG_COORD##");
            string coord_mask = AppSetting("COORD_FORMAT_MASK", Coord.DEFAULT_COORD_MASK);
            bool encode_coord = true;
            Func<string, string> CoordEncoder = new Func<string, string>(s => s);
            GeoPoint gp = null;
            gp = GeoPoint.GetFromImageFile(ofd.FileName);
            url = url
                        .Replace("##LAT_COORD##", CoordEncoder(gp.Latitude.ToString(coord_mask)))
                        .Replace("##LONG_COORD##", CoordEncoder(gp.Longitude.ToString(coord_mask)))
                        .Replace("##LAT_GEO##", latText.Text)
                        .Replace("##LONG_GEO##", LongText.Text);
            webBrowser1.Navigate(url);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url == webBrowser1.Url)
            {
                if (webBrowser1.ReadyState == WebBrowserReadyState.Complete)
                {
                    TakeScreenshot(webBrowser1);
                }
            }
        }
        protected void TakeScreenshot(WebBrowser wb)
        {
            Size pageSize = new Size(wb.Document.Window.Size.Width, wb.Document.Window.Size.Height);
        }
    }
}

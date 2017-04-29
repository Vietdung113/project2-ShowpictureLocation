using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
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
            //System.Diagnostics.Process.Start("http://google.com");
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

        }
        protected void TakeScreenshot(WebBrowser wb)
        {

        }

        private void latText_TextChanged(object sender, EventArgs e)
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

        private void Save_Click(object sender, EventArgs e)
        {
            stringToGPS(latText.Text, pictureBox1.Image.GetPropertyItem(1), pictureBox1.Image.GetPropertyItem(2), true);
            stringToGPS(LongText.Text, pictureBox1.Image.GetPropertyItem(3), pictureBox1.Image.GetPropertyItem(4), false);
        }
        private void stringToGPS(string s, PropertyItem propdir, PropertyItem propCoord, bool islatitude)
        {
            float coord = float.Parse(s);
            // N = 78 S = 83 E = 69 W =87
            if (coord > 0 && islatitude == true)
            {

                propdir.Value[0] = 78;
                propdir.Value[1] = 0;
            }
            if (coord < 0 && islatitude == true)
            {
                propdir.Value[0] = 83;
                propdir.Value[1] = 0;
            }
            if (coord > 0 && islatitude == false)
            {
                propdir.Value[0] = 69;
                propdir.Value[1] = 0;
            }
            if (coord < 0 && islatitude == false)
            {
                propdir.Value[0] = 87;
                propdir.Value[1] = 0;
            }
            if (islatitude)
            {
                propdir.Id = 1;
                pictureBox1.Image.SetPropertyItem(propdir);

                coord = Math.Abs(coord);
                // convert number to bit 
                propCoord.Value = BitConverter.GetBytes(coord);
                propCoord.Id = 2;
                pictureBox1.Image.SetPropertyItem(propCoord);
            }
            else
            {
                propdir.Id = 3;
                pictureBox1.Image.SetPropertyItem(propdir);
                coord = Math.Abs(coord);
                propCoord.Value = BitConverter.GetBytes(coord);
                propCoord.Id = 4;
                pictureBox1.Image.SetPropertyItem(propCoord);
            }
        }
    }
}

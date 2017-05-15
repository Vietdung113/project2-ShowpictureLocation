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

        /// <summary>
        ///      Gui link toi Gooogle Map
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        ///      HAN CHE: bam vao search moi nhay vao toa do mong muon
        ///      NGUYEN NHAN : Thieu app key trong duong dan
        ///      GIAI PHAP DU KIEN: se dang ky app key tren tai khoan gmail , roi xu dung no trong duong link
        /// </remarks>
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
            byte[] longtitude = convertDecimal2Degree(double.Parse(LongText.Text));
            byte[] Latitude = convertDecimal2Degree(double.Parse(latText.Text));
            WriteLongLat(pictureBox1.Image, Latitude[0], Latitude[1], Latitude[2], longtitude[0], longtitude[1], longtitude[2], true, true);

        }
        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                {
                    return encoders[j];
                }
            }
            return null;
        }
        private static void WriteLongLat(Image img, byte latDeg, byte latMin, byte latSec, byte lonDeg, byte lonMin, byte lonSec, bool isWest, bool isNorth)
        {
            const int length = 25;
            System.Drawing.Imaging.Encoder Enc = System.Drawing.Imaging.Encoder.Transformation;
            var EncParms = new EncoderParameters(1);
            ImageCodecInfo CodecInfo = GetEncoderInfo("image/jpeg");

            // TODO: do not load the image to change again 
            PropertyItem[] PropertyItems = img.PropertyItems;
            int oldArrLength = PropertyItems.Length;
            var newProperties = new PropertyItem[oldArrLength];
            img.PropertyItems.CopyTo(newProperties, 0);
            newProperties[0].Id = 0x0002;
            newProperties[0].Type = 5; //5-R 4-L 3-S 
            newProperties[0].Len = length;
            newProperties[0].Value = new byte[length];
            for (int i = 0; i < length; i++)
            {
                newProperties[0].Value[i] = 0;
            }
            //PropertyItems[0].Value = Pic.GetPropertyItem(4).Value; // bDescription; 
            newProperties[0].Value[0] = latDeg;
            newProperties[0].Value[8] = latMin;
            byte secHelper = (byte)(latSec / 2.56);
            byte secRemains = (byte)((latSec - (secHelper * 2.56)) * 100);
            newProperties[0].Value[16] = secRemains; // add to the sum below x_x_*17_+16 
            newProperties[0].Value[17] = secHelper; // multiply by 2.56 
            newProperties[0].Value[20] = 100;
            img.SetPropertyItem(newProperties[0]);
            newProperties[1].Id = 0x0004;
            newProperties[1].Type = 5; //5-R 4-L 3-S 
            newProperties[1].Len = length;
            newProperties[1].Value = new byte[length];
            try
            {
                for (int i = 0; i < length; i++)
                {
                    newProperties[1].Value[i] = 0;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error {0}", e);
            }
            newProperties[1].Value[0] = lonDeg;
            newProperties[1].Value[8] = lonMin;
            secHelper = (byte)(lonSec / 2.56);
            secRemains = (byte)((lonSec - (secHelper * 2.56)) * 100);
            newProperties[1].Value[16] = secRemains;
            // add to the sum bellow x_x_*17_+16 
            newProperties[1].Value[17] = secHelper;
            // multiply by 2.56 
            newProperties[1].Value[20] = 100;
            // multiply by 2.56 

            //PropertyItem current = Pic.GetPropertyItem(2); 
            img.SetPropertyItem(newProperties[1]);
            //GPS Version 
            newProperties[0].Id = 0x0000;
            newProperties[0].Type = 1;
            newProperties[0].Len = 4;
            newProperties[0].Value[0] = 2;
            newProperties[0].Value[1] = 2;
            newProperties[0].Value[2] = 0;
            newProperties[0].Value[3] = 0;
            img.SetPropertyItem(newProperties[0]);

            //GPS Lat REF 
            newProperties[0].Id = 0x0001;
            newProperties[0].Type = 2;
            newProperties[0].Len = 2;
            if (isNorth)
            {
                newProperties[0].Value[0] = 78; //ASCII for N
            }
            else
            {
                newProperties[0].Value[0] = 83; //ASCII for S
            }

            newProperties[0].Value[1] = 0;
            img.SetPropertyItem(newProperties[0]);


            //GPS Lon REF 
            newProperties[0].Id = 0x0003;
            newProperties[0].Type = 2; //5-R 4-L 3-S 
            newProperties[0].Len = 2;
            if (isWest == false)
            {
                newProperties[0].Value[0] = 69; //ASCII for E
            }
            else
            {
                newProperties[0].Value[0] = 87; //ASCII for W
            }
            newProperties[0].Value[1] = 0;
            img.SetPropertyItem(newProperties[0]);
        }
        private byte[] convertDecimal2Degree(double decimal_degrees)
        {

            byte[] result = new byte[3];
            // set decimal_degrees value here

            double minutes = (decimal_degrees - Math.Floor(decimal_degrees)) * 60.0;
            double seconds = (minutes - Math.Floor(minutes)) * 60.0;
            double tenths = (seconds - Math.Floor(seconds)) * 10.0;
            // get rid of fractional part
            result[0] = System.Convert.ToByte(Math.Floor(minutes));
            result[1] = System.Convert.ToByte(Math.Floor(seconds));
            result[2] = System.Convert.ToByte(Math.Floor(tenths));

            return result;
        }
    }
}

using System;
using System.Linq;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Text;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Drawing;


namespace DataAnalysisTool
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //args = new string[2];
            Application.Run(new frmMain(args));

        }

        public static object[] GetRemoteVersion(string remoteVersionURL)
        {
            object[] result = new object[3];

            try
            {
                WebClient webClient = new WebClient();
                // Format:
                //	<version> <url> <hash>
                string remoteVersionText = webClient.DownloadString(remoteVersionURL).Trim();
                string[] remoteVersionParts = (new Regex(@"\s+")).Split(remoteVersionText); // split by space

                Version remoteVersion = new Version(remoteVersionParts[0]);
                string remoteUrl = remoteVersionParts[1];
                string remoteHash = remoteVersionParts[2];


                //if (remoteVersion > localVersion) newVersion = true;


                result[0] = remoteVersion;
                result[1] = remoteUrl;
                result[2] = remoteHash;

            }
            catch
            {
                result[0] = new Version("0.0.0.0");
                result[1] = "";
                result[2] = "";
            }
            return result;
        }

        //static bool PerformUpdate(string remoteUrl, string expectedHash)
        //{
        //    Form f = new Form();
        //    f.BackColor = Color.WhiteSmoke;
        //    f.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        //    f.Width = 600;
        //    f.Height = 500;
        //    f.StartPosition = FormStartPosition.WindowsDefaultLocation;

        //    f.TopMost = true;

        //    Label lblMsg = new Label();
        //    f.Controls.Add(lblMsg);
        //    lblMsg.Dock = DockStyle.Fill;

        //    float currentSize = lblMsg.Font.Size;
        //    currentSize += 2.0F;
        //    lblMsg.Font = new Font(lblMsg.Font.Name, currentSize,
        //        lblMsg.Font.Style, lblMsg.Font.Unit);

        //    Application.EnableVisualStyles();

        //    f.Show();


        //    lblMsg.Text = " - Beginning update."; f.Update();

        //    string downloadDestination = Path.GetTempFileName();

        //    lblMsg.Text += "\r\n - Downloading " + remoteUrl; f.Update();
        //    lblMsg.Text += "\r\n - To " + downloadDestination + " - "; f.Update();
        //    WebClient downloadifier = new WebClient();
        //    downloadifier.DownloadFile(remoteUrl, downloadDestination);
        //    lblMsg.Text += "... done."; f.Update();

        //    lblMsg.Text += "\r\n - Validating download - "; f.Update();
        //    string downloadHash = GetSHA1HashFromFile(downloadDestination);
        //    if (downloadHash.Trim().ToLower() != expectedHash.Trim().ToLower())
        //    {
        //        // The downloaded file looks bad!
        //        // Destroy it quick before it can do any more damage!
        //        File.Delete(downloadDestination);
        //        // Tell the user about what has happened
        //        lblMsg.Text += "... Fail!"; f.Update();
        //        lblMsg.Text += "\r\n - Expected " + expectedHash + ", but actually got " + downloadHash + ")."; f.Update();
        //        lblMsg.Text += "\r\n - The downloaded update may have been modified by an attacker in transit!"; f.Update();
        //        lblMsg.Text += "\r\n - Nothing has been changed, and the downloaded file deleted."; f.Update();
        //        return false;
        //    }
        //    else
        //        lblMsg.Text += "... Ok."; f.Update();

        //    // Since the download doesn't appear to be bad at first sight, let's extract it
        //    lblMsg.Text += "\r\n - Extracting archive - "; f.Update();
        //    string extractTarget = @"./downloadedFiles";

        //    if (Directory.Exists(extractTarget)) Directory.Delete(extractTarget, true);
        //    ZipFile.ExtractToDirectory(downloadDestination, extractTarget);
        //    lblMsg.Text += " ... done."; f.Update();

        //    // Copy the extracted files and replace everything in the current directory to finish the update
        //    // C# doesn't easily let us extract & replace at the same time
        //    // From http://stackoverflow.com/a/3822913/1460422
        //    foreach (string newPath in Directory.GetFiles(extractTarget, "*.*", SearchOption.AllDirectories))
        //    {
        //        lblMsg.Text += "\r\n - Updating : " + newPath.Replace(extractTarget, "."); f.Update();
        //        string name = Path.GetFileNameWithoutExtension(newPath);
        //        Process proc = Process.GetCurrentProcess();

        //        foreach (Process p_temp in Process.GetProcessesByName(name))
        //        {
        //            if (p_temp.Id != proc.Id)
        //            {
        //                p_temp.Kill();
        //            }
        //        }

        //        File.Copy(newPath, newPath.Replace(extractTarget, "."), true);

        //        lblMsg.Text += " ... done."; f.Update();
        //    }

        //    // Clean up the temporary files
        //    lblMsg.Text += "\r\n - Cleaning up - "; f.Update();
        //    //File.Delete("UpdateTemp.exe");
        //    Directory.Delete(extractTarget, true);
        //    lblMsg.Text += "... done."; f.Update();

        //    return true;
        //}

        ///// <summary>
        ///// Gets the SHA1 hash from file.
        ///// Adapted from https://stackoverflow.com/a/16318156/1460422
        ///// </summary>
        ///// <param name="fileName">The filename to hash.</param>
        ///// <returns>The SHA1 hash from file.</returns>
        //static string GetSHA1HashFromFile(string fileName)
        //{
        //    FileStream file = new FileStream(fileName, FileMode.Open);
        //    SHA1 sha1 = new SHA1CryptoServiceProvider();
        //    byte[] byteHash = sha1.ComputeHash(file);
        //    file.Close();

        //    StringBuilder hashString = new StringBuilder();
        //    for (int i = 0; i < byteHash.Length; i++)
        //        hashString.Append(byteHash[i].ToString("x2"));
        //    return hashString.ToString();
        //}

    }
}

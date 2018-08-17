/// Revesion History
///
/// Rev 1.0.0.0         Initial release                             Ace     2016-11-30
/// Rev 1.2.0.0         bug fix                                     Ace     2018-08-17

using System;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Text;
using System.Security.Cryptography;
using System.Security.Permissions;



namespace Update
{
    class Program
    {
        static string remoteVersionURL = "https://raw.githubusercontent.com/AceLZG/data_analysis_tool/master/Release/";
        //static string remoteVersionURL = "https://acelzg.tk/dat/version.txt";
        
        static void Main(string[] args)
        {
            //关闭原有的应用程序 
            System.Diagnostics.Process[] proc = System.Diagnostics.Process.GetProcessesByName("DataAnalysisTool");
            //关闭原有应用程序的所有进程 
            foreach (System.Diagnostics.Process pro in proc)
            {
                pro.Kill();
            }

            // Perform update
            if (args.Length > 0 && args[0] != null && args[0].ToLower() == "update")
            {
                PerformUpdate(args[1], args[2]);
            }
            else
            {
                object[] Result = Program.GetRemoteVersion(remoteVersionURL + "version.txt");
                PerformUpdate(remoteVersionURL + Result[1].ToString(), Result[2].ToString());

            }

            // Start DataAnalysisTool
            Process.Start("DataAnalysisTool.exe");

        }

        public static object[] GetRemoteVersion(string remoteVersionURL)
        {
            object[] result = new object[3];

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

            return result;

        }

        static bool PerformUpdate(string remoteUrl, string expectedHash)
        {

            Console.WriteLine(" - Beginning update.");

            string downloadDestination = Path.GetTempFileName();

            Console.WriteLine(" - Downloading " + remoteUrl);
            Console.WriteLine(" - To " + downloadDestination + " - ");  
            WebClient downloadifier = new WebClient();
            downloadifier.DownloadFile(remoteUrl, downloadDestination);
            Console.WriteLine("... done.");

            Console.WriteLine(" - Validating download - ");
            string downloadHash = GetSHA1HashFromFile(downloadDestination);
            if (downloadHash.Trim().ToLower() != expectedHash.Trim().ToLower())
            {
                // The downloaded file looks bad!
                // Destroy it quick before it can do any more damage!
                File.Delete(downloadDestination);
                // Tell the user about what has happened
                Console.WriteLine("... Fail!");
                Console.WriteLine(" - Expected " + expectedHash + ", but actually got " + downloadHash + ").");
                Console.WriteLine(" - The downloaded update may have been modified by an attacker in transit!");
                Console.WriteLine(" - Nothing has been changed, and the downloaded file deleted.");
                string tmp = Console.ReadLine();
                return false;
            }
            else
                Console.WriteLine("... Ok.");

            // Since the download doesn't appear to be bad at first sight, let's extract it
            Console.WriteLine(" - Extracting archive - ");  
            string extractTarget = @"./downloadedFiles";

            if (Directory.Exists(extractTarget)) Directory.Delete(extractTarget, true);
            ZipFile.ExtractToDirectory(downloadDestination, extractTarget);
            Console.WriteLine(" ... done.");  

            // Copy the extracted files and replace everything in the current directory to finish the update
            // C# doesn't easily let us extract & replace at the same time
            // From http://stackoverflow.com/a/3822913/1460422
            foreach (string newPath in Directory.GetFiles(extractTarget, "*.*", SearchOption.AllDirectories))
            {
                Console.WriteLine(" - Updating : " + newPath.Replace(extractTarget, "."));  
                string name = Path.GetFileNameWithoutExtension(newPath);
                Process proc = Process.GetCurrentProcess();

                foreach (Process p_temp in Process.GetProcessesByName(name))
                {
                    if (p_temp.Id != proc.Id)
                    {
                        Console.WriteLine(" - Killing : " + p_temp.Id + " & " + p_temp.ProcessName);  
                        p_temp.Kill();
                    }
                }

                System.Threading.Thread.Sleep(500);
                File.Copy(newPath, newPath.Replace(extractTarget, "."), true);
              

                Console.WriteLine(" ... done.");  
            }

            // Clean up the temporary files
            Console.WriteLine(" - Cleaning up - ");  
            //File.Delete("UpdateTemp.exe");
            Directory.Delete(extractTarget, true);
            Console.WriteLine("... done.");


            Console.WriteLine(" - Update finished, press any to exit ...");
            string temp = Console.ReadLine();

            return true;
        }

        /// <summary>
        /// Gets the SHA1 hash from file.
        /// Adapted from https://stackoverflow.com/a/16318156/1460422
        /// </summary>
        /// <param name="fileName">The filename to hash.</param>
        /// <returns>The SHA1 hash from file.</returns>
        static string GetSHA1HashFromFile(string fileName)
        {
            FileStream file = new FileStream(fileName, FileMode.Open);
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] byteHash = sha1.ComputeHash(file);
            file.Close();

            StringBuilder hashString = new StringBuilder();
            for (int i = 0; i < byteHash.Length; i++)
                hashString.Append(byteHash[i].ToString("x2"));
            return hashString.ToString();
        }

    }
}
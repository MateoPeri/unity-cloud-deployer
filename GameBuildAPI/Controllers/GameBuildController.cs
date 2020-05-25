using System;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using System.IO.Compression;

namespace GameBuildAPI.Controllers
{
    public class GameBuildController : ApiController
    {
        string basepath = "";

        /* CONFIGURE THESE: */
        // Subdirectory that will contain the builds and logs, relative to your server's root
        string gamesSubdir = "/Games";

        // Subolder that contains the game data inside the unzipped folder. I like to remove this for ease of use
        string subdirName = "Default WebGL";

        // File to write the logs, relative to your basepath (the below config would result in /Games/logs.txt)
        string logPath = "logs.txt";


        // Make a POST to: api/GameBuild
        [HttpPost]
        public string BuildFinished(JObject input)
        {
            // To map from a virtual (URL) path to a physical path
            basepath = HttpContext.Current.Server.MapPath(gamesSubdir);
            Log("STARTED: " + DateTime.Now);
            Log("Webhook received");
            // Log(input); // This clogs the logs. Use only for debug purposes.

            try
            {
                string name = (string)input["projectName"];

                string link = (string)input["links"]["artifacts"][0]["files"][0]["href"];
                Log("Link: " + link);

                string filepath = Path.Combine(basepath, name + ".zip");

                using (WebClient wc = new WebClient())
                {
                    wc.DownloadFile(link, filepath);
                }
                Log("File downloaded");

                Log(name + " (" + (string)input["buildTargetName"] + ", build " + (string)input["buildNumber"] + ")");

                string extractPath = Path.Combine(basepath, name);
                if (Directory.Exists(extractPath))
                {
                    Directory.Delete(extractPath, true);
                }
                Log("Old folder removed");

                ZipFile.ExtractToDirectory(filepath, extractPath);
                File.Delete(filepath);
                Log("File unzipped");

                /*
                 * File structure: Base game folder
                 *                  |- Default WebGL
                 *                      |- index.html
                 *                      |- ... (more files)
                 * We have to move all files from 'Default WebGL to the base game folder.
                 */
                string subdir = Path.Combine(extractPath, subdirName);
                foreach (string filename in Directory.GetFiles(subdir, "*.*"))
                {
                    var f = new FileInfo(filename);
                    var dest = Path.Combine(extractPath, f.Name);
                    Log("Moving " + filename + " to " + dest);
                    File.Move(filename, dest);
                }
                foreach (string dirname in Directory.GetDirectories(subdir, "*.*"))
                {
                    var d = new DirectoryInfo(dirname);
                    var dest = Path.Combine(extractPath, d.Name);
                    Log("Moving " + dirname + " to " + dest);
                    Directory.Move(dirname, dest);
                }
                Log("Deleting " + subdir);
                Directory.Delete(subdir);

                return "Success! More info here: " + Path.Combine(basepath, logPath);
            }
            catch (Exception e)
            {
                return "ERROR: " + e.Message + ", " + e.StackTrace;
            }
        }

        private void Log(params object[] values)
        {
            string filepath = Path.Combine(basepath, logPath);

            using (StreamWriter outputFile = new StreamWriter(filepath, true))
            {
                foreach (object item in values)
                {
                    outputFile.WriteLine(item.ToString());
                }
            }
        }
    }
}

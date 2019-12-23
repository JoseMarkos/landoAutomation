using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace landoAutomatioin
{
    sealed class Automation
    {
        private string landoBoilerplatePath;
        private string landoBoilerplateJoomla;

        private static string currentDirectoryPath = Directory.GetCurrentDirectory();
        public static string siteName { get; private set; }
        public string siteRepoPath { get; private set; }

        private string landoFile = currentDirectoryPath + @"/" + siteName + @"/.lando.yml";

        private StringBuilder stringBuilder = new StringBuilder();
        private StringBuilder copyLandoBoilerplateScript = new StringBuilder();
        

        public Automation()
        {
            landoBoilerplatePath = @"~/github/lando-boilerplates-for-joomla-wordpress-and-prestashop";
            landoBoilerplateJoomla = landoBoilerplatePath + "joomla";
        }

        private void ExecuteBashScript()
        {
            string landoAutomationScript = "copyLandoBoilerplate.sh";

            ProcessStartInfo info = new ProcessStartInfo(landoAutomationScript);
            Process p = Process.Start(info);
            p.WaitForExit();
        }

        private void EditLandoFile()
        {

            Console.WriteLine(landoFile);

            string line;
            string stringToReplace = "$replace-with-a-name$";

            using (StreamReader stream = new StreamReader(landoFile))
            {
                while ((line = stream.ReadLine()) != null)
                {
                    string newLine = line;

                    int count = 0;

                    while (count < 4)
                    {
                        if (!newLine.Contains(stringToReplace))
                        {
                            break;
                        }

                        ReplaceString();

                        count++;
                    }

                    void ReplaceString()
                    {
                        int replaceIndex = newLine.IndexOf(stringToReplace);
                        int restContentIndex = replaceIndex + 21;

                        string prevLine = newLine.Substring(0, replaceIndex);
                        string restLine = newLine.Substring(restContentIndex, newLine.Length - restContentIndex);

                        newLine = prevLine + siteName + restLine;
                    }

                    stringBuilder.AppendLine(newLine);
                }
            }
        }

        private void WriteCopyLandoBoilerplateScript()
        {
            copyLandoBoilerplateScript.AppendLine("#!/bin/bash");
            copyLandoBoilerplateScript.AppendLine("cp -rf ~/github/lando-boilerplates-for-joomla-wordpress-and-prestashop/joomla .");
            copyLandoBoilerplateScript.AppendLine("chwon -Rf dev:dev joomla");
            copyLandoBoilerplateScript.AppendLine("mv joomla " + siteName);
            copyLandoBoilerplateScript.AppendLine("cp -rf " + siteRepoPath + " " + siteName + "/www");
            copyLandoBoilerplateScript.AppendLine("mv " + siteName + "/www/configuration.onl.php " + siteName + "/www/configuration.php");
            copyLandoBoilerplateScript.AppendLine("mv " + siteName + "/www/htaccess.txt " + siteName + "/www/.htaccess");
            copyLandoBoilerplateScript.AppendLine(siteName + "/ lando start");

            using (StreamWriter stream = new StreamWriter(currentDirectoryPath + "\\copyLandoBoilerplate.sh"))
            {
                stream.Write(copyLandoBoilerplateScript);
            }
        }

        private void CopyJoomlaDir()
        {
            if (Directory.Exists(landoBoilerplateJoomla))
            {
                string[] files = Directory.GetFiles(landoBoilerplateJoomla);

                // Copy the files and overwrite destination files if they already exist.
                foreach (string s in files)
                {
                    // Use static Path methods to extract only the file name from the path.
                    string fileName = Path.GetFileName(s);
                    string destDir = currentDirectoryPath + siteName;
                    string destFile = Path.Combine(destDir, fileName);
                    File.Copy(s, destFile, true);
                }
            }
            else
            {
                Console.WriteLine("Source path does not exist!");
            }
        }

        public void AskSiteName(string msg = "What is the site name?")
        {
            Console.WriteLine(msg);
            siteName = Console.ReadLine();
        }

        public void AskRepoSitePath(string msg = "What is the repository path?")
        {
            Console.WriteLine(msg);
            siteRepoPath = Console.ReadLine();
        }

        private void WriteLandoFile()
        {
            using (StreamWriter stream = new StreamWriter(landoFile))
            {
                stream.Write(stringBuilder);
            }
        }


        public void Start()
        {
            AskSiteName();
            AskRepoSitePath();
            WriteCopyLandoBoilerplateScript();
            ExecuteBashScript();
            EditLandoFile();
            WriteLandoFile();
        }
    }
}

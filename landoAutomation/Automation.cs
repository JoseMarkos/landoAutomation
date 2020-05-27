using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace landoAutomation
{
    sealed class Automation
    {
        private string landoBoilerplatePath;
        private string landoBoilerplateJoomla;
        private static string currentDirectoryPath = Directory.GetCurrentDirectory();
        public static string siteName { get; private set; }
        public string siteRepoPath { get; private set; }
        private string landoFile = String.Empty;
        private StringBuilder stringBuilder = new StringBuilder();
        private StringBuilder copyLandoBoilerplateScript = new StringBuilder();

        public Automation()
        {
            landoBoilerplatePath = "/home/josemarcos/Documents/github/lando-boilerplates-for-joomla-wordpress-and-prestashop";
            landoBoilerplateJoomla = landoBoilerplatePath + "/joomla";
        }

        private void ExecuteBashScripts()
        {
            string changePermission = "changePermission.sh";
            ExecuteBashScript(changePermission);
            string landoAutomationScript = "automation.sh";
            ExecuteBashScript(landoAutomationScript);
        }

        private void ExecuteBashScript(string path) {
            ProcessStartInfo info = new ProcessStartInfo(path, "./");
            Process p = Process.Start(info);
            p.WaitForExit();            
        }

        private void EditLandoFile()
        {
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
            copyLandoBoilerplateScript.AppendLine("cp -rf " + landoBoilerplateJoomla + " .");
            copyLandoBoilerplateScript.AppendLine("chown -Rf josemarcos:josemarcos joomla");
            copyLandoBoilerplateScript.AppendLine("mv joomla " + siteName);
            copyLandoBoilerplateScript.AppendLine("cp -rf " + siteRepoPath + "/* " + siteName + "/www");

            if (File.Exists(siteName + "/www/configuration.onl.php"))
            {
                copyLandoBoilerplateScript.AppendLine("mv " + siteName + "/www/configuration.onl.php " + siteName + "/www/configuration.php");
            }
            else {
                System.Console.WriteLine("Configuration file does not exist");
            }

            copyLandoBoilerplateScript.AppendLine("mv " + siteName + "/www/htaccess.txt " + siteName + "/www/.htaccess");

            FileStream _ = File.Create(currentDirectoryPath + "/automation.sh");
	        _.Close();

            using (StreamWriter stream = new StreamWriter(currentDirectoryPath + "/automation.sh"))
            {
                stream.Write(copyLandoBoilerplateScript);
            }
            System.Console.WriteLine(File.GetAttributes("landoAutomation"));
            System.Console.WriteLine(File.GetAttributes("automation.sh"));
            File.SetAttributes("automation.sh", FileAttributes.Normal);
            System.Console.WriteLine(File.GetAttributes("automation.sh"));
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
        private void CopyJoomlaDir()
        {
            string destinationPath = currentDirectoryPath + "/" + "testing";
            Directory.CreateDirectory(destinationPath);
            DirectoryCopy(landoBoilerplateJoomla, destinationPath, true);
        }

        public void AskSiteName(string msg = "What is the site name?")
        {
            Console.WriteLine(msg);
            siteName = Console.ReadLine();
            landoFile = currentDirectoryPath + @"/" + siteName + @"/.lando.yml";
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
            //CopyJoomlaDir();
            WriteCopyLandoBoilerplateScript();
            ExecuteBashScripts();
            EditLandoFile();
            WriteLandoFile();
        }
    }
}

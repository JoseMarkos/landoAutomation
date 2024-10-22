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
        private string landoBasePath = "~/github/lando-boilerplates-for-joomla-wordpress-and-prestashop/";
        private string recipe;
        private static string currentDirectoryPath = Directory.GetCurrentDirectory();
        public static string siteName { get; private set; }
        public string siteRepoPath { get; private set; }
        private string landoFile = String.Empty;
        private StringBuilder stringBuilder = new StringBuilder();
        private StringBuilder landoScript = new StringBuilder();

        public Automation()
        {
            landoBoilerplatePath = @"~/github/lando-boilerplates-for-joomla-wordpress-and-prestashop";
            landoBoilerplateJoomla = landoBoilerplatePath + "joomla";
        }

        private void ExecuteBashScript()
        {
            var landoAutomationScript = "copyLandoBoilerplate.sh";
            var info = new ProcessStartInfo(landoAutomationScript, " ./");
            var p = Process.Start(info);
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
                        var replaceIndex = newLine.IndexOf(stringToReplace);
                        var restContentIndex = replaceIndex + 21;
                        var prevLine = newLine.Substring(0, replaceIndex);
                        var restLine = newLine.Substring(restContentIndex, newLine.Length - restContentIndex);
                        newLine = prevLine + siteName + restLine;
                    }

                    stringBuilder.AppendLine(newLine);
                }
            }
        }

        private void WriteLandoScript()
        {
            landoScript.AppendLine("#!/bin/bash");
            landoScript.AppendLine("cp -rf " + landoBasePath + recipe + " .");
            landoScript.AppendLine("chown -Rf josebailon:josebailon " + recipe);
            landoScript.AppendLine("mv " + recipe + " " + siteName);
            var script = currentDirectoryPath + "/copyLandoBoilerplate.sh";
            FileStream _ = File.Create(script);
	        _.Close();

            using (StreamWriter stream = new StreamWriter(script))
            {
                stream.Write(landoScript);
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

        private void AskSiteName()
        {
            Console.WriteLine("What is the site name?");
            siteName = Console.ReadLine();
            landoFile = currentDirectoryPath + @"/" + siteName + @"/.lando.yml";
        }

        private void AskRepoSitePath()
        {
            Console.WriteLine("What is the repository path?");
            siteRepoPath = Console.ReadLine();
        }
        
        private void AskRecipe()
        {
            Console.WriteLine("What is the recipe name? (joomla, wordpress, lamp, laravel)");
            recipe = Console.ReadLine();
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
            AskRecipe();
            WriteLandoScript();
            ExecuteBashScript();
            EditLandoFile();
            WriteLandoFile();
        }
    }
}

﻿using System.Diagnostics;
using System.IO;

namespace SongTelenkoDFM2
{
    public static class Methods_PythonIntegration
    {
        public static string Run_cmd(string cmd, string args)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = @"C:\Users\amarellapudi6\Desktop\Python\python.exe";
            start.Arguments = string.Format("\"{0}\" \"{1}\"", cmd, args);
            start.UseShellExecute = false;// Do not use OS shell
            start.CreateNoWindow = true; // We don't need new window
            start.RedirectStandardOutput = true;// Any output, generated by application will be redirected back
            start.RedirectStandardError = true; // Any error in standard output will be redirected back (for example exceptions)
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string stderr = process.StandardError.ReadToEnd(); // Here are the exceptions from our Python script
                    string result = reader.ReadToEnd(); // Here is the result of StdOut(for example: print "test")
                    return result;
                }
            }
        }

        // If we want to use a Python script!
        //string script_location = Path.Combine(this.AssemblyPath(), "Test.py");
        //string res = Run_cmd(script_location, string.Empty);
        //Debug.Print(res);
    }
}
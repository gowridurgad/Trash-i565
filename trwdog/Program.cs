﻿using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;

namespace Trash
{
    class Program
    {
        public string SetupFfn = ".trwdog.rc";

        static void Main(string[] args)
        {
            try
            {
                new Program().MainInternal(args);
            }
            catch (Exception e)
            {
                System.Console.Error.WriteLine(e.ToString());
                System.Environment.Exit(1);
            }
        }
        void DisplayHelp<T>(ParserResult<T> result, IEnumerable<Error> errs)
        {
            HelpText helpText = null;
            if (errs.IsVersion())  //check if error is version request
                helpText = HelpText.AutoBuild(result);
            else
            {
                helpText = HelpText.AutoBuild(result, h =>
                {
                    h.AdditionalNewLineAfterOption = false;
                    h.Heading = "trwdog";
                    h.Copyright = "Copyright (c) 2021 Ken Domino"; //change copyright text
                    h.AddPreOptionsText("");
                    return HelpText.DefaultParsingErrorsHandler(result, h);
                }, e => e);
            }
            Console.Error.WriteLine(helpText);
        }

        public void MainInternal(string[] args)
        {
            // Find point in sequence of args and split options from program.
            var divide = 0;
            for (int i = 0; i < args.Length; ++i, divide = i)
            {
                var arg = args[i];
                if (arg[0] == '-' || arg[0] == '/')
                {
                    if (arg.ToLower() == "--timeout" || arg.ToLower() == "-t")
                        ++i;
                    continue;
                }
                else
                {
                    break;
                }
            }
            var command = args.Skip(divide).ToArray();
            var opts = new string[divide];
            Array.Copy(args, opts, divide);


            var config = new Config();

            // Get default from OS, or just default.
            config.Timeout = 300;

            // Get any defaults from ~/.trwdog.rc
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            if (System.IO.File.Exists(home + Path.DirectorySeparatorChar + SetupFfn))
            {
                var jsonString = System.IO.File.ReadAllText(home + Path.DirectorySeparatorChar + SetupFfn);
                var o = JsonSerializer.Deserialize<Config>(jsonString);
                var ty = typeof(Config);
                foreach (var prop in ty.GetProperties())
                {
                    if (prop.GetValue(o, null) != null)
                    {
                        prop.SetValue(config, prop.GetValue(o, null));
                    }
                }
            }

            var result = new CommandLine.Parser().ParseArguments<Config>(opts);
            bool stop = false;
            result.WithNotParsed(
                errs =>
                {
                    DisplayHelp(result, errs);
                    stop = true;
                });
            if (stop) return;
            result.WithParsed(o =>
            {
                var ty = typeof(Config);
                foreach (var prop in ty.GetProperties())
                {
                    if (prop.GetValue(o, null) != null)
                    {
                        prop.SetValue(config, prop.GetValue(o, null));
                    }
                }
            });

            int secs = (int)config.Timeout;
            int delay = secs * 1000;

            var t = new Thread(delegate ()
            {
                int exit_code = 0;
                string cmd = command[0];
                string rest = string.Join(" ", command.Skip(1).Select(a=> '"' + a + '"'));
                try
                {
                    if (cmd.EndsWith(".bat") && !Path.IsPathFullyQualified(cmd))
                    {
                        var p = FindExePath(cmd);
                        if (p != null && p != "") cmd = p;
                    }
                    var processInfo = new ProcessStartInfo(cmd, rest);
                    //System.Console.WriteLine("Cwd " + Directory.GetCurrentDirectory());
                    //System.Console.WriteLine("Starting " + cmd + " " + rest);
                    //processInfo.CreateNoWindow = true;
                    //processInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    //processInfo.UseShellExecute = true;
                    //processInfo.RedirectStandardError = true;
                    //processInfo.RedirectStandardOutput = true;
                    using (Process process = Process.Start(processInfo))
                    {
                        if (process == null)
                        {
                            System.Console.Error.WriteLine("Cannot start process--Process.Start() returned null.");
                            exit_code = 1;
                            System.Environment.Exit(exit_code);
                        }
                        //System.Console.WriteLine("started");
                        process.EnableRaisingEvents = true;
                        //process.BeginOutputReadLine();
                        var has_exited = process.WaitForExit(delay);
                        if (has_exited)
                        {
                            exit_code = process.ExitCode;
                        }
                        else
                        {
                            System.Console.Error.WriteLine("Process is taking longer than " + secs + " seconds. Killing process.");
                            process.Kill(true);
                            exit_code = 1;
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Console.Error.WriteLine(ex.Message);
                    exit_code = 1;
                }
                //System.Console.WriteLine("Finished Post Process");
                System.Environment.Exit(exit_code);
            });
            t.Start();
        }

        public static string FindExePath(string exe)
        {
            exe = Environment.ExpandEnvironmentVariables(exe);
            if (!File.Exists(exe))
            {
                if (Path.GetDirectoryName(exe) == String.Empty)
                {
                    foreach (string test in (Environment.GetEnvironmentVariable("PATH") ?? "").Split(';'))
                    {
                        string path = test.Trim();
                        if (!String.IsNullOrEmpty(path) && File.Exists(path = Path.Combine(path, exe)))
                            return Path.GetFullPath(path);
                    }
                }
                throw new FileNotFoundException(new FileNotFoundException().Message, exe);
            }
            return Path.GetFullPath(exe);
        }
    }
}
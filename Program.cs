using System;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace hacktimer
{
    class Program
    {
        public static DateTime startTime;

        public static string time;

        static void Main(string[] args)
        {
            Menu();

            DrawMain();

            PythonScript("anykey_detection.py");

            startTime = DateTime.Now;

            Thread timerThread = new Thread(() => Timer());
            timerThread.Start();

            while (true) {
                PythonScript("enter_detection.py");

                on++;
                DrawMain();

                if (on == categories[selected].Value.Length) { break; }
            }

            DrawMain();

            Console.WriteLine("\n" + time);

            Environment.Exit(0);
        }


        private static void PythonScript(string script)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "python3";
            start.Arguments = script;
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;

            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                }
            }
        }

        public static void Timer() {
            while (true) {
                TimeSpan change = DateTime.Now - startTime;

                string output = change.ToString(@"hh\:mm\:ss") + ":" + change.Milliseconds;
                time = output;

                Console.SetCursorPosition(0, Console.WindowHeight - 3);
                Console.Write(output);

                Thread.Sleep(10);
            }
        }

        public static void DrawMain() {
            Console.BackgroundColor = ConsoleColor.Black;

            Console.Clear();
            Console.WriteLine("hackmud " + categories[selected].Key);
            Console.WriteLine(new string('-', Console.BufferWidth));

            Console.WriteLine();

            int index = 0;
            foreach (string s in categories[selected].Value) {
                if (index == on) { Console.BackgroundColor = ConsoleColor.DarkGray; }
                else { Console.BackgroundColor = ConsoleColor.Black; }
                
                Console.WriteLine(s);

                index++;
            }

            Console.BackgroundColor = ConsoleColor.Black;

            Console.WriteLine();

            Console.WriteLine(new string('-', Console.BufferWidth));
        }

        public static int selected = 0;

        public static int on = 0;

        public static List<KeyValuePair<string, string[]>> categories = new List<KeyValuePair<string, string[]>>() {
            new KeyValuePair<string, string[]>("T1 Corp Scriptless%", new string[] { "Get corp", "Get places", "Get usage", "Get password / strategy", "Get blog", "Try p key", "Try pass key", "Try password key" }),
            new KeyValuePair<string, string[]>("T2 Corp Scriptless%", new string[] { "Get T2 corp", "Get T1 corp places", "Get T1 corp usage", "Get blog", "Try username" })
        };

        static void DrawMenu() {
            Console.BackgroundColor = ConsoleColor.Black;

            Console.Clear();
            Console.WriteLine("hacktimer - hackmud speedrun timer - by oliver1803");
            Console.WriteLine(new string('-', Console.BufferWidth) + "\n");

            int index = 0;
            foreach (KeyValuePair<string, string[]> category in categories) {
                if (index == selected) { Console.BackgroundColor = ConsoleColor.DarkGray; }
                else { Console.BackgroundColor = ConsoleColor.Black; }

                Console.WriteLine(category.Key);

                index++;
            }

            Console.BackgroundColor = ConsoleColor.Black;

            Console.WriteLine("\n" + new string('-', Console.BufferWidth));
        }

        static void Menu() {
            DrawMenu();

            while (true) {
                ConsoleKey key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.UpArrow) {
                    if (selected > 0) selected--;
                    DrawMenu();
                }

                if (key == ConsoleKey.DownArrow) {
                    if (selected < categories.Count) selected++;
                    DrawMenu();
                }

                if (key == ConsoleKey.Enter) {
                    break;
                }
            }
        }
    }
}

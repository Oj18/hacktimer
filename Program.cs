﻿using System;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

namespace hacktimer
{
    class Program
    {
        public static DateTime startTime;

        public static string time;

        public static string bestTime = null;

        static void Main(string[] args)
        {
            if (File.Exists("best_time.txt")) {
                bestTime = File.ReadAllText("best_time.txt");
            }

            File.WriteAllText("categories.json", JsonConvert.SerializeObject(categories));

            categories = JsonConvert.DeserializeObject<List<KeyValuePair<string, string[]>>>(File.ReadAllText("categories.json"));

            Menu();

            DrawMain();
            PythonScript("anykey_detection.py");


            startTime = DateTime.Now;

            Thread timerThread = new Thread(() => Timer());
            timerThread.Start();

            while (true) {
                string output = PythonScript("enter_detection.py");

                if (output == "stop") {
                    break;
                } else {
                    on++;
                    DrawMain();

                    if (on == categories[selected].Value.Length) { on--; }
                }
            }

            DrawMain();

            Console.WriteLine("\n" + time);

            Environment.Exit(0);
        }

        public static string version = "hacktimer v0.7 - hackmud speedrun timer - by oliver1803";

        private static string PythonScript(string script)
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

                    return result;
                }
            }
        }

        public static string oldTime = null;

        public static List<KeyValuePair<int, int>> showChange = new List<KeyValuePair<int, int>>();

        public static bool highlightChange(int index) {
            if (showChange.Count == 0) return false;

            int i = 0;
            foreach (KeyValuePair<int, int> pair in showChange.ToArray()) {
                if (pair.Key == index) {
                    if (pair.Value > 0) {
                        showChange[i] = new KeyValuePair<int, int>(index, pair.Value - 1);
                        return true;
                    }
                    else { return false; }
                }

                i++;
            }

            return false;
        }

        // Returns true if added new element
        // Returns false if added time to existing element
        public static bool addChange(int index) {
            int change = (int)Math.Pow(13 - index, 2.2);
            if (change < 0) change = 0;

            int i = 0;
            foreach (KeyValuePair<int, int> pair in showChange.ToArray()) {
                if (pair.Key == index) {
                    showChange[i] = new KeyValuePair<int, int>(index, pair.Value + change);
                    
                    return false;
                }

                i++;
            }

            showChange.Add(new KeyValuePair<int, int>(index, change));

            return true;
        }

        public static void Timer() {
            while (true) {
                TimeSpan change = DateTime.Now - startTime;

                string milliseconds = change.Milliseconds.ToString();

                milliseconds = new string('0', 3 - milliseconds.Length) + milliseconds;

                time = change.ToString(@"hh\:mm\:ss") + ":" + milliseconds;

                if (time.Length != 12) Console.WriteLine("AH");

                if (oldTime == null || oldTime.Length != time.Length) oldTime = time;

                Console.BackgroundColor = ConsoleColor.Black;
                Console.SetCursorPosition(0, Console.WindowHeight - 3);

                for (int i = 0; i < time.Length; i++) {
                    if (time[i] != oldTime[i]) { addChange(i); }

                    if (highlightChange(i)) { Console.BackgroundColor = ConsoleColor.DarkGray; }
                    else { Console.BackgroundColor = ConsoleColor.Black; }

                    Console.Write(time[i]);
                }

                Console.BackgroundColor = ConsoleColor.Black;

                Console.WriteLine("\n" + oldTime);

                Console.BackgroundColor = ConsoleColor.Black;

                oldTime = time;

                Thread.Sleep(10);
            }
        }

        public static void DrawMain() {
            Console.BackgroundColor = ConsoleColor.Black;

            Console.Clear();
            Console.WriteLine("hackmud " + categories[selected].Key + "\n" + version);
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
            Console.WriteLine("categories\nhacktimer - hackmud speedrun timer - by oliver1803");
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
                    if (selected < categories.Count - 1) selected++;
                    DrawMenu();
                }

                if (key == ConsoleKey.Enter) {
                    break;
                }
            }
        }
    }
}

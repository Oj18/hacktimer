using System;
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

        public static int runNumber;
        
        static void Main(string[] args)
        {
            categories = JsonConvert.DeserializeObject<List<KeyValuePair<string, string[]>>>(File.ReadAllText("categories.json"));

            while (true) {
                Menu();

                if (selected == categories.Count) {
                    break;
                }

                // get run number
                if (File.Exists(categories[selected].Key + "-runnumber.txt"))
                    runNumber = int.Parse(File.ReadAllText(categories[selected].Key + "-runnumber.txt"));
                else
                    runNumber = 0;

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

                cancelTimer = true;

                while (cancelTimer) { Thread.Sleep(1); }
                
                runNumber++;

                File.WriteAllText(categories[selected].Key + "-runnumber.txt", runNumber.ToString());

                DrawMain();

                on = 0;

                Console.SetCursorPosition(0, Console.WindowHeight - 3);
                Console.WriteLine(time);

                Console.ReadLine();
            }
        }

        public static bool cancelTimer = false;

        public static string version = "hacktimer v1.1.2 - hackmud speedrun timer - by oj / oliver1803";

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

                oldTime = time;

                if (cancelTimer) {
                    break;
                }

                Thread.Sleep(10);
            }

            cancelTimer = false;
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

            Console.WriteLine(new string('-', Console.BufferWidth) + "\n");

            Console.WriteLine("run: " + runNumber.ToString() + "\n");

            Console.WriteLine(new string('-', Console.BufferWidth));
        }

        public static int selected = 0;

        public static int on = 0;

        public static List<KeyValuePair<string, string[]>> categories;

        static void DrawMenu() {
            Console.BackgroundColor = ConsoleColor.Black;

            Console.Clear();
            Console.WriteLine("categories\n" + version);
            Console.WriteLine(new string('-', Console.BufferWidth) + "\n");

            //duplicate the array of categories
            List<KeyValuePair<string, string[]>> addedCategories = new List<KeyValuePair<string, string[]>>(categories);
            addedCategories.Add(new KeyValuePair<string, string[]>("Exit", new string[] { "exit" }));

            int index = 0;
            foreach (KeyValuePair<string, string[]> category in addedCategories) {
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

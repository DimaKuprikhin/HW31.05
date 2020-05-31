using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HW31._05Kuprikhin199
{
    class Program
    {
        /// <summary>
        /// Массив перевода заглавных букв.
        /// </summary>
        private static string[] upperRule = new string[] { 
            "А", // A
            "Б", // B
            "Ц", // C
            "Д", // D
            "Е", // E
            "Ф", // F
            "Г", // G
            "Х", // H
            "И", // I
            "Ж", // J
            "К", // K
            "Л", // L
            "М", // M
            "Н", // N
            "О", // O
            "П", // P
            "КУ", // Q
            "Р", // R
            "С", // S
            "Т", // T
            "У", // U
            "В", // V
            "У", // W
            "КС", // X
            "Ы", // Y
            "З"  // Z
        };

        /// <summary>
        /// Массив перевода строчных букв.
        /// </summary>
        private static string[] lowerRule = new string[] {
            "а", // A
            "б", // B
            "ц", // C
            "д", // D
            "е", // E
            "ф", // F
            "г", // G
            "х", // H
            "и", // I
            "ж", // J
            "к", // K
            "л", // L
            "м", // M
            "н", // N
            "о", // O
            "п", // P
            "ку", // Q
            "р", // R
            "с", // S
            "т", // T
            "у", // U
            "в", // V
            "у", // W
            "кс", // X
            "ы", // Y
            "з"  // Z
        };



        /// <summary>
        /// Читает текст из файла.
        /// </summary>
        /// <param name="path"> Путь файла. </param>
        /// <param name="text"> Строка с текстом. </param>
        /// <returns> True - если чтение прошло успешно. </returns>
        private static bool ReadText(string path, ref string text) {
            try
            {
                StreamReader sr = new StreamReader(path);
                text = sr.ReadToEnd();
                sr.Close();
                return true;
            }
            catch
            {
                Console.WriteLine($"Произошла ошибка при чтении из файла {path}");
                return false;
            }
        }

        /// <summary>
        /// Создает и записывает в файл текст.
        /// </summary>
        /// <param name="path"> Путь файла. </param>
        /// <param name="text"> Текст. </param>
        /// <returns> True - если запись прошла успешно. </returns>
        private static bool WriteText(string path, string text)
        {
            try
            {
                File.WriteAllText(path, "");
                StreamWriter sw = new StreamWriter(path);
                sw.Write(text);
                sw.Close();
                return true;
            }
            catch
            {
                Console.WriteLine($"Произошла ошибка при записи в файл {path}");
                return false;
            }
        }

        /// <summary>
        /// Создает преобразованную строку.
        /// </summary>
        /// <param name="text"> Строка для преобразования. </param>
        /// <param name="begin"> Начальный символ. </param>
        /// <param name="end"> Индекс конечного символа, на включенный. </param>
        /// <returns> Строка, являющаяся преобразованным отрезком оригинальной строки.
        /// </returns>
        private static string ConvertText(string text, int begin, int end)
        {
            StringBuilder newText = new StringBuilder(text.Length);
            for(int i = begin; i < end; ++i)
            {
                if(('a' <= text[i]) && (text[i] <= 'z')) 
                {
                    newText.Append(lowerRule[(char)(text[i] - 'a')]);
                }
                else if(('A' <= text[i]) && (text[i] <= 'Z'))
                {
                    newText.Append(upperRule[(char)(text[i] - 'A')]);
                }
                else
                {
                    newText.Append(text[i]);
                }
            }
            return newText.ToString();
        }

        /// <summary>
        /// Преобазует файл из папки Books.
        /// </summary>
        /// <param name="bookName"> Название файла. </param>
        private static void ConvertBook(string bookName)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            string text = "";
            ReadText($"Books/{bookName}", ref text);
            int textSize = text.Length;

            string newText = ConvertText(text, 0, text.Length);
            int newTextSize = newText.Length;

            WriteText($"new_Books/new_{bookName}", newText);
            Console.WriteLine($"{bookName}: символов до = {textSize}, " +
                $"после = {newTextSize}, время = {watch.ElapsedMilliseconds}мс");
        }

        /// <summary>
        /// Асинхронный метод, преобразующий какую-то часть строки.
        /// </summary>
        /// <param name="text"> Обрабатываемая строка. </param>
        /// <param name="begin"> Начальный индекс. </param>
        /// <param name="end"> Конечный индекс невключительно. </param>
        /// <returns> Возвращает часть преобразованной строки. </returns>
        private static async Task<string> AsyncConvertText(string text, int begin, int end) {
            var result = await Task.Run(() => ConvertText(text, begin, end));
            return result;
        }

        /// <summary>
        /// Асинхронно преобразует строку, полученную get-запросом, 
        /// используя несколько задач.
        /// </summary>
        /// <param name="text"> Обрабатываемая строка. </param>
        /// <param name="taskCount"> Количество задач. </param>
        private static void Part2(string text, int taskCount)
        {

            Stopwatch watch = new Stopwatch();
            watch.Start();

            object[] results = new object[taskCount];
            for(int i = 0; i < taskCount; ++i)
            {
                results[i] = AsyncConvertText(text, text.Length * i / taskCount,
                    text.Length * (i + 1) / taskCount);
            }
            string[] partsOfNewText = new string[taskCount];
            for(int i = 0; i < taskCount; ++i)
            {
                partsOfNewText[i] = ((Task<string>)results[i]).Result;
            }
            string newText = string.Join("", partsOfNewText);

            WriteText("new_book_from_web.txt", newText);

            Console.WriteLine($"new_book_from_web.txt ({taskCount} Task'ов): символов до = {text.Length}" +
                $", после = {newText.Length}, время = {watch.ElapsedMilliseconds}мс");
        }

        static void Main(string[] args)
        {
            string[] fileNames = new string[] { "121-0.txt", "1727-0.txt",
                "4200-0.txt", "58975-0.txt", "pg972.txt", "pg3207.txt",
                "pg19942.txt", "pg27827.txt", "pg43936.txt" };

            // Последовательно преобразуем все книги.
            Stopwatch watch = new Stopwatch();
            watch.Start();
            for(int i = 0; i < fileNames.Length; ++i)
            {
                ConvertBook(fileNames[i]);
            }
            watch.Stop();
            Console.WriteLine($"Общее время = {watch.ElapsedMilliseconds}мс" + 
                Environment.NewLine);

            // Преобразуем книги параллельно.
            watch.Restart();
            Parallel.For(0, fileNames.Length, i =>
            {
                ConvertBook(fileNames[i]);
            }
            );
            watch.Stop();
            Console.WriteLine($"Общее время = {watch.ElapsedMilliseconds}мс" +
                Environment.NewLine);

            // Получаем текст.
            HttpClient client = new HttpClient();
            string text = client.GetStringAsync("https://www.gutenberg.org/files/1342/1342-0.txt").Result;
            // Преобразуем полученный текст с несколькими значениями количества задач.
            Part2(text, 1);
            Part2(text, 2);
            Part2(text, 4);
            Part2(text, 8);
            Part2(text, 16);

            watch.Reset();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumApp.App.Utilities
{
    public static class InputUtility
    {
        public static string ReadString(string prompt)
        {
            string input;
            do
            {
                Console.Write(prompt);
                input = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("--> Входното поле не може да бъде празно. Моля, опитайте отново.");
                }
            } while (string.IsNullOrWhiteSpace(input));

            return input;
        }

        public static int ReadInt(string prompt)
        {
            int result;
            string input;

            while (true)
            {
                Console.Write(prompt);
                input = Console.ReadLine();

                if (int.TryParse(input, out result))
                {
                    return result;
                }

                Console.WriteLine("--> Невалиден вход. Моля, въведете цяло число.");
            }
        }
    }
}

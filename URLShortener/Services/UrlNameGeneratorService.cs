using URLShortener.Interfaces;

namespace URLShortener.Services
{
    public class UrlNameGeneratorService : IUrlNameGeneratorService
    {
        private string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        public string Generate()
        {
            var stringChars = new char[7];
            for (var i = 0; i < stringChars.Length; i++)
            {
                char randomChar;
                do
                {
                    randomChar = GetRandomChar();
                } while (GetCountOfCharInArray(stringChars, randomChar) >= 2);

                stringChars[i] = randomChar;
            }
            return new string(stringChars);
        }

        private int GetCountOfCharInArray(char[] array, char c)
        {
            return Array.FindAll(array, x => x == c).Length;
        }

        private char GetRandomChar()
        {
            var rng = new Random();
            return Chars[rng.Next(Chars.Length)];
        }
    }
}

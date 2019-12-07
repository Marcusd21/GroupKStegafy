using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GroupKStegafy.Utility
{
    public static class CipherTextManager
    {

        private const string KeyWordIdentifier = "#KEY#";
        private const string TextEndingIdentifier = "#.-.-.-#";
        private const string EnglishAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static string VigenereEncrypt(string source, string keyword)
        {
            source = source.ToUpper();
            keyword = keyword.ToUpper();

            var j = 0;

            var encryptedText = new StringBuilder(source.Length);

            encryptedText.Append(keyword);
            encryptedText.Append(KeyWordIdentifier);

            keyword = ExpandString(keyword, source.Length);

            foreach (var character in source)
            {
                encryptedText.Append(EnglishAlphabet.Contains(character)
                    ? EnglishAlphabet[(EnglishAlphabet.IndexOf(character) + EnglishAlphabet.IndexOf(keyword[j])) % EnglishAlphabet.Length]
                    : character);

                j = (j + 1) % keyword.Length;
            }

            encryptedText.Append(TextEndingIdentifier);

            return encryptedText.ToString();
        }

        public static string VigenereDecrypt(string source)
        {
            source = source.ToUpper();
            var keyword = source.Substring(0, source.IndexOf(KeyWordIdentifier, StringComparison.Ordinal));

            var j = 0;

            var start = source.IndexOf(KeyWordIdentifier, StringComparison.Ordinal) + KeyWordIdentifier.Length;
            var end = source.IndexOf(TextEndingIdentifier, StringComparison.Ordinal) - start;

            var decryptedText = new StringBuilder(source.Length);
            var encryptedText = source.Substring(start, end);

            keyword = ExpandString(keyword, source.Length);

            foreach (var character in encryptedText)
            {
                decryptedText.Append(EnglishAlphabet.Contains(character)
                    ? EnglishAlphabet[
                        (EnglishAlphabet.IndexOf(character) - EnglishAlphabet.IndexOf(keyword[j]) + EnglishAlphabet.Length) %
                        EnglishAlphabet.Length]
                    : character);

                j = (j + 1) % keyword.Length;
            }

            return decryptedText.ToString();
        }

        private static string ExpandString(string str, int length)
        {
            if (length <= str.Length) return str.Substring(0, length);

            while (str.Length * 2 <= length)
            {
                str += str;
            }

            if (str.Length < length)
            {
                str += str.Substring(0, length - str.Length);
            }

            return str;
        }
    }
}

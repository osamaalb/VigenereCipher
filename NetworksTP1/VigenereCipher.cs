using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NetworksTP1
{
    [Serializable]
    public class WrongEncryptedTextFormatException : Exception
    {
        public WrongEncryptedTextFormatException() { }

        public WrongEncryptedTextFormatException(string message)
            : base(message) { }

        public WrongEncryptedTextFormatException(string message, Exception inner)
            : base(message, inner) { }
    }
    public static class VigenereCipher
    {
        public static string? encryptText(string originalText, string key)
        {
            string encryptedText = "";

            originalText = preProcessString(originalText);
            key = preProcessString(key);

            for (int i = 0; i < originalText.Length; i++)
            {
                encryptedText += encryptOneLetter(originalText[i], key[i % key.Length]);
            }

            return encryptedText;
        }

        public static string? decryptText(string encryptedText, string key)
        {
            string originalText = "";

            if (encryptedText != preProcessString(encryptedText))
            {
                var resourceManager = new ResourceManager("NetworksTP1.Resources.ExceptionMessages", Assembly.GetExecutingAssembly());
                throw new WrongEncryptedTextFormatException(resourceManager.GetString("WrongEncryptedTextFormatException"));
            }

            key = preProcessString(key);

            for (int i = 0; i < encryptedText.Length; i++)
            {
                originalText += decryptOneLetter(encryptedText[i], key[i % key.Length]);
            }

            return originalText;
        }

        public static Dictionary<string, RepeatedSequenceStatistics> getRepeatedSequences(string encryptedText)
        {
            Dictionary<string, RepeatedSequenceStatistics> repeatedSequences = new Dictionary<string, RepeatedSequenceStatistics>();

            for (int i = 3; i <= encryptedText.Length / 2; i++)
            {
                for (int j = 0; j <= encryptedText.Length - i; j++)
                {
                    if (repeatedSequences.ContainsKey(encryptedText.Substring(j, i)))
                    {
                        repeatedSequences[encryptedText.Substring(j, i)].occurences++;
                        if (repeatedSequences[encryptedText.Substring(j, i)].secondPosition == -1)
                        {
                            repeatedSequences[encryptedText.Substring(j, i)].secondPosition = j;
                        }
                    } else
                    {
                        repeatedSequences.Add(encryptedText.Substring(j, i), new RepeatedSequenceStatistics(1, j, -1));
                    }
                }
            }

            return repeatedSequences;
        }

        public static Dictionary<int, int> calculateCommonDevisors(string encryptedText)
        {
            Dictionary<int, int> commonDevisors = new Dictionary<int, int>();
            Dictionary<string, RepeatedSequenceStatistics> repeatedSequences = getRepeatedSequences(encryptedText);
            // We are removing the less-probable repetitions by removing any sequences the repeat for less than 30% of the maximum repetition of all sequences
            int minOccurences = (int)Math.Round(Math.Max(1, (double)repeatedSequences.Max(p => p.Value.occurences) * 0.3));
            foreach (var sequence in repeatedSequences.Where(p => p.Value.occurences > minOccurences))
            {
                int distance = sequence.Value.secondPosition - sequence.Value.firstPosition;
                for (int i = 2; i <= distance; i++)
                {
                    if (distance % i == 0)
                    {
                        if (commonDevisors.ContainsKey(i))
                        {
                            commonDevisors[i]++;
                        }
                        else
                        {
                            commonDevisors.Add(i, 1);
                        }
                    }
                }
            }

            return commonDevisors;
        }

        public static double calculateIndexOfCoincidence(string text)
        {
            double indexOfCoincidence = 0;

            double n = text.Length;
            double ni;
            
            for(char i = 'a'; i <= 'z'; i++)
            {
                ni = text.Count(c => c == i);
                indexOfCoincidence += (ni / n) * ((ni - 1) / (n - 1));
            }

            return Math.Round(indexOfCoincidence, 4);
        }
        public static double calculateL(string text)
        {
            double K = calculateIndexOfCoincidence(text);
            double Ke = 0.067;
            double Kr = 0.038;
            double L = Math.Round((Ke - Kr) / (K - Kr), 4);
            return L;
        }

        private static string preProcessString(string text)
        {
            string processedText;

            processedText = text.ToLower();
            processedText = Regex.Replace(processedText, "[^a-z]", "");

            return processedText;
        }

        private static char encryptOneLetter(char originalCharacter, char key)
        {
            return (char)((((originalCharacter - 'a') + (key - 'a')) % 26) + 'a');
        }

        private static char decryptOneLetter(char encryptedlCharacter, char key)
        {
            return (char)((((encryptedlCharacter - 'a') - (key - 'a') + 26) % 26) + 'a');
        }
    }
}

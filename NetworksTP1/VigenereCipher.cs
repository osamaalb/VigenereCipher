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

        public static string? decryptText(string encryptedText, string key, int offset = 0)
        {
            string originalText = "";

            if (encryptedText != preProcessString(encryptedText))
            {
                var resourceManager = new ResourceManager("NetworksTP1.Resources.ExceptionMessages", Assembly.GetExecutingAssembly());
                throw new WrongEncryptedTextFormatException(resourceManager.GetString("WrongEncryptedTextFormatException"));
            }

            key = preProcessString(key);

            offset = offset % key.Length;

            for (int i = 0; i < encryptedText.Length; i++)
            {
                originalText += decryptOneLetter(encryptedText[i], key[(i + key.Length - offset) % key.Length]);
            }

            return originalText;
        }

        public static Dictionary<string, RepeatedSequenceStatistics> getRepeatedSequences(string encryptedText, int maxKeyLength)
        {
            Dictionary<string, RepeatedSequenceStatistics> repeatedSequences = new Dictionary<string, RepeatedSequenceStatistics>();

            for (int i = 3; i <= maxKeyLength; i++)
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

        public static Dictionary<int, int> calculateCommonDevisors(string encryptedText, int maxKeyLength)
        {
            Dictionary<int, int> commonDevisors = new Dictionary<int, int>();
            Dictionary<string, RepeatedSequenceStatistics> repeatedSequences = getRepeatedSequences(encryptedText, maxKeyLength);
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

        public static void breakEncryption(string encryptedText, int keyLength)
        {
            if (encryptedText != preProcessString(encryptedText))
            {
                var resourceManager = new ResourceManager("NetworksTP1.Resources.ExceptionMessages", Assembly.GetExecutingAssembly());
                throw new WrongEncryptedTextFormatException(resourceManager.GetString("WrongEncryptedTextFormatException"));
            }

            string key = "";

            for (int i = 0; i < keyLength; i++)
            {
                Dictionary<char, int> letterRepetiotions = new Dictionary<char, int>();
                for (char c = 'a'; c <= 'z'; c++)
                {
                    letterRepetiotions.Add(c, 0);
                }

                for (int j = 0; j < encryptedText.Length; j++)
                {
                    if (j % keyLength == i)
                    {
                        letterRepetiotions[encryptedText[j]]++;
                    }
                }

                Console.WriteLine(String.Format("Number of repetiotions for the {0}th character in the key:", i));

                foreach (var repetition in letterRepetiotions.OrderByDescending(p => p.Value))
                {
                    Console.Write(repetition.Key + ": " + repetition.Value + ", ");
                }
                Console.WriteLine();

                while (true)
                {
                    Console.WriteLine("Choose which letter is e:");
                    char e = Console.ReadLine()[0];

                    if (!(e >= 'a' && e <= 'z'))
                    {
                        Console.WriteLine("Wrong input! Please try again!");
                    } else
                    {
                        key += (char)(((e - 'a') + 26 - ('e' - 'a')) % 26 + 'a');
                        break;
                    }
                }
            }

            Console.WriteLine("The key is: " + key);

            string decryptedText = decryptText(encryptedText, key);

            Console.WriteLine("Decrypted text: " + decryptedText);
        }

        public static void breakEncryptionByProbableWord(string encryptedText, string probableWord)
        {
            Dictionary<string, int> repeatedSequences = new Dictionary<string, int>();
            Console.WriteLine("Decryption for all possitions:");
            for(int i = 0; i < probableWord.Length; i++)
            {
                string decryptedText = decryptText(encryptedText, probableWord, i);
                Console.WriteLine(i + ": " + decryptedText);

                for (int j = 0; j < decryptedText.Length - probableWord.Length; j++)
                {
                    if (repeatedSequences.ContainsKey(decryptedText.Substring(j, probableWord.Length)))
                    {
                        repeatedSequences[decryptedText.Substring(j, probableWord.Length)]++;
                    }
                    else
                    {
                        repeatedSequences.Add(decryptedText.Substring(j, probableWord.Length), 1);
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine("Possible key sequences");

            foreach (var sequence in repeatedSequences.Where(p => p.Value > 1).OrderByDescending(p => p.Value).Take(10))
            {
                Console.WriteLine(sequence.Key + ": " + sequence.Value);
            }
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

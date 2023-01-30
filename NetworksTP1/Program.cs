using NetworksTP1;

string text = "", key = "", controlText = "", probableWord = "";
int keyLength = 0, maxKeyLength = 0;
char[] allowedInputCommands = {'E', 'D', 'K', 'I', 'L', 'B', 'P'};

while (true)
{
    Console.WriteLine("What do you want to do?\n" +
        "E: encrypt text\n" +
        "D: decrypt text\n" +
        "K: calculate possible key length\n" +
        "I: calculate the index of coincidence\n" +
        "L: calculate L\n" +
        "B: break an encryption with a known key length\n" +
        "P: break an encryption by a probable word"

    );
    controlText = Console.ReadLine();

    if (!allowedInputCommands.Contains(controlText[0]) || controlText.Length != 1) {
        Console.WriteLine("Input error!");
        continue;
    }

    Console.WriteLine("Please enter a text:");
    text = Console.ReadLine();

    if (controlText == "E" || controlText == "D")
    {
        Console.WriteLine("Please enter a key:");
        key = Console.ReadLine();
    }

    if (controlText == "E")
    {
        Console.WriteLine("Encrypted text: " + VigenereCipher.encryptText(text, key));
    } else if (controlText == "D")
    {
        Console.WriteLine("Decrypted text: " + VigenereCipher.decryptText(text, key));
    } else if (controlText == "K")
    {
        Console.WriteLine("Please enter the maximum key length:");
        maxKeyLength = int.Parse(Console.ReadLine());
        Dictionary<int, int> commonDivisors = VigenereCipher.calculateCommonDevisors(text, maxKeyLength);
        Console.WriteLine("Possible key length values:");
        foreach (var divisor in commonDivisors.Where(p => p.Value > 1).OrderByDescending(p => p.Value))
        {
            Console.WriteLine(divisor.Key + ": " + divisor.Value);
        }
    } else if (controlText == "I")
    {
        double indexOfCoincidence = VigenereCipher.calculateIndexOfCoincidence(text);
        Console.WriteLine("Index of coincidence : " + indexOfCoincidence);
    }
    else if (controlText == "L")
    {
        double L = VigenereCipher.calculateL(text);
        Console.WriteLine("L : " + L);
    }
    else if (controlText == "B")
    {
        Console.WriteLine("Please enter the key length:");
        keyLength = int.Parse(Console.ReadLine());

        VigenereCipher.breakEncryption(text, keyLength);
    }
    else if (controlText == "P")
    {
        Console.WriteLine("Please enter a probable word:");
        probableWord = Console.ReadLine();

        VigenereCipher.breakEncryptionByProbableWord(text, probableWord);
    }

    Console.WriteLine("Do you want to exit? (Y/N)");
    controlText = Console.ReadLine();
    if (controlText == "Y")
    {
        break;
    }
}
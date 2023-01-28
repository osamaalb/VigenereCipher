using NetworksTP1;

string text = "", key = "", controlText = "";
char[] allowedInputCommands = {'E', 'D', 'B', 'I', 'L'};

while (true)
{
    Console.WriteLine("What do you want to do?\n" +
        "E: encrypt text\n" +
        "D: decrypt text\n" +
        "B: break encryption\n" +
        "I: get the index of coincidence\n" +
        "L: calculate L"
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
    } else if (controlText == "B")
    {
        Dictionary<int, int> commonDivisors = VigenereCipher.calculateCommonDevisors(text);
        Console.WriteLine("Possible key length:");
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

    Console.WriteLine("Do you want to exit? (Y/N)");
    controlText = Console.ReadLine();
    if (controlText == "Y")
    {
        break;
    }
}
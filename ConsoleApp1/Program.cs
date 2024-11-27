namespace DizwnyProgram;

using System.IO;

class Program
{
    static void ConvertNumbers()
    {
        var lines = File.ReadLines("liczby.txt");
        int evenNumbers = 0;
        int numbersWithNineDigits = 0;
        int sumOfNumbersWithNineDigits = 0;

        foreach (var line in lines)
        {
            var number = Convert.ToInt32(line, 2);
            if (number % 2 == 0)
            {
                evenNumbers++;
            }
            if (line.Length == 9)
            {
                numbersWithNineDigits++;
                sumOfNumbersWithNineDigits += number;
            }
            Console.WriteLine(number);
        }

        Console.WriteLine($"Liczba liczb parzystych: {evenNumbers}");

        var greatestNumber = lines.OrderByDescending(line => Convert.ToInt32(line, 2)).First();

        Console.WriteLine(
            $"Największa liczba: {greatestNumber}, w systemie dziesiętnym: {Convert.ToInt32(greatestNumber, 2)}"
        );
        Console.WriteLine($"Liczba liczb z 9 cyframi: {numbersWithNineDigits}");
        Console.WriteLine(
            $"Suma liczb z 9 cyframi: {Convert.ToString(sumOfNumbersWithNineDigits, 2)}"
        );
    }

    static void MaszynaSzyfrujaca()
    {
        var lines = File.ReadAllLines("wiadomosc.txt");

        var encryptedMessage = lines[0];
        var sequenceOfNumbers = lines[1]; // 3 8-bitowe liczby
        var key = lines[2];

        var liczbaPrzesuniec = key.Length;
        var liczbaPrzesuniecWLewo = 0;
        var liczbaPrzesuniecWPrawo = 0;
        var liczbaSymetrii = 0;
        var liczbaResetow = 0;

        var liczbyList = new List<string>();
        for (int i = 0; i < sequenceOfNumbers.Length; i += 8)
        {
            liczbyList.Add(sequenceOfNumbers.Substring(i, 8));
        }

        Console.WriteLine("Liczby: ");
        liczbyList.ForEach(liczba => Console.WriteLine(liczba));

        var liczbyListInt = liczbyList.Select(liczba => Convert.ToInt32(liczba, 2)).ToList();

        Console.WriteLine("Liczby int: ");
        liczbyListInt.ForEach(liczba => Console.WriteLine(liczba));

        Console.WriteLine($"Encrypted message length: {encryptedMessage.Length}");

        var CONSTANT_A1 = liczbyListInt[0];
        var CONSTANT_A2 = liczbyListInt[1];
        var CONSTANT_K = liczbyListInt[2];

        var a1InLoop = CONSTANT_A1;
        var a2InLoop = CONSTANT_A2;
        var k = CONSTANT_K;

        var decryptedMessage = new List<char>();

        var indexInEncryptedMessage = encryptedMessage.Length;

        try
        {
            for (int i = key.Length; i >= 0; i++)
            {
                var character = key[i];

                switch (character)
                {
                    case '\\':
                        // przesuń w lewo o pewną ilość znaków, która jest odpowiednią wartością ciągu liczbowego'
                        char litera = encryptedMessage[indexInEncryptedMessage];

                        var an = (a1InLoop + a2InLoop) % k;
                        Console.WriteLine($"Litera: {litera}, character: {character}, an: {an}");

                        a1InLoop = a2InLoop;
                        a2InLoop = an;

                        Console.WriteLine($"Index of a letter :{litera - an}");
                        liczbaPrzesuniecWLewo++;
                        decryptedMessage.Add(Convert.ToChar(litera - an + 64));

                        indexInEncryptedMessage--;

                        break;
                    case '/': // przesuń w prawo o pewną ilość znaków, która jest odpowiednią wartością ciągu liczbowego
                        var litera = encryptedMessage[indexInEncryptedMessage];

                        an = (a1InLoop + a2InLoop) % k;
                        Console.WriteLine($"Litera: {litera}, character: {character}, an: {an}");

                        a1InLoop = a2InLoop;
                        a2InLoop = an;

                        Console.WriteLine($"Index of a letter :{litera + an}");
                        liczbaPrzesuniecWPrawo++;
                        decryptedMessage.Add(Convert.ToChar(litera + an + 64));

                        indexInEncryptedMessage--;

                        break;
                    case '|': // symetria - wyznacz znak, któy znadjudję sie na pozycji symetrycznej względem środka 26-znakowego alfabetu lacińskiego, np dla litery B symetryczną bedzie litera Y
                        var litera = encryptedMessage[indexInEncryptedMessage];

                        liczbaSymetrii++;
                        Console.WriteLine($"Litera: {litera}");
                        var index = Convert.ToInt32(litera);
                        Console.WriteLine($"Index of a letter: {index}");
                        var middle = 79;
                        var distanceToMiddle = Math.Abs(index - middle);
                        int resultingIndex;
                        if (index < middle)
                        {
                            resultingIndex = middle + distanceToMiddle;
                        }
                        else
                        {
                            resultingIndex = middle - distanceToMiddle;
                        }
                        Console.WriteLine(
                            $"Index: {index}, middle: {middle}, distanceToMiddle: {distanceToMiddle}"
                        );
                        Console.WriteLine(resultingIndex);
                        decryptedMessage.Add(Convert.ToChar(resultingIndex + 64));

                        indexInEncryptedMessage--;

                        break;
                    case '-': // reset - powoduje powrót do wartości startowych a_1, a_2, k, gdzie a_1 < a_2 - są to pierwsze wartośći ciągu liczbowego, w którym każda kolejna wartość jest wyznacazna przez resztę dzielenia przez wartość k sumy dwóch poprzednich wartości
                        // a_n = (a_{n-2}+a_{n+1}) mod k

                        a1InLoop = CONSTANT_A1;
                        a2InLoop = CONSTANT_A2;
                        k = CONSTANT_K;
                        liczbaResetow++;
                        break;
                }
            }

            Console.WriteLine(encryptedMessage);
            Console.WriteLine(key);
            Console.WriteLine(
                $"Liczba przesunięć w lewo: {liczbaPrzesuniecWLewo}, w prawo: {liczbaPrzesuniecWPrawo}, symetrii: {liczbaSymetrii}, resetów: {liczbaResetow}"
            );
            Console.WriteLine($"Decrypted message: {decryptedMessage}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
        }
    }

    static void Main(string[] args)
    {
        ConvertNumbers();
        MaszynaSzyfrujaca();
    }
}

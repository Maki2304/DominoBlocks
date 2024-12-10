class DominoCircle
{
    public static void Main()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Circular Domino Chain Program ===");
            Console.WriteLine("1. Enter dominos manually");
            Console.WriteLine("2. Load dominos from a file");
            Console.WriteLine("3. Save dominos to a file");
            Console.WriteLine("4. Generate random dominos");
            Console.WriteLine("5. Exit");
            Console.Write("\nEnter your choice: ");

            string choice = Console.ReadLine().Trim();

            switch (choice)
            {
                case "1":
                    HandleManualInput();
                    break;
                case "2":
                    HandleLoadFromFile();
                    break;
                case "3":
                    HandleSaveToFile();
                    break;
                case "4":
                    HandleRandomDominos();
                    break;
                case "5":
                    Console.WriteLine("\nThank you for using the program. Goodbye!");
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    Freeze();
                    break;
            }
        }
    }

    static void HandleManualInput()
    {
        Console.WriteLine("\nEnter domino pairs on a single line (e.g., '2 1, 2 3, 1 3, 0 0'):");
        var dominos = ParseInputPairs(Console.ReadLine());

        if (dominos.Count == 0)
            Console.WriteLine("\nNo valid dominos entered.");
        else
            ProcessDominos(dominos, true);

        Freeze();
    }

    static void HandleLoadFromFile()
    {
        Console.Write("\nEnter the file path to load dominos: ");
        string filePath = Console.ReadLine();

        if (File.Exists(filePath))
        {
            var content = File.ReadAllText(filePath);
            var dominos = ParseInputPairs(content);

            if (dominos.Count > 0)
            {
                Console.WriteLine($"\nLoaded {dominos.Count} dominos from the file.");
                ProcessDominos(dominos, true);
            }
            else
            {
                Console.WriteLine("\nThe file did not contain any valid dominos.");
            }
        }
        else
        {
            Console.WriteLine("\nFile not found. Please check the path and try again.");
        }

        Freeze();
    }

    static void HandleSaveToFile()
    {
        Console.Write("\nEnter the file path to save dominos: ");
        string filePath = Console.ReadLine();

        Console.WriteLine("\nEnter domino pairs on a single line (e.g., '2 1, 2 3, 1 3, 0 0'):");
        var dominos = ParseInputPairs(Console.ReadLine());

        if (dominos.Count == 0)
            Console.WriteLine("\nNo valid dominos to save.");
        else
        {
            File.WriteAllText(filePath, string.Join(", ", dominos.Select(d => $"{d.Item1} {d.Item2}")));
            Console.WriteLine($"\nDominos saved successfully to {filePath}.");
        }

        Freeze();
    }

    static void HandleRandomDominos()
    {
        Console.Write("\nEnter the number of dominos to generate: ");
        if (int.TryParse(Console.ReadLine(), out int count) && count > 0)
        {
            var dominos = GenerateRandomDominos(count);
            Console.WriteLine($"\nGenerated {count} random dominos:");
            Console.WriteLine(string.Join(", ", dominos.Select(d => $"[{d.Item1}|{d.Item2}]")));

            ProcessDominos(dominos, true);
        }
        else
        {
            Console.WriteLine("\nInvalid input. Please enter a positive number.");
        }

        Freeze();
    }

    static List<(int, int)> GenerateRandomDominos(int count)
    {
        var dominos = new List<(int, int)>();
        var random = new Random();

        for (int i = 0; i < count; i++)
        {
            int a = random.Next(0, 7);
            int b = random.Next(0, 7);
            dominos.Add((a, b));
        }

        return dominos;
    }

    internal static readonly char[] separator = [',', ';', ' '];

    static List<(int, int)> ParseInputPairs(string inputPairs)
    {
        var dominos = new List<(int, int)>();
        if (string.IsNullOrWhiteSpace(inputPairs)) return dominos;

        var pairs = inputPairs.Split(separator, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < pairs.Length - 1; i += 2)
        {
            if (int.TryParse(pairs[i], out int a) && int.TryParse(pairs[i + 1], out int b))
            {
                dominos.Add((a, b));
            }
            else
            {
                Console.WriteLine($"Invalid pair: '{pairs[i]} {pairs[i + 1]}'. Skipping.");
            }
        }

        return dominos;
    }

    static void ProcessDominos(List<(int, int)> dominos, bool displayMultipleSolutions = false)
    {
        var dominoArray = dominos.ToArray();

        var solutions = new List<List<(int, int)>>();

        FindAllDominoChains(dominoArray, [], new bool[dominoArray.Length], solutions);

        if (solutions.Count > 0)
        {
            Console.WriteLine($"\nFound {solutions.Count} circular domino chains:");
            foreach (var solution in solutions)
            {
                ShowPossibleChains(solution);
                Console.WriteLine();
            }
        }
        else
        {
            Console.WriteLine("\nNo circular chain is possible.");
        }
    }

    static void FindAllDominoChains((int, int)[] dominos, List<(int, int)> chain, bool[] used, List<List<(int, int)>> solutions)
    {
        if (chain.Count == dominos.Length)
        {
            if (chain[0].Item1 == chain[^1].Item2)
                solutions.Add(new List<(int, int)>(chain));
            return;
        }

        for (int i = 0; i < dominos.Length; i++)
        {
            if (used[i]) continue;

            var current = dominos[i];
            used[i] = true;

            if (chain.Count == 0 || chain[^1].Item2 == current.Item1)
            {
                chain.Add(current);
                FindAllDominoChains(dominos, chain, used, solutions);
                chain.RemoveAt(chain.Count - 1);
            }
            else if (chain[^1].Item2 == current.Item2)
            {
                chain.Add((current.Item2, current.Item1));
                FindAllDominoChains(dominos, chain, used, solutions);
                chain.RemoveAt(chain.Count - 1);
            }

            used[i] = false;
        }
    }

    static void ShowPossibleChains(List<(int, int)> chain)
    {
        Console.WriteLine();
        foreach (var domino in chain)
            Console.Write($"[{domino.Item1}|{domino.Item2}] ");
    }

    static void Freeze()
    {
        Console.Write("\nPress any key to return to the menu...");
        Console.ReadKey();
    }
}

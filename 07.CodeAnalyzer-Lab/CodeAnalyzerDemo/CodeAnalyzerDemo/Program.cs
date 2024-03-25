// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

/// <summary>
/// This is the Main class of the Program.
/// </summary>
internal class Program
{
    private static void Main()
    {
        Console.WriteLine("Hi!");
        Console.WriteLine(MakeGreeting("Simeon"));
    }

    private static string MakeGreeting(string name)
    {
        if (name == null) // No braces and spacing issues
        {
            throw new ArgumentNullException(nameof(name));
        }

        return "Hello, " + name;
    }
}

using System;
using System.IO;

class Program
{
	static string version = "0.1";
	static void Main(string[] args)
	{
		// Write out our current version
		File.WriteAllText("version.txt", version);
		// Exit now if we were just asked for our version
		if (args.Length > 0 && args[0].Trim().ToLower() == "writeversion")
			return;

		// Continue on with the main body of the program
		Console.WriteLine("Rockets are cool.");
	}
}
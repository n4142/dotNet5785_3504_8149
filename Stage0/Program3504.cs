partial class Program
{
    private static void Main(string[] args)
    {
        welcome3504();
        welcome8149();
    }

    private static void welcome3504()
    {
        Console.WriteLine("Enter your name:");
        string name = Console.ReadLine();
        Console.WriteLine("{0},welcome to my first console application", name);
    }

    static partial void welcome8149();
}

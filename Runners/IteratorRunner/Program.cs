using Patterns.Iterator;

Console.WriteLine("Depth-first:");
foreach (string item in IteratorDemo.EnumerateDepthFirst())
{
    Console.WriteLine(item);
}

Console.WriteLine();
Console.WriteLine("Breadth-first:");
foreach (string item in IteratorDemo.EnumerateBreadthFirst())
{
    Console.WriteLine(item);
}

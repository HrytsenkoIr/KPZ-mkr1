using Patterns.State;

var result = StateDemo.Run();

Console.WriteLine("Rendered HTML:");
Console.WriteLine(result.HiddenHtml);
Console.WriteLine();
Console.WriteLine("Mutation result:");
Console.WriteLine(result.ErrorMessage);

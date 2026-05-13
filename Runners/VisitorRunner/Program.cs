using Patterns.Visitor;

HtmlStatisticsVisitor statistics = VisitorDemo.CollectStatistics();

Console.WriteLine($"Elements: {statistics.ElementCount}");
Console.WriteLine($"Text nodes: {statistics.TextNodeCount}");
Console.WriteLine($"Total text length: {statistics.TotalTextLength}");

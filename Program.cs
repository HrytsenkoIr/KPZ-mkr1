using System;

class Program
{
    static void Main(string[] args)
    {
        LightElementNode ul = new LightElementNode("ul", DisplayType.Block, ClosingType.Normal);
        ul.Classes.Add("my-list");

        LightElementNode li1 = new LightElementNode("li", DisplayType.Block, ClosingType.Normal);
        li1.Children.Add(new LightTextNode("Item 1"));

        LightElementNode li2 = new LightElementNode("li", DisplayType.Block, ClosingType.Normal);
        li2.Children.Add(new LightTextNode("Item 2"));

        LightElementNode li3 = new LightElementNode("li", DisplayType.Block, ClosingType.Normal);
        li3.Children.Add(new LightTextNode("Item 3"));

        ul.Children.Add(li1);
        ul.Children.Add(li2);
        ul.Children.Add(li3);

        Console.WriteLine("OuterHTML:");
        Console.WriteLine(ul.OuterHTML());

        Console.WriteLine("\nInnerHTML:");
        Console.WriteLine(ul.InnerHTML());
    }
}

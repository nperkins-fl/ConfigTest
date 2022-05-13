using System.Collections.Generic;

namespace ConfigTest;

public class ExampleConfig
{
    public string Setting1 { get; set; }
    public string Setting2 { get; set; }
    public string Setting3 { get; set; }
    public string[] Array { get; set; }
    public Dictionary<string, DictionaryItem> Dictionary { get; set; }
}

public class DictionaryItem
{
    public string Property1 { get; set; }
    public string Property2 { get; set; }
}
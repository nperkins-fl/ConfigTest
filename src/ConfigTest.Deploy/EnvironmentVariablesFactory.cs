using System.Collections;
using Stack = Amazon.CDK.Stack;

namespace ConfigTest.Deploy;

public static class EnvironmentVariablesFactory
{
    public static Dictionary<string, string> AddEnvironmentVariables(Stack stack, string environmentName)
    {
        var environmentVariables = new Dictionary<string, string>();
        environmentVariables.Add("ASPNETCORE_ENVIRONMENT", environmentName);

        if (stack.Node.TryGetContext(environmentName) is IDictionary context)
        {
            AddDictionary(environmentVariables, context, string.Empty);
        }

        return environmentVariables;
    }

    private static void AddDictionary(Dictionary<string, string> environmentVariables,
                                      IDictionary values,
                                      string prefix)
    {
        foreach (DictionaryEntry entry in values)
        {
            var entryKey = prefix + entry.Key;

            switch (entry.Value)
            {
                case IDictionary d:
                    AddDictionary(environmentVariables, d, entryKey + "__");
                    break;
                case Array a:
                    AddArray(environmentVariables, a, entryKey + "__");
                    break;
                case string s:
                    environmentVariables.Add(entryKey, s);
                    break;
            }
        }
    }

    private static void AddArray(Dictionary<string, string> environmentVariables, Array array, string prefix)
    {
        for (var i = 0; i < array.Length; i++)
        {
            var entryKey = prefix + i;
            var entryValue = array.GetValue(i);

            switch (entryValue)
            {
                case IDictionary d:
                    AddDictionary(environmentVariables, d, entryKey + "__");
                    break;
                case Array a:
                    AddArray(environmentVariables, a, entryKey + "__");
                    break;
                case string s:
                    environmentVariables.Add(entryKey, s);
                    break;
            }
        }
    }
}
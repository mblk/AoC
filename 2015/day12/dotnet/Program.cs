using System.Text.Json.Nodes;

const string inputFile = "../../../input.txt";

var rootNode = JsonNode.Parse(File.ReadAllText(inputFile)) 
               ?? throw new Exception("Invalid input");

Console.WriteLine($"Part1: {Visit(rootNode, false)}");
Console.WriteLine($"Part2: {Visit(rootNode, true)}");

int Visit(JsonNode? node, bool ignoreRed) => node switch
    {
        JsonObject jsonObject when !ignoreRed || !ObjectHasRedProperty(jsonObject)
            => jsonObject.Sum(property => Visit(property.Value, ignoreRed)),
        
        JsonArray jsonArray
            => jsonArray.Sum(value => Visit(value, ignoreRed)),
        
        JsonValue jsonValue when jsonValue.TryGetValue(out int intValue)
            => intValue,
        
        _ => 0
    };

bool ObjectHasRedProperty(JsonObject jsonObject) => jsonObject
    .Select(pair => pair.Value)
    .OfType<JsonValue>()
    .Any(value => value.TryGetValue(out string? stringValue) &&
                  stringValue == "red");

// See https://aka.ms/new-console-template for more information
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
// $.00081190.Value[0]
//vr = CS        , $.00080005.vr
//Value = ISO_IR 192, $.00080005.Value[0]


string jsonInput = File.ReadAllText("dicomsample.json");
// regex to test wheather string is in DICOMTag format
Regex regex = new Regex("^[0-9a-fA-F]{8}$");
var jsonOptions = new System.Text.Json.JsonDocumentOptions() { CommentHandling = System.Text.Json.JsonCommentHandling.Skip, AllowTrailingCommas = true };


var parsedDoc = JsonNode.Parse(jsonInput, documentOptions:jsonOptions);

ProcessJsonNode(parsedDoc?.AsObject(), string.Empty, string.Empty);


Console.WriteLine("Done");


// Process JsonObject
//{ 
//    "00080005": {
//        "vr": "CS",               // JsonValue
//        "Value": [ "ISO_IR 192" ] // JsonArray
//      }
//}
void ProcessJsonNode(JsonObject ? jinputObject, string tag, string vr)
{
    if ( jinputObject is null) { return; }

    foreach (var kvp in jinputObject)
    {
        tag = regex.IsMatch(kvp.Key) ? kvp.Key : tag;
        var value = kvp.Value;
        if (value is JsonValue jsonValue)
        {
            if (kvp.Key != "vr")
            {
                Console.WriteLine($"dcmTag:{tag} dcmVR:{vr} dcmValue:{jsonValue,-10}, {jsonValue.GetPath(),-10}");
            }
            else
            {
                vr = kvp.Key == "vr" ? (string)value : vr;
            }
        }
        else if (value is JsonObject jObject)
        {
            ProcessJsonNode(jObject, tag, vr);
        }
        else if (value is JsonArray jArray)
        {
            ProcessJsonArray(jArray, tag, vr, kvp.Key);
        }
    }

    // local function to process JsonArray
    void ProcessJsonArray(JsonArray jinputArray, string tag, string vr, string key)
    {
        foreach (var item in jinputArray)
        {
            if (item is JsonValue jValue)
            {
                Console.WriteLine($"dcmTag:{tag} dcmVR:{vr} dcmValue:{jValue,-10}, {jValue.GetPath(),-10}");
            }
            else if (item is JsonObject jobj)
            {
                ProcessJsonNode(jobj, tag, vr);
            }
            else if (item is JsonArray jarr)
            {
                ProcessJsonArray(jarr, tag, vr, key);
            }
        }
    }
}



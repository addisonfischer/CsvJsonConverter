using System;
using System.Collections.Generic;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapPost("/test", (Input input) =>
{
    var json = csv2Json(input.data, input.delimeter);
    return Results.Text(json, "application/json");
});

static string csv2Json(string data, string delimeter)
{
    if (string.IsNullOrEmpty(data)) return "No data provided, please be sure to pass CSV data to the 'data' parameter.";
    if (string.IsNullOrEmpty(delimeter)) return "No delimeter provided, please be sure to pass a delimeter to the 'delimeter' parameter.";
    if (delimeter.Length != 1) return "Delimeter must be a single character";
    if (!data.Contains(delimeter)) return "Delimeter not found in data";

    var lines = data.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
        .Where(line => !string.IsNullOrWhiteSpace(line))
        .ToArray();

    if (lines.Length < 2) return "Insufficient CSV format: CSV should have at least 2 lines of data";

    var headers = lines[0].Split(delimeter);

    var response = new List<string>();

    foreach (var line in lines.Skip(1))
    {

        if (string.IsNullOrWhiteSpace(line)) continue;

        var values = line.Split(delimeter);

        var objectData = headers.Zip(values, (header, value) =>
        {
            if (int.TryParse(value, out _) || decimal.TryParse(value, out _))
                return $"\"{header}\":{value}";
            else
                return $"\"{header}\":\"{value}\"";
        });
        response.Add("{" + string.Join(",", objectData) + "}");
    }
    return "[" + string.Join(",", response) + "]";
}

app.Run();

public record Input(string data, string delimeter);



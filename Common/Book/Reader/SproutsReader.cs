using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;

namespace BookImpl.Reader;

public class SproutsReader : IDisposable
{
    private readonly List<Entry> _entries = new();
    private readonly Dictionary<FieldRole, int> _columnIndexes = new();


    public Book Read(string path)
    {
        using (var parser = new TextFieldParser(path))
        {
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters("\t");

            var headers = parser.ReadFields();
            if (headers is null)
            {
                throw new Exception("headers is null");
            }

            ReadHeaders(headers);

            while (!parser.EndOfData)
            {
                //Processing row
                var fields = parser.ReadFields();
                if (fields is null)
                {
                    throw new Exception("fields is null");
                }

                ReadFields(fields);
            }
        }

        return new Book("sprouts", _entries);
    }

    public void Dispose()
    {
    }

    private void ReadHeaders(string[] headers)
    {
        for (var i = 0; i < headers.Length; i++)
        {
            var role = SproutsHeaders.FindFieldRole(headers[i]);
            if (!role.HasValue)
            {
                continue;
            }

            if (!_columnIndexes.TryAdd(role.Value, i))
            {
                throw new Exception("wrong header");
            }
        }

        foreach (var field in (FieldRole[])Enum.GetValues(typeof(FieldRole)))
        {
            if (field.IsRequired() && !_columnIndexes.ContainsKey(field))
            {
                throw new Exception($"missed required column: {field}");
            }
        }
    }

    private void ReadFields(string[] fields)
    {

    }
}
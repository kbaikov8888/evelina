using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookImpl.Reader;

public class SproutsReader : IDisposable
{
    private readonly Dictionary<FieldRole, int> _columnIndexes = new();
    private readonly List<Transaction> _failedTransactions = new();

    private readonly Book _book = new("book");

    private readonly bool _demoMode;
    private readonly Random _random = new();


    public SproutsReader(bool demoMode)
    {
        _demoMode = demoMode;
    }


    public Book? TryRead(string path)
    {
        Read(path);
        _book.Finilize();

        return _book;
    }

    public void Dispose()
    {
        _columnIndexes.Clear();
        _failedTransactions.Clear();
    }

    private void Read(string path)
    {
        using (var parser = new TextFieldParser(path, Encoding.UTF8))
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
    }

    private void ReadHeaders(string[] headers)
    {
        for (var i = 0; i < headers.Length; i++)
        {
            var role = SproutsHelper.FindFieldRole(headers[i]);
            if (!role.HasValue)
            {
                continue;
            }

            if (!_columnIndexes.TryAdd(role.Value, i))
            {
                throw new Exception("wrong header");
            }
        }
    }

    private void ReadFields(string[] fields)
    {
        var tr = new Transaction();

        foreach (var field in (FieldRole[])System.Enum.GetValues(typeof(FieldRole)))
        {
            if (_columnIndexes.TryGetValue(field, out var index))
            {
                tr.SetField(field, fields[index]);
            }
        }

        if (!tr.IsValid())
        {
            _failedTransactions.Add(tr);
            return;
        }

        if (_demoMode)
        {
            tr.Amount *= _random.NextDouble() / 2;
        }

        tr.CreateEntry(_book);
    }
}

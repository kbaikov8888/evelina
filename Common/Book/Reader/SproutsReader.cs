using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookImpl.Reader;

public class SproutsReader : IDisposable
{
    private readonly Dictionary<FieldRole, int> _columnIndexes = new();
    private readonly List<Transaction> _failedTransactions = new();

    private Book? _book;


    public Book? TryRead(string path)
    {
        _book = new Book("sprouts");

        try
        {
            Read(path);
        }
        catch (Exception ex)
        {
            _book = null;
        }

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

        //foreach (var field in (FieldRole[])Enum.GetValues(typeof(FieldRole)))
        //{
        //    if (field.IsRequired() && !_columnIndexes.ContainsKey(field))
        //    {
        //        throw new Exception($"missed required column: {field}");
        //    }
        //}
    }

    private void ReadFields(string[] fields)
    {
        var tr = new Transaction();

        foreach (var field in (FieldRole[])Enum.GetValues(typeof(FieldRole)))
        {
            if (_columnIndexes.TryGetValue(field, out int index))
            {
                tr.SetField(field, fields[index]);
            }
        }

        if (!tr.IsValid())
        {
            _failedTransactions.Add(tr);
            return;
        }

        CreateEntry(tr);
    }

    private void CreateEntry(Transaction transaction)
    {
        var currencyRate = transaction.CurrencyRate ?? 1;
        var amount = transaction.Amount ?? throw new InvalidOperationException();
        amount *= currencyRate;
        var dateTime = transaction.Datetime ?? throw new InvalidOperationException();

        if (transaction.Type == Type.expense)
        {
            amount *= -1;

            var account = transaction.Account ?? throw new InvalidOperationException();
            var category = transaction.Category ?? throw new InvalidOperationException();

            var bookCategory = _book.GetOrCreateExpenseCategory(category);
            var bookAccount = _book.GetOrCreateAccount(account);

            _book.AddEntry(new ExpenseEntry(amount, dateTime, bookAccount, bookCategory));
        }
    }
}
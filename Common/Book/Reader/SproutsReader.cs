using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookImpl.Reader;

public class SproutsReader : IDisposable
{
    private readonly Dictionary<FieldRole, int> _columnIndexes = new();
    private readonly List<Transaction> _failedTransactions = new();

    private Book _book = new("sprouts");


    public Book? TryRead(string path)
    {
        try
        {
            Read(path);
        }
        catch (Exception ex)
        {
            return null;
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

        foreach (var field in (FieldRole[])System.Enum.GetValues(typeof(FieldRole)))
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

        Entry entry;

        if (transaction.Type is Type.expense or Type.income)
        {
            var account = transaction.Account ?? throw new InvalidOperationException();
            var category = transaction.Category ?? throw new InvalidOperationException();

            var project = transaction.Project != null ? _book.GetOrCreateProject(transaction.Project) : null;

            var bankAccount = _book.GetOrCreateBankAccount(account);

            if (transaction.Type is Type.expense)
            {
                var expenseCategory = _book.GetOrCreateExpenseCategory(category, transaction.ParentCategory);

                amount *= -1;

                entry = new ExpenseEntry(amount, dateTime, bankAccount, expenseCategory);
            }
            else
            {
                var incomeCategory = _book.GetOrCreateIncomeCategory(category, transaction.ParentCategory);

                entry = new IncomeEntry(amount, dateTime, bankAccount, incomeCategory);
            }

            entry.Project = project;
            entry.Note = transaction.Note;
        }
        else if (transaction.Type == Type.transfer)
        {
            var sender = transaction.ParentCategory ?? throw new InvalidOperationException();
            var receiver = transaction.ReceivableAccount ?? throw new InvalidOperationException();

            var senderInvest = sender.StartsWith("_i");
            var receiverInvest = receiver.StartsWith("_i");

            Account senderAccount = senderInvest ? _book.GetOrCreateInvestAccount(sender) : _book.GetOrCreateBankAccount(sender);
            Account receiverAccount = receiverInvest ? _book.GetOrCreateInvestAccount(receiver) : _book.GetOrCreateBankAccount(receiver);

            if (senderInvest == receiverInvest)
            {
                entry = new TransferEntry(amount, dateTime, senderAccount, receiverAccount);
            }
            else if (senderInvest)
            {
                entry = new ReInvestingEntry(amount, dateTime, (InvestAccount)senderAccount, receiverAccount);
            }
            else
            {
                entry = new InvestingEntry(amount, dateTime, senderAccount, (InvestAccount)receiverAccount);
            }
        }
        else
        {
            throw new InvalidOperationException();
        }

        _book.AddEntry(entry);
    }
}

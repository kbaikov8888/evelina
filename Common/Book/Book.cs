using System;
using System.Collections.Generic;
using System.Linq;

namespace BookImpl;

public class Book
{
    public string Name { get; }

    private readonly List<Entry> _entries = new();
    private readonly List<ExpenseCategory> _expenses = new();
    private readonly List<IncomeCategory> _incomes = new();
    private readonly List<Account> _accounts = new();


    public Book(string name)
    {
        Name = name;
    }


    public List<Entry> GetEntries()
    {
        return _entries.ToList();
    }

    public void AddEntry(Entry entry)
    {
        if (_entries.Contains(entry))
        {
            throw new InvalidOperationException();
        }

        _entries.Add(entry);
    }

    public ExpenseCategory GetOrCreateExpenseCategory(string name)
    {
        var existed =
            _expenses.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase));
        if (existed is not null)
        {
            return existed;
        }

        existed = new ExpenseCategory(name);
        _expenses.Add(existed);
        return existed;
    }

    public Account GetOrCreateAccount(string name)
    {
        var existed =
            _accounts.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase));
        if (existed is not null)
        {
            return existed;
        }

        existed = new Account(name);
        _accounts.Add(existed);
        return existed;
    }
}
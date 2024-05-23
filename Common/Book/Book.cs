using System;
using System.Collections.Generic;
using System.Linq;

namespace BookImpl;

public class Book
{
    public string Name { get; }

    private readonly List<Entry> _entries = new();
    private readonly List<ExpenseCategory> _expenseCategories = new();
    private readonly List<IncomeCategory> _incomeCategories = new();
    private readonly List<BankAccount> _bankAccounts = new();
    private readonly List<InvestAccount> _investAccounts = new();
    private readonly List<Project> _projects = new();


    public Book(string name)
    {
        Name = name;
    }


    public List<Entry> GetEntries()
    {
        return _entries.ToList();
    }

    internal void AddEntry(Entry entry)
    {
        if (_entries.Contains(entry))
        {
            throw new InvalidOperationException("Entry already exist!");
        }

        _entries.Add(entry);
    }

    internal ExpenseCategory GetOrCreateExpenseCategory(string name, string? parentName)
    {
        var existed = _expenseCategories.FirstOrDefault(x => NamesEqual(x.Name, name));
        if (existed is not null)
        {
            if (!NamesEqual(existed.ParentCategory?.Name, parentName))
            {
                throw new InvalidOperationException("Category exist, but parent category not match!");
            }

            return existed;
        }

        existed = new ExpenseCategory(name);

        if (parentName is not null)
        {
            var existedParent = _expenseCategories.FirstOrDefault(x => NamesEqual(x.Name, parentName));
            if (existedParent is not null)
            {
                if (existedParent.ParentCategory is not null)
                {
                    throw new InvalidOperationException("Parent category can't have parent!");
                }
            }
            else
            {
                existedParent = new ExpenseCategory(parentName);
                _expenseCategories.Add(existedParent);
            }

            existed.ParentCategory = existedParent;
        }

        _expenseCategories.Add(existed);
        return existed;
    }

    internal IncomeCategory GetOrCreateIncomeCategory(string name, string? parentName)
    {
        var existed = _incomeCategories.FirstOrDefault(x => NamesEqual(x.Name, name));
        if (existed is not null)
        {
            return existed;
        }

        existed = new IncomeCategory(name);
        _incomeCategories.Add(existed);
        return existed;
    }

    internal BankAccount GetOrCreateBankAccount(string name)
    {
        var existed = _bankAccounts.FirstOrDefault(x => NamesEqual(x.Name, name));
        if (existed is not null)
        {
            return existed;
        }

        existed = new BankAccount(name);
        _bankAccounts.Add(existed);
        return existed;
    }

    internal InvestAccount GetOrCreateInvestAccount(string name)
    {
        var existed = _investAccounts.FirstOrDefault(x => NamesEqual(x.Name, name));
        if (existed is not null)
        {
            return existed;
        }

        existed = new InvestAccount(name);
        _investAccounts.Add(existed);
        return existed;
    }
    internal Project GetOrCreateProject(string name)
    {
        var existed = _projects.FirstOrDefault(x => NamesEqual(x.Name, name));
        if (existed is not null)
        {
            return existed;
        }

        existed = new Project(name);
        _projects.Add(existed);
        return existed;
    }

    internal static bool NamesEqual(string? name1, string? name2)
    {
        if (name1 is null && name2 is null)
        {
            return true;
        }

        if (name1 is null || name2 is null)
        {
            return false;
        }

        return string.Equals(name1, name2, StringComparison.InvariantCulture);
    }
}
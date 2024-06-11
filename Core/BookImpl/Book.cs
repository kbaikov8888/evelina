using BookImpl.Elements;
using BookImpl.Enum;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookImpl;

public class Book
{
    public static Currency DefaultCurrency = Currency.RUB;

    public string Name { get; }

    public BookCalculatedData CalculatedData { get; }

    private readonly HashSet<Entry> _entries = new();

    public IReadOnlyList<Category> AllCategories =>
        _expenseCategories.Concat(_incomeCategories.Cast<Category>()).ToList();

    public IReadOnlyList<ExpenseCategory> ParentExpenseCategories =>
        _expenseCategories.Select(x => x.ParentCategory)
            .Where(x => x is not null)
            .ToHashSet().Cast<ExpenseCategory>().ToList();
    public IReadOnlyList<ExpenseCategory> ExpenseCategories => _expenseCategories.ToList();
    private readonly HashSet<ExpenseCategory> _expenseCategories = new();

    public IReadOnlyList<IncomeCategory> ParentIncomeCategories =>
        _incomeCategories.Select(x => x.ParentCategory)
            .Where(x => x is not null)
            .ToHashSet().Cast<IncomeCategory>().ToList();
    public IReadOnlyList<IncomeCategory> IncomeCategories => _incomeCategories.ToList();
    private readonly HashSet<IncomeCategory> _incomeCategories = new();

    public IReadOnlyList<BankAccount> BankAccounts => _bankAccounts.ToList();
    private readonly HashSet<BankAccount> _bankAccounts = new();

    public IReadOnlyList<InvestAccount> InvestAccounts => _investAccounts.ToList();
    private readonly HashSet<InvestAccount> _investAccounts = new();

    public IReadOnlyList<Project> Projects => _projects.ToList();
    private readonly HashSet<Project> _projects = new();

    public IReadOnlyList<InvestAccountFamily> InvestAccountFamilies => _investAccountFamilies.ToList();
    private readonly HashSet<InvestAccountFamily> _investAccountFamilies = new();


    public Book(string name)
    {
        Name = name;
        CalculatedData = new BookCalculatedData(this);
    }


    // public
    public List<Entry> GetEntriesFromLast()
    {
        return _entries.ToList();
    }

    public List<Entry> GetEntriesFromFirst()
    {
        var res = _entries.ToList();
        res.Reverse();

        return res;
    }

    // private
    internal void AddEntry(Entry entry)
    {
        if (_entries.Contains(entry))
        {
            throw new InvalidOperationException("Entry already exist!");
        }

        _entries.Add(entry);
    }

    private static T GetOrCreateCategory<T>(ICollection<T> source, string name, string? parentName, Func<string, T> createFunc) where T : Category
    {
        var existed = source.FirstOrDefault(x => NamesEqual(x.Name, name));
        if (existed is not null)
        {
            if (!NamesEqual(existed.ParentCategory?.Name, parentName))
            {
                throw new InvalidOperationException("Category exist, but parent category not match!");
            }

            return existed;
        }

        existed = createFunc(name);

        if (parentName is not null)
        {
            var existedParent = source.FirstOrDefault(x => NamesEqual(x.Name, parentName));
            if (existedParent is not null)
            {
                if (existedParent.ParentCategory is not null)
                {
                    throw new InvalidOperationException("Parent category can't have parent!");
                }
            }
            else
            {
                existedParent = createFunc(parentName);
                source.Add(existedParent);
            }

            existed.ParentCategory = existedParent;
        }

        source.Add(existed);
        return existed;
    }

    internal ExpenseCategory GetOrCreateExpenseCategory(string name, string? parentName)
    {
        return GetOrCreateCategory<ExpenseCategory>(_expenseCategories, name, parentName, s => new ExpenseCategory(s));
    }

    internal IncomeCategory GetOrCreateIncomeCategory(string name, string? parentName)
    {
        return GetOrCreateCategory<IncomeCategory>(_incomeCategories, name, parentName, s => new IncomeCategory(s));
    }

    internal BankAccount GetOrCreateBankAccount(string name)
    {
        var existed = _bankAccounts.FirstOrDefault(x => NamesEqual(x.Name, name));
        if (existed is not null)
        {
            return existed;
        }

        //TODO bank accounts not only with DefaultCurrency
        existed = new BankAccount(name, DefaultCurrency);
        _bankAccounts.Add(existed);
        return existed;
    }

    internal InvestAccount GetOrCreateInvestAccount(string name)
    {
        if (!name.StartsWith("_i") || name.Length < 12 || name[2] != '(' || name[5] != ')')
        {
            throw new InvalidOperationException();
        }

        var familySubstring = name.Substring(3, 2);
        var currencySubstring = name.Substring(name.Length - 3, 3);
        var nameSubstring = name.Substring(7, name.Length - 11);

        if (!System.Enum.TryParse(currencySubstring, out Currency currency))
        {
            throw new InvalidOperationException();
        }

        nameSubstring = currency != DefaultCurrency ? $"{nameSubstring} {currency}" : nameSubstring;

        var family = _investAccountFamilies.FirstOrDefault(x => NamesEqual(x.Name, familySubstring));
        if (family is null)
        {
            family = new InvestAccountFamily(familySubstring);
            _investAccountFamilies.Add(family);
        }

        var existed = _investAccounts.FirstOrDefault(x => NamesEqual(x.Name, nameSubstring));
        if (existed is not null)
        {
            if (existed.Currency != currency || existed.Family != family)
            {
                throw new InvalidOperationException();
            }

            return existed;
        }

        existed = new InvestAccount(nameSubstring, currency, family);
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
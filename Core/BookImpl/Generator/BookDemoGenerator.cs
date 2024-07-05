using BookImpl.Elements;
using BookImpl.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Distributions;

namespace BookImpl.Generator;

public class BookDemoGenerator : IDisposable
{
    private readonly Book _book = new("demo");

    private readonly Random _random;
    private readonly GeneratorParameters _parameters;
    private readonly IContinuousDistribution _amountDistr;

    private DateTime[] _dates;
    private BankAccount[] _bankAccounts;
    private InvestAccount[] _investAccounts;
    private ExpenseCategory[] _expenseCategories;
    private IncomeCategory[] _incomeCategories;
 
    private uint _currentTransaction;

    public BookDemoGenerator(int seed, GeneratorParameters parameters)
    {
        _parameters = parameters;

        _random = new Random(seed);
        _dates = new DateTime[parameters.TransactionsCount];
        _amountDistr = new Normal(10000, 1000, _random);
        _bankAccounts = new BankAccount[_parameters.BankAccounts];
        _investAccounts = new InvestAccount[_parameters.InvestAccounts];
        _expenseCategories = new ExpenseCategory[_parameters.ExpenseCategories];
        _incomeCategories = new IncomeCategory[_parameters.IncomeCategories];
    }

    public Book Generate()
    {
        GenerateAccounts();
        CreateCategories();
        GenerateDates();
        GenerateTransactions();

        _book.Finilize();

        return _book;
    }

    public void Dispose()
    {
    }

    private void GenerateDates()
    {
        var start = _parameters.StartDate.Ticks;
        var end = _parameters.EndDate.Ticks;

        var delta = end - start;

        for (uint i = 0; i < _parameters.TransactionsCount; i++)
        {
            _dates[i] = new DateTime(start + _random.NextInt64(delta));
        }

        _dates = _dates.OrderBy(x => x.Ticks).ToArray();
    }

    private void GenerateTransactions()
    {
        for (uint i = 0; i < _parameters.TransactionsCount; ++i)
        {
            _currentTransaction = i;
            _book.AddEntry(GetEntry());
        }
    }

    private void GenerateAccounts()
    {
        for (int i = 0; i < _bankAccounts.Length; i++)
        {
            _bankAccounts[i] = _book.GetOrCreateBankAccount($"Bank {i}");
        }

        for (int i = 0; i < _investAccounts.Length; ++i)
        {
            _investAccounts[i] = _book.GetOrCreateInvestAccount($"_i(a{i}) Invest {i} {Book.DefaultCurrency}");
        }
    }

    private void CreateCategories()
    {
        var expenseParents = new string[_parameters.ExpenseParentCategories];
        for (int i = 0; i < expenseParents.Length; i++)
        {
            expenseParents[i] = $"EXP CAT {i}";
        }

        var incomeParents = new string[_parameters.IncomeParentCategories];
        for (int i = 0; i < incomeParents.Length; i++)
        {
            incomeParents[i] = $"INC CAT {i}";
        }

        for (int i = 0; i < _expenseCategories.Length; i++)
        {
            _expenseCategories[i] = _book.GetOrCreateExpenseCategory($"Exp Cat {i}", GetElement(expenseParents));
        }

        for (int i = 0; i < _incomeCategories.Length; i++)
        {
            _incomeCategories[i] = _book.GetOrCreateIncomeCategory($"Inc Cat {i}", GetElement(incomeParents));
        }
    }

    private T GetByWeights<T>(List<(T, int)> weights)
    {
        var sum = weights.Select(x => x.Item2).Sum();
        var rnd = _random.Next(sum);

        int count = 0;
        for (int i = 0; i < weights.Count - 1; i++)
        {
            count += weights[i].Item2;
            if (rnd < count)
                return weights[i].Item1;
        }

        return weights[weights.Count - 1].Item1;
    }

    private T GetElement<T>(IEnumerable<T> elements) where T : class
    {
        var list = elements.ToList();
        return list[_random.Next(list.Count)];
    }

    private Entry GetEntry()
    {
        var type = GetByWeights(_parameters.EntyrWeights);
        return type switch
        {
            EntryType.Expense => CreateExpenseEntry(),
            EntryType.Income => CreateIncomeEntry(),
            EntryType.Transfer => CreateTransferEntry(),
            EntryType.Invest => CreateInvestingEntry(),
            EntryType.ReInvest => CreateRevestingEntry(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private ExpenseEntry CreateExpenseEntry()
    {
        var cat = GetElement(_expenseCategories);
        var acc = GetElement(_bankAccounts);
        var amount = _amountDistr.Sample();
        var date = _dates[_currentTransaction];

        return new ExpenseEntry(amount, date, acc, cat, 1);
    }

    private IncomeEntry CreateIncomeEntry()
    {
        var cat = GetElement(_incomeCategories);
        var acc = GetElement(_bankAccounts);
        var amount = _amountDistr.Sample();
        var date = _dates[_currentTransaction];

        return new IncomeEntry(amount, date, acc, cat, 1);
    }

    private TransferEntry CreateTransferEntry()
    {
        var acc1 = GetElement(_bankAccounts);
        var acc2 = GetElement(_bankAccounts.Where(x=>x != acc1));
        var amount = _amountDistr.Sample();
        var date = _dates[_currentTransaction];

        return new TransferEntry(date, amount, amount, acc1, acc2, 1, 1);
    }

    private InvestingEntry CreateInvestingEntry()
    {
        var acc1 = GetElement(_bankAccounts);
        var acc2 = GetElement(_investAccounts);
        var amount = _amountDistr.Sample();
        var date = _dates[_currentTransaction];

        return new InvestingEntry(date, amount, amount, acc1, acc2, 1, 1);
    }

    private ReInvestingEntry CreateRevestingEntry()
    {
        var acc1 = GetElement(_bankAccounts);
        var acc2 = GetElement(_investAccounts);
        var amount = _amountDistr.Sample();
        var date = _dates[_currentTransaction];

        return new ReInvestingEntry(date, amount, amount, acc2, acc1, 1, 1);
    }
}
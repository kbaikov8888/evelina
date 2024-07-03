using BookImpl.Elements;
using BookImpl.Enum;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookImpl.Generator;

internal class SproutsDemoGenerator : IDisposable
{
    private readonly Book _book = new("book");

    private readonly Random _random;
    private readonly GeneratorParameters _parameters;

    private DateTime[] _dates;

    private uint _currentTransaction;

    public SproutsDemoGenerator(int seed, GeneratorParameters parameters)
    {
        _random = new Random(seed);
        _parameters = parameters;
        _dates = new DateTime[parameters.TransactionsCount];
    }

    public Book Generate()
    {
        GenerateDates();
        GenerateTransactions();

        return _book;
    }

    public void Dispose()
    {
    }

    private void GenerateDates()
    {
        var start = _parameters.StartDate.TimeOfDay;
        var end = _parameters.EndDate.TimeOfDay;

        var delta = end - start;

        for (uint i = 0; i < _parameters.TransactionsCount; i++)
        {
            var newSpan = new TimeSpan(0, _random.Next(0, (int)delta.TotalMinutes), 0);
            _dates[i] = new DateTime((start + newSpan).Ticks);
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

    private T? GetElement<T>(IEnumerable<T> elements) where T : class
    {
        var list = elements.ToList();

        if (!list.Any())
        {
            return null;
        }

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
        var cat = GetElement(_book.ExpenseCategories.Where(x => x.ParentCategory is not null));
        if (_random.Next(100) < _parameters.ProbabilityToGenerateNewCategory || cat is null)
        {
            cat = new ExpenseCategory($"cat{_currentTransaction}");

            var parent = GetElement(_book.ParentExpenseCategories);
            if (parent is null)
            {
                parent = new ExpenseCategory($"pCat{_currentTransaction}");
            }

            cat.ParentCategory = parent;
        }

        var acc = GetElement(_book.BankAccounts);
        if (_random.Next(100) < _parameters.ProbabilityToGenerateNewAccount || acc is null)
        {
            acc = new BankAccount($"acc{_currentTransaction}", Book.DefaultCurrency);
        }

        //TODO normal distribution
        var amount = _random.Next(10000);

        return new ExpenseEntry(amount, _dates[_currentTransaction], acc, cat, 1);
    }

    private IncomeEntry CreateIncomeEntry()
    {

    }

    private TransferEntry CreateTransferEntry()
    {

    }

    private InvestingEntry CreateInvestingEntry()
    {

    }

    private ReInvestingEntry CreateRevestingEntry()
    {

    }
}
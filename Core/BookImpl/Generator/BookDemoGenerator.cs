using BookImpl.Elements;
using BookImpl.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Distributions;

namespace BookImpl.Generator;

public class BookDemoGenerator : IDisposable
{
    private readonly Book _book = new("book");

    private readonly Random _random;
    private readonly GeneratorParameters _parameters;
    private readonly IContinuousDistribution _amountDistr;

    private DateTime[] _dates;

    private uint _currentTransaction;

    public BookDemoGenerator(int seed, GeneratorParameters parameters)
    {
        _random = new Random(seed);
        _parameters = parameters;
        _dates = new DateTime[parameters.TransactionsCount];
        _amountDistr = new Normal(10000, 1000, _random);
    }

    public Book Generate()
    {
        GenerateDates();
        GenerateTransactions();

        _book.CalculatedData.Calculate();

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

    private T GetCategory<T>(IEnumerable<T> categories, IEnumerable<T> parentCategories, Func<string, T> createFunc) where T : Category
    {
        var cat = GetElement(categories.Where(x => x.ParentCategory is not null));
        if (_random.Next(100) < _parameters.ProbabilityToGenerateNewCategory || cat is null)
        {
            cat = createFunc($"cat{_currentTransaction}");

            var parent = GetElement(parentCategories);
            if (parent is null)
            {
                parent = createFunc($"pCat{_currentTransaction}");
            }

            cat.ParentCategory = parent;
        }

        return cat;
    }

    private BankAccount GetBankAccount()
    {
        var acc = GetElement(_book.BankAccounts);
        if (_random.Next(100) < _parameters.ProbabilityToGenerateNewAccount || acc is null)
        {
            acc = new BankAccount($"acc{_currentTransaction}", Book.DefaultCurrency);
        }

        return acc;
    }

    private InvestAccount GetInvestAccount()
    {
        var acc = GetElement(_book.InvestAccounts);
        if (_random.Next(100) < _parameters.ProbabilityToGenerateNewAccount || acc is null)
        {
            var family = GetElement(_book.InvestAccountFamilies);
            if (family is null)
            {
                family = new InvestAccountFamily($"fam{_currentTransaction}");
            }

            acc = new InvestAccount($"acc{_currentTransaction}", Book.DefaultCurrency, family);
        }

        return acc;
    }

    private ExpenseEntry CreateExpenseEntry()
    {
        var cat = GetCategory(_book.ExpenseCategories, _book.ParentExpenseCategories,
            s => new ExpenseCategory(s));
        var acc = GetBankAccount();

        return new ExpenseEntry(_amountDistr.Sample(), _dates[_currentTransaction], acc, cat, 1);
    }

    private IncomeEntry CreateIncomeEntry()
    {
        var cat = GetCategory(_book.IncomeCategories, _book.ParentIncomeCategories,
            s => new IncomeCategory(s));
        var acc = GetBankAccount();

        return new IncomeEntry(_amountDistr.Sample(), _dates[_currentTransaction], acc, cat, 1);
    }

    private TransferEntry CreateTransferEntry()
    {
        var acc1 = GetBankAccount();
        var acc2 = GetBankAccount();
        var amount = _amountDistr.Sample();

        return new TransferEntry(_dates[_currentTransaction], amount, amount, acc1, acc2, 1, 1);
    }

    private InvestingEntry CreateInvestingEntry()
    {
        var acc1 = GetBankAccount();
        var acc2 = GetInvestAccount();
        var amount = _amountDistr.Sample();

        return new InvestingEntry(_dates[_currentTransaction], amount, amount, acc1, acc2, 1, 1);
    }

    private ReInvestingEntry CreateRevestingEntry()
    {
        var acc1 = GetBankAccount();
        var acc2 = GetInvestAccount();
        var amount = _amountDistr.Sample();

        return new ReInvestingEntry(_dates[_currentTransaction], amount, amount, acc2, acc1, 1, 1);
    }
}
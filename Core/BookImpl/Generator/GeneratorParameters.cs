using BookImpl.Enum;
using System;
using System.Collections.Generic;

namespace BookImpl.Generator;

public readonly struct GeneratorParameters
{
    public uint TransactionsCount { get; init; } = 3000;
    public int StartYear { get; init; } = 2020;
    public int EndYear { get; init; } = DateTime.Today.Year - 1;

    public List<(EntryType, int)> EntyrWeights { get; init; } = new()
    {
        (EntryType.Expense, 70),
        (EntryType.Income, 100),
        (EntryType.Transfer, 60),
        (EntryType.Invest, 20),
        (EntryType.ReInvest, 2),
    };
    public uint ExpenseCategories { get; init; } = 20;
    public uint ExpenseParentCategories { get; init; } = 10;
    public uint IncomeCategories { get; init; } = 10;
    public uint IncomeParentCategories { get; init; } = 5;
    public uint BankAccounts { get; init; } = 6; // > 1
    public uint InvestAccounts { get; init; } = 3;

    public DateTime StartDate => new(StartYear, 1, 1);
    public DateTime EndDate => new(EndYear, 12, 31);


    public GeneratorParameters()
    {
    }
}
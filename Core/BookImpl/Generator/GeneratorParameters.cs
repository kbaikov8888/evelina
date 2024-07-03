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
        (EntryType.Expense, 10),
        (EntryType.Income, 8),
        (EntryType.Transfer, 6),
        (EntryType.Invest, 2),
        (EntryType.ReInvest, 1),
    };
    public uint ProbabilityToGenerateNewCategory { get; init; } = 10;
    public uint ProbabilityToGenerateNewAccount { get; init; } = 5;

    public DateTime StartDate => new(StartYear, 0, 0);
    public DateTime EndDate => new(EndYear, 12, 31);


    public GeneratorParameters()
    {
    }
}
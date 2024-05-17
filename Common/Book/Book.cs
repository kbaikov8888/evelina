﻿namespace BookImpl;

public class Book
{
    public string Name { get; }

    private readonly List<Entry> _entries = new();


    public Book(string name)
    {
        Name = name;
    }


    public List<Entry> GetEntries()
    {
        return _entries.ToList();
    }

    public HashSet<Account> GetAccounts()
    {
        var accounts = new HashSet<Account>();
        foreach (var entry in _entries)
        {
            switch (entry)
            {
                case ExternalEntry externalEntry:
                    accounts.Add(externalEntry.Account);
                    break;
                case TransferEntry transferEntry:
                    accounts.Add(transferEntry.Sender);
                    accounts.Add(transferEntry.Receiver);
                    break;
            }
        }

        return accounts;
    }
}
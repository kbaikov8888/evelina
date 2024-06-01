using System;
using System.Collections.Generic;
using System.Linq;
using PortfolioImpl.DTO;
using PortfolioInterface;
using System.Text.Json;

namespace PortfolioImpl;

internal class Asset : Item, IAsset
{
    public string Name { get; set; } = string.Empty;

    private double? _targetVolume;
    public double? TargetVolume
    {
        get => _targetVolume;
        set
        {
            if (_targetVolume != value)
            {
                _targetVolume = value;
                _parent.UpdateStat();
            }
        }
    }

    private double? _targetSellPrice;
    public double? TargetSellPrice
    {
        get => _targetSellPrice;
        set
        {
            if (_targetSellPrice != value)
            {
                _targetSellPrice = value;
                _parent.UpdateStat();
            }
        }
    }

    private double? _targetShare;
    public double? TargetShare
    {
        get => _targetShare;
        set
        {
            if (_targetShare != value)
            {
                _targetShare = value;
                _parent.UpdateStat();
            }
        }
    }

    public IAssetStat Stat => _stat;

    private readonly List<Transaction> _transactions;
    private readonly List<Target> _targets;
    private readonly Portfolio _parent;
    private readonly AssetStat _stat;


    internal Asset(string id, long creationDate, string parentId, Portfolio parent) : base(id, creationDate, parentId)
    {
        Level = EItemLevel.Asset;
        _transactions = new List<Transaction>();
        _targets = new List<Target>();
        _parent = parent;
        _stat = new AssetStat();
    }


    #region IItem
    public override string ToJson()
    {
        var dto = new AssetDTO()
        {
            Name = Name,
            TargetVolume = TargetVolume,
            TargetSellPrice = TargetSellPrice,
            TargetShare = TargetShare,
        };

        return JsonSerializer.Serialize(dto);
    }

    public override void FromJson(string json)
    {
        var dto = JsonSerializer.Deserialize<AssetDTO>(json);
        if (dto is null) return;

        Name = dto.Name;
        TargetVolume = dto.TargetVolume;
        TargetSellPrice = dto.TargetSellPrice;
        TargetShare = dto.TargetShare;
    }
    #endregion

    public IList<ITransaction> GetTransactions()
    {
        var transactions = new List<ITransaction>();
        foreach (var transaction in _transactions)
        {
            transactions.Add(transaction);
        }
        return transactions;
    }

    public ITransaction CreateTransaction(long datetime, ETransaction type, double price, double amount)
    {
        var now = DateTime.Now.Ticks;
        var uid = Guid.NewGuid().ToString();

        var transaction = new Transaction(uid, now, Id, this)
        {
            Datetime = datetime,
            Type = type,
            Price = price,
            Amount = amount,
        };

        _transactions.Add(transaction);
        _parent.UpdateStat();

        return transaction;
    }

    public void DeleteTransaction(ITransaction transaction)
    {
        if (transaction is not Transaction real || !_transactions.Contains(real))
        {
            throw new InvalidOperationException();
        }

        _transactions.Remove(real);
        _parent.UpdateStat();
    }

    public IList<ITarget> GetTargets()
    {
        return _targets.Cast<ITarget>().ToList();
    }

    public ITarget CreateTarget(double price, double volume)
    {
        var now = DateTime.Now.Ticks;
        var uid = Guid.NewGuid().ToString();

        var target = new Target(uid, now, Id)
        {
            Volume = volume,
            Price = price,
        };

        _targets.Add(target);
        _parent.UpdateStat();

        return target;
    }

    public void DeleteTarget(ITarget target)
    {
        if (target is not Target real || !_targets.Contains(real))
        {
            throw new InvalidOperationException();
        }

        _targets.Remove(real);
        _parent.UpdateStat();
    }

    internal void AddTransaction(Transaction transaction)
    {
        if (transaction.ParentId != Id)
        {
            throw new InvalidOperationException();
        }

        _transactions.Add(transaction);
        _parent.UpdateStat();
    }

    internal void AddTarget(Target target)
    {
        if (target.ParentId != Id)
        {
            throw new InvalidOperationException();
        }

        _targets.Add(target);
        _parent.UpdateStat();
    }

    internal void UpdateStat()
    {
        _parent.UpdateStat();
    }
}
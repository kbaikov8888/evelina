using System;
using System.Collections.Generic;
using System.Linq;
using PortfolioImpl.Database;
using PortfolioInterface;

namespace PortfolioImpl;

public static class PortfolioFactory
{
    public static IPortfolio CreatePortfolio(string name)
    {
        var now = DateTime.Now.Ticks;
        var uid = Guid.NewGuid().ToString();

        var portfolio = new Portfolio(uid, now)
        {
            Name = name
        };

        return portfolio;
    }

    public static IPortfolio? ReadPortfolio(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentNullException(nameof(path));
        }

        try
        {
            using (var db = new PortfolioContext(path))
            {
                List<Thing> things = db.Things.ToList();
                var portfolio = ReadThings(things, path);

                return portfolio;
            }
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    private static Portfolio ReadThings(List<Thing> things, string path)
    {
        Thing? portfolioThing = null;
        List<Thing> assetThings = new();
        List<Thing> transactionThings = new();
        List<Thing> targetThings = new();

        foreach (var thing in things)
        {
            if (thing.Level == EItemLevel.Portfolio)
            {
                if (portfolioThing != null)
                {
                    throw new InvalidOperationException();
                }

                portfolioThing = thing;
            }
            else if (thing.Level == EItemLevel.Asset)
            {
                assetThings.Add(thing);
            }
            else if (thing.Level == EItemLevel.Transaction)
            {
                transactionThings.Add(thing);
            }
            else if (thing.Level == EItemLevel.Target)
            {
                targetThings.Add(thing);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        if (portfolioThing is null)
        {
            throw new InvalidOperationException();
        }

        var portfolio = new Portfolio(portfolioThing.Id, portfolioThing.CreationDate, path);
        portfolio.FromJson(portfolioThing.JsonValue);

        foreach (var thing in assetThings)
        {
            var asset = new Asset(thing.Id, thing.CreationDate, thing.ParentId, portfolio);
            asset.FromJson(thing.JsonValue);
            portfolio.AddAsset(asset);
        }

        var assets = portfolio.GetAssets();

        foreach (var thing in transactionThings)
        {
            var parent = assets.FirstOrDefault(x => x.Id == thing.ParentId) as Asset;
            if (parent is null)
            {
                throw new InvalidOperationException();//TODO
            }

            var transaction = new Transaction(thing.Id, thing.CreationDate, thing.ParentId, parent);
            transaction.FromJson(thing.JsonValue);

            parent.AddTransaction(transaction);
        }

        foreach (var thing in targetThings)
        {
            var parent = assets.FirstOrDefault(x => x.Id == thing.ParentId) as Asset;
            if (parent is null)
            {
                throw new InvalidOperationException();//TODO
            }

            var target = new Target(thing.Id, thing.CreationDate, thing.ParentId);
            target.FromJson(thing.JsonValue);

            parent.AddTarget(target);
        }

        return portfolio;
    }

    internal static bool SavePortfolio(IPortfolio portfolio, string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentNullException(nameof(path));
        }

        try
        {
            using (var db = new PortfolioContext(path))
            {
                Dictionary<string, Thing> oldThings = db.Things.ToDictionary(x => x.Id, x => x);
                Dictionary<string, Thing> newThings = new();

                var thisThing = new Thing(portfolio);
                newThings[thisThing.Id] = thisThing;

                foreach (var asset in portfolio.GetAssets())
                {
                    var thing = new Thing(asset);
                    newThings[thing.Id] = thing;

                    foreach (var transaction in asset.GetTransactions())
                    {
                        var thingTr = new Thing(transaction);
                        newThings[thingTr.Id] = thingTr;
                    }

                    foreach (var target in asset.GetTargets())
                    {
                        var thingTr = new Thing(target);
                        newThings[thingTr.Id] = thingTr;
                    }
                }

                foreach (var newThing in newThings.Values)
                {
                    if (oldThings.TryGetValue(newThing.Id, out var old))
                    {
                        old.JsonValue = newThing.JsonValue;
                        db.Things.Update(old);
                    }
                    else
                    {
                        db.Things.Add(newThing);
                    }
                }

                foreach (var oldThing in oldThings.Values)
                {
                    if (!newThings.TryGetValue(oldThing.Id, out _))
                    {
                        db.Things.Remove(oldThing);
                    }
                }

                db.SaveChanges();
            }
        }
        catch
        {
            return false;
        }

        return true;
    }
}
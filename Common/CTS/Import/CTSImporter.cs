﻿using Microsoft.VisualBasic.FileIO;
using PortfolioInterface;

namespace CTS.Import;

public class CTSImporter : IDisposable
{
    private readonly string _path;
    private readonly List<CTS> _transactions = new();


    public CTSImporter(string path)
    {
        _path = path;
    }


    public void Read()
    {
        Dictionary<ECTSFields, int> columnIndexes = new();

        using (var parser = new TextFieldParser(_path))
        {
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            while (!parser.EndOfData)
            {
                //Processing row
                var fields = parser.ReadFields();
                if (fields is null)
                {
                    throw new Exception("fields is null");
                }

                if (string.IsNullOrEmpty(fields[0]))
                {
                    // this means header row
                    for (var i = 0; i < fields.Length; i++)
                    {
                        if (Enum.TryParse<ECTSFields>(fields[i], out var res))
                        {
                            if (columnIndexes.ContainsKey(res))
                            {
                                throw new Exception("wrong header");
                            }

                            columnIndexes[res] = i;
                        }
                    }

                    CheckAllRequiredExist(columnIndexes.Keys);

                    continue;
                }

                var cts = new CTS();

                foreach (var field in columnIndexes.Keys)
                {
                    var i = columnIndexes[field];
                    cts.SetField(field, fields[i]);
                }

                _transactions.Add(cts);
            }
        }
    }

    public void Dispose()
    {
        _transactions.Clear();
    }

    public void AddToPortfolio(IPortfolio portfolio, bool createNewAssets = true)
    {
        foreach (var cts in _transactions)
        {
            var asset = portfolio.GetAsset(cts.Symbol);
            if (asset is null)
            {
                if (!createNewAssets)
                {
                    continue;
                }

                asset = portfolio.CreateAsset(cts.Symbol);
            }

            var type = cts.Type == EType.buy ? ETransaction.Buy : ETransaction.Sell;

            asset.CreateTransaction(cts.Datetime, type, cts.Price, cts.Amount);
        }
    }

    private void CheckAllRequiredExist(IEnumerable<ECTSFields> source)
    {
        foreach (var field in (ECTSFields[])Enum.GetValues(typeof(ECTSFields)))
        {
            if (field.IsRequired() && !source.Contains(field))
            {
                throw new Exception($"missed required column: {field}");
            }
        }
    }
}
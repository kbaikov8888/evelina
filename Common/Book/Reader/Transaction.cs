using System;
using System.Globalization;
using BookImpl.Elements;
using BookImpl.Enum;

namespace BookImpl.Reader;

internal class Transaction
{
    public TransactionType? Type { get; set; }
    public Currency? Currency { get; set; }
    public double? Amount { get; set; }
    public double? CurrencyRate { get; set; }
    public string? Project { get; set; }
    public string? Category { get; set; }
    public string? ParentCategory { get; set; }
    public string? Account { get; set; }
    public string? PaymentAccount { get; set; }
    public string? ReceivableAccount { get; set; }
    public string? Merchant { get; set; }
    public string? Address { get; set; }
    public DateTime? Datetime { get; set; }
    public string? Tags { get; set; }
    public string? Author { get; set; }
    public string? Note { get; set; }

    internal bool IsValid()
    {
        if (Type is null ||
            Amount is null ||
            Currency is null ||
            CurrencyRate is null ||
            Category is null ||
            (Account is null && (PaymentAccount is null || ReceivableAccount is null)) ||
            Datetime is null)
        {
            return false;
        }

        return true;
    }

    internal void SetField(FieldRole role, string s)
    {
        double? d = null;
        double.TryParse(s, out var dd);
        if (!double.IsNaN(dd))
        {
            d = dd;
        }

        switch (role)
        {
            case FieldRole.type:
                Type = SproutsHelper.FindType(s); 
                break;
            case FieldRole.currency:
                if (System.Enum.TryParse(s, out Currency currency))
                {
                    Currency = currency;
                }
                break;
            case FieldRole.amount:
                Amount = d; break;
            case FieldRole.currencyRate:
                CurrencyRate = d; break;
            case FieldRole.project:
                Project = s; break;
            case FieldRole.category:
                Category = s; break;
            case FieldRole.parentCategory:
                ParentCategory = s; break;
            case FieldRole.account:
                Account = s; break;
            case FieldRole.paymentAccount:
                PaymentAccount = s; break;
            case FieldRole.receivableAccount:
                ReceivableAccount = s; break;
            case FieldRole.merchant:
                Merchant = s; break;
            case FieldRole.address:
                Address = s; break;
            case FieldRole.datetime:
                //if (DateTime.TryParseExact(s, "yyyy-MM-dd HH:nn", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                if (DateTime.TryParse(s, CultureInfo.InvariantCulture, out var dt))
                {
                    Datetime = dt;
                }
                break;
            case FieldRole.tags:
                Tags = s; break;
            case FieldRole.author:
                Author = s; break;
            case FieldRole.note:
                Note = s; break;
            default:
                throw new NotImplementedException(nameof(role));
        }
    }

    internal void CreateEntry(Book book)
    {
        var currencyRate = CurrencyRate ?? throw new InvalidOperationException();
        var currency = Currency ?? throw new InvalidOperationException();
        var amount = Amount ?? throw new InvalidOperationException();
        var dateTime = Datetime ?? throw new InvalidOperationException();
        var project = Project != null ? book.GetOrCreateProject(Project) : null;

        Entry entry;

        if (Type is TransactionType.expense or TransactionType.income)
        {
            var account = Account ?? throw new InvalidOperationException();
            var category = Category ?? throw new InvalidOperationException();

            var bankAccount = book.GetOrCreateBankAccount(account);

            if (Type is TransactionType.expense)
            {
                var expenseCategory = book.GetOrCreateExpenseCategory(category, ParentCategory);

                entry = new ExpenseEntry(-amount, dateTime, bankAccount, expenseCategory, currencyRate);
            }
            else
            {
                var incomeCategory = book.GetOrCreateIncomeCategory(category, ParentCategory);

                entry = new IncomeEntry(amount, dateTime, bankAccount, incomeCategory, currencyRate);
            }
        }
        else if (Type == TransactionType.transfer)
        {
            var sender = PaymentAccount ?? throw new InvalidOperationException();
            var receiver = ReceivableAccount ?? throw new InvalidOperationException();

            var senderInvest = sender.StartsWith("_i");
            var receiverInvest = receiver.StartsWith("_i");

            Account senderAccount = senderInvest ? book.GetOrCreateInvestAccount(sender) : book.GetOrCreateBankAccount(sender);
            Account receiverAccount = receiverInvest ? book.GetOrCreateInvestAccount(receiver) : book.GetOrCreateBankAccount(receiver);

            // bruh
            double senderCurrencyRate;
            double receiverCurrencyRate;
            double senderAmount;
            double receiverAmount;
            if (currency != Book.DefaultCurrency)
            {
                if (senderAccount.Currency == receiverAccount.Currency)
                {
                    senderAmount = amount;
                    receiverAmount = amount;
                    senderCurrencyRate = currencyRate;
                    receiverCurrencyRate = currencyRate;
                }
                else if (senderAccount.Currency == currency)
                {
                    senderAmount = amount;
                    senderCurrencyRate = currencyRate;

                    if (receiverAccount.Currency == Book.DefaultCurrency)
                    {
                        receiverCurrencyRate = 1;
                    }
                    else
                    {
                        receiverCurrencyRate = FindLastCurrencyRateForAccount(receiverAccount);
                    }

                    receiverAmount = amount * senderCurrencyRate / receiverCurrencyRate;
                }
                else // receiverAccount.Currency == currency
                {
                    receiverAmount = amount;
                    receiverCurrencyRate = currencyRate;

                    if (senderAccount.Currency == Book.DefaultCurrency)
                    {
                        senderCurrencyRate = 1;
                    }
                    else
                    {
                        senderCurrencyRate = FindLastCurrencyRateForAccount(senderAccount);
                    }

                    senderAmount = amount * receiverCurrencyRate / senderCurrencyRate;
                }
            }
            else
            {
                senderAmount = amount;
                receiverAmount = amount;
                senderCurrencyRate = 1;
                receiverCurrencyRate = 1;
            }

            if (senderInvest == receiverInvest)
            {
                entry = new TransferEntry(dateTime, senderAmount, receiverAmount, senderAccount, receiverAccount, senderCurrencyRate, receiverCurrencyRate);
            }
            else if (senderInvest)
            {
                entry = new ReInvestingEntry(dateTime, senderAmount, receiverAmount, (InvestAccount)senderAccount, receiverAccount, senderCurrencyRate, receiverCurrencyRate);
            }
            else
            {
                entry = new InvestingEntry(dateTime, senderAmount, receiverAmount, senderAccount, (InvestAccount)receiverAccount, senderCurrencyRate, receiverCurrencyRate);
            }
        }
        else
        {
            throw new InvalidOperationException();
        }


        entry.Project = project;
        entry.Note = Note;

        book.AddEntry(entry);
    }

    // very bad, but sprouts make stupid csv...
    private double FindLastCurrencyRateForAccount(Account account)
    {
        //TODO
        return 1;
    }
}

internal enum FieldRole
{
    type,
    currency,
    amount,
    currencyRate,
    project,
    category,
    parentCategory,
    account,
    paymentAccount,
    receivableAccount,
    merchant,
    address,
    datetime,
    tags,
    author,
    note,
}

internal enum TransactionType
{
    expense,
    income,
    transfer,
}

//internal static class FieldRoleExtension
//{
//    public static bool IsRequired(this FieldRole field)
//        => field switch
//        {
//            FieldRole.type or FieldRole.amount or FieldRole.category or
//                FieldRole.account or FieldRole.paymentAccount or FieldRole.receivableAccount => true,
//            _ => false,
//        };
//}
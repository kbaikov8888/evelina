using System;
using System.Globalization;
using BookImpl.Elements;

namespace BookImpl.Reader;

internal class Transaction
{
    public TransactionType? Type { get; set; }
    public string? Currency { get; set; }
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
                Type = SproutsHelper.FindType(s); break;
            case FieldRole.currency:
                Currency = s; break;
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
        var currencyRate = CurrencyRate ?? 1;
        var amount = Amount ?? throw new InvalidOperationException();
        amount *= currencyRate;
        var dateTime = Datetime ?? throw new InvalidOperationException();

        Entry entry;

        if (Type is TransactionType.expense or TransactionType.income)
        {
            var account = Account ?? throw new InvalidOperationException();
            var category = Category ?? throw new InvalidOperationException();

            var project = Project != null ? book.GetOrCreateProject(Project) : null;

            var bankAccount = book.GetOrCreateBankAccount(account);

            if (Type is TransactionType.expense)
            {
                var expenseCategory = book.GetOrCreateExpenseCategory(category, ParentCategory);

                amount *= -1;

                entry = new ExpenseEntry(amount, dateTime, bankAccount, expenseCategory);
            }
            else
            {
                var incomeCategory = book.GetOrCreateIncomeCategory(category, ParentCategory);

                entry = new IncomeEntry(amount, dateTime, bankAccount, incomeCategory);
            }

            entry.Project = project;
            entry.Note = Note;
        }
        else if (Type == TransactionType.transfer)
        {
            var sender = ParentCategory ?? throw new InvalidOperationException();
            var receiver = ReceivableAccount ?? throw new InvalidOperationException();

            var senderInvest = sender.StartsWith("_i");
            var receiverInvest = receiver.StartsWith("_i");

            Account senderAccount = senderInvest ? book.GetOrCreateInvestAccount(sender) : book.GetOrCreateBankAccount(sender);
            Account receiverAccount = receiverInvest ? book.GetOrCreateInvestAccount(receiver) : book.GetOrCreateBankAccount(receiver);

            if (senderInvest == receiverInvest)
            {
                entry = new TransferEntry(amount, dateTime, senderAccount, receiverAccount);
            }
            else if (senderInvest)
            {
                entry = new ReInvestingEntry(amount, dateTime, (InvestAccount)senderAccount, receiverAccount);
            }
            else
            {
                entry = new InvestingEntry(amount, dateTime, senderAccount, (InvestAccount)receiverAccount);
            }
        }
        else
        {
            throw new InvalidOperationException();
        }

        book.AddEntry(entry);
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
using Domain.Util;
using System;
using System.Transactions;

namespace Domain.Repository
{
    public interface ICommonRepository
    {
        int? CommandTimeout { get; set; }
        CustomProfiledDbConnection Connection { get; }

        TransactionScope AsyncTransactionScope(TimeSpan? timeSpan = null);
        TransactionScope AsyncTransactionScope(Transaction transactionToUse);
        TransactionScope AsyncTransactionScope(Transaction transactionToUse, TimeSpan scopeTimeout);
        TransactionScope AsyncTransactionScope(TransactionScopeOption scopeOption);
        TransactionScope AsyncTransactionScope(TransactionScopeOption scopeOption, TimeSpan scopeTimeout);
        TransactionScope AsyncTransactionScope(TransactionScopeOption scopeOption, TransactionOptions transactionOptions);
        void EnlistTransaction();
    }
}
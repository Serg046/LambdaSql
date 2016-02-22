using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace SqlSelectBuilder
{
    [ContractClass(typeof(ISqlFieldContract))]
    public interface ISqlField
    {
        ISqlAlias Alias { get; set; }
        string Name { get; set; }
        string AsAlias { get; set; }
        string ShortView { get; }
        string View { get; }
        FieldAggregation? Aggregation { get; set; }
    }

    [ContractClassFor(typeof(ISqlField))]
    [ExcludeFromCodeCoverage]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal abstract class ISqlFieldContract : ISqlField
    {
        public FieldAggregation? Aggregation { get; set; }

        public ISqlAlias Alias
        {
            get
            {
                Contract.Ensures(Contract.Result<ISqlAlias>() != null);
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string AsAlias { get; set; }

        public string Name
        {
            get
            {
                Contract.Ensures(Contract.Result<string>().IsNotEmpty());
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public string ShortView
        {
            get
            {
                Contract.Ensures(Contract.Result<string>().IsNotEmpty());
                throw new NotImplementedException();
            }
        }

        public string View
        {
            get
            {
                Contract.Ensures(Contract.Result<string>().IsNotEmpty());
                throw new NotImplementedException();
            }
        }
    }
}

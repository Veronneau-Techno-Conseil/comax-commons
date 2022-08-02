using System;
using System.Collections.Generic;
using System.Text;

namespace Comax.Commons.Shared.RulesEngine
{
    public class RuleField<T>
    {

        public Func<T, bool> Check { get; set; }

        public bool DoCheck(object o)
        {
            if (o.GetType() != typeof(T))
            {
                throw new InvalidCastException($"Parameter o should be of type ${typeof(T)}");
            }

            if (this.Check == null)
            {
                throw new NullReferenceException("Check method has not been set.");
            }

            return this.Check((T)o);
        }

        public bool Mandatory { get; set; }
        public bool Ignore { get; set; }

    }

    public interface IConfigField
    {
        bool DoCheck(object o);
        bool Mandatory { get; set; }
        bool Ignore { get; set; }
    }

    public class RulesRow
    {
        public string Executor { get; set; }
        public IConfigField[] ConfigFields { get; set; }
    }

    public interface IExecutor
    {
        Task Execute(params object[] args);
    }
}

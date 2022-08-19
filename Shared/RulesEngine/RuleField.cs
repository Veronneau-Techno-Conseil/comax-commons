using Orleans.Streams;

namespace CommunAxiom.Commons.Shared.RuleEngine
{
    public class RuleField<T>: IConfigField
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

    public class RulesRow<TParam>
    {
        public IExecutor<TParam> Executor { get; set; }
        public IConfigField[] ConfigFields { get; set; }
    }

    public interface IExecutor<TParam>
    {
        Task Execute(TParam param);
    }
}

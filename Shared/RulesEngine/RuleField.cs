using Orleans.Streams;

namespace CommunAxiom.Commons.Shared.RuleEngine
{
    public class RuleField<T>: IConfigField
    {

        public Func<T, Task<bool>> CheckAsync { get; set; }
        public Func<T, bool> Check { get; set; }

        public async Task<bool> DoCheck(object o)
        {
            if (o.GetType() != typeof(T))
            {
                throw new InvalidCastException($"Parameter o should be of type ${typeof(T)}");
            }

            if (this.Check == null && this.CheckAsync == null)
            {
                throw new NullReferenceException("Check method has not been set.");
            }

            if(this.Check != null)
                return this.Check((T)o);
            else
                return await this.CheckAsync((T)o);
        }

        public bool Mandatory { get; set; }
        public bool Ignore { get; set; }

    }

    public interface IConfigField
    {
        Task<bool> DoCheck(object o);
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

using CommunAxiom.Commons.Shared;
using Neleus.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Comax.Commons.Shared.RulesEngine
{
    public class RulesEngine
    {
        protected readonly IServiceProvider _serviceProvider;
        public RulesEngine(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private List<RulesRow> _rulesTable;
        public IReadOnlyList<RulesRow> RulesTable
        {
            get
            {
                return _rulesTable.AsReadOnly();
            }
        }

        public int FieldCount { get; set; }

        public void AddRule(string executor, params IConfigField[] configFields)
        {
            if (configFields.Length != FieldCount)
                throw new ArgumentException("ConfigFields need to have the same number of items as configured in the RulesEngine");

            //TODO: type check each fields against the first row

            _rulesTable.Add(new RulesRow { Executor = executor, ConfigFields = configFields });
        }

        public virtual OperationResult Validate(params object[] values)
        {
            if(values == null || values.Length == 0)
                return new OperationResult {  Error = OperationResult.ERR_UNEXP_NULL, Detail = "values", IsError = true};
            if (values.Length != FieldCount)
                return new OperationResult { Error = OperationResult.PARAM_ERR, Detail = $"Invalid values count {values.Length}, expected: {FieldCount}", IsError = true};


            return new OperationResult { IsError = false };
        }

        public async Task Process(params object[] values)
        {
            var operationResult = this.Validate(values);
            if (operationResult.IsError)
            {
                throw new Exception(Newtonsoft.Json.JsonConvert.SerializeObject(operationResult));
            }

            foreach (var rule in _rulesTable)
            {
                bool shouldExecute = true;
                for (int ix = 0; ix < rule.ConfigFields.Length; ix++)
                {
                    var field = rule.ConfigFields[ix];
                    if (field.Ignore)
                        continue;

                    if (values[ix] == null && field.Mandatory)
                    {
                        shouldExecute = false;
                        break;
                    }

                    shouldExecute &= field.DoCheck(values[ix]);
                }
                if (shouldExecute)
                {
                    var executor = this._serviceProvider.GetServiceByName<IExecutor>(rule.Executor);
                    await executor.Execute(values);
                }
            }
        }
    }
}

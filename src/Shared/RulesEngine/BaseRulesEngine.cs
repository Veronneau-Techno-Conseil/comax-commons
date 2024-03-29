﻿namespace CommunAxiom.Commons.Shared.RuleEngine
{
    public abstract class RuleEngine<TParam>
    {
        private List<RulesRow<TParam>> _rulesTable;
        public IReadOnlyList<RulesRow<TParam>> RulesTable
        {
            get
            {
                return _rulesTable.AsReadOnly();
            }
        }

        public int FieldCount { get; set; }

        public void AddRule(IExecutor<TParam> executor, params IConfigField[] configFields)
        {
            if (configFields.Length != FieldCount && _rulesTable != null && _rulesTable.Count > 0)
                throw new ArgumentException("ConfigFields need to have the same number of items as configured in the RulesEngine");

            if (_rulesTable == null)
            {
                _rulesTable = new List<RulesRow<TParam>>();
                FieldCount = configFields.Length;
            }

            //TODO: type check each fields against the first row

            _rulesTable.Add(new RulesRow<TParam> { Executor = executor, ConfigFields = configFields });
        }

        public OperationResult Validate(TParam param)
        {
            var objs = this.ExtractValues(param);
            return this.Validate(objs);
        }

        protected virtual OperationResult Validate(params object[] values)
        {
            if(values == null || values.Length == 0)
                return new OperationResult {  Error = OperationResult.ERR_UNEXP_NULL, Detail = "values", IsError = true};
            if (values.Length != FieldCount)
                return new OperationResult { Error = OperationResult.PARAM_ERR, Detail = $"Invalid values count {values.Length}, expected: {FieldCount}", IsError = true};


            return new OperationResult { IsError = false };
        }

        /// <summary>
        /// Extracts data required by the rules table of the busienss rules engine. Should return the same number of items as the number of configFields for executors
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public abstract object[] ExtractValues(TParam param);

        public async Task Process(TParam param)
        {
            if (param == null)
                return;

            var values = this.ExtractValues(param);

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

                    shouldExecute &= await field.DoCheck(values[ix]);
                }
                if (shouldExecute)
                {
                    await rule.Executor.Execute(param);
                }
            }
        }
    }
}

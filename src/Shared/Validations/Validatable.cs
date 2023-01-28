using CommunAxiom.Commons.Shared.RuleEngine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Text;

namespace CommunAxiom.Commons.Shared.Validations
{
    public class Validatable<TObject>
    {
        TObject _toValidate;
        OperationResult _operationResult = new OperationResult();
        List<VObj> _validationList = new List<VObj>();

        public Validatable(TObject o)
        {
            _toValidate = o;
        }

        public Validatable<TObject> Validate(Func<TObject, bool> predicate, string errorCode, string message = null)
        {
            _validationList.Add(new VObj { Predicate = predicate, ErrorCode = errorCode, Message = message });
            return this;
        }

        public Validatable<TObject> NotNull(Expression<Func<TObject, string>> member, string message = null)
        {
            var proc = member.Compile();
            var name = GetMemberInfo(member).Member.Name;
            _validationList.Add(new VObj { Predicate = o => !string.IsNullOrWhiteSpace(proc(o)), ErrorCode = OperationResult.ERR_UNEXP_NULL, 
                Message = $"Field {name}{(string.IsNullOrWhiteSpace(message) ? "" : " - " + message)}" });
            return this;
        }

        public OperationResult ExecIfValid(Action<TObject> action)
        {
            foreach (var p in _validationList)
            {
                if (!WithTry(p))
                    return _operationResult;
            }
            WithTry(action);
            return _operationResult;
        }

        public OperationResult<TReturn> ExecIfValid<TReturn>(Func<TObject, TReturn> action)
        {
            foreach (var p in _validationList)
            {
                if (!WithTry(p))
                    return new OperationResult<TReturn>(_operationResult);
            }
            WithTry(action, out var res);
            return new OperationResult<TReturn>(_operationResult) { Result = res };
        }

        public async Task<OperationResult> ExecIfValid(Func<TObject, Task> action)
        {
            foreach (var p in _validationList)
            {
                if (!WithTry(p))
                    return _operationResult;
            }
            await WithTry(action);
            return _operationResult;
        }

        public async Task<OperationResult<TReturn>> ExecIfValid<TReturn>(Func<TObject, Task<TReturn>> action)
        {
            foreach (var p in _validationList)
            {
                if (!WithTry(p))
                    return new OperationResult<TReturn>(_operationResult);
            }
            var (success, res) = await WithTry(action);

            return new OperationResult<TReturn>(_operationResult) { Result = res };
        }

        async Task<(bool, TRes)> WithTry<TRes>(Func<TObject, Task<TRes>> o)
        {
            try
            {
                TRes res = await o(_toValidate);
                return (true, res);
            }
            catch (Exception ex)
            {
                _operationResult.IsError = true;
                _operationResult.Error = OperationResult.ERR_UNEXP_ERR;
                _operationResult.Detail = ex.Message + ex.StackTrace;
                return (false, default);
            }
        }

        async Task<bool> WithTry(Func<TObject, Task> o)
        {
            try
            {
                await o(_toValidate);
                return true;
            }
            catch (Exception ex)
            {
                _operationResult.IsError = true;
                _operationResult.Error = OperationResult.ERR_UNEXP_ERR;
                _operationResult.Detail = ex.Message + ex.StackTrace;
                return false;
            }
        }

        bool WithTry<TRes>(Func<TObject, TRes> o, out TRes res)
        {
            try
            {
                res = o(_toValidate);
                return true;
            }
            catch (Exception ex)
            {
                res = default;
                _operationResult.IsError = true;
                _operationResult.Error = OperationResult.ERR_UNEXP_ERR;
                _operationResult.Detail = ex.Message + ex.StackTrace;
                return false;
            }
        }

        bool WithTry(Action<TObject> o)
        {
            try
            {
                o(_toValidate);
                return true;
            }
            catch (Exception ex)
            {
                _operationResult.IsError = true;
                _operationResult.Error = OperationResult.ERR_UNEXP_ERR;
                _operationResult.Detail = ex.Message + ex.StackTrace;
                return false;
            }
        }

        bool WithTry(VObj o)
        {
            try
            {
                if (o.Predicate(_toValidate))
                {
                    return true;
                }
                else
                {
                    _operationResult.Error = o.ErrorCode;
                    _operationResult.IsError = true;
                    _operationResult.Detail = o.Message;
                    return false;
                }
            }
            catch (Exception ex)
            {
                _operationResult.IsError = true;
                _operationResult.Error = OperationResult.ERR_UNEXP_ERR;
                _operationResult.Detail = ex.Message + ex.StackTrace;
                return false;
            }
        }
        private static MemberExpression GetMemberInfo(Expression method)
        {
            LambdaExpression lambda = method as LambdaExpression;
            if (lambda == null)
                throw new ArgumentNullException("method");

            MemberExpression memberExpr = null;

            if (lambda.Body.NodeType == ExpressionType.Convert)
            {
                memberExpr =
                    ((UnaryExpression)lambda.Body).Operand as MemberExpression;
            }
            else if (lambda.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpr = lambda.Body as MemberExpression;
            }

            if (memberExpr == null)
                throw new ArgumentException("method");

            return memberExpr;
        }

        private class VObj
        {
            public Func<TObject, bool> Predicate { get; set; }
            public string ErrorCode
            {
                get; set;
            }
            public string Message { get; set; }
        }
    }
}

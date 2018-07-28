﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Prototypist.FunctionGraph
{
    public interface IContainer
    {
        T Get<T>();
    }
        
    public partial class FlowBuilder : IFlowBuilder
    {
        public MethodInfo Parallel { get; set; } = null;

        private readonly Dictionary<Type, Delegate> sources = new Dictionary<Type, Delegate>();
        private readonly Dictionary<Type, Expression> constants = new Dictionary<Type, Expression>();
        private Func<Type, Expression> fallbackSource;
        private readonly List<WorkItem> todo = new List<WorkItem>();

        public FlowBuilder()
        {
        }

        #region Configure

        public void SetContainer(IContainer container)
        {
            var containerParameter = Expression.Constant(container);
            var get = typeof(IContainer).GetMethods().Where(x => x.Name == "Get" && x.IsGenericMethod).Single();
            fallbackSource = (type) => Expression.Call(containerParameter, get.MakeGenericMethod(type));
        }

        public void SetConstant<T>(T t)
        {
            var key = typeof(T);
            if (sources.ContainsKey(key)) {
                throw new Exception($"Source already set for {key}");
            }
            constants.Add(key, Expression.Constant(t));
        }

        // you can make sources loop pretty easy
        // A -> B
        // B -> A
        // or just
        // A -> A
        // is it worth reflecting around and blocking loops?
        // probably atleast worth throwing in BuildExpression
        public void SetSource<T>(T t)
            where T : Delegate
        {
            var key = typeof(T).FuncReturnOrNull();
            if (constants.ContainsKey(key))
            {
                throw new Exception($"Constant already set for {key}");
            }
            sources.Add(key, t);
        }

        public void AddStep(Delegate expression)
        {
            var returnType = expression.GetType().FuncReturnOrNull();
            if (returnType == null)
            {
                todo.Add(new WorkItem(expression, new Type[0]));
            }
            else {
                todo.Add(new WorkItem(expression, new[] { returnType }));
            }
        }

        public void AddStepPacked<T1, T2>(Delegate expression)
        {
            todo.Add(new WorkItem(expression, new[] { typeof(T1),typeof(T2)}));
        }

        public void AddStepPacked<T1, T2, T3>(Delegate expression)
        {
            todo.Add(new WorkItem(expression, new[] { typeof(T1), typeof(T2), typeof(T3)}));
        }

        public void AddStepPacked<T1, T2, T3, T4>(Delegate expression)
        {
            todo.Add(new WorkItem(expression, new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) }));
        }

        public void AddStepPacked<T1, T2, T3, T4, T5>(Delegate expression)
        {
            todo.Add(new WorkItem(expression, new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) }));
        }

        public void AddStepPacked<T1, T2, T3, T4, T5, T6>(Delegate expression)
        {
            todo.Add(new WorkItem(expression, new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) }));
        }

        public void AddStepPacked<T1, T2, T3, T4, T5, T6, T7>(Delegate expression)
        {
            todo.Add(new WorkItem(expression, new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7)}));
        }

        #endregion

        #region Build/Run

        public void Run(params object[] inputs)
        {
            var parameters = inputs.Select(x => Expression.Parameter(x.GetType())).ToArray();

            BuildExpression(
                FromParams(parameters, FromConstants(FromSources(FromContainer()))),
                Parallel,
                new Type[0],
                NoReturn(),
                parameters,
                todo).Compile().DynamicInvoke(inputs);
        }

        public T Run<T>(params object[] inputs)
        {
            var parameters = inputs.Select(x => Expression.Parameter(x.GetType())).ToArray();

            return (T)(BuildExpression(
                FromParams(parameters, FromConstants(FromSources(FromContainer()))),
                Parallel,
                new[] { typeof(T) },
                SingleReturn(typeof(T)),
                parameters,
                todo).Compile().DynamicInvoke(inputs));
        }

        public (T1, T2) Run<T1, T2>(params object[] inputs)
        {
            var parameters = inputs.Select(x => Expression.Parameter(x.GetType())).ToArray();

            return (ValueTuple<T1, T2>)(BuildExpression(
                FromParams(parameters, FromConstants(FromSources(FromContainer()))),
                Parallel,
                new[] { typeof(T1), typeof(T2) },
                MultiReturn(typeof(ValueTuple<T1, T2>)),
                parameters,
                todo).Compile().DynamicInvoke(inputs));
        }

        public (T1, T2, T3) Run<T1, T2, T3>(params object[] inputs)
        {
            var parameters = inputs.Select(x => Expression.Parameter(x.GetType())).ToArray();

            return (ValueTuple<T1, T2, T3>)(BuildExpression(
                FromParams(parameters, FromConstants(FromSources(FromContainer()))),
                Parallel,
                new[] { typeof(T1), typeof(T2), typeof(T3) },
                MultiReturn(typeof(ValueTuple<T1, T2, T3>)),
                parameters,
                todo).Compile().DynamicInvoke(inputs));
        }

        public (T1, T2, T3, T4) Run<T1, T2, T3, T4>(params object[] inputs)
        {
            var parameters = inputs.Select(x => Expression.Parameter(x.GetType())).ToArray();

            return (ValueTuple<T1, T2, T3, T4>)(BuildExpression(
                FromParams(parameters, FromConstants(FromSources(FromContainer()))),
                Parallel,
                new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) },
                MultiReturn(typeof(ValueTuple<T1, T2, T3, T4>)),
                parameters,
                todo).Compile().DynamicInvoke(inputs));
        }

        public (T1, T2, T3, T4, T5) Run<T1, T2, T3, T4, T5>(params object[] inputs)
        {
            var parameters = inputs.Select(x => Expression.Parameter(x.GetType())).ToArray();

            return (ValueTuple<T1, T2, T3, T4, T5>)(BuildExpression(
                FromParams(parameters, FromConstants(FromSources(FromContainer()))),
                Parallel,
                new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) },
                MultiReturn(typeof(ValueTuple<T1, T2, T3, T4, T5>)),
                parameters,
                todo).Compile().DynamicInvoke(inputs));
        }

        public (T1, T2, T3, T4, T5, T6) Run<T1, T2, T3, T4, T5, T6>(params object[] inputs)
        {
            var parameters = inputs.Select(x => Expression.Parameter(x.GetType())).ToArray();

            return (ValueTuple<T1, T2, T3, T4, T5, T6>)(BuildExpression(
                FromParams(parameters, FromConstants(FromSources(FromContainer()))),
                Parallel,
                new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) },
                MultiReturn(typeof(ValueTuple<T1, T2, T3, T4, T5, T6>)),
                parameters,
                todo).Compile().DynamicInvoke(inputs));
        }

        public (T1, T2, T3, T4, T5, T6, T7) Run<T1, T2, T3, T4, T5, T6, T7>(params object[] inputs)
        {
            var parameters = inputs.Select(x => Expression.Parameter(x.GetType())).ToArray();

            return (ValueTuple<T1, T2, T3, T4, T5, T6, T7>)(BuildExpression(
                FromParams(parameters, FromConstants(FromSources(FromContainer()))),
                Parallel,
                new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7) },
                MultiReturn(typeof(ValueTuple<T1, T2, T3, T4, T5, T6, T7>)),
                parameters,
                todo).Compile().DynamicInvoke(inputs));
        }

        public T Build<T>() where T : Delegate
        {
            var parameters = typeof(T).Parameters().Select(x => Expression.Parameter(x)).ToArray();

            return (T)BuildExpression(
                FromParams(parameters, FromConstants(FromSources(FromContainer()))),
                Parallel,
                GetReturns(typeof(T)),
                typeof(T).FuncReturnOrNull() != null ? SingleReturn(typeof(T).FuncReturnOrNull()) : NoReturn(),
                parameters,
                todo).Compile();
        }

        #endregion

        #region Build/Run Help

        private Func<Type, Func<Type, Expression>, Expression> FromParams(ParameterExpression[] parameters, Func<Type, Func<Type, Expression>, Expression> fallback)
        {
            return (type, searchCurrentContext) =>
            {
                var res = parameters.SingleOrDefault(x => x.Type == type);
                if (res == null)
                {
                    return fallback(type, searchCurrentContext);
                }
                return res;
            };
        }

        private Func<Type, Func<Type, Expression>, Expression> FromConstants(Func<Type, Func<Type, Expression>, Expression> fallback)
        {
            return (type, searchCurrent) =>
            {
                if (constants.TryGetValue(type, out var cons))
                {
                    return cons;
                }
                return fallback(type, searchCurrent);
            };
        }

        private Func<Type, Func<Type, Expression>, Expression> FromSources(Func<Type, Func<Type, Expression>, Expression> fallback)
        {
            return (type, searchCurrent) =>
            {
                if (sources.TryGetValue(type, out var source))
                {
                    var parms = source.GetType().Parameters().Select(x => searchCurrent(x)).ToArray();
                    var toCall = Expression.Constant(source);
                    return Expression.Invoke(toCall, parms);
                }
                return fallback(type, searchCurrent);
            };
        }

        private Func<Type, Func<Type, Expression>, Expression> FromContainer()
        {
            return (type, searchCurrent) =>
            {
                if (fallbackSource == null)
                {
                    throw new Exception("Parameter not found");
                }

                return fallbackSource(type);
            };
        }

        private static Type[] GetReturns(Type type)
        {

            var returnType = type.FuncReturnOrNull();

            if (returnType == null)
            {
                return new Type[0];
            }
            return new Type[] { returnType };
        }

        private static Func<Expression[], Expression[]> NoReturn()
        {
            return x => new Expression[0];
        }

        private static Func<Expression[], Expression[]> SingleReturn(Type type)
        {
            return x =>
            {
                var res = new List<Expression>();
                var returnTarget = Expression.Label(type);
                res.Add(Expression.Return(returnTarget, x.Single()));
                res.Add(Expression.Label(returnTarget, Expression.Convert(Expression.Constant(GetDefault(type)), type)));
                return res.ToArray();
            };
        }

        private Func<Expression[], Expression[]> MultiReturn(Type type)
        {
            return x =>
            {
                var res = new List<Expression>();
                var returnTarget = Expression.Label(type);
                var constructor = type.GetConstructor(type.GetGenericArguments());
                res.Add(Expression.Return(returnTarget, Expression.New(constructor, x)));
                res.Add(Expression.Label(returnTarget, Expression.Constant(GetDefault(type))));
                return res.ToArray();
            };
        }

        private static Func<Type, Expression> FromInputs(object[] inputs)
        {

            var parameters = inputs.Select(x => Expression.Constant(x, x.GetType())).ToArray();

            return type => parameters.Single(x => x.Type == type);
        }

        private static object GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }
        
        #endregion

        private static LambdaExpression BuildExpression(
            Func<Type, Func<Type, Expression>, Expression> getParameter,
            MethodInfo parallel,
            Type[] returns,
            Func<Expression[], Expression[]> addReturn,
            ParameterExpression[] parameters,
            List<WorkItem> todo)
        {
            var expressionInfos = todo.Select(x => new ExpressionInfo(x)).ToList();

            // discover dependency chain
            for (int i = 0; i < expressionInfos.Count; i++)
            {
                var expressionInfo = expressionInfos[i];
                for (var parmIndex = 0; parmIndex < expressionInfo.ParameterTypes.Length; parmIndex++)
                {
                    var parmType = expressionInfo.ParameterTypes[parmIndex];
                    expressionInfo.Inputs[parmIndex] = FindInput(expressionInfo, parmType, i);

                }
            }

            var results = new Expression[returns.Length];

            for (int i = 0; i < returns.Length; i++)
            {
                var type = returns[i];
                results[i] = FindInput(null, type, expressionInfos.Count);
            }

            for (int i = 0; i < expressionInfos.Count; i++)
            {
                var expressionInfo = expressionInfos[i];
                if (parallel != null)
                {
                    var waitsOnOrder = expressionInfo.waitsOn.Select(x => x.order);
                    expressionInfo.order = waitsOnOrder.Any() ? waitsOnOrder.Max() + 1 : 0;
                }
                else {
                    expressionInfo.order = i;
                }
            }

            var body = expressionInfos.GroupBy(x => x.order).OrderBy(x => x.Key).SelectMany(group =>
            {
                if (group.Count() == 1)
                {
                    return MakeSingle(group.First());
                }
                else
                {
                    if (parallel == null)
                    {
                        return group.SelectMany(x => MakeSingle(x));
                    }
                    else
                    {
                        var lamddasParameter = Expression.Constant(group.Select(x => Expression.Lambda<Action>(Expression.Block(MakeSingle(x)))).Select(x => x.Compile()).ToArray());
                            return new List<Expression> {
                            Expression.Call(parallel, lamddasParameter)
                        };
                    }
                }
            }).ToList();

            body.AddRange(addReturn(results));

            return Expression.Lambda(Expression.Block(body), parameters);

            #region Help

            List<Expression> MakeSingle(ExpressionInfo target)
            {
                var res = new List<Expression>();
                var innerParameters = new List<Expression>();
                for (var i = 0; i < target.ParameterTypes.Length; i++)
                {
                    var input = target.Inputs[i];
                    innerParameters.Add(Expression.Convert(input, target.ParameterTypes[i]));
                }
                if (target.Result != null)
                {
                    res.Add(Expression.Assign(target.Result, Expression.Invoke(target.backing, innerParameters)));
                }
                else
                {
                    res.Add(Expression.Invoke(target.backing, innerParameters));
                }
                return res;
            }

            Expression GetVar(Type t)
            {
                var type = typeof(GetVarBackingHack<>).MakeGenericType(t);
                var holdShit = Expression.Constant(Activator.CreateInstance(type));
                return Expression.Field(holdShit, type.GetField($"thing"));
            }

            Expression FindInput(ExpressionInfo expressionOrNull, Type parmType, int startAt)
            {
                for (int j = startAt - 1; j >= 0; j--)
                {
                    var sourceExpressionInfo = expressionInfos[j];
                    if (sourceExpressionInfo.returnTypes.Length > 1)
                    {
                        // sourceExpressionInfo.ReturnType is valueTuple
                        // we need to walk over it's members
                        for (int k = 0; k < sourceExpressionInfo.returnTypes.Length; k++)
                        {
                            if (sourceExpressionInfo.returnTypes[k] == parmType)
                            {
                                expressionOrNull?.waitsOn?.Add(sourceExpressionInfo);
                                var tupleType = valueTypeArray.Value[sourceExpressionInfo.returnTypes.Length - 1].MakeGenericType(sourceExpressionInfo.returnTypes);
                                if (sourceExpressionInfo.Result == null)
                                {
                                    sourceExpressionInfo.Result = GetVar(tupleType);
                                }
                                return Expression.Field(sourceExpressionInfo.Result, tupleType.GetField("Item" + (k + 1)));
                            }
                        }
                    }
                    else
                    {
                        if (sourceExpressionInfo.returnTypes[0] == parmType)
                        {
                            expressionOrNull?.waitsOn?.Add(sourceExpressionInfo);
                            if (sourceExpressionInfo.Result == null)
                            {
                                sourceExpressionInfo.Result = GetVar(parmType);
                            }
                            return sourceExpressionInfo.Result;
                        }
                    }
                }
                return getParameter(parmType, t => FindInput(null, t, startAt));

            }

            #endregion
        }
        
        #region Static

        private static readonly Lazy<Type[]> valueTypeArray = new Lazy<Type[]>(() => {
            return new Type[]{
                    typeof(ValueTuple<>),
                    typeof(ValueTuple<,>),
                    typeof(ValueTuple<,,>),
                    typeof(ValueTuple<,,,>),
                    typeof(ValueTuple<,,,,>),
                    typeof(ValueTuple<,,,,,>),
                };
            });

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Reflection;
using Cormo.Contexts;
using Cormo.Impl.Weld.Components;
using Cormo.Impl.Weld.Utils;
using Cormo.Injects;

namespace Cormo.Impl.Weld.Introspectors
{
    public class InjectableMethod : InjectableMethodBase
    {
        public InjectableMethod(IWeldComponent component, MethodInfo method, ParameterInfo specialParameter) : 
            base(component, method, specialParameter)
        {
            Method = method;
        }

        public MethodInfo Method { get; private set; }

        protected override object Invoke(object[] parameters, ICreationalContext creationalContext)
        {
            var containingObject = Method.IsStatic ? null : Component.Manager.GetReference(Component, creationalContext);
            return Method.Invoke(containingObject, parameters);
        }

        public override InjectableMethodBase TranslateGenericArguments(IWeldComponent component, IDictionary<Type, Type> translations)
        {
            var resolvedMethod = GenericUtils.TranslateMethodGenericArguments(Method, translations);
            if (resolvedMethod == null || GenericUtils.MemberContainsGenericArguments(resolvedMethod))
                return null;
            return new InjectableMethod(component, resolvedMethod, SpecialParameter);
        }
    }
}
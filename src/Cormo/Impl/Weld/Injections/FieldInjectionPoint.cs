using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cormo.Impl.Weld.Utils;
using Cormo.Injects;
using Cormo.Reflects;

namespace Cormo.Impl.Weld.Injections
{
    public class FieldInjectionPoint : AbstractInjectionPoint
    {
        private readonly FieldInfo _field;

        public FieldInjectionPoint(IComponent declaringComponent, IAnnotatedField field) :
            this(declaringComponent, field.Field, field.Binders)
        {
            InjectionValidator.Validate(field);
        }

        private FieldInjectionPoint(IComponent declaringComponent, FieldInfo field, IBinders binders) :
            base(declaringComponent, field, field.FieldType, binders)
        {
            _field = field;
        }

        public override IWeldInjetionPoint TranslateGenericArguments(IComponent component, IDictionary<Type, Type> translations)
        {
            if (DeclaringComponent.Type == component.Type)
                return this;
                
            var field = GenericUtils.TranslateFieldType(_field, translations);
            return new FieldInjectionPoint(component, field, Binders);
        }

        protected override InjectPlan BuildInjectPlan(IComponent component)
        {
           return (target, context) =>
           {
               var value = GetValue(context);
               return SetValue(target, value);
            };
        }

        private object SetValue(object target, object instance)
        {
            _field.SetValue(target, instance);
            return instance;
        }

        public override string ToString()
        {
            return string.Format("field [{0}]", Formatters.Field(_field));
        }
    }
}
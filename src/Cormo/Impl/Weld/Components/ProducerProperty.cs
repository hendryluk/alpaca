using System;
using System.Collections.Generic;
using System.Reflection;
using Cormo.Impl.Weld.Utils;
using Cormo.Injects;

namespace Cormo.Impl.Weld.Components
{
    public class ProducerProperty : AbstractProducer
    {
        private readonly PropertyInfo _property;

        public ProducerProperty(PropertyInfo property, IEnumerable<QualifierAttribute> qualifiers, Type scope, WeldComponentManager manager)
            : base(property, property.PropertyType, qualifiers, scope, manager)
        {
            _property = property;
        }


        protected override AbstractProducer TranslateTypes(GenericUtils.Resolution resolution)
        {
            var resolvedProperty = GenericUtils.TranslatePropertyType(_property, resolution.GenericParameterTranslations);
            return new ProducerProperty(resolvedProperty, Qualifiers, Scope, Manager);
        }

        protected override BuildPlan GetBuildPlan()
        {
            return (context, ip) =>
            {
                var containingObject = Manager.GetReference(null, DeclaringComponent, context);
                return _property.GetValue(containingObject);
            };
        }

        public override string ToString()
        {
            return string.Format("Producer Property [{0}] with Qualifiers [{1}]", _property, string.Join(",", Qualifiers));
        }
    }
}
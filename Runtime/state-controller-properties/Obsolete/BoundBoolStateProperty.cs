using System;
using BeatThat.CollectionsExt;
using BeatThat.GetComponentsExt;
using UnityEngine;

namespace BeatThat.StateControllers
{
    /// <summary>
    /// Convenience base class for a bool state property that is uses an external binding component 
    /// to keep the property value in sync with the authoritative value, 
    /// e.g. a property of a service.
    /// 
    /// The behaviour is that when Start is called on this component,
    /// it checks if the binding component (provided as generic type)
    /// is present as a sibling on the GameObject and creates an instance if the binding component is missing.
    /// Any actualy binding behaviour is the responisbility of the binding-component implementation.
    /// </summary>
    /// <typeparam name="BindingComponent"></typeparam>
    [Obsolete("use [OptionalComponent] attributes instead (see base class HasValue::HandleOptionalComponents)")]
    public class BoundBoolStateProperty<BindingComponent> : BoolStateProperty
        where BindingComponent : Component
    {
        /// <summary>
        ///  By default, ensures the BindingComponent is exists as a sibling on Start
        /// </summary>
        public bool m_ensureBindingComponentOnStart = true;

        protected override void Start()
        {
            base.Start();
            if (m_ensureBindingComponentOnStart)
            {
                this.gameObject.AddIfMissing<BindingComponent>();
            }
        }
    }
}




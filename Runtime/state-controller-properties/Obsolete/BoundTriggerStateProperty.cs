using System;
using BeatThat.CollectionsExt;
using BeatThat.GetComponentsExt;
using UnityEngine;

namespace BeatThat.StateControllers
{
    [Obsolete("use [OptionalComponent] attributes instead (see base class HasValue::HandleOptionalComponents)")]
    public class BoundTriggerStateProperty<BindingComponent> : TriggerStateProperty
        where BindingComponent : Component
    {
        /// <summary>
        ///  By default, ensures the BindingComponent exists as a sibling on Start
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

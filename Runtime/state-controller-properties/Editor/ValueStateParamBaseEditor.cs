using System.Collections.Generic;
using BeatThat.OptionalComponents;
using BeatThat.Properties;
using BeatThat.StateControllers.ParamsEditorExtensions;
using UnityEditor;
using UnityEngine;

namespace BeatThat.StateControllers
{

    /// <summary>
    /// Editor functionality that works for [Bool|Int|Float|etc.]StateParamBase
    /// Stored in an abstract class because not allowed to have multiple types in [CustomEditor] attr
    /// </summary>
	public class ValueStateParamBaseEditor<ParamType, ValueType> : UnityEditor.Editor
        where ParamType : Component, Param, IHasValue<ValueType>
	{
        public override void OnInspectorGUI()
        {
            var param = this.target as ParamType;
            var so = this.serializedObject;
            var paramNameProp = so.FindProperty("m_property");

            if (paramNameProp != null)
            {
                param.HandleParamUndefinedInController(paramNameProp);
            }
            else
            {
                param.WarnIfParamUndefined();
            }

            // When app is running, user might want to change the value of the property
            // via the 'value' property in the inspector
            // and have that value update the underlying state, 
            // e.g. the actual property in the Animator, so ...
            var valueProp = so.FindProperty("m_value");
            ValueType valueBefore = default(ValueType);
            var hasPropVal = GetValue(valueProp, ref valueBefore);
            
			base.OnInspectorGUI();

			ValuePropertyEditor<ParamType, ValueType>.EditPropertyBindings (so, this.target.GetType ());

            ValueType valueAfter = default(ValueType);
            hasPropVal = hasPropVal && GetValue(valueProp, ref valueAfter);

            if (!EqualityComparer<ValueType>.Default.Equals(valueBefore, valueAfter)) {
                param.value = valueAfter;
			}

            so.ApplyModifiedProperties();

            if(!Application.isPlaying) {
                (this.target as Component).OnGUIProposeAddOptionalComponents();
            }
        }

        private bool GetValue(SerializedProperty p, ref ValueType value)
        {
            if(p == null)
            {
                return false;
            }

            if(typeof(ValueType) == typeof(int))
            {
                value = (ValueType)System.Convert.ChangeType(p.intValue, typeof(ValueType));
                return true;
            }

            if (typeof(ValueType) == typeof(float))
            {
                value = (ValueType)System.Convert.ChangeType(p.floatValue, typeof(ValueType));
                return true;
            }

            if (typeof(ValueType) == typeof(bool))
            {
                value = (ValueType)System.Convert.ChangeType(p.boolValue, typeof(ValueType));
                return true;
            }

            return false;
        }
	}
}


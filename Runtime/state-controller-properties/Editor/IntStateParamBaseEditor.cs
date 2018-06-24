using BeatThat.Properties;
using UnityEditor;
using UnityEngine;

namespace BeatThat.StateControllers
{
    [CustomEditor(typeof(IntStateParamBase), true)]
    [CanEditMultipleObjects]
    public class IntStateParamBaseEditor : ValueStateParamBaseEditor<IntStateParamBase, int> { }
}

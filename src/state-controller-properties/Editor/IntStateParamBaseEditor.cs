using UnityEditor;
using UnityEngine;

namespace BeatThat
{
    [CustomEditor(typeof(IntStateParamBase), true)]
    [CanEditMultipleObjects]
    public class IntStateParamBaseEditor : ValueStateParamBaseEditor<IntStateParamBase, int> { }
}
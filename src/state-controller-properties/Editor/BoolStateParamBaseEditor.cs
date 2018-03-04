using UnityEditor;

namespace BeatThat
{
    [CustomEditor(typeof(BoolStateParamBase), true)]
	[CanEditMultipleObjects]
    public class BoolStateParamBaseEditor : ValueStateParamBaseEditor<BoolStateParamBase, bool> { }

}
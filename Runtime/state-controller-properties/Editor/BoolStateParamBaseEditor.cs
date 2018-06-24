using BeatThat.Properties;
using UnityEditor;

namespace BeatThat.StateControllers
{
    [CustomEditor(typeof(BoolStateParamBase), true)]
	[CanEditMultipleObjects]
    public class BoolStateParamBaseEditor : ValueStateParamBaseEditor<BoolStateParamBase, bool> { }

}

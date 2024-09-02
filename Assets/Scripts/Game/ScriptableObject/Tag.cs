using UnityEngine;

[CreateAssetMenu(fileName = "Tag", menuName = "MoN/Tag")]
public class Tag : ScriptableObject {
	public string Name;

	[TextArea(2, 10)]
	public string Description;
}
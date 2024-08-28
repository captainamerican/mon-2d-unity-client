using UnityEngine;

[CreateAssetMenu(fileName = "Gameplay", menuName = "MoN/Gameplay")]
public class Gameplay : ScriptableObject {
	public string Name;

	[TextArea(2, 20)]
	public string Information;
}
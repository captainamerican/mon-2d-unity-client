using UnityEngine;

[CreateAssetMenu(fileName = "Spirit Wisdom", menuName = "MoN/Spirit Wisdom")]
public class SpiritWisdom : ScriptableObject {
	public string Name;
	public Game.SpiritId SpiritId;

	[TextArea(2, 10)]
	public string BattleStart;

	[TextArea(2, 10)]
	public string BattleEnd;
}
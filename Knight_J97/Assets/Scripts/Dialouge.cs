using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue")]
public class Dialogue : ScriptableObject
{
   public string npcName;
	public Sprite npcPortrait;
	public string[] dialogueLines;
	public bool[] autoProgressLines;
	public float autoProgressDelay = 1.5f;
	public float typingSpeed = 0.05f;

	
}

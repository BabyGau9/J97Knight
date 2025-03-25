using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPCDialogue : MonoBehaviour
{
	ScoreKeeper scoreKeeper;
	LevelManager levelManager;


	public GameObject Player;

	public Dialogue dialogueData;
	public GameObject dialoguePanel;
	public TMP_Text dialogueText, nameText;
	public Image portraitImage;

	private int dialogueIndex;
	private bool isTyping, isDialogueActive;

	public void Start()
	{
		scoreKeeper = FindObjectOfType<ScoreKeeper>();
		levelManager = FindObjectOfType<LevelManager>();

		dialoguePanel.SetActive(false);
		isDialogueActive = false;
	}

	public void Interact()
	{
		if (dialogueData == null)
			return;
		if (isDialogueActive)
		{
			NextLine();
		}
		else
		{
			StartDialogue();
		}
	}

	void StartDialogue()
	{
		isDialogueActive = true;
		dialogueIndex = 0;

		nameText.SetText(dialogueData.npcName);
		portraitImage.sprite = dialogueData.npcPortrait;

		dialoguePanel.SetActive(true);

		StartCoroutine(TypeLine());
	}

	void NextLine()
	{
		if (isTyping)
		{
			StopAllCoroutines();
			dialogueText.SetText(dialogueData.dialogueLines[dialogueIndex]);
			isTyping = false;
		}
		else if (++dialogueIndex < dialogueData.dialogueLines.Length)
		{
			StartCoroutine(TypeLine());
		}
		else
		{
			EndDialogue();
		}
	}

	IEnumerator TypeLine()
	{
		isTyping = true;
		dialogueText.text = "";
		foreach (char c in dialogueData.dialogueLines[dialogueIndex].ToCharArray())
		{
			dialogueText.text += c;
			yield return new WaitForSeconds(dialogueData.typingSpeed);
		}
		isTyping = false;

		if (dialogueData.autoProgressLines.Length > dialogueIndex && dialogueData.autoProgressLines[dialogueIndex])
		{
			yield return new WaitForSeconds(dialogueData.autoProgressDelay);
			NextLine();
		}
	}

	public void EndDialogue()
	{
		StopAllCoroutines();
		isDialogueActive = false;
		dialogueText.SetText("");
		dialoguePanel.SetActive(false);

		StartCoroutine(DelayFunction(5));
		CheckPoint();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			Interact();
		}
	}

	private void CheckPoint()
	{
		if (scoreKeeper.GetScore() < 500)
		{
			var animator = Player.GetComponent<Health>().GetComponent<Animator>();
			animator.SetTrigger("Die");
			var movement = Player.GetComponent<PlayerMovement>();
			movement.enabled = false;
			levelManager.LoadGameOver();
		}
	}

	IEnumerator DelayFunction(float seconds)
	{
		// Wait for the specified time in seconds
		yield return new WaitForSeconds(seconds);

		// Code to execute after the delay
		Debug.Log("Delay complete!");
	}
}

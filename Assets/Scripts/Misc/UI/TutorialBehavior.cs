using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBehavior : MonoBehaviour
{
	[SerializeField] private SpriteRenderer hint;

	[SerializeField] private Sprite hintimg;

	[SerializeField] private SpriteRenderer hint2;

	[SerializeField] private Sprite hintimg2;

	private Animator anim;

	void OnEnable()
	{	
		anim = this.gameObject.GetComponent<Animator>();
		anim.speed = 1;
		anim.Play("TutorialEnter");
	}

	public void Vanish()
	{
		this.gameObject.SetActive(false);

	}

	public void CloseTutorial()
	{	
		anim.Play("TutorialExit");
	}

	public void AddTutorialImages(Sprite img1)
	{
		hint.sprite = img1;
		hintimg = img1;
	}
	public void AddTutorialImages(Sprite img1, Sprite img2)
	{
		hint.sprite = img1;
		hintimg = img1;
		hint2.sprite = img2;
		hintimg2 = img2;			
	}
}

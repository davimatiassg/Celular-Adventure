using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
	[SerializeField] private TutorialBehavior banner;

	[SerializeField] private GameObject bannerobj;

	[SerializeField] private Sprite backImage;

	[SerializeField] private Sprite frontImage;

	private void OnTriggerEnter2D(Collider2D other)
	{

		if (other.gameObject.tag.Equals("Player"))
		{
			ActivateBanner();
		}

	}
	private void OnTriggerExit2D(Collider2D other)
	{

		if (other.gameObject.tag.Equals("Player") && !other.isTrigger)
		{
			banner.CloseTutorial();
		}

	}

	public void ActivateBanner()
	{
			if(banner)
			{
				banner.gameObject.SetActive(true);
			}
			else
			{
				banner = Instantiate(bannerobj, this.gameObject.transform.position + Vector3.up*5, bannerobj.transform.rotation).GetComponent<TutorialBehavior>();
			}
			if(frontImage)
			{
				banner.AddTutorialImages(backImage, frontImage);
			}
			else
			{
				banner.AddTutorialImages(backImage);
			}
			
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoredNote : MonoBehaviour
{
	[SerializeField] private PhisicNote thisNote;
	[SerializeField] private GameObject fNote;
	[SerializeField] private bool hovered;
	[SerializeField] private GameObject hovernote;

	public void CreateNote(bool h)
	{	
		hovered = h;
		GameObject note = Instantiate(fNote, gameObject.transform.position -Vector3.forward*gameObject.transform.position.z, gameObject.transform.rotation , this.gameObject.transform.parent.parent.parent);
		note.GetComponent<PhisicNote>().isText = thisNote.isText;
		note.GetComponent<PhisicNote>().SetInfo(thisNote.InfoBase);

		if(hovered)
		{
			hovernote = note;
		}
		else
		{	
			Destroy(hovernote);

			hovernote = note;
		}
	}
	public void DeleteNote()
	{
		if(hovered && !hovernote.GetComponent<FlyingNote>().isDrag)
		{
			Destroy(hovernote);
		}
			hovered = false;
		
			
	}
}

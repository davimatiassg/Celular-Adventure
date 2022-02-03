using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoredNote : MonoBehaviour
{
	[SerializeField] private PhisicNote thisNote;
	[SerializeField] private GameObject fNote;

	public void CreateNote(bool h)
	{	
		this.gameObject.transform.parent.parent.parent.gameObject.GetComponent<AnotationHotBar>().TryRespawnNote();
		GameObject note = Instantiate(fNote, GameObject.FindWithTag("Indicator").transform);
		note.GetComponent<PhisicNote>().isText = thisNote.isText;
		note.GetComponent<PhisicNote>().SetInfo(thisNote.InfoBase);
		DeleteNote();

	}
	public void DeleteNote()
	{
		
		Destroy(this.gameObject);
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class DialogContent
{	
	[SerializeField] private string ownerName;
	[SerializeField] private Sprite ownerPhoto;
	[TextArea]
	[SerializeField] private string dialogText;
	[SerializeField] private UnityEvent endAction;
	public string GetOwnerName()
	{
		return ownerName;
	}
	public Sprite GetOwnerPhoto()
	{
		return ownerPhoto;
	}
	public string GetMainText()
	{
		if(dialogText != null)
		{
			return dialogText;
		}
		return "";
	}

	public void FinalAction()
	{
		if(endAction != null)
		{	

			endAction.Invoke();
		}
	}

	public string ToString()
	{
		return "Falante: " + ownerName + "; Texto: " + dialogText + ";";  
	}

}

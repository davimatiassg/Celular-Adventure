	using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogInitializer : MonoBehaviour
{
    // Start is called before the first frame update

	[SerializeField] public List<DialogContent> dialog = new List<DialogContent>();

    // Update is called once per frame
	[SerializeField]
	public static DialogBox TextBox;

	public void OpenDialogBox()
    {   
        GameEvents.ScreamEvent("GetDialogBox");
        DialogInitializer.TextBox.dialogSequence = new List<DialogContent>(dialog);
    	DialogInitializer.TextBox.Activate(true);
        Debug.Log("START DIALOG");
    	
    }
	public void CloseDialogBox()
    {
    	DialogInitializer.TextBox.dialogSequence = new List<DialogContent>();
    	DialogInitializer.TextBox.Activate(false);
    }

}

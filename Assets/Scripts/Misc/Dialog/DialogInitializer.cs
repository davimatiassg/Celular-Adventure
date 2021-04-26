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

    void Start()
    {   
        GameEvents.ScreamEvent("GetDialogBox");
    }
	public void OpenDialogBox()
    {   
        GameEvents.ScreamEvent("GetDialogBox");
        Debug.Log(DialogInitializer.TextBox);
        DialogInitializer.TextBox.dialogSequence = new List<DialogContent>(dialog);
    	DialogInitializer.TextBox.Activate(true);
    	
    }
	public void CloseDialogBox()
    {
    	DialogInitializer.TextBox.dialogSequence = new List<DialogContent>();
    	DialogInitializer.TextBox.Activate(false);
    }

}

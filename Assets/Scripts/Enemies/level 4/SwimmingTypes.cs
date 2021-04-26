using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwimmingTypes : MonoBehaviour
{	
    [SerializeField] private bool insideWater = false;
    // Start is called before the first frame update
    public void SetinWater(bool value)
    {
        insideWater = value;
    }

    // Update is called once per frame
    public bool Getinwater()
    {	
        return insideWater;
    }
}

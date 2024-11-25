using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Social : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OpenSite(string url)
    {
        Application.OpenURL(url);
    }
}

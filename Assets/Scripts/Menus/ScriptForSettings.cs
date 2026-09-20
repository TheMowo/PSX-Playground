using UnityEngine;

public class ScriptForSettings : MonoBehaviour
{
    private void Awake()
    {
        
    }
    private void Start()
    {
        GetComponentInParent<M_MenuManager>().JumpToPage(0);
    }
}

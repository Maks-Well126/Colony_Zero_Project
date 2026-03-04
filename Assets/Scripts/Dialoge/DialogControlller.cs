using Unity.VisualScripting;
using UnityEngine;
using static DialogeSystem;

public class DialogControlller : MonoBehaviour
{
    private void Start()
    {
        if (!Save.LoadLevel1State())
        {
            DialogeSystem.StartDialoge(DialogType.GameStart);
        }        
    }    
}

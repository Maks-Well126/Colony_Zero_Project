using System.Data.Common;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    public static InputManager Instance {get; private set;}
    public InputSystem_Actions actions;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        if(Instance != null) return;

        GameObject go = new GameObject("InputManager");
        Instance = go.AddComponent<InputManager>();
        DontDestroyOnLoad(go);
    }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        actions = new InputSystem_Actions();
        actions.Enable();
    }

    private void Oestroy()
    {

        if(Instance == this)
        {
            actions.Disable();
            actions = null;
            Instance = null;
        }

    }
}

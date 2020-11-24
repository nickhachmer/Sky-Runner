using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ControlOptionsController : MonoBehaviour
{

    public Text jump;

    private InputMaster controls;

    private InputActionRebindingExtensions.RebindingOperation m_RebindOperation;

    // Start is called before the first frame update
    void Start()
    {
        controls = new InputMaster();
    }

    // Update is called once per frame
    void Update()
    {
        jump.text = controls.Player.Jump.GetBindingDisplayString();

        
    }


    public void rebing(int a)
    {
        switch (a) 
        {
            case 1: RemapButtonClicked(controls.Player.Jump); break;
        }

    }
    public void RemapButtonClicked(InputAction actionToRebind)
    {
    }
}

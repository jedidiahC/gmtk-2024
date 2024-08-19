using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIToolbarFrame : MonoBehaviour
{
    [SerializeField] private UIIconInteractable[] _childIconsScripts;
    private void Start()
    {
        ToggleTransformControls(false, false, false);
    }

    public void ToggleTransformControls(bool canTranslate, bool canRotate, bool canScale)
    {
        _childIconsScripts[2].ToggleIcon(canTranslate);
        _childIconsScripts[3].ToggleIcon(canRotate);
        _childIconsScripts[4].ToggleIcon(canScale);
    }
}

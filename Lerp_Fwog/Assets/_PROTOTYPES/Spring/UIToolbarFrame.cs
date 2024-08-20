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

    public void ToggleTransformInUse(eTransformType transformType)
    {
        _childIconsScripts[2].ToggleIsUsing(false);
        _childIconsScripts[3].ToggleIsUsing(false);
        _childIconsScripts[4].ToggleIsUsing(false);

        switch (transformType)
        {
            case eTransformType.Translation:
                _childIconsScripts[2].ToggleIsUsing(true);
                break;
            case eTransformType.Rotation:
                _childIconsScripts[3].ToggleIsUsing(true);
                break;
            case eTransformType.Scale:
                _childIconsScripts[4].ToggleIsUsing(true);
                break;
        }
    }

    public void SetAllInUseToFalse()
    {
        _childIconsScripts[2].ToggleIsUsing(false);
        _childIconsScripts[3].ToggleIsUsing(false);
        _childIconsScripts[4].ToggleIsUsing(false);
    }

    public void TogglePlayInUse(bool inIsUsing) {
        _childIconsScripts[0].ToggleIsUsing(inIsUsing);
    }
    public void TogglePlayInteractable(bool inIsInteractable) {
        _childIconsScripts[0].ToggleIcon(inIsInteractable);
    }

    public void ToggleResetInUse(bool inIsUsing) {
        _childIconsScripts[1].ToggleIsUsing(inIsUsing);
    }
    public void ToggleResetInteractable(bool inIsInteractable) {
        _childIconsScripts[1].ToggleIcon(inIsInteractable);
    }
}

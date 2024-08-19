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
        _childIconsScripts[2].isUsingBool = false;
        _childIconsScripts[3].isUsingBool = false;
        _childIconsScripts[4].isUsingBool = false;

        switch (transformType)
        {
            case eTransformType.Translation:
                _childIconsScripts[2].isUsingBool = true;
                break;
            case eTransformType.Rotation:
                _childIconsScripts[3].isUsingBool = true;
                break;
            case eTransformType.Scale:
                _childIconsScripts[4].isUsingBool = true;
                break;
        }
    }
}

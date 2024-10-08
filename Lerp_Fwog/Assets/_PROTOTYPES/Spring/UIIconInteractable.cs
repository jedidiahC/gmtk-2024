using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UIIconInteractable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    // Yumi test
    [SerializeField] private float _dampingRatio = 0.3f, _angularFrequency = 0.2f, _timeStep = 1.0f;
    [SerializeField] private float _currentX, _velocityX;
    public float _targetX;
    [SerializeField] private float _currentY, _velocityY;
    public float _targetY;
    [SerializeField] private float _currentScale, _velocityScale;
    public float _targetScale;
    private Vector3 _initialLocalPosition;
    public RectTransform rectTransform;
    private Image _imageComponent;
    public Button _buttonComponent;
    [SerializeField] private Color _enabledColour;
    [SerializeField] private Color _disabledColour;
    public float rectWidth;
    public bool isUsingBool;
    public bool isOnBool;
    public Sprite disabledSprite;
    public bool useDisabledSprite;

    private Sprite enabledSprite;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        rectWidth = rectTransform.rect.width;
        _initialLocalPosition = transform.localPosition;

        _imageComponent = GetComponent<Image>();
        _buttonComponent = GetComponent<Button>();

        enabledSprite = _imageComponent.sprite;
    }

    private void Update()
    {
        SpringMath.Lerp(ref _currentX, ref _velocityX, _targetX, _dampingRatio, _angularFrequency, _timeStep);
        SpringMath.Lerp(ref _currentY, ref _velocityY, _targetY, _dampingRatio, _angularFrequency, _timeStep);
        SpringMath.Lerp(ref _currentScale, ref _velocityScale, _targetScale, _dampingRatio, _angularFrequency, _timeStep);
        transform.localPosition = _initialLocalPosition + Vector3.right * _currentX + Vector3.up * _currentY;
        transform.localScale = Vector3.one + Vector3.one * _currentScale;
    }

    public void ToggleIcon(bool isOn, bool handleInteractivity = true)
    {
        isOnBool = isOn;

        if (useDisabledSprite)
        {
            _imageComponent.sprite = isOn ? enabledSprite : disabledSprite;
        }
        else
        {
            _imageComponent.color = isOn ? _enabledColour : _disabledColour;
        }

        // Debug.Log(String.Format("{0} toggled isOn {1}", name, isOn));

        if (handleInteractivity)
        {
            _imageComponent.raycastTarget = isOn;
            _buttonComponent.interactable = isOn;
        }
    }

    public void ToggleIsUsing(bool isUsing)
    {
        // Debug.Log(String.Format("{0} toggled isUsing {1}", name, isUsing));
        isUsingBool = isUsing;
        if (isUsing)
        {
            _targetY = 16.0f;
            _targetScale = 0.2f;
        }
        else
        {
            _targetY = 0.0f;
            _targetScale = 0.0f;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _targetY = 18.0f;
        _targetScale = 0.3f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _targetY = isUsingBool ? 16.0f : 0.0f;
        _targetScale = isUsingBool ? 0.2f : 0.0f;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _currentScale = 0.0f;
    }
}

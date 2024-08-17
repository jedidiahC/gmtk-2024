using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DemoCircle : MonoBehaviour
{
	public float _dampingRatio = 0.5f, _angularFrequency = 0.1f, _timeStep = 1.0f;
	private float _timer;
	private float _currentX, _velocity, _targetX = 5.0f;
	private Vector3 _initialPosition;
	private Vector2 _initialScale;
	[SerializeField] private Slider _sliderDR, _sliderAF;
	[SerializeField] private TextMeshProUGUI _valueText;
	[SerializeField] private float _currentScaleX, _scaleVelocityX, _targetScaleX = 0.75f;

	void Start()
	{
		_initialPosition = transform.position;
		_initialScale = transform.localScale;

		if (_valueText)
			_valueText.text = "Damping " + _dampingRatio + "\nFrequency " + _angularFrequency;
	}

	void Update()
	{
		SpringMath.Lerp(ref _currentX, ref _velocity, _targetX, _dampingRatio, _angularFrequency, _timeStep);
		transform.position = _initialPosition + Vector3.right * _currentX;

		SpringMath.Lerp(ref _currentScaleX, ref _scaleVelocityX, _targetScaleX, _dampingRatio, _angularFrequency, _timeStep);
		transform.localScale = new Vector2(_initialScale.x + _currentScaleX, 1.0f);

		_timer += Time.deltaTime;
		if (_timer > 3.0f/*transform.position.x == _targetX*/)
		{
			_timer -= 3.0f;
			_targetX *= -1;
			_targetScaleX *= -1;
		}
	}

	public void SetDampingRatio()
	{
		_dampingRatio = _sliderDR.value;
		_valueText.text = "Damping " + _dampingRatio + "\nFrequency " + _angularFrequency;
	}

	public void SetAngularFrequency()
	{
		_angularFrequency = _sliderAF.value;
		_valueText.text = "Damping " + _dampingRatio + "\nFrequency " + _angularFrequency;
	}
}

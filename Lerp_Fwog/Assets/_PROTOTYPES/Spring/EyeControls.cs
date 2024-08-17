using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeControls : MonoBehaviour
{
  [SerializeField] private GameObject _eyeL;
  [SerializeField] private GameObject _eyeR;
  private Animator _animatorL;
  private Animator _animatorR;

  [Space]
  public bool triggerNormal;
  public bool triggerBored;
  public bool triggerShook;
  public bool triggerFrown;

  private void Awake()
  {
    _animatorL = _eyeL.GetComponent<Animator>();
    _animatorR = _eyeR.GetComponent<Animator>();
  }

  private void Update()
  {
    if (triggerNormal) { triggerNormal = false; _animatorL.SetTrigger("Normal"); _animatorR.SetTrigger("Normal"); }
    if (triggerBored) { triggerBored = false; _animatorL.SetTrigger("Bored"); _animatorR.SetTrigger("Bored"); }
    if (triggerShook) { triggerShook = false; _animatorL.SetTrigger("Shook"); _animatorR.SetTrigger("Shook"); }
    if (triggerFrown) { triggerFrown = false; _animatorL.SetTrigger("Frown"); _animatorR.SetTrigger("Frown"); }
  }
}

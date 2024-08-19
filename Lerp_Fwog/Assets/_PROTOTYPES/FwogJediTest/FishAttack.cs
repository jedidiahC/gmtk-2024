using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FishAttack : MonoBehaviour
{
    // [SerializeField] private TadpoleLife target = null;
    // [SerializeField] private float speed = 5.0f;

    //private void Update()
    //{
        // ChooseClosestTarget();
        // Chase();
    //}

    //private void ChooseClosestTarget()
    //{
    //    List<TadpoleLife> prey = GameTracker.GetInstance().GetTadpoles();

    //    if (prey != null)
    //    {
    //        foreach (var candidateTarget in prey)
    //        {
    //            if (candidateTarget == null || candidateTarget.IsDead)
    //            {
    //                continue;
    //            }

    //            if (target == null || target.IsDead || GetDist(candidateTarget) < GetDist(target))
    //            {
    //                target = candidateTarget;
    //            }
    //        }
    //    }
    //}

    //private void Chase()
    //{
    //    if (target == null)
    //    {
    //        return;
    //    }

    //    transform.position += (target.transform.position - this.transform.position).normalized * speed * Time.deltaTime;
    //}

    //private float GetDist(TadpoleLife life)
    //{
    //    return Vector3.Distance(this.transform.position, life.transform.position);
    //}


    void OnCollisionEnter2D(Collision2D col)
    {
        TadpoleLife victim = col.gameObject.GetComponent<TadpoleLife>();

        if (victim != null)
        {
            victim.Kill();
            // this.target = null;
        }
    }
}

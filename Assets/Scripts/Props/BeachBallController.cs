using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BeachBallController : MonoBehaviour, IInputClickHandler
{
    public void OnInputClicked(InputClickedEventData eventData)
    {
        var rigidBody = GetComponent<Rigidbody>();

        if (rigidBody == null)
            return;

        var direction = (transform.position - Camera.main.transform.position).normalized;
        direction.y = 3f;

        rigidBody.AddForce(direction * 60f);

    }
}

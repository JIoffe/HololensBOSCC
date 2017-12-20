using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherMachineController : MonoBehaviour {
    public GameObject rainPrefab;
    public int sprayCount = 12;
    public float sprayCone = 30f;


    public void InvokeRain()
    {
        if (rainPrefab == null)
            return;

        for(var i = 0; i < sprayCount; ++i)
        {
            var projectile = Instantiate(rainPrefab);

            var direction = transform.forward;

            var sprayOffset = Quaternion.Euler(Random.Range(-sprayCone, sprayCone), Random.Range(-sprayCone, sprayCone), 0);

            direction = sprayOffset * direction;
            projectile.transform.position = transform.position + direction * 0.5f;

            var rigidBody = projectile.GetComponent<Rigidbody>();
            if(rigidBody != null)
            {
                rigidBody.AddForce(direction * 380f);
            }
        }
    }
}

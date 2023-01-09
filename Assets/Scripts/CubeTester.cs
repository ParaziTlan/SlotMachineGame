using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeTester : MonoBehaviour
{
    float position = 6;
    float speed = 10;
    float time = 0.1f;
    float acceleration;
    private void Start()
    {
        //acceleration = (speed * speed) / (2 * (position - speed * time));
        acceleration = (2 * (6 + position + speed * time)) / (time * time);
    }

    void Update()
    {
        if (speed > 0)
        {
            speed -= acceleration * Time.deltaTime;
            position -= speed * Time.deltaTime;
            transform.position = new Vector3(transform.position.x, position, transform.position.z);
        }

    }
}

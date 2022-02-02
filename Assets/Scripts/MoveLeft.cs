using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLeft : MonoBehaviour
{
    public static float currentSpeed = 13;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.left * Time.deltaTime * currentSpeed, Space.World);

        if (transform.position.x < -15)
        {
            gameObject.SetActive(false);
        }
    }
}

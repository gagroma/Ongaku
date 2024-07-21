using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightArm : MonoBehaviour
{
    void Update()
    {
        Vector2 cursorPosition = Input.mousePosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(cursorPosition);
        Vector2 direction = worldPosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

    }

}

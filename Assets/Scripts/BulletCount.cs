using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BulletCount : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textMeshPro;
    
    private void Update()
    {
        textMeshPro.text = Pistol.instance.currentBulletCount.ToString();
    }
}

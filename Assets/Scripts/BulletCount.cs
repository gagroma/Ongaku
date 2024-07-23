using TMPro;
using UnityEngine;

public class BulletCount : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textMeshPro;
    private void Update()
    {
        textMeshPro.text = Weapon.instance.currentBulletCount.ToString();
    }
}
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
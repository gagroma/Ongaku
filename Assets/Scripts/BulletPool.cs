using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool instance;
    [SerializeField] private GameObject pulledBullet;
    private bool notEnoughBulletsInPool = false;
    private List<GameObject> bullets;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        bullets = new List<GameObject>();
    }
    public GameObject GetBullet()
    {
        if (bullets.Count > 0)
        {
            for (int i = 0; i < bullets.Count; i++)
            {
                if (!bullets[i].activeInHierarchy)
                {
                    return bullets[i];
                }
            }
        }
        if (notEnoughBulletsInPool)
        {
            GameObject bul = Instantiate(pulledBullet);
            bul.SetActive(false);
            bullets.Add(bul);
            return bul;
        }
        return null;
    }
    void Update()
    {
        
    }
}

using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private GameObject heartPrefab;
    List<HealthSystem> hearts = new List<HealthSystem>();

    private void Start()
    {
        DrawHearts();
    }
    private void OnEnable()
    {
        Player.instance.OnPlayerTakedDamage += DrawHearts;
    }
    private void OnDisable()
    {
        Player.instance.OnPlayerTakedDamage -= DrawHearts;
    }
    public void DrawHearts()
    {
        ClearHearts();
        float maxHealthRemainder = Player.instance.playerHealthMax % 2;
        int heartsToMake = (int)((Player.instance.playerHealthMax / 2) + maxHealthRemainder);
        for (int i = 0; i < heartsToMake ; i++)
        {
            CreateEmptyHeart();
        }
        for (int i = 0; i < hearts.Count; i++)
        {
            int heartStatusRemainder = (int)Mathf.Clamp(Player.instance.playerHealth - (i*2), 0, 2);
            hearts[i].SetHeartImage((HeartStatus)heartStatusRemainder);
        }
    }
    public void CreateEmptyHeart()
    {

        GameObject newHeart = Instantiate(heartPrefab);
        newHeart.transform.SetParent(transform);
        newHeart.transform.localScale = new Vector3(1, 1, 0);
        HealthSystem heartComponent = newHeart.GetComponent<HealthSystem>();
        heartComponent.SetHeartImage(HeartStatus.Empty);
        hearts.Add(heartComponent);

    }
    public void ClearHearts()
    {
        foreach (Transform t in transform) Destroy(t.gameObject);
        hearts = new List<HealthSystem>();
    }
}

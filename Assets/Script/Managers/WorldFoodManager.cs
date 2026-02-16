using System.Collections.Generic;
using UnityEngine;

public class WorldFoodManager : MonoBehaviour
{
    public static WorldFoodManager Instance;

    private List<Transform> foodSources = new List<Transform>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterFood(Transform food)
    {
        if (!foodSources.Contains(food))
            foodSources.Add(food);
    }

    public void UnregisterFood(Transform food)
    {
        if (foodSources.Contains(food))
            foodSources.Remove(food);
    }

    public Transform GetClosestFood(Vector3 position, float maxDistance)
    {
        Transform closest = null;
        float minDist = Mathf.Infinity;

        foreach (var food in foodSources)
        {
            float dist = Vector3.Distance(position, food.position);
            if (dist < minDist && dist <= maxDistance)
            {
                minDist = dist;
                closest = food;
            }
        }

        return closest;
    }
}
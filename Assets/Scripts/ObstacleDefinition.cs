using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObstacleColorDefinition", menuName = "ScriptableObjects/ObstacleColorDefinition", order = 1)]
public class ObstacleDefinition : ScriptableObject
{
    public List<ColorDefinition> colorDefinitions = new ();

    public Config.OBSTACLE_TYPE FindObstacle(Sprite sprite)
    {
        var definition = colorDefinitions.Find(x => x.sprite == sprite);
        if (definition == null)
            return Config.OBSTACLE_TYPE.NONE;
        
        return colorDefinitions.Find(x => x.sprite == sprite).obstacleType;
    }
}

[Serializable]
public class ColorDefinition
{
    public Sprite sprite;
    public Config.OBSTACLE_TYPE obstacleType;
}

using UnityEngine;

[CreateAssetMenu(fileName = "Avatar_1", menuName = "ScriptableObjects/AvatarDefinition", order = 1)]
public class AvatarDefinition : ScriptableObject
{
   public int avatarId;
   public Sprite avatarSprite;
}

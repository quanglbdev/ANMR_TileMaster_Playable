using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AvatarController : MonoBehaviour
{
    [SerializeField] private AvatarCard avatarPrefabs;
    [SerializeField] private Transform avatarGroup;

    [FormerlySerializedAs("_avatarCards")] public List<AvatarCard> avatarCards = new();
    public void Init()
    {
        foreach (var definition in AssetManager.Instance.avatarDefinitions)
        {
            var avatarCard = Instantiate(avatarPrefabs, avatarGroup);
            avatarCard.Init(definition, this);
            avatarCards.Add(avatarCard);
        }
    }

    public void Load()
    {
        foreach (var avatarCard in avatarCards)
        {
            avatarCard.Load();
        }
    }
}

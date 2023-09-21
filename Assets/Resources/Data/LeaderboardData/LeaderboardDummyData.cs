using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "LeaderboardDummyData", menuName = ("ScriptableObjects/LeaderboardDummyData"))]
public class LeaderboardDummyData : ScriptableObject
{
    public List<UserRankSO> leaderboardDefinitions = new();

    public void RecruitData(UserRank data)
    {
        leaderboardDefinitions.Add(new UserRankSO(data.id, data.username, data.score, data.avatarId));   
    }

    public void Clear()
    {
        leaderboardDefinitions.Clear();
        SetDirty();
    }
}

[System.Serializable]
public class UserRankSO
{
    public int id;
    public string playerName;
    public int score;
    public int avatarId;

    public UserRankSO(int id, string playerName, int score, int avatarId)
    {
        this.id = id;
        this.playerName = playerName;
        this.score = score;
        this.avatarId = avatarId;
    }
}
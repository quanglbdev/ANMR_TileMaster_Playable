using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class WriteCSV : MonoBehaviour
{
    [FormerlySerializedAs("leaderboardDummyData")] [SerializeField]
    private LeaderboardDummyData leaderboardDummyData_WEEK;
    [SerializeField] private int minScoreWEEK, maxScoreWEEK;

    [Button]
    [Obsolete("Obsolete")]
    public void WriteCsvFakeLeaderBoardWeek()
    {
        var userRanks = new List<UserRank>();
        var sheet = CSVParser.LoadFromPath("Assets/CSV Parser/LeaderboardDummy(CSV).csv");
        for (var index = 1; index < sheet.Count; index++)
        {
            var row = sheet[index];

            userRanks.Add(new UserRank(row[0], row[1], Random.Range(minScoreWEEK, maxScoreWEEK)));
        }

        leaderboardDummyData_WEEK.leaderboardDefinitions.Clear();
        foreach (var data in userRanks)
        {
            leaderboardDummyData_WEEK.RecruitData(data);
            leaderboardDummyData_WEEK.SetDirty();
            Debug.Log($"ID : {data.id}, Score : {data.score}, Name : {data.username}, AVT : {data.avatarId}");
        }
    }
    [Space(30)]
    [SerializeField] private LeaderboardDummyData leaderboardDummyData_TOP;

    [SerializeField] private int minScore, maxScore;
    [Button]
    [Obsolete("Obsolete")]
    public void WriteCsvFakeLeaderBoardTop()
    {
        var userRanks = new List<UserRank>();
        var sheet = CSVParser.LoadFromPath("Assets/CSV Parser/LeaderboardDummy2(CSV).csv");
        for (var index = 1; index < sheet.Count; index++)
        {
            var row = sheet[index];

            userRanks.Add(new UserRank(row[0], row[1], Random.Range(minScore, maxScore)));
        }

        leaderboardDummyData_TOP.leaderboardDefinitions.Clear();
        foreach (var data in userRanks)
        {
            leaderboardDummyData_TOP.RecruitData(data);
            leaderboardDummyData_TOP.SetDirty();
            Debug.Log($"ID : {data.id}, Score : {data.score}, Name : {data.username}, AVT : {data.avatarId}");
        }
    }
}

public class UserRank
{
    public int id;
    public int score;
    public int avatarId;
    public string username;

    public UserRank(string id, string username, int score)
    {
        this.id = StringParse(id);
        this.score = score;
        avatarId = Random.Range(1, 8);
        this.username = username;
    }

    private int StringParse(string str)
    {
        if (string.IsNullOrEmpty(str) || str == "")
            return -1;

        return int.Parse(str);
    }
}
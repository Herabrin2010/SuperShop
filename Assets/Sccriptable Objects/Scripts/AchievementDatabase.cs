using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

[CreateAssetMenu(fileName = "AchievementDatabase", menuName = "Game/Achievement Database")]
public class AchievementDatabase : ScriptableObject
{
    public List<Achievement> achievements = new List<Achievement>();

    public Achievement GetAchievement(string id)
    {
        return achievements.Find(a => a.id == id);
    }
}
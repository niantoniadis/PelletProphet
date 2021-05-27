using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPatterns
{
    List<List<EnemySpawnPattern>> patternsByTier;

    public EnemySpawnPatterns()
    {
        patternsByTier = new List<List<EnemySpawnPattern>>();

        //tier 1 spawn patterns
        patternsByTier.Add(new List<EnemySpawnPattern>());
        patternsByTier[0].Add(new EnemySpawnPattern(1, 0, 0, 0));
        patternsByTier[0].Add(new EnemySpawnPattern(2, 0, 0, 0));
        patternsByTier[0].Add(new EnemySpawnPattern(0, 1, 0, 0));
        patternsByTier[0].Add(new EnemySpawnPattern(0, 0, 1, 0));
        //tier 2 spawn patterns
        patternsByTier.Add(new List<EnemySpawnPattern>());
        patternsByTier[1].Add(new EnemySpawnPattern(0, 2, 0, 0));
        patternsByTier[1].Add(new EnemySpawnPattern(2, 1, 0, 0));
        patternsByTier[1].Add(new EnemySpawnPattern(1, 0, 0, 1));
        patternsByTier[1].Add(new EnemySpawnPattern(2, 0, 1, 0));
        patternsByTier[1].Add(new EnemySpawnPattern(0, 1, 0, 1));
        patternsByTier[1].Add(new EnemySpawnPattern(1, 1, 1, 0));
        patternsByTier[1].Add(new EnemySpawnPattern(1, 0, 1, 1));
        //tier 3 spawn patterns
        patternsByTier.Add(new List<EnemySpawnPattern>());
        patternsByTier[2].Add(new EnemySpawnPattern(2, 1, 1, 0));
        patternsByTier[2].Add(new EnemySpawnPattern(0, 3, 0, 1));
        patternsByTier[2].Add(new EnemySpawnPattern(2, 2, 0, 0));
        patternsByTier[2].Add(new EnemySpawnPattern(3, 1, 0, 1));
        patternsByTier[2].Add(new EnemySpawnPattern(1, 3, 0, 0));
        patternsByTier[2].Add(new EnemySpawnPattern(2, 0, 1, 1));
        patternsByTier[2].Add(new EnemySpawnPattern(1, 0, 2, 1));
        patternsByTier[2].Add(new EnemySpawnPattern(1, 1, 1, 1));
        patternsByTier[2].Add(new EnemySpawnPattern(1, 0, 0, 3));
        patternsByTier[2].Add(new EnemySpawnPattern(0, 2, 2, 0));
        patternsByTier[2].Add(new EnemySpawnPattern(0, 2, 1, 1));
        //boss tier spawn patterns
    }

    public List<EnemySpawnPattern> GetPatternsOfTier(int tier)
    {
        return patternsByTier[tier];
    }
}

public struct EnemySpawnPattern
{
    private int[] pattern;

    public EnemySpawnPattern(int enemy1Count, int enemy2Count, int enemy3Count, int enemy4Count)
    {
        pattern = new int[]{ enemy1Count, enemy2Count, enemy3Count, enemy4Count };
    }

    public int this[int i]
    {
        get
        {
            return pattern[i];
        }
    }

    public int Size
    {
        get
        {
            return pattern.Length;
        }
    }

    public static EnemySpawnPattern operator *(EnemySpawnPattern first, float second)
    {
        return new EnemySpawnPattern(first[0] * (int)second, first[1] * (int)second, first[2] * (int)second, first[3] * (int)second);
    }
}

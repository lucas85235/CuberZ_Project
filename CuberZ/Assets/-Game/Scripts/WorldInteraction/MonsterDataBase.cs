
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDataBase : MonoBehaviour
{

    public MonsterIndividual[] monster;

    #region Singleton
    public static MonsterDataBase instance { get { return instance_; } }
    private static MonsterDataBase instance_;

    private void Awake()
    {
        instance_ = this;
    }

    #endregion
}

[System.Serializable]
public struct MonsterIndividual
{
    public string monsterName;
    public MonsterBase monster;
    public bool beenSeen;
    //Opção de colocar todos os stats aqui talvez? 
}

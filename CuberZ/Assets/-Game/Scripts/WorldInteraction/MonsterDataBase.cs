
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDataBase : MonoBehaviour
{
    public KubberDex[] kubberDex; 
}

[System.Serializable]
public struct KubberDex
{
    public string monsterName;
    public int identifier;
    public MonsterID monsterID;
    public GameObject monster;
    public bool beenSeen;
    //Opção de colocar todos os stats aqui talvez? 
}

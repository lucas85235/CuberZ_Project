using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCaster : MonoBehaviour
{
    public GameObject[] skills;

    #region Singleton
    public static SkillCaster instance { get { return instance_; } }
    private static SkillCaster instance_;

    private void Awake()
    {
        instance_ = this;
    }
    #endregion


    /// <summary>
    /// Essa função chama o ataque FireBall do tipo Lava
    /// </summary>
    /// <param name="pooling"> Se for falso usa instantiate, verdadeiro usa Pooling  </param> 
    /// <param name="spawnPoint"> Onde irá spawnar o objeto </param> 
    /// <param name="sumWithSpawnPoint"> spawinPoint += sumWithSpawnPoint --> spawnPoint += new Vector3(0,1,1) </param> 
    public void FireBallAttack(Transform spawnPoint, Vector3 sumWithSpawnPoint, bool pooling = false)
    {
        GameObject objTemp = (pooling) ? Pooling.Instantiate(skills[0], spawnPoint.position += sumWithSpawnPoint, spawnPoint.rotation)
             : Instantiate(skills[0], spawnPoint.position + sumWithSpawnPoint, spawnPoint.rotation);
    }


}

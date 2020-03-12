using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawManager : MonoBehaviour
{
    private bool isPolling;

    #region Singleton
    public static SpawManager getInstance;
    
    private void Awake()
    {
        getInstance = this;

        SetPooling(false);
    }
    #endregion

    public void SetPooling(bool choice) 
    {
        isPolling = choice;
    }

    public void SpawObject(GameObject objectSpaw, Transform spawnPoint)
    {
        GameObject objTemp = (isPolling) ? 
               Pooling.Instantiate(objectSpaw, spawnPoint.position, spawnPoint.rotation) : 
               Instantiate(objectSpaw, spawnPoint.position, spawnPoint.rotation);
    }

    public void SpawObject(GameObject objectSpaw, Transform spawnPoint, Vector3 adjustSpawnPoint)
    {
        GameObject objTemp = (isPolling) ? 
               Pooling.Instantiate(objectSpaw, spawnPoint.position += adjustSpawnPoint, spawnPoint.rotation) : 
               Instantiate(objectSpaw, spawnPoint.position + adjustSpawnPoint, spawnPoint.rotation);
    }
}

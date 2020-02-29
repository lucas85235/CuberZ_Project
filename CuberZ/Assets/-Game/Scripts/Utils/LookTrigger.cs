using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookTrigger : MonoBehaviour
{
    private List<GameObject> enemys = new List<GameObject>();
    private CameraProperties camera_;
    private MonsterBase moster_;

    private int currentTargetIndex = 0;
    private bool inTargetMode = false;
    private const string enemyTag = "Enemy";

    public GameObject arrowTargetPrefab;
    public float maxTargetDistance = 22.0f;

    void Start()
    {
        camera_ = Camera.main.GetComponent<CameraProperties>();
        moster_ = GetComponent<MonsterBase>();
    }

    void Update()
    {
        if (moster_.isEnabled)
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                enemys =  NewEnemyList();
                currentTargetIndex = 0;

                if (enemys == null) 
                {
                    Debug.Log("Não Existem Inimigos em Cena!");
                    return;
                }
                else Debug.Log("Existem Inimigos em Cena!");
                
                if (MaxTargetDistance())
                {
                    inTargetMode = !inTargetMode;

                    if (inTargetMode) 
                    {
                        ActiveArrowTarget(enemys[currentTargetIndex]);
                        camera_.SetEnemyTarget(enemys[currentTargetIndex].transform);
                        camera_.SetCameraMode(CameraProperties.CameraMode.TargetEnemy);
                    }
                    else 
                    {
                        DisableArrowTarget();
                        camera_.SetCameraMode(CameraProperties.CameraMode.FollowPlayer);
                    }
                }
                else Debug.Log("Inimigos não estam proximos!");
            } 

            if (Input.GetKeyDown(KeyCode.R) && inTargetMode) 
            {
                DisableArrowTarget();
                enemys =  NewEnemyList();
                NextTarget();
                ActiveArrowTarget(enemys[currentTargetIndex]);
            }

            if (inTargetMode && !MaxTargetDistance()) 
            {
                Debug.Log("Saiu da area de combate!");
                DisableArrowTarget();
                camera_.SetCameraMode(CameraProperties.CameraMode.FollowPlayer);     
                inTargetMode = false;
            }
        }
    }

    private void ActiveArrowTarget(GameObject enemy) 
    {
        GameObject arrow = Instantiate(arrowTargetPrefab, 
            enemy.transform.position + new Vector3 (0, 6.5f, 0),
            Quaternion.identity);
        arrow.transform.parent = enemy.transform;
        arrow.name = "ArrowTarget";
    }

    private void DisableArrowTarget() 
    {
        foreach (var currentEnemy in enemys)
        {
            if (currentEnemy.transform.Find("ArrowTarget") != null) 
            {
                GameObject arrow = currentEnemy.transform.Find("ArrowTarget").gameObject;
                Destroy(arrow);
            }            
        }
    }

    private bool MaxTargetDistance() 
    {
        for (int i = 0; i < enemys.Count; i++) 
        {
            if (Vector3.Distance(transform.position, enemys[i].transform.position) < maxTargetDistance)
                return true;
        }
        return false;
    }

    private void NextTarget() 
    {
        currentTargetIndex++;

        if (currentTargetIndex < enemys.Count)          
            camera_.SetEnemyTarget(enemys[currentTargetIndex].transform);
        else
        {
            currentTargetIndex = 0;
            camera_.SetEnemyTarget(enemys[currentTargetIndex].transform);
        }
    }

    private List<GameObject> NewEnemyList() 
    {
        GameObject[] newEnemyList = GameObject.FindGameObjectsWithTag(enemyTag);
        List<GameObject> returnList = new List<GameObject>();

        foreach (GameObject currentEnemy in newEnemyList)
        {
            if (Vector3.Distance(transform.position, currentEnemy.transform.position) < maxTargetDistance)
            {
                returnList.Add(currentEnemy);
            }
        }

        return returnList;
    }
}

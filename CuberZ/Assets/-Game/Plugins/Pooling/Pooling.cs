using System.Collections.Generic;
using UnityEngine;

public class Pooling : MonoBehaviour
{
    static Dictionary<GameObject, List<GameObject>> ObjectPools = new Dictionary<GameObject, List<GameObject>>();

    public static GameObject SelfDisableObject(GameObject ObjectToPool, float Timer = 0, Vector3 Position = default, Quaternion Rotation = default)
    {
        GameObject obj = InstantiatePooling(ObjectToPool, Position, Rotation);

        if (!obj.GetComponent<PoolingSelfDisable>()) obj.AddComponent<PoolingSelfDisable>();
        else obj.GetComponent<PoolingSelfDisable>().enabled = true;

        obj.GetComponent<PoolingSelfDisable>().PoolingSelfDisableSetTimer(Timer);
        return obj;
    }

    public static GameObject InstantiatePooling(GameObject ObjectToPool, Vector3 Position = default, Quaternion Rotation = default)
    {
        if (ObjectPools.ContainsKey(ObjectToPool))
        {
            foreach (GameObject obj in ObjectPools[ObjectToPool])
            {
                if (obj == null)
                {
                    ObjectPools[ObjectToPool].Remove(obj);
                }
                else if (!obj.activeInHierarchy)
                {
                    obj.transform.position = Position;
                    obj.transform.rotation = Rotation;

                    obj.SetActive(true);
                    return obj;
                }
            }

            ObjectPools[ObjectToPool].Add(Instantiate(ObjectToPool, Position, Rotation));
            return ObjectPools[ObjectToPool][ObjectPools[ObjectToPool].Count - 1];
        }

        ObjectPools.Add(ObjectToPool, new List<GameObject>());

        ObjectPools[ObjectToPool].Add(Instantiate(ObjectToPool, Position, Rotation));
        return ObjectPools[ObjectToPool][ObjectPools[ObjectToPool].Count - 1];
    }
}

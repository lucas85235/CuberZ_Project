using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour
{


    public GameObject kubberCube;
    public Transform spawnPoint;
    private GameObject objT_;
    private GameObject monsterObj_;
    private CaptureSystemNew captureSystemNew_;
    private PlayerController playerController_;
    private InputSystem input_;
    private KubberzInventory kubberzInventory_;
    private PlayerAnimation playerAnimation_;
    private MonsterDataBase monsterDataBase_;
    private CharacterAbstraction characterAbstraction_;
    public bool spawned_ { get; set; }
    private bool spinn_;
    private bool CanSpawnKubberOnWorld() => (input_.RescueKubberInput() && !captureSystemNew_.captureMode_ && playerController_.CanMove_ && !spawned_) ? true : false;
    private bool CanReturnKubber() => (input_.RescueKubberInput() && !captureSystemNew_.captureMode_ && playerController_.CanMove_ && spawned_) ? true : false;
    private void Start()
    {
        captureSystemNew_ = GetComponent<CaptureSystemNew>();
        playerController_ = GetComponent<PlayerController>();
        input_ = FindObjectOfType<InputSystem>();
        kubberzInventory_ = FindObjectOfType<KubberzInventory>();
        playerAnimation_ = FindObjectOfType<PlayerAnimation>();
        monsterDataBase_ = FindObjectOfType<MonsterDataBase>();

    }

    private void Update()
    {
        SpawnKubber();
        if (spinn_) objT_.transform.Rotate(Vector3.forward * Time.deltaTime * -1500);
    }

    private void SpawnKubber()
    {
        if (CanSpawnKubberOnWorld() && playerController_.VerifyLenght())
        {
            playerController_.CanMove_ = false;
            spawned_ = true;
            objT_ = Instantiate(kubberCube, spawnPoint.transform.position, spawnPoint.rotation);
            objT_.transform.SetParent(spawnPoint);
            playerAnimation_.ThrowCubeNear();
        }

        else if (CanReturnKubber() && playerController_.VerifyLenght())
        {
            playerController_.CanMove_ = true;
            objT_.GetComponent<Rigidbody>().velocity = Vector3.zero;
            objT_.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            objT_.transform.SetParent(spawnPoint);
            objT_.transform.position = spawnPoint.transform.position;
            objT_.transform.rotation = spawnPoint.transform.rotation;
            playerAnimation_.CallMoster();
        }
    }

    public void Anim_StartSuckingMan()
    {
        StartCoroutine(StopAnimator());
    }

    public void Anim_ThrowKubberCube()
    {
        objT_.transform.SetParent(null);
        objT_.GetComponent<Rigidbody>().AddForce(playerController_.transform.forward * 10 + new Vector3(0, 4, 0), ForceMode.Impulse);
        spinn_ = true;
        StartCoroutine(KubberCubeAction());
    }

    public void Anim_StopSuckingMan()
    {
        objT_.GetComponent<KubberCube>().suck = false;
    }

    public void Anim_DesactivateMonster()
    {
        monsterObj_.SetActive(false);
        Destroy(objT_);
        playerController_.CanMove_ = true;
        spawned_ = false;
    }

    private IEnumerator KubberCubeAction()
    {

        yield return new WaitForSeconds(1f);
        objT_.GetComponent<KubberCube>().explode = true;
        spinn_ = false;

        GameObject obj = FoundSameIDKubber();

        if (obj)
        {
            monsterObj_ = Instantiate(obj, objT_.transform.position, Quaternion.identity);
            playerController_.SwitchCharacterController(monsterObj_.GetComponent<CharacterAbstraction>());
        }
        yield break;

    }

    private IEnumerator StopAnimator()
    {
        objT_.GetComponent<KubberCube>().suck = true;
        playerController_.GetComponent<Animator>().enabled = false;
        yield return new WaitForSeconds(2.5f);
        playerController_.GetComponent<Animator>().enabled = true;
        yield break;

    }

    private GameObject FoundSameIDKubber()
    {
        for (int i = 0; i < monsterDataBase_.kubberDex.Length; i++)
        {
            if (playerController_.monster[i].GetComponent<MonsterID>().id == monsterDataBase_.kubberDex[i].identifier)
            {
                return monsterDataBase_.kubberDex[i].monster;
            }
        }

        return null;

    }


}


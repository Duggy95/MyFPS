using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.Demo.Cockpit;
using Unity.VisualScripting;

//무기를 관리하기 위한 매니저.
//무기 버리기, 줍기, 새로 얻기, 무기 바꾸기 기능.
public class WeaponManager : MonoBehaviourPun
{
    private static WeaponManager _instance;  //싱글톤
    public GunData[] datas = new GunData[4];  //무기 데이터들
    public Weapon[] weapons = new Weapon[4];  //내가 가지고 있는 무기들. 0 = 주무기, 1 = 보조무기, 2 = 나이프, 3 = 수류탄.
    public Transform[] poses = new Transform[4];  //내가 무기를 가져야 할 위치들. 0 = 주무기, 1 = 보조무기, 2 = 나이프, 3 = 수류탄
    PlayerInput playerInput;  //플레이어 입력
    FireCtrl fireCtrl;  //플레이어가 가지고 있는 발사기능
    public TestPhoton testPhoton;
    public Weapon currWeapon;  //현재 무기
    PhotonView pv;
    Vector3 throwDirection = new Vector3(0, 1, 0);  //던지는 방향

    public static WeaponManager instance  //WeaponManager타입의 변수 instance는 아래 코드대로 _instance를 반환하는 스태틱 매서드 역할.
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                _instance = FindObjectOfType<WeaponManager>();
            }
            // 싱글톤 오브젝트를 반환
            return _instance;
        }
    }

    private void Awake()
    {
        // 씬에 싱글톤 오브젝트가 된 다른 GameManager 오브젝트가 있다면
        if (instance != this)   //주소값으로 비교하는것
        {
            // 자신을 파괴
            Destroy(gameObject);
        }

        pv = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (playerInput != null && playerInput.weapon != 10)
        {
            switch (playerInput.weapon)  //플레이어 입력을 받아서 
            {
                //받은 입력에 따라 무기를 바꿈.
                case 0:
                    //SwitchWeapon(0);
                    Switch(0);
                    //pv.RPC("SwitchWeapon", RpcTarget.Others, 0);
                    break;
                case 1:
                    //SwitchWeapon(1);
                    Switch(1);
                    //pv.RPC("SwitchWeapon", RpcTarget.Others, 1);
                    break;
                case 2:
                    //SwitchWeapon(2);
                    Switch(2);
                    //pv.RPC("SwitchWeapon", RpcTarget.Others, 2);
                    break;
                case 3:
                    //SwitchWeapon(3);
                    Switch(3);
                    //pv.RPC("SwitchWeapon", RpcTarget.Others, 3);
                    break;
            }

            StateCheck(currWeapon);
        }
    }

    public void Switch(int i)
    {
        pv.RPC("SwichRPC", RpcTarget.All, i);
    }

    [PunRPC]
    void SwichRPC(int i)
    {
        if(!pv.IsMine)
        {
            return;
        }

        switch (i)  //플레이어 입력을 받아서 
        {
            //받은 입력에 따라 무기를 바꿈.
            case 0:
                SwitchWeapon(0);
                //pv.RPC("SwitchWeapon", RpcTarget.Others, 0);
                break;
            case 1:
                SwitchWeapon(1);
                //pv.RPC("SwitchWeapon", RpcTarget.Others, 1);
                break;
            case 2:
                SwitchWeapon(2);
                //pv.RPC("SwitchWeapon", RpcTarget.Others, 2);
                break;
            case 3:
                SwitchWeapon(3);
                //pv.RPC("SwitchWeapon", RpcTarget.Others, 3);
                break;
        }

        StateCheck(currWeapon);
    }

    public void PlayerFind()  //맨 처음 플레이어 생성되었을 때 호출되는 메서드
    {
        /*playerInput = testPhoton.player.GetComponent<PlayerInput>();
        fireCtrl = testPhoton.player.GetComponent<FireCtrl>();
        poses = fireCtrl.poses; //플레이어가 가지고 있는 무기 위치들 받아옴.
        for (int i = 0; i < poses.Length; i++)
        {
            //무기 위치들의 자식에 무기로부터 무기가져와서 배열에 넣어줌. 무기를 안들고 있으면 null값 들어갈 것임.
            weapons[i] = poses[i].GetComponentInChildren<Weapon>();
            //photonViews[i] = poses[i].GetComponent<PhotonView>();
        }

        Setting();  //초기 무기값 세팅.
        SwitchWeapon(1);  //보조무기 활성화.*/
        //photonView.RPC("SwitchWeapon", RpcTarget.Others, 1);
        pv.RPC("PvPlayerFind", RpcTarget.All);
    }

    [PunRPC]
    void PvPlayerFind()
    {
        playerInput = testPhoton.player.GetComponent<PlayerInput>();
        fireCtrl = testPhoton.player.GetComponent<FireCtrl>();
        poses = fireCtrl.poses; //플레이어가 가지고 있는 무기 위치들 받아옴.
        for (int i = 0; i < poses.Length; i++)
        {
            //무기 위치들의 자식에 무기로부터 무기가져와서 배열에 넣어줌. 무기를 안들고 있으면 null값 들어갈 것임.
            weapons[i] = poses[i].GetComponentInChildren<Weapon>();
            //photonViews[i] = poses[i].GetComponent<PhotonView>();
        }

        Setting();  //초기 무기값 세팅.
        SwitchWeapon(1);  //보조무기 활성화.
    }

    void Setting()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] == null)  //무기가 null일 때 생략
            {
                continue;
            }
            else  //무기의 정보를 받고 발사 위치 찾기.
            {
                weapons[i].SetWeapon(datas[i]);
                weapons[i].FindFirePos();
            }
        }
    }

    void SwitchWeapon(int weaponIndex)  //현재 무기 바꾸는 메서드.
    {
        if (weapons[weaponIndex] == null)
        {
            return;
        }

        for (int i = 0; i < weapons.Length; i++)
        {
            //인수로 받은 무기외에는 전부 비활성화. 만약 무기가 null이면 비활성화.
            if (weapons[i] != null)
            {
                bool isActive = (i == weaponIndex) ? true : false;
                poses[i].gameObject.SetActive(isActive);
            }
            else
            {
                poses[i].gameObject.SetActive(false);
            }
        }

        //현재무기 설정. fireCtrl에 전달.
        currWeapon = weapons[weaponIndex];
        fireCtrl.weapon = currWeapon;
        fireCtrl.gunAnim = currWeapon.anim;
        fireCtrl.lineRenderer.material = currWeapon.fireMaterial;
    }

    public void StateCheck(Weapon currWeapon)  //현재 상태 체크 메서드.
    {
        if (currWeapon.weaponType <= 1)  //주 무기일 때
        {
            if (currWeapon.magAmmo <= 0)  //총알이 없으면 Empty. => 발사 불가능
            {
                fireCtrl.state = FireCtrl.State.Empty;
            }
            else if (currWeapon.magAmmo >= 1 && fireCtrl.state != FireCtrl.State.Reloading)  //총알이 있고, 장전중이 아니라면 공격가능 상태.
            {
                fireCtrl.state = FireCtrl.State.Ready;
            }
        }
        else  //주 무기 아니면 항상 공격가능 상태
        {
            fireCtrl.state = FireCtrl.State.Ready;
        }
    }

    public void GetWeapon(Weapon weapon)  //무기를 줍는 메서드.
    {
        //무기 생성해서 pos의 자식으로 넣어줌.
        GameObject gun = PhotonNetwork.Instantiate(weapon.weaponName, Vector3.zero,
                                                                                                     Quaternion.identity);
        gun.transform.SetParent(poses[weapon.weaponType]);
        gun.transform.localPosition = Vector3.zero;
        gun.transform.localEulerAngles = Vector3.zero;

        //새로 생성한 총에다가 주운 총의 정보들을 넣어줌.
        Weapon _weapon = gun.GetComponent<Weapon>();
        _weapon.ShareInfo(weapon);
        _weapon.FindFirePos();
        weapons[weapon.weaponType] = _weapon;  //무기 배열에 정보 넣어줌.

        SwitchWeapon(weapon.weaponType);  //주운 총을 현재 총으로.
    }

    public void BuyWeapon(GunData gunData)
    {
        GameObject gun = PhotonNetwork.Instantiate(gunData.gunName, Vector3.zero, Quaternion.identity);

        gun.transform.SetParent(poses[gunData.weaponType]);
        gun.transform.localPosition = Vector3.zero;
        gun.transform.localEulerAngles = Vector3.zero;

        Weapon _weapon = gun.GetComponent<Weapon>();
        _weapon.SetWeapon(gunData);
        _weapon.FindFirePos();
        weapons[_weapon.weaponType] = _weapon;

        SwitchWeapon(_weapon.weaponType);
    }

    public void RemoveWeapon(Weapon weapon)  //무기를 지우는 메서드.
    {
        if (weapon.weaponType == 0 || weapon.weaponType == 3)  //주 무기, 수류탄에만 씀.
        {
            //무기 오브젝트 삭제하고, 현재 무기를 권총으로
            Destroy(weapons[weapon.weaponType].gameObject);
            weapons[weapon.weaponType] = null;

            SwitchWeapon(1);
        }
        else
            return;
    }

    public void ThrowWeapon(Weapon weapon)  //무기 던지기.(버리기)
    {
        if (weapon.weaponType != 0)  //주 무기만 가능.
            return;

        //생성 위치와 회전값
        Vector3 spawnPosition = weapon.transform.position + Camera.main.transform.forward;
        Vector3 spawnRotation = fireCtrl.transform.rotation.eulerAngles + new Vector3(0, -90, -90);

        //던지기 전용 오브젝트 생성. 
        GameObject _weaponDrop = PhotonNetwork.Instantiate(weapon.weaponName + "_Drop", spawnPosition, Quaternion.Euler(spawnRotation));
        //현재 무기의 정보를 던진 무기에 전해줌.
        Weapon _weapon = _weaponDrop.GetComponent<Weapon>();
        _weapon.anim.enabled = false;
        _weapon.ShareInfo(weapon);

        //리지드바디를 추가해서 앞으로 날림.
        Rigidbody rb = _weapon.gameObject.AddComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        Vector3 ThrowDirection = (Camera.main.transform.forward + throwDirection).normalized;
        rb.AddForce(ThrowDirection * 3, ForceMode.VelocityChange);
    }
}

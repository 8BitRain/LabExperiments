using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXController : MonoBehaviour
{
    public enum VFX{Attack1,Attack2,Attack3,Attack4,Hit,Jump,Dodge,TakeHit,Block,HitShield}

    [Header("VFX")]
    public GameObject VFXAttack1;
    public GameObject VFXAttack2;
    public GameObject VFXAttack3;
    public GameObject VFXAttack4;
    public GameObject VFXHit;
    public GameObject VFXTakeHit;
    public GameObject VFXDodge;
    public GameObject VFXJump;
    public GameObject VFXBlock;
    public GameObject VFXHitShield;

    [Header("VFX Spawn Position")]
    public Transform VFXAttack1Spawn;
    public Transform VFXAttack2Spawn;
    public Transform VFXAttack3Spawn;
    public Transform VFXAttack4Spawn;
    public Transform VFXHitSpawn;
    public Transform VFXTakeHitSpawn;
    public Transform VFXDodgeSpawn;
    public Transform VFXJumpSpawn;
    public Transform VFXBlockSpawn;

    // Start is called before the first frame update
    private VFX currVFX;
    private GameObject currentVFXInstance;
    private ParticleSystem[] _vfxQueue;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //Playing VFX from script
    public void PlayVFX(VFX vfx, Transform parent, Vector3 position, Vector3 rotation, float keepAlive = 0)
    {
        switch (vfx)
        {
            case VFX.Hit:
                if (VFXHit != null) Instantiate(VFXHit, position, Quaternion.Euler(rotation.x, rotation.y, rotation.z));
                currentVFXInstance = null;
                break;
            case VFX.TakeHit:
                if (VFXTakeHit != null) Instantiate(VFXHit, position, Quaternion.Euler(rotation.x, rotation.y, rotation.z));
                currentVFXInstance = null;
                break;
            case VFX.Jump:
                if (VFXJump != null) Instantiate(VFXHit, position, Quaternion.Euler(rotation.x, rotation.y, rotation.z));
                currentVFXInstance = null;
                break;
            case VFX.Dodge:
                if (VFXDodge != null) Instantiate(VFXHit, position, Quaternion.Euler(rotation.x, rotation.y, rotation.z));
                currentVFXInstance = null;
                break;
            case VFX.Block:
                if (VFXBlock != null)
                {
                    GameObject vfxInstance = Instantiate(VFXBlock, VFXBlockSpawn.position, Quaternion.Euler(rotation.x, rotation.y, rotation.z));

                    if(parent != null){
                        vfxInstance.transform.SetParent(parent);
                    }
                    currVFX = VFX.Block;
                    currentVFXInstance = vfxInstance;
                    if(keepAlive != 0)
                    {
                        Destroy(vfxInstance, keepAlive);
                    }

                    
                }

                break;
            case VFX.HitShield:
                if(VFXHitShield != null)
                {
                    GameObject vfxInstance = Instantiate(VFXHitShield, position, Quaternion.Euler(rotation.x, rotation.y, rotation.z));
                    if(keepAlive != 0)
                    {
                        Destroy(vfxInstance, keepAlive);
                    }
                    currentVFXInstance = null;
                }
                break;
            default:
                break;
        }
    }

    //Playing VFX from animation
    public void PlayAnimationTriggeredVFX(string vfx)
    {
        VFX parsedEnumVFX = (VFX)System.Enum.Parse( typeof(VFX), vfx);
        switch (parsedEnumVFX)
        {
            case VFX.Attack1:
                if (VFXHit != null)
                {
                    GameObject vfxInstance = Instantiate(VFXAttack1, VFXAttack1Spawn.position, VFXAttack1Spawn.rotation);
                    vfxInstance.transform.SetParent(VFXHitSpawn);
                }
                break;
            case VFX.Attack2:
                if (VFXHit != null)
                {
                    GameObject vfxInstance = Instantiate(VFXAttack2, VFXAttack2Spawn.position, VFXAttack2Spawn.rotation);
                    vfxInstance.transform.SetParent(VFXHitSpawn);
                }
                break;
            case VFX.Attack3:
                if (VFXHit != null)
                {
                    GameObject vfxInstance = Instantiate(VFXAttack3, VFXAttack3Spawn.position, VFXAttack3Spawn.rotation);
                    vfxInstance.transform.SetParent(VFXHitSpawn);
                }
                break;
            case VFX.Attack4:
                if (VFXHit != null)
                {
                    GameObject vfxInstance = Instantiate(VFXAttack4, VFXAttack4Spawn.position, VFXAttack4Spawn.rotation);
                    vfxInstance.transform.SetParent(VFXHitSpawn);
                }
                break;
            case VFX.Hit:
                if (VFXHit != null)
                {
                    GameObject vfxInstance = Instantiate(VFXHit, VFXHitSpawn.position, VFXHitSpawn.rotation);
                    vfxInstance.transform.SetParent(VFXHitSpawn);
                }
                break;
            case VFX.TakeHit:
                if (VFXTakeHit != null) Instantiate(VFXTakeHit, VFXTakeHitSpawn.position, VFXTakeHitSpawn.rotation);
                break;
            case VFX.Jump:
                if (VFXJump != null) Instantiate(VFXJump, VFXJumpSpawn.position, VFXJumpSpawn.rotation);
                break;
            case VFX.Dodge:
                if (VFXDodge != null) Instantiate(VFXDodgeSpawn, VFXDodgeSpawn.position, VFXDodgeSpawn.rotation);
                break;
            default:
                break;
        }
    }

    public void DestroyCurrentVFX(VFX vfx)
    {
        if(currVFX == vfx)
        {
            Destroy(currentVFXInstance);
        }
    }
}

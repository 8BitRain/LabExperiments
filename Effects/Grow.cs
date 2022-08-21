using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Grow : MonoBehaviour
{
    public float scaleX;
    public float scaleY;
    public float scaleZ;
    public float scaleOverTime;
    public bool destroyOnComplete = false;
    // Start is called before the first frame update
    void Start()
    {
        this.transform.DOScale(new Vector3(scaleX, scaleY, scaleZ), scaleOverTime).OnComplete(() => {
            if(destroyOnComplete)
            {
                transform.GetComponent<Renderer>().material.DOFade(0f, 5f);
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

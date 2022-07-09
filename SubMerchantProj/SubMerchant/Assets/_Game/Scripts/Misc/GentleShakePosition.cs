using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GentleShakePosition : MonoBehaviour
{
    Vector3 orgPos;
    public float maxShift = 1;
    public float shiftDuration = 1;
    public AnimationCurve shiftCurve;
    float normTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        orgPos = transform.position;
        StartCoroutine(PositionShake());
    }

    
    IEnumerator PositionShake()
    {
        float timer = 0;
        //float normTime;
        Vector3 targetPos, lastPos;
        targetPos = GetRandomShiftPos();
        lastPos = orgPos;

        while(true)
        {
            timer += Time.deltaTime;
            
            if(timer >= shiftDuration)
            {
                timer = 0;
                lastPos = transform.position;
                targetPos = GetRandomShiftPos();
            }
            
            normTime = Mathf.InverseLerp(0, shiftDuration, timer);


            transform.position = Vector3.Lerp(lastPos, targetPos, shiftCurve.Evaluate(normTime));


            yield return null; 
        }
    }

    Vector3 GetRandomShiftPos()
    {
        Vector3 shiftedPos;
        shiftedPos.x = orgPos.x + Random.Range(-maxShift,maxShift);
        shiftedPos.y = orgPos.y + Random.Range(-maxShift,maxShift);
        shiftedPos.z = orgPos.z + Random.Range(-maxShift,maxShift);

        return shiftedPos;
    }

}


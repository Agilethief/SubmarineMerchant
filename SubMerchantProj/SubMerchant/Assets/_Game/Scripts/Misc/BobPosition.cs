using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobPosition : MonoBehaviour
{
    public AnimationCurve Ycurve;
    public float YMultiplier = 1;
    public float speed = 1;

    bool bobbing = true;
    float timer;
    Vector3 tempVec, startPos;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        startPos = transform.localPosition;

        while (bobbing)
        {
            tempVec = startPos;
            tempVec.y = startPos.y + Ycurve.Evaluate(timer) * YMultiplier;

            transform.localPosition = tempVec;

            timer += Time.deltaTime * speed;
            if (timer > 1)
                timer = 0;

            yield return null;
        }


        yield return null;
    }


}

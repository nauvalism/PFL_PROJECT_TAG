using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharBomb : MonoBehaviour
{
    [SerializeField] BaseChar coreChar;
    [SerializeField] Animator anim;
    [SerializeField] AnimatorClips clips;
    [SerializeField] bool bombed;
    [SerializeField] int maxCountDown;
    [SerializeField] int countDown;
    [SerializeField] Transform bombRoot;
    [SerializeField] Transform countDownScale;
    [SerializeField] TextMeshPro countDownTxt;

    private void Update() {

        if(countDownTxt.transform.localScale.x > 1)
        {
            countDownTxt.transform.localScale = new Vector3(countDownTxt.transform.localScale.x - Time.deltaTime, countDownTxt.transform.localScale.y - Time.deltaTime, 0);
        }
    }

    public void UpDownScaleEffect()
    {
        float scale = maxCountDown - countDown;
        scale = (Mathf.Sin(Time.time * 2.0f)) + 1.5f;
        
        
        countDownScale.localScale = new Vector3(scale,scale,scale);
        
    }

    public void GiveBomb()
    {
        StopCoroutine("CountingDown");
        bombed = false;
        countDown = maxCountDown;
        HideBomb();
    }

    public void ReceiveBomb()
    {
        bombed = true;
        countDown = maxCountDown;
        //StopCoroutine("CountingDown");
        ShowBomb();
        StartCountingDown();
    }

    public void StartCountingDown()
    {
        
        StopCoroutine("CountingDown");
        StartCoroutine("CountingDown");
        
    }

    IEnumerator CountingDown()
    {
        countDown = maxCountDown;
        for(int i = maxCountDown ; i > 0 ; i--)
        {
            countDown = i;
            UpdateText();
            yield return new WaitForSeconds(1);
        }

        Explode();
    }

    public void UpdateText()
    {
        countDownTxt.text = countDown.ToString();
        countDownTxt.transform.localScale = new Vector3(1.5f,1.5f,1.5f);
    }

    public void ShowBomb()
    {
        LeanTween.cancel(bombRoot.gameObject);
        bombRoot.transform.localScale = Vector3.one;
        LeanTween.scale(bombRoot.gameObject, new Vector3(1.2f,1.2f,1.25f), 1.5f).setEase(LeanTweenType.punch);
        anim.Play("BombTss", -1, 0);
    }

    public void HideBomb()
    {
        LeanTween.cancel(bombRoot.gameObject);
        bombRoot.transform.localScale = Vector3.one;
        LeanTween.scale(bombRoot.gameObject, Vector3.zero, 0.25f).setEase(LeanTweenType.easeOutQuad);
        anim.Play("Idle");
    }

    public void Explode()
    {
        //LOCAL
        HideBomb();
        coreChar.Explode();
        
        //MULTI
    }



    public bool GetBombed()
    {
        return bombed;
    }
}

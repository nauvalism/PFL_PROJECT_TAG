using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharRunBar : MonoBehaviour
{
    [SerializeField] Transform rootCharRunBar;
    [SerializeField] SpriteRenderer runBar;
    [SerializeField] float barDefaultValue;
    [SerializeField] float tresholdPercentage = 20;
    [SerializeField] float tresholdValue = 20;
    [SerializeField] float maxValue = 100;
    [SerializeField] float runBarPerSecond = 20;
    [SerializeField] float runBarRecovery = 15;
    [SerializeField] float currentValue = 9;
    [SerializeField] float convertedBarValue = 0;
    [SerializeField] bool running = false;
    
    private void OnValidate() {
        barDefaultValue = runBar.size.x;
        currentValue = maxValue;
    }

    public void AddDetail(float maxRunValue, float runRate, float runRecoveryRate)
    {
        this.maxValue = maxRunValue;
        this.runBarPerSecond = runRate;
        this.runBarRecovery = runRecoveryRate;
        this.tresholdValue = maxValue * (tresholdPercentage / 100);
    }

    public void Run()
    {
        this.running = true;
        ShowRunBar();
    }

    public void NotRun()
    {
        this.running = false;
        HideRunBar();
    }

    private void Update() {
        if(!running)
        {
            currentValue = currentValue + Time.deltaTime * runBarRecovery;
        }
        else
        {
            currentValue = currentValue - Time.deltaTime * runBarPerSecond;
        }

        currentValue = Mathf.Clamp(currentValue, 0, maxValue);
        
        convertedBarValue = currentValue * (barDefaultValue / maxValue);
        
        convertedBarValue = Mathf.Clamp(convertedBarValue, 0, barDefaultValue);
    
        runBar.size = new Vector2(convertedBarValue, runBar.size.y);
    }

    public void ShowRunBar()
    {
        LeanTween.cancel(rootCharRunBar.gameObject);
        rootCharRunBar.transform.localScale = Vector3.one;
        LeanTween.scaleY(rootCharRunBar.gameObject, 1.5f, 1.0f).setEase(LeanTweenType.punch);

    }

    public void HideRunBar()
    {
        LeanTween.cancel(rootCharRunBar.gameObject);

        LeanTween.scaleY(rootCharRunBar.gameObject, .0f, 0.125f).setDelay(2.0f).setEase(LeanTweenType.easeOutQuad);
    }

    public bool AvailableToRun()
    {
        if(currentValue < tresholdValue)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public float GetRunBarValue()
    {
        return currentValue;
    }

}

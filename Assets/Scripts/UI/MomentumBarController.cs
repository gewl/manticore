using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MomentumBarController : MonoBehaviour {

    [SerializeField]
    float momentumBarAdjustmentTime;
    AnimationCurve momentumTransitionCurve { get { return GameManager.BelovedSwingCurve; } }
    [SerializeField]
    Image momentumBar;

    CoroutineQueue momentumBarTransitionsQueue;

    float percentageProgressToNextMomentum = 0f;
    int currentTotalMomentumPoints = 0;
    
    float momentumBarXPos, momentumBarYPos, maximumMomentumBarWidth, momentumBarHeight;

    private void Awake()
    {
        momentumBarTransitionsQueue = new CoroutineQueue(this);

        maximumMomentumBarWidth = momentumBar.rectTransform.sizeDelta.x;
        momentumBarHeight = momentumBar.rectTransform.sizeDelta.y;
        percentageProgressToNextMomentum = (float)MomentumManager.CurrentMomentumData.ProgressTowardNextMomentum / (float)MomentumManager.CurrentMomentumData.MomentumRequiredForNextPoint;
        currentTotalMomentumPoints = MomentumManager.CurrentMomentumData.TotalMomentumPoints;

        momentumBarTransitionsQueue.EnqueueAction(UpdateMomentumBarProgress());
    }

    void OnEnable()
    {
        MomentumManager.OnMomentumUpdated += OnMomentumChangedHandler;
    }

    void OnDisable()
    {
        MomentumManager.OnMomentumUpdated -= OnMomentumChangedHandler;
    }

    void OnMomentumChangedHandler(MomentumData momentumData)
    {
        // Both the following visually represent gaining/losing an entire point. 
        while (momentumData.TotalMomentumPoints > currentTotalMomentumPoints)
        {
            percentageProgressToNextMomentum = 0;
            currentTotalMomentumPoints++;

            momentumBarTransitionsQueue.EnqueueAction(FillAndResetBar());
        }
        while (momentumData.TotalMomentumPoints < currentTotalMomentumPoints)
        {
            percentageProgressToNextMomentum = 0;
            currentTotalMomentumPoints--;

            momentumBarTransitionsQueue.EnqueueAction(EmptyAndResetBar());
        }

        float updatedPercentageProgress = (float)momentumData.ProgressTowardNextMomentum / (float)momentumData.MomentumRequiredForNextPoint;

        if (updatedPercentageProgress != percentageProgressToNextMomentum)
        {
            percentageProgressToNextMomentum = updatedPercentageProgress;

            momentumBarTransitionsQueue.EnqueueAction(UpdateMomentumBarProgress());
        }
    }

    IEnumerator UpdateMomentumBarProgress()
    {
        float initialWidth = momentumBar.rectTransform.sizeDelta.x;
        float targetWidth = percentageProgressToNextMomentum * maximumMomentumBarWidth;

        float timeElapsed = 0.0f;

        while (timeElapsed < momentumBarAdjustmentTime)
        {
            timeElapsed += Time.deltaTime;
            float percentageComplete = timeElapsed / momentumBarAdjustmentTime;

            float progressCurveEvaluation = momentumTransitionCurve.Evaluate(percentageComplete);
            float updatedWidth = Mathf.Lerp(initialWidth, targetWidth, progressCurveEvaluation);

            momentumBar.rectTransform.sizeDelta = new Vector2(updatedWidth, momentumBarHeight);
            yield return null;
        }

        yield break;
    }

    IEnumerator FillAndResetBar()
    {
        float initialWidth = momentumBar.rectTransform.sizeDelta.x;
        float targetWidth = maximumMomentumBarWidth;

        float timeElapsed = 0.0f;

        while (timeElapsed < momentumBarAdjustmentTime)
        {
            timeElapsed += Time.deltaTime;
            float percentageComplete = timeElapsed / momentumBarAdjustmentTime;

            float progressCurveEvaluation = momentumTransitionCurve.Evaluate(percentageComplete);
            float updatedWidth = Mathf.Lerp(initialWidth, targetWidth, progressCurveEvaluation);

            momentumBar.rectTransform.sizeDelta = new Vector2(updatedWidth, momentumBarHeight);
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        momentumBar.rectTransform.sizeDelta = new Vector2(0f, momentumBarHeight);

        yield break;
    }

    IEnumerator EmptyAndResetBar()
    {
        float initialWidth = momentumBar.rectTransform.sizeDelta.x;
        float targetWidth = 0f;

        float timeElapsed = 0.0f;

        while (timeElapsed < momentumBarAdjustmentTime)
        {
            timeElapsed += Time.deltaTime;
            float percentageComplete = timeElapsed / momentumBarAdjustmentTime;

            float progressCurveEvaluation = momentumTransitionCurve.Evaluate(percentageComplete);
            float updatedWidth = Mathf.Lerp(initialWidth, targetWidth, progressCurveEvaluation);

            momentumBar.rectTransform.sizeDelta = new Vector2(updatedWidth, momentumBarHeight);
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        momentumBar.rectTransform.sizeDelta = new Vector2(maximumMomentumBarWidth, momentumBarHeight);

        yield break;
    }
}

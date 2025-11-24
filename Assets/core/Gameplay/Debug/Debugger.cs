using Core;
using Gameplay.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Debugger : MonoBehaviour
{
    [Header("References")]
    public Slider valueSlider;
    public TextMeshProUGUI outputText;
    public BallController ball;
    public Toggle debugToggle; 

    [Header("Target Range")]
    private const float MinTargetValue = 5f;
    private const float MaxTargetValue = 30f;

    [Tooltip("The actual value being controlled, mapped from slider (0-1).")]
    public float controlledValue;


    void Start()
    {
        GameManager.Instance.IsDebugMode = true;


        if (valueSlider == null)
        {
            Debug.LogError("Slider reference is not set on " + gameObject.name);
            enabled = false;
            return;
        }

        if (debugToggle == null)
        {
            Debug.LogError("Toggle reference is not set on " + gameObject.name);
            enabled = false;
            return;
        }
        GameManager.Instance.IsDebugMode = true; 

        OnSliderValueChanged(valueSlider.value);

        valueSlider.onValueChanged.AddListener(OnSliderValueChanged);

      
        debugToggle.onValueChanged.AddListener(OnToggleValueChanged);

    
        OnToggleValueChanged(debugToggle.isOn);
    }

    public void OnSliderValueChanged(float sliderValueNormalized)
    {
        controlledValue = Mathf.Lerp(MinTargetValue, MaxTargetValue, sliderValueNormalized);

    
        ball.moveSpeed = controlledValue;
     

        if (outputText != null)
        {
            outputText.text = "Ball Speed: " + (int)controlledValue;
        }
    }

    public void OnToggleValueChanged(bool isOn)
    {

        if (isOn)
        {

            GameManager.Instance.IsDebugMode = true;
        
        }
        else
        {
            GameManager.Instance.IsDebugMode = false;

        }
    }

    private void OnDestroy()
    {
        if (valueSlider != null)
        {
            valueSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }

        if (debugToggle != null)
        {
            debugToggle.onValueChanged.RemoveListener(OnToggleValueChanged);
        }
    }
}

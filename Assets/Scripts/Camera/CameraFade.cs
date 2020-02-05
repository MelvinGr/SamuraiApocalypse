using UnityEngine;

public class CameraFade : MonoBehaviour
{
    public delegate void OnFadeCompletedDelegate();

    GUIStyle _BackgroundStyle = new GUIStyle();
    Texture2D _FadeTexture; 
    Color _CurrentScreenOverlayColor = new Color(0, 0, 0, 0); 
    Color _TargetScreenOverlayColor = new Color(0, 0, 0, 0); 
    Color _DeltaColor = new Color(0, 0, 0, 0);  
    int _FadeGUIDepth = -1000;

    bool _isFadingIn = false;

    public OnFadeCompletedDelegate OnFadeInCompleted = null;
    public OnFadeCompletedDelegate OnFadeOutCompleted = null;

    public float fadeDurationSec = 5;
    public bool fadeInOnStart = true;

    void Awake()
    {
        _FadeTexture = new Texture2D(1, 1);
        _BackgroundStyle.normal.background = _FadeTexture;
        SetScreenOverlayColor(_CurrentScreenOverlayColor);
    }

    void Start()
    {
        if (fadeInOnStart)
            FadeIn();
    }

    void OnGUI()
    {
		// blijf faden als de kleuren nog niet gelijk zijn
        if (_CurrentScreenOverlayColor != _TargetScreenOverlayColor)
        {
            if (Mathf.Abs(_CurrentScreenOverlayColor.a - _TargetScreenOverlayColor.a) < Mathf.Abs(_DeltaColor.a) * Time.deltaTime)
            {
                _CurrentScreenOverlayColor = _TargetScreenOverlayColor;
                SetScreenOverlayColor(_CurrentScreenOverlayColor);
                _DeltaColor = new Color(0, 0, 0, 0);

                if (_isFadingIn)
                {
                    if (OnFadeInCompleted != null)
                        OnFadeInCompleted();
                }
                else
                {
                    if (OnFadeOutCompleted != null)
                        OnFadeOutCompleted();
                }
            }
            else
                SetScreenOverlayColor(_CurrentScreenOverlayColor + _DeltaColor * Time.deltaTime);
        }

		// teken alleen de texture als apha > 0
        if (_CurrentScreenOverlayColor.a > 0)
        {
            GUI.depth = _FadeGUIDepth;
            GUI.Label(new Rect(-10, -10, Screen.width + 10, Screen.height + 10), _FadeTexture, _BackgroundStyle);
        }
    }

    void SetScreenOverlayColor(Color newScreenOverlayColor)
    {
        _CurrentScreenOverlayColor = newScreenOverlayColor;
        _FadeTexture.SetPixel(0, 0, _CurrentScreenOverlayColor);
        _FadeTexture.Apply();
    }

    void StartFade(Color newScreenOverlayColor, float fadeDuration)
    {
        if (fadeDuration <= 0.0f) 
            SetScreenOverlayColor(newScreenOverlayColor);
        else   
            _TargetScreenOverlayColor = newScreenOverlayColor;
            _DeltaColor = (_TargetScreenOverlayColor - _CurrentScreenOverlayColor) / fadeDuration;
    }

    public void FadeIn()
    {
        _isFadingIn = true;
        SetScreenOverlayColor(new Color(0, 0, 0));
        StartFade(new Color(0, 0, 0, 0), fadeDurationSec);
    }

    public void FadeOut()
    {
        _isFadingIn = false;
        SetScreenOverlayColor(new Color(0, 0, 0, 0));
        StartFade(new Color(0, 0, 0, 1), fadeDurationSec);
    }
}
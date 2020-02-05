using UnityEngine;
using System.Collections;

public class Combo : MonoBehaviour
{
    public float startTime;
    public float durationTime;

    public void SetActive(Vector2 position, float startTime, float duration)
    {
        gameObject.transform.localPosition = new Vector3(position.x, position.y);
        this.startTime = startTime;
        this.durationTime = duration;

        gameObject.active = true;
    }

    void Update()
    {
        if (!gameObject.active)
            return;

        float precentage = (Time.time - startTime) / durationTime * 100;
        guiTexture.color = new Color(0.5f, 0.5f, 0.5f, 1f - (precentage / 100));

        if (precentage >= 100f)
        {
            guiTexture.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            gameObject.active = false;
        }
    }
};
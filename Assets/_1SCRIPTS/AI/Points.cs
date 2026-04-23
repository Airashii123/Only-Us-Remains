using TMPro;
using UnityEngine;

public class Points : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI pointertext;

    public float score = 0;

    public float timePenalty = -0.1f;

    void Update()
    {
        AddScore(timePenalty * Time.deltaTime);
        pointertext.text = string.Format("Points: {00}",score);
    }

    public void AddScore(float value)
    {
        score += value;
    }

    public void ResetScore()
    {
        score = 0;
    }
}
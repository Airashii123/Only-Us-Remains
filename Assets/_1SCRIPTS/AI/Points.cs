using TMPro;
using UnityEngine;

public class Points : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pointertext;

    public float score = 0f;
    public float timePenalty = -0.1f;

    void Update()
    {
        AddScore(timePenalty * Time.deltaTime);

        if (pointertext != null)
        {
            pointertext.text = "Points: " + Mathf.RoundToInt(score);
        }
    }

    public void AddScore(float value)
    {
        score += value;
    }

    public void ResetScore()
    {
        score = 0f;
    }
}
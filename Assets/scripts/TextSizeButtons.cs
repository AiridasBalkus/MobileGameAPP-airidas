using UnityEngine;

public class TextSizeButtons : MonoBehaviour
{
    public void SetSmall() => TextScale.Factor = 0.85f;
    public void SetNormal() => TextScale.Factor = 1.0f;
    public void SetLarge() => TextScale.Factor = 1.25f;
}

using UnityEngine;
using UnityEngine.UI;

public class SliderValue : MonoBehaviour
{
    public string variable;
    public Text textValue;

    void OnEnable()
    {
        GetComponent<Slider>().value=PlayerPrefs.GetFloat(variable, 50);
        if(textValue!=null)
        {
            textValue.text = GetComponent<Slider>().value.ToString();
        }
    }

    public void UpdateValue()
    {
        PlayerPrefs.SetFloat(variable, GetComponent<Slider>().value);
        if (textValue != null)
        {
            textValue.text = GetComponent<Slider>().value.ToString();
        }
    }
}

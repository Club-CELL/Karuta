using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TrialParamButton : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private string[] values;
    [SerializeField] private int startIndex;
    
    [Header("Dependancies")]
    [SerializeField] private Text paramText;
    
    private int valueIndex;

    public int Param()
    {
        if (valueIndex == values.Length - 1) return int.MinValue;
        else return int.Parse(values[valueIndex]);
    }

    void Start()
    {
        valueIndex = startIndex;
        paramText.text = values[valueIndex];
    }

    public void Add()
    {
        if (valueIndex >= values.Count() - 1) return;

        valueIndex++;
        paramText.text = values[valueIndex];
    }

    public void Remove()
    {
        if (valueIndex <= 0) return;

        valueIndex--;
        paramText.text = values[valueIndex];
    }
}

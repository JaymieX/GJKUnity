using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public InputField TX, TY, TZ;
    public InputField RX, RY, RZ;
    public InputField IX, IY, IZ;

    public GameObject ObjectA, ObjectB;

    private void SetGameObjectValues(ref GameObject go)
    {
        Vector3 oldTraslate = go.transform.position;
        go.transform.position =
            new Vector3
            (GetTextValue(TX, oldTraslate.x), GetTextValue(TY, oldTraslate.y), GetTextValue(TZ, oldTraslate.z));

        Vector3 oldRot = go.transform.eulerAngles;
        go.transform.eulerAngles =
            new Vector3
            (GetTextValue(RX, oldRot.x), GetTextValue(RY, oldRot.y), GetTextValue(RZ, oldRot.z));
    }

    private float GetTextValue(InputField field, float fallBackValue)
    {
        string text = field.text.Trim();

        if (text == string.Empty)
        {
            return fallBackValue;
        }

        if (float.TryParse(text, out float res))
        {
            return res;
        }

        return fallBackValue;
    }

    public void SetValueA()
    {
        SetGameObjectValues(ref ObjectA);
        SimulationManager.RenewState();
    }

    public void SetValueB()
    {
        SetGameObjectValues(ref ObjectB);
        SimulationManager.RenewState();
    }

    public void FixedUpdate()
    {
        SimulationManager.initDirection.x = GetTextValue(IX, 0f);
        SimulationManager.initDirection.y = GetTextValue(IY, 0f);
        SimulationManager.initDirection.z = GetTextValue(IZ, 0f);
    }
}

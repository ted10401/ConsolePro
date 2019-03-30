using UnityEngine;

public class UnitTesting_ConsolePro : MonoBehaviour
{
    private void Update()
    {
        if(Input.GetKey(KeyCode.Alpha1))
        {
            Debug.Log("Log");
        }

        if (Input.GetKey(KeyCode.Alpha2))
        {
            Debug.LogWarning("Warning");
        }

        if (Input.GetKey(KeyCode.Alpha3))
        {
            Debug.LogError("Error");
        }

        if (Input.GetKey(KeyCode.Alpha4))
        {
            Debug.LogAssertion("Assetion");
        }

        if (Input.GetKey(KeyCode.Alpha5))
        {
            Debug.LogException(new System.Exception("Exception"));
        }
    }
}

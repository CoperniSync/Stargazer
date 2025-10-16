using UnityEngine;

public class UnityInterface : MonoBehaviour
{
    //class that holds the core game loop

    Core core;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        core = new Core();
    }

    // Update is called once per frame
    void Update()
    {
        core.Update(Time.deltaTime);
    }
}

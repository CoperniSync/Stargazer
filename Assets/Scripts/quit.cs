using UnityEngine;

namespace Assets.Scripts
{
    public class quit : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetAxis("Exit") != 0)
            {
                Debug.Log("aaa");
                Application.Quit();
            }
        }
    }
}
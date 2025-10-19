using Assets.Scripts.UI;
using System;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
// Morgan Hendon 10/17/2025
public class CameraControls : MonoBehaviour
{
    /// <summary>
    /// Class for controlling the rotation of the camera
    /// </summary>

    // Speed to rotate the camera x axis in degrees per second times the field of view
    public float verticalRotationSpeed = 1f;

    // Speed to rotate the camera y axis in degrees per second times the field of view
    public float horizontalRotationSpeed = 1f;

    //Speed to change the zoom of the camera's field of view. 
    public float zoomSpeed = 1.5f;

    //max FOV
    public float zoomMax = 160;

    //min FOV
    public float zoomMin = 10;


    InputContainer inputs = InputContainer.Container;

    private float xAxisDegrees;
    private float yAxisDegrees;
    private float fieldOfView;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        fieldOfView = transform.GetComponent<Camera>().fieldOfView;
        xAxisDegrees = 0;
        yAxisDegrees = 0;
        UpdateInputContaner();
    }

    // Update is called once per frame
    void Update()
    {

        bool cameraHasChanged = false;
        //flag for if the camera has changed at all

        //get user camera inputs
        float horizontalChange = Input.GetAxis("Horizontal") *horizontalRotationSpeed *Time.deltaTime * fieldOfView;
        float verticalChange = -(Input.GetAxis("Vertical") *verticalRotationSpeed * Time.deltaTime * fieldOfView);
        //float tiltChange = -(Input.GetAxis("Tilt") * TILT_ROTATION_SPEED * Time.deltaTime);
        float zoomChange = -(Input.GetAxis("Zoom") * zoomSpeed * Time.deltaTime * fieldOfView);

        if (horizontalChange != 0 || verticalChange != 0)
        { //add rotation inputs to the degree storage

            //
            yAxisDegrees = yAxisDegrees + horizontalChange;
            xAxisDegrees = xAxisDegrees + verticalChange;
            //zAxisDegrees = zAxisDegrees + tiltChange;
            
            //update rotation
            transform.rotation = Quaternion.Euler(xAxisDegrees, yAxisDegrees,0);

            //save angles to prevent potential overflow
            xAxisDegrees = transform.rotation.eulerAngles.x;
            yAxisDegrees = transform.rotation.eulerAngles.y;

            // update flag
            cameraHasChanged = true;
        }
        // calculate new field of view
        if (zoomChange != 0)
        {
            fieldOfView = Math.Clamp(fieldOfView + zoomChange, zoomMin, zoomMax);
            transform.GetComponent<Camera>().fieldOfView = fieldOfView;
            cameraHasChanged = true;
        }

        if (cameraHasChanged) //update input container
        {
            UpdateInputContaner();
        }
        
    }

    private void UpdateInputContaner()
    {
        // calculate fov bounds and pass them to the input container in terms of RA and Declination

        // calculate the bounds of field of view as far as right ascention
        float minChangeA = (360f-((yAxisDegrees + (Camera.VerticalToHorizontalFieldOfView(fieldOfView, 16f / 9f)/2f))%360f)) % 360f;
        float maxChangeA = (360f - ((yAxisDegrees - (Camera.VerticalToHorizontalFieldOfView(fieldOfView, 16f / 9f) / 2f))%360f))%360f;
        if(minChangeA < 0f)
        {
            minChangeA = 360f - minChangeA;
        }
        inputs.MaxAscension = maxChangeA*(24f/360f);
        inputs.MinAscension = minChangeA * (24f / 360f);

        // calculate the bounds of field of view in terms of declination
        float degDec = 360f - xAxisDegrees;
        if (degDec - 90f > 0f)
        {
            degDec = degDec - 360f;
        }
        inputs.MinDeclination = degDec - fieldOfView / 2f;
        inputs.MaxDeclination = degDec + fieldOfView / 2f;

        Debug.Log("minA: " + inputs.MinAscension + " maxA: "+inputs.MaxAscension + " minD: "+ inputs.MinDeclination + " maxD: " + inputs.MaxDeclination);

    }
    
}

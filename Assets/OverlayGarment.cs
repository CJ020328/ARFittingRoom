using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Windows.Kinect;
using TMPro;


public class GarmentSelector : MonoBehaviour
{
    public GameObject[] clothModels; // Array to hold different clothing models
    public BodySourceManager bodySourceManager;
    public CoordinateMapper coordinateMapper;
    public Camera mainCamera;
    public Canvas canvas; // Reference to the World Space Canvas
    public Image[] clothingImages; // Array to hold references to the clothing UI images
    public Image closeButtonImage; // Reference to the close button UI image
    public Image sizeButtonImage; // Reference to the size button UI image
    public Image screenshotButtonImage; // Reference to the screenshot button UI image
    public Sprite sizeMImage; // Image to display for M size
    public Sprite sizeLImage; // Image to display for L size
    public Sprite sizeSImage; // Image to display for S size
    public TextMeshProUGUI countdownText; // Reference to the countdown Text UI element
    public AudioClip hoverStartSound;
    public AudioClip hoverCompleteSound;
    public AudioClip cameraCaptureSound; // Sound to play when the screenshot is taken
    public float yOffset = 0.1f;
    public float xOffset = 0.1f;
    public float zOffset = 0.1f;
    public float leftHandXOffset = 0.05f; // Additional offset for the left hand
    public float leftHandYOffset = -0.05f; // Additional Y-offset for the left hand
    public float smoothingFactor = 0.5f;
    public float scaleFactor = 1.0f;
    public float hoverTime = 3.0f;
    public float handYOffset = 0.1f; // Additional offset for hand position
    public Color hoverColor = Color.yellow; // Color to indicate hover
    public Color defaultColor = Color.white; // Default color for images
    private UnityEngine.AudioSource audioSource;

    private Dictionary<Image, float> hoverTimers;
    private Dictionary<Image, Slider> progressBars;
    private Dictionary<GameObject, Vector3> originalScales; // Store original scales of clothing models
    private GameObject activeClothModel;
    private Vector3 previousPosition;
    private Quaternion previousRotation;

    private SwipeGestureDetector swipeGestureDetector;
    private ColorChanger colorChanger;

    private enum Size { S, M, L }
    private Size currentSize = Size.M;

    void Start()
    {
        if (!InitializeComponents())
        {
            Debug.LogError("Initialization failed.");
            return;
        }

        hoverTimers = new Dictionary<Image, float>();
        progressBars = new Dictionary<Image, Slider>();
        originalScales = new Dictionary<GameObject, Vector3>(); // Initialize original scales dictionary
        foreach (var img in clothingImages)
        {
            hoverTimers[img] = 0.0f;
            Slider progressBar = img.GetComponentInChildren<Slider>();
            if (progressBar != null)
            {
                progressBars[img] = progressBar;
                progressBar.maxValue = hoverTime;
                progressBar.value = 0;
                progressBar.gameObject.SetActive(false); // Hide initially
            }
        }

        // Initialize hover timer for the close, size, and screenshot buttons
        hoverTimers[closeButtonImage] = 0.0f;
        hoverTimers[sizeButtonImage] = 0.0f;
        hoverTimers[screenshotButtonImage] = 0.0f;

        Slider closeButtonProgressBar = closeButtonImage.GetComponentInChildren<Slider>();
        if (closeButtonProgressBar != null)
        {
            progressBars[closeButtonImage] = closeButtonProgressBar;
            closeButtonProgressBar.maxValue = hoverTime;
            closeButtonProgressBar.value = 0;
            closeButtonProgressBar.gameObject.SetActive(false); // Hide initially
        }

        Slider sizeButtonProgressBar = sizeButtonImage.GetComponentInChildren<Slider>();
        if (sizeButtonProgressBar != null)
        {
            progressBars[sizeButtonImage] = sizeButtonProgressBar;
            sizeButtonProgressBar.maxValue = hoverTime;
            sizeButtonProgressBar.value = 0;
            sizeButtonProgressBar.gameObject.SetActive(false); // Hide initially
        }

        Slider screenshotButtonProgressBar = screenshotButtonImage.GetComponentInChildren<Slider>();
        if (screenshotButtonProgressBar != null)
        {
            progressBars[screenshotButtonImage] = screenshotButtonProgressBar;
            screenshotButtonProgressBar.maxValue = hoverTime;
            screenshotButtonProgressBar.value = 0;
            screenshotButtonProgressBar.gameObject.SetActive(false); // Hide initially
        }

        audioSource = canvas.gameObject.AddComponent<UnityEngine.AudioSource>();
        foreach (var model in clothModels)
        {
            originalScales[model] = model.transform.localScale; // Store the original scale
        }

        if (clothModels.Length > 0)
        {
            activeClothModel = clothModels[0];
            SetActiveClothModel(false);
        }

        swipeGestureDetector = GetComponent<SwipeGestureDetector>();
        colorChanger = GetComponent<ColorChanger>();

        if (swipeGestureDetector != null)
        {
            swipeGestureDetector.OnSwipe += HandleSwipe;
        }

        // Set the initial size button image to M size
        sizeButtonImage.sprite = sizeMImage;

        // Initialize the countdown text
        if (countdownText != null)
        {
            countdownText.text = "";
        }
    }

    void OnDestroy()
    {
        if (swipeGestureDetector != null)
        {
            swipeGestureDetector.OnSwipe -= HandleSwipe;
        }
    }

    private void HandleSwipe(int direction)
    {
        colorChanger.SetActiveClothModel(activeClothModel);
        colorChanger.CycleColor(direction);
    }

    void Update()
    {
        if (!AreComponentsValid()) return;

        Body body = GetFirstTrackedBody();
        if (body == null)
        {
            SetActiveClothModel(false);
            ResetHoverTimers();
            return;
        }

        SetActiveClothModel(true);
        UpdateGarmentPosition(body);
        UpdateClothingSelection(body);
        UpdateCloseButton(body);
        UpdateSizeButton(body);
        UpdateScreenshotButton(body);
    }

    private bool InitializeComponents()
    {
        if (bodySourceManager == null)
        {
            Debug.LogError("BodySourceManager is not assigned.");
            return false;
        }

        var sensor = KinectSensor.GetDefault();
        if (sensor == null)
        {
            Debug.LogError("Kinect sensor is not initialized.");
            return false;
        }

        coordinateMapper = sensor.CoordinateMapper;
        if (coordinateMapper == null)
        {
            Debug.LogError("CoordinateMapper is not initialized.");
            return false;
        }

        mainCamera = mainCamera ?? Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main camera is not assigned.");
            return false;
        }

        if (canvas == null)
        {
            Debug.LogError("Canvas is not assigned.");
            return false;
        }

        return true;
    }

    private bool AreComponentsValid()
    {
        if (bodySourceManager == null || coordinateMapper == null || canvas == null)
        {
            Debug.LogWarning("Missing references");
            return false;
        }
        return true;
    }

    private Body GetFirstTrackedBody()
    {
        Body[] bodyData = bodySourceManager.GetData();
        if (bodyData == null || bodyData.Length == 0)
        {
            Debug.LogWarning("No body data");
            return null;
        }

        return bodyData.FirstOrDefault(b => b.IsTracked);
    }

    private void UpdateGarmentPosition(Body body)
    {
        Transform garmentShoulderLeft = activeClothModel.transform.Find("ShoulderLeft");
        Transform garmentShoulderRight = activeClothModel.transform.Find("ShoulderRight");
        Transform garmentElbowLeft = activeClothModel.transform.Find("ElbowLeft");
        Transform garmentElbowRight = activeClothModel.transform.Find("ElbowRight");
        Transform garmentSpineShoulder = activeClothModel.transform.Find("SpineShoulder");
        Transform garmentSpineMid = activeClothModel.transform.Find("SpineMid");

        UpdateBonePosition(body, JointType.ShoulderLeft, garmentShoulderLeft);
        UpdateBonePosition(body, JointType.ShoulderRight, garmentShoulderRight);
        UpdateBonePosition(body, JointType.ElbowLeft, garmentElbowLeft);
        UpdateBonePosition(body, JointType.ElbowRight, garmentElbowRight);
        UpdateBonePosition(body, JointType.SpineShoulder, garmentSpineShoulder);
        UpdateBonePosition(body, JointType.SpineMid, garmentSpineMid);

        UpdateGarmentTransform(body, garmentSpineMid, garmentShoulderLeft, garmentShoulderRight);
    }

    private void UpdateBonePosition(Body body, JointType jointType, Transform boneTransform)
    {
        if (boneTransform == null) return;

        var joint = body.Joints[jointType];
        if (joint.TrackingState == TrackingState.Tracked)
        {
            CameraSpacePoint position = joint.Position;
            ColorSpacePoint colorSpacePoint = coordinateMapper.MapCameraPointToColorSpace(position);

            if (colorSpacePoint.X != float.NegativeInfinity && colorSpacePoint.Y != float.NegativeInfinity)
            {
                Vector3 worldPosition = new Vector3(
                    position.X * scaleFactor + xOffset,
                    position.Y * scaleFactor + yOffset,
                    position.Z * scaleFactor + zOffset);

                boneTransform.position = worldPosition;
            }
        }
        else
        {
            Debug.LogWarning($"{jointType} is not tracked.");
        }
    }

    private void UpdateGarmentTransform(Body body, Transform spineMidTransform, Transform shoulderLeftTransform, Transform shoulderRightTransform)
    {
        var shoulderLeft = body.Joints[JointType.ShoulderLeft];
        var shoulderRight = body.Joints[JointType.ShoulderRight];
        var spineMid = body.Joints[JointType.SpineMid];

        if (shoulderLeft.TrackingState == TrackingState.Tracked &&
            shoulderRight.TrackingState == TrackingState.Tracked &&
            spineMid.TrackingState == TrackingState.Tracked)
        {
            Vector3 shoulderLeftPos = GetWorldPosition(shoulderLeft.Position);
            Vector3 shoulderRightPos = GetWorldPosition(shoulderRight.Position);
            Vector3 spineMidPos = GetWorldPosition(spineMid.Position);

            Vector3 targetPosition = spineMidPos + new Vector3(xOffset, yOffset, zOffset);
            targetPosition = Vector3.Lerp(previousPosition, targetPosition, smoothingFactor);

            Vector3 shoulderDirection = (shoulderRightPos - shoulderLeftPos).normalized;
            Vector3 forwardDirection = Vector3.Cross(shoulderDirection, Vector3.up);

            Quaternion targetRotation = Quaternion.LookRotation(forwardDirection, Vector3.up);
            targetRotation = Quaternion.Slerp(previousRotation, targetRotation, smoothingFactor);

            activeClothModel.transform.position = targetPosition;
            activeClothModel.transform.rotation = targetRotation;

            previousPosition = targetPosition;
            previousRotation = targetRotation;
        }
    }

    private Vector3 GetWorldPosition(CameraSpacePoint position)
    {
        Vector3 worldPosition = new Vector3(
            position.X * scaleFactor + xOffset,
            position.Y * scaleFactor + yOffset,
            position.Z * scaleFactor + zOffset);

        return worldPosition;
    }

    private void UpdateClothingSelection(Body body)
    {
        UpdateHoverDetection(body.Joints[Windows.Kinect.JointType.HandRight], clothingImages, handYOffset);
    }

    private void UpdateCloseButton(Body body)
    {
        UpdateHoverDetection(body.Joints[Windows.Kinect.JointType.HandLeft], new[] { closeButtonImage }, handYOffset + leftHandYOffset, leftHandXOffset);
    }

    private void UpdateSizeButton(Body body)
    {
        UpdateHoverDetection(body.Joints[Windows.Kinect.JointType.HandLeft], new[] { sizeButtonImage }, handYOffset + leftHandYOffset, leftHandXOffset);
    }

    private void UpdateScreenshotButton(Body body)
    {
        UpdateHoverDetection(body.Joints[Windows.Kinect.JointType.HandLeft], new[] { screenshotButtonImage }, handYOffset + leftHandYOffset, leftHandXOffset);
    }

    private void UpdateHoverDetection(Windows.Kinect.Joint handJoint, Image[] images, float yOffset, float xOffset = 0f)
    {
        if (handJoint.TrackingState == Windows.Kinect.TrackingState.Tracked)
        {
            Vector3 handPosition = GetWorldPosition(handJoint.Position);
            handPosition.x += xOffset;
            handPosition.y += yOffset;

            Vector3 screenPoint = mainCamera.WorldToScreenPoint(handPosition);

            bool hoverDetected = false;
            foreach (var img in images)
            {
                RectTransform rectTransform = img.GetComponent<RectTransform>();
                if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, screenPoint, mainCamera))
                {
                    img.color = hoverColor;

                    if (hoverTimers.ContainsKey(img))
                    {
                        if (hoverTimers[img] == 0)
                        {
                            audioSource.PlayOneShot(hoverStartSound);
                        }
                        hoverTimers[img] += Time.deltaTime;

                        if (hoverTimers[img] >= hoverTime)
                        {
                            if (img == closeButtonImage)
                            {
                                CloseApplication();
                            }
                            else if (img == sizeButtonImage)
                            {
                                CycleSize();
                            }
                            else if (img == screenshotButtonImage)
                            {
                                StartCoroutine(ScreenshotCountdown());
                            }
                            else
                            {
                                ChangeClothes(img);
                            }
                            ResetHoverTimers();
                            audioSource.PlayOneShot(hoverCompleteSound);
                        }

                        if (progressBars.ContainsKey(img))
                        {
                            progressBars[img].gameObject.SetActive(true);
                            progressBars[img].value = hoverTimers[img];
                        }
                    }
                    hoverDetected = true;
                }
                else
                {
                    img.color = defaultColor;
                    if (progressBars.ContainsKey(img))
                    {
                        progressBars[img].value = 0;
                        progressBars[img].gameObject.SetActive(false);
                    }
                }
            }

            if (!hoverDetected)
            {
                ResetHoverTimers(images);
            }
        }
        else
        {
            ResetHoverTimers(images);
        }
    }

    private IEnumerator ScreenshotCountdown()
    {
        int countdown = 3;
        while (countdown > 0)
        {
            countdownText.text = countdown.ToString();
            yield return new WaitForSeconds(1);
            countdown--;
        }

        countdownText.text = "";
        TakeScreenshot();
        audioSource.PlayOneShot(cameraCaptureSound);
    }

    private void TakeScreenshot()
    {
        string screenshotFilename = $"Screenshot_{System.DateTime.Now:yyyyMMdd_HHmmss}.png";
        string path = System.IO.Path.Combine(@"C:\Users\jingh\OneDrive\Pictures\Screenshots", screenshotFilename);
        ScreenCapture.CaptureScreenshot(path);
        Debug.Log($"Screenshot taken: {path}");
    }

    private void CycleSize()
    {
        if (originalScales.TryGetValue(activeClothModel, out Vector3 originalScale))
        {
            switch (currentSize)
            {
                case Size.M:
                    currentSize = Size.L;
                    activeClothModel.transform.localScale = originalScale * 1.1f; // Scale up by 1.1 for L size
                    sizeButtonImage.sprite = sizeLImage;
                    break;
                case Size.L:
                    currentSize = Size.S;
                    activeClothModel.transform.localScale = originalScale * 0.9f; // Scale down by 0.9 for S size
                    sizeButtonImage.sprite = sizeSImage;
                    break;
                case Size.S:
                    currentSize = Size.M;
                    activeClothModel.transform.localScale = originalScale; // Original scale for M size
                    sizeButtonImage.sprite = sizeMImage;
                    break;
            }
        }
    }

    private void CloseApplication()
    {
        Debug.Log("Application closing...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void ChangeClothes(Image selectedImage)
    {
        // Deactivate the current clothing model
        SetActiveClothModel(false);

        // Activate the selected clothing model based on the selected image
        // Assuming the image name matches the model name for simplicity
        foreach (var model in clothModels)
        {
            if (model.name == selectedImage.name)
            {
                activeClothModel = model;
                SetActiveClothModel(true);
                break;
            }
        }

        Debug.Log($"Clothes changed to: {selectedImage.name}");
    }

    private void SetActiveClothModel(bool isActive)
    {
        if (activeClothModel != null)
        {
            activeClothModel.SetActive(isActive);
            if (isActive)
            {
                previousPosition = activeClothModel.transform.position;
                previousRotation = activeClothModel.transform.rotation;
            }
        }
    }

    private void ResetHoverTimers()
    {
        var keys = hoverTimers.Keys.ToList();
        foreach (var key in keys)
        {
            hoverTimers[key] = 0.0f;
        }
        foreach (var img in clothingImages)
        {
            img.color = defaultColor; // Reset color
            if (progressBars.ContainsKey(img))
            {
                progressBars[img].value = 0;
                progressBars[img].gameObject.SetActive(false);
            }
        }
        closeButtonImage.color = defaultColor; // Reset close button color
        if (progressBars.ContainsKey(closeButtonImage))
        {
            progressBars[closeButtonImage].value = 0;
            progressBars[closeButtonImage].gameObject.SetActive(false);
        }
        sizeButtonImage.color = defaultColor; // Reset size button color
        if (progressBars.ContainsKey(sizeButtonImage))
        {
            progressBars[sizeButtonImage].value = 0;
            progressBars[sizeButtonImage].gameObject.SetActive(false);
        }
        screenshotButtonImage.color = defaultColor; // Reset screenshot button color
        if (progressBars.ContainsKey(screenshotButtonImage))
        {
            progressBars[screenshotButtonImage].value = 0;
            progressBars[screenshotButtonImage].gameObject.SetActive(false);
        }
    }

    private void ResetHoverTimers(Image[] images)
    {
        foreach (var img in images)
        {
            hoverTimers[img] = 0.0f;
            img.color = defaultColor; // Reset color
            if (progressBars.ContainsKey(img))
            {
                progressBars[img].value = 0;
                progressBars[img].gameObject.SetActive(false);
            }
        }
    }
}

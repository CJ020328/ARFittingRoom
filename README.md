# **AR 3D Clothing Virtual Fitting Room**

## **Overview**
This project is an **AR-based 3D clothing virtual fitting room** that allows users to try on garments digitally using **Kinect-based body tracking**. By integrating **Unity, Blender, and Kinect sensors**, users can experience real-time garment overlay and interactive fitting. 

The system enhances online shopping and digital fashion experiences by providing:
- **Real-time body tracking**
- **Virtual garment overlay**
- **Gesture-based interaction**
- **Color and size customization**

It is developed as part of a **Final Year Project (FYP)**.

---

## **Important Note About 3D Models**
**Large 3D model files (>50MB) are not included in this repository** due to GitHub file size limitations. These include:
- Tshirt.blend (~115MB)
- Tshirt 1.blend (~95MB) 
- Associated .blend1 backup files

**To get the complete project with all 3D models:**
1. Download these files from [Google Drive Link](https://drive.google.com/drive/folders/your-folder-id)
2. Place them in their respective folders:
   - `Assets/Tshirt.blend`
   - `Assets/Clothes Model/Tshirt 1.blend`
   - `Assets/Clothes Model/Tshirt.blend1`
   - `Assets/Clothes Model/Tshirt 1.blend1`

The repository includes smaller model files (Jacket and Skirt) which are sufficient for demonstrating the basic functionality.

---

## **Key Features**
- 🧍 **Kinect Body Tracking** – Detects body movements to overlay virtual garments.
- 👗 **3D Garment Visualization** – Displays clothing on a virtual avatar with high realism.
- ✋ **Gesture Recognition** – Users can browse, select, and customize garments using **hand gestures**.
- 🎨 **Color and Size Customization** – Users can switch garment colors and adjust sizes dynamically.
- 📸 **Screenshot Capture** – Saves images of users wearing virtual garments for reference.
- 🔄 **Real-time Feedback** – Ensures garments follow body movements smoothly.

---

## **Technology Stack**
### **🖥️ Software**
- **Unity** – Game engine for rendering and interactive logic.
- **Blender** – 3D modeling and garment rigging.
- **Visual Studio Code** – Used for writing and debugging C# scripts.
- **Kinect SDK** – Handles body tracking and motion detection.
- **Figma** – UI/UX design for the virtual fitting experience.

### **📌 Programming Languages**
- **C#** – Core scripting in Unity.
- 
### **🔧 Hardware**
- **Microsoft Kinect Sensor** – Tracks body movements for AR garment overlay.
- **High-performance PC** – Needed for real-time rendering and Kinect processing.

---

## **System Setup & Environment Configuration**
### **Step 1: Install Unity**
1. Download and install **Unity Hub** from [Unity's official site](https://unity.com).
2. In Unity Hub, go to `Installs` > `Add` and install **Unity LTS version** (recommended for stability).
3. During installation, select:
   - **Windows Build Support**
   - **Android Build Support** (if mobile compatibility is required).

### **Step 2: Setup a New Unity Project**
1. Open Unity Hub, go to `Projects` > `New` > Select `3D Template`.
2. Name the project and set a preferred location.
3. Click `Create`.

### **Step 3: Install Required Packages**
In **Unity Package Manager** (`Window > Package Manager`):
- **Import "Standard Assets"** for basic UI/controls.
- **Import Kinect SDK** from Microsoft.

### **Step 4: Integrate Kinect Sensor**
1. **Download Kinect SDK** from [Microsoft's site](https://developer.microsoft.com/en-us/windows/kinect).
2. **Import Kinect Package** into Unity (`Assets > Import Package > Custom Package`).
3. **Initialize Kinect Sensor** in `KinectManager.cs`:
   ```csharp
   void Start() {
       KinectSensor sensor = KinectSensor.GetDefault();
       sensor.Open();
   }
   ```

### **Step 5: Set Up 3D Models in Blender**
1. **Download Blender** from [Blender's site](https://www.blender.org).
2. Import 3D clothing models or create them from scratch.
3. Rig the garments using **armature bones**:
   - Select Garment > `Shift + A` > `Add Armature`.
   - Parent to bones using `Ctrl + P` > `Armature Deform` > `With Automatic Weights`.
4. **Export the model** in `.FBX` format and import it into Unity.

### **Step 6: Gesture Controls Integration**
1. Enable `SwipeGestureDetector` for detecting **swipe gestures** to change colors.
2. Map gestures to specific actions:
   ```csharp
   if (SwipeGestureDetector.DetectSwipe()) {
       ChangeGarmentColor();
   }
   ```

### **Step 7: UI & Interaction**
- Use **Canvas & Panels** in Unity for interface elements.
- Use **Graphic Raycaster** for UI interactions.

---

## **How to Build & Run**
### **Running in Unity**
1. Open **Unity Project** (`File > Open Project`).
2. Connect **Kinect Sensor** to your PC.
3. Press `Play` to test in the **Unity Editor**.

### **Building as an Executable**
1. Go to `File > Build Settings`.
2. Select **Windows** as the platform.
3. Click **Build and Run**.

---

## **Unique Aspects of This Project**
🔹 **Hybrid Approach** – Combines **Blender-based 3D modeling** with **real-time Unity rendering**.  
🔹 **Kinect-Based Gesture Recognition** – Hands-free clothing selection & size adjustments.  
🔹 **Real-Time Color Customization** – Swipe gestures allow instant **color switching**.  
🔹 **Optimized for Performance** – Uses **shader optimizations** for smooth rendering.  
🔹 **User-Friendly UI** – Intuitive design inspired by modern AR apps.

---

## **Future Improvements**
🚀 Database Integration – Implement a database system for managing and dynamically loading garments.

🚀 Advanced Garment Materials – Improve visual quality with detailed textures, patterns, and dynamic lighting effects.

🚀 Full-Body Tracking – Extend support to lower-body garments with robust motion capture for a more immersive experience.

🚀 Improved Clothing Movement Tracking – Develop advanced algorithms for skeletal tracking and garment positioning to enhance movement accuracy.

🚀 Advanced Rigging Techniques – Implement professional rigging methods like weight painting and bone constraints to improve garment movement realism.

---

## **Screenshots**

🧥 Real-Time Garment Positioning
Dynamically adjusts the clothing models based on the user's movements, ensuring garments rotate and move in sync with body motion for a seamless virtual try-on experience.

👗 Clothing Selection
Users can browse and select different clothing models via an interactive UI. Once selected, the garment is applied to the user's tracked body, allowing for quick outfit changes.

📏 Adjustable Garment Sizing
Allows users to cycle through different garment sizes with a simple button interaction. The system updates the garment size dynamically, providing a more personalized virtual fitting experience.

📸 Screenshot Capture
Enables users to take screenshots of their virtual try-on experience. A countdown timer provides visual feedback before capturing the image, which is then saved for later reference.

🎨 Garment Color Customization
Users can change the garment's color using swipe gestures detected by the Kinect sensor. This feature enhances customization, allowing users to preview different color options dynamically.

---

## **Author**
👤 **CHAI JING HANG**  
📧 [jinghangchai95@gmail.com]  
🔗 [https://www.linkedin.com/in/chai-jinghang-847461294/]  

---

## **License**
This project is licensed under the **MIT License**. 

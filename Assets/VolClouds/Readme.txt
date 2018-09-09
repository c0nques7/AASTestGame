LemonSpawn VolClouds 1.0

VolClouds is a simple but powerful volumetric cloud ray-tracer for Unity that produces realistic-looking real-time 3D clouds 
together with Unity's built in skybox. 

See http://www.lemonspawn.com/volclouds/ for online instructions. 

How it works:
****************************************
Please see the example scene (ExampleScene) for a simple set-up.  

VolClouds contains a material (with a custom-made ray-tracing shader) that is assigned to a sphere centered around the camera. 
In the example scene, the sphere has a size of 5000 and is assigned as a child gameobject of the main camera. 
The sphere should be large enough to be drawn behind any foreground geometry (landscapes etc), 
and should also be set to have no colliders, and neither send nor receive shadows.  

All cloud parameters are defined in the material inspector (Material/VolClouds), which should be assigned to the
cloud sphere. The parameters are:

- Height (from, to) : The lower & upper height of the ray-traced cloud layer
- Scale : The average size of the clouds
- Max Detail: Ray marching steps, lower values improve performance but lowers quality
- Distance: Far cloud view distance
- Color: Base cloud color
- Subtract: Cloud "fullness" - the lower the value, the higher proportion of the cloud layer is filled
- Scattering: Noise frequency falloff. Low values yields small-scale modes, while large values emphasizes large-scale modes
- Height scattering: Y-value scale. Low values produces large circular clouds, while large values yields layered flat clouds
- Density: Cloud alpha
- Hardness: Cloud smoothness, edges and dark areas in particular
- Sun glare: The intensity of the sun glare (when viewing the sun while behind / close to clouds)
- X/Y/Z shift: Translation of the cloud coordinates in the x/y/z directions 
- Time: Cloud evolution time. Typically, clouds completely change form after delta T~20
  
Please play around with these parameters to get a feel for the optimal configuration for your project. Parameters can also be changed 
real-time by setting material properties in code. 

Note:
*****************************************
The current version does not support moving the camera within the cloud layer. Therefore, the camera should always be constrained to be
below the lowest value of the cloud parameter height. 

VolClouds is quite GPU intensive, and is not recommended for mobile usage. However, play around with the detail parameter in order to find 
a value that agrees with your particular graphic card. In addition, since VolClouds is a per-pixel ray tracer, lower screen resolution will 
significantly improve performance.  



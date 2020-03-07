# 3D-Engine-OpenGL-4
### 3D Game Engine (made with API OpenGL 4.1) for C++

### Engine features:
- Blinn-Phong lighting
- Bloom/HDR effect
- Omnidirectional shadows
- Perspective/orthogonal camera
- Multisampling
- Particles generator
- Terrain generator based on 2D heightmap and tessellation
- Terrain LOD - level of detail
- Water with reflection and refraction
- Heightmap 2D/3D generator based on Perlin noise and random noise
- Heightmap to normalmap converter based on Sobel operator
- Input/output based on GLFW
- Model loader based on Assimp
- Skeletal animations
- Font rendering based on FreeType

#### Engine delivers simple shaders that support features of this engine.

### Engine structure:
```
  / - base engine class, config, include 
  base/ - shader and program compilators, texture class
  buffer/ - buffers e.g framebuffers
  camera/ - classes of cameras 
  renderable/ - everything what you can render
  scene/ - scene class
  support/ - supporting files
  texture-generator/ - noise, heightmap, normalmap generators
  window/ - window class
  shaders/ - proposed shaders
```
  
### Requirements:
- Graphics card that support OpenGL 4.1
- GLEW library - http://glew.sourceforge.net
- GLFW library - https://www.glfw.org
- GLM library - https://glm.g-truc.net
- Assimp library - https://github.com/assimp/assimp
- FreeType library - https://www.freetype.org
- STB_IMAGE library - https://github.com/nothings/stb


#### Example application that uses this graphics engine: [Procedural Terrain Generator](https://github.com/stanfortonski/Procedural-Terrain-Generator-OpenGL)
  

# ProneVR: Underwater Demo
This project is a VR port of the collaborative repositories  
- [ChanGarrickT/WaterPrototype](https://github.com/ChanGarrickT/WaterPrototype/tree/main)
- [MichaelMartinTech/ProneVR_WaterDev_MRM](https://github.com/MichaelMartinTech/ProneVR_WaterDev_MRM) **(fork)**
---
## Demo Synopsis
**[VIDi's](https://vidi.cs.ucdavis.edu/) ProneVR Underwater Prototype**  
A Unity-based VR application designed to support research towards immersive, patient-focused tools.  
**Goal**: Provide a calming underwater VR environment during medical procedures, helping reduce anxiety and improve comfort.  

---

## Added Features (as of 10/3/2025)

### Fish Movement Component ([@MichaelMartinTech](https://github.com/MichaelMartinTech), updated from 'Spawner.cs': [@MichaelMartinTech](https://github.com/MichaelMartinTech), [@ChanGarrickT](https://github.com/ChanGarrickT))
- Spline-based pathing with adjustable offsets for natural variation (``ChanGarrickT``)
- Side wandering for more lifelike behavior  
- Camera-avoidance Algorithmic-AI-spline-component behavior to simulate awareness of the user/camera (``MichaelMartinTech``)
- Looping and non-looping path options with smooth speed profiles  (``MichaelMartinTech``)

### VR SDK Integration ([@MichaelMartinTech](https://github.com/MichaelMartinTech))
- Meta XR SDK support for immersive headset use  
- Centralized camera manager for cross-scene access (CenterEyeAnchor persistence)  

### Environment Enhancements ([@MichaelMartinTech](https://github.com/MichaelMartinTech), [@ChanGarrickT](https://github.com/ChanGarrickT))
- **Coral shader update**: switched to opacity-based visibility for improved rendering in VR (``MichaelMartinTech``)  
- Environment setup, coral/rocks, and initial shader graphs (``ChanGarrickT``)
- Bubble/particle and caustics improvements (``MichaelMartinTech``)  
- Atmospheric perspective, fog, and depth blending (``MichaelMartinTech``, ``ChanGarrickT``)  

---

## Acknowledgements
**[Visualization and Interface Design Innovation Research Group](https://vidi.cs.ucdavis.edu/)**, UC Davis

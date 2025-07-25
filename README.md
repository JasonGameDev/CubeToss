# **CubeToss**

## **Project Overview**
**CubeToss** is an arcade‑style Unity game where you play as a disembodied celestial being who can conjure and fling antimatter cubes into a galactic arena. Your goal is to rack up points by skillfully striking asteroids and, if you’re precise enough, demolishing planets for huge bonuses.

---

## **Gameplay**

- **Summon and Grab Cubes:**  
  A swirl of antimatter cubes orbits your viewpoint. Click/tap a cube to telekinetically pull it towards you. When it reaches the grab point, you can hold and aim it.

- **Flick to Throw:**  
  Drag and release to flick the cube into space. The throw direction and power depend on the movement of your cursor/finger when you release. A power meter in the UI shows how hard you flicked.

- **Collide with Objects:**
  - **Asteroids:** Spin lazily around the galactic plane. Hitting them awards points based on the cube’s impact velocity and how many asteroids you’ve hit in a chain.
  - **Planets:** Rarer targets; hitting one destroys it and grants a massive score bonus.
  - **Gravitational Wells:** Pull thrown cubes into spiraling orbits, adding unpredictability to their trajectories.
  - **Stars:** Vaporize cubes on contact, ending your current throw.

- **Chain Scoring:**  
  Successive collisions without touching the galactic plane increase your chain counter, which multiplies the points you earn.

- **Galactic Plane:**  
  When a thrown cube crosses the galactic plane it triggers camera transitions and will eventually despawn if it leaves the arena bounds.

- **Dynamic Camera:**  
  A Cinemachine‑based camera system tracks your cube in flight and switches between different perspectives when you throw and when it enters the plane.

- **UI:**  
  A score counter animates your current score, and a power meter changes color to reflect the strength of your flick.

---

## **Gameplay Scene Location**

The main gameplay scene can be found at:

```
Assets/Scenes/Gameplay.unity
```

---

## **Controls**

- **Grab:** Click and hold (or tap and hold) on a cube as it orbits to start grabbing it.
- **Aim:** Move your cursor/finger to reposition the cube while holding.
- **Flick/Throw:** Release the mouse button/finger to throw. The flick power and direction are computed from your last cursor movement.
- **Cancel:** Release before the cube reaches your grab point or before flicking to cancel and let it return to its orbit.

---

## **Assets and Acknowledgements**

This project uses several free VFX and skybox packs (included in the `Assets/Content` folder) as well as Unity’s Input System and Cinemachine packages. See the `Packages/manifest.json` for package details. All scripts in `Assets/Scripts` were authored for this take‑home assignment.

---

**Enjoy flinging antimatter cubes through space!**

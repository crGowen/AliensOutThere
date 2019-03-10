# AliensOutThere

Build with Unity 2018.3.8f1 (if it asks you what the build target is supposed to be, select WebGL).

Open in Unity Editor, go to Assets > Scenes and open the Galaxy scene. Then switch from the default "Scene" tab to the "Game" tab, set the display resolution to 1920x1080 (may need to define a custom resolution to do this) and set the scale to the lowest possible for your monitor (0.67x for my 1440p) and with Maximise on Play ticked it should default to 1x anyway, click play and everything should run well.

Main menu not going to be featured (final build will be web app)

# Controls:

R - reset view

Left click (on a civilisation in list) - look to civilisation

Left click and drag - rotate view

Scroll wheel - zoom in or out (does the same thing as W / S but scroll wheel has better control)

, / . - increase / decrease time warp

Space - change between civilisation/realistic view (the realistic view isn't yet implemented, but essentially replaces all stars with, well, things that look like actual stars (and the galactic nucleus will be visible in realistic view)

WASD - move camera manually

Q / E - roll view

Z / X - move view up and down

Shift / Ctrl - hold to accelerate or fine control whilst pressing WASD/Q+E controls

# Still to be done:

Realistic view

Sounds

Star info panel

Optimisation and refactoring... there's a lot of this to be done, the code is written with most variables poorly name and set to public access (public access so they can be viewed in the Unity Editor)

Commenting the code

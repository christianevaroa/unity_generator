unity_generator
===============

Dungeon floor generator in Unity / C#. Super basic at the moment.
Just generates rectangles, tries to fit them into an array so they don't overlap,
carves out hallways between them and then puts in some rocks and torches.

Pretty ugly way of doing it, I want to redo it using a BSP tree when I get time.
Height layers would be nice too instead of flat floors.


Uses a cave tileset I found on opengameart.org and split up to work with my code,
player model is some skeleton I got in a free pack off the Unity store.

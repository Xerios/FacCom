Faction Command RTS project
=============
Abandonned SharpDX port of my old real-time strategy project.

I've lost many sources, and this one was a port of my RTS game and engine made in DirectX9.
Unfortunately, I never completed this port, but you're free to check the source code out and learn a thing or two.

The engine was made from ground up and made to be as simple as possible.

Previous engine ran fully on DirectX9 and was more feature complete ( had multiplayer, model converter, particle editor, etc.. ), 
and ever since I switched from SlimDX ( which was rarely updated ) to SharpDX. I thought it would be better to recreate the
whole code in a more cleaner way, using the sweet new clean DX11 architecture.

Features
-----------

* Lockstep networking architecture ( though it lacks network part in this port )
* Complete map editor with all fancy professional map editor features ( dynamic terrain modification, lightmap generation, pathfinding, and more )
* External scripting support and a flexible infrastructure allowing radical game modding
* Experimental velocity field pathfinding, steering behaviors ( bad port, but what the heck )
* Awesomium library for Webkit UI
* Input Manager ( with remappable keys )
* Locality Query Database ( grid based space partinioning )
* Simple GUI system + text input field ( for anything that doesn't require Webkit UI )
* Optimized custom DirectX 11 engine ( RageEngine )
  * Materials system 
  * Optimized sorted rendering ( with post screen effects support )
  * 2D line rendering tools
  * Batched 2D renderer
  * Font rendering ( bitmap fonts generated using bmfont by angelcode )
  * Terrain system ( load maps made in the editor )
  * Batching tools, Cameras ( 2D / 3D ) and lots of small useful tools 
* Clean code and few comments

Requirements
-----------

This project includes all necessary libraries to compile it in one go.
I recently compiled it fine, all it was lacking was DirectX June 2010 redist.

* DirectX June 2010 Redist
* SharpDX SDK ( included )
* Awesomium ( included )
* 

Any questions?
-----------

Just contact me, I dunno.
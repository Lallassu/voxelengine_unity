==================================================
= Qake - Voxel Engine
= Magnus Persson 2016
==================================================

v1.3
--------------------------------------------------
- In World.Start() you can either use Proc.Landscape() to generate the "town".
  or you can use MapHandler (default example in World.Start()) to generate
  a map using textures.
- New files are MapHandler and Texture.
- Updated physics blocks to be faster for rigidbody physics.

==================================================
= Engine Structure
==================================================
The world consists of XxY blocks where each block consists of one 32bit integer.
Block structure:
[8 bit = R][8 bit = G][8 bit = B][1bit free][1bit floodfill][6bit used for chunk creation]

The world is divided into chunks. Where each chunk consists of 16x16 (default) blocks. The world
is only divided into XxZ chunks where each chunk has a static height.

Each chunk is rebuilt by only showing the visible sides of the chunk if not hidden behind another chunk.
World chunks are currently not drawn on the bottom to save vertices since the engine is supposed to be 
used for 3rd person player. However, it is very simple to draw bottom as well and the code is already 
included for it. Upon explosions, a flood fill algorithm is used to determine if there will be "loose"
blocks and if so, a new chunk will be created as a standalone mesh including the blocks that are loose.

There are two types of chunks. World (for world stuff such as houses, grass, lamps etc). Object type which 
is used for player and loaded vox files.

The World file handles the rebuilds, adding and removing of blocks. The "Start()" function in World
triggers everything in the example.


==================================================
= Files
==================================================
World.cs - Most of the actual engine (chunk reubild, add/remove blocks, Explosions, flood fill etc)
Block.cs - Not yet finished block implementation to use own simple physics for blocks instead of rigidbody.
Blockpool.cs - Pool of Boxes used for explosion debris.
Chunk.cs - The chunks, handle phsyics for chunks etc. 
ChunkCollision.cs - Handle actual collision events for chunks.
Enemy.cs - Not yet finished. But will be some basic enemy.
Missile.cs - Simple missile for player testing and explosion testing. 
MissileCollision.cs - Collision handler for missiles.
Player.cs - Basic player with a static vox file model.
Proc.cs - Procedurally generated content (Houses, lamps, roads, grass, trees, flowers etc)
Steam.cs - Steam from street-sewers.
Vox.cs - Read .vox files from Magicavoxel and creates chunks to be used on the world.
Weapon.csa - Not yet finished weapon handling system.

==================================================
= Usage
==================================================
Look at the Start() function in World.cs. That will trigger the creation if the world. An object called "World" in the 
project includes the files for World, block pool, steam and player.

Functions that are most interesting are:
RemoveBlock(...);
AddBlock(...);
Explode(...);

Due to thirdparty library I cannot include the use of explosion effect other than lightning. But a tips
is to use Detonator Explosion package from Asset store. The code for the explosion instantiation is included
but unmarked in the code. Using the "ignitor" effect gives a really nice look of burning debris.

==================================================
= Vox Files
==================================================
There is a script included that reads .vox files and imports the voxel models as chunks
into the World. These files are created with a program called MagicaVoxel.

==================================================
= Other...
==================================================
The engine is easily portable due to its design and usage of low-level
bit operations. An early playable version was created in javascript with ThreeJS
and can be played here: http://qake.se 


==================================================
= Contact And Questions
==================================================
Twitter: @lallassu
Email: magnus@nergal.se

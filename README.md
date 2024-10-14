Setup
• isometric perspective camera. No rotation, no zoom in/out

• a water shader that reacts to the player character’s movement, that has refraction, and 
that has reflection coming from the skybox and the characters when they are nearby
Playable Character

• Character must use the IK System for movements. Bonus: the character should evade 
obstacles (example: when approaching a wall he slows down the movement, when walking 
besides a wall he puts his hands in front of him as if glancing over its surface, he crouches 
when a tree branch is in his way, etc)

• Character must have a basic melee attack
Non-Playable Characters (NPCs)

• Implement NPCs. There are 2 groups of NPCS, and each group has its own predefined 
behavior. Do this as you like, but the main reasoning behind it is that one group should act as 
guards (patrolling around the village) and the other group is a group of people who
peacefully moves around

• NPCs react to the player character’s actions. If the player attacks a guard, for example, the 
other guards react and attack the player if they’re within a certain radius (visible radius). 
Peaceful NPCs run away from the player if they are attacked because they are scared, and 
they seek a safe spot to hide

• implement small fishes inside a pond or small river. The fishes must move randomly and they 
must react to the player character (example: when the player gets inside the pond, the fishes 
get scared and they change their trajectory)




Leftovers:
a tool to create-add vines to locations. A random mesh must have geometry and the vines 
must spread across the geometry. It must have parameters, the vines must be larger in the 
beginning and then grow thinner and thinner until its end.

If I had more time I would take a cylinder as a prefab and create a script that would generate a few instances of this vine prefab. 
The vine prefab pieces would spread downwards from where the user clicks and generate smaller segments as it goes,
perhaps instantiating leaf prefabs as it goes as well, making sure the leaf is not clipping through the wall as it is generated.

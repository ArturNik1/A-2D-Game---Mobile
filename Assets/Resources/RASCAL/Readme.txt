Contents:
1. Intro
2. Usage
3. Functions
4. Example Scenes
5. Physics Materials & Exclusions
6. Troubleshooting
7. Notes




----- 1. Intro -----

Thanks for using my skinned mesh collision script!
Lovingly nicknamed RASCAL, after that rascal, Unity-Chan.
It's an acronym for Realtime Accurate SkinnedMesh Collision Autogeneration Logic.
Or something like that anyway, I know it's silly but that's kind of the point.

I made this because I've got ideas for things that require having accurate collision on skinned meshes and I didnt think there was a good enough solution and figured I could make something like this. so I did I guess, huh.
Anyway, I'll be the first to say this obviously shouldn't be used as a replacement for more conventional collision needs, the higher accuracy comes at performance costs, so if you dont need the accuracy then you probably dont need this.
With all powerful tools comes a great responsibility, etc. Used wisely though, it can be a great tool to accomplish some really cool things, I hope you enjoy!




----- 2. Usage -----

There are a few ways to use this script but all of them start with the RASCAL Skinned Mesh Collider component you can find under the Physics section of components.
You should add the component to the base of your skinned mesh. It will work on all SkinnedMeshRenderers under the gameobject that you add the RASCAL component to.
From here, you can modify settings to your liking. I wont go over what they all do here as I've added some rather (perhaps overly) detailed tooltips to each setting/button.

It might work well enough for you to slap RASCAL on your mesh, press generate, and remove RASCAL and be done. RASCAL generates colliders per-bone, meaning colliders try to contain only the collision for the part of the mesh most affected by each bone.
The colliders are placed on the transforms for the bones so as the mesh animates using the bones the colliders will be moved with it to match the mesh itself.
This can work fairly well on its own, but as the mesh deforms due to bones moving it wont perfectly match up. If you need even more accurate collision you can enable the asynchronous updating feature.

The asynchronous updating feature is one of the neatest part of the script, basically it allows you to set a CPU budget in milliseconds which will tell the script how long its allowed per every fixed update to work on regenerating the mesh colliders.
Because of the way the work is segmented, the entire mesh can be regenerated over time during gameplay very smoothly without any hitching.
This will allow you keep the collision as up to date as possible, though there will likely still be inaccuracies as the mesh animates, especially if a drastic deformation happens rapidly.

Most of the time there's probably no reason to use GenerateOnStart as you can pre-generate all required data using the generate button and save on scene load time. But it's included for posterity.
In case you're changing the mesh somehow with something else before the game starts.

If you know when your mesh will need to be updated as if before an important event you will need accurate collision for, or after a known change to the mesh shape, you can save on performance by only updating the mesh when you need to, 
instead of having it always updating. You can use either the immediate update function which will do it all at once but may cause a small stutter in the framerate, or you can call the asynchronous updating function with the continuous
parameter set to false. This will only update the mesh once, using the CPU budget settings to regenerate the collision over time.

The script basically works in 2 parts, the first part is processing which just processes all the required stuff and calculates things and builds the data structure.
The second part is actually generating the collision meshes using all that processed data. And as discussed previously, this can either be done immediately or asynchronously.

Finally, almost all functions and variables on the script are exposed so you can interface with the data structure of the script via C#




----- 3. Functions -----

I wont describe them all here as they all have descriptions and summaries that should work with intellisense and the monodevelop equivalent. Barring that, you can always just open the script up and read them from there.
So here's a list of public functions on the script you can use.

public void ProcessMesh()
public void ImmediateUpdateColliders(bool force = false)
public Coroutine StartAsyncUpdating(bool continuous)
public void StopAsyncUpdating()
public void CleanUpMeshes()
public void CleanUpAllMeshColliders()

Events:
event RASCALTimedEvent OnAsyncUpdateYield;
event RASCALTimedEvent OnAsyncPassComplete;

And I'll just mention the variable asyncUpdating here as well.
Setting this to false while the asynchronous update function is running will allow it to finish updating all the meshes and then stop.
As apposed to StopAsyncUpdating() which will stop it immediately.
It otherwise is just an indication of whether or not asynchronous updating is currently running.




----- 4. Example Scenes -----

I just want to mention that example scenes are provided and include some scripts that show how to interface with RASCAL.
The main example requires the Unity-Chan asset you can download for free on the asset store.
https://assetstore.unity.com/packages/3d/characters/unity-chan-model-18705




----- 5. Physics Materials & Exclusions -----

I figured people would probably want a method of applying different physics materials to the colliders if not only for nice way of being able to categorize what area was hit in a collision or raycast.
So I've added a few methods of applying physics materials to the colliders that I think should work well.

First theres just the physics material variable that is the blanket material for all colliders that will be used if no other higher priority setting is found for that particular collider.

Then there's the material association list, you can use this to simply associate materials from the mesh with physics materials.

Lastly, if you need control for what materials go on what bones youll need to add a RASCAL Phys Material Properties component to the bone transform or skinned-mesh transform.
The priority goes from highest to lowest: Material-Association-List -> Bone-Transform -> SkinnedMesh-Transform -> PhysicsMaterial-Variable

Come to think of it I probably should've set up bone and skinned mesh transforms the same way as I did materials but its set up this way already and this way also allows for you to override other settings.
Maybe I'll add both later, yell at me if you want this I guess.


And for exclusions, you can simply drag a transform for a bone, a SkinnedMeshRenderer, or material.
This will make it so collision is not generated for these items.




----- 6. Troubleshooting -----

Hopefully most of the time if something goes wrong it can be fixed by simply regenerating.
The way I coded the transformations of meshes from mesh coordinate space to bone coordinate space should be robust but I've only tested it on 3 different meshes.
If you find the collision meshes are placed strangely feel free to contact me and hopefully we can work out whats going wrong.
You can try enabling the zeroBoneMeshAlternateTransform option, but it probably wont help because I'm pretty sure in all situations skinned meshes with no bones (I.e. only blendshapes) need to be treated differently which is how it is by default.

If you're trying to use convex colliders and getting errors about vertex count, make sure the max vertices allowed is 255 or less, but probably not too low either.

If you're trying to set up physics materials for the colliders by material and your skinned mesh has multiple materials, make sure you enable splitCollisionMeshesByMaterial.

Make sure to regenerate the mesh to apply any changes you make to the RASCAL component.

Again feel free to contact me with questions or issues.





----- 7. Notes -----

Just some other random things worth mentioning:

Disabling the RASCAL component will pause asynchronous updating.

RASCAL contains some additional classes for the data structure thats created to house all the mesh info and stuff. You can iterate over the data structure starting with the "skinfos" array in RASCAL.
An example of this is shown in the UnityChanExample script.
But each element of the data structure can be updated independently. So you could extend this script to allow for even more granular and specific control of updating to fit your needs if you wish.

Performance of the script is known to be faster in newer versions of unity, and even more so, using the .NET 4.x equivalent runtime instead of the .NET 3.5 one.

I've worked pretty hard on this script to try and make it fairly robust. A lot of work went into optimization, getting the generation time down for example from 2.7 seconds to just about 70 milliseconds on Unity-Chan, with similar improvements being made to update speed.
So I really hope this can be of use to many people who needed a solution like this, like I did.
If you like it, it would be great if you could leave a review!



Hopefully all the settings have good tooltips and the script does everything you need.
If not, feel free to send me a message at boltsoft@gmail.com

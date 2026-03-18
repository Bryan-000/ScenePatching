# ScenePatching
A super simple library that allows you to extremely simply edit scenes and such via your mods, made for [ULTRAKILL](https://store.steampowered.com/app/1229490/ULTRAKILL/).

# How to use the library
### -- IMPORTANT --
It's super important to call this line anywhere in your mod **ONCE** before loading the game.\
`SceneModding.PatchAll()`\
so your patches actually work :3

### Making a ScenePatch
Make a new **static class/method** and add the ```[ScenePatch]``` attribute to it\
In the class make a new method with ```[ScenePatch("TARGET_SCENE")]``` and set target to the desired scene (Level 1-4, CreditsMuseum2, uk_construct, Endless, etc)

Example:\
![whar...](https://raw.githubusercontent.com/Bryan-000/ScenePatching/refs/heads/master/gitassets/example1.png "meow :3")

### TargetObjects
The `[ScenePatch]` attribute also has a targetObject's parameter,
using this u can add a ```GameObject``` or any other component(Transform, Door, Light, etc) parameter(s) to your method and the GameObject/a component off of the GameObject will be passed along.\
![uh oh](https://raw.githubusercontent.com/Bryan-000/ScenePatching/refs/heads/master/gitassets/example2.png "haiiiiii :3")
![why are u no work](https://raw.githubusercontent.com/Bryan-000/ScenePatching/refs/heads/master/gitassets/example3.png "uwu")\
You can also add multiple targetObject's and itll run the patch on all of them :3

#### UnityExtensions
oki so this isnt like the main part of the library but its coolio to me so :3\
check out ScenePatching.UnityExtensions 
[here](https://github.com/Bryan-000/ScenePatching/blob/master/src/UnityExtensions.cs), it itself has documentation in the file so \:P
# unity-script-templates

This repository provides templates for common C# classes for the Unity Game Engine.

## Installation

### Using git submodule

Assuming your project already is backed by a [git](https://git-scm.com/book/en/v2/Getting-Started-Installing-Git) 
repository, you can add this project as a submodule:

```git submodule add git@github.com:Lovely-Bytes-Gaming/unity-script-templates.git Assets/ScriptTemplates```

The target directory 'Assets/ScriptTemplates' is important 
for the unity editor to recognize the new templates. 

If your **Assets** folder does not sit in the root directory of your git project,
you have to adjust the path accordingly, e.g.:

**Relative/Path/To/Assets/ScriptTemplates**

### Drag and Drop
Alternatively to the approach above, simply download the 
[zip](https://github.com/Lovely-Bytes-Gaming/unity-script-templates/archive/refs/heads/main.zip)
file, unpack it, rename it to **ScriptTemplates** and drop it into
your **Assets** folder.


## Usage
After restarting the editor, the script templates should be available via the context menu:

**Create &rarr; Lovely Bytes &rarr; C# &rarr; Template of your choice**

The following templates are available so far:

**Pure C#**
+ class
+ struct
+ enum
+ interface
+ singleton

**Unity Specific**
+ MonoBehaviour
+ ScriptableObject
+ StateMachineBehaviour (for Animator States)
+ Aspect (ECS)
+ AuthoringComponent (ECS)
+ ComponentData (ECS)
+ System (ECS)

If you have ideas for new templates, template changes, or additional
editor features,
we're happy to take your suggestions and/or pull requests!
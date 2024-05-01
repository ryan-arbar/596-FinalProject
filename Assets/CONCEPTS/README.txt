Room Creation Notes - Ryan

This is just if you want to try to concept the rooms in Unity.
It's also so future me doesn't forget how this project works.

-I would recommend creating a prefab of the entire room and then deleting the 
room from the scene. This is so there is no conflict when others try to
pull from the repository. You could also create a seperate scene and create there.
That scene will be safe to push to the repository.
TRY *NOT* TO MODIFY THE MAIN SCENE IF YOU CAN

-Use the BlankRoom prefab in this folder to start creating the room. 
It has the basics of what is currently in the game at this point in time. 
You can save a new prefab of the room you just made when done and save it 
to this folder.

-ProBuilder makes for easier room concepting. 
(Tools -> ProBuilder -> ProBuilder Window)

-Each room has a PuzzleController to choose what prerequisites are needed to
open the ExitDoor as well as which doors to unlock.

-Each room will have an EntranceDoor and at least one ExitDoor.
(Ignore the ExitDoor(Yellow) object)

-The rooms can be expanded as much as needed.

-Keep in mind the isometric view of the rooms as there is no visual depth.
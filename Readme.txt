File structure :
    -Assets 
        -Resources
            -Prefabs (Contains all the Prefabs used in game)
            -Sprites (Sprite used in game)
        -Rock CLimber (UI Assets from Unity Asset Store)
        -Scenes (Contains the main scene SampleScene.scene)
        -Scripts (Contains all the scripts)
            -AnalyticsController.cs (Used for logging details in Console)
            -DataContainerController.cs (Used to initialize values for Helper UI)
            -GameController.cs (Manages the overall gameplay)
            -ReelController.cs (Controls the spawing of elements in Each Reel)
            -SymbolDatabase.cs (Contains Symbol types, images of each symbol and its respective weights)
            -UIController.cs (Controls overall UI functionalities)

AnalyticsController.cs
    - Prints the Log of gameplay after each spin with details of event name, win amount, symbols, and win streak

DataController.cs
    -Initializes value for the Container with the Symbol Data provided to the Function SetData(SymbolData sd)

GameController.cs
    -Manages the Spin, Bonus Game, Evaluation, Checking, and Result.


Chosen Bonus Game - Wild 

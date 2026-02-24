README

[1] OVERVIEW
[2] CHANGES
[3] HOW IT WORKS
[4] DETAILS
[5] TIME SPENT

----------------------



[1] OVERVIEW

The game presents the three exercises detailed in the spec: Ace of Shadows, Magic Words, Phoenix Flame. Each game extends AbstractExerciseController, and returns to the menu via the OnEnded event when finished.

[2] CHANGES

The original version was too focused on visual fidelity, as I was too tempted to implement things from a 'player-focused' perspective to make an 'impressive gameplay demo' rather than a vertical slice of production-oriented code, which is now the focus, alongside preserving the visual and UX fidelity.

This has resulted in an overhaul of most of the game code, such that, where possible, game session logic (such as cards being sorted into stacks) and utility functions (such as jSon web requests) are completely decoupled from Unity/MonoBehaviour lifecycle / update loop. The way session are configured has also changed massively, adding support for different session variants and setup by config-injection of the main menu.

[3] HOW IT WORKS

+ Configs:
----------------------

The game is now capable of launching different variants of each exercise, driven by instances of ExerciseConfig.cs - different numbers of card stacks in AoS, alternative handling of emojis in MW, the colour-buttons interface in PF, etc. Each game has a strongly-typed config override which its inheriting ~ExerciseController receives and configures itself with (e.g. AceOfShadowsConfig is expected by AceOfShadows.cs via <T>). Everything which is configurable, including additional config files such as in Magic Words, is dependency-injected at startup from the config file, before Begin() is called. This is intended to provide flexibilty and decoupled logic by configuring the gameplay scene via dependency injection of all the important values, and could be considered to mimic game sessions being received from a backend server, for example.

Limitations:

- When values become configurable, this naturally creates the risk of logical or functional misconfiguration - for example creating a session of AoS with 0 cards, or 1 million cards. We could solve this just by adding [Range] tags to the serialized fields, but I also added a function (ExerciseConfig.GetClampedProperty) for clamping the returned value within a range, and showing a warning if it was outside it. This logic is held on the controller itself (see AceOfShadows.cs:MAX_CARDS), since logically it is the UI which should be aware of any physical limitations based on layout / performance / pool sizes etc. In production code, I would include both a 'logical' restriction of values which 'make sense' to the gameplay logic, and this kind of on-device clamping to respect 'physical' limits in the gameplay scene, such as performance.

+ Main Menu:
----------------------

On Startup, MainScreen.cs calls ExerciseConfig.LoadAll(), which searches Resources/ for game configs, and adds them to the menu dynamically. The exercises themselves are each held in a separate scene, and MainScreen.cs is responsible for handling the scene transitions and returning at the end of gameplay. All of this is done in an async fashion so as not to produce frame drops to the user. When the scene is loaded, the game checks the root GameObjects for a dedicated ExerciseReference.cs component, which points to the exercise in the scene. No assumptions are made about where in the scene the actual Exercise might live, improving flexibility. At the moment, the scene names are generated deterministically 1:1 from the exercise type, but these could be overridden in the config if there were ever a case where a certain variant needed a different scene (see config.GetSceneName()).

+ Exercise lifecycle:
----------------------

When an exercise starts, a class inheriting from ExerciseController first initialises itself in an async fashion (see ExerciseController.InitialiseAsyncInternal) - for AoS and PF, this is technically unnecessary as their initialisation is purely synchronous, but in MW this covers the initial download of the jSon file. This all happens while MainScreeen's loading overlay is still showing, so exercise initialisation is hidden from the player, who experiences it as a normal part of the main loading screen.

Each exercise (except Phoenix Flame) creates a synchronous 'Session' class which represents the exercise's core logic, completely decoupled from anything Unity-related. For instance, logical card stacks and the card transitions in AoS (see AceOfShadowsSession.cs). The lightweight MonoBehaviour-derived class can run the session logic externally, and listen to state-change events to drive the UI. The session logic itself is not driven from outside, nor dependent on anything in the scene. When the session signals that it has ended, the Mono class calls OnEnded and the main menu takes over exiting and disposing of the scene.

* Phoenix Flame is more of a visual demo and has no real concept of 'session logic', but it could easily be added in future if some kind of gameplay were designed.

This decoupling of game logic from the UI is valuable because it allows...

+ Unit tests:
----------------------

As the game logic for AoS and MW is capable of being instantiated and run synchronously, this makes it possible to unit-test entirely outside of the game scene (and of course then independent from the Unity classes related to it). Each config extends a RunUnitTest() method which returns true/false on pass/fail. The best example is Ace of Shadows (see AceOfShadowsConfig_UnitTests.cs). It creates a fresh session from the config, runs it to completion, then checks for a correct state (all cards have moved, number of cards adds up to the total, etc). This makes it possible to expose 'Unit test' buttons on the main menu, which can validate the correctness of the config before playing. Keeping the unit tests in partial class files helps readability by not inflating the core class file.

Limitations:

- As is often the case with unit testing, this requires the test wrapper to create a session of gameplay then run it to completion using custom code - for instance having to call session.MoveNextCard(); in AoS. If the core gameplay were changed (e.g. if cards were not always moved, or something else needed to happen), the unit test would need to be updated to handle this. 

- You could fix this by making all Session objects inherit from a common parent/interface, and the unit test could call e.g. 'while (!iSession.IsComplete) iSession.RunNextStep();', but I think this would likely become brittle quickly. What if a type of game cannot simply be stepped-through and requires player input, for example...

[4] DETAILS

- I previously used a few instances of Transform.Find() as a lazy way to apply some simple colours/alpha to button components. I would obviously not do this in a finished application or critical-path code. This has been replaced with components which intercompose with a Button component and handle their extra graphics (ExerciseButton.cs, UnitTestButton.cs).

[5] TIME SPENT

One day (Monday 23rd) - verify by checking 'assetCreated' fields in the .meta files of new assets.

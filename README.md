RimWorld Conflict Checker

	Code: https://github.com/biship/RimworldConflictChecker
	Details: https://ludeon.com/forums/index.php?topic=25305
	Latest Version: https://github.com/biship/RimworldConflictChecker/releases

Please report any bugs either on GitHub, or the Ludeon forum thread.

	GitHub Issues: https://github.com/biship/RimworldConflictChecker/issues

Usage:

	RCC.exe [-all] [path(s)]
    
		-all   : Run as if all mods are enabled (ignoring what is set in ModsConfig.xml)
		[path] : Path(s) (each within quotes) seperated by spaces
			 Where paths are : (required) Rimworld.exe location 
				  (optional) Rimworld Game Mod Folder
				  (optional) Steam Mod Folder
				      (optional) ModsConfig.xml location
Example:

	RCC.exe "D:\SteamLibrary\steamapps\common\RimWorld" "D:\SteamLibrary\steamapps\workshop\content\294100"

Alternatively, just run RCC.exe to get a folder picker & config saver.
	
Results of the checks are written to file RCC.txt in the executable folder.

RimWorld Conflict Checker is an expanded and updated version of: https://ludeon.com/forums/index.php?topic=20211

	Licensed under MIT
	https://github.com/biship/RimworldConflictChecker/blob/master/license.txt

	Please include credit.
	Please do not distribute or post without my permission.
# RetroLab
This is a long time due work of mine, which was originally supposed to be an event in the summer of 2023 buuut .. I didn't find the time to finish it. So, I'm doing it now.

**RetroLab** is a modification for **SCP: Secret Laboratory** version **8.0.0** *(Revision III)*. It allows you to host a dedicated server on Windows *or* Linux along with your very own central server, which provides player authentification (including global moderator ranks and global bans) and a server list. Basically, an old version working with all features as a new one would.

**Also, the server build comes packaged with ServerMod v3.2.0-A, which allows you to install plugins made for this version.*

# Installation
This step is common for every client type.
1) Go to the Releases page and find the release that contains full SCP: Secret Laboratory builds. Then download the one you need (**SCP Secret Laboratory Client** is the game itself, **SCP Secret Laboratory Server_Windows** is the Windows server and **SCP Secret Laboratory Server_Linux** is the Linux server).
2) Find the latest release (it's tag will contain the type of your client, ex. **v1.0.0-game** for the game, **v1.0.0-server** for the dedicated server.
3) Download all the files ending with **.dll** listed in that release and put them in the **Managed** folder (which is located under **SCPSL_Data**)

## Central
Central Server releases are tagged as -central (ex. **v1.0.0-central**), so just download the correct build (**RetroLab.Server_Linux** is for Linux *x64* and **RetroLab.Server_Windows.exe** is for Windows *x86*).

# Usage
1) You need a central server hosted somewhere in order to use this mod. Then, you need to enter it's IP *(and port, if changed)* in the **config.json** file located right next to the game's executable (**SCPSL.exe**/**SCPSL.x86_64**)
2) You **NEED** to have your Discord application open, otherwise you **won't** be able to authentificate!
3) Just launch the game's executable (if launching the game itself, otherwise launch via **MultiAdmin.exe**).

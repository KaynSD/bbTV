# Blaseball TV
Blaseball TV Viewer, written in C# for Unity 2019.3
Download the latest release in the panel to the right to try it out today!

## Features
* **Datablase Scraper**; Copies relevant stats from the datablase in a gradual fashion to keep load on the site low
* **Live Game Connection**; Connects using the blaseball service endpoint and keeps logs of the games it plays for future reference or live viewing
* **Machinima Recreation**; and the major feature of this viewer is a machinima recreation of each play, strikeout, incineration, random event and home-run.
* **User Customisation**; each league, subleague, division, team and player gets it's own folder for placement of Tlopps card, textures and images for use within the game. Fully customise your own blaseball viewing experience!

## TODO
* **Bug Fixing**, there's a few bugs, leaks, and undisposed listeners that will need tidying up before an actual release can be made
* **Loading Models**, IS possible with assetbundles given the code included, but needs documenting and made easier for third party developers
* **QOL Features**, opening user directories, browsing teams, etc.
* **Remaining Cutscenes**, as of right now there is only a catch all Technical Difficulties, a step up to plate, and a pair of strike animations. 
* **Better Machinima**, I'm good, but not great. Plus more variety of cutscenes will make the application better for everyone. Bats and balls and other things attach points in the model animations setting up as well, and I'm pretty sure I've aligned everything very badly.

## Packages and Prerequisites (Included)
* Unity 2019.3
* TextMeshPro
* [Zenject 8.0.0](https://github.com/modesttree/Zenject) dependency injection
* [NuGet for Unity](https://github.com/GlitchEnzo/NuGetForUnity), to install packages from NuGet
    * [NewtonSoft JSON.net](https://www.newtonsoft.com/json)
    * [EvtSource](https://github.com/3ventic/EvtSource) for Server-Sent Event connection

Current Maintainer: [@kaynSD](https://twitter.com/kaynSD)
Massive Thanks to: 
* [@4damAvenue](https://twitter.com/4damAvenue), Adam Streeter, for the amazing logo artwork (used with permission!)

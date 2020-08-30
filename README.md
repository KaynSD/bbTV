# Blaseball TV
Blaseball TV Viewer, written in C# for Unity 2019.3

Not ready for the big time yet; just here in case something goes wrong!

## Features
* **Datablase Scraper**; Copies relevant stats from the datablase in a gradual fashion to keep load on the site low
* **Live Game Connection**; Connects using the blaseball service endpoint and keeps logs of the games it plays for future reference or live viewing
* **Machinima Recreation**; *Coming soon*, and the major feature of this viewer is a machinima recreation of each play, strikeout, incineration, random event and home-run.
* **User Customisation**; each league, subleague, division, team and player gets it's own folder for placement of Tlopps card, textures and images for use within the game. Fully customise your own blaseball viewing experience!

## Packages and Prerequisites (Included)
* Unity 2019.3
* TextMeshPro
* [Zenject 8.0.0](https://github.com/modesttree/Zenject) dependency injection
* [NuGet for Unity](https://github.com/GlitchEnzo/NuGetForUnity), to install packages from NuGet
    * [NewtonSoft JSON.net](https://www.newtonsoft.com/json)
    * [EvtSource](https://github.com/3ventic/EvtSource) for Server-Sent Event connection

Current Maintainer: [@kaynSD](https://twitter.com/kaynSD)
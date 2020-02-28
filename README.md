# Sen's fortress
![SensFortress View_bpbjukvQ81](https://user-images.githubusercontent.com/49918134/74938679-bf98b780-53ee-11ea-9349-dd9f153e179a.jpg)

Sen's fortress is a desktop based, open source and completly free password manager, that runs on Windows only (for now). It uses the Advanced Encryption Standard (short: AES) in combination with Password-Based Key Derivation Function 2 (short: PBKDF2) for storing data savely on your harddrive, while also implementing other security features, which will be elucidated further down below. 

## Getting started
![SensFortress View_EOMMUfUtlv](https://user-images.githubusercontent.com/49918134/74938684-c0c9e480-53ee-11ea-91e0-4b199d202e70.jpg)

### Prerequisites
#### Usage
* [Windows](https://www.microsoft.com/de-de/software-download/) - Sen's fortress is windows only (for now)
* [.NET Core runtime](https://dotnet.microsoft.com/download) - Runtime required for using Sen's fortress

```
Download the latest .zip version of the Release folder and run the SensFortress.View.exe
Make sure the application has the rights to read and write files from and onto your harddrive.
```

#### Developing
* [.NET Core 3.0 SDK](https://dotnet.microsoft.com/download) - SDK required for deveolping Sen's fortress
* [Visual Studio / WPF](https://visualstudio.microsoft.com/de/downloads/) - Graphical framework.

#### External libraries / Nuget packages

* [Prism.Wpf](https://www.nuget.org/packages/Prism.Wpf/) - MVVM framework
* [Live Charts.Wpf](https://www.nuget.org/packages/LiveCharts.Wpf/) - Charts / Graphs library
* [Material Design](https://material.io/resources/) - UI designing language 

```
Clone the repository.
Make sure all Nuget packages and externally added libraries are referenced. (VS should do this by itself - if not, it will tell you what's missing. You must then download these packages yourself.)
Make sure VS has full writing access. (Try starting as admin if it does not work initially.)
Build the solution.
Start
```

### Technologies

* C#
* .NET Stack / .NET Core 3.0
* WPF => MVVM
* Visual Studio 2019
* Git
* Material Design

## About
![SensFortress View_AGQcJO348o](https://user-images.githubusercontent.com/49918134/75542539-50d7e180-5a20-11ea-82ad-19363c28d4ef.png)

Sen's fortress is currently completly offline. Against the trend of many other password managers, there are no future plans in implementing a cloud structure - instead a local synchronisation progress with other instances (PC's, Mobile-apps) in a closed network (Home WiFi) will be featured.

### Why no cloud?
Let's adress the elephant in the room immediately. Anyone that has been interested in Cryptography may have heard about some standard procedures which password managers perform, to obtain maximum security for highly sensitive data such as your passwords.

* AES
* Hashes
* SSL/TCP
* VPN
* SSH

so on and so forth. The inner conflict of anyone that tries to develop a password manager now lies in the combination of comfort and security. How much comfort is the user ready to sacrifice in order to actually use a password manager, while also being provided the highest level of security. **The answer is: pretty damn little.**
It is actually so little that password managers today focus more on developing new comfort features than actually trying to increase the level of security. To push this even further, one may throw in the daring statement: **They are ready to sacrifice security for comfort**.

Let's elaborate on this statement.
There are some that critizise password managers on very deepest level: **Storing passwords on the harddrive in the first place instead of just remembering or writing them down.** While it is true that a hacker/virus cannot steal something that is not a computer, they can certainly leak databases of big coorporations thousands of miles away from your home PC or your notepad. In this case there is only one thing that decides whether your passwords will be leaked or not: **The password itself.**
Spoiler alert: Anyone that has thought "password123" was a good idea will be in for a rude awakening. 
Others however who may have used a password manager and actually used it properly can calmly relax: Generated strong passwords such as: *Y!Y-iuicn.h;gT/RqUtfY$uI<yFH/D>g* have a 99,99% of not being leaked. They are however pretty difficult to memorize or written down. **And that is why you should use a password manager.** 

In the age of digitalisation where your internet identity is no longer just that one silly ICQ or myspace account but your whole sensible online banking credibility, a password manager should remind everyone that it is crucial to be caucious, preventively and save and in that matter helps achieving these conditions. That is the core of any password manager. **Well, it should be.**

Somewhere along the road of the last years that core principle of any password manager has slowly been forgotten. It is now a race of: Which password manager on the market takes the least clicks to set up and use. Which software provides the least effort for me to have my passwords anywhere at any time on every hardware I have without me even copy and pasting in the passwords on a website. Are the developers or coorporations to blame for that? The answer is pretty obvious. **The market provides what the customer wants.** In a time where people actually order water and food from amazon instead of going to the supermarket down the street, it's pretty much no surprise.   

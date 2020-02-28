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

## About / Sen's fortress mission
![vHYVfGUTmw](https://user-images.githubusercontent.com/49918134/75565301-00777880-5a4e-11ea-8950-7a1439f9cf53.png)

Sen's fortress is currently completly offline. Against the trend of many other password managers, there are no future plans in implementing a cloud structure - instead a local synchronisation progress with other instances (PC's, Mobile-apps) in a closed network (Home WiFi) will be featured.

### Why no cloud?
Let's adress the elephant in the room immediately. Anyone who has been interested in Cryptography may have heard about some standard procedures which password managers perform, to obtain maximum security for highly sensitive data such as your passwords.

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

In the age of digitalisation where your internet identity is no longer just that one silly ICQ or myspace account but your whole sensible online banking credibility, a password manager should remind the user that it is crucial to be caucious, preventively and save and in that matter helps achieving these conditions. That is the core of any password manager. **Well, it should be.**

Somewhere along the road of the last years that core principle of any password manager has slowly been forgotten. It is now a race of: Which password manager on the market takes the least clicks to set up and use. Which software provides the least effort for me to have my passwords anywhere at any time on every hardware I have without me even copy and pasting in the passwords on a website. Are the developers or coorporations to blame for that? The answer is pretty obvious. **The market provides what the customer wants.** In a time where people actually order water and food from amazon instead of going to the supermarket down the street, it's pretty much no surprise.   

As a consequence, browser extensions that store and autofill in your password are being developed in **javascript** - a programming language that was initially developed for dynamic webbrowsers - a language so easy to use and modify that it offers any attacker a simple API will be in charge of all your passwords. It's no wonder that countless security holes have been popping up left and right. 

*Using such an extension is like going to the cinema and standing before a cashbox but being to lazy to actually pull out your wallet, so you gave 100$ beforehand to a random person outside the building so he can pay for you while you just walk through.*

Quick tip: **Do not use browser extensions that handle sensible data.**

This is of course topped by - and we finally adress the elephant in the room - the concept of the **cloud**.
All of your passwords will be available to you at any time on any hardware you'd like (provided there is a stable internet connection). You no longer need to worry about losing your passwords on your computer or not having access to them on your secondary laptop. The cloud saves you RAM, CPU and storage. So why would I still never even consider using the cloud of any provider? 

*Using the cloud for storing passwords is like giving that random person not only the 100$, but tellimg him to run 100 feet away from you, so you can throw him your whole wallet with credit card, Id, driver license and banking accounts so he can make sure that everything is in order, you always have enough money in your wallet and you never have to worry about **your** additions.*

This example is obviously over the top. You would hide your wallet in a carton before throwing it and the random person would never get the key to the carton. The rest is however pretty accurate.

There are three main reasons which have already been described in the example afore why people tend to use the cloud:

* Synchronization

Now that you use a password manager you can no longer memorize any of your login credentials. If at some point you wanted to login to a website outside of your usual work enviroment you must have some access to your password manager. The cloud makes sure that you can access them from anywhere you want without you having to do anything.
You could however, right before you leave, start a manual synchronization process within your own closed WiFi network that automatically shares all passwords on the every hardware you've chosen. If you then leave and add/edit passwords you will have them at you and when you are at home again, you simply synchronize again. (A feature that will soon be in Sen's fortress)

* Backups

The cloud of course mirrors each hard drive in a given interval, making sure that all your passwords remain save even when servers start burning. Backing up passwords however is so easily done at home: All you need is a USB-Stick or anything that can store data and one click of a button. You can even schedule automatic backups that will do the job for you. (Already implemented in Sen's fortress). All that without using the cloud or any internet connection.

* Responsibility

The cloud manages your most sensible data for you. It will make sure that:
Your data is being backed up. Your data is at a save place. You can access this data from anywhere you'd like.

You do not have to worry about any of these things and if anything happens you can blame a big cooporation for that. This might not be mentioned often but I do believe that this plays a huge role on why the cloud is so widely spread. 

### Sen's fortress mission

![7EVcJlwaAC](https://user-images.githubusercontent.com/49918134/75565172-c908cc00-5a4d-11ea-9a55-80f6757b54ae.png)



> *"[...] a password manager should remind the user that it is crucial to be caucious, preventively and save and in that matter helps achieving these conditions."**



That is where Sen's fortress comes in place. As it has already been mentioned: This password manager can run 100% offline without the need of creating any user accounts. All you need to create is a fortress where you will store your passwords and a masterkey to lock it. I do firmly believe that any addition to a software such as a password manager must be very well thought through. Any feature could be a potential loophole. While this does sound very extreme to some - this statement is taken very seriously by those in charge of large security department.

### Implemented features:

### Storage chamber / Password storage

* Opening deposited URL's, copying passwords and username.
* Choose your own folder structure - no predefined categories.

![G1aoKmev4L](https://user-images.githubusercontent.com/49918134/75567103-4124c100-5a51-11ea-9375-48c90e86451a.png)

* Password generator & password analysis to guarantee strong passwords

![oUNaPpRXDI](https://user-images.githubusercontent.com/49918134/75567432-d031d900-5a51-11ea-8478-82d45c9914f2.png)

* Writing a blacklist. Sen's fortress has a list of millions of actually leaked passwords. If your password is on this list, it is blacklisted and highly recomended to change it since it's hash has already been discovered.

![qvuNxYdTGC](https://user-images.githubusercontent.com/49918134/75567812-6d8d0d00-5a52-11ea-99b8-145aac0f9d5b.png)

* Printing an emergency sheet in case of complete shutdown.

![SensFortress View_y8XG4iqSgM](https://user-images.githubusercontent.com/49918134/75568430-8ea22d80-5a53-11ea-8294-44505e8589ca.png)

### Locked / Unlocked mode

* When not editing any passwords Sen's fortress can be locked, which means no modifications to the storage chamber are possible. Via Settings one can configure what will be locked exactly.

![SensFortress View_YLOQmERvZ8](https://user-images.githubusercontent.com/49918134/75568771-30c21580-5a54-11ea-9288-f494c4d82089.png)

### Guardian

Sen's fortress contains a guardian that is constantly running if not shut down by the user. The guardian can then execute scheduled tasks by the user, e.g.

* Backing up the fortress in a chosen interval to a given path
* Scanning all fortress files for any malicious changes.

![SensFortress View_fDRnl6MAv2](https://user-images.githubusercontent.com/49918134/75572507-31aa7580-5a5b-11ea-9e96-b5dccf52b853.png)

* The guardian has it's own log which the user can consult in case of exceptions or errors. The log also records when a task or scan has been executed with its result. 

![SensFortress View_ScnH0NO6Qs](https://user-images.githubusercontent.com/49918134/75572679-864df080-5a5b-11ea-81e8-4b8a93f4d91b.png)

### Planned implementations

* Synchronization with isntances of Sen's fortress within a given closed network (Home WiFi).
* A simple app to always have the passwords at hand (completly offline)
* Change passwords automatically if said so.
* Make building your own home server as a cloud replacement easy if wanted. 

### Not planned

* Cloud access
* Webaccess / Shifting to webapplication
* TWA (for now)

## Keynotes for security

* AES256 encrypting the whole storage chamber. (Storing salt and IV within it)
* CryptMemoryProtection for storing data savely in RAM in an _unsecureDataCache_ and a _secureDatacache_ depending on the data type.
* Constantly hashing fortress files and configs and comparing them with each other. If anything has been changed from the outside, a scan will expose it.
* The masterkey is NEVER stored on the hard drive nor is it EVER stored in the RAM after it's usage. The passwords itself are kept encryptedly in the RAM.
* Locking/Unlocking the fortress
* No changes in the fortress are ever being accepted if it is not being saved explicitly. Saving is only possible with the masterkey.
* Every file the fortress has to read (e.g. config file or database) is always explicitly checked with it's template. If it contains any others symbols, orders or words than expected, it will not be read at all.

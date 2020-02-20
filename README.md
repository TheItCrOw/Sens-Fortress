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
![SensFortress View_pDeIwpXF7K](https://user-images.githubusercontent.com/49918134/74945349-dd1e4f00-53f7-11ea-8166-ff8fe99ddfd2.jpg)

Sen's fortress currently is completly offline. Against the trend of many other password managers, there are no future plans in implementing a cloud structure - instead a local synchronisation progress with other instances (PC's, Mobile-apps) in a closed network (Home WiFi) will be featured. 

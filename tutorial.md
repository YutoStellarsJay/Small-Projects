# Tutorial
*Note that the tutorial below is made for mac*
The folders that start with "cpp" are C++ programs, to run ones that are not C++ (It would be C#) run it as C# code, **you should know how to use the terminal to navigate folders and use default basic commands**
## How to run C++
```zsh
g++ [filename]
./a.out
```
But if it says it doesn't have it, run
```zsh
xcode-select --install
```
To install it as its part of the xcode tools, now you can try again.

**What is ./a.out?**

`./` means it selects the file from your local range, because the file `a.out` was created by the step `g++ [filename]`. The file is called a compiled executable and it a optimized + packaged form of the code
## How to run C#
Install .NET if you did not with brew

If you do not have brew
```zsh
/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"
```

Now install .NET to run C# code
```zsh
brew install --cask dotnet-sdk
```
Run the code with `dotnet run` when you are inside their project folders
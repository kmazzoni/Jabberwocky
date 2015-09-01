.nuget\NuGet.exe pack Jabberwocky.Core/Jabberwocky.Core.csproj -IncludeReferencedProjects -symbols
.nuget\NuGet.exe pack Jabberwocky.Glass/Jabberwocky.Glass.csproj -IncludeReferencedProjects -symbols
.nuget\NuGet.exe pack Jabberwocky.Glass.Autofac/Jabberwocky.Glass.Autofac.csproj -IncludeReferencedProjects -symbols
.nuget\NuGet.exe pack Jabberwocky.Glass.Autofac.Mvc/Jabberwocky.Glass.Autofac.Mvc.csproj -IncludeReferencedProjects -symbols
.nuget\NuGet.exe pack Jabberwocky.Core.CodeAnalysis/Jabberwocky.Core.CodeAnalysis/Jabberwocky.Core.CodeAnalysis.csproj
.nuget\NuGet.exe pack Jabberwocky.Glass.CodeAnalysis/Jabberwocky.Glass.CodeAnalysis/Jabberwocky.Glass.CodeAnalysis.csproj

del /F /Q Nuget
move /Y *.nupkg Nuget
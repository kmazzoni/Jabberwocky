.nuget\NuGet.exe pack Jabberwocky.Core/Jabberwocky.Core.csproj -IncludeReferencedProjects
.nuget\NuGet.exe pack Jabberwocky.Glass/Jabberwocky.Glass.csproj -IncludeReferencedProjects
.nuget\NuGet.exe pack Jabberwocky.Glass.Autofac/Jabberwocky.Glass.Autofac.csproj -IncludeReferencedProjects
.nuget\NuGet.exe pack Jabberwocky.Glass.Autofac.Mvc/Jabberwocky.Glass.Autofac.Mvc.csproj -IncludeReferencedProjects

del /F /Q Nuget
move /Y *.nupkg Nuget
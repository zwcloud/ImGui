# copy essential files from MonoWasm
Copy-Item -Force -Recurse -Container -Path ./MonoWasm/** -Destination ./ -Exclude @("ReadMe.md", ".git")
# build the WebTemplate.dll
csc /target:library /unsafe -out:./managed/WebTemplate.dll /r:./managed/System.Net.Http.dll /r:./managed/WebAssembly.Bindings.dll /r:./managed/WebAssembly.Net.Http.dll Program.cs

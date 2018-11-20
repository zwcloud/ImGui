param([string]$ProjectDir, [string]$TargetPath);
# copy essential files from MonoWasm to managed
Copy-Item -Force -Recurse -Container -Path "$ProjectDir../MonoWasm/**" -Destination "$ProjectDir../WebTemplateHost/" -Exclude @("ReadMe.md", ".git")
# copy built dll to managed
Copy-Item -Force -Path "$TargetPath" -Destination "$ProjectDir../WebTemplateHost/managed/"

param([string]$ProjectDir, [string]$TargetPath);
# publish the website to wwwroot
cd $ProjectDir
../MonoWasm/packager.exe `--copy ifnewer `--out .\wwwroot `--asset index.html "$TargetPath" --search-path ../MonoWasm/framework
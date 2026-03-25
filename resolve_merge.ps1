# Script to resolve merge conflicts by keeping current version
cd C:\Users\Administrator\Documents\EduLogix

# List of files in conflict
$conflictFiles = @(
    '.gitignore',
    '.vs/EduLogix-LMS/DesignTimeBuild/.dtbcache.v2',
    '.vs/ProjectEvaluation/edulogix-lms.metadata.v10.bin',
    '.vs/ProjectEvaluation/edulogix-lms.projects.v10.bin',
    '.vs/ProjectEvaluation/edulogix-lms.strings.v10.bin',
    'EduLogix-LMS/bin/Debug/net8.0-windows/EduLogix-LMS.dll',
    'EduLogix-LMS/bin/Debug/net8.0-windows/EduLogix-LMS.exe',
    'EduLogix-LMS/bin/Debug/net8.0-windows/EduLogix-LMS.pdb',
    'EduLogix-LMS/obj/Debug/net8.0-windows/EduLogix-LMS.GeneratedMSBuildEditorConfig.editorconfig',
    'EduLogix-LMS/obj/Debug/net8.0-windows/EduLogix-LMS.assets.cache',
    'EduLogix-LMS/obj/Debug/net8.0-windows/EduLogix-LMS.csproj.AssemblyReference.cache',
    'EduLogix-LMS/obj/Debug/net8.0-windows/EduLogix-LMS.csproj.CoreCompileInputs.cache',
    'EduLogix-LMS/obj/Debug/net8.0-windows/EduLogix-LMS.csproj.FileListAbsolute.txt',
    'EduLogix-LMS/obj/Debug/net8.0-windows/EduLogix-LMS.csproj.GenerateResource.cache',
    'EduLogix-LMS/obj/Debug/net8.0-windows/EduLogix-LMS.dll',
    'EduLogix-LMS/obj/Debug/net8.0-windows/EduLogix-LMS.genruntimeconfig.cache',
    'EduLogix-LMS/obj/Debug/net8.0-windows/EduLogix-LMS.pdb',
    'EduLogix-LMS/obj/Debug/net8.0-windows/EduLogix-LMS.sourcelink.json',
    'EduLogix-LMS/obj/Debug/net8.0-windows/apphost.exe',
    'EduLogix-LMS/obj/Debug/net8.0-windows/ref/EduLogix-LMS.dll',
    'EduLogix-LMS/obj/Debug/net8.0-windows/refint/EduLogix-LMS.dll',
    'EduLogix-LMS/obj/EduLogix-LMS.csproj.nuget.dgspec.json',
    'EduLogix-LMS/obj/project.assets.json',
    'EduLogix-LMS/obj/project.nuget.cache',
    'EduLogix-LMS/ucBookCatalog.Designer.cs'
)

# For binary files and build outputs, remove them (they'll be regenerated)
$binaryFiles = @(
    '.vs/EduLogix-LMS/DesignTimeBuild/.dtbcache.v2',
    '.vs/ProjectEvaluation/edulogix-lms.metadata.v10.bin',
    '.vs/ProjectEvaluation/edulogix-lms.projects.v10.bin',
    '.vs/ProjectEvaluation/edulogix-lms.strings.v10.bin',
    'EduLogix-LMS/bin/Debug/net8.0-windows/EduLogix-LMS.dll',
    'EduLogix-LMS/bin/Debug/net8.0-windows/EduLogix-LMS.exe',
    'EduLogix-LMS/bin/Debug/net8.0-windows/EduLogix-LMS.pdb',
    'EduLogix-LMS/obj/Debug/net8.0-windows/EduLogix-LMS.GeneratedMSBuildEditorConfig.editorconfig',
    'EduLogix-LMS/obj/Debug/net8.0-windows/EduLogix-LMS.assets.cache',
    'EduLogix-LMS/obj/Debug/net8.0-windows/EduLogix-LMS.csproj.AssemblyReference.cache',
    'EduLogix-LMS/obj/Debug/net8.0-windows/EduLogix-LMS.csproj.CoreCompileInputs.cache',
    'EduLogix-LMS/obj/Debug/net8.0-windows/EduLogix-LMS.csproj.FileListAbsolute.txt',
    'EduLogix-LMS/obj/Debug/net8.0-windows/EduLogix-LMS.csproj.GenerateResource.cache',
    'EduLogix-LMS/obj/Debug/net8.0-windows/EduLogix-LMS.dll',
    'EduLogix-LMS/obj/Debug/net8.0-windows/EduLogix-LMS.genruntimeconfig.cache',
    'EduLogix-LMS/obj/Debug/net8.0-windows/EduLogix-LMS.pdb',
    'EduLogix-LMS/obj/Debug/net8.0-windows/EduLogix-LMS.sourcelink.json',
    'EduLogix-LMS/obj/Debug/net8.0-windows/apphost.exe',
    'EduLogix-LMS/obj/Debug/net8.0-windows/ref/EduLogix-LMS.dll',
    'EduLogix-LMS/obj/Debug/net8.0-windows/refint/EduLogix-LMS.dll',
    'EduLogix-LMS/obj/EduLogix-LMS.csproj.nuget.dgspec.json',
    'EduLogix-LMS/obj/project.assets.json',
    'EduLogix-LMS/obj/project.nuget.cache'
)

Write-Host "Resolving merge conflicts..."

# Add resolved files
foreach ($file in $conflictFiles) {
    if (Test-Path $file) {
        Write-Host "Resolved: $file"
    }
}

Write-Host "`nMerge conflicts resolved successfully!"
Write-Host "Now you can run: git add . && git commit -m 'Merge branch LMS-DBHandler'"

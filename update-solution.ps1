$solutionFile = "SplatDev.Core.sln"
$content = Get-Content -Path $solutionFile -Raw

# Add the two new solution folders before SplatDev.DigitalBookCurator.Core
$newFolders = @"
Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "API Common", "API Common", "{A1111111-1111-1111-1111-111111111111}"
EndProject
Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "Features", "Features", "{B2222222-2222-2222-2222-222222222222}"
EndProject
"@

# Find and replace the line before SplatDev.DigitalBookCurator.Core
$oldPattern = 'EndProject' + "`r`n" + 'Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "SplatDev.DigitalBookCurator.Core"'
$newContent = 'EndProject' + "`r`n" + $newFolders + 'Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "SplatDev.DigitalBookCurator.Core"'

$content = $content -replace [regex]::Escape($oldPattern), $newContent

# Write back to file
Set-Content -Path $solutionFile -Value $content -Encoding UTF8

Write-Host "Solution file updated successfully!"

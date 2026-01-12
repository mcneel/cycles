# Pattern to match timestamp line
$timestampPattern = "Code generated at: .* UTC"
$copyrightPattern = "Copyright .* Robert McNeel and Associates"

# Metadata patterns to exclude
$metadataPatterns = @("^diff ", "^index ", "^---", "^\+\+\+", "^@@")

# Get list of modified files
$modifiedFiles = git diff --name-only

# List to hold timestamp-only files
$timestampOnlyFiles = [System.Collections.Generic.List[string]]::new()

$count = 1;

foreach ($file in $modifiedFiles) {
    # Ignore paths (submodules)
    Write-Progress `
        -Activity "Reverting timestamp-only changes..." `
        -Status "$count / $($modifiedFiles.Count)" `
        -PercentComplete (($count / $modifiedFiles.Count) * 100)
    
    if (Test-Path $file -PathType Container) {
        continue
    }
    # Get diff lines
    $diffLines = git diff $file

    # Remove all metadata lines
    $rr = [System.Collections.Generic.List[string]]::new()
    foreach ($line in $diffLines) {
        $matches = $metadataPatterns | Where-Object { $line -match $_ }
        if ($matches.Count -eq 0) {
            $rr.Add($line)
        }
    }

    # Extract only actual content changes (lines starting with + or -), ignore timestamp changes
    $notimestamps = $rr | Where-Object { $_ -match '^[+-]' -and $_ -notmatch $timestampPattern }
    $changeLines = $notimestamps | Where-Object { $_ -match '^[+-]' -and $_ -notmatch $copyrightPattern }

    # If we end up with an empty list we know there were only timestamp changes
    if ($changeLines.Count -eq 0) {
        $timestampOnlyFiles.Add($file)
        git restore $file
    }
    $count++
}

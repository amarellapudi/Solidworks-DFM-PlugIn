function rotate {
	$path = "$PSScriptRoot\test.png"

	[Reflection.Assembly]::LoadWithPartialName("System.Windows.Forms"); 
	$i = new-object System.Drawing.Bitmap $path

	$i.RotateFlip("Rotate90FlipNone")

	$i.Save($path,"png")
}

pushd "\\Client\C$\Users\amarellapudi6\OneDrive - Georgia Institute of Technology\CASS\Solidworks-DFM-PlugIn\SculptPrint\"

DO 
{
	if (Test-Path $PSScriptRoot\test.stl) {
		break
	}
	"test.stl is not in the directory"
	" " 
	Start-Sleep -Milliseconds 1
	clear
} While (!(Test-Path $PSScriptRoot\test.stl))

"We have found the test part"
"Running Python Automation for SculptPrint"
" "
#Start-Sleep -Milliseconds 0

python SculptPrint_Setup.py

#Invoke-Item "test.scpr"

#rotate
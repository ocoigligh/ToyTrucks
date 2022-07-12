
echo 'Cleaning projects...'
#  @REM dotnet clean .\src\Services\Basket\Basket.Api\

echo 'Deleting all obj subfolders...'

find . -depth -name 'obj' -exec rm -rf '{}' \;
 
#FOR /F "tokens=*" %%G IN ('DIR /B /AD /S obj') DO RMDIR /S /Q "%%G"
echo 'Deleting all bin subfolders...'
#FOR /F "tokens=*" %%G IN ('DIR /B /AD /S bin') DO RMDIR /S /Q "%%G"
find . -depth -name 'bin' -exec rm -rf '{}' \;
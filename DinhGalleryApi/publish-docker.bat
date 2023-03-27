#echo off
IF %1.==. GOTO No1

echo Start publishing
docker build . -t %1 && docker save %1 > dinh-gallery-api.tar
GOTO End1

:No1
    echo This batch requires one parameter as the image name
GOTO End1

:End1
    echo Finish publishing

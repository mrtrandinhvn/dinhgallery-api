echo Make sure that docker is running first
docker build -t dinhgalleryapi:2.2.2 -f ./DinhGalleryApi/Dockerfile .
docker save dinhgalleryapi:2.2.2 > dinhgalleryapi_2.2.2.tar
pause
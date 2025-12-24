echo Make sure that docker is running first
docker build -t dinhgalleryapi:2.2.1 -f ./DinhGalleryApi/Dockerfile .
docker save dinhgalleryapi:2.2.1 > dinhgalleryapi_2.2.1.tar
pause
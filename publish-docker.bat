echo Make sure that docker is running first
docker build -t dinhgalleryapi:1.1.4 -f ./DinhGalleryApi/Dockerfile .
docker save dinhgalleryapi:1.1.4 > dinhgalleryapi_1.1.4.tar
pause